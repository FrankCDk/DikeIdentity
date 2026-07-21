namespace Dike.Identity.Core.DTOs.Application
{
    public record ApplicationSyncRequest(Guid ClientId, string ClientSecret);

    public record ApplicationSyncResponse(
        Guid Id,
        string Code,
        string Name,
        string SecretHash,
        List<string> CorsOrigins,
        List<string> RedirectUris
    );

}
