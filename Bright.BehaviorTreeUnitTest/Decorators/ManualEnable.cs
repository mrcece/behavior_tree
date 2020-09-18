using System;
using System.Collections.Generic;
using System.Text;
using Bright.BehaviorTree;

namespace Pefect.BehaviorTreeUnitTest.Decorators
{
    class ManualEnable : AbstractDecorator
    {
        public ManualEnable(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode) : base(bt, id, flowAbortMode)
        {
        }

        public bool Condition { get; set; }

        public override bool PerformConditionCheck()
        {
            return Condition;
        }
    }
}
