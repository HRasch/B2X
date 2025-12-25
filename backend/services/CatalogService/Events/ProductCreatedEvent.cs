using System;
using B2Connect.Shared.Events;

namespace B2Connect.CatalogService.Events
{
    /// <summary>
    /// Event triggered when a new product is created
    /// Published to: product.created RabbitMQ routing key
    /// </summary>
    public record ProductCreatedEvent(
        Guid ProductId,
        string Sku,
        string Name,
        string Description,
        string Category,
        decimal Price,
        decimal? B2bPrice,
        int StockQuantity,
        string[] Tags,
        ProductAttributesDto Attributes,
        string[] ImageUrls,
        Guid TenantId) : DomainEvent
    {
        public ProductCreatedEvent()
            : this(
                Guid.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                0m,
                null,
                0,
                Array.Empty<string>(),
                new ProductAttributesDto(),
                Array.Empty<string>(),
                Guid.Empty)
        {
            AggregateId = ProductId;
            AggregateType = "Product";
            EventType = "product.created";
            Timestamp = DateTime.UtcNow;
            Version = 1;
        }
    }

    /// <summary>
    /// Event triggered when an existing product is updated
    /// Published to: product.updated RabbitMQ routing key
    /// </summary>
    public record ProductUpdatedEvent(
        Guid ProductId,
        Dictionary<string, object> Changes,
        Guid TenantId) : DomainEvent
    {
        public ProductUpdatedEvent(Guid productId, Dictionary<string, object> changes, Guid tenantId)
            : this(productId, changes ?? new Dictionary<string, object>(), tenantId)
        {
            AggregateId = productId;
            AggregateType = "Product";
            EventType = "product.updated";
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Event triggered when a product is deleted
    /// Published to: product.deleted RabbitMQ routing key
    /// </summary>
    public record ProductDeletedEvent(Guid ProductId, Guid TenantId) : DomainEvent
    {
        public ProductDeletedEvent(Guid productId, Guid tenantId)
            : this(productId, tenantId)
        {
            AggregateId = productId;
            AggregateType = "Product";
            EventType = "product.deleted";
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Event triggered when bulk products are imported
    /// Published to: products.bulk-imported RabbitMQ routing key
    /// </summary>
    public record ProductsBulkImportedEvent(
        Guid[] ProductIds,
        int TotalCount,
        Guid TenantId) : DomainEvent
    {
        public ProductsBulkImportedEvent(Guid[] productIds, int totalCount, Guid tenantId)
            : this(productIds, totalCount, tenantId)
        {
            AggregateType = "Product";
            EventType = "products.bulk-imported";
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// DTO for product attributes (immutable record)
    /// </summary>
    public record ProductAttributesDto(
        string? Brand = null,
        string[]? Colors = null,
        string? Material = null,
        string[]? Sizes = null,
        Dictionary<string, string>? Custom = null);
}
