using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using Pefect.BehaviorTreeUnitTest.Decorators;
using Pefect.BehaviorTreeUnitTest.Tasks;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;

namespace Pefect.BehaviorTreeUnitTest.Basics
{
    [TestClass]
    public class Test_Decorator_Root
    {

        [TestMethod]
        public void NotExecuting_False_NotExecuting()
        {

            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE);

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

        }

        [TestMethod]
        public void NotExecuting_True_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
        }

        [TestMethod]
        public void NotExecuting_FalseFalse_NotExecuting()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = false };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
        }

        [TestMethod]
        public void NotExecuting_FalseTrue_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = false };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);

            d1.ForceNotifyObserver(true);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
        }



        [TestMethod]
        public void NotExecuting_TrueFalseAbortNone_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(false);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);
        }


        [TestMethod]
        public void NotExecuting_TrueFalseAbortSelf_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(false);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(2, root.Version);
        }



        [TestMethod]
        public void NotExecuting_TrueFalseAbortLowPriority_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(false);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);
        }


        [TestMethod]
        public void NotExecuting_TrueFalseAbortBoth_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(false);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(2, root.Version);
        }

        [TestMethod]
        public void NotExecuting_TrueTrueAbortNone_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(true);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);
        }


        [TestMethod]
        public void NotExecuting_TrueTrueAbortSelf_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(true);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(3, root.Version);
        }



        [TestMethod]
        public void NotExecuting_TrueTrueAbortLowPriority_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(true);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);
        }


        [TestMethod]
        public void NotExecuting_TrueTrueAbortBoth_Executing()
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = true };

            var root = new Selector(bt, 1, null, new List<AbstractDecorator> { d1 }, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(1, root.Version);

            d1.ForceNotifyObserver(true);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(3, root.Version);
        }

        [TestMethod]
        public void ProcessResult_Succ2Abort_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ModifyProcessResult(bt, 10, EFlowAbortMode.NONE, ENodeResult.ABORT);
            var t1 = new ManualTask(bt, 2, null, new List<AbstractDecorator> { d1 });


            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);

        }

        [TestMethod]
        public void ProcessResult_Fail2Succ_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ModifyProcessResult(bt, 10, EFlowAbortMode.NONE, ENodeResult.SUCC);
            var t1 = new ManualTask(bt, 2, null, new List<AbstractDecorator> { d1 });


            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);

            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t1.FinalResult);
            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
        }


        [TestMethod]
        public void ProcessResult_Abort2Succ_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ModifyProcessResult(bt, 10, EFlowAbortMode.NONE, ENodeResult.SUCC);
            var t1 = new ManualTask(bt, 2, null, new List<AbstractDecorator> { d1 });


            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.AbortByExternal();

            bt.Tick(0, 0);

            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }
    }
}
