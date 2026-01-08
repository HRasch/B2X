using System.Text.Json;
using System.Text.Json.Serialization;

namespace B2X.Types.Localization;

/// <summary>
/// Represents translated content stored as JSON in entity properties.
/// Allows entities to have multi-language properties without separate tables.
/// </summary>
/// <example>
/// Usage in an Entity:
/// <code>
/// public class Product : Entity
/// {
///     public LocalizedContent Name { get; set; } = new();
///     public LocalizedContent Description { get; set; } = new();
/// }
///
/// // Set translations
/// product.Name.Set("en", "Product Name")
///          .Set("de", "Produktname")
///          .Set("fr", "Nom du produit");
///
/// // Get translations
/// string enName = product.Name.Get("en");
/// string currentLangName = product.Name.GetForLocale("de");
/// </code>
/// </example>
public class LocalizedContent
{
    /// <summary>
    /// Stores translations with language code as key (e.g., "en", "de", "fr")
    /// </summary>
    [JsonPropertyName("translations")]
    public Dictionary<string, string> Translations { get; set; } = new();

    /// <summary>
    /// Default/fallback language code (usually "en")
    /// </summary>
    [JsonPropertyName("defaultLanguage")]
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Gets the translation for a specific language code
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "de")</param>
    /// <returns>Translated value or empty string if not found</returns>
    public string Get(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return GetDefault();
        }

        return Translations.TryGetValue(languageCode.ToLowerInvariant(), out var value)
            ? value
            : GetDefault();
    }

    /// <summary>
    /// Gets the default/fallback translation
    /// </summary>
    /// <returns>Default translation or first available translation</returns>
    public string GetDefault()
    {
        if (Translations.TryGetValue(DefaultLanguage.ToLowerInvariant(), out var value))
        {
            return value;
        }

        return Translations.FirstOrDefault().Value ?? string.Empty;
    }

    /// <summary>
    /// Sets a translation for a specific language
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "de")</param>
    /// <param name="value">The translated value</param>
    /// <returns>This instance for method chaining</returns>
    public LocalizedContent Set(string languageCode, string value)
    {
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            Translations[languageCode.ToLowerInvariant()] = value ?? string.Empty;
        }
        return this;
    }

    /// <summary>
    /// Sets multiple translations at once
    /// </summary>
    /// <param name="translations">Dictionary of language code -> translation</param>
    /// <returns>This instance for method chaining</returns>
    public LocalizedContent SetMany(Dictionary<string, string> translations)
    {
        if (translations != null)
        {
            foreach (var kvp in translations)
            {
                Set(kvp.Key, kvp.Value);
            }
        }
        return this;
    }

    /// <summary>
    /// Checks if a translation exists for a language
    /// </summary>
    public bool HasTranslation(string languageCode) =>
        !string.IsNullOrWhiteSpace(languageCode) &&
        Translations.ContainsKey(languageCode.ToLowerInvariant());

    /// <summary>
    /// Gets all available language codes in this content
    /// </summary>
    public IEnumerable<string> GetAvailableLanguages() =>
        Translations.Keys;

    /// <summary>
    /// Gets the count of available translations
    /// </summary>
    public int Count => Translations.Count;

    /// <summary>
    /// Checks if this content has any translations
    /// </summary>
    public bool IsEmpty => Translations.Count == 0;

    /// <summary>
    /// Removes a translation for a specific language
    /// </summary>
    public bool Remove(string languageCode) =>
        !string.IsNullOrWhiteSpace(languageCode) &&
        Translations.Remove(languageCode.ToLowerInvariant());

    /// <summary>
    /// Clears all translations
    /// </summary>
    public void Clear() => Translations.Clear();

    /// <summary>
    /// Creates a LocalizedContent from a JSON string
    /// </summary>
    public static LocalizedContent FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new LocalizedContent();
        }

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<LocalizedContent>(json, options) ?? new LocalizedContent();
        }
        catch
        {
            return new LocalizedContent();
        }
    }

    /// <summary>
    /// Creates a LocalizedContent from a dictionary
    /// </summary>
    public static LocalizedContent FromDictionary(Dictionary<string, string>? dict, string defaultLanguage = "en")
    {
        var content = new LocalizedContent { DefaultLanguage = defaultLanguage };
        if (dict != null)
        {
            content.SetMany(dict);
        }
        return content;
    }

    /// <summary>
    /// Converts this instance to a JSON string
    /// </summary>
    public string ToJson()
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return JsonSerializer.Serialize(this, options);
    }

    /// <summary>
    /// Converts this instance to a dictionary
    /// </summary>
    public Dictionary<string, string> ToDictionary() => new(Translations);

    /// <summary>
    /// Creates a copy of this LocalizedContent
    /// </summary>
    public LocalizedContent Clone() => new()
    {
        DefaultLanguage = DefaultLanguage,
        Translations = new Dictionary<string, string>(Translations)
    };

    /// <summary>
    /// Merges another LocalizedContent into this one
    /// Languages from 'other' override existing ones
    /// </summary>
    public LocalizedContent Merge(LocalizedContent other)
    {
        if (other?.Translations != null)
        {
            foreach (var kvp in other.Translations)
            {
                Translations[kvp.Key] = kvp.Value;
            }
        }
        return this;
    }

    /// <summary>
    /// Gets all translations as a formatted string (useful for logging/debugging)
    /// </summary>
    public override string ToString()
    {
        if (IsEmpty)
        {
            return "[empty]";
        }

        var items = Translations.Select(kvp => $"{kvp.Key}='{kvp.Value}'");
        return $"[{string.Join(", ", items)}]";
    }

    /// <summary>
    /// Validates that all required languages have translations
    /// </summary>
    public bool HasAllLanguages(params string[] requiredLanguages)
    {
        var required = requiredLanguages.Select(l => l.ToLowerInvariant()).ToHashSet();
        return required.All(lang => Translations.ContainsKey(lang));
    }

    /// <summary>
    /// Gets translations for multiple languages at once
    /// </summary>
    public Dictionary<string, string> GetMany(params string[] languageCodes) =>
        languageCodes.ToDictionary(
            lang => lang,
            lang => Get(lang)
        );
}
