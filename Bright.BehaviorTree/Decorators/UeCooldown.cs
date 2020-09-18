using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Decorators
{
    /// <summary>
    /// ue4 内置的 Cooldown decorator 的 Observer aborts 只有 NONE 值. 非常奇怪
    /// </summary>
    public class UeCooldown : AbstractDecorator
    {
        private readonly long _cooldownTime;
        private long _expireTime;

        public UeCooldown(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode, float cooldownTime) : base(bt, id, flowAbortMode)
        {
            _cooldownTime = (long)(cooldownTime * 1000);
        }

        public override bool PerformConditionCheck()
        {
            return Bt.NowMills >= _expireTime;
        }

        public override void ReceiveExecutionFinish(ENodeResult result)
        {
            _expireTime = Bt.NowMills + _cooldownTime;
        }

        protected override void ReceiveObserverActivated()
        {
            if (_expireTime > Bt.NowMills)
            {
                Bt.ScheduleJob(this, () => CompareAndNotifyObserverWhenChange(true), _expireTime - Bt.NowMills, 0);
            }
        }

        protected override void ReceiveObserverDeactivated()
        {

        }
    }
}
