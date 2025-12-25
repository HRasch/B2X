using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Shared.Types;

namespace B2Connect.CatalogService.Models;

/// <summary>
/// Product entity with multilingual support and complex relationships
/// </summary>
public class Product
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the product SKU (unique product code)</summary>
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>Gets or sets the product slug (URL-friendly identifier)</summary>
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>Gets or sets the multilingual product name</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Gets or sets the multilingual short description</summary>
    public LocalizedContent? ShortDescription { get; set; } = new();

    /// <summary>Gets or sets the multilingual full description</summary>
    public LocalizedContent? Description { get; set; } = new();

    /// <summary>Gets or sets the SEO multilingual meta description</summary>
    public LocalizedContent? MetaDescription { get; set; } = new();

    /// <summary>Gets or sets the SEO multilingual keywords</summary>
    public LocalizedContent? MetaKeywords { get; set; } = new();

    /// <summary>Gets or sets the brand ID</summary>
    [ForeignKey(nameof(Brand))]
    public Guid? BrandId { get; set; }

    /// <summary>Navigation property to the brand</summary>
    public Brand? Brand { get; set; }

    /// <summary>Gets or sets the base price</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    /// <summary>Gets or sets the special/promotional price</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SpecialPrice { get; set; }

    /// <summary>Gets or sets the cost price (for margin calculations)</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CostPrice { get; set; }

    /// <summary>Gets or sets the weight in the specified unit</summary>
    public decimal? Weight { get; set; }

    /// <summary>Gets or sets the weight unit (kg, g, lb, oz)</summary>
    [MaxLength(10)]
    public string? WeightUnit { get; set; }

    /// <summary>Gets or sets the stock/inventory quantity</summary>
    public int StockQuantity { get; set; } = 0;

    /// <summary>Gets or sets the low stock threshold for alerts</summary>
    public int? LowStockThreshold { get; set; }

    /// <summary>Gets or sets whether the product is trackable by stock</summary>
    public bool IsStockTracked { get; set; } = true;

    /// <summary>Gets or sets whether the product is available for purchase</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>Gets or sets whether the product is active/published</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets whether this is a featured product</summary>
    public bool IsFeatured { get; set; } = false;

    /// <summary>Gets or sets whether this is a new product</summary>
    public bool IsNew { get; set; } = true;

    /// <summary>Gets or sets the product rating (0-5)</summary>
    public decimal? AverageRating { get; set; }

    /// <summary>Gets or sets the number of reviews</summary>
    public int ReviewCount { get; set; } = 0;

    /// <summary>Gets or sets the URL for the product thumbnail image</summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>Navigation property to product categories</summary>
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    /// <summary>Navigation property to product variants</summary>
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    /// <summary>Navigation property to product images</summary>
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    /// <summary>Navigation property to product documents</summary>
    public ICollection<ProductDocument> Documents { get; set; } = new List<ProductDocument>();

    /// <summary>Navigation property to product attributes</summary>
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this product</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this product</summary>
    public string? UpdatedBy { get; set; }
}
