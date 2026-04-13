namespace Dike.Identity.Core.Entities
{
    public class RolePermission
    {
        // Las llaves foráneas
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        // Propiedades de navegación (Opcionales, pero recomendadas para joins rápidos)
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
