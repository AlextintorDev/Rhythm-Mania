using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Permite serializar y deserializar pares clave-valor en un string.
/// Utiliza Dictionary para mayor eficiencia y facilidad de uso.
/// </summary>
public class SerializableString
{
    const string DELIM = ";";
    const string VALUE_DELIM = ":";

    private readonly Dictionary<string, string> _dict = new();

    public SerializableString() { }
    public SerializableString(string stringSerialized)
    {
        Dictionary<string, string> dict = ParseToDictionary(stringSerialized);
        foreach (var kv in dict)
            _dict[kv.Key] = kv.Value;
    }

    /// <summary>
    /// Agrega o actualiza un par clave-valor.
    /// </summary>
    public void AddValue(string key, string value)
    {
        _dict[key] = value;
    }

    /// <summary>
    /// Agrega varios pares serializados (en formato clave:valor;clave:valor).
    /// </summary>
    public void AddValues(string serializedValues)
    {
        Dictionary<string,string> dict = ParseToDictionary(serializedValues);
        foreach (var kv in dict)
            _dict[kv.Key] = kv.Value;
    }

    /// <summary>
    /// Devuelve el string serializado.
    /// </summary>
    public string GetSerializedString()
    {
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (var kv in _dict)
        {
            if (!first) sb.Append(DELIM);
            sb.Append(Escape(kv.Key)).Append(VALUE_DELIM).Append(Escape(kv.Value));
            first = false;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Devuelve todos los pares clave-valor como un diccionario.
    /// </summary>
    public Dictionary<string, string> GetKeyValues()
    {
        // Devuelve una copia para evitar modificaciones externas
        return new Dictionary<string, string>(_dict);
    }

    /// <summary>
    /// Devuelve todos los pares clave-valor de un string serializado como un diccionario.
    /// </summary>
    public static Dictionary<string, string> GetKeyValues(string stringSerialized)
    {
        return ParseToDictionary(stringSerialized);
    }

    /// <summary>
    /// Obtiene el valor asociado a una clave. Devuelve null si no existe.
    /// </summary>
    public string GetValue(string key)
    {
        return _dict.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Elimina una clave (y su valor) si existe.
    /// </summary>
    public void RemoveKey(string key)
    {
        _dict.Remove(key);
    }

    /// <summary>
    /// Escapa los delimitadores en claves y valores.
    /// </summary>
    private static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace(DELIM, "\\;").Replace(VALUE_DELIM, "\\:");
    }

    /// <summary>
    /// Desescapa los delimitadores en claves y valores.
    /// </summary>
    private static string Unescape(string s)
    {
        return s.Replace("\\:", ":").Replace("\\;", ";").Replace("\\\\", "\\");
    }

    /// <summary>
    /// Parsea un string serializado a un diccionario.
    /// </summary>
    private static Dictionary<string, string> ParseToDictionary(string stringSerialized)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(stringSerialized))
            return dict;

        string[] strings = stringSerialized.Split(new string[] { DELIM }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var str in strings)
        {
            string[] values = str.Split(new string[] { VALUE_DELIM }, 2, StringSplitOptions.None);
            if (values.Length < 2) continue;

            string key = Unescape(values[0]);
            string value = Unescape(values[1]);
            dict[key] = value;
        }
        return dict;
    }

    public static implicit operator string(SerializableString ss)
    {
        return ss.GetSerializedString();
    }

    public override string ToString()
    {
        return GetSerializedString();
    }
}
