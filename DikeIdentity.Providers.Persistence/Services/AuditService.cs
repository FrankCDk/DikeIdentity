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

        public async Task SaveAuditAsync(
            string action,
            LogSeverity severity,
            object? details = null,
            Guid? appId = null,
            string? overrideIp = null,
            string? overrideUserAgent = null,
            Guid? overrideUserId = null)
        {
            string ipAddress;
            string userAgent;
            Guid? userId = null;

            // 🧠 SI EL MIDDLEWARE YA CAPTURÓ LOS DATOS, LOS USAMOS DIRECTO
            if (overrideIp != null)
            {
                ipAddress = overrideIp;
                userAgent = overrideUserAgent ?? "unknown";
                userId = overrideUserId;
            }
            else
            {
                // Flujo tradicional alternativo (Uso sínscrono desde controladores)
                var context = _httpContextAccessor.HttpContext;
                ipAddress = context?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                userAgent = context?.Request.Headers["User-Agent"].ToString() ?? "unknown";

                var userIdClaim = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var parsedId)) userId = parsedId;
            }

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
