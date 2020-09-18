using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree
{
    /// <summary>
    /// Service 与 Decorator类型不能独立存在,只在附加到 Composite和Task上
    /// </summary>
    public abstract class AbstractAttachNode : AbstractNode
    {
        /// <summary>
        /// 被附加的节点
        /// </summary>
        public AbstractFlowNode AttachedNode { get; internal set; }

        protected AbstractAttachNode(BehaviorTreeObject bt, int id) : base(bt, id)
        {
        }
    }
}
