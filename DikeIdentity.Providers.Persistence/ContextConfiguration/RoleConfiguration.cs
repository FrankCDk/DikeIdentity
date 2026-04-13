using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.Code)
                .HasColumnName("code")
                .HasColumnType("char(10)")
                .IsRequired();

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(e => e.NormalizedName)
                .HasColumnName("normalized_name")
                .HasMaxLength(80)
                .IsRequired();

            builder.HasIndex(e => e.NormalizedName)
                .HasDatabaseName("ix_roles_normalized_name")
                .IsUnique();


            builder.Property(e => e.Description)
                .HasColumnName("description");

            builder.Property(e => e.IsDefault)
                .HasColumnName("is_default")
                .HasDefaultValue(false);

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("state_type")
                .HasDefaultValue(StateStatus.active)
                .IsRequired();

            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.CreatedBy).HasColumnName("created_by");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");


        }
    }
}
