namespace Dike.Identity.Core.DTOs.Auth
{
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime Expiration);
}
