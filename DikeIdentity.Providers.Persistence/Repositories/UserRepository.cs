using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Providers.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly IdentityDbContext _dbContext;

        public UserRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.NormalizedEmail == email.ToUpper());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users
                .FindAsync(id);
        }
    }
}
