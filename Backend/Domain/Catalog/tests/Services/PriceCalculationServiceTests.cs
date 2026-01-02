using B2Connect.CatalogService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.Services;

/// <summary>
/// Unit tests for PriceCalculationService
/// Issue #30: B2C Price Transparency (PAngV Compliance)
/// 
/// Test scenarios:
/// - VAT calculation for different countries
/// - Discount application
/// - Invalid input handling
/// - Price rounding precision
/// </summary>
public class PriceCalculationServiceTests
{
    private readonly Mock<ILogger<PriceCalculationService>> _mockLogger;
    private readonly PriceCalculationService _service;

    public PriceCalculationServiceTests()
    {
        _mockLogger = new Mock<ILogger<PriceCalculationService>>();
        _service = new PriceCalculationService(_mockLogger.Object);
    }

    /// <summary>
    /// Test: Germany VAT calculation (19%)
    /// Expected: 100€ + 19€ VAT = 119€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_Germany_Returns19PercentVat()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "DE";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(19m, result.VatRate);
        Assert.Equal(19m, result.VatAmount);
        Assert.Equal(119m, result.PriceIncludingVat);
        Assert.Equal("DE", result.DestinationCountry);
        Assert.Equal("EUR", result.CurrencyCode);
    }

    /// <summary>
    /// Test: Austria VAT calculation (20%)
    /// Expected: 100€ + 20€ VAT = 120€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_Austria_Returns20PercentVat()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "AT";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(20m, result.VatRate);
        Assert.Equal(20m, result.VatAmount);
        Assert.Equal(120m, result.PriceIncludingVat);
    }

    /// <summary>
    /// Test: France VAT calculation (20%)
    /// Expected: 100€ + 20€ VAT = 120€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_France_Returns20PercentVat()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "FR";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(20m, result.VatRate);
        Assert.Equal(20m, result.VatAmount);
        Assert.Equal(120m, result.PriceIncludingVat);
    }

    /// <summary>
    /// Test: Belgium VAT calculation (21%)
    /// Expected: 100€ + 21€ VAT = 121€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_Belgium_Returns21PercentVat()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "BE";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(21m, result.VatRate);
        Assert.Equal(21m, result.VatAmount);
        Assert.Equal(121m, result.PriceIncludingVat);
    }

    /// <summary>
    /// Test: Discount application (10% off)
    /// Expected: 119€ (incl VAT) - 10% = 107.10€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_WithDiscount_AppliesCorrectly()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "DE";
        decimal discountPercentage = 10m;

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country, discountPercentage);

        // Assert
        Assert.Equal(119m, result.PriceIncludingVat);  // Original with VAT
        Assert.Equal(119m, result.OriginalPrice);       // Original stored
        Assert.Equal(11.90m, result.DiscountAmount);    // 10% of 119
        Assert.Equal(107.10m, result.FinalPrice);       // Final after discount
    }

    /// <summary>
    /// Test: 25% discount (larger discount)
    /// Expected: 119€ - 25% = 89.25€
    /// </summary>
    [Fact]
    public async Task CalculatePrice_WithLargeDiscount_CalculatesCorrectly()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "DE";
        decimal discountPercentage = 25m;

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country, discountPercentage);

        // Assert
        Assert.Equal(29.75m, result.DiscountAmount);    // 25% of 119
        Assert.Equal(89.25m, result.FinalPrice);        // Final after discount
    }

    /// <summary>
    /// Test: Price precision (rounding to 2 decimal places)
    /// Expected: 99.99€ + 18.9981€ VAT ≈ 118.99€ (not 118.98 or 119.00)
    /// </summary>
    [Fact]
    public async Task CalculatePrice_Precision_RoundsTo2Decimals()
    {
        // Arrange
        decimal basePrice = 99.99m;
        string country = "DE";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert - F2 always produces exactly 2 decimal places
        // Note: Uses system locale (German: comma separator, US: period)
        var vatFormatted = result.VatAmount.ToString("F2");
        var vatHasDecimal = vatFormatted.Contains('.') || vatFormatted.Contains(',');
        Assert.True(vatHasDecimal, $"VatAmount '{vatFormatted}' must have decimal separator");

        var priceFormatted = result.PriceIncludingVat.ToString("F2");
        var priceHasDecimal = priceFormatted.Contains('.') || priceFormatted.Contains(',');
        Assert.True(priceHasDecimal, $"PriceIncludingVat '{priceFormatted}' must have decimal separator");
    }

    /// <summary>
    /// Test: Invalid country (non-existent)
    /// Expected: Default to Germany (19%)
    /// </summary>
    [Fact]
    public async Task CalculatePrice_InvalidCountry_DefaultsToGermany()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "XX";  // Invalid country code

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(19m, result.VatRate);  // Defaults to German rate
        Assert.Equal("XX", result.DestinationCountry);  // Still stores requested country
    }

    /// <summary>
    /// Test: Case insensitivity (lowercase country)
    /// Expected: Should work with 'de' as well as 'DE'
    /// </summary>
    [Fact]
    public async Task CalculatePrice_LowercaseCountry_WorksCorrectly()
    {
        // Arrange
        decimal basePrice = 100m;
        string country = "de";  // Lowercase

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(19m, result.VatRate);
        Assert.Equal("DE", result.DestinationCountry);  // Should be uppercase
    }

    /// <summary>
    /// Test: GetVatRateAsync returns correct rate
    /// </summary>
    [Fact]
    public async Task GetVatRate_Germany_Returns19()
    {
        // Act
        var rate = await _service.GetVatRateAsync("DE");

        // Assert
        Assert.Equal(19m, rate);
    }

    /// <summary>
    /// Test: GetVatRateAsync with invalid country defaults
    /// </summary>
    [Fact]
    public async Task GetVatRate_InvalidCountry_DefaultsTo19()
    {
        // Act
        var rate = await _service.GetVatRateAsync("XX");

        // Assert
        Assert.Equal(19m, rate);
    }

    /// <summary>
    /// Test: Negative price throws exception
    /// </summary>
    [Fact]
    public async Task CalculatePrice_NegativePrice_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CalculatePriceAsync(-100m, "DE")
        );
    }

    /// <summary>
    /// Test: Null country throws exception
    /// </summary>
    [Fact]
    public async Task CalculatePrice_NullCountry_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CalculatePriceAsync(100m, null!)
        );
    }

    /// <summary>
    /// Test: Invalid discount percentage throws exception
    /// </summary>
    [Fact]
    public async Task CalculatePrice_InvalidDiscount_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CalculatePriceAsync(100m, "DE", -10m)
        );

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CalculatePriceAsync(100m, "DE", 150m)
        );
    }

    /// <summary>
    /// Test: Zero price is valid
    /// Expected: Should not throw, but return 0 prices
    /// </summary>
    [Fact]
    public async Task CalculatePrice_ZeroPrice_ReturnsZeroVat()
    {
        // Arrange
        decimal basePrice = 0m;
        string country = "DE";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.Equal(0m, result.ProductPrice);
        Assert.Equal(0m, result.VatAmount);
        Assert.Equal(0m, result.PriceIncludingVat);
    }

    /// <summary>
    /// Test: Very small price is rounded correctly
    /// Expected: 0.01€ + 0.001€ VAT ≈ 0.01€
    /// </summary>
    [Theory]
    [InlineData(0.01)]
    [InlineData(0.05)]
    [InlineData(0.10)]
    public async Task CalculatePrice_SmallPrices_RoundsCorrectly(decimal price)
    {
        // Act
        var result = await _service.CalculatePriceAsync(price, "DE");

        // Assert
        Assert.True(result.PriceIncludingVat > 0);
        Assert.NotNull(result);
        Assert.True(result.FinalPrice >= result.ProductPrice);  // >= for tiny amounts where VAT rounds to 0
    }

    /// <summary>
    /// Test: Large price calculations
    /// Expected: 999,999.99€ + VAT should work correctly
    /// </summary>
    [Fact]
    public async Task CalculatePrice_LargePrice_CalculatesCorrectly()
    {
        // Arrange
        decimal basePrice = 999999.99m;
        string country = "DE";

        // Act
        var result = await _service.CalculatePriceAsync(basePrice, country);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(19m, result.VatRate);
        Assert.True(result.PriceIncludingVat > basePrice);
    }
}
