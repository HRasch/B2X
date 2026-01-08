namespace B2X.LocalizationService.Models;

/// <summary>
/// Represents a localized string that can be translated into multiple languages
/// </summary>
public class LocalizedString
{
    /// <summary>Gets or sets the unique identifier</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the translation key (e.g., "login", "required")</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the category (e.g., "auth", "errors", "ui")</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the translations dictionary (language code -> translation)</summary>
    public Dictionary<string, string> Translations { get; set; } = new(StringComparer.Ordinal);

    /// <summary>Gets or sets the default English value (fallback)</summary>
    public string DefaultValue { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional tenant ID for tenant-specific overrides</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets whether this translation is active</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the user who created this translation</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Gets or sets the creation timestamp</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the last modification timestamp</summary>
    public DateTime UpdatedAt { get; set; }
}
