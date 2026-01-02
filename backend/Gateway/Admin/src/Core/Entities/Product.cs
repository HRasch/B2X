using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Types.Localization;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product entity with multilingual support and complex relationships
/// </summary>
public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public LocalizedContent Name { get; set; } = new();

    public LocalizedContent? ShortDescription { get; set; } = new();
    public LocalizedContent? Description { get; set; } = new();
    public LocalizedContent? MetaDescription { get; set; } = new();
    public LocalizedContent? MetaKeywords { get; set; } = new();

    [ForeignKey(nameof(Brand))]
    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

    [ForeignKey(nameof(Category))]
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SpecialPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CostPrice { get; set; }

    public decimal? Weight { get; set; }
    [MaxLength(10)]
    public string? WeightUnit { get; set; }

    public int StockQuantity { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool IsStockTracked { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; } = true;

    public decimal? AverageRating { get; set; }
    public int ReviewCount { get; set; }

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductDocument> Documents { get; set; } = new List<ProductDocument>();
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();

    public Guid? TenantId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
