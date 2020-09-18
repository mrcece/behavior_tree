using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Decorators
{
    public class UeTimeLimit : AbstractDecorator
    {
        private readonly long _limitMills;
        public UeTimeLimit(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode, float limit) : base(bt, id, flowAbortMode)
        {
            _limitMills = (long)(limit * 1000);
        }

        public override void ReceiveExecutionStart()
        {
            Bt.ScheduleJob(this, () => CompareAndNotifyObserverWhenChange(false), _limitMills, 0);
        }

        public override void ReceiveExecutionFinish(ENodeResult result)
        {
            ++Version;
        }
    }
}
