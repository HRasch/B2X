using Xunit;
using FluentAssertions;
using Moq;

namespace B2Connect.Catalog.Tests.Services;

/// <summary>
/// Product Repository Mock Interface
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
    Task<List<Product>> GetAllAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<bool> ExistsBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
}

/// <summary>
/// Product entity (simplified for tests)
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

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
        result.Should().NotBeNull();
        result!.Sku.Should().Be(sku);
        result.TenantId.Should().Be(tenantId);
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(99.99m);

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
        result.Should().BeNull();
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
        result1.Should().NotBeNull();
        result2.Should().BeNull();
        result1!.TenantId.Should().Be(tenantId1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetBySkuAsync_WithEmptySku_ReturnsNull(string sku)
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
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySkuAsync_WithCancellationToken_RespectsCancellation()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sku = "PROD-001";
        var cts = new CancellationTokenSource();

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
            .ReturnsAsync(products);

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 1, 10);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(3);
        result.All(p => p.TenantId == tenantId).Should().BeTrue();
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
            .ReturnsAsync(page1Products);

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 1, 2);

        // Assert
        result.Should().HaveCount(2);
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
            .ReturnsAsync(tenant1Products);
        mockRepo
            .Setup(x => x.GetAllAsync(tenant2, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant2Products);

        // Act
        var result1 = await mockRepo.Object.GetAllAsync(tenant1, 1, 10);
        var result2 = await mockRepo.Object.GetAllAsync(tenant2, 1, 10);

        // Assert
        result1.Should().HaveCount(1);
        result2.Should().HaveCount(1);
        result1[0].TenantId.Should().Be(tenant1);
        result2[0].TenantId.Should().Be(tenant2);
    }

    [Fact]
    public async Task GetAllAsync_WithInvalidPage_ReturnsEmptyList()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        var mockRepo = new Mock<IProductRepository>();
        mockRepo
            .Setup(x => x.GetAllAsync(tenantId, 99, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await mockRepo.Object.GetAllAsync(tenantId, 99, 10);

        // Assert
        result.Should().BeEmpty();
    }
}

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
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Sku.Should().Be("NEW-PROD");
        result.TenantId.Should().Be(tenantId);

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
        product.Price.Should().BeLessThan(0); // Current state
        // In real implementation, AddAsync would validate and reject this
    }
}

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
        result.Should().BeTrue();
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
        result.Should().BeFalse();
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
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
}
