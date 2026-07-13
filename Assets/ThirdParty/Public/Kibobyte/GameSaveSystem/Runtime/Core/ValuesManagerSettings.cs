using GameSaveSystem.Encryption;
using GameSaveSystem.Serialization.Values;
using GameSaveSystem.Storage;
using UnityEngine;

namespace GameSaveSystem.Core
{
    [CreateAssetMenu(menuName = "Game Save System/Values Manager Settings", fileName = "ValuesManagerSettings", order = 0)]
    public class ValuesManagerSettings : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] private string valuesfileName = "values";
        public string ValuesFileName => valuesfileName;

        [SerializeField] private bool createValueIfNotFound = true;
        public bool CreateKeyValueIfNotFound => createValueIfNotFound;

        [SerializeField] private bool autoSaveAfterSet = true;
        public bool AutoSaveAfterSet => autoSaveAfterSet;

        [SerializeField] private InitializationMode initializationMode = InitializationMode.Sync;
        public InitializationMode InitializationMode => initializationMode;

        [SerializeField] private InitializationMode autoSaveMode = InitializationMode.Sync;
        public InitializationMode AutoSaveMode => autoSaveMode;


        [Header("Module Settings")]
        [SerializeField] private ValuesSerializer valuesSerializer;
        public ValuesSerializer ValuesSerializer => valuesSerializer;
        [SerializeField] private Encrypter encrypter;
        public Encrypter Encrypter => encrypter;
        [SerializeField] private Storage.Storage storage;
        public Storage.Storage Storage => storage;
    }
}
