using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bright.BehaviorTree.Composites
{
    class Root : AbstractComposite
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly AbstractComposite _topNode;

        public Root(BehaviorTreeObject bt, AbstractComposite topNode)
            : base(bt, 0, null, null)
        {
            _topNode = topNode;
            topNode.Parent = this;
        }

        public AbstractComposite TopNode => _topNode;

        public bool NeedRestart { get; private set; }

        public override void OnChildFinish(ENodeResult result, bool repeat)
        {
            // DoNodeDeactivation(result);
            //if (!repeat)
            //{
            //    Bt.FinishRootExecution();
            //}
            //else
            //{
            //    _topNode.DoNodeActivation();
            //}
            // 防止为 Top Node 不成功时 死循环执行
            //if (!_inTopNodeActivation)
            //{
            //    _inTopNodeActivation = true;
            //    _topNode.DoNodeActivation();
            //    _inTopNodeActivation = false;
            //}
            // 在下一帧再执行 行为树
            // 避免无任何节点可以执行时无限循环
            NeedRestart = true;
        }

        public void Restart()
        {
            s_logger.Trace("root restart");
            Debug.Assert(NeedRestart);
            NeedRestart = false;
            if (!_topNode.IsExecuting && _topNode.CanRunTopNode())
            {
                _topNode.DoNodeActivation();
            }
        }

        protected sealed override void ActivateChildrenDecorators()
        {
            s_logger.Trace("root activate decorators");
            if (_topNode.Decorators != null)
            {
                foreach (AbstractDecorator d in _topNode.Decorators)
                {
                    d.DoObserverActivated();
                }
            }
        }

        protected sealed override void DeactivateChildrenDecorators()
        {
            s_logger.Trace("root deactivate decorators");
            if (_topNode.Decorators != null)
            {
                foreach (AbstractDecorator d in _topNode.Decorators)
                {
                    d.DoObserverDeactivated();
                }
            }
        }

        protected override void OnNodeActivation()
        {
            s_logger.Trace("=========== bt start. id:{id} =============", Bt.Id);
            // _topNode.DoNodeActivation();

            if (_topNode.CanRunTopNode())
            {
                _topNode.DoNodeActivation();
            }
        }

        protected override void OnNodeDeactivation()
        {
            s_logger.Trace("=========== bt end. id:{id} =========== ", Bt.Id);
        }

        protected internal override void OnAbort()
        {
            s_logger.Trace("on abort");
            if (_topNode.IsExecuting)
            {
                _topNode.AbortByObserver();
            }
        }

        protected internal override void ProcessObserveDecoratorsChange()
        {
            s_logger.Trace("process observed decorators change");
            // 对于 Root，LOW_PRIORITY没有意义
            var topNodeDecorators = _topNode.Decorators;
            Debug.Assert(topNodeDecorators != null);


            if (_topNode.IsExecuting)
            {
                if (ObserveNotifiedDecorators.Any(d => d.ShouldAbortSelf))
                {
                    _topNode.AbortByObserver();
                }
            }
            ObserveNotifiedDecorators.Clear();

            if (!_topNode.IsExecuting && _topNode.CanRunTopNode())
            {
                _topNode.DoNodeActivation();
            }
        }
    }
}
