using Dike.Identity.Application.Interfaces;
using Dike.Identity.Infrastructure.Entities;
using Dike.Identity.Infrastructure.Persistence;
using Dike.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContextFactory<UsersDbContext>(
    options => options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")),
    lifetime: ServiceLifetime.Scoped);

            services.AddDataProtection();

            // ASP.NET Core Identity
            services.AddIdentityCore<UserIdentity>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }

}
