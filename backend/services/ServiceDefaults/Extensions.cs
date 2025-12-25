using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace B2Connect.ServiceDefaults;

public static class Extensions
{
    public static IHostBuilder AddServiceDefaults(this IHostBuilder builder)
    {
        builder.UseSerilog((context, configuration) =>
            configuration
                .MinimumLevel.Information()
                .WriteTo.Console()
                .Enrich.FromLogContext()
        );

        builder.ConfigureServices(services =>
        {
            // Health checks
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            services.AddLogging(logging =>
            {
                logging.AddConsole();
            });
        });

        return builder;
    }

    public static IApplicationBuilder UseServiceDefaults(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }
}
