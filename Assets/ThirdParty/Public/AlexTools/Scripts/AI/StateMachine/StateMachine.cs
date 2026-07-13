using System;
using UnityEngine;

/// <summary>
/// Máquina de Estados Finitos (FSM) genérica.
/// </summary>
[SerializeField]
public class StateMachine<T> where T : MonoBehaviour
{
    [SerializeField] private string debugInfo;
    /// <summary>
    /// Referencia a la instancia contenedora
    /// </summary>
    public T Owner { get; }

    private State<T> _currentState;
    public State<T> CurrentState => _currentState;

    /// <summary>
    /// Indica si la máquina de estados está actualmente en ejecución.
    /// Cuando está pausada, no se ejecuta el Update del estado actual.
    /// </summary>
    public bool IsRunning { get; private set; } = true;

    /// <param name="owner">Instancia de la clase contenedora.</param>
    /// <param name="initialState">Estado inicial de la máquina.</param>
    public StateMachine(T owner, State<T> initialState)
    {
        Owner = owner;

        if(initialState == null)
            Debug.LogWarning("El estado inicial es ninguno");

        ChangeState(initialState);
    }

    /// <summary>
    /// Llama a Update del estado actual y evalúa transiciones.
    /// </summary>
    public void Update()
    {
        debugInfo = GetDebugInfo();
        if (!IsRunning || _currentState == null) return;

        // Ejecutar la lógica del estado actual
        _currentState.Update();

        // Evaluar si el estado quiere transicionar devolviendo una instancia
        if (_currentState.GetTransition(out State<T> nextState) && nextState != _currentState)
        {
            TransitionTo(nextState);
        }
    }

    /// <summary>
    /// Transiciona a una instancia de estado específica.
    /// </summary>
    public void TransitionTo(State<T> newState)
    {
        ChangeState(newState);
    }

    /// <summary>
    /// Cambia al nuevo estado, ejecutando Exit() en el estado actual y Enter() en el nuevo.
    /// Asigna Machine al nuevo estado si aún no la tiene.
    /// </summary>
    private void ChangeState(State<T> newState)
    {
        // Salir del estado actual
        _currentState?.Exit();

        // Asegurar enlace de Machine
        newState?.AttachMachine(this);

        // Cambiar al nuevo estado
        _currentState = newState;

        // Entrar al nuevo estado
        _currentState?.Enter();

        Debug.Log($"StateMachine<{typeof(T).Name}>: Cambiado a estado {(_currentState?.Name ?? "None")}");
    }

    /// <summary>
    /// Verifica si el estado actual es del tipo indicado.
    /// </summary>
    public bool IsInState<TState>() where TState : State<T>
        => _currentState is TState;

    /// <summary>
    /// Pausa la ejecución de la máquina de estados.
    /// El estado actual permanece activo pero no se ejecuta su Update().
    /// </summary>
    public void Pause() => IsRunning = false;

    /// <summary>
    /// Reanuda la ejecución de la máquina de estados.
    /// </summary>
    public void Resume() => IsRunning = true;

    /// <summary>
    /// Detiene completamente la máquina de estados.
    /// Sale del estado actual y deja la máquina sin estado activo.
    /// </summary>
    public void Stop()
    {
        _currentState?.Exit();
        _currentState = null;
        IsRunning = false;
    }

    /// <summary>
    /// Devuelve información de debugging sobre el estado actual de la máquina.
    /// </summary>
    public string GetDebugInfo() => $"StateMachine<{typeof(T).Name}> - Running: {IsRunning}, Current State: {(_currentState?.Name ?? "None")}";
}