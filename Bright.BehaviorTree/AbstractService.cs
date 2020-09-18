using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree
{
    public abstract class AbstractService : AbstractAttachNode
    {
        protected AbstractService(BehaviorTreeObject bt, int id) : base(bt, id)
        {
        }


        public abstract void ReceiveActivation();

        public abstract void ReceiveDeactivation();

        [Nop]
        public virtual void ReceiveSearchStart()
        {

        }
    }
}
