using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Bright.BehaviorTree
{
    public abstract class AbstractFlowNode : AbstractNode
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public bool IgnoreRestartSelf { get; set; }

        public AbstractComposite Parent { get; internal set; }

        public List<AbstractService> Services { get; private set; }

        public List<AbstractDecorator> Decorators { get; private set; }

#if DEBUG
        public ENodeResult OriginResult { get; protected set; }

        public ENodeResult FinalResult { get; protected set; }
#endif

        /// <summary>
        /// 是否 已经触发 BehaviorTree 的事件并且提交到待执行队列
        /// </summary>
        protected AbstractEventJob EventJob { get; set; }

        protected bool NotSubmitEventJob => EventJob == null || EventJob.Canceled;

        protected AbstractFlowNode(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators) : base(bt, id)
        {

            Services = services;

            if (services != null)
            {
                foreach (AbstractService s in services)
                {
                    s.AttachedNode = this;
                }
            }

            Decorators = decorators;

            if (decorators != null)
            {
                foreach (AbstractDecorator d in decorators)
                {
                    d.AttachedNode = this;
                }
            }
        }


        public bool CanRunTopNode()
        {
            return Decorators == null || Decorators.TrueForAll(d => d.CacheConditionResult);
        }

        private bool CheckNeedRepeat()
        {
            if (Decorators != null)
            {
                foreach (AbstractDecorator d in Decorators)
                {
                    if (d.NeedRepeat())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void FinishByExternal(ENodeResult result)
        {
            Debug.Assert(result != ENodeResult.ABORT);
            Debug.Assert(IsExecuting);
            DoFinishDefer(result);
        }

        private void DoFinishDefer(ENodeResult result)
        {
            if (IsExecuting && NotSubmitEventJob)
            {
                s_logger.Trace("FinishExecutionDefer. node:{type} {id} result:{result}", GetType(), Id, result);
                EventJob = Bt.AddJob(this, () => DoFinishExecution(result));
            }
        }

        private void DoFinishExecution(ENodeResult result, bool notCallbackParent = false)
        {
            s_logger.Trace("FinishExecution. node:{type} {id} result:{result}", GetType(), Id, result);
#if DEBUG
            OriginResult = result;
#endif
            if (result != ENodeResult.ABORT && Decorators != null)
            {
                foreach (AbstractDecorator d in Decorators)
                {
                    d.ProcessResult(ref result);
                }
            }
#if DEBUG
            FinalResult = result;
#endif
            DoNodeDeactivation(result);


            // 疑问.到底是 != Abort 时 可以重复呢，还是 != Succ 时可以重复?
            // ue4的做法是 == Abort 时肯定不重复
            // 但 到底是否重复，还是由上层 Composite 来决定
            if (!notCallbackParent)
            {
                Parent.OnChildFinish(result, result != ENodeResult.ABORT ? CheckNeedRepeat() : false);
            }
        }

        #region node activation

        internal void DoNodeActivation()
        {
#if DEBUG
            OriginResult = ENodeResult.NONE;
            FinalResult = ENodeResult.NONE;
#endif
            Debug.Assert(!IsExecuting);
            Debug.Assert(EventJob == null);
            s_logger.Trace("node DoNodeActivation. node:{name} {id}", GetType(), Id);
            IsExecuting = true;
            if (AutoTick)
            {
                Bt.ScheduleTickJob(this);
            }
            if (Decorators != null)
            {
                foreach (AbstractDecorator d in Decorators)
                {
                    s_logger.Trace("decorator ReceiveExecutionStart. decorator:{id}", d.Id);
                    d.ReceiveExecutionStart();
                }
            }
            if (Services != null)
            {
                foreach (AbstractService s in Services)
                {
                    s_logger.Trace("service ReceiveActivation. service:{id}", s.Id);
                    s.IsExecuting = true;
                    if (AutoTick)
                    {
                        Bt.ScheduleTickJob(s);
                    }
                    s.ReceiveActivation();
                }
            }
            ActivateChildrenDecorators();
            OnNodeActivation();
        }

        internal void DoNodeDeactivation(ENodeResult re)
        {
            Debug.Assert(IsExecuting);
            IsExecuting = false;

            EventJob = null;

            s_logger.Trace("node DoNodeDectivation. node:{name} {id}", GetType(), Id);

            if (Decorators != null)
            {
                foreach (AbstractDecorator d in Decorators)
                {
                    s_logger.Trace("decorator ReceiveExecutionFinish. decorator:{type} {id}", d.GetType(), d.Id);
                    d.ReceiveExecutionFinish(re);
                }
            }
            if (Services != null)
            {
                foreach (AbstractService s in Services)
                {
                    s_logger.Trace("service ReceiveDeactivation. service:{type} {id}", s.GetType(), s.Id);
                    s.ReceiveDeactivation();
                    s.IsExecuting = false;
                }
            }
            DeactivateChildrenDecorators();
            OnNodeDeactivation();
        }

        protected void FinishBySelf(ENodeResult re)
        {
            DoFinishExecution(re);
        }

        /// <summary>
        /// 打断自身. 一般来说. 由外部事件触发节点打断时，调用此节点。
        /// 
        /// </summary>
        public void AbortByExternal()
        {
            Debug.Assert(IsExecuting);
            OnAbort();
            DoFinishDefer(ENodeResult.ABORT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notCallbackParent">结束时不要通知Parent,即不调用Parent.OnChildFinish </param>
        internal void AbortByObserver()
        {
            OnAbort();
            DoFinishExecution(ENodeResult.ABORT, true);
        }

        protected internal abstract void OnAbort();

        protected abstract void ActivateChildrenDecorators();

        protected abstract void DeactivateChildrenDecorators();

        protected abstract void OnNodeActivation();

        protected abstract void OnNodeDeactivation();
        #endregion
    }
}
