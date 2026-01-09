using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Types.Localization;

namespace B2X.Admin.Core.Entities;

/// <summary>
/// Product variant (e.g., different sizes, colors) with multilingual support
/// </summary>
public class ProductVariant
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the product ID this variant belongs to</summary>
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    /// <summary>Navigation property to the parent product</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Gets or sets the variant SKU (e.g., main-sku-size-color)</summary>
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;

    /// <summary>Gets or sets the multilingual variant name/label</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Gets or sets the multilingual variant description</summary>
    public LocalizedContent? Description { get; set; } = new();

    /// <summary>Gets or sets the variant price (if different from base product price)</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    /// <summary>Gets or sets the variant special/promotional price</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SpecialPrice { get; set; }

    /// <summary>Gets or sets the variant cost price</summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CostPrice { get; set; }

    /// <summary>Gets or sets the variant weight</summary>
    public decimal? Weight { get; set; }

    /// <summary>Gets or sets the variant stock quantity</summary>
    public int StockQuantity { get; set; }

    /// <summary>Gets or sets whether this variant is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the display order among variants</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Gets or sets whether this is the default variant</summary>
    public bool IsDefault { get; set; }

    /// <summary>Gets or sets the image URL for this variant (if different from main product image)</summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>Navigation property to variant attribute values</summary>
    public ICollection<VariantAttributeValue> AttributeValues { get; set; } = new List<VariantAttributeValue>();

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this variant</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this variant</summary>
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Product variant attribute value (e.g., size=Large, color=Red)
/// </summary>
public class VariantAttributeValue
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the variant ID</summary>
    [ForeignKey(nameof(Variant))]
    public Guid VariantId { get; set; }

    /// <summary>Navigation property to the variant</summary>
    public ProductVariant Variant { get; set; } = null!;

    /// <summary>Gets or sets the attribute ID</summary>
    [ForeignKey(nameof(Attribute))]
    public Guid AttributeId { get; set; }

    /// <summary>Navigation property to the attribute</summary>
    public ProductAttribute Attribute { get; set; } = null!;

    /// <summary>Gets or sets the attribute option ID (if it's a select/option attribute)</summary>
    [ForeignKey(nameof(Option))]
    public Guid? OptionId { get; set; }

    /// <summary>Navigation property to the attribute option</summary>
    public ProductAttributeOption? Option { get; set; }

    /// <summary>Gets or sets the attribute value (for text, number, date attributes)</summary>
    [MaxLength(1000)]
    public string? Value { get; set; }

    /// <summary>Gets or sets whether this attribute value is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
