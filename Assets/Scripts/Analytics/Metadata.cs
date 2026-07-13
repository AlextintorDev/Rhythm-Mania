using System.Collections.Generic;

public class Metadata
{
    private static Dictionary<string, string> metadata = new Dictionary<string, string>();

    public static void SetMetadata(string key, string value)
    {
        if (!metadata.ContainsKey(key))
        {
            metadata.Add(key, value);
        }
        else
        {
            metadata[key] = value;
        }
    }

    public static string GetMetadata(string key, bool removeAfterGet = false)
    {
        if (metadata.ContainsKey(key))
        {
            string value = metadata[key];
            if (removeAfterGet)
            {
                metadata.Remove(key);
            }
            return value;
        }
        else
        {
            return null;
        }
    }
}
