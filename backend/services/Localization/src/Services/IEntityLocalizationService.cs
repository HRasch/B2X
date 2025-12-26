using B2Connect.Types.Domain;
using B2Connect.Types.Localization;
using B2Connect.Types.Utilities;

namespace B2Connect.LocalizationService.Services;

/// <summary>
/// Service for working with localized entity content stored as JSON
/// Complements ILocalizationService for entity-specific translations
/// </summary>
public interface IEntityLocalizationService
{
    /// <summary>
    /// Sets a translation for an entity's localized property
    /// </summary>
    Task<bool> SetPropertyTranslationAsync(
        Guid entityId,
        string propertyName,
        string languageCode,
        string value,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a translation for an entity's localized property
    /// </summary>
    Task<string?> GetPropertyTranslationAsync(
        Guid entityId,
        string propertyName,
        string languageCode,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all translations for a specific property in all languages
    /// </summary>
    Task<Dictionary<string, string>> GetPropertyTranslationsAsync(
        Guid entityId,
        string propertyName,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets multiple translations for a property at once
    /// </summary>
    Task<bool> SetPropertyTranslationsAsync(
        Guid entityId,
        string propertyName,
        Dictionary<string, string> translations,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the LocalizedContent object for a property
    /// </summary>
    Task<LocalizedContent?> GetPropertyContentAsync(
        Guid entityId,
        string propertyName,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the complete LocalizedContent for a property
    /// </summary>
    Task<bool> SetPropertyContentAsync(
        Guid entityId,
        string propertyName,
        LocalizedContent content,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a property has translations for all required languages
    /// </summary>
    Task<bool> ValidatePropertyLanguagesAsync(
        Guid entityId,
        string propertyName,
        string[] requiredLanguages,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets missing translations for required languages
    /// </summary>
    Task<List<string>> GetMissingLanguagesAsync(
        Guid entityId,
        string propertyName,
        string[] requiredLanguages,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all localized properties for an entity
    /// </summary>
    Task<Dictionary<string, LocalizedContent>> GetEntityLocalizationsAsync(
        Guid entityId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of entity localization service
/// Stores localized content as JSON in entity properties
/// </summary>
public class EntityLocalizationService : IEntityLocalizationService
{
    private readonly ILocalizationService _localizationService;

    public EntityLocalizationService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<bool> SetPropertyTranslationAsync(
        Guid entityId,
        string propertyName,
        string languageCode,
        string value,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log the change using main localization service
            var key = $"{entityId}_{propertyName}";
            var translations = new Dictionary<string, string> { { languageCode, value } };
            await _localizationService.SetStringAsync(tenantId, key, "entity-localization", translations, cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetPropertyTranslationAsync(
        Guid entityId,
        string propertyName,
        string languageCode,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"{entityId}_{propertyName}";
            var result = await _localizationService.GetStringAsync(key, "entity-localization", languageCode, cancellationToken);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public Task<Dictionary<string, string>> GetPropertyTranslationsAsync(
        Guid entityId,
        string propertyName,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would query from database
        // For now, return empty dictionary
        return Task.FromResult(new Dictionary<string, string>());
    }

    public async Task<bool> SetPropertyTranslationsAsync(
        Guid entityId,
        string propertyName,
        Dictionary<string, string> translations,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var kvp in translations)
            {
                await SetPropertyTranslationAsync(entityId, propertyName, kvp.Key, kvp.Value, tenantId, cancellationToken);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<LocalizedContent?> GetPropertyContentAsync(
        Guid entityId,
        string propertyName,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Returns LocalizedContent object
        // In a real scenario, this would deserialize from stored JSON
        return Task.FromResult<LocalizedContent?>(new LocalizedContent());
    }

    public Task<bool> SetPropertyContentAsync(
        Guid entityId,
        string propertyName,
        LocalizedContent content,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Serializes LocalizedContent to JSON and stores
        var json = content.ToJson();
        // Store json in database...
        return Task.FromResult(true);
    }

    public async Task<bool> ValidatePropertyLanguagesAsync(
        Guid entityId,
        string propertyName,
        string[] requiredLanguages,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var content = await GetPropertyContentAsync(entityId, propertyName, tenantId, cancellationToken);
        return content?.HasAllLanguages(requiredLanguages) ?? false;
    }

    public async Task<List<string>> GetMissingLanguagesAsync(
        Guid entityId,
        string propertyName,
        string[] requiredLanguages,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var content = await GetPropertyContentAsync(entityId, propertyName, tenantId, cancellationToken);
        if (content == null)
            return requiredLanguages.ToList();

        return requiredLanguages
            .Where(lang => !content.HasTranslation(lang))
            .ToList();
    }

    public async Task<Dictionary<string, LocalizedContent>> GetEntityLocalizationsAsync(
        Guid entityId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // This is a placeholder implementation
        // In a real scenario, this would query from the database to retrieve all LocalizedContent properties
        // For entity localization, you typically need to know which properties are localized
        // This would require a metadata table or reflection-based approach
        return await Task.FromResult(new Dictionary<string, LocalizedContent>());
    }
}
