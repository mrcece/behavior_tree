using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pefect.BehaviorTreeUnitTest.Services;
using Pefect.BehaviorTreeUnitTest.Tasks;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;
using Bright.BehaviorTreeUnitTest.Tasks;

namespace Pefect.BehaviorTreeUnitTest
{
    [TestClass]
    public class Test_Selector
    {
        [TestMethod]
        public void Test_NotChild_Fail()
        {

            var bt = new BehaviorTreeObject(1, null);


            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { });
            bt.SetRoot(root);

            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(0, root.Version);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            bt.Start(0);
            Assert.IsFalse(root.IsExecuting);
            Assert.AreEqual(2, root.Version);
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }

        [TestMethod]
        public void Test_OneTaskSucc_Succ()
        {

            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t1.FinalResult);


            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);
        }

        [TestMethod]
        public void Test_OneTaskFail_Fail()
        {

            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            t1.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t1.FinalResult);


            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }

        [TestMethod]
        public void Test_OneTaskAbort_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);
            var t1 = new ManualTask(bt, 2, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            t1.AbortByExternal();

            bt.Tick(0, 0);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);


            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }

        [TestMethod]
        public void TwoTask_Succ_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);
            var t1 = new ManualTask(bt, 2, null, null);
            var t2 = new ManualTask(bt, 3, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t1.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t1.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t1.FinalResult);

            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);
        }

        [TestMethod]
        public void TwoTask_FailSucc_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);
            var t1 = new ManualTask(bt, 2, null, null);
            var t2 = new ManualTask(bt, 3, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t1.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t1.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);

            t1.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t1.FinalResult);

            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);


            t2.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t1.FinalResult);

            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, t2.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t2.FinalResult);

        }

        [TestMethod]
        public void TwoTask_AbortSucc_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);
            var t1 = new ManualTask(bt, 2, null, null);
            var t2 = new ManualTask(bt, 3, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t1.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t1.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);

            t1.AbortByExternal();

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);

            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);


            t2.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);

            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.SUCC, t2.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t2.FinalResult);
        }

        [TestMethod]
        public void TwoTask_FailFail_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);
            var t1 = new ManualTask(bt, 2, null, null);
            var t2 = new ManualTask(bt, 3, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1, t2 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t1.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t1.FinalResult);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);

            t1.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);

            Assert.IsTrue(root.IsExecuting);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t1.FinalResult);

            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, t2.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, t2.FinalResult);


            t2.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);

            Assert.IsFalse(root.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);

            Assert.IsFalse(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, t1.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t1.FinalResult);

            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(ENodeResult.FAIL, t2.OriginResult);
            Assert.AreEqual(ENodeResult.FAIL, t2.FinalResult);
        }



        [TestMethod]
        public void Test_OneTaskTwoRoundSucc_Succ()
        {

            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            // round 1
            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t1.FinalResult);


            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);

            // round 2
            bt.Tick(1, 1);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(2, 2);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, t1.FinalResult);


            Assert.AreEqual(ENodeResult.SUCC, root.OriginResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);
        }


        [TestMethod]
        public void Test_OneTaskRunningAbortSelf_Fail()
        {

            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 2, null, null);

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, root.OriginResult);
            Assert.AreEqual(ENodeResult.NONE, root.FinalResult);

            root.AbortByExternal();

            bt.Tick(0, 0);

            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(root.IsExecuting);

            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);


            Assert.AreEqual(ENodeResult.ABORT, root.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, root.FinalResult);
        }
    }
}
