using Dike.Identity.Core.Entities;

namespace Dike.Identity.Core.Interfaces.Repositories
{
    public interface IAuditRepository
    {
        Task AddAssync(AuditLog log);
    }
}
