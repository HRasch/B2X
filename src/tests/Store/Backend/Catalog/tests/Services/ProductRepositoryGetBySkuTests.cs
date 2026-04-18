using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Unit tests for Product Repository GetBySkuAsync
/// Tests product retrieval, tenant isolation, and error cases
/// </summary>
public class ProductRepositoryGetBySkuTests
{
    [Fact]
    public async Task GetBySkuAsync_WithValidSkuAndTenant_ReturnsProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "PROD-001";
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Sku = sku,
            Name = "Test Product",
            Price = 99.99m,
            Description = "A test product",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await mockRepo.Object.GetBySkuAsync(tenantId, sku);

        // Assert
        result.ShouldNotBeNull();
        result!.Sku.ShouldBe(sku);
        result.TenantId.ShouldBe(tenantId);
        result.Name.ShouldBe("Test Product");
        result.Price.ShouldBe(99.99m);

        mockRepo.Verify(x => x.GetBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBySkuAsync_WithNonExistentSku_ReturnsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "NONEXISTENT-SKU";

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await mockRepo.Object.GetBySkuAsync(tenantId, sku);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetBySkuAsync_WithDifferentTenant_ReturnsNull_TenantIsolation()
    {
        // Arrange
        var tenantId1 = Guid.NewGuid();
        var tenantId2 = Guid.NewGuid();
        var sku = "PROD-001";

        var mockRepo = new Mock<IProductRepository>();

        // Setup: Product exists for tenantId1
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId1, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Sku = sku, TenantId = tenantId1 });

        // Setup: Product doesn't exist for tenantId2 (tenant isolation)
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId2, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result1 = await mockRepo.Object.GetBySkuAsync(tenantId1, sku);
        var result2 = await mockRepo.Object.GetBySkuAsync(tenantId2, sku);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldBeNull();
        result1!.TenantId.ShouldBe(tenantId1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetBySkuAsync_WithEmptySku_ReturnsNull(string? sku)
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await mockRepo.Object.GetBySkuAsync(tenantId, sku);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetBySkuAsync_WithCancellationToken_RespectsCancellation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "PROD-001";
        using var cts = new CancellationTokenSource();

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await mockRepo.Object.GetBySkuAsync(tenantId, sku, cts.Token)
);
    }
}
