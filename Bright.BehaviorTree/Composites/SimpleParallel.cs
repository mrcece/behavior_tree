using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Composites
{

    public enum EFinishMode
    {
        IMMEDIATE, // main 节点完成后，立即打断 backgroundNode 执行
        DELAYED,  // main节点完成后，等待backgroundNode完成
    }

    public class SimpleParallel : AbstractComposite
    {
        private readonly AbstractTask _mainTask;

        private readonly AbstractFlowNode _backgroundNode;

        private readonly EFinishMode _finishMode;

        public SimpleParallel(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators,
            AbstractTask mainTask, AbstractFlowNode backgroundNode, EFinishMode finishMode) : base(bt, id, services, decorators)
        {
            _mainTask = mainTask;
            _backgroundNode = backgroundNode;
            _finishMode = finishMode;
        }

        public override void OnChildFinish(ENodeResult result, bool repeat)
        {
            throw new NotImplementedException();
        }

        protected override void ActivateChildrenDecorators()
        {
            throw new NotImplementedException();
        }

        protected override void DeactivateChildrenDecorators()
        {
            throw new NotImplementedException();
        }

        protected override void OnNodeActivation()
        {
            throw new NotImplementedException();
        }

        protected override void OnNodeDeactivation()
        {
            throw new NotImplementedException();
        }

        protected internal override void ProcessObserveDecoratorsChange()
        {
            throw new NotImplementedException();
        }

        protected internal override void OnAbort()
        {
            throw new NotImplementedException();
        }
    }
}
