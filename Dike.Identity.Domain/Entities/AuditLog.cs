
namespace Dike.Identity.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }

        // Información del evento
        public string EventType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Recurso afectado
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }

        // Usuario responsable
        public Guid? UserId { get; set; }

        // Información de la solicitud HTTP
        public string? RequestId { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Resultado de la operación
        public bool IsSuccessful { get; set; }
        public int? StatusCode { get; set; }

        // Información adicional
        public string? AdditionalData { get; set; }

        // Fecha del evento
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
