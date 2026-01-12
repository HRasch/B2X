using B2X.ReverseProxy.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace B2X.ReverseProxy.Tests.Services;

public class TenantDomainResolverTests : IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantDomainResolver> _logger;
    private readonly TenantDomainResolver _resolver;

    public TenantDomainResolverTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _logger = Substitute.For<ILogger<TenantDomainResolver>>();

        var configData = new Dictionary<string, string?>
        {
            ["Multitenancy:BaseDomain"] = "b2xgate.com",
            ["Multitenancy:CustomDomains:shop.customer.de:TenantId"] = "11111111-1111-1111-1111-111111111111",
            ["Multitenancy:CustomDomains:shop.customer.de:TenantSlug"] = "customer",
            ["Multitenancy:CustomDomains:shop.customer.de:DisplayName"] = "Customer Shop",
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _resolver = new TenantDomainResolver(_cache, _configuration, _logger);
    }

    [Theory]
    [InlineData("tenant1.b2xgate.com", "tenant1")]
    [InlineData("demo.b2xgate.com", "demo")]
    [InlineData("my-shop.b2xgate.com", "my-shop")]
    public async Task ResolveAsync_SubdomainPattern_ReturnsTenantFromSlug(string domain, string expectedSlug)
    {
        // Act
        var result = await _resolver.ResolveAsync(domain);

        // Assert
        result.ShouldNotBeNull();
        result.Slug.ShouldBe(expectedSlug);
        result.Status.ShouldBe(TenantStatus.Active);
    }

    [Fact]
    public async Task ResolveAsync_CustomDomain_ReturnsTenantFromConfig()
    {
        // Act
        var result = await _resolver.ResolveAsync("shop.customer.de");

        // Assert
        result.ShouldNotBeNull();
        result.TenantId.ShouldBe(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        result.Slug.ShouldBe("customer");
        result.DisplayName.ShouldBe("Customer Shop");
    }

    [Fact]
    public async Task ResolveAsync_UnknownDomain_ReturnsNull()
    {
        // Act
        var result = await _resolver.ResolveAsync("unknown.example.com");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ResolveAsync_CachesResult()
    {
        // Arrange
        var domain = "cached.b2xgate.com";

        // Act - First call
        var result1 = await _resolver.ResolveAsync(domain);

        // Verify it's cached
        var cacheKey = $"tenant:domain:{domain.ToLowerInvariant()}";
        _cache.TryGetValue<TenantInfo>(cacheKey, out var cachedValue).ShouldBeTrue();
        cachedValue.ShouldNotBeNull();
        cachedValue.Slug.ShouldBe("cached");

        // Act - Second call (should use cache)
        var result2 = await _resolver.ResolveAsync(domain);

        // Assert
        result1!.TenantId.ShouldBe(result2!.TenantId);
    }

    [Fact]
    public async Task ResolveAsync_BaseDomainOnly_ReturnsNull()
    {
        // Act
        var result = await _resolver.ResolveAsync("b2xgate.com");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task ResolveAsync_MultiLevelSubdomain_ReturnsNull()
    {
        // Sub-subdomains like "a.b.b2xgate.com" should not match
        // Act
        var result = await _resolver.ResolveAsync("sub.tenant1.b2xgate.com");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void InvalidateCache_RemovesCachedEntry()
    {
        // Arrange
        var domain = "toclear.b2xgate.com";
        var cacheKey = $"tenant:domain:{domain.ToLowerInvariant()}";
        var tenantInfo = new TenantInfo(Guid.NewGuid(), "toclear", "To Clear", TenantStatus.Active);
        _cache.Set(cacheKey, tenantInfo);

        // Verify it's cached
        _cache.TryGetValue<TenantInfo>(cacheKey, out _).ShouldBeTrue();

        // Act
        _resolver.InvalidateCache(domain);

        // Assert
        _cache.TryGetValue<TenantInfo>(cacheKey, out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("TENANT1.B2XGATE.COM", "tenant1")]  // Uppercase
    [InlineData("Tenant1.B2xgate.Com", "tenant1")]  // Mixed case
    public async Task ResolveAsync_CaseInsensitive(string domain, string expectedSlug)
    {
        // Act
        var result = await _resolver.ResolveAsync(domain);

        // Assert
        result.ShouldNotBeNull();
        result.Slug.ShouldBe(expectedSlug);
    }

    [Fact]
    public async Task ResolveAsync_DeterministicGuid_ConsistentAcrossCalls()
    {
        // For subdomain-based tenants, the GUID should be deterministic
        // Act
        var result1 = await _resolver.ResolveAsync("consistent.b2xgate.com");
        var result2 = await _resolver.ResolveAsync("consistent.b2xgate.com");

        // Assert
        result1!.TenantId.ShouldBe(result2!.TenantId);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
