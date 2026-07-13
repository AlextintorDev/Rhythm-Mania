using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Cambia solo el eje X de la posición mundial.
    /// </summary>
    public static void SetX(this Transform t, float x)
    {
        Vector3 pos = t.position;
        pos.x = x;
        t.position = pos;
    }

    /// <summary>
    /// Cambia solo el eje Y de la posición mundial.
    /// </summary>
    public static void SetY(this Transform t, float y)
    {
        Vector3 pos = t.position;
        pos.y = y;
        t.position = pos;
    }

    /// <summary>
    /// Cambia solo el eje Z de la posición mundial.
    /// </summary>
    public static void SetZ(this Transform t, float z)
    {
        Vector3 pos = t.position;
        pos.z = z;
        t.position = pos;
    }
}
