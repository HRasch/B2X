namespace B2Connect.LocalizationService.Services;

public interface IEntityLocalizationService
{
    Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, Guid tenantId, CancellationToken ct = default);
    Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, Guid tenantId, CancellationToken ct = default);
    Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, Guid tenantId, CancellationToken ct = default);
}

public class EntityLocalizationService : IEntityLocalizationService
{
    public Task<bool> SetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, string value, Guid tenantId, CancellationToken ct = default) 
        => Task.FromResult(true);

    public Task<string?> GetPropertyTranslationAsync(Guid entityId, string propertyName, string languageCode, Guid tenantId, CancellationToken ct = default) 
        => Task.FromResult<string?>(null);

    public Task<bool> SetPropertyTranslationsAsync(Guid entityId, string propertyName, Dictionary<string, string> translations, Guid tenantId, CancellationToken ct = default) 
        => Task.FromResult(true);
}
