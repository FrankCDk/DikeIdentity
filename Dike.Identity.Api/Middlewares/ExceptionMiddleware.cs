using System.Net;
using System.Text.Json;
using Dike.Identity.Core.Common;
using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Services;

namespace Dike.Identity.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ha ocurrido una excepcion no controlada en: {Path}", context.Request.Path);

                await TryLogToDatabase(context, auditService, ex);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task TryLogToDatabase(HttpContext context, IAuditService auditService, Exception ex)
        {
            try
            {
                var details = new
                {
                    exceptionType = ex.GetType().Name,
                    stackTrace = ex.StackTrace,
                    path = context.Request.Path.Value,
                    method = context.Request.Method,
                    query = context.Request.QueryString.Value
                };

                await auditService.SaveAuditAsync(
                    action: "SYSTEM_EXCEPTION",
                    severity: LogSeverity.error,
                    details: details
                );
            }
            catch (Exception dbEx)
            {
                _logger.LogCritical(dbEx, "ERROR CRITICO: No se pudo registrar la excepcion en la base de datos. Detalles del error original: {OriginalError}", context.Request.Path);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message, internalCode, errors) = exception switch
            {

                // Caso: Errores de validación (FluentValidation)
                FluentValidation.ValidationException valEx => (
                    HttpStatusCode.BadRequest,
                    "Se encontraron errores de validación.",
                    InternalCodes.ValidationError,
                    valEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())
                ),

                // Caso: Errores de negocio creados por ti
                IdentityException idEx => (
                    HttpStatusCode.BadRequest,
                    idEx.Message,
                    idEx.InternalCode,
                    null
                ),

                // Caso: No encontrado
                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    "El recurso solicitado no existe.",
                    InternalCodes.NotFound,
                    null
                ),

                // Caso: Error catastrófico (Default)
                _ => (
                    HttpStatusCode.InternalServerError,
                    "Ocurrió un error inesperado en el servidor.", // Mensaje genérico por seguridad
                    InternalCodes.GenericError,
                    null
                )
            };

            context.Response.StatusCode = (int)statusCode;

            var response = BaseResponse<object>.Failure(internalCode, message, errors);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
