using Dike.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("permissions");

            builder.HasKey(x => x.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.Action)
                .HasColumnName("action")
                .HasColumnType("action_type")
                .IsRequired();

            builder.Property(e => e.Resource)
                .HasColumnName("resource")
                .HasColumnType("resource_type")
                .IsRequired();

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100);                

            builder.Property(e => e.NormalizedName)
                .HasColumnName("normalized_name")
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasColumnName("description");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("state_type")
                .IsRequired();

            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasIndex(e => e.NormalizedName).IsUnique();
        }
    }
}
