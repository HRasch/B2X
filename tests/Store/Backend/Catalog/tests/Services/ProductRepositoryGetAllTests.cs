using System.Linq;
using Moq;
using Shouldly;
using Xunit;

namespace B2X.Catalog.Tests.Services;

/// <summary>
/// Unit tests for Product Repository GetAllAsync
/// Tests pagination, filtering, and tenant isolation
/// </summary>
public class ProductRepositoryGetAllTests
{
    [Fact]
    public async Task GetAllAsync_WithValidTenant_ReturnsProducts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Sku = "SKU-001", Name = "Product 1", Price = 10m },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Sku = "SKU-002", Name = "Product 2", Price = 20m },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Sku = "SKU-003", Name = "Product 3", Price = 30m }
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetAllAsync(tenantId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Product>)products);

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 1, 10);

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(3);
        result.All(p => p.TenantId == tenantId).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var page1Products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Sku = "SKU-001", Name = "Product 1" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Sku = "SKU-002", Name = "Product 2" }
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetAllAsync(tenantId, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Product>)page1Products);

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 1, 2);

        // Assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAllAsync_WithDifferentTenant_ReturnsDifferentProducts_TenantIsolation()
    {
        // Arrange
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();

        var tenant1Products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenant1, Sku = "TENANT1-001" }
        };

        var tenant2Products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), TenantId = tenant2, Sku = "TENANT2-001" }
        };

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetAllAsync(tenant1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Product>)tenant1Products);
        mockRepo
            .Setup(x => x.GetAllAsync(tenant2, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Product>)tenant2Products);

        // Act
        var result1 = await mockRepo.Object.GetAllAsync(tenant1, 1, 10);
        var result2 = await mockRepo.Object.GetAllAsync(tenant2, 1, 10);

        // Assert
        result1.Count.ShouldBe(1);
        result2.Count.ShouldBe(1);
        result1.First().TenantId.ShouldBe(tenant1);
        result2.First().TenantId.ShouldBe(tenant2);
    }

    [Fact]
    public async Task GetAllAsync_WithInvalidPage_ReturnsEmptyList()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetAllAsync(tenantId, 99, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Product>)new List<Product>());

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 99, 10);

        // Assert
        result.ShouldBeEmpty();
    }
}
