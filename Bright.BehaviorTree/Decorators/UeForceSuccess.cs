using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Bright.BehaviorTree.Decorators
{
    public class UeForceSuccess : AbstractDecorator
    {
        public UeForceSuccess(BehaviorTreeObject bt, int id) : base(bt, id, EFlowAbortMode.NONE)
        {
        }

        public override void ProcessResult(ref ENodeResult result)
        {
            Debug.Assert(result != ENodeResult.ABORT);
            result = ENodeResult.SUCC;
        }
    }
}
