using UnityEngine;

public static class RandomExtensions
{
    /// <summary>
    /// Devuelve true con la probabilidad indicada (0 a 1).
    /// </summary>
    public static bool Bool(float probabilityOfTrue = 0.5f)
    {
        return Random.value < probabilityOfTrue;
    }

    /// <summary>
    /// Devuelve 1 o -1 de manera aleatoria.
    /// </summary>
    public static int Sign()
    {
        return Random.value < 0.5f ? -1 : 1;
    }

    /// <summary>
    /// Devuelve un punto aleatorio dentro de un círculo en el plano X-Y.
    /// </summary>
    public static Vector2 InCircle(float radius = 1f)
    {
        return Random.insideUnitCircle * radius;
    }

    /// <summary>
    /// Devuelve un punto aleatorio dentro de una esfera en 3D.
    /// </summary>
    public static Vector3 InSphere(float radius = 1f)
    {
        return Random.insideUnitSphere * radius;
    }
}
