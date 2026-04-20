using Serilog;

namespace Dike.Identity.Api.Configurations
{
    public static class SerilogServiceRegistration
    {
        public static void AddSerilogConfiguration(this ConfigureHostBuilder host, IConfiguration configuration)
        {
            host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(configuration);

                loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName();

            });
        }
    }
}
