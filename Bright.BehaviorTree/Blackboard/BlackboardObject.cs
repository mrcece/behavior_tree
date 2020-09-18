using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Bright.BehaviorTree.Blackboard
{
    public class BlackboardObject
    {
        internal static KeyData CreateKey(BlackboardKeyData cfgData)
        {
            switch (cfgData.Type)
            {
                case EKeyType.BOOL: return new BoolKeyData(cfgData);
                case EKeyType.INT: return new IntKeyData(cfgData);
                case EKeyType.FLOAT: return new FloatKeyData(cfgData);
                case EKeyType.STRING: return new StringKeyData(cfgData);
                case EKeyType.VECTOR: return new VectorKeyData(cfgData);
                case EKeyType.ROTATOR: return new RotatorKeyData(cfgData);
                case EKeyType.NAME: return new NameKeyData(cfgData);
                case EKeyType.CLASS: return new ClassKeyData(cfgData);
                case EKeyType.ENUM: return new EnumKeyData(cfgData);
                case EKeyType.OBJECT: return new ObjectKeyData(cfgData);
                default: throw new ArgumentException($"unknown key type:{cfgData.Type}");
            }
        }

        public BlackboardData CfgData { get; }

        private readonly List<KeyData> _keys;

        private readonly List<(int Index, Action Callback)> _keyChangeListener = new List<(int Index, Action Callback)>();


        public BlackboardObject(BlackboardData cfgData)
        {
            CfgData = cfgData;
            _keys = new List<KeyData>(cfgData.Keys.Count);

            foreach (BlackboardKeyData key in cfgData.Keys)
            {
                _keys.Add(CreateKey(key));
            }
        }

        public int GetKeyIndexByName(string keyName)
        {
            return CfgData.GetKeyIndexByName(keyName);
        }

        public void ListenKeyChange(int index, Action action)
        {
            Debug.Assert(!_keyChangeListener.Contains((index, action)));
            _keyChangeListener.Add((index, action));
        }

        public void UnlistenKeyChange(int index, Action action)
        {
            bool removeAny = _keyChangeListener.Remove((index, action));
            Debug.Assert(removeAny);
        }

        private void FireKeyChange(int index)
        {
            foreach (var e in _keyChangeListener)
            {
                if (e.Index == index)
                {
                    e.Callback();
                }
            }
        }

        #region Get&Set KeyValue

        public KeyData GetValueByIndex(int index)
        {
            return _keys[index];
        }

        public T GetValueByName<T>(string name) where T : KeyData
        {
            return (T)_keys[GetKeyIndexByName(name)];
        }

        private T GetValue<T>(int index) where T : KeyData
        {
            return (T)_keys[index];
        }

        public bool GetBoolValue(int index)
        {
            return GetValue<BoolKeyData>(index).Value;
        }

        public void SetBoolValue(int index, bool value)
        {
            var data = GetValue<BoolKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public int GetIntValue(int index)
        {
            return GetValue<IntKeyData>(index).Value;
        }

        public void SetIntValue(int index, int value)
        {
            var data = GetValue<IntKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public float GetFloatValue(int index)
        {
            return GetValue<FloatKeyData>(index).Value;
        }

        public void SetFloatValue(int index, float value)
        {
            var data = GetValue<FloatKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public string GetStringValue(int index)
        {
            return GetValue<StringKeyData>(index).Value;
        }

        public void SetStringValue(int index, string value)
        {
            var data = GetValue<StringKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public Vector3 GetVectorValue(int index)
        {
            return GetValue<VectorKeyData>(index).Value;
        }

        public void SetVectorValue(int index, Vector3 value)
        {
            var data = GetValue<VectorKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public Vector3 GetRotatorValue(int index)
        {
            return GetValue<RotatorKeyData>(index).Value;
        }

        public void SetRotatorValue(int index, Vector3 value)
        {
            var data = GetValue<VectorKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public string GetNameValue(int index)
        {
            return GetValue<NameKeyData>(index).Value;
        }

        public void SetNameValue(int index, string value)
        {
            var data = GetValue<NameKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public object GetClassValue(int index)
        {
            return GetValue<ClassKeyData>(index).Value;
        }

        public void SetClassValue(int index, object value)
        {
            var data = GetValue<ClassKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public int GetEnumValue(int index)
        {
            return GetValue<EnumKeyData>(index).Value;
        }

        public void SetEnumValue(int index, int value)
        {
            var data = GetValue<EnumKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }

        public object GetObjectValue(int index)
        {
            return GetValue<ObjectKeyData>(index).Value;
        }

        public void SetObjectValue(int index, object value)
        {
            var data = GetValue<ObjectKeyData>(index);
            if (data.Value != value)
            {
                data.Value = value;
                FireKeyChange(index);
            }
        }
        #endregion
    }
}
