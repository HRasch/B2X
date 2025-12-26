using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.CatalogService.Tests;

public class ProductServiceTests
{
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<ISearchIndexService> _mockSearchIndex;
    private readonly ProductService _service;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ProductServiceTests()
    {
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockSearchIndex = new Mock<ISearchIndexService>();
        _service = new ProductService(_mockLogger.Object, _mockSearchIndex.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "SKU-001",
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10,
            Categories = new() { "Electronics" }
        };

        // Act
        var result = await _service.CreateAsync(_tenantId, request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("SKU-001", result.Sku);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(99.99m, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingProduct_ReturnsProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "SKU-002",
            Name = "Test Product 2",
            Price = 49.99m,
            StockQuantity = 5
        };

        var created = await _service.CreateAsync(_tenantId, request);

        // Act
        var result = await _service.GetByIdAsync(_tenantId, created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("SKU-002", result.Sku);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResults()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            var request = new CreateProductRequest
            {
                Sku = $"SKU-{i:000}",
                Name = $"Product {i}",
                Price = 10m + i,
                StockQuantity = i
            };
            await _service.CreateAsync(_tenantId, request);
        }

        // Act
        var result = await _service.GetPagedAsync(_tenantId, pageNumber: 1, pageSize: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_UpdatesProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "SKU-003",
            Name = "Original Name",
            Price = 100m,
            StockQuantity = 10
        };

        var created = await _service.CreateAsync(_tenantId, request);

        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Price = 150m
        };

        // Act
        var result = await _service.UpdateAsync(_tenantId, created.Id, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(150m, result.Price);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_DeletesProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "SKU-004",
            Name = "To Delete",
            Price = 50m,
            StockQuantity = 1
        };

        var created = await _service.CreateAsync(_tenantId, request);

        // Act
        var deleted = await _service.DeleteAsync(_tenantId, created.Id);
        var retrieved = await _service.GetByIdAsync(_tenantId, created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingProducts()
    {
        // Arrange
        await _service.CreateAsync(_tenantId, new CreateProductRequest
        {
            Sku = "PHONE-001",
            Name = "Smartphone X",
            Description = "Latest smartphone",
            Price = 999m,
            StockQuantity = 5
        });

        await _service.CreateAsync(_tenantId, new CreateProductRequest
        {
            Sku = "LAPTOP-001",
            Name = "Gaming Laptop",
            Description = "High performance laptop",
            Price = 1299m,
            StockQuantity = 3
        });

        // Act
        var result = await _service.SearchAsync(_tenantId, "Smartphone");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Smartphone X", result.Items[0].Name);
    }

    [Fact]
    public async Task IsAvailable_CalculatesCorrectly()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Sku = "STOCK-001",
            Name = "Test Product",
            Price = 50m,
            StockQuantity = 10
        };

        var created = await _service.CreateAsync(_tenantId, request);

        // Assert
        Assert.True(created.IsAvailable);

        // Update to out of stock
        var updateRequest = new UpdateProductRequest { StockQuantity = 0 };
        var updated = await _service.UpdateAsync(_tenantId, created.Id, updateRequest);

        Assert.False(updated.IsAvailable);
    }
}
