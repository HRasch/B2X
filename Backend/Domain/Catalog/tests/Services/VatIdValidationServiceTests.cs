using System.Text;
using B2Connect.Catalog.Infrastructure;
using B2Connect.Catalog.Models;
using B2Connect.Catalog.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.Services;

/// <summary>
/// Unit tests for VAT ID validation service
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class VatIdValidationServiceTests
{
    private readonly Mock<IViesApiClient> _mockViesClient;
    private readonly Mock<ILogger<VatIdValidationService>> _mockLogger;

    public VatIdValidationServiceTests()
    {
        _mockViesClient = new Mock<IViesApiClient>();
        _mockLogger = new Mock<ILogger<VatIdValidationService>>();
    }

    [Fact]
    public async Task ValidateVatIdAsync_ValidId_ReturnsIsValidTrue()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        var result = new VatValidationResult
        {
            IsValid = true,
            VatId = "DE123456789",
            CompanyName = "Example GmbH",
            CompanyAddress = "123 Main St, Berlin",
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(365)
        };

        _mockViesClient.Setup(v => v.ValidateVatIdAsync("DE", "123456789", It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actual = await service.ValidateVatIdAsync("DE", "123456789", CancellationToken.None);

        // Assert
        Assert.NotNull(actual);
        Assert.True(actual.IsValid);
        Assert.Equal("DE123456789", actual.VatId);
        Assert.Equal("Example GmbH", actual.CompanyName);
    }

    [Fact]
    public async Task ValidateVatIdAsync_InvalidId_ReturnsIsValidFalse()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        var result = new VatValidationResult
        {
            IsValid = false,
            VatId = "DE000000000",
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _mockViesClient.Setup(v => v.ValidateVatIdAsync("DE", "000000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actual = await service.ValidateVatIdAsync("DE", "000000000", CancellationToken.None);

        // Assert
        Assert.NotNull(actual);
        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task ValidateVatIdAsync_CacheMiss_CallsViesAndCaches()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        var result = new VatValidationResult
        {
            IsValid = true,
            VatId = "FR123456789",
            CompanyName = "Example SARL",
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(365)
        };

        _mockViesClient.Setup(v => v.ValidateVatIdAsync("FR", "123456789", It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actual = await service.ValidateVatIdAsync("FR", "123456789", CancellationToken.None);

        // Assert
        Assert.NotNull(actual);
        Assert.True(actual.IsValid);

        // Verify VIES API was called
        _mockViesClient.Verify(v => v.ValidateVatIdAsync("FR", "123456789", It.IsAny<CancellationToken>()),
            Times.Once);

        // Verify result was cached
        Assert.True(mockCache.ContainsKey("vat:FR:123456789"));
    }

    [Fact]
    public async Task ValidateVatIdAsync_CacheHit_ReturnsCachedResult()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        var cachedResult = new VatValidationResult
        {
            IsValid = true,
            VatId = "AT987654321",
            CompanyName = "Test AG",
            ValidatedAt = DateTime.UtcNow.AddDays(-30),
            ExpiresAt = DateTime.UtcNow.AddDays(335)
        };

        // Pre-populate cache
        var json = System.Text.Json.JsonSerializer.Serialize(cachedResult);
        await mockCache.SetStringAsync("vat:AT:987654321", json, new DistributedCacheEntryOptions(), CancellationToken.None);

        // Act
        var actual = await service.ValidateVatIdAsync("AT", "987654321", CancellationToken.None);

        // Assert
        Assert.NotNull(actual);
        Assert.True(actual.IsValid);
        Assert.Equal("Test AG", actual.CompanyName);

        // Verify VIES API was NOT called (cache hit)
        _mockViesClient.Verify(v => v.ValidateVatIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void ShouldApplyReverseCharge_ValidIdDifferentCountries_ReturnsTrue()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);
        var validation = new VatValidationResult { IsValid = true, VatId = "AT123456789" };

        // Act - Buyer in Austria, Seller in Germany
        var result = service.ShouldApplyReverseCharge(validation, "AT", "DE");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldApplyReverseCharge_InvalidId_ReturnsFalse()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);
        var validation = new VatValidationResult { IsValid = false, VatId = "AT000000000" };

        // Act
        var result = service.ShouldApplyReverseCharge(validation, "AT", "DE");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldApplyReverseCharge_SameCountry_ReturnsFalse()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);
        var validation = new VatValidationResult { IsValid = true, VatId = "DE123456789" };

        // Act - Both buyer and seller in Germany
        var result = service.ShouldApplyReverseCharge(validation, "DE", "DE");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldApplyReverseCharge_NonEuCountry_ReturnsFalse()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);
        var validation = new VatValidationResult { IsValid = true, VatId = "CH123456789" };

        // Act - Switzerland is not EU
        var result = service.ShouldApplyReverseCharge(validation, "CH", "DE");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateVatIdAsync_EmptyCountryCode_ThrowsArgumentException()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateVatIdAsync("", "123456789", CancellationToken.None));
    }

    [Fact]
    public async Task ValidateVatIdAsync_NullCountryCode_ThrowsArgumentException()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ValidateVatIdAsync(string.Empty, "123456789", CancellationToken.None));
    }

    [Fact]
    public void ShouldApplyReverseCharge_NullValidation_ThrowsArgumentNullException()
    {
        // Arrange
        var mockCache = new TestDistributedCache();
        var service = new VatIdValidationService(_mockViesClient.Object, mockCache, _mockLogger.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => service.ShouldApplyReverseCharge(null!, "DE", "AT"));
    }
}

/// <summary>
/// Test implementation of IDistributedCache for VAT validation tests
/// </summary>
public class TestDistributedCache : IDistributedCache
{
    private readonly Dictionary<string, byte[]> _data = new();

    public bool ContainsKey(string key) => _data.ContainsKey(key);

    public byte[]? Get(string key)
    {
        _data.TryGetValue(key, out var value);
        return value;
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        return Get(key);
    }

    public void Refresh(string key) { }

    public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;

    public void Remove(string key)
    {
        _data.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions? options = null)
    {
        _data[key] = value;
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions? options = null, CancellationToken token = default)
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task SetStringAsync(string key, string? value, DistributedCacheEntryOptions? options = null, CancellationToken token = default)
    {
        if (value == null)
        {
            return Task.CompletedTask;
        }

        Set(key, Encoding.UTF8.GetBytes(value));
        return Task.CompletedTask;
    }

    public Task<string?> GetStringAsync(string key, CancellationToken token = default)
    {
        var bytes = Get(key);
        return Task.FromResult(bytes == null ? null : Encoding.UTF8.GetString(bytes));
    }
}
