using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Services;

namespace Dike.Identity.Api.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuditMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            // 1. Extraemos los datos calientes del HTTP Context antes de que se destruya la petición
            var path = context.Request.Path.Value;
            var method = context.Request.Method;
            var queryString = context.Request.QueryString.Value;

            // Capturamos la IP y el UserAgent aquí mismo, en el hilo principal
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "unknown";

            // Intentamos extraer el usuario si ya está autenticado
            Guid? userId = null;
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var parsedId)) userId = parsedId;

            // 2. Dejamos que la petición siga su flujo normal hacia el cliente
            await _next(context);

            var statusCode = context.Response.StatusCode;

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                // Ejecutamos en segundo plano para no retrasar el cierre de la conexión
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

                        var details = new { path, method, statusCode, queryString };

                        await auditService.SaveAuditAsync(
                            action: $"HTTP_ACCESS_{method}",
                            severity: LogSeverity.info,
                            details: details,
                            appId: null,
                            overrideIp: ipAddress,
                            overrideUserAgent: userAgent,
                            overrideUserId: userId
                        );
                    }
                    catch
                    {
                        // Fallo silencioso en segundo plano: 
                        // No podemos usar ILogger aquí fácilmente porque Task.Run pierde el scope,
                        // pero el AuditService por dentro debería ser robusto.
                    }
                });
            }

        }

    }
}
