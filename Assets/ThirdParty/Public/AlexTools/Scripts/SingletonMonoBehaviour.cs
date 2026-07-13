using UnityEngine;


/// <summary>
/// Clase utilizada para Singletons que sean MonoBehaviour. Si al llamar a la Instancia, su valor es nulo, buscar� formas de autoinicializarse de las siguentes formas:
/// <list type="number">
/// <item><description>Buscar un objeto existente en escena.</description></item>
/// <item><description>Instanciar un objeto vac�o con el componente <typeparamref name="T"/>.</description></item>
/// <item><description>Si <see cref="PrefabPath"/> no es vac�o, intentar cargar desde Resources.</description></item>
/// </list>
/// Luego, si <see cref="Persistant"/> es <c>true</c>, se utiliza <see cref="Object.DontDestroyOnLoad(UnityEngine.Object)"/>.
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    /// <summary>
    /// Instancia privada con el valor del singleton real
    /// </summary>
    private static T instance;

    /// <summary>
    /// Referencia de la instancia
    /// </summary>
    public static T Instance
    {
        get
        {
            //No existe el singleton
            if (instance == null)
            {
                //Buscamos en escena
                instance = FindFirstObjectByType<T>();

                //Si no encontramos
                if (instance == null)
                {
                    // Accedemos a PrefabPath mediante una instancia temporal del tipo
                    T GO = new GameObject("Temp_" + typeof(T).Name).AddComponent<T>();
                    string path = (GO as SingletonMonoBehaviour<T>)?.PrefabPath ?? "";
                    bool persistant = (GO as SingletonMonoBehaviour<T>)?.Persistant ?? false;

                    //Existe un prefab definido
                    if (!string.IsNullOrEmpty(path))
                    {
                        T prefab = Resources.Load<T>(path);
                        if (prefab != null && prefab.TryGetComponent(out T _))
                        {
                            DestroyImmediate(GO.gameObject);
                            GO = Instantiate(prefab);
                        }
                    }

                    instance = GO;
                    GO.name = typeof(T).Name;
                    if (persistant)
                    {
                        GO.transform.SetParent(null);
                        DontDestroyOnLoad(GO.gameObject);
                    }
                }
                //Encontramos el objeto
                else
                {
                    bool persistant = (instance as SingletonMonoBehaviour<T>)?.Persistant ?? false;
                    if (persistant)
                    {
                        instance.transform.SetParent(null);
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Ruta del prefab a cargar desde la carpeta <c>Resources</c>.
    /// Puede ser sobrescrita mediante <see langword="override"/> en las clases hijas. 
    /// Si no se encuentra o la ruta est� vac�a, se instanciar� un objeto vac�o con el componente.
    /// <para>
    /// Ejemplo de uso: para un prefab ubicado en <c>Assets/Resources/Prefabs/MusicManager.prefab</c>,
    /// este valor deber�a ser <c>"Prefabs/MusicManager"</c>.
    /// </para>
    /// </summary>
    public virtual string PrefabPath => "";


    /// <summary>
    /// Indica si la instancia debe mantenerse entre escenas.
    /// Si es <c>true</c>, se aplicar� <see cref="Object.DontDestroyOnLoad(UnityEngine.Object)"/> autom�ticamente.
    /// Las clases hijas pueden sobreescribir este valor utilizando <c>override</c>.
    /// </summary>
    public virtual bool Persistant => false;


    public virtual void Awake()
    {
        CheckSingletonInstaces();
    }

    /// <summary>
    /// Metodo para asegurarse que solo existe un objeto de la Instancia
    /// </summary>
    protected virtual void CheckSingletonInstaces()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metodo ejecutado al ser instanciado el singleton
    /// </summary>
    protected virtual void OnInstantiate()
    {
    }
}

