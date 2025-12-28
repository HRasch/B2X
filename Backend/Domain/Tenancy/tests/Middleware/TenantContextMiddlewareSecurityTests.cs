using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using B2Connect.Shared.Tenancy.Infrastructure.Middleware;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using B2Connect.Shared.Infrastructure.ServiceClients;
using System.Security.Claims;

namespace B2Connect.Shared.Tenancy.Tests.Middleware;

/// <summary>
/// Security-focused integration tests for TenantContextMiddleware.
/// Tests all security fixes from Pentesting Review.
/// </summary>
public class TenantContextMiddlewareSecurityTests
{
    private readonly Mock<ILogger<TenantContextMiddleware>> _mockLogger;
    private readonly Mock<IHostEnvironment> _mockEnvironment;
    private readonly Mock<ITenancyServiceClient> _mockTenancyClient;
    private readonly TenantContext _tenantContext;

    public TenantContextMiddlewareSecurityTests()
    {
        _mockLogger = new Mock<ILogger<TenantContextMiddleware>>();
        _mockEnvironment = new Mock<IHostEnvironment>();
        _mockTenancyClient = new Mock<ITenancyServiceClient>();
        _tenantContext = new TenantContext();
    }

    /// <summary>
    /// Creates a real IConfiguration from a dictionary (can't mock extension methods)
    /// </summary>
    private static IConfiguration CreateConfiguration(Dictionary<string, string?>? values = null)
    {
        var builder = new ConfigurationBuilder();
        if (values != null)
        {
            builder.AddInMemoryCollection(values);
        }
        return builder.Build();
    }

    #region CVE-2025-001: Tenant Spoofing Prevention

    [Fact]
    public async Task InvokeAsync_WithMismatchedJwtAndHeader_Returns403()
    {
        // Arrange
        var jwtTenantId = Guid.NewGuid();
        var headerTenantId = Guid.NewGuid();

        var context = CreateHttpContext();
        context.User = CreateUserWithTenant(jwtTenantId);
        context.Request.Headers["X-Tenant-ID"] = headerTenantId.ToString();
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        _tenantContext.TenantId.Should().Be(Guid.Empty, "Tenant should not be set on security violation");
    }

    [Fact]
    public async Task InvokeAsync_WithMatchingJwtAndHeader_AllowsRequest()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var context = CreateHttpContext();
        context.User = CreateUserWithTenant(tenantId);
        context.Request.Headers["X-Tenant-ID"] = tenantId.ToString();
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        _tenantContext.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task InvokeAsync_WithJwtOnly_UsesTenantFromJwt()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var context = CreateHttpContext();
        context.User = CreateUserWithTenant(tenantId);
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        _tenantContext.TenantId.Should().Be(tenantId);
    }

    #endregion

    #region CVE-2025-002: Development Fallback Security

    [Fact]
    public async Task InvokeAsync_WithFallbackInProduction_Returns500()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Path = "/api/products";
        // IMPORTANT: Must provide a valid host so middleware passes step 6 (host validation)
        // and reaches step 7 (fallback logic)
        context.Request.Host = new HostString("unknown.example.com");

        // Note: IsDevelopment() is an extension method that checks EnvironmentName == "Development"
        // We only mock EnvironmentName and let the extension method work naturally
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        // Mock the tenancy client to return null (host not found in DB)
        _mockTenancyClient.Setup(c => c.GetTenantByDomainAsync("unknown.example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDto?)null);

        // Use real configuration since GetValue<T>() is an extension method that can't be mocked
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            { "Tenant:Development:UseFallback", "true" }
        });

        var middleware = CreateMiddleware(configuration);

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _tenantContext.TenantId.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task InvokeAsync_WithFallbackInDevelopment_AllowsRequest()
    {
        // Arrange
        var fallbackTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var context = CreateHttpContext();
        context.Request.Path = "/api/products";
        // IMPORTANT: Must provide a valid host so middleware passes step 6 (host validation)
        // and reaches step 7 (fallback logic)
        context.Request.Host = new HostString("unknown.example.com");

        // Note: IsDevelopment() is an extension method that checks EnvironmentName == "Development"
        // We only mock EnvironmentName and let the extension method work naturally
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");

        // Mock the tenancy client to return null (host not found in DB)
        _mockTenancyClient.Setup(c => c.GetTenantByDomainAsync("unknown.example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantDto?)null);

        // Use real configuration since GetValue<T>() is an extension method that can't be mocked
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            { "Tenant:Development:UseFallback", "true" },
            { "Tenant:Development:FallbackTenantId", fallbackTenantId.ToString() }
        });

        var middleware = CreateMiddleware(configuration);

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        _tenantContext.TenantId.Should().Be(fallbackTenantId);
    }

    #endregion

    #region VUL-2025-004: Information Disclosure Prevention

    [Fact]
    public async Task InvokeAsync_WithInvalidRequest_ReturnsGenericError()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Path = "/api/products";

        // Note: IsDevelopment() is an extension method that checks EnvironmentName == "Development"
        // We only mock EnvironmentName and let the extension method work naturally
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Response should NOT contain detailed information
        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("Invalid request");
        responseBody.Should().NotContain("host");
        responseBody.Should().NotContain("path");
        responseBody.Should().NotContain("headerProvided");
    }

    #endregion

    #region VUL-2025-007: Tenant Ownership Validation

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedTenantAccess_Returns403()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var context = CreateHttpContext();
        context.User = CreateUserWithId(userId);
        context.Request.Headers["X-Tenant-ID"] = tenantId.ToString();
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        _mockTenancyClient.Setup(c => c.ValidateTenantAccessAsync(tenantId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        _tenantContext.TenantId.Should().Be(Guid.Empty);
    }

    #endregion

    #region VUL-2025-008: Host Input Validation

    [Fact]
    public async Task InvokeAsync_WithInvalidHost_Returns400()
    {
        // Arrange
        var context = CreateHttpContext();
        // Use a host with dangerous characters that are still valid for HostString
        // but should be rejected by the middleware's input validation
        context.Request.Host = new HostString("tenant<script>alert(1)</script>.evil.com");
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithValidHost_AllowsRequest()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var context = CreateHttpContext();
        context.Request.Host = new HostString("tenant1.b2connect.com");
        context.Request.Path = "/api/products";

        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        _mockTenancyClient.Setup(c => c.GetTenantByDomainAsync("tenant1.b2connect.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TenantDto(tenantId, "Tenant 1", "tenant1.b2connect.com", true));

        var middleware = CreateMiddleware();

        // Act
        await middleware.InvokeAsync(context, _tenantContext, _mockTenancyClient.Object);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        _tenantContext.TenantId.Should().Be(tenantId);
    }

    #endregion

    #region Helper Methods

    private TenantContextMiddleware CreateMiddleware(IConfiguration? configuration = null)
    {
        return new TenantContextMiddleware(
            async (context) => { context.Response.StatusCode = StatusCodes.Status200OK; await Task.CompletedTask; },
            configuration ?? CreateConfiguration(),
            _mockLogger.Object,
            _mockEnvironment.Object);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static ClaimsPrincipal CreateUserWithTenant(Guid tenantId)
    {
        var claims = new[]
        {
            new Claim("tenant_id", tenantId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    private static ClaimsPrincipal CreateUserWithId(Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    private static async Task<string> GetResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    #endregion
}
