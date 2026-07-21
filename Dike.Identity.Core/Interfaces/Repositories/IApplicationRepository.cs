using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Repositories
{
    public interface IApplicationRepository
    {
        Task<Application?> GetByIdAsync(Guid applicationId);
        Task<Application?> GetByCodeAsync(string code);


        Task AddAsync(Application application);
        Task UpdateAsync(Application application);

        Task<bool> ExistsByCodeAsync(string code);
        Task<Application?> GetByIdWithRedirectUrisAsync(Guid Id);
        Task<Application?> GetByIdWithCorsAsync(Guid Id);
        Task<bool> IsRedirectUriValidAsync(Guid applicationId, string redirectUri);
        Task<bool> IsCorsOriginValidAsync(Guid applicationId, string originUrl);
    }
}
