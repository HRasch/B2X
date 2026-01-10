namespace B2X.Variants.Models;

/// <summary>
/// Product variant aggregate root (SKU)
/// Represents different variations of a product (size, color, style, etc.)
/// </summary>
public class Variant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }

    // Variant identification
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    // Variant attributes (flexible key-value pairs)
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.Ordinal);

    // Pricing (can override product price)
    public decimal? Price { get; set; }
    public decimal? CompareAtPrice { get; set; } // Original price for showing discounts

    // Inventory
    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorders { get; set; } = false;

    // Variant media
    public List<string> ImageUrls { get; set; } = new();
    public string? PrimaryImageUrl { get; set; }

    // Status and availability
    public bool IsActive { get; set; } = true;
    public bool IsAvailable => (!TrackInventory || StockQuantity > 0 || AllowBackorders) && IsActive;

    // Metadata
    public int DisplayOrder { get; set; } = 0;
    public string? Barcode { get; set; }
    public string? Weight { get; set; }
    public string? Dimensions { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// DTO for variant responses
/// </summary>
public class VariantDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.Ordinal);
    public decimal? Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; }
    public bool AllowBackorders { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public string? PrimaryImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public string? Barcode { get; set; }
    public string? Weight { get; set; }
    public string? Dimensions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating variants
/// </summary>
public class CreateVariantDto
{
    public Guid ProductId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.Ordinal);
    public decimal? Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorders { get; set; } = false;
    public List<string> ImageUrls { get; set; } = new();
    public string? PrimaryImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public string? Barcode { get; set; }
    public string? Weight { get; set; }
    public string? Dimensions { get; set; }
}

/// <summary>
/// DTO for updating variants
/// </summary>
public class UpdateVariantDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.Ordinal);
    public decimal? Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool AllowBackorders { get; set; } = false;
    public List<string> ImageUrls { get; set; } = new();
    public string? PrimaryImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public string? Barcode { get; set; }
    public string? Weight { get; set; }
    public string? Dimensions { get; set; }
}

/// <summary>
/// Paged result wrapper for variant queries
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
