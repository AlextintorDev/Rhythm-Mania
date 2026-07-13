using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Activa el GameObject.
    /// </summary>
    public static void Activate(this GameObject go)
    {
        go.SetActive(true);
    }

    /// <summary>
    /// Desactiva el GameObject.
    /// </summary>
    public static void Deactivate(this GameObject go)
    {
        go.SetActive(false);
    }

    /// <summary>
    /// Busca un componente en el GameObject. Si no existe, lo añade y lo retorna.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>() ?? go.AddComponent<T>();
        return comp;
    }

    /// <summary>
    /// Elimina todos los hijos del GameObject.
    /// </summary>
    public static void DestroyAllChildren(this GameObject go)
    {
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(go.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Cambia la capa del GameObject y de todos sus hijos.
    /// </summary>
    public static void SetLayerRecursively(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }
}