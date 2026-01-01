using System.ComponentModel.DataAnnotations;
using B2Connect.Shared.Core;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product category with multilingual support (Hybrid Localization Pattern).
/// Default values stored in indexed columns, translations in JSON.
/// See ADR: ADR-entity-localization-pattern.md
/// </summary>
public class Category
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the category slug (URL-friendly identifier)</summary>
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;

    // === Localized Properties (Hybrid Pattern) ===

    /// <summary>Category name in default language (indexed for search)</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Category name translations</summary>
    public LocalizedContent? NameTranslations { get; set; }

    /// <summary>Category description in default language</summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>Category description translations</summary>
    public LocalizedContent? DescriptionTranslations { get; set; }

    /// <summary>SEO meta description in default language</summary>
    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    /// <summary>SEO meta description translations</summary>
    public LocalizedContent? MetaDescriptionTranslations { get; set; }

    // === Localization Helpers ===
    public string GetLocalizedName(string lang) => NameTranslations?.GetValue(lang) ?? Name;
    public string GetLocalizedDescription(string lang) => DescriptionTranslations?.GetValue(lang) ?? Description ?? string.Empty;
    public string GetLocalizedMetaDescription(string lang) => MetaDescriptionTranslations?.GetValue(lang) ?? MetaDescription ?? string.Empty;

    /// <summary>Gets or sets the parent category ID for hierarchical structure</summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>Navigation property to parent category</summary>
    public Category? ParentCategory { get; set; }

    /// <summary>Navigation property to child categories</summary>
    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    /// <summary>Gets or sets the display order</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Gets or sets whether the category is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the category image URL</summary>
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>Gets or sets the alternate text for category image</summary>
    [MaxLength(255)]
    public string? ImageAltText { get; set; }

    /// <summary>Navigation property to products in this category</summary>
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this category</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this category</summary>
    public string? UpdatedBy { get; set; }
}
