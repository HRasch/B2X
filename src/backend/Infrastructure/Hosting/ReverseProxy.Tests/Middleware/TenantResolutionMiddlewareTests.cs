using B2X.ReverseProxy.Middleware;
using B2X.ReverseProxy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Net;
using Xunit;

namespace B2X.ReverseProxy.Tests.Middleware;

public class TenantResolutionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ITenantDomainResolver> _resolverMock;
    private readonly Mock<ILogger<TenantResolutionMiddleware>> _loggerMock;
    private readonly TenantResolutionMiddleware _middleware;

    public TenantResolutionMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _resolverMock = new Mock<ITenantDomainResolver>();
        _loggerMock = new Mock<ILogger<TenantResolutionMiddleware>>();

        _middleware = new TenantResolutionMiddleware(
            _nextMock.Object,
            _resolverMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ValidTenant_AddsTenantHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("tenant1.b2xgate.com");

        var tenantInfo = new TenantInfo(
            Guid.NewGuid(),
            "tenant1",
            "Tenant 1",
            TenantStatus.Active);

        _resolverMock.Setup(r => r.ResolveAsync("tenant1.b2xgate.com"))
            .ReturnsAsync(tenantInfo);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeTrue();
        context.Request.Headers["X-Tenant-ID"].ToString().ShouldBe(tenantInfo.TenantId.ToString());
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_InvalidTenant_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("invalid.b2xgate.com");

        _resolverMock.Setup(r => r.ResolveAsync("invalid.b2xgate.com"))
            .ReturnsAsync((TenantInfo?)null);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeFalse();
        _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_InactiveTenant_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("inactive.b2xgate.com");

        var tenantInfo = new TenantInfo(
            Guid.NewGuid(),
            "inactive",
            "Inactive Tenant",
            TenantStatus.Inactive);

        _resolverMock.Setup(r => r.ResolveAsync("inactive.b2xgate.com"))
            .ReturnsAsync(tenantInfo);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeFalse();
        _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ExistingTenantHeader_PreservesOriginal()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("tenant1.b2xgate.com");
        context.Request.Headers["X-Tenant-ID"] = "original-tenant-id";

        var tenantInfo = new TenantInfo(
            Guid.NewGuid(),
            "tenant1",
            "Tenant 1",
            TenantStatus.Active);

        _resolverMock.Setup(r => r.ResolveAsync("tenant1.b2xgate.com"))
            .ReturnsAsync(tenantInfo);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers["X-Tenant-ID"].ToString().ShouldBe("original-tenant-id");
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ResolverThrowsException_Returns500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("tenant1.b2xgate.com");

        _resolverMock.Setup(r => r.ResolveAsync("tenant1.b2xgate.com"))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeFalse();
        _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Theory]
    [InlineData("localhost")]
    [InlineData("127.0.0.1")]
    [InlineData("localhost:8080")]
    [InlineData("127.0.0.1:8080")]
    public async Task InvokeAsync_LocalhostHost_SkipsResolution(string host)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString(host);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeFalse();
        _resolverMock.Verify(r => r.ResolveAsync(It.IsAny<string>()), Times.Never);
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_CustomDomain_ResolvesCorrectly()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("shop.customer.de");

        var tenantInfo = new TenantInfo(
            Guid.NewGuid(),
            "customer",
            "Customer GmbH",
            TenantStatus.Active);

        _resolverMock.Setup(r => r.ResolveAsync("shop.customer.de"))
            .ReturnsAsync(tenantInfo);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.ContainsKey("X-Tenant-ID").ShouldBeTrue();
        context.Request.Headers["X-Tenant-ID"].ToString().ShouldBe(tenantInfo.TenantId.ToString());
        _nextMock.Verify(next => next(context), Times.Once);
    }
}