using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Providers.Persistence.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IdentityDbContext _context;
        public ApplicationRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Applications.AnyAsync(a => a.Code == code);
        }

        public async Task<Application?> GetByCodeAsync(string code)
        {
            return await _context.Applications.FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task<Application?> GetByIdAsync(Guid applicationId)
        {
            return await _context.Applications
                .Include(a => a.CorsOrigins)
                .Include(a => a.RedirectUris)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<Application?> GetByIdWithCorsAsync(Guid Id)
        {
            return await _context.Applications
                .Include(a => a.CorsOrigins)
                .FirstOrDefaultAsync(a => a.Id == Id);
        }
        
        public async Task<Application?> GetByIdWithRedirectUrisAsync(Guid Id)
        {
            return await _context.Applications
                .Include(a => a.RedirectUris)
                .FirstOrDefaultAsync(a => a.Id == Id);
        }

        public async Task<bool> IsCorsOriginValidAsync(Guid applicationId, string originUrl)
        {
            return await _context.ApplicationCorsOrigins.AnyAsync(c => c.ApplicationId == applicationId && c.OriginUrl == originUrl);
        }

        public async Task<bool> IsRedirectUriValidAsync(Guid applicationId, string redirectUri)
        {
            return await _context.ApplicationRedirectUris.AnyAsync(r => r.ApplicationId == applicationId && r.RedirectUri == redirectUri);
        }

        public async Task UpdateAsync(Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}
