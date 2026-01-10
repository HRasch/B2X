using System.Text.Json;
using System.Text.Json.Serialization;
using B2X.Types.Localization;

namespace B2X.Types.Utilities;

/// <summary>
/// Utility class for working with JSON serialization of LocalizedContent
/// Handles storing and retrieving translations from database
/// </summary>
public static class LocalizationJsonUtility
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Serializes a LocalizedContent object to JSON string
    /// </summary>
    public static string Serialize(LocalizedContent content, bool pretty = false)
    {
        if (content == null)
        {
            return "{}";
        }

        var options = pretty ? PrettyOptions : DefaultOptions;
        return JsonSerializer.Serialize(content, options);
    }

    /// <summary>
    /// Deserializes a JSON string to LocalizedContent object
    /// </summary>
    public static LocalizedContent Deserialize(string? json)
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
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize localized content: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Tries to deserialize a JSON string safely without throwing exceptions
    /// </summary>
    public static bool TryDeserialize(string? json, out LocalizedContent result)
    {
        result = new LocalizedContent();

        if (string.IsNullOrWhiteSpace(json))
        {
            return true;
        }

        try
        {
            result = Deserialize(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Serializes a dictionary to LocalizedContent JSON string
    /// </summary>
    public static string SerializeDictionary(Dictionary<string, string>? dictionary, string defaultLanguage = "en")
    {
        var content = LocalizedContent.FromDictionary(dictionary, defaultLanguage);
        return Serialize(content);
    }

    /// <summary>
    /// Deserializes JSON to a dictionary
    /// </summary>
    public static Dictionary<string, string> DeserializeToDictionary(string? json)
    {
        var content = Deserialize(json);
        return content.ToDictionary();
    }

    /// <summary>
    /// Validates JSON structure is valid LocalizedContent format
    /// </summary>
    public static bool IsValidLocalizedJson(string? json)
    {
        return TryDeserialize(json, out _);
    }

    /// <summary>
    /// Merges multiple LocalizedContent JSON strings
    /// Later strings override earlier ones
    /// </summary>
    public static string MergeJsonStrings(params string?[] jsons)
    {
        var result = new LocalizedContent();

        foreach (var json in jsons.Where(j => !string.IsNullOrWhiteSpace(j)))
        {
            if (TryDeserialize(json, out var content))
            {
                result.Merge(content);
            }
        }

        return Serialize(result);
    }

    /// <summary>
    /// Extracts specific languages from a LocalizedContent JSON
    /// </summary>
    public static string ExtractLanguages(string? json, params string[] languageCodes)
    {
        var content = Deserialize(json);
        var filtered = new LocalizedContent
        {
            DefaultLanguage = content.DefaultLanguage
        };

        foreach (var lang in languageCodes)
        {
            if (content.HasTranslation(lang))
            {
                filtered.Set(lang, content.Get(lang));
            }
        }

        return Serialize(filtered);
    }

    /// <summary>
    /// Transforms all translations in a LocalizedContent using a function
    /// </summary>
    public static string TransformTranslations(string? json, Func<string, string> transformer)
    {
        var content = Deserialize(json);
        var result = new LocalizedContent { DefaultLanguage = content.DefaultLanguage };

        foreach (var kvp in content.Translations)
        {
            try
            {
                result.Set(kvp.Key, transformer(kvp.Value));
            }
            catch
            {
                // Keep original value if transformation fails
                result.Set(kvp.Key, kvp.Value);
            }
        }

        return Serialize(result);
    }

    /// <summary>
    /// Adds missing language translations (fills gaps with a default value)
    /// </summary>
    public static string FillMissingLanguages(string? json, string[] requiredLanguages, string defaultValue = "")
    {
        var content = Deserialize(json);

        foreach (var lang in requiredLanguages)
        {
            if (!content.HasTranslation(lang))
            {
                content.Set(lang, defaultValue);
            }
        }

        return Serialize(content);
    }

    /// <summary>
    /// Gets a language from LocalizedContent JSON without full deserialization
    /// More efficient for single language access
    /// </summary>
    public static string? GetLanguageFromJson(string? json, string languageCode)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("translations", out var translations) &&
                translations.TryGetProperty(languageCode.ToLowerInvariant(), out var value))
            {
                return value.GetString();
            }
        }
        catch
        {
            // Fall back to full deserialization
        }

        var content = Deserialize(json);
        return content.Get(languageCode);
    }

    /// <summary>
    /// Gets all language codes available in a LocalizedContent JSON
    /// </summary>
    public static IEnumerable<string> GetLanguagesFromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("translations", out var translations))
            {
                return translations.EnumerateObject().Select(prop => prop.Name).ToList();
            }
        }
        catch
        {
            // Fallback
            var content = Deserialize(json);
            return content.GetAvailableLanguages();
        }

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Compacts a LocalizedContent JSON by removing empty translations
    /// </summary>
    public static string CompactJson(string? json)
    {
        var content = Deserialize(json);
        var result = new LocalizedContent { DefaultLanguage = content.DefaultLanguage };

        foreach (var kvp in content.Translations.Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value)))
        {
            result.Set(kvp.Key, kvp.Value);
        }

        return Serialize(result);
    }

    /// <summary>
    /// Gets statistics about a LocalizedContent JSON
    /// </summary>
    public static LocalizedContentStats GetStats(string? json)
    {
        var content = Deserialize(json);
        var stats = new LocalizedContentStats
        {
            TotalLanguages = content.Count,
            DefaultLanguage = content.DefaultLanguage,
            Languages = content.GetAvailableLanguages().ToList(),
            TotalCharacters = content.Translations.Values.Sum(v => v.Length),
            IsEmpty = content.IsEmpty
        };

        return stats;
    }
}

/// <summary>
/// Statistics about LocalizedContent
/// </summary>
public class LocalizedContentStats
{
    public int TotalLanguages { get; set; }
    public string DefaultLanguage { get; set; } = "en";
    public List<string> Languages { get; set; } = new();
    public int TotalCharacters { get; set; }
    public bool IsEmpty { get; set; }

    public override string ToString() =>
        $"Languages: {TotalLanguages}, Default: {DefaultLanguage}, Chars: {TotalCharacters}, Empty: {IsEmpty}";
}
