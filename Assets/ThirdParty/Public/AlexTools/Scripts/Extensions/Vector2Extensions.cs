using UnityEngine;

public static class Vector2Extensions
{
    /// <summary>
    /// Cambia solo el valor de X
    /// </summary>
    public static void SetX(this Vector2 v, float x)
    {
        v.x = x;
    }

    /// <summary>
    /// Cambia solo el valor de Y
    /// </summary>
    public static void SetY(this Vector2 v, float y)
    {
        v.y = y;
    }
}
