using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace B2Connect.Tests.Integration;

/// <summary>
/// Aspire-based test fixture that starts the AppHost and provides HttpClients
/// for all services with dynamically resolved ports.
/// 
/// Resource names (from AppHost/Program.cs):
/// - Gateways: store-gateway (port 8000), admin-gateway (port 8080)
/// - Frontends: frontend-store (port 5173), frontend-admin (port 5174)
/// - Services: auth-service, catalog-service, localization-service, tenant-service, etc.
/// 
/// Usage:
///   [Collection("AspireApp")]
///   public class MyTests(AspireAppFixture fixture) { ... }
/// </summary>
public sealed class AspireAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    /// <summary>
    /// Get HttpClient for Store Frontend (port 5173)
    /// </summary>
    public HttpClient StoreFrontendClient => _app!.CreateHttpClient("frontend-store");

    /// <summary>
    /// Get HttpClient for Admin Frontend (port 5174)
    /// </summary>
    public HttpClient AdminFrontendClient => _app!.CreateHttpClient("frontend-admin");

    /// <summary>
    /// Get HttpClient for Store Gateway API (port 8000)
    /// </summary>
    public HttpClient StoreGatewayClient => _app!.CreateHttpClient("store-gateway");

    /// <summary>
    /// Get HttpClient for Admin Gateway API (port 8080)
    /// </summary>
    public HttpClient AdminGatewayClient => _app!.CreateHttpClient("admin-gateway");

    /// <summary>
    /// Get HttpClient for Auth Service (dynamic port)
    /// </summary>
    public HttpClient AuthServiceClient => _app!.CreateHttpClient("auth-service");

    /// <summary>
    /// Get HttpClient for Catalog Service (dynamic port)
    /// </summary>
    public HttpClient CatalogServiceClient => _app!.CreateHttpClient("catalog-service");

    /// <summary>
    /// Get HttpClient for any named resource
    /// </summary>
    public HttpClient GetClient(string resourceName) => _app!.CreateHttpClient(resourceName);

    /// <summary>
    /// Get the endpoint URL for a specific resource
    /// </summary>
    public async Task<string> GetEndpointUrlAsync(string resourceName, string endpointName = "http")
    {
        var endpoint = _app!.GetEndpoint(resourceName, endpointName);
        return endpoint.ToString();
    }

    /// <summary>
    /// Resource notification service for waiting on resources
    /// </summary>
    public ResourceNotificationService ResourceNotifications =>
        _app!.Services.GetRequiredService<ResourceNotificationService>();

    public async Task InitializeAsync()
    {
        // Create the test builder from AppHost
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.B2Connect_AppHost>();

        // Configure logging for tests
        appHost.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
            // CRITICAL: Re-enable health check logs (Aspire suppresses by default)
            // See: https://github.com/dotnet/aspire/issues/13714
            logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService",
                LogLevel.Information);
            logging.AddFilter("Aspire", LogLevel.Warning);
            logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
        });

        // Configure HTTP client defaults for resilience
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Build and start the application
        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // Wait for infrastructure to be healthy before running tests
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

        // CRITICAL: Wait for infrastructure containers FIRST
        // Services cannot pass health checks until database/cache are available
        // See: .ai/knowledgebase/testing/aspire-integration-testing-timeouts.md
        await ResourceNotifications.WaitForResourceHealthyAsync("postgres", cts.Token);
        await ResourceNotifications.WaitForResourceHealthyAsync("redis", cts.Token);
        await ResourceNotifications.WaitForResourceHealthyAsync("rabbitmq", cts.Token);
        await ResourceNotifications.WaitForResourceHealthyAsync("elasticsearch", cts.Token);

        // Give services time to connect to infrastructure and become healthy
        // Services were failing because PostgreSQL wasn't ready yet
        await Task.Delay(15000, cts.Token); // 15 seconds for services to stabilize
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}
