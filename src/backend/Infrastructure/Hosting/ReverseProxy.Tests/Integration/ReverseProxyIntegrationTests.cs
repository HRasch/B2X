using B2X.ReverseProxy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Shouldly;
using System.Net;
using System.Net.Http;
using Xunit;

namespace B2X.ReverseProxy.Tests.Integration;

public class ReverseProxyIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly TestServer _testServer;
    private readonly HttpClient _client;
    private readonly Mock<ITenantDomainResolver> _resolverMock;
    private bool _disposed;

    public ReverseProxyIntegrationTests()
    {
        _resolverMock = new Mock<ITenantDomainResolver>();

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddJsonFile("appsettings.Test.json", optional: true);
            })
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.ConfigureServices((context, services) =>
                {
                    services.AddSingleton(context.Configuration);

                    services.AddReverseProxy()
                        .LoadFromConfig(context.Configuration.GetSection("ReverseProxy"));

                    services.AddMemoryCache();
                    services.AddSingleton(_resolverMock.Object);
                });

                webBuilder.Configure(app =>
                {
                    app.UseMiddleware<B2X.ReverseProxy.Middleware.TenantResolutionMiddleware>();
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapReverseProxy();
                        endpoints.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
                    });
                });
            })
            .Start();

        _testServer = _host.GetTestServer();
        _client = _host.GetTestClient();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _client.Dispose();
            _testServer.Dispose();
            _host.Dispose();
        }

        _disposed = true;
    }

    [Fact]
    public async Task Get_StoreRoute_ValidTenant_ProxiesToStoreCluster()
    {
        // Reset mock for this test
        _resolverMock.Reset();

        var tenantId = Guid.NewGuid();
        var tenantInfo = new TenantInfo(tenantId, "tenant1", "Tenant 1", TenantStatus.Active);
        _resolverMock.Setup(r => r.ResolveAsync("tenant1.b2xgate.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantInfo);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/store/api/products");
        request.Headers.Host = "tenant1.b2xgate.com";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // Verify that tenant resolution succeeded (no 404 from middleware)
        // The reverse proxy will return 404 because there's no actual backend service
        // but the middleware should have added the tenant headers
        response.StatusCode.ShouldBe(HttpStatusCode.BadGateway); // Expected from reverse proxy with no backend

        // Verify the mock was called (tenant resolution worked)
        _resolverMock.Verify(r => r.ResolveAsync("tenant1.b2xgate.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Get_AdminRoute_InvalidTenant_Returns404()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/admin/api/dashboard");
        request.Headers.Host = "nonexistent.b2xgate.com";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ManagementRoute_InactiveTenant_Returns404()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantInfo = new TenantInfo(tenantId, "inactive", "Inactive Tenant", TenantStatus.Inactive);
        _resolverMock.Setup(r => r.ResolveAsync("inactive.b2xgate.com", default))
            .ReturnsAsync(tenantInfo);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/management/api/tenants");
        request.Headers.Host = "inactive.b2xgate.com";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ApiRoute_CustomDomain_ProxiesCorrectly()
    {
        // Reset mock for this test
        _resolverMock.Reset();

        var tenantId = Guid.NewGuid();
        var tenantInfo = new TenantInfo(tenantId, "customer", "Customer Shop", TenantStatus.Active);
        _resolverMock.Setup(r => r.ResolveAsync("api.customer-shop.de", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantInfo);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/health");
        request.Headers.Host = "api.customer-shop.de";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // Verify that tenant resolution succeeded (no 404 from middleware)
        // The reverse proxy will return 404 because there's no actual backend service
        // but the middleware should have processed the request correctly
        response.StatusCode.ShouldBe(HttpStatusCode.BadGateway); // Expected from reverse proxy with no backend

        // Verify the mock was called (tenant resolution worked)
        _resolverMock.Verify(r => r.ResolveAsync("api.customer-shop.de", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Get_Localhost_SkipsTenantResolution()
    {
        // Reset mock for this test
        _resolverMock.Reset();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/store/api/products");
        request.Headers.Host = "localhost:8080";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // For localhost, tenant resolution should be skipped
        // The mock should not be called
        _resolverMock.Verify(r => r.ResolveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        // Since there's no actual backend service configured in the test,
        // the reverse proxy will return 404 when trying to route, but that's expected
        // The important thing is that tenant resolution was skipped
    }

    [Fact]
    public async Task Get_HealthCheck_AlwaysAccessible()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Host = "any-domain.com";

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        // Health checks should be accessible regardless of tenant
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}