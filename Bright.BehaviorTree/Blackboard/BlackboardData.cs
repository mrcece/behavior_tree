using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bright.BehaviorTree.Blackboard
{
    public class BlackboardData
    {
        public string Name { get; }

        public string Desc { get; }

        public BlackboardData Parent { get; }

        public List<BlackboardKeyData> Keys { get; }

        public BlackboardData(string name, string desc, BlackboardData parent, List<BlackboardKeyData> keys)
        {
            Name = name;
            Desc = desc;
            Parent = parent;
            if (parent != null)
            {
                foreach (BlackboardKeyData key in parent.Keys)
                {
                    if (keys.FindIndex(k => k.Name == key.Name) >= 0)
                    {
                        throw new DuplicateNameException($"blackboard type:{name} key:{key.Name} override parent:{parent.Name} same key");
                    }
                }
                Keys = new List<BlackboardKeyData>(parent.Keys.Count + keys.Count);
                Keys.AddRange(parent.Keys);
                Keys.AddRange(keys);
            }
            else
            {
                Keys = keys;
            }
        }

        public int GetKeyIndexByName(string keyName)
        {
            return Keys.FindIndex(k => k.Name == keyName);
        }
    }
}
