using Dike.Identity.Core.Interfaces.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Providers.Jwt
{
    public static class JwtServiceRegistration
    {
        public static IServiceCollection AddJwtInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IJwtProvider, JwtProvider>();
            return services;
        }
    }
}
