using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Xunit;
using Shouldly;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Validators;
using B2Connect.Types.Localization;

namespace B2Connect.CatalogService.Tests;

/// <summary>
/// Unit tests for Catalog Validators
/// Demonstrates how to test FluentValidation rules
/// </summary>
public class CatalogValidatorsTests
{
    #region CreateProductRequestValidator Tests

    [Fact]
    public async Task CreateProductValidator_WithValidData_Succeeds()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var validRequest = new CreateProductRequest(
            Sku: "PROD-001",
            Name: "Test Product",
            Description: "A valid test product",
            Price: 99.99m,
            B2bPrice: 89.99m,
            StockQuantity: 100,
            Tags: new[] { "test", "sample" },
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                {
                    "en",
                    new LocalizedContent("Test Product", "A test product")
                }
            }
        );

        // Act
        var result = await validator.ValidateAsync(validRequest);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]  // Empty
    [InlineData("AB")]  // Too short
    [InlineData("prod-001")]  // Lowercase (invalid)
    [InlineData("PROD 001")]  // Spaces (invalid)
    public async Task CreateProductValidator_WithInvalidSku_Fails(string sku)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = new CreateProductRequest(
            Sku: sku,
            Name: "Valid Name",
            Description: "Valid description",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 10,
            Tags: new[] { "tag1" },
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                { "en", new LocalizedContent("Name", "Desc") }
            }
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Select(e => e.PropertyName).ShouldContain("Sku");
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]  // Too short
    public async Task CreateProductValidator_WithInvalidName_Fails(string name)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = CreateValidRequest() with { Name = name };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(99.999)]  // More than 2 decimals
    public async Task CreateProductValidator_WithInvalidPrice_Fails(decimal price)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = CreateValidRequest() with { Price = price };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task CreateProductValidator_WithNegativeStock_Fails(int stock)
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = CreateValidRequest() with { StockQuantity = stock };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task CreateProductValidator_WithoutEnglishLocalization_Fails()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = CreateValidRequest() with
        {
            LocalizedNames = new Dictionary<string, LocalizedContent>
            {
                { "de", new LocalizedContent("Produktname", "Beschreibung") }
            }
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        var error = result.Errors.FirstOrDefault(e => e.PropertyName == "LocalizedNames");
        Assert.NotNull(error);
        Assert.Contains("English", error!.ErrorMessage);
    }

    [Fact]
    public async Task CreateProductValidator_WithoutAnyTags_Fails()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var request = CreateValidRequest() with { Tags = Array.Empty<string>() };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion

    #region UpdateProductRequestValidator Tests

    [Fact]
    public async Task UpdateProductValidator_WithPartialData_Succeeds()
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var request = new UpdateProductRequest(
            Sku: "PROD-002",
            Name: "Updated Product",
            Description: null,  // Null is OK for update
            Price: 150m,
            B2bPrice: null,
            StockQuantity: null,  // Null is OK for update
            Tags: null,
            LocalizedNames: null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateProductValidator_WithAllNull_Succeeds()
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var request = new UpdateProductRequest(
            Sku: null,
            Name: null,
            Description: null,
            Price: null,
            B2bPrice: null,
            StockQuantity: null,
            Tags: null,
            LocalizedNames: null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);  // All null = partial update
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task UpdateProductValidator_WithInvalidPrice_Fails(decimal price)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();
        var request = new UpdateProductRequest(
            Sku: null,
            Name: null,
            Description: null,
            Price: price,  // Provided, so validated
            B2bPrice: null,
            StockQuantity: null,
            Tags: null,
            LocalizedNames: null
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion

    #region CreateCategoryRequestValidator Tests

    [Fact]
    public async Task CreateCategoryValidator_WithValidData_Succeeds()
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest(
            Name: "Electronics",
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                { "en", new LocalizedContent("Electronics", "Electronic devices") }
            }
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]  // Too short
    public async Task CreateCategoryValidator_WithInvalidName_Fails(string name)
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest(
            Name: name,
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                { "en", new LocalizedContent("Valid", "Name") }
            }
        );

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion

    #region Helper Methods

    private static CreateProductRequest CreateValidRequest()
    {
        return new CreateProductRequest(
            Sku: "PROD-001",
            Name: "Valid Product",
            Description: "Valid description",
            Price: 99.99m,
            B2bPrice: 89.99m,
            StockQuantity: 100,
            Tags: new[] { "tag1" },
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                { "en", new LocalizedContent("Product Name", "Description") }
            }
        );
    }

    #endregion
}
