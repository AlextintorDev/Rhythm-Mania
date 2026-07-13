using Newtonsoft.Json;
using UnityEngine;

namespace GameSaveSystem.Serialization.Objects
{
    [CreateAssetMenu(menuName = "Game Save System/Serialization/Objects/Json", fileName = "JsonSerializer", order = 0)]
    public class JsonSerializer : Serializer
    {
        [SerializeField] private bool minimizeJson = false;
        public override T Deserialize<T>(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError("[JsonSerializer] data given is null");
                return default(T);
            }

            T Obj = JsonConvert.DeserializeObject<T>(serializedData);
            return Obj;
        }

        public override string Serialize<T>(T data)
        {
            if (data == null)
            {
                Debug.LogError("[JsonSerializer] data given is null");
                return "";
            }

            JsonSerializerSettings settings = new()
            {
                Formatting = minimizeJson ? Formatting.None : Formatting.Indented

            };

            string serializedObject = JsonConvert.SerializeObject(data, settings);
            return serializedObject;
        }
    }
}
