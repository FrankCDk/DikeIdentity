namespace Dike.Identity.Core.DTOs.Application
{
    public record UpdateCorsOriginsRequest(Guid ApplicationId, List<string> NewOrigins);    
}
