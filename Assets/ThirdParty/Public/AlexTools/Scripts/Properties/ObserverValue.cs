using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contenedor observable para un valor de tipo <typeparamref name="T"/>.
/// Notifica a los suscriptores mediante <see cref="UnityEvent"/> cuando el valor cambia.
/// </summary>
/// <remarks>
/// Los cambios se notifican al llamar a <see cref="Set(T)"/> o al establecer <see cref="Value"/>.
/// Puede suscribirse con <see cref="AddListener(UnityAction{T})"/> y desuscribirse con <see cref="RemoveListener(UnityAction{T})"/>.
/// </remarks>
[Serializable]
public class ObserverValue<T>
{
    [SerializeField] T value;

    public T Value
    {
        get => value;
        set => Set(value);
    }

    /// <summary>
    /// Evento invocado cuando el valor cambia. Pasa el nuevo valor como parámetro.
    /// </summary>
    [SerializeField] UnityEvent<T> onValueChanged;

    /// Conversión implícita a al tipo del valor.
    public static implicit operator T(ObserverValue<T> observer) => observer.value;

    /// <summary>
    /// Constructor con un valor inicial y un callback opcional.
    /// </summary>
    /// <param name="value">Valor inicial.</param>
    /// <param name="callback">Suscriptor inicial invocado cuando el valor cambie.</param>
    public ObserverValue(T value, UnityAction<T> callback = null)
    {
        this.value = value;
        onValueChanged = new UnityEvent<T>();
        if (callback != null) onValueChanged.AddListener(callback);
    }

    /// <summary>
    /// Establece el valor si es diferente al actual y notifica a los suscriptores llamando a <see cref="Invoke"/>.
    /// </summary>
    /// <param name="value">Nuevo valor.</param>
    public void Set(T value)
    {
        if (Equals(this.value, value)) return;
        this.value = value;
        Invoke();
    }

    /// <summary>
    /// Invoca el evento de cambio con el valor actual.
    /// </summary>
    public void Invoke()
    {
        Debug.Log($"Invoking {onValueChanged.GetPersistentEventCount()} listeners");
        onValueChanged.Invoke(value);
    }

    /// <summary>
    /// Agrega un suscriptor al evento de cambio.
    /// Ignora callbacks nulos.
    /// </summary>
    public void AddListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if (onValueChanged == null) onValueChanged = new UnityEvent<T>();

        onValueChanged.AddListener(callback);
    }

    /// <summary>
    /// Elimina un suscriptor del evento de cambio.
    /// </summary>
    public void RemoveListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if (onValueChanged == null) return;

        onValueChanged.RemoveListener(callback);
    }

    /// <summary>
    /// Elimina todos los suscriptores registrados.
    /// </summary>
    public void RemoveAllListeners()
    {
        if (onValueChanged == null) return;
  
        onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Elimina suscriptores y restablece el valor.
    /// </summary>
    public void Dispose()
    {
        RemoveAllListeners();
        onValueChanged = null;
        value = default;
    }
}