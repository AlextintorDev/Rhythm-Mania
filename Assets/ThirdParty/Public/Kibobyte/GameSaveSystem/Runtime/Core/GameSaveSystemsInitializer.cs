using UnityEngine;

namespace GameSaveSystem.Core
{
    public static class SaveSystemsInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async void Initialize()
        {
            SaveManager.Initialize();

            ValuesManagerSettings settings = Resources.Load<ValuesManagerSettings>("ValuesManagerSettings");
            if (settings == null)
                settings = ScriptableObject.CreateInstance<ValuesManagerSettings>();

            if (settings.InitializationMode == InitializationMode.Sync)
                ValuesManager.Initialize();
            else
                await ValuesManager.InitializeAsync();
        }
    }
}
