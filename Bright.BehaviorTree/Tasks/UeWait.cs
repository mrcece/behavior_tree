using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Tasks
{
    public class UeWait : AbstractTask
    {
        private readonly long _delayMills;
        private readonly int _randomDeviation;

        public UeWait(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators, float delayTime, float randomDeviation = 0f)
            : base(bt, id, services, decorators)
        {
            _delayMills = (long)(delayTime * 1000);
            _randomDeviation = (int)(randomDeviation * 1000);
        }

        protected override void OnNodeActivation()
        {
            Bt.ScheduleJob(this, () => FinishByExternal(ENodeResult.SUCC), GenDelayMills(), 0);
        }

        private long GenDelayMills()
        {
            return _randomDeviation <= 0 ? _delayMills : _delayMills + Bright.Common.ThreadLocalRandomUtil.Next(_randomDeviation);
        }

        [Nop]
        protected override void OnNodeDeactivation()
        {

        }

        [Nop]
        protected internal override void OnAbort()
        {

        }
    }
}
