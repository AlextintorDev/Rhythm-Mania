using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameSaveSystem.Serialization.Values
{
    [CreateAssetMenu(menuName = "Game Save System/Serialization/Values/Text", fileName = "TextValuesSerializer", order = 0)]
    public class TextValuesSerializer : ValuesSerializer
    {
        [SerializeField] private string delimiter = ";";

        public override string Serialize<T>(Dictionary<string, string> values)
        {
            if (values == null)
            {
                Debug.LogError("[TextValuesSerializer] values given is null");
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in values)
            {
                sb.Append(kvp.Key);
                sb.Append(delimiter);
                sb.Append(kvp.Value);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public override Dictionary<string, string> Deserialize<T>(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError("[TextValuesSerializer] data given is null");
                return null;
            }

            Dictionary<string, string> values = new Dictionary<string, string>();
            string[] lines = serializedData.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                int index = line.IndexOf(delimiter);
                if (index < 0)
                {
                    Debug.LogWarning($"[TextValuesSerializer] line missing delimiter: {line}");
                    continue;
                }

                string key = line.Substring(0, index);
                string value = line.Substring(index + delimiter.Length);
                values[key] = value;
            }

            return values;
        }
    }
}
