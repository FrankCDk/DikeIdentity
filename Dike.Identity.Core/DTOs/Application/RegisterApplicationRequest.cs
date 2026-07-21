namespace Dike.Identity.Core.DTOs.Application
{
    public record RegisterApplicationRequest
    (
        string Code,
        string Name,
        List<RegisterRedirectUriRequest> RedirectUris,
        List<string> CorsOrigins
    );

    public record RegisterRedirectUriRequest(
        string Uri,
        string Description
    );

    public record RegisterApplicationResponse
    (
        Guid Id,
        string ClientSecret // Este valor viaja al frontend para que el admin lo copie
    );
}
