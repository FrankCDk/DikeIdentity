using Dike.Identity.Core.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace Dike.Identity.Providers.Security
{
    public class DefaultPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
            => _hasher.HashPassword(new object(), password);

        public bool VerifyPassword(string password, string hashedPassword)
            => _hasher.VerifyHashedPassword(new object(), hashedPassword, password) != PasswordVerificationResult.Failed;
    }
}
