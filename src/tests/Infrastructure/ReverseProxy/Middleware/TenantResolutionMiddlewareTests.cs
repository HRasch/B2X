using B2X.ReverseProxy.Middleware;
using B2X.ReverseProxy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace B2X.ReverseProxy.Tests.Middleware;

public class TenantResolutionMiddlewareTests
{
    private readonly ITenantDomainResolver _resolver;
    private readonly ILogger<TenantResolutionMiddleware> _logger;
    private readonly TenantResolutionMiddleware _middleware;
    private bool _nextCalled;

    public TenantResolutionMiddlewareTests()
    {
        _resolver = Substitute.For<ITenantDomainResolver>();
        _logger = Substitute.For<ILogger<TenantResolutionMiddleware>>();
        _nextCalled = false;

        _middleware = new TenantResolutionMiddleware(
            next: _ =>
            {
                _nextCalled = true;
                return Task.CompletedTask;
            },
            domainResolver: _resolver,
            logger: _logger);
    }

    [Fact]
    public async Task InvokeAsync_HealthCheckPath_SkipsTenantResolution()
    {
        // Arrange
        var context = CreateHttpContext("/health", "any.domain.com");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextCalled.ShouldBeTrue();
        await _resolver.DidNotReceive().ResolveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvokeAsync_ValidTenant_AddsTenantHeaders()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantInfo = new TenantInfo(tenantId, "demo", "Demo Tenant", TenantStatus.Active);
        var context = CreateHttpContext("/api/products", "demo.b2xgate.com");

        _resolver.ResolveAsync("demo.b2xgate.com", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TenantInfo?>(tenantInfo));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextCalled.ShouldBeTrue();
        context.Request.Headers[TenantResolutionMiddleware.TenantIdHeader].ToString()
            .ShouldBe(tenantId.ToString());
        context.Request.Headers[TenantResolutionMiddleware.TenantSlugHeader].ToString()
            .ShouldBe("demo");
    }

    [Fact]
    public async Task InvokeAsync_UnknownDomain_Returns404()
    {
        // Arrange
        var context = CreateHttpContext("/api/products", "unknown.domain.com");

        _resolver.ResolveAsync("unknown.domain.com", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TenantInfo?>(null));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task InvokeAsync_CustomDomain_ResolvesTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantInfo = new TenantInfo(tenantId, "customer", "Customer Shop", TenantStatus.Active);
        var context = CreateHttpContext("/checkout", "shop.customer.de");

        _resolver.ResolveAsync("shop.customer.de", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TenantInfo?>(tenantInfo));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextCalled.ShouldBeTrue();
        context.Request.Headers[TenantResolutionMiddleware.TenantIdHeader].ToString()
            .ShouldBe(tenantId.ToString());
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task InvokeAsync_AllHealthEndpoints_SkipResolution(string path)
    {
        // Arrange
        var context = CreateHttpContext(path, "any.domain.com");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextCalled.ShouldBeTrue();
        await _resolver.DidNotReceive().ResolveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static DefaultHttpContext CreateHttpContext(string path, string host)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Host = new HostString(host);
        context.Response.Body = new MemoryStream();
        return context;
    }
}
