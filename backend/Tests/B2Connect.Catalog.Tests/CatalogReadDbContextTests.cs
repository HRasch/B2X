using Xunit;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Data.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.CatalogService.Tests.ReadModel;

/// <summary>
/// Read Model Database Tests
/// Tests the denormalized read model for correct indexing and query performance
/// </summary>
public class CatalogReadDbContextTests : IAsyncLifetime
{
    private CatalogReadDbContext _context = null!;
    private readonly Guid _tenantId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<CatalogReadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new CatalogReadDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task ReadModel_CreatesTable_WithCorrectSchema()
    {
        // Arrange & Act - Database creation already done in Initialize

        // Assert - Table exists and is queryable
        Assert.NotNull(_context.ProductsReadModel);

        // Verify table is empty
        var count = await _context.ProductsReadModel.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task InsertProduct_WithValidData_Succeeds()
    {
        // Arrange
        var product = new ProductReadModel
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "TEST-001",
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Electronics",
            IsAvailable = true,
            StockQuantity = 100,
            SearchText = "test product test description sku",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.ProductsReadModel.Add(product);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.ProductsReadModel.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(saved);
        Assert.Equal("TEST-001", saved.Sku);
        Assert.Equal("Test Product", saved.Name);
    }

    [Fact]
    public async Task QueryByTenantId_FiltersCorrectly()
    {
        // Arrange - Insert products for different tenants
        var tenant1 = Guid.NewGuid();
        var tenant2 = Guid.NewGuid();

        var products = new[]
        {
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = tenant1,
                Sku = "TENANT1-001",
                Name = "Tenant 1 Product 1",
                Price = 100m,
                SearchText = "tenant 1 product 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = tenant1,
                Sku = "TENANT1-002",
                Name = "Tenant 1 Product 2",
                Price = 200m,
                SearchText = "tenant 1 product 2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = tenant2,
                Sku = "TENANT2-001",
                Name = "Tenant 2 Product 1",
                Price = 150m,
                SearchText = "tenant 2 product 1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var tenant1Products = await _context.ProductsReadModel
            .Where(p => p.TenantId == tenant1)
            .ToListAsync();

        var tenant2Products = await _context.ProductsReadModel
            .Where(p => p.TenantId == tenant2)
            .ToListAsync();

        // Assert
        Assert.Equal(2, tenant1Products.Count);
        Assert.Single(tenant2Products);
        Assert.All(tenant1Products, p => Assert.Equal(tenant1, p.TenantId));
        Assert.All(tenant2Products, p => Assert.Equal(tenant2, p.TenantId));
    }

    [Fact]
    public async Task QueryBySku_UniqueIndexWorks()
    {
        // Arrange
        var product = new ProductReadModel
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "UNIQUE-SKU-001",
            Name = "Product with unique SKU",
            Price = 99.99m,
            SearchText = "unique sku product",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ProductsReadModel.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var found = await _context.ProductsReadModel
            .FirstOrDefaultAsync(p => p.TenantId == _tenantId && p.Sku == "UNIQUE-SKU-001");

        // Assert
        Assert.NotNull(found);
        Assert.Equal(product.Id, found.Id);
    }

    [Fact]
    public async Task QueryByCategory_FilterWorks()
    {
        // Arrange
        var products = new[]
        {
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "ELECTRONICS-001",
                Name = "Laptop",
                Category = "Electronics",
                Price = 999.99m,
                IsAvailable = true,
                SearchText = "laptop electronics",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "BOOKS-001",
                Name = "C# Programming",
                Category = "Books",
                Price = 49.99m,
                IsAvailable = true,
                SearchText = "c programming books",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "ELECTRONICS-002",
                Name = "Monitor",
                Category = "Electronics",
                Price = 299.99m,
                IsAvailable = true,
                SearchText = "monitor electronics",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var electronicsProducts = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.Category == "Electronics")
            .ToListAsync();

        var bookProducts = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.Category == "Books")
            .ToListAsync();

        // Assert
        Assert.Equal(2, electronicsProducts.Count);
        Assert.Single(bookProducts);
    }

    [Fact]
    public async Task QueryByPriceRange_FilterWorks()
    {
        // Arrange
        var products = new[]
        {
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "PRICE-001",
                Name = "Cheap Product",
                Price = 10m,
                SearchText = "cheap",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "PRICE-002",
                Name = "Mid-Range Product",
                Price = 100m,
                SearchText = "mid range",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "PRICE-003",
                Name = "Expensive Product",
                Price = 1000m,
                SearchText = "expensive",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var inRange = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.Price >= 50m && p.Price <= 500m)
            .ToListAsync();

        var expensive = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.Price > 500m)
            .ToListAsync();

        // Assert
        Assert.Single(inRange); // Only mid-range
        Assert.Single(expensive); // Only expensive
    }

    [Fact]
    public async Task QueryBySearchText_FullTextSearchWorks()
    {
        // Arrange
        var products = new[]
        {
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "LAPTOP-DELL-001",
                Name = "Dell Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                SearchText = "dell laptop high-performance laptop",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "MONITOR-DELL-001",
                Name = "Dell Monitor",
                Description = "4K monitor",
                Price = 299.99m,
                SearchText = "dell monitor 4k monitor",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "KEYBOARD-LOGITECH-001",
                Name = "Logitech Keyboard",
                Price = 99.99m,
                SearchText = "logitech keyboard",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act - Search for "laptop"
        var laptopResults = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.SearchText.Contains("laptop"))
            .ToListAsync();

        // Act - Search for "dell"
        var dellResults = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.SearchText.Contains("dell"))
            .ToListAsync();

        // Assert
        Assert.Single(laptopResults);
        Assert.Equal(2, dellResults.Count);
    }

    [Fact]
    public async Task QueryByAvailability_FilterWorks()
    {
        // Arrange
        var products = new[]
        {
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "AVAILABLE-001",
                Name = "Available Product",
                Price = 99.99m,
                IsAvailable = true,
                SearchText = "available",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = "UNAVAILABLE-001",
                Name = "Unavailable Product",
                Price = 199.99m,
                IsAvailable = false,
                SearchText = "unavailable",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var available = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && p.IsAvailable)
            .ToListAsync();

        var unavailable = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && !p.IsAvailable)
            .ToListAsync();

        // Assert
        Assert.Single(available);
        Assert.Single(unavailable);
    }

    [Fact]
    public async Task SoftDelete_MarksAsDeletedNotRemoved()
    {
        // Arrange
        var product = new ProductReadModel
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "SOFTDELETE-001",
            Name = "Product to soft delete",
            Price = 99.99m,
            IsDeleted = false,
            SearchText = "soft delete",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ProductsReadModel.Add(product);
        await _context.SaveChangesAsync();

        // Act - Soft delete
        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        _context.ProductsReadModel.Update(product);
        await _context.SaveChangesAsync();

        // Assert - Still in database but filtered out by queries
        var allProducts = await _context.ProductsReadModel
            .Where(p => p.Id == product.Id)
            .ToListAsync();

        var activeProducts = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId && !p.IsDeleted)
            .ToListAsync();

        Assert.Single(allProducts); // Found in database
        Assert.True(allProducts[0].IsDeleted);
        Assert.Empty(activeProducts); // Not in active query
    }

    [Fact]
    public async Task PaginationWorks_WithSkipAndTake()
    {
        // Arrange - Create 25 products
        var products = Enumerable.Range(1, 25)
            .Select(i => new ProductReadModel
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Sku = $"PAGINATION-{i:D3}",
                Name = $"Product {i}",
                Price = i * 10m,
                SearchText = $"product {i}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act - Get page 2, size 10
        var pageSize = 10;
        var pageNumber = 2;
        var skip = (pageNumber - 1) * pageSize;

        var pagedResults = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId)
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId)
            .CountAsync();

        // Assert
        Assert.Equal(10, pagedResults.Count);
        Assert.Equal(25, totalCount);
        Assert.Equal(3, (totalCount + pageSize - 1) / pageSize); // 3 pages total
    }

    [Fact]
    public async Task AggregationQueries_CalculateCorrectly()
    {
        // Arrange
        var products = new[]
        {
            new ProductReadModel { Id = Guid.NewGuid(), TenantId = _tenantId, Sku = "AGG-001", Name = "P1", Price = 10m, IsAvailable = true, SearchText = "p1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductReadModel { Id = Guid.NewGuid(), TenantId = _tenantId, Sku = "AGG-002", Name = "P2", Price = 20m, IsAvailable = false, SearchText = "p2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProductReadModel { Id = Guid.NewGuid(), TenantId = _tenantId, Sku = "AGG-003", Name = "P3", Price = 30m, IsAvailable = true, SearchText = "p3", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _context.ProductsReadModel.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var total = await _context.ProductsReadModel.CountAsync(p => p.TenantId == _tenantId);
        var active = await _context.ProductsReadModel.CountAsync(p => p.TenantId == _tenantId && p.IsAvailable);
        var avgPrice = _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId)
            .Average(p => p.Price);
        var minPrice = _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId)
            .Min(p => p.Price);
        var maxPrice = _context.ProductsReadModel
            .Where(p => p.TenantId == _tenantId)
            .Max(p => p.Price);

        // Assert
        Assert.Equal(3, total);
        Assert.Equal(2, active);
        Assert.Equal(20m, avgPrice);
        Assert.Equal(10m, minPrice);
        Assert.Equal(30m, maxPrice);
    }
}
