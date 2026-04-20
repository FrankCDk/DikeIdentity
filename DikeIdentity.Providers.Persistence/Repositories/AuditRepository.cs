using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;

namespace Dike.Identity.Providers.Persistence.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly IdentityDbContext _context;

        public AuditRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddAssync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
