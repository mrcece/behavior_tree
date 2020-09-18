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
    public class Test_TimeLimit
    {
        [TestMethod]
        public void Test_NotExceedLimit()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeTimeLimit(bt, 10, EFlowAbortMode.SELF, 1f);
            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1 });
            var t2 = new Counter(bt, 3, null, new List<AbstractDecorator> { });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);
            bt.Start(0);


            // run t1, loop 1
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            t1.FinishByExternal(ENodeResult.SUCC);

            // run t2
            bt.Tick(30, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }

        [TestMethod]
        public void Test_ExceedLimit()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeTimeLimit(bt, 10, EFlowAbortMode.SELF, 1f);
            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1 });
            var t2 = new Counter(bt, 3, null, new List<AbstractDecorator> { });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);
            bt.Start(0);


            // run t1, loop 1
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);

            // run t1, loop 2
            bt.Tick(999, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);

            // run t1, loop 3
            bt.Tick(1000, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }
    }
}
