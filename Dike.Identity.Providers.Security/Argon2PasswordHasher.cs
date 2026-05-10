using System.Security.Cryptography;
using System.Text;
using Dike.Identity.Core.Interfaces.Security;
using Konscious.Security.Cryptography;

namespace Dike.Identity.Providers.Security
{

    /// <summary>
    /// Hasher de contraseña avanzado utilizando el algoritmo Argon2id, recomendado por OWASP para nuevas implementaciones.
    /// </summary>
    public class Argon2PasswordHasher : IPasswordHasher
    {
        // Configuraciones recomendadas por OWASP (ajustadas para el MVP)
        private const int DegreeOfParallelism = 8;
        private const int Iterations = 4;
        private const int MemorySize = 65536; // 64 MB
        private const int SaltSize = 16;      // 128 bits
        private const int HashSize = 32;      // 256 bits

        public string HashPassword(string password)
        {
            // 1. Generamos un salt aleatorio para cada contraseña
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // 2. Configuramos Argon2id
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize,
            };

            // 3. Obtenemos los bytes del hash
            byte[] hash = argon2.GetBytes(HashSize);

            // 4. Combinamos Salt + Hash en una sola cadena base64 para guardarla
            // Formaro: [Salt(16 bytes)][Hash(32 bytes)]
            byte[] combinedBytes = new byte[SaltSize + HashSize];

            Buffer.BlockCopy(salt, 0, combinedBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, combinedBytes, SaltSize, HashSize);
            return Convert.ToBase64String(combinedBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {

            // 1. Decodificamos la cadena base64 de la base de datos
            byte[] combinedBytes = Convert.FromBase64String(hashedPassword);

            // 2. Extraemos el Salt original (primeros 16 bytes)
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(combinedBytes, 0, salt, 0, SaltSize);

            // 3. Extraemos el Hash original (últimos 32 bytes)
            byte[] originalHash = new byte[HashSize];
            Buffer.BlockCopy(combinedBytes, SaltSize, originalHash, 0, HashSize);

            // 4. Generamos un nuevo hash con la contraseña ingresada y el mismo salt
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize,
            };

            byte[] newHash = argon2.GetBytes(HashSize);

            // 5. Comparación segura (Time-constant) para evitar ataques de tiempo
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
    }
}
