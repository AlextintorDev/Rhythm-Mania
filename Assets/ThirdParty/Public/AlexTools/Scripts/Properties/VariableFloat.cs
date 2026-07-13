using UnityEngine;

[System.Serializable]
public class VariableFloat
{
    [SerializeField] private bool isRandom = false;
    [ShowIf(nameof(isRandom), false, true), SerializeField] private float value = 0f;
    [ShowIf(nameof(isRandom), true, true), SerializeField] private float min = 0f;
    [ShowIf(nameof(isRandom), true, true), SerializeField] private float max = 1f;

    /// <summary>
    /// Retorna el valor, ya sea fijo o aleatorio según isRandom.
    /// </summary>
    public float Value
    {
        get
        {
            if (!isRandom) return value;
            return Random.Range(min, max);
        }
    }

    // Operador implícito para que se use como float
    public static implicit operator float(VariableFloat v)
    {
        return v.Value;
    }

    /// <summary>
    /// Define un nuevo rango aleatorio.
    /// </summary>
    public void SetRandom(float min, float max)
    {
        isRandom = true;
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// Define un nuevo valor constante.
    /// </summary>
    public void SetManual(float v)
    {
        isRandom = false;
        this.value = v;
    }
}
