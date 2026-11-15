
using System.Security.Claims;
using Dike.Identity.Application.Interfaces;
using Dike.Identity.Application.Models.Auditing;
using Dike.Identity.Domain.Entities;
using Dike.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly UsersDbContext _context;
        private readonly IDbContextFactory<UsersDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(
            UsersDbContext context,
            IDbContextFactory<UsersDbContext> contextFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // Agrega el evento a la transacción principal
        public Task RegisterAsync(
            AuditEventRequest request,
            CancellationToken cancellationToken = default)
        {
            var auditLog = CreateAuditLog(request);

            _context.AuditLogs.Add(auditLog);

            return Task.CompletedTask;
        }

        // Persiste el evento mediante un contexto independiente
        public async Task RegisterAndSaveAsync(
            AuditEventRequest request,
            CancellationToken cancellationToken = default)
        {
            var auditLog = CreateAuditLog(request);

            await using var context =
                await _contextFactory.CreateDbContextAsync(cancellationToken);

            context.AuditLogs.Add(auditLog);

            await context.SaveChangesAsync(cancellationToken);
        }

        private AuditLog CreateAuditLog(AuditEventRequest request)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            Guid? userId = null;

            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                var identifier = httpContext.User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (Guid.TryParse(identifier, out var parsedId))
                {
                    userId = parsedId;
                }
            }

            return new AuditLog
            {
                Id = Guid.NewGuid(),

                EventType = request.EventType,
                Action = request.Action,
                Description = request.Description,

                EntityName = request.EntityName,
                EntityId = request.EntityId,
                UserId = userId,

                RequestId = httpContext?.TraceIdentifier,
                HttpMethod = httpContext?.Request.Method,
                RequestPath = httpContext?.Request.Path.Value,
                IpAddress = httpContext?.Connection
                    .RemoteIpAddress?.ToString(),
                UserAgent = httpContext?.Request.Headers
                    .UserAgent.ToString(),

                IsSuccessful = request.IsSuccessful,
                StatusCode = request.StatusCode,
                AdditionalData = request.AdditionalData,

                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
