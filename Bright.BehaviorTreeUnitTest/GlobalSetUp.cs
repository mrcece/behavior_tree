using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pefect.BehaviorTreeUnitTest
{
    [TestClass]
    public static class GlobalSetUp
    {
        [AssemblyInitialize]
        public static void SetUp(TestContext _)
        {
            Bright.Common.LogUtil.InitSimpleNLogConfigure(NLog.LogLevel.Trace);
        }
    }
}
