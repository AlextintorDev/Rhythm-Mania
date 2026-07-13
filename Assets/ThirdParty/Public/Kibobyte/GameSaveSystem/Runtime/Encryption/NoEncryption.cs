using UnityEngine;

namespace GameSaveSystem.Encryption
{
    [CreateAssetMenu(
        fileName = "NoEncryption",
        menuName = "Game Save System/Encryption/None", order = 0)]
    public class NoEncryption : Encrypter
    {
        public override string Encrypt(string _)
        {
            return _;
        }

        public override string Decrypt(string _)
        {
            return _;
        }
    }
}
