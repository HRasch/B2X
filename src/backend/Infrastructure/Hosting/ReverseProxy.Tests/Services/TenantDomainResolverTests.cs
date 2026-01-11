using B2X.ReverseProxy.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.ReverseProxy.Tests.Services;

public class TenantDomainResolverTests : IDisposable
{
    private readonly MemoryCache _cache;
    private readonly IConfiguration _config;
    private readonly Mock<ILogger<TenantDomainResolver>> _loggerMock;
    private readonly TenantDomainResolver _resolver;
    private bool _disposed;

    public TenantDomainResolverTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Multitenancy:BaseDomain"] = "b2xgate.com"
            });
        _config = configBuilder.Build();
        _loggerMock = new Mock<ILogger<TenantDomainResolver>>();

        _resolver = new TenantDomainResolver(
            _cache,
            _config,
            _loggerMock.Object);
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
            _cache.Dispose();
        }

        _disposed = true;
    }

    [Theory]
    [InlineData("tenant1.b2xgate.com", "tenant1")]
    [InlineData("demo.b2xgate.com", "demo")]
    [InlineData("test-tenant.b2xgate.com", "test-tenant")]
    public async Task TryResolveFromSubdomain_ValidSubdomain_ReturnsTenantInfo(string domain, string expectedSlug)
    {
        // Act
        var result = _resolver.GetType()
            .GetMethod("TryResolveFromSubdomain", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_resolver, new object[] { domain }) as TenantInfo;

        // Assert
        result.ShouldNotBeNull();
        result.Slug.ShouldBe(expectedSlug);
        result.Status.ShouldBe(TenantStatus.Active);
    }

    [Theory]
    [InlineData("b2xgate.com")]
    [InlineData("shop.customer.de")]
    [InlineData("localhost")]
    public async Task TryResolveFromSubdomain_InvalidSubdomain_ReturnsNull(string domain)
    {
        // Act
        var result = _resolver.GetType()
            .GetMethod("TryResolveFromSubdomain", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_resolver, new object[] { domain }) as TenantInfo;

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ResolveAsync_SubdomainPattern_ReturnsTenantInfo()
    {
        // Arrange
        var domain = "tenant1.b2xgate.com";

        // Act
        var result = await _resolver.ResolveAsync(domain);

        // Assert
        result.ShouldNotBeNull();
        result!.Slug.ShouldBe("tenant1");
        result.Status.ShouldBe(TenantStatus.Active);
        result.TenantId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task ResolveAsync_SubdomainPattern_CacheMiss_ResolvesAndCaches()
    {
        // Arrange
        var domain = "tenant1.b2xgate.com";

        // Act
        var first = await _resolver.ResolveAsync(domain);
        var second = await _resolver.ResolveAsync(domain);

        // Assert
        first.ShouldNotBeNull();
        second.ShouldNotBeNull();
        first!.Slug.ShouldBe("tenant1");
        first.Status.ShouldBe(TenantStatus.Active);
        first.TenantId.ShouldNotBe(Guid.Empty);
        // Cache returns the same instance for repeated resolution
        second.ShouldBeSameAs(first);
    }

    [Fact]
    public void InvalidateCache_DoesNotThrow()
    {
        // Arrange
        var domain = "tenant1.b2xgate.com";

        // Act & Assert
        Should.NotThrow(() => _resolver.InvalidateCache(domain));
    }

    [Fact]
    public void CreateDeterministicGuid_ConsistentResults()
    {
        // Arrange
        var input = "test-tenant";

        // Act
        var result1 = typeof(TenantDomainResolver)
            .GetMethod("CreateDeterministicGuid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, new object[] { input }) as Guid?;

        var result2 = typeof(TenantDomainResolver)
            .GetMethod("CreateDeterministicGuid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, new object[] { input }) as Guid?;

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.ShouldBe(result2);
    }
}