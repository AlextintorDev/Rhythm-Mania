using GameSaveSystem.Encryption;
using GameSaveSystem.Serialization.Objects;
using GameSaveSystem.Storage;
using UnityEngine;

namespace GameSaveSystem.Core
{
    [CreateAssetMenu(menuName = "Game Save System/Save Manager Settings", fileName = "SaveManagerSettings", order = 0)]
    public class SaveManagerSettings : ScriptableObject
    {
        [Header("Save Settings")]
        [SerializeField] private Serializer serializer;
        public Serializer Serializer => serializer;
        [SerializeField] private Encrypter encrypter;
        public Encrypter Encrypter => encrypter;
        [SerializeField] private Storage.Storage storage;
        public Storage.Storage Storage => storage;
    }
}
