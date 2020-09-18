using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pefect.BehaviorTreeUnitTest.Decorators;
using Pefect.BehaviorTreeUnitTest.Tasks;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;
using Bright.BehaviorTreeUnitTest.Tasks;

namespace Pefect.BehaviorTreeUnitTest
{
    [TestClass]
    public class Test_Decorator_Sequence
    {
        [TestMethod]
        public void ThreeTask_Task1FalseTask2True_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.SELF) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);
            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, d2.Version);
            Assert.AreEqual(0, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_Task1TrueTask2True_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.SELF) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            t1.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(1, t2.Version);
            Assert.AreEqual(0, t3.Version);

            t2.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsTrue(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(2, t2.Version);
            Assert.AreEqual(1, t3.Version);


            t3.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);

            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.SUCC, seq.FinalResult);
            Assert.AreEqual(ENodeResult.SUCC, root.FinalResult);
            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(2, t2.Version);
            Assert.AreEqual(2, t3.Version);
        }


        [TestMethod]
        public void ThreeTask_Task1TrueTask2False_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.SELF) { Condition = false };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            t1.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);

            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);

            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.FAIL, seq.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_Task1TrueTask2Fail_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.SELF) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            t1.FinishByExternal(ENodeResult.SUCC);
            bt.Tick(0, 0);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(1, t2.Version);
            Assert.AreEqual(0, t3.Version);

            t2.FinishByExternal(ENodeResult.FAIL);

            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);

            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.FAIL, seq.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, d2.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(2, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ThreeTask_SelfExecutingAbortNone_Succ(bool enable)
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = enable;
            d1.ForceNotifyObserver(enable);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortSelfTrue_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = true;
            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(3, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortSelfFalse_False()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = false;
            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);

            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, seq.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortLowPriorityTrue_StillRunning()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = true;
            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortLowPriorityFalse_StillRunning()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortBothTrue_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = true;
            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(3, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);
        }

        [TestMethod]
        public void ThreeTask_SelfExecutingAbortBothFalse_False()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 21, null, null);
            var t3 = new ManualTask(bt, 30, null, null);

            var seq = new Sequence(bt, 2, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(1, d1.Version);
            Assert.AreEqual(1, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            d1.Condition = false;
            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);

            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            Assert.AreEqual(2, d1.Version);
            Assert.AreEqual(2, t1.Version);
            Assert.AreEqual(0, t2.Version);
            Assert.AreEqual(0, t3.Version);

            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, seq.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.FinalResult);
        }


        [TestMethod]
        public void ProcessResult_Succ2Abort_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ModifyProcessResult(bt, 10, EFlowAbortMode.NONE, ENodeResult.ABORT);
            var t1 = new ManualTask(bt, 2, null, new List<AbstractDecorator> { d1 });


            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);

            Assert.AreEqual(ENodeResult.SUCC, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
        }

        [TestMethod]
        public void ProcessResult_Fail2Succ_Abort()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ModifyProcessResult(bt, 10, EFlowAbortMode.NONE, ENodeResult.SUCC);
            var t1 = new ManualTask(bt, 2, null, new List<AbstractDecorator> { d1 });


            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
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


            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { t1 });
            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(root.IsExecuting);
            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);

            t1.AbortByExternal();

            bt.Tick(0, 0);

            Assert.AreEqual(ENodeResult.ABORT, t1.OriginResult);
            Assert.AreEqual(ENodeResult.ABORT, t1.FinalResult);
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
        }
    }

}
