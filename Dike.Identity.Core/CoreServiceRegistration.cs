using Dike.Identity.Core.Interfaces.Services;
using Dike.Identity.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Core
{
    public static class CoreServiceRegistration
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            return services;
        }
    }
}
