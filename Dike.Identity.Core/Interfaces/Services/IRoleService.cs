using Dike.Identity.Core.DTOs.Role;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IRoleService
    {
        Task RegisterAsync(RoleRegisterRequest request);
    }
}
