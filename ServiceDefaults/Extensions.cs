using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace B2Connect.ServiceDefaults;

/// <summary>
/// Health check configuration options for B2Connect services.
/// Configure via IConfiguration section "HealthChecks".
/// </summary>
public class HealthCheckOptions
{
    public string? PostgresConnectionString { get; set; }
    public string? RedisConnectionString { get; set; }
    public string? ElasticsearchUri { get; set; }
    public string? RabbitMqConnectionString { get; set; }
    public string[]? DependencyUris { get; set; }
}

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
        // app.UseLongRunningRequestDetection(); // Temporarily disabled for debugging

        // Standard health check endpoints per ADR-025
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    /// <summary>
    /// Writes health check response as JSON with detailed status per ADR-025.
    /// </summary>
    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                tags = e.Value.Tags
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }

    /// <summary>
    /// Adds PostgreSQL health check for database connectivity.
    /// Connection string from configuration: "ConnectionStrings:postgres" or explicit.
    /// </summary>
    public static IHealthChecksBuilder AddPostgresHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        string? connectionStringName = null)
    {
        var connectionString = connectionStringName != null
            ? configuration.GetConnectionString(connectionStringName)
            : configuration.GetConnectionString("postgres")
              ?? configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddNpgSql(
                connectionString,
                name: "postgresql",
                tags: ["ready", "database"],
                timeout: TimeSpan.FromSeconds(5));
        }

        return builder;
    }

    /// <summary>
    /// Adds Redis health check for cache connectivity.
    /// Connection string from configuration: "ConnectionStrings:redis" or explicit.
    /// </summary>
    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        string? connectionStringName = null)
    {
        var connectionString = connectionStringName != null
            ? configuration.GetConnectionString(connectionStringName)
            : configuration.GetConnectionString("redis");

        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddRedis(
                connectionString,
                name: "redis",
                tags: ["ready", "cache"],
                timeout: TimeSpan.FromSeconds(3));
        }

        return builder;
    }

    /// <summary>
    /// Adds Elasticsearch health check for search service connectivity.
    /// URI from configuration: "Elasticsearch:Uri" or "ConnectionStrings:elasticsearch".
    /// </summary>
    public static IHealthChecksBuilder AddElasticsearchHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var elasticUri = configuration["Elasticsearch:Uri"]
            ?? configuration.GetConnectionString("elasticsearch");

        if (!string.IsNullOrEmpty(elasticUri))
        {
            builder.AddElasticsearch(
                elasticUri,
                name: "elasticsearch",
                tags: ["ready", "search"],
                timeout: TimeSpan.FromSeconds(5));
        }

        return builder;
    }

    /// <summary>
    /// Adds RabbitMQ health check for message broker connectivity.
    /// Connection string from configuration: "ConnectionStrings:rabbitmq" or "RabbitMQ:ConnectionString".
    /// </summary>
    public static IHealthChecksBuilder AddRabbitMqHealthCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("rabbitmq")
            ?? configuration["RabbitMQ:ConnectionString"];

        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.AddRabbitMQ(
                name: "rabbitmq",
                tags: ["ready", "messaging"],
                timeout: TimeSpan.FromSeconds(5));
        }

        return builder;
    }

    /// <summary>
    /// Adds health checks for upstream service dependencies via HTTP.
    /// Services resolved via Aspire service discovery.
    /// </summary>
    public static IHealthChecksBuilder AddUpstreamServiceHealthCheck(
        this IHealthChecksBuilder builder,
        string serviceName,
        string healthEndpoint = "/health")
    {
        builder.AddUrlGroup(
            new Uri($"http://{serviceName}{healthEndpoint}"),
            name: serviceName,
            tags: ["ready", "upstream"],
            timeout: TimeSpan.FromSeconds(5));

        return builder;
    }

    /// <summary>
    /// Adds all standard infrastructure health checks based on configuration.
    /// Automatically detects configured services.
    /// </summary>
    public static IHealthChecksBuilder AddInfrastructureHealthChecks(
        this IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        return builder
            .AddPostgresHealthCheck(configuration)
            .AddRedisHealthCheck(configuration)
            .AddElasticsearchHealthCheck(configuration)
            .AddRabbitMqHealthCheck(configuration);
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
        return app.UseMiddleware<LongRunningRequestMiddleware>(warningThreshold, criticalThreshold);
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
                    })
                    .AddQuartzInstrumentation(options =>
                    {
                        // Record job exceptions in traces
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
