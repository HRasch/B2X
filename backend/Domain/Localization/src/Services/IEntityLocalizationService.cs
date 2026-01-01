namespace B2Connect.LocalizationService.Services;

public interface IEntityLocalizationService
{
    /// <summary>
    /// Sets a property translation for the current tenant (from ambient context)
    /// </summary>
    Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, CancellationToken ct = default);

    /// <summary>
    /// Gets a property translation for the current tenant (from ambient context)
    /// </summary>
    Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, CancellationToken ct = default);

    /// <summary>
    /// Sets multiple property translations for the current tenant (from ambient context)
    /// </summary>
    Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, CancellationToken ct = default);

    /// <summary>
    /// Sets a property translation for a specific tenant (overrides ambient context)
    /// </summary>
    Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, Guid tenantId, CancellationToken ct = default);

    /// <summary>
    /// Gets a property translation for a specific tenant (overrides ambient context)
    /// </summary>
    Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, Guid tenantId, CancellationToken ct = default);

    /// <summary>
    /// Sets multiple property translations for a specific tenant (overrides ambient context)
    /// </summary>
    Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, Guid tenantId, CancellationToken ct = default);
}

public class EntityLocalizationService : IEntityLocalizationService
{
    // Ambient context methods (use current tenant from TenantContext)
    public Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, CancellationToken ct = default)
        => SetPropertyTranslationAsync(entityId, propertyName, languageCode, value, TenantContext.RequireTenantId(), ct);

    public Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, CancellationToken ct = default)
        => GetPropertyTranslationAsync(entityId, propertyName, languageCode, TenantContext.RequireTenantId(), ct);

    public Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, CancellationToken ct = default)
        => SetPropertyTranslationsAsync(entityId, propertyName, translations, TenantContext.RequireTenantId(), ct);

    // Explicit tenant methods (override ambient context)
    public Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, Guid tenantId, CancellationToken ct = default)
        => Task.FromResult(true);

    public Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, Guid tenantId, CancellationToken ct = default)
        => Task.FromResult<string?>(null);

    public Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, Guid tenantId, CancellationToken ct = default)
        => Task.FromResult(true);
}
