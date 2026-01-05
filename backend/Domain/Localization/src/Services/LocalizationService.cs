using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using B2Connect.LocalizationService.Data;
using B2Connect.LocalizationService.Models;

namespace B2Connect.LocalizationService.Services;

/// <summary>
/// Implementation of localization service with distributed caching support
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly LocalizationDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CACHE_PREFIX = "i18n:";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
    };
    private static readonly DistributedCacheEntryOptions SsrCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    };
    private static readonly string[] SupportedLanguages = { "en", "de", "fr", "es", "it", "pt", "nl", "pl" };

    public LocalizationService(
        LocalizationDbContext dbContext,
        IDistributedCache cache,
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

    public Task<string> GetStringAsync(string key, string category, CancellationToken cancellationToken = default)
    {
        var language = GetCurrentLanguage();
        return GetStringAsync(key, category, language, cancellationToken);
    }

    public async Task<string> GetStringAsync(string key, string category, string language, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        }

        language = NormalizeLanguage(language);

        var cacheKey = $"{CACHE_PREFIX}{category}:{key}:{language}";

        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return cachedValue;
        }

        var localized = await _dbContext.LocalizedStrings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                ls => ls.Key == key && ls.Category == category,
                cancellationToken);

        if (localized is null)
        {
            return $"[{category}.{key}]";
        }

        // Try exact language match
        if (localized.Translations.TryGetValue(language, out var value))
        {
            await _cache.SetStringAsync(cacheKey, value, CacheOptions, cancellationToken);
            return value;
        }

        // Fallback to English
        if (localized.Translations.TryGetValue("en", out var fallback))
        {
            await _cache.SetStringAsync(cacheKey, fallback, CacheOptions, cancellationToken);
            return fallback;
        }

        // Final fallback to default value
        await _cache.SetStringAsync(cacheKey, localized.DefaultValue, CacheOptions, cancellationToken);
        return localized.DefaultValue;
    }

    public async Task<Dictionary<string, string>> GetCategoryAsync(string category, string language, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        }

        language = NormalizeLanguage(language);

        var cacheKey = $"{CACHE_PREFIX}{category}:{language}";

        var cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(cachedJson)!;
        }

        var strings = await _dbContext.LocalizedStrings
            .AsNoTracking()
            .Where(ls => ls.Category == category)
            .ToListAsync(cancellationToken);

        var result = new Dictionary<string, string>();

        foreach (var localized in strings)
        {
            var translatedValue = localized.Translations.TryGetValue(language, out var val)
                ? val
                : localized.DefaultValue;

            result[localized.Key] = translatedValue;
        }

        var resultJson = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, resultJson, CacheOptions, cancellationToken);
        return result;
    }

    public Task<IEnumerable<string>> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IEnumerable<string>)SupportedLanguages);
    }

    public async Task SetStringAsync(
        Guid? tenantId,
        string key,
        string category,
        Dictionary<string, string> translations,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category cannot be null or empty", nameof(category));
        }

        if (translations is null || translations.Count == 0)
        {
            throw new ArgumentException("Translations cannot be null or empty", nameof(translations));
        }

        var existing = await _dbContext.LocalizedStrings
            .FirstOrDefaultAsync(
                ls => ls.Key == key && ls.Category == category && ls.TenantId == tenantId,
                cancellationToken);

        if (existing is null)
        {
            existing = new LocalizedString
            {
                Id = Guid.NewGuid(),
                Key = key,
                Category = category,
                TenantId = tenantId,
                DefaultValue = translations.GetValueOrDefault("en") ?? key,
                Translations = new Dictionary<string, string>(translations),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.LocalizedStrings.Add(existing);
        }
        else
        {
            existing.Translations = new Dictionary<string, string>(translations);
            existing.DefaultValue = translations.GetValueOrDefault("en") ?? existing.DefaultValue;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        InvalidateCache(category);
    }

    public async Task<Dictionary<string, string>> GetMergedTranslationsAsync(Guid tenantId, string languageCode, CancellationToken cancellationToken = default)
    {
        languageCode = NormalizeLanguage(languageCode);

        var cacheKey = $"{CACHE_PREFIX}merged:{tenantId}:{languageCode}";

        var cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(cachedJson)!;
        }

        // Get all global translations (TenantId is null)
        var globalTranslations = await _dbContext.LocalizedStrings
            .AsNoTracking()
            .Where(ls => ls.TenantId == null && ls.IsActive)
            .Select(ls => new
            {
                Key = $"{ls.Category}.{ls.Key}",
                Translations = ls.Translations
            })
            .ToListAsync(cancellationToken);

        // Get tenant-specific overrides
        var tenantOverrides = await _dbContext.LocalizedStrings
            .AsNoTracking()
            .Where(ls => ls.TenantId == tenantId && ls.IsActive)
            .Select(ls => new
            {
                Key = $"{ls.Category}.{ls.Key}",
                Translations = ls.Translations
            })
            .ToListAsync(cancellationToken);

        // Merge translations: tenant overrides take precedence
        var merged = new Dictionary<string, string>();

        // Add global translations first
        foreach (var global in globalTranslations)
        {
            if (global.Translations.TryGetValue(languageCode, out var value))
            {
                merged[global.Key] = value;
            }
            else if (global.Translations.TryGetValue("en", out var fallback))
            {
                merged[global.Key] = fallback;
            }
        }

        // Override with tenant-specific translations
        foreach (var tenant in tenantOverrides)
        {
            if (tenant.Translations.TryGetValue(languageCode, out var value))
            {
                merged[tenant.Key] = value;
            }
        }

        var mergedJson = JsonSerializer.Serialize(merged);
        await _cache.SetStringAsync(cacheKey, mergedJson, CacheOptions, cancellationToken);
        return merged;
    }

    public async Task BulkUpsertTranslationsAsync(Guid tenantId, string languageCode, Dictionary<string, string> translations, Guid? userId, CancellationToken cancellationToken = default)
    {
        if (translations is null || translations.Count == 0)
        {
            throw new ArgumentException("Translations cannot be null or empty", nameof(translations));
        }

        languageCode = NormalizeLanguage(languageCode);

        foreach (var kvp in translations)
        {
            var keyParts = kvp.Key.Split('.', 2);
            if (keyParts.Length != 2)
            {
                throw new ArgumentException($"Invalid key format: {kvp.Key}. Expected 'category.key'", nameof(translations));
            }

            var category = keyParts[0];
            var key = keyParts[1];

            var existing = await _dbContext.LocalizedStrings
                .FirstOrDefaultAsync(
                    ls => ls.Key == key && ls.Category == category && ls.TenantId == tenantId,
                    cancellationToken);

            if (existing is null)
            {
                existing = new LocalizedString
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Category = category,
                    TenantId = tenantId,
                    DefaultValue = kvp.Value,
                    Translations = new Dictionary<string, string> { [languageCode] = kvp.Value },
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _dbContext.LocalizedStrings.Add(existing);
            }
            else
            {
                existing.Translations[languageCode] = kvp.Value;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        InvalidateCache($"tenant:{tenantId}");
    }

    public async Task ResetTranslationToDefaultAsync(Guid tenantId, string languageCode, string keyPath, CancellationToken cancellationToken = default)
    {
        var keyParts = keyPath.Split('.', 2);
        if (keyParts.Length != 2)
        {
            throw new ArgumentException($"Invalid key format: {keyPath}. Expected 'category.key'", nameof(keyPath));
        }

        var category = keyParts[0];
        var key = keyParts[1];
        languageCode = NormalizeLanguage(languageCode);

        var tenantOverride = await _dbContext.LocalizedStrings
            .FirstOrDefaultAsync(
                ls => ls.Key == key && ls.Category == category && ls.TenantId == tenantId,
                cancellationToken);

        if (tenantOverride != null)
        {
            // Remove the language-specific override
            tenantOverride.Translations.Remove(languageCode);

            // If no translations left, deactivate the override
            if (tenantOverride.Translations.Count == 0)
            {
                tenantOverride.IsActive = false;
            }

            tenantOverride.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        InvalidateCache($"tenant:{tenantId}");
    }

    private static void InvalidateCache(string category)
    {
        // In a production app, use IDistributedCache with explicit cache invalidation
        // For now, we rely on TTL expiration
    }

    private static string NormalizeLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return "en";
        }

        // Extract first 2 characters (e.g., "de" from "de-DE")
        var normalized = language.Substring(0, Math.Min(2, language.Length)).ToLower(System.Globalization.CultureInfo.CurrentCulture);
        return normalized;
    }
}
