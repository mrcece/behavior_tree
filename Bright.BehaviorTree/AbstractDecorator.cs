using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text;

namespace Bright.BehaviorTree
{
    public enum EFlowAbortMode
    {
        NONE,
        LOWER_PRIORITY,
        SELF,
        BOTH,
    }

    [Flags]
    public enum EDecoratorFlags
    {
        // REVERSE_RESULT = 0x1,  // 是否取反 PerformConditionCheck 结果
        PERFORM_CONDITION_CHECK = 0x2,
        CHECK_REPEAT = 0x4,
        PROCESS_RESULT = 0x8,
    }


    /// <summary>
    /// decorator 负责以下几件事情
    /// 1. 控制 节点是否能执行 (FlowAbortMode == SELF,LOWER_PRIORITY, BOTH), 对应  PerformConditionCheck
    /// 2. 控制 节点是否重复执行, 对应 NeedRepeat()
    /// 3. 控制 节点的执行结果,  对应 ProcessResult(ref ENodeResult result)
    ///
    /// 2和3都只对所在的节点, 1 比较特殊,为BOTH或者LOWER_PRIORITY时，也能影响其他节点执行
    /// </summary>
    public abstract class AbstractDecorator : AbstractAttachNode
    {
        public EFlowAbortMode FlowAbortMode { get; }

        public bool ShouldAbortSelf => FlowAbortMode == EFlowAbortMode.SELF || FlowAbortMode == EFlowAbortMode.BOTH;

        /// <summary>
        /// 指示涉及的装饰器操作. 出于优化目的.
        /// 目前暂未实现
        /// </summary>
        public EDecoratorFlags Flag { get; }

        #region optimization

        // 以下一些定义为了帮助优化. 暂时未用上

        /*

        /// <summary>
        /// 是否需要计算 PerformConditionCheck
        /// </summary>
        public bool NeedReCheckCondition { get; set; }

        /// <summary>
        /// 上一回更新时 ConditionResult的值.
        /// 如果为 false, 则当前 ConditionResult 变为true时，需要从 NotPassDecorators 队列移除(目前没维护这个队列)
        /// </summary>
        public bool LastConditionResult { get; set; }


    */

        public bool CacheConditionResult { get; set; }
        #endregion

        protected AbstractDecorator(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode) : base(bt, id)
        {
            FlowAbortMode = flowAbortMode;
        }

        #region decorator funtions
        [Nop]
        public virtual bool PerformConditionCheck()
        {
            return true;
        }

        [Nop]
        public virtual bool NeedRepeat()
        {
            return false;
        }

        [Nop]
        public virtual void ProcessResult(ref ENodeResult result)
        {

        }
        #endregion


        public void DoObserverActivated()
        {
            Debug.Assert(!IsExecuting);
            IsExecuting = true;
            if (AutoTick)
            {
                Bt.ScheduleTickJob(this);
            }
            CacheConditionResult = PerformConditionCheck();
            ReceiveObserverActivated();
        }

        public void DoObserverDeactivated()
        {
            Debug.Assert(IsExecuting);
            IsExecuting = false;
            ReceiveObserverDeactivated();
        }


        private void NotifyObserver()
        {
            Debug.Assert(IsExecuting);
            if (IsExecuting)
            {
                AttachedNode.Parent.NotifyObserveDecoratorEvent(this);
            }
        }

        public void CheckAndNotifyObserverWhenChange()
        {
            if (!IsExecuting)
            {
                return;
            }
            bool newResult = PerformConditionCheck();
            if (newResult != CacheConditionResult)
            {
                CacheConditionResult = newResult;
                NotifyObserver();
            }
        }

        public void CompareAndNotifyObserverWhenChange(bool newResult)
        {
            if (!IsExecuting)
            {
                return;
            }
            if (newResult != CacheConditionResult)
            {
                CacheConditionResult = newResult;
                NotifyObserver();
            }
        }

        public void ForceNotifyObserver(bool newResult)
        {
            CacheConditionResult = newResult;
            NotifyObserver();
        }

        #region observe & child event

        /// <summary>
        /// 当 Observer 激活时触发
        /// </summary>
        [Nop]
        protected virtual void ReceiveObserverActivated()
        {

        }

        /// <summary>
        /// 当 Observer 失活时触发
        /// </summary>
        [Nop]
        protected virtual void ReceiveObserverDeactivated()
        {

        }

        /// <summary>
        /// 当子节点 开始执行时 触发. 这个事件可以用于 Loop 之类的 Decorator
        /// </summary>
        [Nop]
        public virtual void ReceiveExecutionStart()
        {

        }

        /// <summary>
        /// 当子节点 执行结束时触发, 这个事件可以用于 Cooldown 或者 TimeLimit 之类的Decorator
        /// </summary>
        /// <param name="result"></param>
        [Nop]
        public virtual void ReceiveExecutionFinish(ENodeResult result)
        {

        }
        #endregion

    }
}
