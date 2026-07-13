using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Baraja los elementos de la lista en orden aleatorio.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    /// <summary>
    /// Devuelve un elemento aleatorio de la lista.
    /// </summary>
    public static T RandomElement<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("Intentando obtener elemento aleatorio de una lista vacía.");
            return default;
        }
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Añade un elemento si no existe ya en la lista.
    /// Retorna true si lo añadió, false si ya estaba.
    /// </summary>
    public static bool AddIfNotExists<T>(this IList<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }
}
