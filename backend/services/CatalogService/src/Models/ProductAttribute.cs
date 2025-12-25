using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Shared.Types;

namespace B2Connect.CatalogService.Models;

/// <summary>
/// Product attribute definition with multilingual support
/// </summary>
public class ProductAttribute
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the attribute code (unique identifier for attribute)</summary>
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Gets or sets the multilingual attribute name/label</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Gets or sets the multilingual attribute description</summary>
    public LocalizedContent? Description { get; set; } = new();

    /// <summary>Gets or sets the attribute type (text, select, multiselect, dropdown, date, etc.)</summary>
    [Required]
    [MaxLength(50)]
    public string AttributeType { get; set; } = "text";

    /// <summary>Gets or sets the input type for attribute value validation</summary>
    [MaxLength(50)]
    public string? InputType { get; set; }

    /// <summary>Gets or sets the JSON serialized default value</summary>
    public string? DefaultValue { get; set; }

    /// <summary>Gets or sets whether this attribute is required</summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>Gets or sets whether this attribute is searchable</summary>
    public bool IsSearchable { get; set; } = true;

    /// <summary>Gets or sets whether this attribute is filterable</summary>
    public bool IsFilterable { get; set; } = true;

    /// <summary>Gets or sets whether this attribute is visible in product list</summary>
    public bool IsVisibleInProductList { get; set; } = false;

    /// <summary>Gets or sets whether this attribute is visible in product detail</summary>
    public bool IsVisibleInProductDetail { get; set; } = true;

    /// <summary>Gets or sets the display order</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Gets or sets whether this attribute is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property to attribute options</summary>
    public ICollection<ProductAttributeOption> Options { get; set; } = new List<ProductAttributeOption>();

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this attribute</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this attribute</summary>
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Product attribute option with multilingual support
/// </summary>
public class ProductAttributeOption
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the attribute ID this option belongs to</summary>
    [ForeignKey(nameof(Attribute))]
    public Guid AttributeId { get; set; }

    /// <summary>Navigation property to the parent attribute</summary>
    public ProductAttribute Attribute { get; set; } = null!;

    /// <summary>Gets or sets the option code</summary>
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Gets or sets the multilingual option label</summary>
    [Required]
    public LocalizedContent Label { get; set; } = new();

    /// <summary>Gets or sets the color value for color attributes</summary>
    [MaxLength(7)]
    public string? ColorValue { get; set; }

    /// <summary>Gets or sets the display order</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Gets or sets whether this option is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
