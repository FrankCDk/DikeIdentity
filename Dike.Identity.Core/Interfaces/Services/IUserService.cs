using Dike.Identity.Core.DTOs.Auth;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Guid> RegisterStandardAsync(RegisterRequest request);
        Task<Guid> RegisterWithArgon2Async(RegisterRequest request);
    }
}