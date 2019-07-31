using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Services
{
    /// <summary>
    /// https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] saltAndHashBytes = new byte[36];
            Array.Copy(salt, 0, saltAndHashBytes, 0, 16);
            Array.Copy(hash, 0, saltAndHashBytes, 16, 20);

            return Convert.ToBase64String(saltAndHashBytes);
        }

        public bool ValidatePassword(string password, string savedPasswordSaltAndHash)
        {
            // Extract the bytes
            byte[] saltAndHashBytes = Convert.FromBase64String(savedPasswordSaltAndHash);
            // Get the salt
            byte[] salt = new byte[16];
            Array.Copy(saltAndHashBytes, 0, salt, 0, 16);
            // Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            // Get the hash
            var savedHash = saltAndHashBytes.AsSpan<byte>(16, 20);
            // Compare hashes
            return savedHash.SequenceCompareTo(hash) == 0;
        }
    }
}
