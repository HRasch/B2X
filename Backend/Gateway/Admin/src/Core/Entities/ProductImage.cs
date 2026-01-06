using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2Connect.Admin.Core.Entities;

/// <summary>
/// Product image with support for multiple images per product
/// </summary>
public class ProductImage
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the product ID this image belongs to</summary>
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    /// <summary>Navigation property to the product</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Gets or sets the image URL</summary>
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the alternate text for the image (for accessibility)</summary>
    [MaxLength(255)]
    public string? AltText { get; set; }

    /// <summary>Gets or sets the image title</summary>
    [MaxLength(255)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the thumbnail URL (optional)</summary>
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>Gets or sets the medium size image URL (optional)</summary>
    [MaxLength(500)]
    public string? MediumUrl { get; set; }

    /// <summary>Gets or sets the large size image URL (optional)</summary>
    [MaxLength(500)]
    public string? LargeUrl { get; set; }

    /// <summary>Gets or sets the image mime type (e.g., image/jpeg, image/png)</summary>
    [MaxLength(50)]
    public string? MimeType { get; set; }

    /// <summary>Gets or sets the file size in bytes</summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>Gets or sets the image width in pixels</summary>
    public int? Width { get; set; }

    /// <summary>Gets or sets the image height in pixels</summary>
    public int? Height { get; set; }

    /// <summary>Gets or sets whether this is the main/primary image for the product</summary>
    public bool IsPrimary { get; set; }

    /// <summary>Gets or sets the display order among product images</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Gets or sets whether this image is active/visible</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the external image provider (e.g., cloudinary, s3)</summary>
    [MaxLength(100)]
    public string? Provider { get; set; }

    /// <summary>Gets or sets the external provider image ID/reference</summary>
    [MaxLength(255)]
    public string? ExternalId { get; set; }

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this image</summary>
    public string? CreatedBy { get; set; }
}
