namespace Dike.Identity.Core.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string Token { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? RevokedAt { get; set; }


        // Propiedad de navegación (Opcional pero recomendada)
        public User? User { get; set; }
        public Application? Application { get; set; }

        // Helper para saber si el token sigue vigente
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}