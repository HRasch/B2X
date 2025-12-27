using B2Connect.Store.Core.Store.Entities;

namespace B2Connect.Store.Core.Store.Interfaces;

/// <summary>
/// Repository interface for ShippingMethod operations
/// Store-specific interface for Shipping Method management
/// </summary>
public interface IShippingMethodRepository : B2Connect.Store.Core.Common.Interfaces.IRepository<ShippingMethod>
{
    /// <summary>Gets a shipping method by code</summary>
    Task<ShippingMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Gets all active shipping methods for a store</summary>
    Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);

    /// <summary>Gets shipping methods for a specific country</summary>
    Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default);

    /// <summary>Gets shipping methods by carrier</summary>
    Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default);

    /// <summary>Gets cheapest shipping method for destination</summary>
    Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default);
}

