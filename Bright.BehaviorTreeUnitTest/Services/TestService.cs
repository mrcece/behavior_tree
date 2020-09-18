using System;
using System.Collections.Generic;
using System.Text;
using Bright.BehaviorTree;

namespace Pefect.BehaviorTreeUnitTest.Services
{
    class TestService : AbstractService
    {
        public TestService(BehaviorTreeObject bt, int id) : base(bt, id)
        {
        }

        public int State { get; set; }

        public override void ReceiveActivation()
        {
            State = 1;
        }

        public override void ReceiveDeactivation()
        {
            State = 2;
        }
    }
}
