using System;
using System.Threading.Tasks;
using GameSaveSystem.Serialization.Objects;
using GameSaveSystem.Encryption;
using GameSaveSystem.Tools;
using UnityEngine;

namespace GameSaveSystem.Core
{
    public static class SaveManager
    {
        private static SaveManagerSettings settings;


        public static void Initialize()
        {
            Debug.Log("[SaveManager] Initializing...");
            settings = Resources.Load<SaveManagerSettings>("SaveManagerSettings");
            if (settings == null)
                settings = ScriptableObject.CreateInstance<SaveManagerSettings>();
            Debug.Log("[SaveManager] Initialized");
        }


        #region Save

        public static void Save<T>(string filename, T saveObject, Storage.Storage storage = null, Serializer serializer = null, Encrypter encrypter = null)
        {
            if (storage == null)
                storage = settings.Storage;
            if (serializer == null)
                serializer = settings.Serializer;
            if (encrypter == null)
                encrypter = settings.Encrypter;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                string save = serializer.Serialize<T>(saveObject);
                if (encrypter != null)
                    save = encrypter.Encrypt(save);
                storage.Save(cleanedFilename, save);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Save.\nFilename:{filename}\nError{e}");
            }
        }

        public static void Save<T>(string filename, T saveObject)
        {
            Save<T>(filename, saveObject, settings.Storage, settings.Serializer, settings.Encrypter);
        }

        public static async Task SaveAsync<T>(string filename, T saveObject, Storage.Storage storage = null, Serializer serializer = null, Encrypter encrypter = null)
        {
            if (storage == null)
                storage = settings.Storage;
            if (serializer == null)
                serializer = settings.Serializer;
            if (encrypter == null)
                encrypter = settings.Encrypter;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                string save = serializer.Serialize<T>(saveObject);
                if (encrypter != null)
                    save = encrypter.Encrypt(save);
                await storage.SaveAsync(cleanedFilename, save);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Save.\nFilename:{filename}\nError{e}");
            }
        }

        public static async Task SaveAsync<T>(string filename, T saveObject)
        {
            await SaveAsync<T>(filename, saveObject, settings.Storage, settings.Serializer, settings.Encrypter);
        }

        #endregion

        #region Load

        public static T Load<T>(string filename, Storage.Storage storage = null, Serializer serializer = null, Encrypter encrypter = null)
        {
            if (storage == null)
                storage = settings.Storage;
            if (serializer == null)
                serializer = settings.Serializer;
            if (encrypter == null)
                encrypter = settings.Encrypter;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                string save = storage.Load(cleanedFilename);
                if (encrypter != null)
                    save = encrypter.Decrypt(save);
                return serializer.Deserialize<T>(save);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Load.\nFilename:{filename}\nError{e}");
                return default(T);
            }
        }

        public static T Load<T>(string filename, Action<T> callback, Storage.Storage storage = null, Serializer serializer = null, Encrypter encrypter = null)
        {
            T result = Load<T>(filename, storage, serializer, encrypter);
            callback?.Invoke(result);
            return result;
        }

        public static T Load<T>(string filename, Action<T> callback)
        {
            return Load<T>(filename, callback, settings.Storage, settings.Serializer, settings.Encrypter);
        }

        public static async Task<T> LoadAsync<T>(string filename, Storage.Storage storage = null, Serializer serializer = null, Encrypter encrypter = null)
        {
            if (storage == null)
                storage = settings.Storage;
            if (serializer == null)
                serializer = settings.Serializer;
            if (encrypter == null)
                encrypter = settings.Encrypter;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                string save = await storage.LoadAsync(cleanedFilename);
                if (encrypter != null)
                    save = encrypter.Decrypt(save);
                return serializer.Deserialize<T>(save);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Load.\nFilename:{filename}\nError{e}");
                return default(T);
            }
        }

        public static async Task<T> LoadAsync<T>(string filename)
        {
            return await LoadAsync<T>(filename, settings.Storage, settings.Serializer, settings.Encrypter);
        }

        #endregion

        #region Delete

        public static void Delete(string filename, Storage.Storage storage = null)
        {
            if (storage == null)
                storage = settings.Storage;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                storage.Delete(cleanedFilename);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Delete.\nFilename:{filename}\nError{e.Message}");
            }
        }

        public static async Task DeleteAsync(string filename, Storage.Storage storage = null)
        {
            if (storage == null)
                storage = settings.Storage;

            try
            {
                string cleanedFilename = FilenameCleaner.Clean(filename);
                await storage.DeleteAsync(cleanedFilename);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Unexpected error on Delete.\nFilename:{filename}\nError{e.Message}");
            }
        }

        #endregion
    }
}