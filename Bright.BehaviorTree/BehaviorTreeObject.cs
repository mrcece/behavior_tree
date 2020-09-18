using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Bright.BehaviorTree.Blackboard;
using Bright.BehaviorTree.Composites;
using Bright.Collections;

namespace Bright.BehaviorTree
{

    public abstract class AbstractNodeJob
    {
        public AbstractNode Node { get; }

        public int Version { get; }

        public bool Avaliable => Version == Node.Version;

        public bool Canceled => Version != Node.Version;

        protected AbstractNodeJob(AbstractNode node)
        {
            Node = node;
            Version = node.Version;
        }
    }


    public class BehaviorTreeObject
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        class TimerJob : AbstractNodeJob
        {
            public long NextExecuteMillsTime { get; set; }

            public long IntervalMills { get; }

            public Action Action { get; }

            public TimerJob(AbstractNode node, Action action, long firstExecuteTime, long interval) : base(node)
            {
                Action = action;
                IntervalMills = interval;
                NextExecuteMillsTime = firstExecuteTime;
            }
        }

        class TickJob : AbstractNodeJob
        {

            public TickJob(AbstractNode node) : base(node)
            {
            }
        }

        class ActionEventJob : AbstractEventJob
        {
            private readonly Action _action;

            public ActionEventJob(AbstractNode node, Action action) : base(node)
            {
                _action = action;
            }

            public override void Execute()
            {
                _action();
            }
        }

        public long Id { get; }

        private Root _root;

        private List<AbstractEventJob> _newJobs = new List<AbstractEventJob>();

        private List<AbstractEventJob> _curJobs = new List<AbstractEventJob>();

        private readonly List<TimerJob> _scheduleJobs = new List<TimerJob>();
        private readonly List<TimerJob> _pendingRemoveScheduleJobs = new List<TimerJob>();

        private readonly List<TickJob> _tickJobs = new List<TickJob>();
        private readonly List<TickJob> _pendingRemoveTickJobs = new List<TickJob>();

        public BlackboardObject BlackboardObject { get; }

        public long NowMills { get; private set; }

        internal Root Root => _root;

        public BehaviorTreeObject(long id, BlackboardObject blackboardObject)
        {
            Id = id;
            BlackboardObject = blackboardObject;
        }

        public void SetRoot(AbstractComposite root)
        {
            _root = new Root(this, root);
        }

        public void Start(long nowMills)
        {
            NowMills = nowMills;
            _curJobs.Clear();
            _newJobs.Clear();
            _root.DoNodeActivation();
        }

        public void Close()
        {
            _root.AbortByObserver();
        }

        public void ScheduleTickJob(AbstractNode node)
        {
            Debug.Assert(_tickJobs.All(job => job.Node != node || job.Canceled));
            _tickJobs.Add(new TickJob(node));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        /// <param name="delayMills"></param>
        /// <param name="intervalMills"> 小于等于0时表示这是单次定时任务</param>
        public void ScheduleJob(AbstractNode node, Action action, long delayMills, long intervalMills)
        {
            _scheduleJobs.Add(new TimerJob(node, action, NowMills + delayMills, intervalMills));
        }


        public void AddJob(AbstractEventJob job)
        {
            _newJobs.Add(job);
        }

        public AbstractEventJob AddJob(AbstractNode node, Action job)
        {
            var eventJob = new ActionEventJob(node, job);
            _newJobs.Add(eventJob);
            return eventJob;
        }

        //internal AbstractEventJob SubmitChange(AbstractDecorator decorator)
        //{
        //    s_logger.Debug("submit decorator change. decorator:{id}", decorator.Id);
        //    var job = new DecoratorChange(decorator);
        //    AddJob(job);
        //    return job;
        //}

        //internal AbstractEventJob SubmitChildFinishExecution(AbstractFlowNode node, ENodeResult result)
        //{
        //    s_logger.Debug("submit finish execution. node:{id}", node.Id);
        //    var job = new ChildNodeFinish(node, result);
        //    AddJob(job);
        //    return job;
        //}

        //public void FinishRootExecution()
        //{
        //    _activateRoot = false;
        //    _newJobs.Clear();
        //    s_logger.Trace("finish root execution");
        //}

        public void Tick(long nowMills, float deltaTime)
        {
            NowMills = nowMills;
            s_logger.Trace("tick. now:{now} delta:{delta}", nowMills, deltaTime);

            // s_logger.Trace("============ tick =============");

            if (_root.NeedRestart)
            {
                _root.Restart();
            }

            // 普通的 定时任务
            if (_tickJobs.Count > 0)
            {
                // TODO 应该像 _jobs 那样有 _cur和 _new 两个队列
                // 否则遍历过程中添加新job会有问题
                foreach (TickJob job in _tickJobs)
                {
                    if (!job.Canceled)
                    {
                        // TODO 这儿直接Tick有微妙的次序问题
                        // 必须与 _curJobs队列中的 job 按prioprity排序后执行
                        job.Node.Tick(deltaTime);
                    }
                    else
                    {
                        _pendingRemoveTickJobs.Add(job);
                    }
                }
                if (_pendingRemoveTickJobs.Count > 0)
                {
                    _tickJobs.RemoveAll(_pendingRemoveTickJobs);
                    _pendingRemoveTickJobs.Clear();
                }
            }

            // 检查定时器任务
            // 临时性这么写，将来再优化.
            if (_scheduleJobs.Count > 0)
            {
                // TODO 应该像 _jobs 那样有 _cur和 _new 两个队列
                // 否则遍历过程中添加新job会有问题
                foreach (TimerJob job in _scheduleJobs)
                {
                    if (job.Avaliable)
                    {
                        if (job.NextExecuteMillsTime <= nowMills)
                        {
                            job.Action();
                            if (job.IntervalMills > 0)
                            {
                                job.NextExecuteMillsTime = nowMills + job.IntervalMills;
                            }
                            else
                            {
                                // once timer
                                _pendingRemoveScheduleJobs.Add(job);
                            }
                        }
                    }
                    else
                    {
                        _pendingRemoveScheduleJobs.Add(job);
                    }
                }
                if (_pendingRemoveScheduleJobs.Count > 0)
                {
                    _scheduleJobs.RemoveAll(_pendingRemoveScheduleJobs);
                    _pendingRemoveScheduleJobs.Clear();
                }
            }

            while (_newJobs.Count > 0)
            {
                Bright.Common.ValueUtil.Swap(ref _curJobs, ref _newJobs);

                foreach (AbstractEventJob job in _curJobs)
                {
                    s_logger.Trace(" run job. job:{job}", job);
                    if (!job.Canceled)
                    {
                        job.Execute();
                    }
                }
                _curJobs.Clear();
            }
        }
    }
}
