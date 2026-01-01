namespace B2Connect.Shared.Core;

/// <summary>
/// Value Object for embedding localized content directly in domain entities.
/// Use this pattern when an entity property needs multi-language support.
/// 
/// Example usage in a domain entity:
/// <code>
/// public class Product : ITenantEntity
/// {
///     public Guid Id { get; set; }
///     public Guid TenantId { get; set; }
///     public LocalizedContent Name { get; private set; }
///     public LocalizedContent Description { get; private set; }
///     public decimal Price { get; set; }
/// }
/// </code>
/// 
/// EF Core configuration (in DbContext.OnModelCreating):
/// <code>
/// modelBuilder.Entity&lt;Product&gt;().OwnsOne(p => p.Name, name =>
/// {
///     name.Property(n => n.DefaultValue).HasColumnName("Name_Default");
///     name.Property(n => n.Translations).HasColumnName("Name_Translations").HasConversion(...);
/// });
/// </code>
/// </summary>
public sealed class LocalizedContent : IValueObject, IEquatable<LocalizedContent>
{
    /// <summary>Gets the default value (typically English)</summary>
    public string DefaultValue { get; }

    /// <summary>Gets the translations dictionary (language code -> translation)</summary>
    public IReadOnlyDictionary<string, string> Translations { get; }

    /// <summary>
    /// Creates a new LocalizedContent with a default value and optional translations.
    /// </summary>
    public LocalizedContent(string defaultValue, Dictionary<string, string>? translations = null)
    {
        DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        Translations = translations ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Creates a LocalizedContent with only a default value (no translations).
    /// Useful for initial entity creation.
    /// </summary>
    public static LocalizedContent Create(string defaultValue)
        => new(defaultValue);

    /// <summary>
    /// Creates a LocalizedContent with default value and translations.
    /// </summary>
    public static LocalizedContent Create(string defaultValue, Dictionary<string, string> translations)
        => new(defaultValue, translations);

    /// <summary>
    /// Gets the localized value for the specified language.
    /// Falls back to default value if translation not found.
    /// </summary>
    public string GetValue(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return DefaultValue;

        return Translations.TryGetValue(languageCode.ToLowerInvariant(), out var translation)
            ? translation
            : DefaultValue;
    }

    /// <summary>
    /// Returns a new LocalizedContent with an added or updated translation.
    /// Value objects are immutable - this creates a new instance.
    /// </summary>
    public LocalizedContent WithTranslation(string languageCode, string value)
    {
        var newTranslations = new Dictionary<string, string>(Translations)
        {
            [languageCode.ToLowerInvariant()] = value
        };
        return new LocalizedContent(DefaultValue, newTranslations);
    }

    /// <summary>
    /// Returns a new LocalizedContent with updated default value.
    /// </summary>
    public LocalizedContent WithDefaultValue(string newDefaultValue)
    {
        return new LocalizedContent(newDefaultValue, new Dictionary<string, string>(Translations));
    }

    /// <summary>
    /// Returns a new LocalizedContent with replaced translations.
    /// </summary>
    public LocalizedContent WithTranslations(Dictionary<string, string> newTranslations)
    {
        return new LocalizedContent(DefaultValue, newTranslations);
    }

    /// <summary>
    /// Checks if a translation exists for the specified language.
    /// </summary>
    public bool HasTranslation(string languageCode)
        => Translations.ContainsKey(languageCode.ToLowerInvariant());

    /// <summary>
    /// Gets all available language codes.
    /// </summary>
    public IEnumerable<string> GetAvailableLanguages()
        => Translations.Keys;

    // Value Object equality - based on DefaultValue and all Translations
    public bool Equals(LocalizedContent? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (DefaultValue != other.DefaultValue) return false;
        if (Translations.Count != other.Translations.Count) return false;
        return Translations.All(kvp =>
            other.Translations.TryGetValue(kvp.Key, out var value) && value == kvp.Value);
    }

    public override bool Equals(object? obj) => Equals(obj as LocalizedContent);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(DefaultValue);
        foreach (var kvp in Translations.OrderBy(k => k.Key))
        {
            hash.Add(kvp.Key);
            hash.Add(kvp.Value);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(LocalizedContent? left, LocalizedContent? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(LocalizedContent? left, LocalizedContent? right)
        => !(left == right);

    public override string ToString()
        => $"{DefaultValue} ({Translations.Count} translations)";

    // Implicit conversion from string for convenience
    public static implicit operator LocalizedContent(string defaultValue)
        => new(defaultValue);

    /// <summary>
    /// Gets a concatenated string of all values (default + translations) for full-text search.
    /// This can be stored in a separate indexed column for efficient searching across all languages.
    /// </summary>
    public string GetSearchableText()
    {
        var allValues = new List<string> { DefaultValue };
        allValues.AddRange(Translations.Values);
        return string.Join(" ", allValues.Distinct());
    }

    /// <summary>
    /// Checks if any translation (including default) contains the search term.
    /// For in-memory filtering only - not translatable to SQL.
    /// </summary>
    public bool ContainsText(string searchTerm, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return true;

        if (DefaultValue.Contains(searchTerm, comparison))
            return true;

        return Translations.Values.Any(t => t.Contains(searchTerm, comparison));
    }

    // EF Core needs a parameterless constructor for materialization
    private LocalizedContent()
    {
        DefaultValue = string.Empty;
        Translations = new Dictionary<string, string>();
    }
}
