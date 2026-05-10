using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Providers.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _dbContext;

    public RefreshTokenRepository(IdentityDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.Set<RefreshToken>().AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _dbContext.Set<RefreshToken>().Update(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllByUserIdAsync(Guid userId)
    {
        await _dbContext.Set<RefreshToken>()
            .Where(t => t.UserId == userId)
            .ExecuteDeleteAsync(); // Borrado eficiente en .NET 8/10
    }
}