using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Services
{
    public class ExecuteTimeStatistic : AbstractService
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private long _startTime;

        public ExecuteTimeStatistic(BehaviorTreeObject bt, int id) : base(bt, id)
        {
        }

        //public override void Tick(BehaviorTreeObject bt, float deltaTime)
        //{
        //    _totalTime += deltaTime;
        //}

        public override void ReceiveActivation()
        {
            _startTime = Bt.NowMills;
        }

        public override void ReceiveDeactivation()
        {
            s_logger.Debug("node:{id} totaltime:{time}", Id, Bt.NowMills - _startTime);
        }
    }
}
