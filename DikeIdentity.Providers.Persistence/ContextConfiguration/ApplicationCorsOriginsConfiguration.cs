using Dike.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class ApplicationCorsOriginsConfiguration : IEntityTypeConfiguration<ApplicationCorsOrigins>
    {
        public void Configure(EntityTypeBuilder<ApplicationCorsOrigins> builder)
        {
            builder.ToTable("application_cors_origins");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.ApplicationId)
                .HasColumnName("application_id")
                .IsRequired();

            builder.Property(x => x.OriginUrl)
                .HasColumnName("origin_url")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("LOCALTIMESTAMP");

            // Indice
            builder.HasIndex(x => x.ApplicationId)
                .HasDatabaseName("idx_app_cors_origin");

            // Relaciones -> una aplicacion con muchas cors origins
            builder.HasOne(x => x.Application)
                .WithMany(x => x.CorsOrigins)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
