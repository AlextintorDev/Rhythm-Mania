using System;
using UnityEngine;

/// <summary>
/// Clase base para estados.
/// </summary>
public abstract class State<T> where T : MonoBehaviour
{
    /// <summary>
    /// Referencia a la máquina de estados que administra este estado.
    /// </summary>
    protected StateMachine<T> Machine { get; private set; }

    /// <summary>
    /// Asigna la máquina al estado. Solo debe ser llamado por la implementación de la StateMachine.
    /// </summary>
    internal void AttachMachine(StateMachine<T> machine)
    {
        Machine = machine;
    }

    /// <summary>
    /// Referencia a la instancia de la clase contenedora.
    /// </summary>
    protected T Owner => Machine?.Owner;

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

    /// <summary>
    /// Evalúa si el estado quiere transicionar a otro estado.
    /// Debe retornar una instancia del próximo estado o null si desea un estado vacio.
    /// </summary>
    public abstract bool GetTransition(out State<T> state);

    /// <summary>
    /// Nombre para debug, utilza por defecto el de la clase.
    /// </summary>
    public virtual string Name => GetType().Name;
}