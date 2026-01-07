using System.ComponentModel.DataAnnotations;
using B2X.Types.Localization;

namespace B2X.Admin.Core.Entities;

/// <summary>
/// Product category with multilingual support
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

    /// <summary>Gets or sets the multilingual category name</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Gets or sets the multilingual category description</summary>
    public LocalizedContent? Description { get; set; } = new();

    /// <summary>Gets or sets the SEO multilingual meta description</summary>
    public LocalizedContent? MetaDescription { get; set; } = new();

    /// <summary>Gets or sets the parent category ID for hierarchical structure</summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>Navigation property to parent category</summary>
    public Category? ParentCategory { get; set; }

    /// <summary>Navigation property to child categories</summary>
    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    /// <summary>Gets or sets the display order</summary>
    public int DisplayOrder { get; set; }

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
