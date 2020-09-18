using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pefect.BehaviorTreeUnitTest.Decorators;
using Pefect.BehaviorTreeUnitTest.Tasks;
using Bright.BehaviorTree;
using Bright.BehaviorTree.Composites;
using Bright.BehaviorTreeUnitTest.Tasks;

namespace Pefect.BehaviorTreeUnitTest
{
    [TestClass]
    public class Test_Decorator_Selector
    {
        [TestMethod]
        public void TwoTask_FalseFalse_Fail()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE);
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.NONE);
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });

            var seq = new Selector(bt, 30, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);
            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.FAIL, seq.FinalResult);
        }

        [TestMethod]
        public void TwoTask_TrueTrue_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.NONE) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });

            var seq = new Selector(bt, 30, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsTrue(seq.IsExecuting);

            t1.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);


            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.SUCC, seq.FinalResult);
        }

        [TestMethod]
        public void TwoTask_FalseTrue_Succ()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var d2 = new ManualEnable(bt, 20, EFlowAbortMode.NONE) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });

            var seq = new Selector(bt, 30, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Sequence(bt, 1, null, null, new List<AbstractFlowNode> { seq });

            bt.SetRoot(root);

            bt.Start(0);

            Assert.IsTrue(d1.IsExecuting);
            Assert.IsTrue(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.IsTrue(seq.IsExecuting);
            Assert.AreEqual(ENodeResult.NONE, t1.FinalResult);

            t2.FinishByExternal(ENodeResult.SUCC);

            bt.Tick(0, 0);
            Assert.IsFalse(d1.IsExecuting);
            Assert.IsFalse(d2.IsExecuting);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(seq.IsExecuting);


            Assert.AreEqual(2, seq.Version);
            Assert.AreEqual(ENodeResult.SUCC, seq.FinalResult);
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
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
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
            Assert.AreEqual(ENodeResult.FAIL, root.OriginResult);
        }

        #region before

        [TestMethod]
        public void AbortMode_BeforeExecutingAbortNoneConditionTrue_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }

        [TestMethod]
        public void AbortMode_BeforeExecutingAbortNoneConditionFalse_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }



        [TestMethod]
        public void AbortMode_BeforeExecutingAbortSelfConditionTrue_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }

        [TestMethod]
        public void AbortMode_BeforeExecutingAbortSelfConditionFalse_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
        }



        [TestMethod]
        public void AbortMode_BeforeExecutingAbortLowPriorityConditionTrue_RunBefore()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);

            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }

        [TestMethod]
        public void AbortMode_BeforeExecutingAbortLowPriorityConditionFalse_Restart()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(1, t2.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(3, t2.Version);
        }



        [TestMethod]
        public void AbortMode_BeforeExecutingAbortBothConditionTrue_RunBefore()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);

            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }

        [TestMethod]
        public void AbortMode_BeforeExecutingAbortBothConditionFalse_Restart()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = false };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(1, t2.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(3, t2.Version);
        }

        #endregion

        #region Current
        [TestMethod]
        public void AbortMode_CurrentExecutingAbortNoneConditionTrue_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
        }

        [TestMethod]
        public void AbortMode_CurrentExecutingAbortNoneConditionFalse_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.NONE) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);

            d1.Condition = false;
            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);
        }



        [TestMethod]
        public void AbortMode_CurrentExecutingAbortSelfConditionTrue_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(3, t1.Version);
        }

        [TestMethod]
        public void AbortMode_CurrentExecutingAbortSelfConditionFalse_NotChange()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.SELF) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(1, t2.Version);
        }



        [TestMethod]
        public void AbortMode_CurrentExecutingAbortLowPriorityConditionTrue_RunBefore()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);
        }

        [TestMethod]
        public void AbortMode_CurrentExecutingAbortLowPriorityConditionFalse_Restart()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.LOWER_PRIORITY) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);
        }



        [TestMethod]
        public void AbortMode_CurrentExecutingAbortBothConditionTrue_RunBefore()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(true);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(3, t1.Version);
        }

        [TestMethod]
        public void AbortMode_CurrentExecutingAbortBothConditionFalse_Restart()
        {
            var bt = new BehaviorTreeObject(1, null);

            var d1 = new ManualEnable(bt, 10, EFlowAbortMode.BOTH) { Condition = true };
            var t1 = new ManualTask(bt, 11, null, new List<AbstractDecorator> { d1 });
            var t2 = new ManualTask(bt, 20, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.AreEqual(1, t1.Version);

            d1.ForceNotifyObserver(false);
            bt.Tick(0, 0);
            Assert.IsFalse(t1.IsExecuting);
            Assert.IsTrue(t2.IsExecuting);
            Assert.AreEqual(1, t2.Version);
        }
        #endregion

        #region After
        [TestMethod]
        [DataRow(EFlowAbortMode.NONE, true)]
        [DataRow(EFlowAbortMode.NONE, false)]
        [DataRow(EFlowAbortMode.SELF, true)]
        [DataRow(EFlowAbortMode.SELF, false)]
        [DataRow(EFlowAbortMode.LOWER_PRIORITY, true)]
        [DataRow(EFlowAbortMode.LOWER_PRIORITY, false)]
        [DataRow(EFlowAbortMode.BOTH, true)]
        [DataRow(EFlowAbortMode.BOTH, false)]
        public void AbortMode_AfterExecutingAbortXXXConditionXXX_NotChange(EFlowAbortMode abortMode, bool condition)
        {
            var bt = new BehaviorTreeObject(1, null);

            var t1 = new ManualTask(bt, 10, null, null);
            var d2 = new ManualEnable(bt, 20, abortMode) { Condition = true };
            var t2 = new ManualTask(bt, 21, null, new List<AbstractDecorator> { d2 });
            var t3 = new ManualTask(bt, 30, null, null);

            var sel = new Selector(bt, 40, null, null, new List<AbstractFlowNode> { t1, t2, t3 });

            var root = new Selector(bt, 1, null, null, new List<AbstractFlowNode> { sel });
            bt.SetRoot(root);

            bt.Start(0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);

            d2.ForceNotifyObserver(condition);
            bt.Tick(0, 0);
            Assert.IsTrue(t1.IsExecuting);
            Assert.IsFalse(t2.IsExecuting);
            Assert.IsFalse(t3.IsExecuting);
        }


        #endregion
    }
}
