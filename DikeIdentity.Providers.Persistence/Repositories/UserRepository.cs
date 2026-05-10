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

        public async Task<bool> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
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

        public async Task<bool> FailedLoginAttemptAsync(Guid userId, int maxAttempts = 5, int lockoutMinutes = 15)
        {
            var now = DateTime.UtcNow;

            // Actualizamos directamente en la base de datos sin traer la entidad completa primero
            // Esto genera un solo comando SQL: UPDATE users SET failed_login_attempts = failed_login_attempts + 1...
            await _dbContext.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(u => u.FailedLoginAttempts, u => u.FailedLoginAttempts + 1)
                    .SetProperty(u => u.UpdatedAt, now));

            // Consultamos solo lo necesario para validar el bloqueo
            var userStatus = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.FailedLoginAttempts })
                .FirstOrDefaultAsync();

            if (userStatus != null && userStatus.FailedLoginAttempts >= maxAttempts)
            {
                // Aplicamos el bloqueo si superó el umbral
                await _dbContext.Users
                    .Where(u => u.Id == userId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(u => u.IsLocked, true)
                        .SetProperty(u => u.LockoutEnd, now.AddMinutes(lockoutMinutes)));

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

    }
}
