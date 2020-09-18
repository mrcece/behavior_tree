using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree
{
    public abstract class AbstractNode
    {
        public BehaviorTreeObject Bt { get; }

        public int Id { get; }

        public int Version { get; protected set; }

        public bool AutoTick { get; protected set; }

        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                ++Version;
            }
        }

        protected AbstractNode(BehaviorTreeObject bt, int id)
        {
            Bt = bt;
            Id = id;
        }

        [Nop]
        public virtual void Tick(float deltaTime)
        {

        }
    }
}
