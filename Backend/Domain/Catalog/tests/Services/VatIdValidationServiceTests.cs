using B2Connect.Catalog.Application.Services;
using B2Connect.Catalog.Application.DTOs;
using B2Connect.Catalog.Application.Interfaces;
using B2Connect.Catalog.Infrastructure.ExternalServices;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.Services;

/// <summary>
/// Integration tests for VatIdValidationService
/// Issue #31: B2B VAT-ID Validation
/// 
/// Test scenarios:
/// - Valid VAT-IDs from different EU countries
/// - Invalid VAT-IDs
/// - Reverse charge logic (seller in DE, buyer in other EU country with valid VAT-ID)
/// - Caching (365 days per NIS2)
/// - API failures and error handling
/// - Input validation
/// </summary>
public class VatIdValidationServiceTests
{
    private readonly VatIdValidationService _service;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly MockViesClient _mockViesClient;
    private readonly Mock<ILogger<VatIdValidationService>> _mockLogger;

    public VatIdValidationServiceTests()
    {
        _mockCache = new Mock<IDistributedCache>();
        _mockViesClient = new MockViesClient(new Mock<ILogger<MockViesClient>>().Object);
        _mockLogger = new Mock<ILogger<VatIdValidationService>>();

        // Setup cache to return null by default (cache miss)
        _mockCache
            .Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _mockCache
            .Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new VatIdValidationService(
            _mockViesClient,
            _mockCache.Object,
            _mockLogger.Object

    [Fact]
    public async Task ValidateVatId_ValidGermanVatId_ReturnsValid()
    {
        // Arrange
        var country = "DE";
        var vatId = "188596368";

        // Act
        var result = await _service.ValidateVatIdAsync(country, vatId);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("DE", result.CountryCode);
        Assert.Equal("188596368", result.VatNumber);
        Assert.NotEmpty(result.CompanyName);
        Assert.False(result.WasFromCache);
    }

    [Fact]
    public async Task ValidateVatId_ValidAustrianVatId_ReturnsValid()
    {
        // Arrange
        var country = "AT";
        var vatId = "U12345678";

        // Act
        var result = await _service.ValidateVatIdAsync(country, vatId);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("AT", result.CountryCode);
    }

    [Fact]
    public async Task ValidateVatId_ValidFrenchVatId_ReturnsValid()
    {
        // Arrange
        var country = "FR";
        var vatId = "12345678901";

        // Act
        var result = await _service.ValidateVatIdAsync(country, vatId);

        // Assert
        Assert.True(result.IsValid);
        Assert.Equal("FR", result.CountryCode);
    }

    // ===== Invalid VAT-ID Tests =====

    [Fact]
    public async Task ValidateVatId_InvalidVatId_ReturnsInvalid()
    {
        // Arrange
        var country = "DE";
        var vatId = "999999999";  // Not in known test list

        // Act
        var result = await _service.ValidateVatIdAsync(country, vatId);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateVatId_EmptyVatId_ThrowsValidationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.ValidateVatIdAsync("DE", "")
        );
    }

    [Fact]
    public async Task ValidateVatId_InvalidCountryCode_ThrowsValidationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.ValidateVatIdAsync("ZZ", "123456789")
        );
    }

    [Fact]
    public async Task ValidateVatId_LowercaseCountryCode_ThrowsValidationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.ValidateVatIdAsync("de", "188596368")
        );
    }

    // ===== Caching Tests =====

    [Fact]
    public async Task ValidateVatId_CachesMissingResult_ForOneYear()
    {
        // Arrange
        var country = "DE";
        var vatId = "188596368";

        // Act
        await _service.ValidateVatIdAsync(country, vatId);

        // Assert - Verify SetStringAsync was called with cache duration
        _mockCache.Verify(
            x => x.SetStringAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.Is<DistributedCacheEntryOptions>(o =>
                    o.AbsoluteExpirationRelativeToNow == TimeSpan.FromDays(365)
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ValidateVatId_ReturnsCachedResult_WhenAvailable()
    {
        // Arrange
        var country = "DE";
        var vatId = "188596368";
        var cachedResult = """{"isValid":true,"countryCode":"DE","vatNumber":"188596368","companyName":"Cached Company","companyAddress":"Test St.","validatedAt":"2025-12-28T00:00:00","expiresAt":"2026-12-28T00:00:00","wasFromCache":false,"errorMessage":null}""";

        _mockCache
            .Setup(x => x.GetStringAsync($"vat-validation:{country}:{vatId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedResult);

        // Act
        var result = await _service.ValidateVatIdAsync(country, vatId);

        // Assert
        Assert.True(result.WasFromCache);
        Assert.Equal("Cached Company", result.CompanyName);
    }

    // ===== Reverse Charge Tests =====

    [Fact]
    public void ReverseCharge_ValidVatId_DifferentEuCountries_AppliesToTrue()
    {
        // Arrange
        var validation = VatValidationResult.Valid("AT", "U12345678", "Test Co", "Test St");
        var buyerCountry = "AT";    // Austria
        var sellerCountry = "DE";   // Germany

        // Act
        var result = _service.ShouldApplyReverseCharge(validation, buyerCountry, sellerCountry);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ReverseCharge_ValidVatId_SameCountry_AppliesToFalse()
    {
        // Arrange
        var validation = VatValidationResult.Valid("DE", "188596368", "Test Co", "Test St");
        var buyerCountry = "DE";    // Same as seller
        var sellerCountry = "DE";

        // Act
        var result = _service.ShouldApplyReverseCharge(validation, buyerCountry, sellerCountry);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReverseCharge_InvalidVatId_AppliesToFalse()
    {
        // Arrange
        var validation = VatValidationResult.Invalid("Invalid VAT-ID");
        var buyerCountry = "AT";
        var sellerCountry = "DE";

        // Act
        var result = _service.ShouldApplyReverseCharge(validation, buyerCountry, sellerCountry);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReverseCharge_NonEuBuyer_AppliesToFalse()
    {
        // Arrange
        var validation = VatValidationResult.Valid("GB", "987654321", "Test Co", "Test St");  // GB not in EU
        var buyerCountry = "GB";    // Not EU
        var sellerCountry = "DE";

        // Act
        var result = _service.ShouldApplyReverseCharge(validation, buyerCountry, sellerCountry);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("AT", "DE")]  // Austria -> Germany
    [InlineData("FR", "DE")]  // France -> Germany
    [InlineData("IT", "DE")]  // Italy -> Germany
    [InlineData("ES", "AT")]  // Spain -> Austria
    [InlineData("NL", "BE")]  // Netherlands -> Belgium
    public void ReverseCharge_AllEuCombinations_AppliesCorrectly(string buyer, string seller)
    {
        // Arrange
        var validation = VatValidationResult.Valid(buyer, "123456789", "Test Co", "Test St");

        // Act
        var result = _service.ShouldApplyReverseCharge(validation, buyer, seller);

        // Assert - Should apply if different countries
        Assert.True(result);
    }

    // ===== Error Handling Tests =====

    [Fact]
    public async Task ValidateVatId_ApiFailure_ThrowsVatIdValidationException()
    {
        // Arrange - Use a pattern that triggers API error in mock
        var country = "DE";
        var vatId = "000000000";  // Triggers simulated API failure

        // Act & Assert
        await Assert.ThrowsAsync<VatIdValidationException>(
            () => _service.ValidateVatIdAsync(country, vatId)
        );
    }

    // ===== Validation Request Tests =====

    [Fact]
    public void VatIdValidationRequestValidator_ValidInput_Passes()
    {
        // Arrange
        var request = new VatIdValidationRequest
        {
            CountryCode = "DE",
            VatNumber = "188596368"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void VatIdValidationRequestValidator_InvalidCountryCode_Fails()
    {
        // Arrange
        var request = new VatIdValidationRequest
        {
            CountryCode = "ZZZ",  // 3 letters instead of 2
            VatNumber = "188596368"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CountryCode");
    }

    [Fact]
    public void VatIdValidationRequestValidator_TooLongVatNumber_Fails()
    {
        // Arrange
        var request = new VatIdValidationRequest
        {
            CountryCode = "DE",
            VatNumber = "1234567890123"  // 13 chars, max is 12
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VatNumber");
    }
}
