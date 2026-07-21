namespace Dike.Identity.Core.DTOs.Auth
{
    public record LoginRequest(string Email, string Password, Guid ClientId);
}
