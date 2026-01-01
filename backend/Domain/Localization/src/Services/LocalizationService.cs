using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;

namespace B2Connect.LocalizationService.Services;

/// <summary>
/// Implementation of localization service with caching support
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly LocalizationDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CACHE_PREFIX = "i18n:";
    private const int CACHE_DURATION_MINUTES = 60;
    private static readonly string[] SupportedLanguages = { "en", "de", "fr", "es", "it", "pt", "nl", "pl" };

    public LocalizationService(
        LocalizationDbContext dbContext,
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetCurrentLanguage()
    {
        var language = _httpContextAccessor.HttpContext?.Items["Language"] as string;
        return string.IsNullOrEmpty(language) ? "en" : language;
    }

    public async Task<string> GetStringAsync(string key, string category, CancellationToken cancellationToken = default)
    {
        var language = GetCurrentLanguage();
        return await GetStringAsync(key, category, language, cancellationToken);
    }

    public async Task<string> GetStringAsync(string key, string category, string language, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));

        language = NormalizeLanguage(language);

        var cacheKey = $"{CACHE_PREFIX}{category}:{key}:{language}";

        if (_cache.TryGetValue(cacheKey, out var cachedValue))
        {
            return (string)cachedValue!;
        }

        var localized = await _dbContext.LocalizedStringEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(
                ls => ls.LocalizedString.Key == key && ls.LocalizedString.Category == category,
                cancellationToken);

        if (localized is null)
        {
            return $"[{category}.{key}]";
        }

        // Try exact language match
        if (localized.LocalizedString.Translations.TryGetValue(language, out var value))
        {
            _cache.Set(cacheKey, value, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
            return value;
        }

        // Fallback to English
        if (localized.LocalizedString.Translations.TryGetValue("en", out var fallback))
        {
            _cache.Set(cacheKey, fallback, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
            return fallback;
        }

        // Final fallback to default value
        _cache.Set(cacheKey, localized.LocalizedString.DefaultValue, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        return localized.LocalizedString.DefaultValue;
    }

    public async Task<Dictionary<string, string>> GetCategoryAsync(string category, string language, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));

        language = NormalizeLanguage(language);

        var cacheKey = $"{CACHE_PREFIX}{category}:{language}";

        if (_cache.TryGetValue(cacheKey, out var cachedDict))
        {
            return (Dictionary<string, string>)cachedDict!;
        }

        var strings = await _dbContext.LocalizedStringEntities
            .AsNoTracking()
            .Where(ls => ls.LocalizedString.Category == category)
            .ToListAsync(cancellationToken);

        var result = new Dictionary<string, string>();

        foreach (var localized in strings)
        {
            var translatedValue = localized.LocalizedString.Translations.TryGetValue(language, out var val)
                ? val
                : localized.LocalizedString.DefaultValue;

            result[localized.LocalizedString.Key] = translatedValue;
        }

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
        return result;
    }

    public Task<IEnumerable<string>> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IEnumerable<string>)SupportedLanguages);
    }

    public Task SetStringAsync(string key, string category, Dictionary<string, string> translations, CancellationToken cancellationToken = default)
        => SetStringAsync(TenantContext.CurrentTenantId, key, category, translations, cancellationToken);

    public async Task SetStringAsync(
        Guid? tenantId,
        string key,
        string category,
        Dictionary<string, string> translations,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be null or empty", nameof(category));

        if (translations is null || translations.Count == 0)
            throw new ArgumentException("Translations cannot be null or empty", nameof(translations));

        var existing = await _dbContext.LocalizedStringEntities
            .FirstOrDefaultAsync(
                ls => ls.LocalizedString.Key == key && ls.LocalizedString.Category == category && ls.TenantId == tenantId,
                cancellationToken);

        if (existing is null)
        {
            var localizedString = new LocalizedString(
                key: key,
                category: category,
                defaultValue: translations.GetValueOrDefault("en") ?? key,
                translations: new Dictionary<string, string>(translations)
            );

            existing = new LocalizedStringEntity(
                tenantId: tenantId ?? Guid.Empty,
                localizedString: localizedString
            );

            _dbContext.LocalizedStringEntities.Add(existing);
        }
        else
        {
            // Create new LocalizedString with updated translations
            var updatedLocalizedString = existing.LocalizedString.WithTranslations(new Dictionary<string, string>(translations));
            existing.LocalizedString = updatedLocalizedString;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        InvalidateCache(category);
    }

    private void InvalidateCache(string category)
    {
        // In a production app, use IDistributedCache with explicit cache invalidation
        // For now, we rely on TTL expiration
    }

    private string NormalizeLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return "en";

        // Extract first 2 characters (e.g., "de" from "de-DE")
        var normalized = language.Substring(0, Math.Min(2, language.Length)).ToLower();
        return normalized;
    }
}
