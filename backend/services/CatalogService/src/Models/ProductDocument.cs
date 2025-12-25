using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Shared.Types;

namespace B2Connect.CatalogService.Models;

/// <summary>
/// Product document (specs, manuals, certifications, etc.) with multilingual support
/// </summary>
public class ProductDocument
{
    /// <summary>Gets or sets the unique identifier</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the product ID this document belongs to</summary>
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    /// <summary>Navigation property to the product</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Gets or sets the multilingual document name/title</summary>
    [Required]
    public LocalizedContent Name { get; set; } = new();

    /// <summary>Gets or sets the multilingual document description</summary>
    public LocalizedContent? Description { get; set; } = new();

    /// <summary>Gets or sets the document type (specification, manual, certification, datasheet, etc.)</summary>
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = "specification";

    /// <summary>Gets or sets the document URL</summary>
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the document file name</summary>
    [MaxLength(255)]
    public string? FileName { get; set; }

    /// <summary>Gets or sets the document file mime type (e.g., application/pdf)</summary>
    [MaxLength(50)]
    public string? MimeType { get; set; }

    /// <summary>Gets or sets the file size in bytes</summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>Gets or sets the language code this document is in (e.g., en, de, fr)</summary>
    [MaxLength(10)]
    public string? Language { get; set; }

    /// <summary>Gets or sets the version of the document</summary>
    [MaxLength(50)]
    public string? Version { get; set; }

    /// <summary>Gets or sets whether this document is publicly visible</summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>Gets or sets the display order among product documents</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Gets or sets whether this document is active/visible</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the external document provider (e.g., s3, dropbox)</summary>
    [MaxLength(100)]
    public string? Provider { get; set; }

    /// <summary>Gets or sets the external provider document ID/reference</summary>
    [MaxLength(255)]
    public string? ExternalId { get; set; }

    /// <summary>Gets or sets the release/publication date</summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>Gets or sets the expiration date (if applicable)</summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>Gets or sets the tenant ID for multi-tenancy</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets the user ID who created this document</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Gets or sets the user ID who last updated this document</summary>
    public string? UpdatedBy { get; set; }
}
