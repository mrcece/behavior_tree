using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree
{
    public abstract class AbstractTask : AbstractFlowNode
    {
        protected AbstractTask(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators)
            : base(bt, id, services, decorators)
        {

        }

        protected sealed override void ActivateChildrenDecorators()
        {

        }

        protected sealed override void DeactivateChildrenDecorators()
        {

        }

        /// <summary>
        /// 当被 父或者更高层级 Observer Abort 时调用此接口.
        /// 即使 触发此函数. OnNodeDeactive 依然会被调用.
        /// 故一般来说, 实现 OnNodeDeactive 就够了
        /// </summary>
        [Nop]
        protected internal virtual void ReceiveAbort()
        {

        }
    }
}
