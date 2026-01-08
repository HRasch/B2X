using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;
using B2X.Store.Core.Store.Entities;
using B2X.Store.Core.Store.Interfaces;

namespace B2X.Store.Application.Store.ReadServices;

/// <summary>
/// Read-only service for PaymentMethod queries
/// Optimized for public API access with no write operations
/// </summary>
public interface IPaymentMethodReadService
{
    Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default);
    Task<PaymentMethod?> GetPaymentMethodByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default);
    Task<ICollection<string>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default);
    Task<int> GetPaymentMethodCountAsync(Guid storeId, CancellationToken cancellationToken = default);
}

public class PaymentMethodReadService : IPaymentMethodReadService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ILogger<PaymentMethodReadService> _logger;

    public PaymentMethodReadService(IPaymentMethodRepository paymentMethodRepository, ILogger<PaymentMethodReadService> logger)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _logger = logger;
    }

    public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment method by ID: {PaymentMethodId}", paymentMethodId);
        return await _paymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken);
    }

    public async Task<PaymentMethod?> GetPaymentMethodByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment method by code: {Code}", code);
        var method = await _paymentMethodRepository.GetByCodeAsync(code, cancellationToken);

        if (method == null)
            _logger.LogWarning("Payment method not found with code: {Code}", code);

        return method;
    }

    public async Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching active payment methods for store: {StoreId}", storeId);
        return await _paymentMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment methods for currency: {CurrencyCode}, store: {StoreId}", currencyCode, storeId);
        return await _paymentMethodRepository.GetByCurrencyAsync(currencyCode, storeId, cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment methods by provider: {Provider}", provider);
        return await _paymentMethodRepository.GetByProviderAsync(provider, cancellationToken);
    }

    public async Task<ICollection<string>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching available payment providers");
        var methods = await _paymentMethodRepository.GetAllAsync(cancellationToken);

        return methods
            .Where(m => m.IsActive && !string.IsNullOrWhiteSpace(m.Provider))
            .Select(m => m.Provider!)
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    public async Task<int> GetPaymentMethodCountAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting active payment methods for store: {StoreId}", storeId);
        var methods = await _paymentMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
        return methods.Count;
    }
}


