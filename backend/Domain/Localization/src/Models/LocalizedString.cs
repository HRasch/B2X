namespace B2Connect.LocalizationService.Models;

/// <summary>
/// Represents a localized string that can be translated into multiple languages.
/// This is a value object - it has no identity and is defined by its values.
/// </summary>
public class LocalizedString : B2Connect.Shared.Core.IValueObject, IEquatable<LocalizedString>
{
    /// <summary>Gets the translation key (e.g., "login", "required")</summary>
    public string Key { get; private set; }

    /// <summary>Gets the category (e.g., "auth", "errors", "ui")</summary>
    public string Category { get; private set; }

    /// <summary>Gets the translations dictionary (language code -> translation)</summary>
    public IReadOnlyDictionary<string, string> Translations { get; private set; }

    /// <summary>Gets the default English value (fallback)</summary>
    public string DefaultValue { get; private set; }

    /// <summary>
    /// Private parameterless constructor for EF Core materialization.
    /// </summary>
    private LocalizedString()
    {
        Key = string.Empty;
        Category = string.Empty;
        DefaultValue = string.Empty;
        Translations = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the LocalizedString class.
    /// </summary>
    public LocalizedString(string key, string category, string defaultValue, Dictionary<string, string>? translations = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        Translations = translations ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Gets the translation for the specified language, falling back to the default value.
    /// </summary>
    public string GetTranslation(string languageCode)
    {
        return Translations.TryGetValue(languageCode, out var translation)
            ? translation
            : DefaultValue;
    }

    /// <summary>
    /// Creates a new LocalizedString with updated translations.
    /// </summary>
    public LocalizedString WithTranslations(Dictionary<string, string> newTranslations)
    {
        return new LocalizedString(Key, Category, DefaultValue, new Dictionary<string, string>(newTranslations));
    }

    /// <summary>
    /// Creates a new LocalizedString with an additional translation.
    /// </summary>
    public LocalizedString WithTranslation(string languageCode, string translation)
    {
        var newTranslations = new Dictionary<string, string>(Translations);
        newTranslations[languageCode] = translation;
        return new LocalizedString(Key, Category, DefaultValue, newTranslations);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current LocalizedString.
    /// Two LocalizedString instances are equal if they have the same Key and Category.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as LocalizedString);
    }

    /// <summary>
    /// Determines whether the specified LocalizedString is equal to the current LocalizedString.
    /// </summary>
    public bool Equals(LocalizedString? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key == other.Key && Category == other.Category;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Category);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
    {
        return $"{Category}:{Key} ({Translations.Count} translations)";
    }

    /// <summary>
    /// Determines whether two LocalizedString instances are equal.
    /// </summary>
    public static bool operator ==(LocalizedString? left, LocalizedString? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two LocalizedString instances are not equal.
    /// </summary>
    public static bool operator !=(LocalizedString? left, LocalizedString? right)
    {
        return !(left == right);
    }
}
