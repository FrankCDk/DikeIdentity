using Dike.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Providers.Persistence
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options){}

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationCorsOrigins> ApplicationCorsOrigins { get; set; }
        public DbSet<ApplicationRedirectUris> ApplicationRedirectUris { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserApplication> UserApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Crear en un archivo aparte para que no se vea tan saturado el DbContext
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
