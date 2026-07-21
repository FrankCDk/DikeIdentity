namespace Dike.Identity.Core.DTOs.Application
{
    public record UpdateRedirectUrisRequest(
            Guid ApplicationId,
            List<RedirectUriItem> NewRedirectUris
        );

    public record RedirectUriItem(string Uri, string? Description);
}
