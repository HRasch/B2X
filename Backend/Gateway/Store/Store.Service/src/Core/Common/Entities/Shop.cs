using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Types.Localization;

namespace B2Connect.Store.Core.Common.Entities;

/// <summary>
/// Shop entity representing a retail location or online storefront
/// Supports multilingual shop information and configuration
/// </summary>
public class Shop
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public LocalizedContent Name { get; set; } = new();

    public LocalizedContent? Description { get; set; } = new();

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [ForeignKey(nameof(Country))]
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    /// <summary>Default currency code (e.g., USD, EUR, GBP)</summary>
    [MaxLength(3)]
    public string? CurrencyCode { get; set; } = "EUR";

    /// <summary>Default language for shop</summary>
    [ForeignKey(nameof(DefaultLanguage))]
    public Guid? DefaultLanguageId { get; set; }
    public Language? DefaultLanguage { get; set; }

    /// <summary>Shop timezone (e.g., Europe/Berlin)</summary>
    [MaxLength(50)]
    public string? TimeZone { get; set; } = "Europe/Berlin";

    public bool IsActive { get; set; } = true;
    public bool IsMainShop { get; set; } = false;

    /// <summary>Supported languages for this shop</summary>
    public ICollection<Language> SupportedLanguages { get; set; } = new List<Language>();

    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

