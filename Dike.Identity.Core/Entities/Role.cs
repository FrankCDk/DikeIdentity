using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public StateStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Relación Many-to-Many
        // Aca se define la colección de RolePermission que representa la relación entre Role y Permission.
        // No es necesario definir una colección de Permissions directamente en Role, ya que la relación se maneja a través de RolePermission.
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
    }
}
