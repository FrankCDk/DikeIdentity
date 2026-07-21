using Dike.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration
{
    public class ApplicationRedirectUrisConfiguration : IEntityTypeConfiguration<ApplicationRedirectUris>
    {
        public void Configure(EntityTypeBuilder<ApplicationRedirectUris> builder)
        {
            builder.ToTable("application_redirect_uris");
            builder.HasKey(x => x.Id);
        
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.ApplicationId)
                .HasColumnName("application_id")
                .IsRequired();

            builder.Property(x => x.RedirectUri)
                .HasColumnName("redirect_uri")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("LOCALTIMESTAMP");

            builder.HasIndex(x => x.ApplicationId)
                .HasDatabaseName("idx_app_redirect_uri");

            builder.HasOne(x => x.Application)
                .WithMany(x => x.RedirectUris)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
