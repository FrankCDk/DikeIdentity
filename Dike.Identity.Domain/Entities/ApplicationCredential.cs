using Dike.Identity.Domain.Enums;

namespace Dike.Identity.Domain.Entities
{
    public class ApplicationCredential
    {
        public Guid Id { get; set; }

        // Relación con Application
        public Guid ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        // Identificación del cliente
        public string Name { get; set; } = string.Empty;
        public ApplicationClientType ClientType { get; set; }
        public ClientClassification Classification { get; set; }

        public string ClientId { get; set; } = string.Empty;

        // Solamente para clientes confidenciales
        public string? ClientSecretHash { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
