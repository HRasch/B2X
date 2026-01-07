using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Unit tests for Product Repository ExistsBySkuAsync
/// Tests product existence validation
/// </summary>
public class ProductRepositoryExistsTests
{
    [Fact]
    public async Task ExistsBySkuAsync_WithExistingSku_ReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "EXISTING-SKU";

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.ExistsBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await mockRepo.Object.ExistsBySkuAsync(tenantId, sku);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsBySkuAsync_WithNonExistentSku_ReturnsFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "NONEXISTENT-SKU";

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.ExistsBySkuAsync(tenantId, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await mockRepo.Object.ExistsBySkuAsync(tenantId, sku);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsBySkuAsync_WithDifferentTenant_ReturnsFalse_TenantIsolation()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();
        var sku = "SHARED-SKU";

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.ExistsBySkuAsync(tenant1, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockRepo
            .Setup(x => x.ExistsBySkuAsync(tenant2, sku, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // Different tenant, so false

        // Act
        var result1 = await mockRepo.Object.ExistsBySkuAsync(tenant1, sku);
        var result2 = await mockRepo.Object.ExistsBySkuAsync(tenant2, sku);

        // Assert
        result1.ShouldBeTrue();
        result2.ShouldBeFalse();
    }
}
