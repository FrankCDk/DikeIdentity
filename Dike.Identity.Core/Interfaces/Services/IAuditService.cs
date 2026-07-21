using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IAuditService
    {
        Task SaveAuditAsync(
            string action,
            LogSeverity severity,
            object? details = null,
            Guid? appId = null,
            string? overrideIp = null,      
            string? overrideUserAgent = null, 
            Guid? overrideUserId = null       
        );
    }
}
