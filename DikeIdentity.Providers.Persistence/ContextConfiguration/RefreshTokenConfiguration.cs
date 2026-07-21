using Dike.Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dike.Identity.Providers.Persistence.ContextConfiguration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("user_refresh_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(t => t.ApplicationId)
            .HasColumnName("application_id")
            .IsRequired();

        builder.Property(t => t.Token)
            .HasColumnName("token")
            .IsRequired();

        builder.Property(t => t.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestampt")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestampt")
            .HasDefaultValueSql("LOCALTIMESTAMP");

        builder.Property(t => t.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestampt");

        // Relación con la tabla Users
        builder.HasOne(t => t.User)
            .WithMany() // Un usuario puede tener muchos refresh tokens (varias sesiones)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relación con la tabla Applications
        builder.HasOne(t => t.Application)
            .WithMany()
            .HasForeignKey(t => t.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices Mapeados de tu SQL
        builder.HasIndex(t => t.Token)
            .HasDatabaseName("idx_user_refresh_tokens_token")
            .IsUnique();

        // ◄ NUEVO: Agregamos el índice compuesto para búsquedas óptimas por Usuario y App
        builder.HasIndex(t => new { t.UserId, t.ApplicationId })
            .HasDatabaseName("idx_user_refresh_tokens_user_app");
    }
}