using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("applications");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(a => a.Code)
                .HasColumnName("code")
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.SecretHash)
                .HasColumnName("secret_hash")
                .IsRequired();

            builder.Property(a => a.RedirectUri)
                .HasColumnName("redirect_uri");

            builder.Property(a => a.Status)
                .HasColumnName("status")
                .HasColumnType("state_type")
                .HasDefaultValue(StateStatus.active)
                .IsRequired();

            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            builder.HasIndex(a => a.Code).IsUnique();
            builder.HasIndex(a => a.Name).IsUnique();
        }
    }
}
