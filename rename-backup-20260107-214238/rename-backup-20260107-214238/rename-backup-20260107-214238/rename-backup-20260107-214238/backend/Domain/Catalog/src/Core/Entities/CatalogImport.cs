namespace B2X.Catalog.Core.Entities;

/// <summary>
/// Represents a catalog import operation
/// Used for tracking and identifying imported catalogs for removal
/// </summary>
public class CatalogImport
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string CatalogId { get; set; } = string.Empty;
    public DateTime ImportTimestamp { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public ImportStatus Status { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<CatalogProduct> Products { get; set; } = new List<CatalogProduct>();
}

/// <summary>
/// Represents a product from an imported catalog
/// Stores the raw product data as JSON for flexibility
/// </summary>
public class CatalogProduct
{
    public Guid Id { get; set; }
    public Guid CatalogImportId { get; set; }
    public string SupplierAid { get; set; } = string.Empty; // Article ID from supplier
    public string ProductData { get; set; } = string.Empty; // JSON representation of product
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public CatalogImport CatalogImport { get; set; } = null!;
}

/// <summary>
/// Status of a catalog import operation
/// </summary>
public enum ImportStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}
