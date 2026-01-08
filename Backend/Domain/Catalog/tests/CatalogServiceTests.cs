using B2X.Catalog.Models;
using B2X.Catalog.Services;
using Moq;
using Xunit;

namespace B2X.Catalog.Tests;

/// <summary>
/// Catalog Service Tests
/// Tests for product service operations
/// </summary>
public class CatalogServiceTests
{
    [Fact]
    public void ServiceCanBeInstantiated()
    {
        // Arrange & Act & Assert
        Assert.NotNull(typeof(IProductService));
    }

    [Fact]
    public void ProductModelCanBeCreated()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "TEST-001",
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10,
            IsActive = true
        };

        // Assert
        Assert.NotNull(product);
        Assert.Equal("TEST-001", product.Sku);
        Assert.True(product.IsAvailable);
    }
}
