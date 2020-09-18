using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bright.BehaviorTree.Composites
{
    public class Sequence : AbstractComposite
    {
        private int CurrentIndex { get; set; }

        private List<AbstractFlowNode> Children { get; } = new List<AbstractFlowNode>();

        public Sequence(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators, List<AbstractFlowNode> children)
            : base(bt, id, services, decorators)
        {
            foreach (AbstractFlowNode c in children)
            {
                c.Parent = this;
                Children.Add(c);
            }
        }

        public override void OnChildFinish(ENodeResult result, bool repeat)
        {
            switch (result)
            {
                case ENodeResult.ABORT:
                case ENodeResult.FAIL:
                    {
                        FinishBySelf(ENodeResult.FAIL);
                        return;
                    }
                case ENodeResult.SUCC:
                    {
                        if (repeat)
                        {
                            --CurrentIndex;
                        }
                        break;
                    }
            }

            ExecuteNextChild();
        }



        private void ExecuteNextChild()
        {
            for (; ++CurrentIndex < Children.Count;)
            {
                AbstractFlowNode child = Children[CurrentIndex];
                if (child.CanRunTopNode())
                {
                    child.DoNodeActivation();
                    return;
                }
                else
                {
                    FinishBySelf(ENodeResult.FAIL);
                    return;
                }
            }

            FinishBySelf(ENodeResult.SUCC);
        }

        protected override void OnNodeActivation()
        {
            CurrentIndex = -1;

            ExecuteNextChild();
        }

        protected override void OnNodeDeactivation()
        {
            CurrentIndex = -1;
        }

        protected sealed override void ActivateChildrenDecorators()
        {
            foreach (AbstractFlowNode c in Children)
            {
                if (c.Decorators != null)
                {
                    foreach (AbstractDecorator d in c.Decorators)
                    {
                        d.DoObserverActivated();
                    }
                }
            }
        }

        protected sealed override void DeactivateChildrenDecorators()
        {
            foreach (AbstractFlowNode c in Children)
            {
                if (c.Decorators != null)
                {
                    foreach (AbstractDecorator d in c.Decorators)
                    {
                        d.DoObserverDeactivated();
                    }
                }
            }
        }

        protected internal override void ProcessObserveDecoratorsChange()
        {
            // 对于 Sequence， EFlowAbortMode.LOW_PRIORITY没有意义
            Debug.Assert(CurrentIndex >= 0 && CurrentIndex < Children.Count);
            AbstractFlowNode c = Children[CurrentIndex];

            if (ObserveNotifiedDecorators.Any(d => d.ShouldAbortSelf && d.AttachedNode == c))
            {
                c.AbortByObserver();
            }
            ObserveNotifiedDecorators.Clear();

            if (!c.IsExecuting)
            {
                if (c.CanRunTopNode())
                {
                    c.DoNodeActivation();
                }
            }

            // 如果被打断, 并且当前节点无法再执行
            // Sequence 失败
            if (!c.IsExecuting)
            {
                FinishBySelf(ENodeResult.FAIL);
                return;
            }
        }

        protected internal override void OnAbort()
        {
            Debug.Assert(CurrentIndex >= 0 && CurrentIndex < Children.Count);

            AbstractFlowNode c = Children[CurrentIndex];
            // 被打断时, 不需要通知parent
            c.AbortByObserver();
        }

    }
}
