namespace B2X.Catalog.Models;

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
    public List<Guid> CategoryIds { get; set; } = new();
    public string? BrandName { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Barcode { get; set; } // Added for barcode scanning support
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Create product request
/// </summary>
public class CreateProductRequest
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public required int StockQuantity { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public string? BrandName { get; set; }
    public List<string>? Tags { get; set; }
    public string? Barcode { get; set; } // Added for barcode scanning support
}

/// <summary>
/// Update product request
/// </summary>
public class UpdateProductRequest
{
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public string? BrandName { get; set; }
    public List<string>? Tags { get; set; }
    public string? Barcode { get; set; } // Added for barcode scanning support
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
    public required decimal ProductPrice { get; set; }

    /// <summary>
    /// VAT rate as percentage (e.g., 19 for 19%)
    /// </summary>
    public required decimal VatRate { get; set; }

    /// <summary>
    /// Calculated VAT amount
    /// </summary>
    public required decimal VatAmount { get; set; }

    /// <summary>
    /// Final price including VAT - THIS IS WHAT CUSTOMERS SEE
    /// </summary>
    public required decimal PriceIncludingVat { get; set; }

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
