using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.CQRS.Events;
using B2Connect.CatalogService.CQRS.Handlers.Commands;
using B2Connect.CatalogService.CQRS.Handlers.Queries;
using B2Connect.CatalogService.CQRS.Handlers.Events;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Data.ReadModel;
using B2Connect.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.CatalogService.Tests.CQRS;

/// <summary>
/// End-to-End CQRS Tests
/// Tests the complete flow: Command → Write Model → Event → Read Model → Query
/// </summary>
public class CQRSEndToEndTests : IAsyncLifetime
{
    private CatalogDbContext _writeDb = null!;
    private CatalogReadDbContext _readDb = null!;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public CQRSEndToEndTests()
    {
        _logger = new Mock<ILogger<ProductCreatedEventHandler>>().Object;
    }

    public async Task InitializeAsync()
    {
        // Setup write model in-memory database
        var writeOptions = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _writeDb = new CatalogDbContext(writeOptions);
        await _writeDb.Database.EnsureCreatedAsync();

        // Setup read model in-memory database
        var readOptions = new DbContextOptionsBuilder<CatalogReadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _readDb = new CatalogReadDbContext(readOptions);
        await _readDb.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _writeDb.DisposeAsync();
        await _readDb.DisposeAsync();
    }

    [Fact]
    public async Task CreateProduct_PublishesEvent_UpdatesReadModel()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "TEST-SKU-001",
            Name = "Test Product",
            Price = 99.99m,
            Description = "A test product",
            Category = "Electronics",
            IsAvailable = true,
            StockQuantity = 100
        };

        var createHandler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());

        // Act
        var result = await createHandler.Handle(command, CancellationToken.None);

        // Assert - Command executed
        Assert.True(result.Success);
        Assert.NotNull(result.Id);

        // Assert - Written to write model
        var writeModelProduct = await _writeDb.Products.FirstOrDefaultAsync(p => p.Id == result.Id);
        Assert.NotNull(writeModelProduct);
        Assert.Equal("TEST-SKU-001", writeModelProduct.Sku);
        Assert.Equal("Test Product", writeModelProduct.Name);
        Assert.Equal(99.99m, writeModelProduct.Price);

        // Simulate event handler updating read model
        var evt = new ProductCreatedEvent
        {
            ProductId = result.Id,
            TenantId = _tenantId,
            Sku = command.Sku,
            Name = command.Name,
            Price = command.Price,
            Description = command.Description,
            Category = command.Category,
            IsAvailable = command.IsAvailable,
            StockQuantity = command.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
        await eventHandler.Handle(evt, CancellationToken.None);

        // Assert - Read model updated
        var readModelProduct = await _readDb.ProductsReadModel
            .FirstOrDefaultAsync(p => p.Id == result.Id);
        Assert.NotNull(readModelProduct);
        Assert.Equal(_tenantId, readModelProduct.TenantId);
        Assert.Equal("TEST-SKU-001", readModelProduct.Sku);
        Assert.Equal("Test Product", readModelProduct.Name);
        Assert.False(readModelProduct.IsDeleted);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesReadModel_QueryReturnsNewData()
    {
        // Arrange - Create product first
        var productId = Guid.NewGuid();
        var createCommand = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "UPDATE-TEST-001",
            Name = "Original Name",
            Price = 50.00m,
            IsAvailable = true,
            StockQuantity = 50
        };

        var createHandler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
        var createResult = await createHandler.Handle(createCommand, CancellationToken.None);

        // Create event and update read model
        var createEvent = new ProductCreatedEvent
        {
            ProductId = createResult.Id,
            TenantId = _tenantId,
            Sku = createCommand.Sku,
            Name = createCommand.Name,
            Price = createCommand.Price,
            IsAvailable = createCommand.IsAvailable,
            StockQuantity = createCommand.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
        await eventHandler.Handle(createEvent, CancellationToken.None);

        // Act - Update product
        var updateCommand = new UpdateProductCommand
        {
            TenantId = _tenantId,
            ProductId = createResult.Id,
            Name = "Updated Name",
            Price = 75.00m,
            StockQuantity = 30
        };

        var updateHandler = new UpdateProductCommandHandler(_writeDb, Mock.Of<ILogger<UpdateProductCommandHandler>>());
        var updateResult = await updateHandler.Handle(updateCommand, CancellationToken.None);

        // Assert - Write model updated
        Assert.True(updateResult.Success);

        var updatedWriteModel = await _writeDb.Products.FirstOrDefaultAsync(p => p.Id == createResult.Id);
        Assert.Equal("Updated Name", updatedWriteModel!.Name);
        Assert.Equal(75.00m, updatedWriteModel.Price);

        // Act - Update read model via event
        var updateEvent = new ProductUpdatedEvent
        {
            ProductId = createResult.Id,
            TenantId = _tenantId,
            Name = updateCommand.Name,
            Price = updateCommand.Price,
            StockQuantity = updateCommand.StockQuantity,
            UpdatedAt = DateTime.UtcNow
        };

        var updateEventHandler = new ProductUpdatedEventHandler(_readDb, Mock.Of<ILogger<ProductUpdatedEventHandler>>());
        await updateEventHandler.Handle(updateEvent, CancellationToken.None);

        // Assert - Read model updated
        var readModel = await _readDb.ProductsReadModel
            .FirstOrDefaultAsync(p => p.Id == createResult.Id);
        Assert.NotNull(readModel);
        Assert.Equal("Updated Name", readModel.Name);
        Assert.Equal(75.00m, readModel.Price);
        Assert.Equal(30, readModel.StockQuantity);

        // Act - Query read model
        var query = new GetProductByIdQuery
        {
            TenantId = _tenantId,
            ProductId = createResult.Id
        };

        var queryHandler = new GetProductByIdQueryHandler(_readDb, Mock.Of<ILogger<GetProductByIdQueryHandler>>());
        var queryResult = await queryHandler.Handle(query, CancellationToken.None);

        // Assert - Query returns updated data
        Assert.NotNull(queryResult);
        Assert.Equal("Updated Name", queryResult.Name);
        Assert.Equal(75.00m, queryResult.Price);
    }

    [Fact]
    public async Task DeleteProduct_SoftDeleteInReadModel_QueryFiltersDeletedProducts()
    {
        // Arrange - Create product
        var productId = Guid.NewGuid();
        var createCommand = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "DELETE-TEST-001",
            Name = "Product to Delete",
            Price = 100.00m,
            IsAvailable = true
        };

        var createHandler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
        var createResult = await createHandler.Handle(createCommand, CancellationToken.None);

        // Add to read model
        var createEvent = new ProductCreatedEvent
        {
            ProductId = createResult.Id,
            TenantId = _tenantId,
            Sku = createCommand.Sku,
            Name = createCommand.Name,
            Price = createCommand.Price,
            IsAvailable = createCommand.IsAvailable,
            CreatedAt = DateTime.UtcNow
        };

        var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
        await eventHandler.Handle(createEvent, CancellationToken.None);

        // Act - Delete product
        var deleteCommand = new DeleteProductCommand
        {
            TenantId = _tenantId,
            ProductId = createResult.Id
        };

        var deleteHandler = new DeleteProductCommandHandler(_writeDb, Mock.Of<ILogger<DeleteProductCommandHandler>>());
        var deleteResult = await deleteHandler.Handle(deleteCommand, CancellationToken.None);

        // Assert - Soft deleted in write model
        Assert.True(deleteResult.Success);

        // Act - Update read model with delete event
        var deleteEvent = new ProductDeletedEvent
        {
            ProductId = createResult.Id,
            TenantId = _tenantId,
            Sku = createCommand.Sku
        };

        var deleteEventHandler = new ProductDeletedEventHandler(_readDb, Mock.Of<ILogger<ProductDeletedEventHandler>>());
        await deleteEventHandler.Handle(deleteEvent, CancellationToken.None);

        // Assert - Read model marked as deleted
        var readModel = await _readDb.ProductsReadModel
            .FirstOrDefaultAsync(p => p.Id == createResult.Id);
        Assert.NotNull(readModel);
        Assert.True(readModel.IsDeleted);

        // Act - Query should not return deleted products
        var query = new GetProductByIdQuery
        {
            TenantId = _tenantId,
            ProductId = createResult.Id
        };

        var queryHandler = new GetProductByIdQueryHandler(_readDb, Mock.Of<ILogger<GetProductByIdQueryHandler>>());

        // Assert - Query throws KeyNotFoundException
        await Assert.ThrowsAsync<KeyNotFoundException>(() => queryHandler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetProductsPagedQuery_FiltersAndPaginates_ReturnsCorrectResults()
    {
        // Arrange - Create multiple products
        for (int i = 0; i < 5; i++)
        {
            var cmd = new CreateProductCommand
            {
                TenantId = _tenantId,
                Sku = $"PAGINATION-{i:D3}",
                Name = $"Product {i}",
                Price = (i + 1) * 10m,
                Category = i % 2 == 0 ? "Electronics" : "Books",
                IsAvailable = i < 3,
                StockQuantity = (i + 1) * 10
            };

            var handler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Add to read model
            var evt = new ProductCreatedEvent
            {
                ProductId = result.Id,
                TenantId = _tenantId,
                Sku = cmd.Sku,
                Name = cmd.Name,
                Price = cmd.Price,
                Category = cmd.Category,
                IsAvailable = cmd.IsAvailable,
                StockQuantity = cmd.StockQuantity,
                CreatedAt = DateTime.UtcNow
            };

            var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
            await eventHandler.Handle(evt, CancellationToken.None);
        }

        // Act - Query with category filter
        var query = new GetProductsPagedQuery
        {
            TenantId = _tenantId,
            Category = "Electronics",
            Page = 1,
            PageSize = 10
        };

        var handler2 = new GetProductsPagedQueryHandler(_readDb, Mock.Of<ILogger<GetProductsPagedQueryHandler>>());
        var result2 = await handler2.Handle(query, CancellationToken.None);

        // Assert - Only Electronics products returned
        Assert.Equal(3, result2.Items.Count());
        Assert.All(result2.Items, item => Assert.Equal("Electronics", item.Category));
        Assert.Equal(3, result2.TotalCount);
    }

    [Fact]
    public async Task SearchProducts_FindsMatchingTerms_ReturnsPaginatedResults()
    {
        // Arrange - Create products with searchable content
        var products = new[]
        {
            new CreateProductCommand
            {
                TenantId = _tenantId,
                Sku = "SEARCH-001",
                Name = "Red Laptop",
                Description = "High-performance computing device",
                Price = 999.99m,
                IsAvailable = true
            },
            new CreateProductCommand
            {
                TenantId = _tenantId,
                Sku = "SEARCH-002",
                Name = "Blue Monitor",
                Description = "4K laptop display",
                Price = 299.99m,
                IsAvailable = true
            },
            new CreateProductCommand
            {
                TenantId = _tenantId,
                Sku = "SEARCH-003",
                Name = "Keyboard",
                Description = "Mechanical gaming keyboard",
                Price = 149.99m,
                IsAvailable = true
            }
        };

        foreach (var cmd in products)
        {
            var handler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            var evt = new ProductCreatedEvent
            {
                ProductId = result.Id,
                TenantId = _tenantId,
                Sku = cmd.Sku,
                Name = cmd.Name,
                Description = cmd.Description,
                Price = cmd.Price,
                IsAvailable = cmd.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
            await eventHandler.Handle(evt, CancellationToken.None);
        }

        // Act - Search for "laptop"
        var query = new SearchProductsQuery
        {
            TenantId = _tenantId,
            SearchTerm = "laptop",
            Page = 1,
            PageSize = 10
        };

        var queryHandler = new SearchProductsQueryHandler(_readDb, Mock.Of<ILogger<SearchProductsQueryHandler>>());
        var result = await queryHandler.Handle(query, CancellationToken.None);

        // Assert - Found products with "laptop" in name or description
        Assert.Equal(2, result.Items.Count()); // "Red Laptop" and "4K laptop display"
        Assert.All(result.Items, item =>
            Assert.Contains("laptop", item.Name.ToLower() + " " + (item.Description ?? "").ToLower())
        );
    }

    [Fact]
    public async Task GetCatalogStats_AggregatesCorrectly_ReturnsStats()
    {
        // Arrange - Create products with different prices
        var prices = new[] { 10m, 20m, 30m, 40m, 50m };
        foreach (var price in prices)
        {
            var cmd = new CreateProductCommand
            {
                TenantId = _tenantId,
                Sku = $"STATS-{price:F0}",
                Name = $"Product at ${price}",
                Price = price,
                IsAvailable = price > 15m,
                Category = "TestCategory"
            };

            var handler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            var evt = new ProductCreatedEvent
            {
                ProductId = result.Id,
                TenantId = _tenantId,
                Sku = cmd.Sku,
                Name = cmd.Name,
                Price = cmd.Price,
                IsAvailable = cmd.IsAvailable,
                Category = cmd.Category,
                CreatedAt = DateTime.UtcNow
            };

            var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
            await eventHandler.Handle(evt, CancellationToken.None);
        }

        // Act
        var query = new GetCatalogStatsQuery { TenantId = _tenantId };
        var handler2 = new GetCatalogStatsQueryHandler(_readDb, Mock.Of<ILogger<GetCatalogStatsQueryHandler>>());
        var result2 = await handler2.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result2.TotalProducts);
        Assert.Equal(4, result2.ActiveProducts); // Price > 15m
        Assert.Equal(30m, result2.AveragePrice); // (10+20+30+40+50)/5
        Assert.Equal(10m, result2.MinPrice);
        Assert.Equal(50m, result2.MaxPrice);
        Assert.Equal(1, result2.TotalCategories); // Only "TestCategory"
    }

    [Fact]
    public async Task MultiTenant_IsolatesData_DoesNotMixTenants()
    {
        // Arrange
        var tenant1Id = Guid.NewGuid();
        var tenant2Id = Guid.NewGuid();

        var cmd1 = new CreateProductCommand
        {
            TenantId = tenant1Id,
            Sku = "TENANT1-001",
            Name = "Tenant 1 Product",
            Price = 100m,
            IsAvailable = true
        };

        var cmd2 = new CreateProductCommand
        {
            TenantId = tenant2Id,
            Sku = "TENANT2-001",
            Name = "Tenant 2 Product",
            Price = 200m,
            IsAvailable = true
        };

        // Act - Create products for different tenants
        var handler = new CreateProductCommandHandler(_writeDb, Mock.Of<ILogger<CreateProductCommandHandler>>());
        var result1 = await handler.Handle(cmd1, CancellationToken.None);
        var result2 = await handler.Handle(cmd2, CancellationToken.None);

        // Add to read model
        foreach (var (res, cmd) in new[] { (result1, cmd1), (result2, cmd2) })
        {
            var evt = new ProductCreatedEvent
            {
                ProductId = res.Id,
                TenantId = cmd.TenantId,
                Sku = cmd.Sku,
                Name = cmd.Name,
                Price = cmd.Price,
                IsAvailable = cmd.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            var eventHandler = new ProductCreatedEventHandler(_readDb, _logger);
            await eventHandler.Handle(evt, CancellationToken.None);
        }

        // Act - Query tenant 1 products
        var query1 = new GetProductsPagedQuery
        {
            TenantId = tenant1Id,
            Page = 1,
            PageSize = 10
        };

        var queryHandler = new GetProductsPagedQueryHandler(_readDb, Mock.Of<ILogger<GetProductsPagedQueryHandler>>());
        var queryResult1 = await queryHandler.Handle(query1, CancellationToken.None);

        // Assert - Only tenant 1 products
        Assert.Single(queryResult1.Items);
        Assert.Equal("Tenant 1 Product", queryResult1.Items.First().Name);

        // Act - Query tenant 2 products
        var query2 = new GetProductsPagedQuery
        {
            TenantId = tenant2Id,
            Page = 1,
            PageSize = 10
        };

        var queryResult2 = await queryHandler.Handle(query2, CancellationToken.None);

        // Assert - Only tenant 2 products
        Assert.Single(queryResult2.Items);
        Assert.Equal("Tenant 2 Product", queryResult2.Items.First().Name);
    }
}
