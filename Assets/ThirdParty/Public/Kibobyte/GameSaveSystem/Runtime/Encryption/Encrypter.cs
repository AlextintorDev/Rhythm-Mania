using UnityEngine;

namespace GameSaveSystem.Encryption
{
    public abstract class Encrypter : ScriptableObject
    {
        public abstract string Encrypt(string text);
        public abstract string Decrypt(string text);
    }
}
