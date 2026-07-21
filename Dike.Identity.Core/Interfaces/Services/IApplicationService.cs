using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Application;

namespace Dike.Identity.Core.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<Response<RegisterApplicationResponse>> RegisterApplicationAsync(RegisterApplicationRequest request);
        Task<Response<ApplicationSyncResponse>> SyncConfigurationAsync(ApplicationSyncRequest request);

        Task<Response<bool>> UpdateCorsOriginsAsync(UpdateCorsOriginsRequest request);
        Task<Response<bool>> UpdateRedirectUrisAsync(UpdateRedirectUrisRequest request);

    }
}
