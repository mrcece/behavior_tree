using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;
using Bright.BehaviorTree.Decorators;
using Bright.BehaviorTreeUnitTest.Tasks;

namespace Pefect.BehaviorTreeUnitTest.Decorators
{
    [TestClass]
    public class Test_Cooldown
    {
        [TestMethod]
        public void TestCooldown()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeCooldown(bt, 10, EFlowAbortMode.SELF, 0.2f);
            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);


            // run t1
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.FinishByExternal(ENodeResult.SUCC);


            bt.Tick(10, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(20, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(190, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(200, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(200, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(210, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(220, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            bt.Tick(420, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
        }
    }
}
