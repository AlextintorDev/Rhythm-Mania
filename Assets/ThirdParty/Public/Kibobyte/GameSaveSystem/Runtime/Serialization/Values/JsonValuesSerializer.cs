using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GameSaveSystem.Serialization.Values
{
    [CreateAssetMenu(menuName = "Game Save System/Serialization/Values/Json", fileName = "JsonValuesSerializer", order = 0)]
    public class JsonValuesSerializer : ValuesSerializer
    {
        [SerializeField] private bool minimizeJson = false;

        public override string Serialize<T>(Dictionary<string, string> values)
        {
            if (values == null)
            {
                Debug.LogError("[JsonValuesSerializer] values given is null");
                return "";
            }

            JsonSerializerSettings settings = new()
            {
                Formatting = minimizeJson ? Formatting.None : Formatting.Indented
            };
            return JsonConvert.SerializeObject(values, settings);
        }

        public override Dictionary<string, string> Deserialize<T>(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError("[JsonValuesSerializer] data given is null");
                return null;
            }

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedData);
        }
    }
}
