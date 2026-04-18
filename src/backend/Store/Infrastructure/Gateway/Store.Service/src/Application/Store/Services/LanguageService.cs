using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;

namespace B2X.Store.Application.Store.Services;

public interface ILanguageService
{
    Task<Language?> GetLanguageByIdAsync(Guid languageId, CancellationToken cancellationToken = default);
    Task<Language?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Language>> GetAllActiveLanguagesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<Language> CreateLanguageAsync(Language language, CancellationToken cancellationToken = default);
    Task<Language> UpdateLanguageAsync(Language language, CancellationToken cancellationToken = default);
    Task DeleteLanguageAsync(Guid languageId, CancellationToken cancellationToken = default);
}

public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;

    public LanguageService(ILanguageRepository languageRepository)
    {
        _languageRepository = languageRepository;
    }

    public async Task<Language?> GetLanguageByIdAsync(Guid languageId, CancellationToken cancellationToken = default)
    {
        return await _languageRepository.GetByIdAsync(languageId, cancellationToken);
    }

    public async Task<Language?> GetLanguageByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _languageRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default)
    {
        return await _languageRepository.GetDefaultLanguageAsync(cancellationToken);
    }

    public async Task<ICollection<Language>> GetAllActiveLanguagesAsync(CancellationToken cancellationToken = default)
    {
        return await _languageRepository.GetActiveLanguagesAsync(cancellationToken);
    }

    public async Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _languageRepository.GetLanguagesByStoreAsync(storeId, cancellationToken);
    }

    public async Task<Language> CreateLanguageAsync(Language language, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(language.Code))
            throw new ArgumentException("Language code is required", nameof(language.Code));

        var existing = await _languageRepository.GetByCodeAsync(language.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Language with code '{language.Code}' already exists");

        language.Id = Guid.NewGuid();
        language.CreatedAt = DateTime.UtcNow;
        language.UpdatedAt = DateTime.UtcNow;
        language.Code = language.Code.ToUpper();

        await _languageRepository.AddAsync(language, cancellationToken);
        return language;
    }

    public async Task<Language> UpdateLanguageAsync(Language language, CancellationToken cancellationToken = default)
    {
        var existing = await _languageRepository.GetByIdAsync(language.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Language with ID '{language.Id}' not found");

        language.UpdatedAt = DateTime.UtcNow;
        language.CreatedAt = existing.CreatedAt;
        language.Code = language.Code.ToUpper();

        await _languageRepository.UpdateAsync(language, cancellationToken);
        return language;
    }

    public async Task DeleteLanguageAsync(Guid languageId, CancellationToken cancellationToken = default)
    {
        var language = await _languageRepository.GetByIdAsync(languageId, cancellationToken);
        if (language == null)
            throw new InvalidOperationException($"Language with ID '{languageId}' not found");

        await _languageRepository.DeleteAsync(language, cancellationToken);
    }
}


