using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> FailedLoginAttemptAsync(Guid userId, int maxAttempts = 5, int lockoutMinutes = 15);
    }
}
