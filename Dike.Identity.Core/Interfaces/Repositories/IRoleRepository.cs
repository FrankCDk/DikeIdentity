using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task RegisterAsync(Role role);
        Task UpdateAsync(Role role);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> FindByIdAsync(Guid id);
        Task<Role?> FindByCodeAsync(string code);
    }
}
