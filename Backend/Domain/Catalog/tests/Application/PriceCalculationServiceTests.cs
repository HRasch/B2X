
using B2Connect.Catalog.Application.Handlers;
using B2Connect.Catalog.Application.Validators;
using B2Connect.Catalog.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Catalog.Tests.Application;
public class PriceCalculationServiceTests : IAsyncLifetime
{
    private readonly Mock<ITaxRateService> _mockTaxService = new();
    private readonly Mock<ILogger<PriceCalculationService>> _mockLogger = new();
    private PriceCalculationService _service = null!;
    private CalculatePriceValidator _validator = null!;

    public async Task InitializeAsync()
    {
        _validator = new CalculatePriceValidator();

        _service = new PriceCalculationService(_mockTaxService.Object, _mockLogger.Object, _validator);

        // Setup default tax rates
        _mockTaxService.Setup(x => x.GetVatRateAsync("DE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(19.00m);
        _mockTaxService.Setup(x => x.GetVatRateAsync("AT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(20.00m);
        _mockTaxService.Setup(x => x.GetVatRateAsync("FR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(20.00m);

        await Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CalculatePrice_Germany_Applies19PercentVat()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(100m, "DE");

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Breakdown);
        Assert.Equal(19m, result.Breakdown.VatAmount);
        Assert.Equal(119m, result.Breakdown.TotalWithVat);
    }

    [Fact]
    public async Task CalculatePrice_Austria_Applies20PercentVat()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(100m, "AT");

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Breakdown);
        Assert.Equal(20m, result.Breakdown.VatAmount);
        Assert.Equal(120m, result.Breakdown.TotalWithVat);
    }

    [Theory]
    [InlineData("DE", 100, 119)]      // 19% VAT
    [InlineData("AT", 100, 120)]      // 20% VAT
    [InlineData("FR", 50, 60)]        // 20% VAT
    public async Task CalculatePrice_MultipleCountries_CalculatesCorrectly(
        string country, decimal price, decimal expectedTotal)
    {
        // Arrange
        var cmd = new CalculatePriceCommand(price, country);

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedTotal, result.Breakdown!.TotalWithVat);
    }

    [Fact]
    public async Task CalculatePrice_WithShipping_IncludesShippingInFinalTotal()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(100m, "DE", ShippingCost: 9.99m);

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(9.99m, result.Breakdown!.ShippingCost);
        Assert.Equal(128.99m, result.Breakdown.FinalTotal);
    }

    [Fact]
    public async Task CalculatePrice_InvalidPrice_ReturnsFailed()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(-10m, "DE");

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("VALIDATION_ERROR", result.Error);
    }

    [Fact]
    public async Task CalculatePrice_InvalidCountry_ReturnsFailed()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(100m, "XX"); // Invalid country code

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("VALIDATION_ERROR", result.Error);
    }

    [Fact]
    public async Task CalculatePrice_ValidatesInput()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(0m, "DE"); // Invalid: price = 0

        // Act
        var result = await _service.CalculatePrice(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("must be greater than 0", result.Message);
    }

    [Fact]
    public async Task GetPriceBreakdown_CallsCalculatePrice()
    {
        // Arrange
        var query = new GetPriceBreakdownQuery(100m, "DE", 10m);

        // Act
        var result = await _service.GetPriceBreakdown(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Breakdown);
        Assert.Equal(129m, result.Breakdown.FinalTotal); // 100 + 19 VAT + 10 shipping
    }
}
