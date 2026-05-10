using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Services;
using Dike.Identity.Providers.Persistence.Repositories;
using Dike.Identity.Providers.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Providers.Persistence
{
    public static class PersistenceServiceRegistration
    {

        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            return services;
        }

    }
}
