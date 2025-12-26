using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product attribute value (junction entity for Product-Attribute relationship)
/// </summary>
public class ProductAttributeValue
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the product ID</summary>
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    /// <summary>Navigation property to the product</summary>
    public Product Product { get; set; } = null!;

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

    /// <summary>Gets or sets the position/order of this attribute value</summary>
    public int Position { get; set; } = 0;

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
