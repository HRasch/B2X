using B2X.Store.Core.Store.Entities;

namespace B2X.Store.Core.Store.Interfaces;

/// <summary>
/// Repository interface for PaymentMethod operations
/// Store-specific interface for Payment Method management
/// </summary>
public interface IPaymentMethodRepository : B2X.Store.Core.Common.Interfaces.IRepository<PaymentMethod>
{
    /// <summary>Gets a payment method by code</summary>
    Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Gets all active payment methods for a store</summary>
    Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);

    /// <summary>Gets payment methods by category</summary>
    Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default);

    /// <summary>Gets payment methods supporting a specific currency</summary>
    Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default);
}

