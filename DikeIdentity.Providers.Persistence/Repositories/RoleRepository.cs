using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Providers.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _dbContext;

        public RoleRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> FindByCodeAsync(string code)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Code == code);
        }

        public async Task<Role?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task RegisterAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
        }
        
    }
}
