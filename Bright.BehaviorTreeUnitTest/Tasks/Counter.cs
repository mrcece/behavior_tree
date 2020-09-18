using System;
using System.Collections.Generic;
using System.Text;
using Bright.BehaviorTree;

namespace Bright.BehaviorTreeUnitTest.Tasks
{
    class Counter : AbstractTask
    {
        public Counter(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators)
            : base(bt, id, services, decorators)
        {
        }

        public int TotalCount { get; set; }

        public bool IsAbort { get; set; }

        protected override void OnAbort()
        {
            IsAbort = true;
        }

        protected override void OnNodeActivation()
        {
            ++TotalCount;
            IsAbort = false;
        }

        protected override void OnNodeDeactivation()
        {

        }
    }
}
