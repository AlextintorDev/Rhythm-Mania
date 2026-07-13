using System;
using System.Collections.Generic;

/// <summary>
/// GameEventBus es un sistema de eventos global y est�tico que permite la suscripci�n, desuscripci�n y publicaci�n de eventos
/// de manera desacoplada en todo el proyecto. Los eventos (que suelen ser clases) deben implementar la interfaz IGameEvent.
/// </summary>

//Ejemplo de evento
//public class GetEXPEvent : IGameEvent
//{
//    public int amount;
//    public GetEXPEvent(int amount)
//    {
//        this.amount = amount;
//    }
//}

public static class GameEventBus
{
    /// <summary>
    /// Diccionario que almacena listas de callbacks (delegados) para cada tipo de evento.
    /// La clave es el tipo de evento, y el valor es una lista de m�todos que ser�n llamados cuando se publique ese evento.
    /// </summary>
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    /// <summary>
    /// Suscribe un m�todo (callback) a un tipo de evento espec�fico.
    /// Cuando se publique un evento de este tipo, se llamar� a este m�todo.
    /// </summary>
    /// <typeparam name="T">Tipo de evento al que se suscribe. Debe implementar IGameEvent.</typeparam>
    /// <param name="callback">M�todo que se ejecutar� cuando se publique el evento.</param>
    public static void Subscribe<T>(Action<T> callback) where T : IGameEvent
    {
        var type = typeof(T);
        // Si no hay suscriptores para este tipo de evento, crea una nueva lista
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();
        // Agrega el callback a la lista de suscriptores de este tipo de evento
        _subscribers[type].Add(callback);
    }

    /// <summary>
    /// Elimina la suscripci�n de un m�todo (callback) a un tipo de evento espec�fico.
    /// </summary>
    /// <typeparam name="T">Tipo de evento del que se desuscribe. Debe implementar IGameEvent.</typeparam>
    /// <param name="callback">M�todo que se dejar� de ejecutar cuando se publique el evento.</param>
    public static void Unsubscribe<T>(Action<T> callback) where T : IGameEvent
    {
        var type = typeof(T);
        // Si existen suscriptores para este tipo de evento, elimina el callback de la lista
        if (_subscribers.TryGetValue(type, out var list))
            list.Remove(callback);
    }

    /// <summary>
    /// Publica (emite, invoka) un evento. Todos los m�todos suscritos a este tipo de evento ser�n llamados.
    /// </summary>
    /// <typeparam name="T">Tipo de evento a publicar. Debe implementar IGameEvent.</typeparam>
    /// <param name="gameEvent">Instancia del evento a publicar.</param>
    public static void Invoke<T>(T gameEvent) where T : IGameEvent
    {
        var type = typeof(T);
        
        // Si existen suscriptores para este tipo de evento, llama a cada uno de ellos pasando el evento como par�metro
        if (_subscribers.TryGetValue(type, out var list))
        {
            List<Delegate> callbacks = new(list);
            foreach (var callback in callbacks)
                ((Action<T>)callback)?.Invoke(gameEvent);
        }
    }
}

