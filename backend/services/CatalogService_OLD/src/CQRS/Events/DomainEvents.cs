namespace B2Connect.CatalogService.CQRS.Events;

/// <summary>
/// Domain event published when a product is created
/// Multiple handlers can subscribe to this event
/// </summary>
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event published when a product is updated
/// </summary>
public class ProductUpdatedEvent
{
    public Guid ProductId { get; set; }
    public Guid TenantId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event published when a product is deleted
/// </summary>
public class ProductDeletedEvent
{
    public Guid ProductId { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Domain event published when products are bulk imported
/// </summary>
public class ProductsBulkImportedEvent
{
    public Guid TenantId { get; set; }
    public string ImportId { get; set; }
    public int ProductCount { get; set; }
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
}
