using B2X.Store.Core.Common.Entities;

namespace B2X.Store.Core.Common.Interfaces;

/// <summary>
/// Repository interface for Shop entity
/// </summary>
public interface IShopRepository : IRepository<Shop>
{
    /// <summary>
    /// Gets a shop by its code
    /// </summary>
    Task<Shop?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the main shop
    /// </summary>
    Task<Shop?> GetMainShopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active shops
    /// </summary>
    Task<ICollection<Shop>> GetActiveShopsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets shops in a specific country
    /// </summary>
    Task<ICollection<Shop>> GetShopsByCountryAsync(Guid countryId, CancellationToken cancellationToken = default);
}
