using System.Text;
using Konscious.Security.Cryptography;

namespace User.Common.PasswordService
{
    public class PasswordHashService
    {
        public string HashPassword(string password, Guid userId)
        {
            var salt = GenerateSalt();
            var byteArray = GetHash(password, salt, userId);

            return Convert.ToBase64String(byteArray);
        }

        public bool VerifyPassword(string password, string hash, Guid userId)
        {
            var hashOriginal = Convert.FromBase64String(hash);

            byte[] salt = new byte[128];
            Buffer.BlockCopy(hashOriginal, 0, salt, 0, 128);

            var hashNew = GetHash(password, salt, userId);
            return hashNew.SequenceEqual(hashOriginal);
        }

        private byte[] GetHash(string password, byte[] salt, Guid userId)
        {
            var argon = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon.Salt = salt;
            argon.DegreeOfParallelism = 4;
            argon.MemorySize = 1048576;
            argon.Iterations = 4;
            argon.AssociatedData = userId.ToByteArray();
            var hash = argon.GetBytes(128);

            return ConcatinateArrays(hash, salt);
        }

        private byte[] ConcatinateArrays(byte[] password, byte[] salt)
        {
            byte[] concat = new byte[256];
            Buffer.BlockCopy(salt, 0 , concat, 0, 128);
            Buffer.BlockCopy(password, 0, concat, 128, 128);

            return concat;
        }

        private byte[] GenerateSalt()
        {
            Random random = new();
            byte[] array = new byte[128];
            random.NextBytes(array);
            return array;
        }
    }
}