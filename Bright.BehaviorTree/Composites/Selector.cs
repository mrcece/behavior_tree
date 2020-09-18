using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using NLog.LayoutRenderers;

namespace Bright.BehaviorTree.Composites
{

    public class Selector : AbstractComposite
    {
        private int CurrentIndex { get; set; }

        private List<AbstractFlowNode> Children { get; } = new List<AbstractFlowNode>();

        public Selector(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators, List<AbstractFlowNode> children)
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
            // if child execute fail or abort and need repeat
            if (repeat)
            {
                --CurrentIndex;
                ExecuteNextChild();
                return;
            }

            if (result == ENodeResult.SUCC)
            {
                FinishBySelf(ENodeResult.SUCC);
                return;
            }

            ExecuteNextChild();
        }



        private void ExecuteNextChild()
        {
            for (; ++CurrentIndex < Children.Count;)
            {
                AbstractFlowNode child = Children[CurrentIndex];
                if (!child.CanRunTopNode())
                {
                    continue;
                }
                child.DoNodeActivation();
                return;
            }
            FinishBySelf(ENodeResult.FAIL);
        }

        private int ChooseNextChildIndex()
        {
            for (; ++CurrentIndex < Children.Count;)
            {
                AbstractFlowNode child = Children[CurrentIndex];
                if (!child.CanRunTopNode())
                {
                    continue;
                }
                return CurrentIndex;
            }
            return -1;
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
            Debug.Assert(CurrentIndex >= 0 && CurrentIndex < Children.Count);
            AbstractFlowNode c = Children[CurrentIndex];
            bool abortNode = false;
            foreach (var decorator in ObserveNotifiedDecorators)
            {
                switch (decorator.FlowAbortMode)
                {
                    case EFlowAbortMode.SELF:
                        {
                            abortNode = decorator.AttachedNode == c;

                            break;
                        }
                    case EFlowAbortMode.LOWER_PRIORITY:
                        {
                            // 比decorator 所在的 node 的优先级低
                            abortNode = c.Id > decorator.AttachedNode.Id;

                            break;
                        }

                    case EFlowAbortMode.BOTH:
                        {
                            // 被打断时, 不需要通知parent
                            abortNode = c.Id >= decorator.AttachedNode.Id;

                            break;
                        }
                }
            }

            ObserveNotifiedDecorators.Clear();

            if (abortNode)
            {
                int oldIndex = CurrentIndex;

                // 从头选过节点
                CurrentIndex = -1;
                int newIndex = ChooseNextChildIndex();

                if (newIndex < 0)
                {
                    c.AbortByObserver();
                    FinishBySelf(ENodeResult.FAIL);
                    return;
                }

                AbstractFlowNode newNode = Children[newIndex];

                // ue 里有一个 ignore restart self的选项
                // 如果 重新选择节点又是当前节点时
                if (newIndex != oldIndex || (newIndex == oldIndex && !newNode.IgnoreRestartSelf))
                {
                    c.AbortByObserver();
                    newNode.DoNodeActivation();
                }
            }

        }

        protected internal override void OnAbort()
        {
            if (CurrentIndex >= 0 && CurrentIndex < Children.Count)
            {
                AbstractFlowNode c = Children[CurrentIndex];
                if (c.IsExecuting)
                {
                    c.AbortByObserver();
                }
            }
        }

    }
}
