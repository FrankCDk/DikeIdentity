using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAllByUserIdAsync(Guid userId); // Útil para desloguear de todos los dispositivos
}