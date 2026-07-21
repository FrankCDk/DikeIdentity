using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Response<AuthResponse>> LoginStandardAsync(LoginRequest request);
        Task<Response<AuthResponse>> LoginWithArgon2Async(LoginRequest request);
        Task<Response<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}