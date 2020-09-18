using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bright.BehaviorTree
{

    public abstract class AbstractComposite : AbstractFlowNode
    {

        protected List<AbstractDecorator> ObserveNotifiedDecorators { get; } = new List<AbstractDecorator>();

        protected AbstractComposite(BehaviorTreeObject bt, int id, List<AbstractService> services, List<AbstractDecorator> decorators)
             : base(bt, id, services, decorators)
        {

        }


        public void NotifyObserveDecoratorEvent(AbstractDecorator decorator)
        {
            if (!ObserveNotifiedDecorators.Contains(decorator))
            {
                if (ObserveNotifiedDecorators.Count == 0)
                {
                    Bt.AddJob(this, () => ProcessObserveDecoratorsChange());
                }
                ObserveNotifiedDecorators.Add(decorator);
            }
        }

        /// <summary>
        /// 调用此回调时，未必是 Decorator 的 condition change了.比如 UeBlackboard key 为 object类型,
        /// 当它选择的目标变化时,condition同样为true，此时应该打断并且重新执行,而不是保持原有的执行
        /// </summary>
        /// <param name="decorator"></param>
        protected internal abstract void ProcessObserveDecoratorsChange();
        //{
        //    DoObservedDecoratorsAbortCurrent();
        //    ObserveNotifiedDecorators.Clear();

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns> 是否打断了当前节点 </returns>
        //protected abstract bool DoObservedDecoratorsAbortCurrent();

        //protected abstract void ChooseNextNodeOnAbortCurrrent();

        public abstract void OnChildFinish(ENodeResult result, bool repeat);
    }
}
