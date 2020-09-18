using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pefect.BehaviorTreeUnitTest.Decorators;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;
using Bright.BehaviorTree.Decorators;
using Bright.BehaviorTree.Tasks;
using Bright.BehaviorTreeUnitTest.Tasks;

namespace Pefect.BehaviorTreeUnitTest.Tasks
{
    [TestClass]
    public class Test_Wait
    {
        [TestMethod]
        public void Test_Wait_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new UeWait(bt, 2, null, new List<AbstractDecorator> { }, 1f);

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);
            bt.Start(0);


            // run t1, 
            bt.Tick(0, 0);

            Assert.IsTrue(t1.IsExecuting);

            // run t1
            bt.Tick(999, 0);
            Assert.IsTrue(t1.IsExecuting);

            // finish t2
            bt.Tick(1000, 0);
            Assert.IsFalse(t1.IsExecuting);
        }

        [TestMethod]
        public void Test_Wait_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };

            var t1 = new UeWait(bt, 2, null, new List<AbstractDecorator> { d1 }, 1f);

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);
            bt.Start(0);


            // run t1, 
            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            d1.CompareAndNotifyObserverWhenChange(false);
            // run t1
            bt.Tick(999, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(d1.IsExecuting);

        }
    }
}
