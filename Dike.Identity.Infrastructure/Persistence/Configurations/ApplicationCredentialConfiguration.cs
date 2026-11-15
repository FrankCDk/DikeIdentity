using Dike.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Infrastructure.Persistence.Configurations
{
    public class ApplicationCredentialConfiguration
        : IEntityTypeConfiguration<ApplicationCredential>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationCredential> builder)
        {
            builder.ToTable("ApplicationCredentials");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ApplicationId)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ClientType)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Classification)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.ClientSecretHash)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.ExpiresAt)
                .IsRequired(false);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            // Auditoría
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .IsRequired(false);

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false);

            // Índices
            builder.HasIndex(x => x.ClientId)
                .IsUnique();

            builder.HasIndex(x => x.ApplicationId);

            // Relación
            builder.HasOne(x => x.Application)
                .WithMany(x => x.Credentials)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
