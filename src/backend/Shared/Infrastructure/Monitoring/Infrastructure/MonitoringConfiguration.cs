using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace B2X.Shared.Monitoring.Infrastructure;

/// <summary>
/// Configuration class for monitoring infrastructure including OpenTelemetry setup.
/// </summary>
public class MonitoringConfiguration
{
    private const string MeterName = "B2X.Metrics";
    private const string ActivitySourceName = "B2X.Tracing";

    /// <summary>
    /// Gets the OpenTelemetry meter for custom metrics.
    /// </summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Gets the activity source for custom tracing.
    /// </summary>
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    /// <summary>
    /// Business metrics counters.
    /// </summary>
    public static class BusinessMetrics
    {
        public static readonly Counter<long> OrderVolume = Meter.CreateCounter<long>(
            "B2X_orders_total",
            description: "Total number of orders processed");

        public static readonly Counter<long> UserSessions = Meter.CreateCounter<long>(
            "B2X_user_sessions_total",
            description: "Total number of user sessions");

        public static readonly Histogram<double> ApiLatency = Meter.CreateHistogram<double>(
            "B2X_api_latency_seconds",
            description: "API request latency in seconds");
    }

    /// <summary>
    /// Multi-tenant metrics.
    /// </summary>
    public static class TenantMetrics
    {
        public static readonly Counter<long> TenantRequests = Meter.CreateCounter<long>(
            "B2X_tenant_requests_total",
            description: "Total requests per tenant",
            unit: "requests");

        public static readonly Histogram<double> TenantLatency = Meter.CreateHistogram<double>(
            "B2X_tenant_latency_seconds",
            description: "Request latency per tenant in seconds",
            unit: "s");

        public static readonly UpDownCounter<long> ActiveTenants = Meter.CreateUpDownCounter<long>(
            "B2X_active_tenants",
            description: "Number of currently active tenants");
    }

    /// <summary>
    /// Configures OpenTelemetry with Prometheus exporter and custom metrics.
    /// </summary>
    public static void ConfigureOpenTelemetry(IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OTEL_SERVICE_NAME"] ?? "B2X";
        var prometheusPort = configuration.GetValue<int>("Prometheus:Port", 9464);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(MeterName)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter(options =>
                    {
                        options.ScrapeEndpointPath = "/metrics";
                        options.ScrapeResponseCacheDurationMilliseconds = 1000;
                    });

                // Add console exporter for development
                if (configuration.GetValue<bool>("Prometheus:EnableConsoleExporter", false))
                {
                    metrics.AddConsoleExporter();
                }
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ActivitySourceName);

                // Add console exporter for development
                if (configuration.GetValue<bool>("Tracing:EnableConsoleExporter", false))
                {
                    tracing.AddConsoleExporter();
                }
            });
    }

    /// <summary>
    /// Maps the Prometheus metrics endpoint.
    /// </summary>
    public static void MapMetricsEndpoint(WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint("/metrics");
    }
}
