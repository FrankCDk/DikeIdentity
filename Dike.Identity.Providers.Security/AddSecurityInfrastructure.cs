using Dike.Identity.Core.Interfaces.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Providers.Security
{
    public static class SecurityServiceRegistration
    {
        public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services)
        {

            // Registramos Argon2id con la clave "Hardened"
            services.AddKeyedSingleton<IPasswordHasher, Argon2PasswordHasher>("Hardened");

            // Registramos el Hasher por defecto con la clave "Classic"
            services.AddKeyedSingleton<IPasswordHasher, DefaultPasswordHasher>("Classic");

            return services;
        }
    }
}
