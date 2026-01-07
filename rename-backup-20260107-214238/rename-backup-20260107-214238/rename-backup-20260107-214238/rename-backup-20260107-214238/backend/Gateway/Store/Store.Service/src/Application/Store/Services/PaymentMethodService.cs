using B2Connect.Store.Core.Common.Entities;
using B2Connect.Store.Core.Common.Interfaces;
using B2Connect.Store.Core.Store.Entities;
using B2Connect.Store.Core.Store.Interfaces;

namespace B2Connect.Store.Application.Store.Services;

public interface IPaymentMethodService
{
    Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default);
    Task<PaymentMethod?> GetPaymentMethodByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default);
    Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default);
    Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod method, CancellationToken cancellationToken = default);
    Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod method, CancellationToken cancellationToken = default);
    Task DeletePaymentMethodAsync(Guid paymentMethodId, CancellationToken cancellationToken = default);
}

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken);
    }

    public async Task<PaymentMethod?> GetPaymentMethodByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByProviderAsync(string provider, CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.GetByProviderAsync(provider, cancellationToken);
    }

    public async Task<ICollection<PaymentMethod>> GetByCurrencyAsync(string currencyCode, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _paymentMethodRepository.GetByCurrencyAsync(currencyCode, storeId, cancellationToken);
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod method, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(method.Code))
            throw new ArgumentException("Payment method code is required", nameof(method.Code));

        var existing = await _paymentMethodRepository.GetByCodeAsync(method.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Payment method with code '{method.Code}' already exists");

        method.Id = Guid.NewGuid();
        method.CreatedAt = DateTime.UtcNow;
        method.UpdatedAt = DateTime.UtcNow;
        method.Code = method.Code.ToUpper();

        await _paymentMethodRepository.AddAsync(method, cancellationToken);
        return method;
    }

    public async Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod method, CancellationToken cancellationToken = default)
    {
        var existing = await _paymentMethodRepository.GetByIdAsync(method.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Payment method with ID '{method.Id}' not found");

        method.UpdatedAt = DateTime.UtcNow;
        method.CreatedAt = existing.CreatedAt;
        method.Code = method.Code.ToUpper();

        await _paymentMethodRepository.UpdateAsync(method, cancellationToken);
        return method;
    }

    public async Task DeletePaymentMethodAsync(Guid paymentMethodId, CancellationToken cancellationToken = default)
    {
        var method = await _paymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken);
        if (method == null)
            throw new InvalidOperationException($"Payment method with ID '{paymentMethodId}' not found");

        await _paymentMethodRepository.DeleteAsync(method, cancellationToken);
    }
}


