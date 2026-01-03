namespace B2Connect.Catalog.Models;

/// <summary>
/// Product DTO for API responses
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public List<string> Categories { get; set; } = new();
    public string? BrandName { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Paged result for list responses
/// </summary>
public class PagedResult<T>
{
    public required List<T> Items { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalCount { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Create product request
/// </summary>
public class CreateProductRequest
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public List<string>? Categories { get; set; }
    public string? BrandName { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Update product request
/// </summary>
public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsActive { get; set; }
    public List<string>? Categories { get; set; }
    public string? BrandName { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Price breakdown with VAT details
/// Issue #30: B2C Price Transparency (PAngV Compliance)
/// Shows customer the final price including VAT
/// </summary>
public class PriceBreakdownDto
{
    /// <summary>
    /// Product price without VAT
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// VAT rate as percentage (e.g., 19 for 19%)
    /// </summary>
    public decimal VatRate { get; set; }

    /// <summary>
    /// Calculated VAT amount
    /// </summary>
    public decimal VatAmount { get; set; }

    /// <summary>
    /// Final price including VAT - THIS IS WHAT CUSTOMERS SEE
    /// </summary>
    public decimal PriceIncludingVat { get; set; }

    /// <summary>
    /// Discount amount if applicable
    /// </summary>
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Final discounted price (if discount applied)
    /// </summary>
    public decimal? FinalPrice { get; set; }

    /// <summary>
    /// Original price (for comparison if discounted)
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// ISO 4217 currency code
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Destination country for VAT calculation
    /// </summary>
    public string DestinationCountry { get; set; } = string.Empty;
}
