using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Application.Store.ReadServices;

/// <summary>
/// Read-only service for Language queries
/// Optimized for public API access with no write operations
/// </summary>
public interface ILanguageReadService
{
    Task<Language?> GetLanguageByIdAsync(Guid languageId, CancellationToken cancellationToken = default);
    Task<Language?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Language>> GetAllLanguagesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<int> GetLanguageCountAsync(CancellationToken cancellationToken = default);
}

public class LanguageReadService : ILanguageReadService
{
    private readonly ILanguageRepository _languageRepository;
    private readonly ILogger<LanguageReadService> _logger;

    public LanguageReadService(ILanguageRepository languageRepository, ILogger<LanguageReadService> logger)
    {
        _languageRepository = languageRepository;
        _logger = logger;
    }

    public async Task<Language?> GetLanguageByIdAsync(Guid languageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching language by ID: {LanguageId}", languageId);
        return await _languageRepository.GetByIdAsync(languageId, cancellationToken);
    }

    public async Task<Language?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching language by code: {Code}", code);
        var language = await _languageRepository.GetByCodeAsync(code, cancellationToken);

        if (language == null)
            _logger.LogWarning("Language not found with code: {Code}", code);

        return language;
    }

    public async Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching default language");
        return await _languageRepository.GetDefaultLanguageAsync(cancellationToken);
    }

    public async Task<ICollection<Language>> GetAllLanguagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all active languages");
        return await _languageRepository.GetActiveLanguagesAsync(cancellationToken);
    }

    public async Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching languages for store: {StoreId}", storeId);
        return await _languageRepository.GetLanguagesByStoreAsync(storeId, cancellationToken);
    }

    public async Task<int> GetLanguageCountAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting active languages");
        return await _languageRepository.CountAsync(cancellationToken);
    }
}


