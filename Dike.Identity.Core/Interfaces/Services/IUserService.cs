using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Response<Guid>> RegisterStandardAsync(RegisterRequest request);
        Task<Response<Guid>> RegisterWithArgon2Async(RegisterRequest request);
    }
}