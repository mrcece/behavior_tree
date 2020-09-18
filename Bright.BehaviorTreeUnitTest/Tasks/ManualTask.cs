using System;
using System.Collections.Generic;
using System.Text;
using Bright.BehaviorTree;

namespace Pefect.BehaviorTreeUnitTest.Tasks
{
    class ManualTask : Bright.BehaviorTree.AbstractTask
    {
        public ManualTask(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators)
            : base(bt, id, services, decorators)
        {
        }

        protected override void OnAbort()
        {

        }

        protected override void OnNodeActivation()
        {

        }

        protected override void OnNodeDeactivation()
        {

        }
    }
}
