using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Shared.Core;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product attribute definition with multilingual support (Hybrid Localization Pattern)
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

    /// <summary>Gets or sets the attribute name (default language, indexed for admin search)</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the translations for the name (JSON storage)</summary>
    public LocalizedContent? NameTranslations { get; set; }

    /// <summary>Gets or sets the attribute description (default language)</summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the translations for the description (JSON storage)</summary>
    public LocalizedContent? DescriptionTranslations { get; set; }

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

    /// <summary>Gets the localized name for a specific language</summary>
    public string GetLocalizedName(string languageCode) =>
        NameTranslations?.GetValue(languageCode) ?? Name;

    /// <summary>Gets the localized description for a specific language</summary>
    public string? GetLocalizedDescription(string languageCode) =>
        DescriptionTranslations?.GetValue(languageCode) ?? Description;
}

/// <summary>
/// Product attribute option with multilingual support (Hybrid Localization Pattern)
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

    /// <summary>Gets or sets the option label (default language, indexed for admin search)</summary>
    [Required]
    [MaxLength(255)]
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the translations for the label (JSON storage)</summary>
    public LocalizedContent? LabelTranslations { get; set; }

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

    /// <summary>Gets the localized label for a specific language</summary>
    public string GetLocalizedLabel(string languageCode) =>
        LabelTranslations?.GetValue(languageCode) ?? Label;
}
