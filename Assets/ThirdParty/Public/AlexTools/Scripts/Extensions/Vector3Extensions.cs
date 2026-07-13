using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Cambia solo el valor de X
    /// </summary>
    public static void SetX(this Vector3 v, float x)
    {
        v.x = x;
    }

    /// <summary>
    /// Cambia solo el valor de Y
    /// </summary>
    public static void SetY(this Vector3 v, float y)
    {
        v.y = y;
    }

    /// <summary>
    /// Cambia solo el valor de Z
    /// </summary>
    public static void SetZ(this Vector3 v, float z)
    {
        v.z = z;
    }
}
