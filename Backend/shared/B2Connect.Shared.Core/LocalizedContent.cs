using B2Connect.Types.Domain;

namespace B2Connect.Shared.Core;

/// <summary>
/// Value Object for storing translations of a property.
/// Used with the Hybrid Localization Pattern where the default value is stored
/// in a separate indexed column and translations are stored as JSON.
///
/// Pattern: Default column + Translations JSON
/// - Default value: Stored in a dedicated column (indexed for search)
/// - Translations: Stored in this Value Object as JSON
///
/// Example usage in a domain entity:
/// <code>
/// public class Product : ITenantEntity
/// {
///     public Guid Id { get; set; }
///     public Guid TenantId { get; set; }
///
///     // Default value in indexed column
///     public string Name { get; set; }
///     // Translations as JSON
///     public LocalizedContent? NameTranslations { get; set; }
///
///     // Helper to get localized value with fallback
///     public string GetLocalizedName(string languageCode)
///         => NameTranslations?.GetValue(languageCode) ?? Name;
/// }
/// </code>
///
/// EF Core configuration:
/// <code>
/// modelBuilder.Entity&lt;Product&gt;(entity =>
/// {
///     entity.HasIndex(p => p.Name); // Indexed for search
///     entity.OwnsOne(p => p.NameTranslations, t => t.ToJson("NameTranslations"));
/// });
/// </code>
///
/// See ADR: ADR-entity-localization-pattern.md for full documentation.
/// </summary>
public sealed class LocalizedContent : IValueObject, IEquatable<LocalizedContent>
{
    /// <summary>Gets the translations dictionary (language code -> translation)</summary>
    public IReadOnlyDictionary<string, string> Translations { get; }

    /// <summary>
    /// Creates a new LocalizedContent with the given translations.
    /// </summary>
    public LocalizedContent(Dictionary<string, string>? translations = null)
    {
        Translations = translations ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Creates an empty LocalizedContent (no translations yet).
    /// </summary>
    public static LocalizedContent Empty => new();

    /// <summary>
    /// Creates a LocalizedContent from a dictionary of translations.
    /// </summary>
    public static LocalizedContent Create(Dictionary<string, string> translations)
        => new(translations);

    /// <summary>
    /// Creates a LocalizedContent with a single translation.
    /// </summary>
    public static LocalizedContent Create(string languageCode, string value)
        => new(new Dictionary<string, string> { [languageCode.ToLowerInvariant()] = value });

    /// <summary>
    /// Gets the translation for the specified language.
    /// Returns null if no translation exists for that language.
    /// Use in combination with the default column: entity.NameTranslations?.GetValue("de") ?? entity.Name
    /// </summary>
    public string? GetValue(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return null;
        }

        return Translations.TryGetValue(languageCode.ToLowerInvariant(), out var translation)
            ? translation
            : null;
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
        return new LocalizedContent(newTranslations);
    }

    /// <summary>
    /// Returns a new LocalizedContent with the specified translation removed.
    /// </summary>
    public LocalizedContent WithoutTranslation(string languageCode)
    {
        var newTranslations = new Dictionary<string, string>(Translations);
        newTranslations.Remove(languageCode.ToLowerInvariant());
        return new LocalizedContent(newTranslations);
    }

    /// <summary>
    /// Returns a new LocalizedContent with replaced translations.
    /// </summary>
    public static LocalizedContent WithTranslations(Dictionary<string, string> newTranslations)
    {
        return new LocalizedContent(newTranslations);
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

    /// <summary>
    /// Gets the number of translations.
    /// </summary>
    public int Count => Translations.Count;

    /// <summary>
    /// Returns true if there are no translations.
    /// </summary>
    public bool IsEmpty => Translations.Count == 0;

    // Value Object equality - based on all Translations
    public bool Equals(LocalizedContent? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (Translations.Count != other.Translations.Count)
        {
            return false;
        }

        return Translations.All(kvp =>
            other.Translations.TryGetValue(kvp.Key, out var value) && value == kvp.Value);
    }

    public override bool Equals(object? obj) => Equals(obj as LocalizedContent);

    public override int GetHashCode()
    {
        var hash = default(HashCode);
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
        => Translations.Count == 0
            ? "(no translations)"
            : $"({Translations.Count} translations: {string.Join(", ", Translations.Keys)})";

    /// <summary>
    /// Gets a concatenated string of all translation values for full-text search.
    /// Note: Does NOT include the default value (that's in a separate column).
    /// </summary>
    public string GetSearchableText()
    {
        return string.Join(" ", Translations.Values.Distinct());
    }

    /// <summary>
    /// Checks if any translation contains the search term.
    /// For in-memory filtering only - not translatable to SQL.
    /// Note: Does NOT search the default value (that's in a separate column).
    /// </summary>
    public bool ContainsText(string searchTerm, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return true;
        }

        return Translations.Values.Any(t => t.Contains(searchTerm, comparison));
    }

    // EF Core needs a parameterless constructor for materialization
    private LocalizedContent()
    {
        Translations = new Dictionary<string, string>();
    }
}
