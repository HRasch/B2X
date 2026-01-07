using System.Reflection;
using B2X.Types.Domain;
using B2X.Types.Localization;

namespace B2X.Types.Extensions;

/// <summary>
/// Extension methods for working with localized content in entities
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Gets all LocalizedContent properties from an entity
    /// </summary>
    public static Dictionary<string, LocalizedContent> GetLocalizedProperties(this Entity entity)
    {
        var result = new Dictionary<string, LocalizedContent>();
        var properties = entity.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(LocalizedContent) && p.CanRead);

        foreach (var property in properties)
        {
            if (property.GetValue(entity) is LocalizedContent value)
            {
                result[property.Name] = value;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets a specific localized property by name
    /// </summary>
    public static LocalizedContent? GetLocalizedProperty(this Entity entity, string propertyName)
    {
        var property = entity.GetType()
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property?.PropertyType != typeof(LocalizedContent) || !property.CanRead)
        {
            return null;
        }

        return property.GetValue(entity) as LocalizedContent;
    }

    /// <summary>
    /// Sets a localized property on an entity
    /// </summary>
    public static void SetLocalizedProperty(this Entity entity, string propertyName, LocalizedContent content)
    {
        var property = entity.GetType()
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property?.PropertyType != typeof(LocalizedContent) || !property.CanWrite)
        {
            return;
        }

        property.SetValue(entity, content);
    }

    /// <summary>
    /// Gets a translated value for a specific property and language
    /// </summary>
    public static string? GetTranslation(this Entity entity, string propertyName, string languageCode)
    {
        var content = entity.GetLocalizedProperty(propertyName);
        if (content?.IsEmpty != false)
        {
            return null;
        }

        var translation = content.Get(languageCode);
        return string.IsNullOrEmpty(translation) ? null : translation;
    }

    /// <summary>
    /// Sets a translation for a specific property and language
    /// </summary>
    public static void SetTranslation(this Entity entity, string propertyName, string languageCode, string value)
    {
        var content = entity.GetLocalizedProperty(propertyName) ?? new LocalizedContent();
        content.Set(languageCode, value);
        entity.SetLocalizedProperty(propertyName, content);
    }

    /// <summary>
    /// Gets all translations for a property across all languages
    /// </summary>
    public static Dictionary<string, string> GetAllTranslations(this Entity entity, string propertyName)
    {
        var content = entity.GetLocalizedProperty(propertyName);
        return content?.ToDictionary() ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Sets multiple translations for a property at once
    /// </summary>
    public static void SetAllTranslations(this Entity entity, string propertyName, Dictionary<string, string> translations)
    {
        var content = LocalizedContent.FromDictionary(translations);
        entity.SetLocalizedProperty(propertyName, content);
    }

    /// <summary>
    /// Gets the language codes available for a specific property
    /// </summary>
    public static IEnumerable<string> GetAvailableLanguagesForProperty(this Entity entity, string propertyName)
    {
        var content = entity.GetLocalizedProperty(propertyName);
        return content?.GetAvailableLanguages() ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Checks if a property has a translation for a specific language
    /// </summary>
    public static bool HasTranslation(this Entity entity, string propertyName, string languageCode)
    {
        var content = entity.GetLocalizedProperty(propertyName);
        return content?.HasTranslation(languageCode) ?? false;
    }

    /// <summary>
    /// Gets a summary of all localized content in an entity
    /// </summary>
    public static Dictionary<string, Dictionary<string, string>> GetLocalizationSummary(this Entity entity)
    {
        var summary = new Dictionary<string, Dictionary<string, string>>();
        var localizedProps = entity.GetLocalizedProperties();

        foreach (var kvp in localizedProps)
        {
            summary[kvp.Key] = kvp.Value.ToDictionary();
        }

        return summary;
    }

    /// <summary>
    /// Validates that all localized properties have translations for required languages
    /// </summary>
    public static bool HasAllRequiredTranslations(this Entity entity, params string[] requiredLanguages)
    {
        var localizedProps = entity.GetLocalizedProperties();
        if (localizedProps.Count == 0)
        {
            return true;
        }

        return localizedProps.Values.All(content => content.HasAllLanguages(requiredLanguages));
    }

    /// <summary>
    /// Gets missing translations for required languages across all properties
    /// </summary>
    public static Dictionary<string, List<string>> GetMissingTranslations(this Entity entity, params string[] requiredLanguages)
    {
        var missing = new Dictionary<string, List<string>>();
        var localizedProps = entity.GetLocalizedProperties();
        var requiredLangs = requiredLanguages.Select(l => l.ToLowerInvariant()).ToHashSet();

        foreach (var kvp in localizedProps)
        {
            var missingLangs = requiredLangs.Where(lang => !kvp.Value.HasTranslation(lang)).ToList();
            if (missingLangs.Count > 0)
            {
                missing[kvp.Key] = missingLangs;
            }
        }

        return missing;
    }

    /// <summary>
    /// Clones localized content from one entity property to another entity property
    /// </summary>
    public static void CloneLocalization(this Entity sourceEntity, string sourcePropertyName,
        Entity targetEntity, string targetPropertyName)
    {
        var content = sourceEntity.GetLocalizedProperty(sourcePropertyName);
        if (content == null)
        {
            return;
        }

        targetEntity.SetLocalizedProperty(targetPropertyName, content.Clone());
    }

    /// <summary>
    /// Merges localized content from another entity property
    /// </summary>
    public static void MergeLocalization(this Entity entity, string targetPropertyName,
        LocalizedContent sourceContent)
    {
        var content = entity.GetLocalizedProperty(targetPropertyName) ?? new LocalizedContent();
        content.Merge(sourceContent);
        entity.SetLocalizedProperty(targetPropertyName, content);
    }
}
