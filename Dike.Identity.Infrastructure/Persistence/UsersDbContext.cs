using Dike.Identity.Domain.Entities;
using Dike.Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dike.Identity.Infrastructure.Persistence
{
    public class UsersDbContext : IdentityDbContext<UserIdentity, IdentityRole<Guid>, Guid>
    {

        public DbSet<Domain.Entities.Application> Applications { get; set; }
        public DbSet<ApplicationCredential> ApplicationCredentials { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
