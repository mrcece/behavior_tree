using System;
using System.Collections.Generic;
using System.Text;
using Bright.BehaviorTree;

namespace Pefect.BehaviorTreeUnitTest.Decorators
{
    class ModifyProcessResult : AbstractDecorator
    {
        public ModifyProcessResult(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode, ENodeResult result) : base(bt, id, flowAbortMode)
        {
            Result = result;
        }


        public ENodeResult Result { get; }


        public override bool PerformConditionCheck()
        {
            return true;
        }

        public override void ProcessResult(ref ENodeResult result)
        {
            result = this.Result;
        }
    }
}
