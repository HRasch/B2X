using System.ComponentModel.DataAnnotations;
using B2X.Types.Localization;

namespace B2X.Store.Core.Common.Entities;

/// <summary>
/// Country entity for geographic management
/// Includes region information and shipping configuration
/// </summary>
public class Country
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>ISO 3166-1 alpha-2 country code (e.g., DE, FR, GB, US)</summary>
    [Required]
    [MaxLength(2)]
    public string Code { get; set; } = string.Empty;

    /// <summary>ISO 3166-1 alpha-3 country code</summary>
    [MaxLength(3)]
    public string? CodeAlpha3 { get; set; }

    /// <summary>Numeric country code</summary>
    [MaxLength(3)]
    public string? NumericCode { get; set; }

    /// <summary>Country name in English</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Localized country names</summary>
    public LocalizedContent? LocalizedName { get; set; }

    /// <summary>Region/Continent (e.g., Europe, Asia, Americas)</summary>
    [MaxLength(50)]
    public string? Region { get; set; }

    /// <summary>Sub-region (e.g., Northern Europe, Western Europe)</summary>
    [MaxLength(50)]
    public string? SubRegion { get; set; }

    /// <summary>Capital city</summary>
    [MaxLength(100)]
    public string? Capital { get; set; }

    /// <summary>Currency code (e.g., EUR, GBP)</summary>
    [MaxLength(3)]
    public string? CurrencyCode { get; set; }

    /// <summary>Telephone country code (e.g., +49 for Germany)</summary>
    [MaxLength(10)]
    public string? PhoneCode { get; set; }

    /// <summary>Default language for country</summary>
    [MaxLength(10)]
    public string? LanguageCode { get; set; }

    /// <summary>Enable shipping to this country</summary>
    public bool IsShippingEnabled { get; set; } = true;

    /// <summary>Require customs declaration for imports</summary>
    public bool RequiresCustomsDeclaration { get; set; } = false;

    public bool IsActive { get; set; } = true;

    /// <summary>Display order for country selection</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Shops in this country</summary>
    public ICollection<Shop> Shops { get; set; } = new List<Shop>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

