using System.Security.Claims;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Dike.Identity.Providers.Persistence.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IAuditRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SaveAuditAsync(string action, LogSeverity severity, object? details = null, Guid? appId = null)
        {
            var context = _httpContextAccessor.HttpContext;

            // Recolectamos información del contexto actual
            var ipAddress = context?.Connection.RemoteIpAddress?.ToString() ?? "unknown"; // Obtenemos la IP
            var userAgent = context?.Request.Headers["User-Agent"].ToString() ?? "unknown"; // Obtenemos el User-Agent

            // Intentamos obtener el ID del usuario autenticado, si existe
            Guid? userId = null;
            var userIdClaim = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(Guid.TryParse(userIdClaim, out var parsedId)) userId = parsedId;

            // Creamos el registro de auditoría
            var log = new AuditLog
            {
                Action = action,
                Severity = severity,
                Details = details != null ? System.Text.Json.JsonSerializer.Serialize(details) : null,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                UserId = userId,
                ApplicationId = appId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repository.AddAssync(log);

        }
    }
}
