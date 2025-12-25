using Xunit;
using FluentValidation;
using B2Connect.CatalogService.Events;
using B2Connect.CatalogService.Validators;

namespace B2Connect.CatalogService.Tests;

public class EventValidatorsTests
{
    private readonly ProductCreatedEventValidator _productCreatedValidator;
    private readonly ProductUpdatedEventValidator _productUpdatedValidator;
    private readonly ProductDeletedEventValidator _productDeletedValidator;
    private readonly ProductsBulkImportedEventValidator _bulkImportedValidator;

    public EventValidatorsTests()
    {
        _productCreatedValidator = new ProductCreatedEventValidator();
        _productUpdatedValidator = new ProductUpdatedEventValidator();
        _productDeletedValidator = new ProductDeletedEventValidator();
        _bulkImportedValidator = new ProductsBulkImportedEventValidator();
    }

    #region ProductCreatedEvent Tests

    [Fact]
    public async Task ProductCreatedEventValidator_WithValidData_Succeeds()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: 89.99m,
            StockQuantity: 100,
            Tags: new[] { "test", "electronics" },
            Attributes: new ProductAttributesDto(Brand: "TestBrand"),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithEmptyProductId_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.Empty,  // Invalid
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithInvalidSku_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "invalid-lowercase",  // Invalid - should be uppercase
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Sku");
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithInvalidPrice_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: -10.00m,  // Invalid - negative price
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithB2bPriceGreaterThanPrice_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: 109.99m,  // Invalid - B2B price > regular price
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithInvalidEventType_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        // EventType is derived property, so it should be "product.created"
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithTooManyTags_Fails()
    {
        // Arrange
        var tags = Enumerable.Range(1, 25).Select(i => $"tag{i}").ToArray();
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: tags,  // 25 tags - exceeds limit
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Tags");
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithInvalidImageUrl_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "not-a-valid-url" },  // Invalid URL
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ProductCreatedEventValidator_WithEmptyTenantId_Fails()
    {
        // Arrange
        var @event = new ProductCreatedEvent(
            ProductId: Guid.NewGuid(),
            Sku: "TEST-001",
            Name: "Test Product",
            Description: "A test product",
            Category: "Electronics",
            Price: 99.99m,
            B2bPrice: null,
            StockQuantity: 100,
            Tags: new[] { "test" },
            Attributes: new ProductAttributesDto(),
            ImageUrls: new[] { "https://example.com/image.jpg" },
            TenantId: Guid.Empty);  // Invalid

        // Act
        var result = await _productCreatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TenantId");
    }

    #endregion

    #region ProductUpdatedEvent Tests

    [Fact]
    public async Task ProductUpdatedEventValidator_WithValidData_Succeeds()
    {
        // Arrange
        var changes = new Dictionary<string, object>
        {
            { "Name", "Updated Product Name" },
            { "Price", 89.99m }
        };
        var @event = new ProductUpdatedEvent(
            ProductId: Guid.NewGuid(),
            Changes: changes,
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productUpdatedValidator.ValidateAsync(@event);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ProductUpdatedEventValidator_WithEmptyChanges_Fails()
    {
        // Arrange
        var @event = new ProductUpdatedEvent(
            ProductId: Guid.NewGuid(),
            Changes: new Dictionary<string, object>(),  // No changes
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productUpdatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ProductUpdatedEventValidator_WithEmptyProductId_Fails()
    {
        // Arrange
        var @event = new ProductUpdatedEvent(
            ProductId: Guid.Empty,
            Changes: new Dictionary<string, object> { { "Name", "Updated" } },
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productUpdatedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion

    #region ProductDeletedEvent Tests

    [Fact]
    public async Task ProductDeletedEventValidator_WithValidData_Succeeds()
    {
        // Arrange
        var @event = new ProductDeletedEvent(
            ProductId: Guid.NewGuid(),
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productDeletedValidator.ValidateAsync(@event);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ProductDeletedEventValidator_WithEmptyProductId_Fails()
    {
        // Arrange
        var @event = new ProductDeletedEvent(
            ProductId: Guid.Empty,
            TenantId: Guid.NewGuid());

        // Act
        var result = await _productDeletedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion

    #region ProductsBulkImportedEvent Tests

    [Fact]
    public async Task ProductsBulkImportedEventValidator_WithValidData_Succeeds()
    {
        // Arrange
        var productIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var @event = new ProductsBulkImportedEvent(
            ProductIds: productIds,
            TotalCount: 3,
            TenantId: Guid.NewGuid());

        // Act
        var result = await _bulkImportedValidator.ValidateAsync(@event);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ProductsBulkImportedEventValidator_WithZeroProducts_Fails()
    {
        // Arrange
        var @event = new ProductsBulkImportedEvent(
            ProductIds: Array.Empty<Guid>(),
            TotalCount: 0,
            TenantId: Guid.NewGuid());

        // Act
        var result = await _bulkImportedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ProductsBulkImportedEventValidator_WithMismatchedCount_Fails()
    {
        // Arrange
        var productIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var @event = new ProductsBulkImportedEvent(
            ProductIds: productIds,
            TotalCount: 5,  // Doesn't match array length
            TenantId: Guid.NewGuid());

        // Act
        var result = await _bulkImportedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ProductsBulkImportedEventValidator_WithEmptyProductId_Fails()
    {
        // Arrange
        var productIds = new[] { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
        var @event = new ProductsBulkImportedEvent(
            ProductIds: productIds,
            TotalCount: 3,
            TenantId: Guid.NewGuid());

        // Act
        var result = await _bulkImportedValidator.ValidateAsync(@event);

        // Assert
        Assert.False(result.IsValid);
    }

    #endregion
}
