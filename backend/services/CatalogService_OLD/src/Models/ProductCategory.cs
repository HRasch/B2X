using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2Connect.CatalogService.Models;

/// <summary>
/// Junction entity for Product-Category relationship (many-to-many)
/// </summary>
public class ProductCategory
{
    /// <summary>Gets or sets the product ID</summary>
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    /// <summary>Navigation property to the product</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Gets or sets the category ID</summary>
    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }

    /// <summary>Navigation property to the category</summary>
    public Category Category { get; set; } = null!;

    /// <summary>Gets or sets whether this is the primary category for the product</summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>Gets or sets the display order in this category</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
