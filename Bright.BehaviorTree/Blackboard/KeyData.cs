using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Bright.BehaviorTree.Blackboard
{
    public abstract class KeyData
    {
        public BlackboardKeyData CfgData { get; }

        public EKeyType Type => CfgData.Type;

        protected KeyData(BlackboardKeyData cfgData)
        {
            CfgData = cfgData;
        }
    }

    public abstract class KeyData<T> : KeyData
    {
        public T Value { get; internal set; }

        public KeyData(BlackboardKeyData cfgData) : base(cfgData)
        {

        }
    }

    public class BoolKeyData : KeyData<bool>
    {
        public BoolKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class IntKeyData : KeyData<int>
    {
        public IntKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class FloatKeyData : KeyData<float>
    {
        public FloatKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class StringKeyData : KeyData<string>
    {
        public StringKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
            Value = "";
        }
    }

    public class VectorKeyData : KeyData<Vector3>
    {
        public VectorKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class RotatorKeyData : KeyData<Vector3>
    {
        public RotatorKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }


    public class NameKeyData : KeyData<string>
    {
        public NameKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
            Value = "";
        }
    }

    public class ClassKeyData : KeyData<object>
    {
        public ClassKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class EnumKeyData : KeyData<int>
    {
        public EnumKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }

    public class ObjectKeyData : KeyData<object>
    {
        public ObjectKeyData(BlackboardKeyData cfgData) : base(cfgData)
        {
        }
    }
}
