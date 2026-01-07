using B2X.Tenancy.Repositories;
using B2X.Tenancy.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Tenancy.Tests.Services;

public class DomainLookupServiceTests : IDisposable
{
    private readonly Mock<ITenantDomainRepository> _mockRepo;
    private readonly MemoryCache _memoryCache;
    private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly Mock<ILogger<DomainLookupService>> _mockLogger;
    private readonly DomainLookupService _service;

    public DomainLookupServiceTests()
    {
        _mockRepo = new Mock<ITenantDomainRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockDistributedCache = new Mock<IDistributedCache>();
        _mockLogger = new Mock<ILogger<DomainLookupService>>();

        _service = new DomainLookupService(
            _mockRepo.Object,
            _memoryCache,
            _mockDistributedCache.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_WithCachedValue_ReturnsCachedTenantId()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domainName = "cached.B2X.de";
        var cacheKey = $"tenant:domain:{domainName}";

        _memoryCache.Set(cacheKey, (Guid?)tenantId);

        // Act
        var result = await _service.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Equal(tenantId, result);
        _mockRepo.Verify(r => r.ResolveTenantIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_WithoutCache_QueriesDatabase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domainName = "uncached.B2X.de";

        _mockRepo.Setup(r => r.ResolveTenantIdAsync(domainName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantId);

        _mockDistributedCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _service.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Equal(tenantId, result);
        _mockRepo.Verify(r => r.ResolveTenantIdAsync(domainName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_NormalizesDomainnameToLowercase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domainName = "TEST.B2X.DE";
        var normalizedDomain = "test.B2X.de";

        _mockRepo.Setup(r => r.ResolveTenantIdAsync(normalizedDomain, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantId);

        _mockDistributedCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _service.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Equal(tenantId, result);
        _mockRepo.Verify(r => r.ResolveTenantIdAsync(normalizedDomain, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_WithUnknownDomain_ReturnsNull()
    {
        // Arrange
        var domainName = "unknown.example.com";

        _mockRepo.Setup(r => r.ResolveTenantIdAsync(domainName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        _mockDistributedCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _service.ResolveTenantIdAsync(domainName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task InvalidateCacheAsync_RemovesFromMemoryCache()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var domainName = "toclear.B2X.de";
        var cacheKey = $"tenant:domain:{domainName}";

        _memoryCache.Set(cacheKey, (Guid?)tenantId);
        Assert.True(_memoryCache.TryGetValue(cacheKey, out _));

        // Act
        await _service.InvalidateCacheAsync(domainName);

        // Assert
        Assert.False(_memoryCache.TryGetValue(cacheKey, out _));
    }

    [Fact]
    public async Task InvalidateCacheAsync_RemovesFromDistributedCache()
    {
        // Arrange
        var domainName = "toclear.B2X.de";

        // Act
        await _service.InvalidateCacheAsync(domainName);

        // Assert
        _mockDistributedCache.Verify(c => c.RemoveAsync(
            It.Is<string>(k => k.Contains(domainName)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }
}
