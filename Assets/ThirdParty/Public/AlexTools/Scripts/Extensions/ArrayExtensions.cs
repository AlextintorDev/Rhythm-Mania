using System;
using UnityEngine;

public static class ArrayExtensions
{
    /// <summary>
    /// Baraja los elementos del array en orden aleatorio.
    /// </summary>
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
        }
    }

    /// <summary>
    /// Devuelve un elemento aleatorio del array.
    /// </summary>
    public static T RandomElement<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
        {
            Debug.LogWarning("Intentando obtener elemento aleatorio de un array vacío.");
            return default;
        }
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    /// <summary>
    /// Añade un elemento si no existe ya en el array.
    /// Retorna un nuevo array si lo añadió, el original si ya estaba.
    /// </summary>
    public static T[] AddIfNotExists<T>(this T[] array, T item)
    {
        if (Array.IndexOf(array, item) == -1)
        {
            T[] newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = item;
            return newArray;
        }
        return array;
    }
}
