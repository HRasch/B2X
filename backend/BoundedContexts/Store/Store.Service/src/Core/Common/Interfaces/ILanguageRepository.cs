using B2Connect.Store.Core.Common.Entities;

namespace B2Connect.Store.Core.Common.Interfaces;

/// <summary>
/// Repository interface for Language operations
/// Common interface for Language entity management
/// </summary>
public interface ILanguageRepository : IRepository<Language>
{
    /// <summary>Gets a language by code (e.g., 'de', 'en')</summary>
    Task<Language?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Gets the default language</summary>
    Task<Language?> GetDefaultLanguageAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets all active languages</summary>
    Task<ICollection<Language>> GetActiveLanguagesAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets languages supported by a store</summary>
    Task<ICollection<Language>> GetLanguagesByStoreAsync(Guid storeId, CancellationToken cancellationToken = default);
}

