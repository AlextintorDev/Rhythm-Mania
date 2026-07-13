using System.Threading.Tasks;
using UnityEngine;

namespace GameSaveSystem.Storage
{
    public abstract class Storage : ScriptableObject
    {
        public abstract void Save(string savename, string data);
        public abstract Task SaveAsync(string savename, string data);
        public abstract string Load(string savename);
        public abstract Task<string> LoadAsync(string savename);
        public abstract void Delete(string savename);
        public abstract Task DeleteAsync(string savename);
    }
}
