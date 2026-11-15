using Dike.Identity.Domain.Enums;

namespace Dike.Identity.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ApplicationType Type { get; set; }

        public DateTime ProjectCreatedAt { get; set; }

        public int MaxUsers { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Relación
        public ICollection<ApplicationCredential> Credentials { get; set; } = new List<ApplicationCredential>();
    }
}
