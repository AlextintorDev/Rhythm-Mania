using System.Collections.Generic;
using UnityEngine;

namespace GameSaveSystem.Serialization.Values
{
    public abstract class ValuesSerializer : ScriptableObject
    {
        public abstract string Serialize<T>(Dictionary<string,string> values);
        public abstract Dictionary<string,string> Deserialize<T>(string serializedData);
    }
}
