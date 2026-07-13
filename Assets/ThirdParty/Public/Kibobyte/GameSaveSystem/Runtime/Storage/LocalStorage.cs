using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace GameSaveSystem.Storage
{
    [CreateAssetMenu(menuName = "Game Save System/Storage/Local", fileName = "LocalStorage", order = 0)]
    public class LocalStorage : Storage
    {
        [SerializeField] private string folderName = "Saves";
        [SerializeField] private string defaultExtension = "";

        public override void Save(string savename, string data)
        {
            string path = GetPath(savename);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, data);
        }

        public override async Task SaveAsync(string savename, string data)
        {
            string path = GetPath(savename);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(path, data);
        }

        public override string Load(string savename)
        {
            string path = GetPath(savename);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[LocalSaveStorage] Save not found: {path}");
                return null;
            }

            return File.ReadAllText(path);
        }

        public override async Task<string> LoadAsync(string savename)
        {
            string path = GetPath(savename);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[LocalSaveStorage] Save not found: {path}");
                return null;
            }

            return await File.ReadAllTextAsync(path);
        }

        public override void Delete(string savename)
        {
            string path = GetPath(savename);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.LogWarning($"[LocalSaveStorage] Cannot delete, save not found: {path}");
            }
        }

        public override Task DeleteAsync(string savename)
        {
            Delete(savename);
            return Task.CompletedTask;
        }

        private string GetPath(string savename)
        {
            string extension = string.IsNullOrEmpty(defaultExtension) ? "" : $".{defaultExtension}";
            string relativePath = Path.Combine(folderName, $"{savename}{extension}");
            return Path.Combine(Application.persistentDataPath, relativePath);
        }
    }
}
