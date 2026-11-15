using Dike.Identity.Application.Models.Auditing;

namespace Dike.Identity.Application.Interfaces
{
    public interface IAuditService
    {
        Task RegisterAsync(
                    AuditEventRequest request,
                    CancellationToken cancellationToken = default);

        Task RegisterAndSaveAsync(
            AuditEventRequest request,
            CancellationToken cancellationToken = default);
    }
}
