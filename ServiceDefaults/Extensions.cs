using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Microsoft.Extensions.ServiceDiscovery;

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

        builder.ConfigureServices((context, services) =>
        {
            // Add Service Discovery
            services.AddServiceDiscovery();

            // Configure HttpClient defaults with Service Discovery
            services.ConfigureHttpClientDefaults(http =>
            {
                // Enable service discovery for all HttpClients
                http.AddServiceDiscovery();

                // Add standard resilience policies
                http.AddStandardResilienceHandler();
            });

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
        // Add long-running request detection early in the pipeline
        app.UseLongRunningRequestDetection();

        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }

    /// <summary>
    /// Adds middleware to detect and log long-running requests.
    /// Default thresholds: Warning at 3s, Critical at 10s.
    /// </summary>
    public static IApplicationBuilder UseLongRunningRequestDetection(
        this IApplicationBuilder app,
        TimeSpan? warningThreshold = null,
        TimeSpan? criticalThreshold = null)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<LongRunningRequestMiddleware>>();
        return app.UseMiddleware<LongRunningRequestMiddleware>(logger, warningThreshold, criticalThreshold);
    }

    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Add OpenTelemetry configuration
        builder.AddOpenTelemetry();

        return builder;
    }

    public static IHostApplicationBuilder AddOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "b2connect";
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                // Add console exporter for development
                if (builder.Environment.IsDevelopment())
                {
                    metrics.AddConsoleExporter();
                }

                // Add OTLP exporter if endpoint is configured (e.g., Aspire Dashboard)
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    metrics.AddOtlpExporter();
                }
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        // Enrich traces with request duration for identifying slow requests
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.request.content_length", request.ContentLength);
                        };
                        options.EnrichWithHttpResponse = (activity, response) =>
                        {
                            activity.SetTag("http.response.content_length", response.ContentLength);
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });

                // Add console exporter for development
                if (builder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                // Add OTLP exporter if endpoint is configured (e.g., Aspire Dashboard)
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    tracing.AddOtlpExporter();
                }
            });

        return builder;
    }
}
