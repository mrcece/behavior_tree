using System;
using System.Collections.Generic;
using System.Text;

namespace Bright.BehaviorTree.Blackboard
{
    public class BlackboardKeyData
    {
        public string Name { get; }

        public string Desc { get; }

        public bool IsStatic { get; }

        public EKeyType Type { get; }

        public string TypeClassName { get; }


        public BlackboardKeyData(string name, string desc, bool isStatic, EKeyType type, string typeClassName)
        {
            Name = name;
            Desc = desc;
            IsStatic = isStatic;
            Type = type;
            TypeClassName = typeClassName;
        }
    }
}
