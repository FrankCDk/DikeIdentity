using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users"); 

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(u => u.NormalizedEmail)
                .HasColumnName("normalized_email")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash");

            builder.Property(u => u.AuthProvider)
                .HasColumnName("auth_provider")
                .HasColumnType("auth_provider_type")
                .HasDefaultValue(AuthProvider.local)
                .IsRequired();

            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnName("lastname")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(u => u.State)
                .HasColumnName("state")
                .HasColumnType("state_type")
                .HasDefaultValue(StateStatus.active)
                .IsRequired();

            builder.Property(u => u.EmailConfirmed)
                .HasColumnName("email_confirmed")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(u => u.FailedLoginAttempts)
                .HasColumnName("failed_login_attempts")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(u => u.IsLocked)
               .HasColumnName("is_locked")
               .HasColumnType("boolean")
               .HasDefaultValue(false)
               .IsRequired();

            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");


            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("IX_users_normalized_email").IsUnique();
        }
    }
}
