using Xunit;
using FluentValidation;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS.Validators;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2Connect.CatalogService.Tests.CQRS.Validation;

/// <summary>
/// CQRS Command Validation Tests
/// Tests FluentValidation rules for all commands
/// Includes async uniqueness checks and business rule validation
/// </summary>
public class CQRSCommandValidationTests : IAsyncLifetime
{
    private CatalogDbContext _dbContext = null!;
    private readonly Guid _tenantId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new CatalogDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task CreateProductValidator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "VALID-SKU-001",
            Name = "Valid Product Name",
            Price = 99.99m,
            Description = "A valid product description",
            IsAvailable = true,
            StockQuantity = 100
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new CreateProductCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreateProductValidator_WithDuplicateSku_FailsValidation()
    {
        // Arrange - Create existing product with same SKU
        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Sku = "DUPLICATE-SKU",
            Name = "Existing Product",
            Price = 50m,
            IsActive = true
        };
        _dbContext.Products.Add(existingProduct);
        await _dbContext.SaveChangesAsync();

        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "DUPLICATE-SKU", // Same SKU
            Name = "New Product",
            Price = 99.99m
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new CreateProductCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("already exists"));
    }

    [Fact]
    public async Task CreateProductValidator_WithInvalidPrice_FailsValidation()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "INVALID-PRICE-SKU",
            Name = "Product with invalid price",
            Price = -50m // Negative price
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new CreateProductCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task CreateProductValidator_WithEmptyName_FailsValidation()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "EMPTY-NAME-SKU",
            Name = "", // Empty name
            Price = 99.99m
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new CreateProductCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateProductValidator_WithMissingTenantId_FailsValidation()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = Guid.Empty, // Missing tenant
            Sku = "NO-TENANT-SKU",
            Name = "Product",
            Price = 99.99m
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new CreateProductCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TenantId");
    }

    [Fact]
    public async Task UpdateProductValidator_WithValidPartialUpdate_PassesValidation()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            TenantId = _tenantId,
            ProductId = Guid.NewGuid(),
            Name = "Updated Name" // Only updating name
        };

        var validator = new UpdateProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateProductValidator_WithInvalidPrice_FailsValidation()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            TenantId = _tenantId,
            ProductId = Guid.NewGuid(),
            Price = -100m // Invalid price
        };

        var validator = new UpdateProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task UpdateProductValidator_WithEmptyProductId_FailsValidation()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            TenantId = _tenantId,
            ProductId = Guid.Empty, // Empty product ID
            Name = "Updated Name"
        };

        var validator = new UpdateProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task DeleteProductValidator_WithValidCommand_PassesValidation()
    {
        // Arrange
        var command = new DeleteProductCommand
        {
            TenantId = _tenantId,
            ProductId = Guid.NewGuid()
        };

        var validator = new DeleteProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteProductValidator_WithEmptyProductId_FailsValidation()
    {
        // Arrange
        var command = new DeleteProductCommand
        {
            TenantId = _tenantId,
            ProductId = Guid.Empty // Empty ID
        };

        var validator = new DeleteProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task DeleteProductValidator_WithEmptyTenantId_FailsValidation()
    {
        // Arrange
        var command = new DeleteProductCommand
        {
            TenantId = Guid.Empty, // Empty tenant
            ProductId = Guid.NewGuid()
        };

        var validator = new DeleteProductCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TenantId");
    }

    [Fact]
    public async Task BulkImportValidator_WithValidCommands_PassesValidation()
    {
        // Arrange
        var command = new BulkImportProductsCommand
        {
            TenantId = _tenantId,
            Products = new[]
            {
                new ProductImportData { Sku = "BULK-001", Name = "Product 1", Price = 99.99m },
                new ProductImportData { Sku = "BULK-002", Name = "Product 2", Price = 149.99m }
            }
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new BulkImportProductsCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task BulkImportValidator_WithTooManyProducts_FailsValidation()
    {
        // Arrange
        var products = Enumerable.Range(1, 10001)
            .Select(i => new ProductImportData
            {
                Sku = $"BULK-{i:D5}",
                Name = $"Product {i}",
                Price = 99.99m
            })
            .ToArray();

        var command = new BulkImportProductsCommand
        {
            TenantId = _tenantId,
            Products = products
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new BulkImportProductsCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Products");
    }

    [Fact]
    public async Task BulkImportValidator_WithDuplicateSkus_FailsValidation()
    {
        // Arrange
        var command = new BulkImportProductsCommand
        {
            TenantId = _tenantId,
            Products = new[]
            {
                new ProductImportData { Sku = "DUPLICATE", Name = "Product 1", Price = 99.99m },
                new ProductImportData { Sku = "DUPLICATE", Name = "Product 2", Price = 149.99m } // Same SKU
            }
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new BulkImportProductsCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task BulkImportValidator_WithInvalidProductData_FailsValidation()
    {
        // Arrange
        var command = new BulkImportProductsCommand
        {
            TenantId = _tenantId,
            Products = new[]
            {
                new ProductImportData { Sku = "", Name = "", Price = -50m } // All invalid
            }
        };

        var repository = new ProductRepository(_dbContext);
        var validator = new BulkImportProductsCommandValidator(repository);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
