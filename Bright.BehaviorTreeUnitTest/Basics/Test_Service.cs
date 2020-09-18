using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pefect.BehaviorTreeUnitTest.Services;
using Pefect.BehaviorTreeUnitTest.Tasks;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;

namespace Pefect.BehaviorTreeUnitTest
{
    [TestClass]
    public class Test_Service
    {
        [TestMethod]
        public void Test_Activation()
        {
            var bt = new BehaviorTreeObject(1, null);

            var s1 = new TestService(bt, 10);
            var t1 = new ManualTask(bt, 2, null, null);
            var root = new Sequence(bt, 1, new List<AbstractService> { s1 }, null, new List<AbstractFlowNode> { t1 });

            bt.SetRoot(root);


            Assert.IsFalse(s1.IsExecuting);
            Assert.AreEqual(0, s1.State);

            bt.Start(0);

            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, s1.State);

            t1.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);

            Assert.IsFalse(s1.IsExecuting);
            Assert.AreEqual(2, s1.State);
        }

    }
}
