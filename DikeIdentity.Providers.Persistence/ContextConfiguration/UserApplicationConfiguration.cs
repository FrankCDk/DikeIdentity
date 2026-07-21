using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplication>
    {
        public void Configure(EntityTypeBuilder<UserApplication> builder)
        {
            builder.ToTable("user_applications");

            builder.HasKey(u => new { u.UserId, u.ApplicationId });

            builder.Property(u => u.UserId)
                .HasColumnName("user_id");

            builder.Property(u => u.ApplicationId)
                .HasColumnName("application_id");

            builder.Property(u => u.RoleId)
                .HasColumnName("role_id");

            builder.Property(u => u.Status)
                .HasColumnName("status")
                .HasColumnType("state_type")
                .HasDefaultValue(StateStatus.active)
                .IsRequired();

            builder.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.AssignedBy).HasColumnName("assigned_by");

            // Relaciones
            builder.HasOne(u => u.User)
                .WithMany(u => u.UserApplications)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Application)
                .WithMany(u => u.UserApplications)
                .HasForeignKey(u => u.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Role)
                .WithMany(u => u.UserApplications)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
