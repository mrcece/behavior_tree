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
    public class Test_ForceSuccess
    {
        [TestMethod]
        public void Test_ForceSucc_When_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeForceSuccess(bt, 10);
            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1 });
            var t2 = new Counter(bt, 3, null, new List<AbstractDecorator> { });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);

            bt.Start(0);

            // run t1
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            t1.FinishByExternal(ENodeResult.SUCC);


            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            t2.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }

        [TestMethod]
        public void Test_ForceSucc_When_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeForceSuccess(bt, 10);
            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1 });
            var t2 = new Counter(bt, 3, null, new List<AbstractDecorator> { });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);
            bt.Start(0);

            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            t1.FinishByExternal(ENodeResult.FAIL);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            t2.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }

        [TestMethod]
        public void Test_ForceSucc_When_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new UeForceSuccess(bt, 10);
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.SELF) { Condition = true };

            var t1 = new Counter(bt, 2, null, new List<AbstractDecorator> { d1, d2 });
            var t2 = new Counter(bt, 3, null, new List<AbstractDecorator> { });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);
            bt.Start(0);

            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);

            d2.Condition = false;
            d2.CheckAndNotifyObserverWhenChange();
            bt.Tick(0, 0);

            //Assert.IsTrue(d1.IsExecuting);
            //Assert.IsFalse(t1.IsExecuting);
            //Assert.IsTrue(t2.IsExecuting);
            //t2.FinishExecutionDefer(ENodeResult.SUCC);

            //bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }
    }
}
