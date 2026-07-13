using UnityEngine;

[System.Serializable]
public class RandomFloat
{
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 1f;

    /// <summary>
    /// Retorna el valor, ya sea fijo o aleatorio según isRandom.
    /// </summary>
    public float Value
    {
        get
        {
            return Random.Range(min, max);
        }
    }

    // Operador implícito para que se use como float
    public static implicit operator float(RandomFloat v)
    {
        return v.Value;
    }

    /// <summary>
    /// Define un nuevo rango aleatorio.
    /// </summary>
    public void SetRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
