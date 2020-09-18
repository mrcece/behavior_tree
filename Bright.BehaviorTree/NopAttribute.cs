using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree
{

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NopAttribute : Attribute
    {
    }
}
