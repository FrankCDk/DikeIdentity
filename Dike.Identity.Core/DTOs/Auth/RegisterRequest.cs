namespace Dike.Identity.Core.DTOs.Auth
{
    public record RegisterRequest(
        string Email,
        string Password,
        string Name,
        string LastName,
        Guid ApplicationId,
        Guid RoleId
    );
}
