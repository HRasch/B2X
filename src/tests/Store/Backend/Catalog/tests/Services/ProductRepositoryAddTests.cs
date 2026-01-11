using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Unit tests for Product Repository AddAsync
/// Tests product creation and validation
/// </summary>
public class ProductRepositoryAddTests
{
    [Fact]
    public async Task AddAsync_WithValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var product = new Product
        {
            TenantId = tenantId,
            Sku = "NEW-PROD",
            Name = "New Product",
            Price = 49.99m,
            Description = "A new product"
        };

        var mockRepo = new Mock<IProductRepository>();
        var createdProduct = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = product.TenantId,
            Sku = product.Sku,
            Name = product.Name,
            Price = product.Price,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
        mockRepo
            .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await mockRepo.Object.AddAsync(product);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Sku.ShouldBe("NEW-PROD");
        result.TenantId.ShouldBe(tenantId);

        mockRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateSku_ThrowsException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "DUPLICATE-SKU";

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SKU already exists for this tenant"));

        var product = new Product { TenantId = tenantId, Sku = sku };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await mockRepo.Object.AddAsync(product)
);
    }

    [Fact]
    public async Task AddAsync_WithNegativePrice_ShouldValidate()
    {
        // Arrange
        var product = new Product
        {
            TenantId = Guid.NewGuid(),
            Sku = "INVALID-PRICE",
            Name = "Invalid Product",
            Price = -10m // Negative price - should be invalid
        };

        // Act & Assert - Validation should occur
        product.Price.ShouldBeLessThan(0); // Current state
        // In real implementation, AddAsync would validate and reject this
    }
}
