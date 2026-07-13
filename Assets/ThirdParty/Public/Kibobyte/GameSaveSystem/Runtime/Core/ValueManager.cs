using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using GameSaveSystem.Tools;

namespace GameSaveSystem.Core
{
    public static class ValuesManager
    {
        private static ValuesManagerSettings settings;
        private static Dictionary<string, string> valueBuffer;
        private static InitializationState initState = InitializationState.NotInitialized;
        public static InitializationState InitializationState => initState; 

        public static void Initialize()
        {
            Debug.Log("[ValuesManager] Initializing...");
            initState = InitializationState.Initializing;
            settings = Resources.Load<ValuesManagerSettings>("ValuesManagerSettings");
            if (settings == null)
                settings = ScriptableObject.CreateInstance<ValuesManagerSettings>();
            Debug.Log("[ValuesManager] Loading buffer...");
            valueBuffer = LoadValues();

            if(valueBuffer == null)
            {
                Debug.LogWarning("[ValuesManager] No existing buffer found, created new one.");
                valueBuffer = new Dictionary<string, string>();
            }
            initState = InitializationState.Initialized;
            Debug.Log("[ValuesManager] Initialized");
        }

        public static async Task InitializeAsync()
        {
            Debug.Log("[ValuesManager] Initializing...");
            initState = InitializationState.Initializing;
            settings = Resources.Load<ValuesManagerSettings>("ValuesManagerSettings");
            if (settings == null)
                settings = ScriptableObject.CreateInstance<ValuesManagerSettings>();
            Debug.Log("[ValuesManager] Loading buffer...");
            valueBuffer = await LoadValuesAsync();

            if(valueBuffer == null)
            {
                Debug.LogWarning("[ValuesManager] No existing buffer found, created new one.");
                valueBuffer = new Dictionary<string, string>();
            }
            initState = InitializationState.Initialized;
            Debug.Log("[ValuesManager] Initialized");
        }

        #region LoadValues

        private static Dictionary<string, string> LoadValues()
        {
            try
            {
                string filename = FilenameCleaner.Clean(settings.ValuesFileName);
                string data = settings.Storage.Load(filename);
                if (string.IsNullOrEmpty(data))
                    return null;
                if (settings.Encrypter != null)
                    data = settings.Encrypter.Decrypt(data);
                return settings.ValuesSerializer.Deserialize<Dictionary<string, string>>(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ValuesManager] Unexpected error on Load.\nFilename:{settings.ValuesFileName}\nError{e}");
                return null;
            }
        }

        private static async Task<Dictionary<string, string>> LoadValuesAsync()
        {
            try
            {
                string filename = FilenameCleaner.Clean(settings.ValuesFileName);
                string data = await settings.Storage.LoadAsync(filename);
                if (string.IsNullOrEmpty(data))
                    return null;
                if (settings.Encrypter != null)
                    data = settings.Encrypter.Decrypt(data);
                return settings.ValuesSerializer.Deserialize<Dictionary<string, string>>(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ValuesManager] Unexpected error on Load.\nFilename:{settings.ValuesFileName}\nError{e}");
                return null;
            }
        }

        #endregion

        #region SaveValues

        public static void SaveValues()
        {
            try
            {
                string filename = FilenameCleaner.Clean(settings.ValuesFileName);
                string data = settings.ValuesSerializer.Serialize<Dictionary<string, string>>(valueBuffer);
                if (settings.Encrypter != null)
                    data = settings.Encrypter.Encrypt(data);
                settings.Storage.Save(filename, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ValuesManager] Unexpected error on Save.\nFilename:{settings.ValuesFileName}\nError{e}");
            }
        }

        public static async Task SaveValuesAsync()
        {
            try
            {
                string filename = FilenameCleaner.Clean(settings.ValuesFileName);
                string data = settings.ValuesSerializer.Serialize<Dictionary<string, string>>(valueBuffer);
                if (settings.Encrypter != null)
                    data = settings.Encrypter.Encrypt(data);
                await settings.Storage.SaveAsync(filename, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ValuesManager] Unexpected error on Save.\nFilename:{settings.ValuesFileName}\nError{e}");
            }
        }

        private static void AutoSaveValues()
        {
            if (!settings.AutoSaveAfterSet)
                return;

            if (settings.AutoSaveMode == InitializationMode.Sync)
                SaveValues();
            else
                SaveValuesAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        Debug.LogError($"[ValuesManager] AutoSave async failed: {t.Exception?.GetBaseException().Message}");
                });
        }

        #endregion

        #region Int

        public static void SetInt(string key, int value)
        {
            valueBuffer[key] = value.ToString(CultureInfo.InvariantCulture);
            AutoSaveValues();
        }

        public static int GetInt(string key, int defaultValue)
        {
            if (valueBuffer.TryGetValue(key, out string valueStr) && int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                return value;

            if(settings.CreateKeyValueIfNotFound)
                SetInt(key, defaultValue);
            return defaultValue;
        }

        #endregion

        #region Float

        public static void SetFloat(string key, float value)
        {
            valueBuffer[key] = value.ToString(CultureInfo.InvariantCulture);
            AutoSaveValues();
        }

        public static float GetFloat(string key, float defaultValue)
        {
            if (valueBuffer.TryGetValue(key, out string valueStr) && float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                return value;

            if(settings.CreateKeyValueIfNotFound)
                SetFloat(key, defaultValue);
            return defaultValue;
        }

        #endregion

        #region Double

        public static void SetDouble(string key, double value)
        {
            valueBuffer[key] = value.ToString(CultureInfo.InvariantCulture);
            AutoSaveValues();
        }

        public static double GetDouble(string key, double defaultValue)
        {
            if (valueBuffer.TryGetValue(key, out string valueStr) && double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return value;

            if(settings.CreateKeyValueIfNotFound)
                SetDouble(key, defaultValue);
            return defaultValue;
        }

        #endregion

        #region Bool

        public static void SetBool(string key, bool value)
        {
            valueBuffer[key] = value.ToString();
            AutoSaveValues();
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            if (valueBuffer.TryGetValue(key, out string valueStr) && bool.TryParse(valueStr, out bool value))
                return value;

            if(settings.CreateKeyValueIfNotFound)
                SetBool(key, defaultValue);
            return defaultValue;
        }

        #endregion

        #region String

        public static void SetString(string key, string value)
        {
            valueBuffer[key] = value;
            AutoSaveValues();
        }

        public static string GetString(string key, string defaultValue)
        {
            if (valueBuffer.TryGetValue(key, out string value))
                return value;

            if(settings.CreateKeyValueIfNotFound)
                SetString(key, defaultValue);
            return defaultValue;
        }

        #endregion
    }
}
