using Dike.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Event information
            builder.Property(x => x.EventType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            // Affected entity
            builder.Property(x => x.EntityName)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(x => x.EntityId)
                .HasMaxLength(100)
                .IsRequired(false);

            // Responsible user
            builder.Property(x => x.UserId)
                .IsRequired(false);

            // HTTP request information
            builder.Property(x => x.RequestId)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(x => x.HttpMethod)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(x => x.RequestPath)
                .HasMaxLength(2048)
                .IsRequired(false);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45)
                .IsRequired(false);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(1024)
                .IsRequired(false);

            // Operation result
            builder.Property(x => x.IsSuccessful)
                .IsRequired();

            builder.Property(x => x.StatusCode)
                .IsRequired(false);

            // Additional information
            builder.Property(x => x.AdditionalData)
                .HasColumnType("jsonb")
                .IsRequired(false);

            // Audit date
            builder.Property(x => x.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.RequestId);

            builder.HasIndex(x => new
            {
                x.EntityName,
                x.EntityId
            });
        }
    }
}
