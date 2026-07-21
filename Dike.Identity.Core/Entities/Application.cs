using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SecretHash { get; set; } = string.Empty;
        public StateStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<ApplicationCorsOrigins> CorsOrigins { get; set; } = new List<ApplicationCorsOrigins>();
        public ICollection<ApplicationRedirectUris> RedirectUris { get; set; } = new List<ApplicationRedirectUris>();
    }
}
