using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Entities
{
    /// <summary>
    /// Tabla de permisos que define las acciones permitidas sobre los recursos del sistema. 
    /// Cada permiso se asocia a un recurso específico y una acción concreta, lo que permite 
    /// controlar el acceso de los usuarios a las funcionalidades del sistema de manera granular.
    /// </summary>
    public class Permission
    {
        public Guid Id { get; set; }
        public PermissionAction Action { get; set; }
        public PermissionResource Resource { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StateStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
