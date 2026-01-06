
using B2Connect.Catalog.Application.Handlers;
using B2Connect.Catalog.Application.Validators;
using Xunit;

namespace B2Connect.Catalog.Tests.Application.Validators;
public class CalculatePriceValidatorTests
{
    private readonly CalculatePriceValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_Passes()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(99.99m, "DE");

        // Act
        var result = await _validator.ValidateAsync(cmd);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_ZeroPrice_Fails()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(0m, "DE");

        // Act
        var result = await _validator.ValidateAsync(cmd);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductPrice");
    }

    [Fact]
    public async Task Validate_NegativePrice_Fails()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(-10m, "DE");

        // Act
        var result = await _validator.ValidateAsync(cmd);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyCountry_Fails()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(99.99m, "");

        // Act
        var result = await _validator.ValidateAsync(cmd);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DestinationCountry");
    }

    [Fact]
    public async Task Validate_InvalidCountryCode_Fails()
    {
        // Arrange
        var cmd = new CalculatePriceCommand(99.99m, "DEU"); // Wrong format

        // Act
        var result = await _validator.ValidateAsync(cmd);

        // Assert
        Assert.False(result.IsValid);
    }
}
