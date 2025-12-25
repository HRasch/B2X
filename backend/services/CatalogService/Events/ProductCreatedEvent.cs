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
        public override Guid AggregateId => ProductId;
        public override string AggregateType => "Product";
        public override string EventType => "product.created";
        public override int Version => 1;
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
        public override Guid AggregateId => ProductId;
        public override string AggregateType => "Product";
        public override string EventType => "product.updated";
    }

    /// <summary>
    /// Event triggered when a product is deleted
    /// Published to: product.deleted RabbitMQ routing key
    /// </summary>
    public record ProductDeletedEvent(Guid ProductId, Guid TenantId) : DomainEvent
    {
        public override Guid AggregateId => ProductId;
        public override string AggregateType => "Product";
        public override string EventType => "product.deleted";
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
        public override Guid AggregateId => ProductIds.Length > 0 ? ProductIds[0] : Guid.Empty;
        public override string AggregateType => "Product";
        public override string EventType => "products.bulk-imported";
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
