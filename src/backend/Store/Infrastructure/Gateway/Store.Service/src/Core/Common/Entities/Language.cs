using System.ComponentModel.DataAnnotations;

namespace B2X.Store.Core.Common.Entities;

/// <summary>
/// Language entity for multilingual support
/// Defines available languages and their properties
/// </summary>
public class Language
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>ISO 639-1 language code (e.g., de, en, fr, es)</summary>
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Display name (e.g., Deutsch, English, Français)</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Native language name (e.g., Deutsch, English, Français)</summary>
    [Required]
    [MaxLength(100)]
    public string NativeName { get; set; } = string.Empty;

    /// <summary>Language direction (LTR or RTL)</summary>
    [MaxLength(3)]
    public string Direction { get; set; } = "LTR"; // LTR = Left-to-Right, RTL = Right-to-Left

    /// <summary>Date format pattern (e.g., dd.MM.yyyy)</summary>
    [MaxLength(50)]
    public string? DateFormat { get; set; }

    /// <summary>Time format pattern (e.g., HH:mm:ss)</summary>
    [MaxLength(50)]
    public string? TimeFormat { get; set; }

    /// <summary>Number format culture (e.g., de-DE, en-US)</summary>
    [MaxLength(10)]
    public string? CultureCode { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;

    /// <summary>Display order for language selection</summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>Shops supporting this language</summary>
    public ICollection<Shop> Shops { get; set; } = new List<Shop>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

