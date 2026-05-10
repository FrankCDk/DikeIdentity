using Dike.Identity.Core.DTOs.Auth;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginStandardAsync(LoginRequest request);
        Task<AuthResponse> LoginWithArgon2Async(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}