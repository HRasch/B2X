using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Shared.Core;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product entity with multilingual support (Hybrid Localization Pattern).
/// Default values stored in indexed columns, translations in JSON.
/// See ADR: ADR-entity-localization-pattern.md
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

    // === Localized Properties (Hybrid Pattern) ===
    // Default value in column (indexed), translations in JSON

    /// <summary>Product name in default language (indexed for search)</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Product name translations</summary>
    public LocalizedContent? NameTranslations { get; set; }

    /// <summary>Short description in default language</summary>
    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    /// <summary>Short description translations</summary>
    public LocalizedContent? ShortDescriptionTranslations { get; set; }

    /// <summary>Full description in default language</summary>
    [MaxLength(4000)]
    public string? Description { get; set; }

    /// <summary>Full description translations</summary>
    public LocalizedContent? DescriptionTranslations { get; set; }

    /// <summary>SEO meta description in default language</summary>
    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    /// <summary>SEO meta description translations</summary>
    public LocalizedContent? MetaDescriptionTranslations { get; set; }

    /// <summary>SEO meta keywords in default language</summary>
    [MaxLength(500)]
    public string? MetaKeywords { get; set; }

    /// <summary>SEO meta keywords translations</summary>
    public LocalizedContent? MetaKeywordsTranslations { get; set; }

    // === Localization Helpers ===
    public string GetLocalizedName(string lang) => NameTranslations?.GetValue(lang) ?? Name;
    public string GetLocalizedShortDescription(string lang) => ShortDescriptionTranslations?.GetValue(lang) ?? ShortDescription ?? string.Empty;
    public string GetLocalizedDescription(string lang) => DescriptionTranslations?.GetValue(lang) ?? Description ?? string.Empty;
    public string GetLocalizedMetaDescription(string lang) => MetaDescriptionTranslations?.GetValue(lang) ?? MetaDescription ?? string.Empty;
    public string GetLocalizedMetaKeywords(string lang) => MetaKeywordsTranslations?.GetValue(lang) ?? MetaKeywords ?? string.Empty;

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

    public int StockQuantity { get; set; } = 0;
    public int? LowStockThreshold { get; set; }
    public bool IsStockTracked { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool IsNew { get; set; } = true;

    public decimal? AverageRating { get; set; }
    public int ReviewCount { get; set; } = 0;

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductDocument> Documents { get; set; } = new List<ProductDocument>();
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();

    public Guid? TenantId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
