using Xunit;
using Moq;
using B2X.CatalogService.Services;
using B2X.CatalogService.Models;
using Microsoft.Extensions.Logging;

namespace B2X.CatalogService.Tests;

public class ProductServiceTests
{
    private readonly Mock<ISearchIndexService> _mockSearchIndexService;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _mockSearchIndexService = new Mock<ISearchIndexService>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockLogger.Object, _mockSearchIndexService.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "Test Product",
            Description = "A test product",
            Price = 99.99m,
            StockQuantity = 10
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.CreateAsync(tenantId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD-001", result.Sku);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(99.99m, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        var created = await _productService.CreateAsync(tenantId, request);
        var productId = created.Id;

        // Act
        var result = await _productService.GetByIdAsync(tenantId, productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("PROD-001", result.Sku);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        var result = await _productService.GetByIdAsync(tenantId, productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_ShouldUpdateProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "Original Name",
            Price = 99.99m,
            StockQuantity = 10
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        var created = await _productService.CreateAsync(tenantId, request);
        var productId = created.Id;

        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Price = 199.99m
        };

        // Act
        var result = await _productService.UpdateAsync(tenantId, productId, updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(199.99m, result.Price);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_ShouldDeleteProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        _mockSearchIndexService.Setup(x => x.DeleteProductAsync(It.IsAny<Guid>(), default))
            .Returns(Task.CompletedTask);

        var created = await _productService.CreateAsync(tenantId, request);
        var productId = created.Id;

        // Act
        var deleted = await _productService.DeleteAsync(tenantId, productId);

        // Assert
        Assert.True(deleted);
        var result = await _productService.GetByIdAsync(tenantId, productId);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        for (int i = 1; i <= 25; i++)
        {
            var request = new CreateProductRequest
            {
                Sku = $"PROD-{i:D3}",
                Name = $"Product {i}",
                Price = 10m * i,
                StockQuantity = i
            };

            await _productService.CreateAsync(tenantId, request);
        }

        // Act
        var page1 = await _productService.GetPagedAsync(tenantId, 1, 10);
        var page2 = await _productService.GetPagedAsync(tenantId, 2, 10);
        var page3 = await _productService.GetPagedAsync(tenantId, 3, 10);

        // Assert
        Assert.Equal(10, page1.Items.Count);
        Assert.Equal(10, page2.Items.Count);
        Assert.Equal(5, page3.Items.Count);
        Assert.Equal(25, page1.TotalCount);
        Assert.True(page1.HasNextPage);
        Assert.True(page2.HasNextPage);
        Assert.False(page3.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnSearchResults()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-001",
            Name = "Test Product",
            Price = 99.99m,
            StockQuantity = 10
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        _mockSearchIndexService
            .Setup(x => x.SearchAsync(tenantId, "Test", 1, 20, default))
            .ReturnsAsync(new PagedResult<ProductDto>
            {
                Items = new List<ProductDto>
                {
                    new() { Sku = "PROD-001", Name = "Test Product", Price = 99.99m }
                },
                PageNumber = 1,
                PageSize = 20,
                TotalCount = 1
            });

        await _productService.CreateAsync(tenantId, request);

        // Act
        var result = await _productService.SearchAsync(tenantId, "Test");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_WithCategoryIds_ShouldCreateProductWithCategories()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            Sku = "PROD-CAT-001",
            Name = "Product with Categories",
            Description = "A product with category assignments",
            Price = 149.99m,
            StockQuantity = 5,
            CategoryIds = new List<Guid> { categoryId1, categoryId2 }
        };

        _mockSearchIndexService.Setup(x => x.IndexProductAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.CreateAsync(tenantId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD-CAT-001", result.Sku);
        Assert.Equal("Product with Categories", result.Name);
        Assert.Equal(149.99m, result.Price);
        Assert.NotNull(result.CategoryIds);
        Assert.Equal(2, result.CategoryIds.Count);
        Assert.Contains(categoryId1, result.CategoryIds);
        Assert.Contains(categoryId2, result.CategoryIds);
    }
}
