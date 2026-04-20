using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Services;
using Dike.Identity.Providers.Persistence.Services;

namespace Dike.Identity.Api.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuditService auditService)
        {
            await _next(context);

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                // Ejecutamos en segundo plano para no retrasar el cierre de la conexión
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var details = new
                        {
                            path = context.Request.Path.Value,
                            method = context.Request.Method,
                            statusCode = context.Response.StatusCode,
                            queryString = context.Request.QueryString.Value
                        };

                        await auditService.SaveAuditAsync(
                            action: $"HTTP_ACCESS_{context.Request.Method}",
                            severity: LogSeverity.info,
                            details: details
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
