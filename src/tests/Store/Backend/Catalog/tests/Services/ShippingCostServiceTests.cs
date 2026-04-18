using B2X.Catalog.Models;
using B2X.Catalog.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2X.Catalog.Tests.Services;

public class ShippingCostServiceTests : IAsyncLifetime
{
    private ShippingCostService _service = null!;
    private Mock<ILogger<ShippingCostService>> _mockLogger = null!;

    public async ValueTask InitializeAsync()
    {
        _mockLogger = new Mock<ILogger<ShippingCostService>>();
        _service = new ShippingCostService(_mockLogger.Object);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetShippingMethodsAsync_ValidCountry_ReturnsAvailableMethods()
    {
        // Arrange
        var country = "DE";

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Methods);
        Assert.Equal(3, result.Methods.Count);
    }

    [Fact]
    public async Task GetShippingMethodsAsync_Germany_ReturnsCorrectCosts()
    {
        // Arrange
        var country = "DE";
        var expectedCosts = new[] { 3.99m, 4.99m, 5.99m };

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.All(result.Methods, m => Assert.Equal("EUR", m.CurrencyCode));
        Assert.Equal(expectedCosts, result.Methods.Select(m => m.Cost).OrderBy(c => c));
    }

    [Fact]
    public async Task GetShippingMethodsAsync_AustriaWithMultiplier_ReturnsAdjustedCosts()
    {
        // Arrange
        var country = "AT"; // 1.5x multiplier
        var expectedDHL = 4.99m * 1.5m; // 7.49

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        var dhlMethod = result.Methods.FirstOrDefault(m => string.Equals(m.Id, "dhl-express", StringComparison.Ordinal));
        Assert.NotNull(dhlMethod);
        Assert.Equal(Math.Round(expectedDHL, 2), dhlMethod.Cost);
    }

    [Fact]
    public async Task GetShippingMethodsAsync_SwitzerlandWithHighMultiplier_ReturnsExpensiveShipping()
    {
        // Arrange
        var country = "CH"; // 2.0x multiplier
        var expectedDHL = 4.99m * 2.0m; // 9.98

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        var dhlMethod = result.Methods.FirstOrDefault(m => string.Equals(m.Id, "dhl-express", StringComparison.Ordinal));
        Assert.NotNull(dhlMethod);
        Assert.Equal(Math.Round(expectedDHL, 2), dhlMethod.Cost);
    }

    [Fact]
    public async Task GetShippingMethodsAsync_ExceedsMaxWeight_FiltersOutMethods()
    {
        // Arrange
        var country = "DE";
        var tooHeavyWeight = 25m; // PostNL max is 20kg

        // Act
        var result = await _service.GetShippingMethodsAsync(country, totalWeight: tooHeavyWeight, cancellationToken: CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.DoesNotContain(result.Methods, m => string.Equals(m.Provider, "PostNL", StringComparison.Ordinal));
        Assert.Equal(2, result.Methods.Count); // Only DHL and DPD
    }

    [Fact]
    public async Task GetShippingMethodsAsync_EmptyCountry_ReturnsFailed()
    {
        // Arrange
        var country = "";

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Empty(result.Methods);
    }

    [Fact]
    public async Task CalculateTotalAsync_WithShippingCost_ReturnsCorrectTotal()
    {
        // Arrange
        var subtotal = 100m;
        var shippingMethodId = "dhl-express";
        var country = "DE";
        var expectedTotal = 104.99m;

        // Act
        var result = await _service.CalculateTotalAsync(
            subtotal, shippingMethodId, country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(expectedTotal, result);
    }

    [Fact]
    public async Task CalculateTotalAsync_WithCountryMultiplier_IncludesAdjustedShipping()
    {
        // Arrange
        var subtotal = 100m;
        var shippingMethodId = "dhl-express";
        var country = "AT"; // 1.5x multiplier
        var expectedShipping = Math.Round(4.99m * 1.5m, 2); // 7.49
        var expectedTotal = 100m + expectedShipping; // 107.49

        // Act
        var result = await _service.CalculateTotalAsync(
            subtotal, shippingMethodId, country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(expectedTotal, result);
    }

    [Fact]
    public async Task CalculateTotalAsync_InvalidShippingMethod_ReturnSubtotalOnly()
    {
        // Arrange
        var subtotal = 100m;
        var invalidMethodId = "invalid-method";
        var country = "DE";

        // Act
        var result = await _service.CalculateTotalAsync(
            subtotal, invalidMethodId, country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(subtotal, result);
    }

    [Fact]
    public void GetFreeShippingThreshold_Germany_Returns50()
    {
        // Act
        var threshold = _service.GetFreeShippingThreshold("DE");

        // Assert
        Assert.Equal(50.00m, threshold);
    }

    [Fact]
    public void GetFreeShippingThreshold_Austria_Returns75()
    {
        // Act
        var threshold = _service.GetFreeShippingThreshold("AT");

        // Assert
        Assert.Equal(75.00m, threshold);
    }

    [Fact]
    public void GetFreeShippingThreshold_France_Returns100()
    {
        // Act
        var threshold = _service.GetFreeShippingThreshold("FR");

        // Assert
        Assert.Equal(100.00m, threshold);
    }

    [Fact]
    public void GetFreeShippingThreshold_UnknownCountry_Returns150()
    {
        // Act
        var threshold = _service.GetFreeShippingThreshold("XX");

        // Assert
        Assert.Equal(150.00m, threshold);
    }

    [Fact]
    public async Task GetShippingMethodsAsync_AllMethodsHaveValidDates()
    {
        // Arrange
        var country = "DE";

        // Act
        var result = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);

        // Assert
        Assert.All(result.Methods, m =>
        {
            Assert.NotEmpty(m.Name);
            Assert.NotEmpty(m.Provider);
            Assert.True(m.Cost > 0);
            Assert.True(m.EstimatedDaysMin > 0);
            Assert.True(m.EstimatedDaysMax >= m.EstimatedDaysMin);
        });
    }

    [Fact]
    public async Task GetShippingMethodsAsync_MultipleCountries_ReturnsConsistentMethods()
    {
        // Arrange
        var countries = new[] { "DE", "AT", "FR", "CH" };

        // Act
        var results = new Dictionary<string, GetShippingMethodsResponse>(StringComparer.Ordinal);
        foreach (var country in countries)
        {
            results[country] = await _service.GetShippingMethodsAsync(country, cancellationToken: CancellationToken.None);
        }

        // Assert
        Assert.All(results.Values, r =>
        {
            Assert.True(r.Success);
            Assert.NotEmpty(r.Methods);
        });
    }
}
