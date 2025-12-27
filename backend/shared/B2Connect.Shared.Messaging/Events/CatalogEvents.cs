namespace B2Connect.Shared.Messaging.Events;

/// <summary>
/// Event raised when a product is created
/// </summary>
public record ProductCreatedEvent(
    Guid TenantId,
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal Price,
    DateTimeOffset CreatedAt);

/// <summary>
/// Event raised when a product is updated
/// </summary>
public record ProductUpdatedEvent(
    Guid TenantId,
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal Price,
    DateTimeOffset UpdatedAt);

/// <summary>
/// Event raised when a product is deleted
/// </summary>
public record ProductDeletedEvent(
    Guid TenantId,
    Guid ProductId,
    DateTimeOffset DeletedAt);

/// <summary>
/// Event raised when product stock is updated
/// </summary>
public record ProductStockUpdatedEvent(
    Guid TenantId,
    Guid ProductId,
    int StockQuantity,
    DateTimeOffset UpdatedAt);

/// <summary>
/// Event raised when a product price is changed
/// </summary>
public record ProductPriceChangedEvent(
    Guid TenantId,
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice,
    DateTimeOffset ChangedAt);
