using System.Net;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {

        IPAddress ParsearIpSegura(string ipString)
        {
            return IPAddress.TryParse(ipString, out var ip) ? ip : IPAddress.Loopback;
        }

        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()")
                .IsRequired();

            builder.Property(l => l.ApplicationId)
                .HasColumnName("application_id");

            builder.Property(l => l.UserId)
                .HasColumnName("user_id");

            builder.Property(l => l.Action)
                .HasColumnName("action")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(l => l.Severity)
                .HasColumnName("severity")
                .HasColumnType("log_severity")
                .HasDefaultValue(LogSeverity.info)
                .IsRequired();

            builder.Property(l => l.Details)
                .HasColumnName("details")
                .HasColumnType("jsonb");

            var ipConverter = new ValueConverter<string, IPAddress>(
                v => ParsearIpSegura(v),  // De C# (string) a la DB (IPAddress)
                v => v.ToString()         // De la DB (IPAddress) a C# (string)
            );


            builder.Property(l => l.IpAddress)
                .HasColumnName("ip_address")
                .HasColumnType("inet")
                .HasConversion(ipConverter);

            builder.Property(l => l.UserAgent)
                .HasColumnName("user_agent");

            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(l => l.User)
                .WithMany(l => l.AuditLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(l => l.Application)
                .WithMany(a => a.AuditLogs)
                .HasForeignKey(l => l.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(l => l.UserId).HasDatabaseName("IX_audit_logs_user_id");
            builder.HasIndex(l => l.Action).HasDatabaseName("IX_audit_logs_action");
            builder.HasIndex(l => l.CreatedAt).HasDatabaseName("IX_audit_logs_created_at");

            builder.HasIndex(l => l.Details)
            .HasMethod("GIN")
            .HasDatabaseName("IX_audit_logs_details");
        }
    }
}
