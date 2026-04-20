using Dike.Identity.Core.DTOs.Auth;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IAuthService
    {
        // Login
        Task<AuthResponse> LoginStandardAsync(LoginRequest request);
        Task<AuthResponse> LoginWithArgon2Async(LoginRequest request);

        // Registro
        Task<bool> RegisterStandardAsync(RegisterRequest request);
        Task<bool> RegisterWithArgon2Async(RegisterRequest request);
    }
}
