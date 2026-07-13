using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace GameSaveSystem.Serialization.Values
{
    [CreateAssetMenu(menuName = "Game Save System/Serialization/Values/XML", fileName = "XMLValuesSerializer", order = 0)]
    public class XMLValuesSerializer : ValuesSerializer
    {
        private const string RootElementName = "values";

        public override string Serialize<T>(Dictionary<string, string> values)
        {
            if (values == null)
            {
                Debug.LogError("[XMLValuesSerializer] values given is null");
                return "";
            }

            XElement root = new XElement(RootElementName);
            foreach (KeyValuePair<string, string> kvp in values)
            {
                root.Add(new XElement(kvp.Key, kvp.Value));
            }

            XDocument doc = new XDocument(root);
            return doc.ToString();
        }

        public override Dictionary<string, string> Deserialize<T>(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Debug.LogError("[XMLValuesSerializer] data given is null");
                return null;
            }

            XDocument doc = XDocument.Parse(serializedData);
            XElement root = doc.Element(RootElementName);
            if (root == null)
            {
                Debug.LogError($"[XMLValuesSerializer] root element '{RootElementName}' not found");
                return null;
            }

            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (XElement element in root.Elements())
            {
                values[element.Name.LocalName] = element.Value;
            }

            return values;
        }
    }
}
