using System.Collections;
using UnityEngine;

public class CoroutineRunner : SingletonMonoBehaviour<CoroutineRunner>
{
    public override bool Persistant => true;

    /// <summary>
    /// Lanza una coroutine desde cualquier lugar sin MonoBehaviour.
    /// </summary>
    public static Coroutine Run(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Detiene una coroutine lanzada con Run.
    /// </summary>
    public static void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
            Instance.StopCoroutine(coroutine);
    }
}
