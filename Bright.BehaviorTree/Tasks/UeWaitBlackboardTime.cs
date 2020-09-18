using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Tasks
{
    public class UeWaitBlackboardTime : AbstractTask
    {
        private readonly int _blackboardKeyIndex;

        public UeWaitBlackboardTime(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators, string blackboardKey)
            : base(bt, id, services, decorators)
        {
            _blackboardKeyIndex = bt.BlackboardObject.GetKeyIndexByName(blackboardKey);
        }

        protected override void OnNodeActivation()
        {
            Bt.ScheduleJob(this, () => FinishByExternal(ENodeResult.SUCC), (long)(Bt.BlackboardObject.GetFloatValue(_blackboardKeyIndex) * 1000), 0);
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
