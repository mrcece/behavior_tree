using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Decorators
{
    public class UeLoop : AbstractDecorator
    {
        private readonly int _maxLoopNum;
        private readonly long? _infiniteLoopTimeoutMills;
        private int _curLoopNum;
        private long _curLoopTimeoutTime;

        public UeLoop(BehaviorTreeObject bt, int id, EFlowAbortMode flowAbortMode, int maxLoopNum, float? infiniteLoopTimeout) : base(bt, id, flowAbortMode)
        {
            _maxLoopNum = maxLoopNum;
            _curLoopNum = 0;
            _infiniteLoopTimeoutMills = infiniteLoopTimeout != null ? (long?)(infiniteLoopTimeout.Value * 1000) : null;
        }

        public override bool NeedRepeat()
        {
            return _infiniteLoopTimeoutMills == null ? _curLoopNum < _maxLoopNum : Bt.NowMills < _curLoopTimeoutTime;
        }

        public override void ReceiveExecutionFinish(ENodeResult result)
        {
            if (_infiniteLoopTimeoutMills == null)
            {
                ++_curLoopNum;
            }
        }

        protected override void ReceiveObserverActivated()
        {
            _curLoopNum = 0;
            if (_infiniteLoopTimeoutMills is long loopTimeout)
            {
                _curLoopTimeoutTime = loopTimeout > 0 ? Bt.NowMills + loopTimeout : long.MaxValue;
            }
        }
    }
}
