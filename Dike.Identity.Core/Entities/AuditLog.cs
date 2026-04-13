using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ApplicationId { get; set; }
        public string Action { get; set; } = string.Empty;
        public LogSeverity Severity { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        // --- PROPIEDADES DE NAVEGACIÓN ---
        // Nos permiten ir del Log -> al Usuario o a la App
        public virtual User? User { get; set; }
        public virtual Application? Application { get; set; }

    }
}
