namespace B2X.LocalizationService.Services;

/// <summary>
/// Service interface for managing localized strings and translations
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets a translated string using the current culture
    /// </summary>
    /// <param name="key">The translation key</param>
    /// <param name="category">The translation category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translated string or fallback value</returns>
    Task<string> GetStringAsync(string key, string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a translated string for a specific language
    /// </summary>
    /// <param name="key">The translation key</param>
    /// <param name="category">The translation category</param>
    /// <param name="language">The language code (e.g., "en", "de")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The translated string or fallback value</returns>
    Task<string> GetStringAsync(string key, string category, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all translations for a category and language
    /// </summary>
    /// <param name="category">The translation category</param>
    /// <param name="language">The language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of all translations in the category</returns>
    Task<Dictionary<string, string>> GetCategoryAsync(string category, string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all supported language codes
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of supported language codes</returns>
    Task<IEnumerable<string>> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets merged translations for a tenant (global + tenant overrides)
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Merged translations dictionary</returns>
    Task<Dictionary<string, string>> GetMergedTranslationsAsync(Guid tenantId, string languageCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk upserts translations for a tenant
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code</param>
    /// <param name="translations">Translations to upsert</param>
    /// <param name="userId">User performing the update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BulkUpsertTranslationsAsync(Guid tenantId, string languageCode, Dictionary<string, string> translations, Guid? userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a tenant translation to global default
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="languageCode">The language code</param>
    /// <param name="keyPath">The translation key path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ResetTranslationToDefaultAsync(Guid tenantId, string languageCode, string keyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets or updates translations for a key
    /// </summary>
    /// <param name="tenantId">Optional tenant ID for tenant-specific translations</param>
    /// <param name="key">The translation key</param>
    /// <param name="category">The translation category</param>
    /// <param name="translations">Dictionary of language codes to translated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetStringAsync(Guid? tenantId, string key, string category, Dictionary<string, string> translations, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current language from the HTTP context
    /// </summary>
    /// <returns>The current language code</returns>
    string GetCurrentLanguage();
}
