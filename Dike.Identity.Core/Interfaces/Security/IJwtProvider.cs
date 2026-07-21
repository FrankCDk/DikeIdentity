using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Security
{
    public interface IJwtProvider
    {
        AuthResponse GenerateTokens(User user, string clientSecret, string keyId);
    }
}
