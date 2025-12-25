using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Validators;
using B2Connect.Types.Localization;

namespace B2Connect.CatalogService.Tests;

/// <summary>
/// Basic validation tests for CatalogService
/// </summary>
public class BasicValidationTests
{
    [Fact]
    public async Task CreateProductValidator_WithValidData_Succeeds()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var validRequest = new CreateProductRequest(
            Sku: "TEST-SKU-001",
            Name: "Test Product",
            Description: "A test product",
            Price: 99.99m,
            B2bPrice: 89.99m,
            StockQuantity: 100,
            Tags: new[] { "test" },
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                ["en"] = new LocalizedContent { Translations = new Dictionary<string, string> { ["name"] = "Test Product" } }
            }
        );

        // Act
        var result = await validator.ValidateAsync(validRequest);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateProductValidator_WithoutSku_Fails()
    {
        // Arrange
        var validator = new CreateProductRequestValidator();
        var invalidRequest = new CreateProductRequest(
            Sku: "",
            Name: "Test Product",
            Description: "A test product",
            Price: 99.99m,
            B2bPrice: 89.99m,
            StockQuantity: 100,
            Tags: new[] { "test" },
            LocalizedNames: new Dictionary<string, LocalizedContent>()
        );

        // Act
        var result = await validator.ValidateAsync(invalidRequest);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
    }

    [Fact]
    public async Task CreateCategoryValidator_WithValidData_Succeeds()
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var validRequest = new CreateCategoryRequest(
            Name: "Test Category",
            LocalizedNames: new Dictionary<string, LocalizedContent>
            {
                ["en"] = new LocalizedContent { Translations = new Dictionary<string, string> { ["name"] = "Test Category" } }
            }
        );

        // Act
        var result = await validator.ValidateAsync(validRequest);

        // Assert
        Assert.True(result.IsValid);
    }
}
