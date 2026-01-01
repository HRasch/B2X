using System.ComponentModel.DataAnnotations;
using B2Connect.Shared.Core;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product brand with multilingual support (Hybrid Localization Pattern).
/// Default values stored in indexed columns, translations in JSON.
/// See ADR: ADR-entity-localization-pattern.md
/// </summary>
public class Brand
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the brand slug (URL-friendly identifier)</summary>
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    // === Localized Properties (Hybrid Pattern) ===

    /// <summary>Brand name in default language (indexed for search)</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Brand name translations</summary>
    public LocalizedContent? NameTranslations { get; set; }

    /// <summary>Brand description in default language</summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>Brand description translations</summary>
    public LocalizedContent? DescriptionTranslations { get; set; }

    // === Localization Helpers ===
    public string GetLocalizedName(string lang) => NameTranslations?.GetValue(lang) ?? Name;
    public string GetLocalizedDescription(string lang) => DescriptionTranslations?.GetValue(lang) ?? Description ?? string.Empty;

    /// <summary>Gets or sets the brand logo URL</summary>
    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    /// <summary>Gets or sets the brand website URL</summary>
    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    /// <summary>Gets or sets whether the brand is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the display order</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Navigation property to products of this brand</summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this brand</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this brand</summary>
    public string? UpdatedBy { get; set; }
}
