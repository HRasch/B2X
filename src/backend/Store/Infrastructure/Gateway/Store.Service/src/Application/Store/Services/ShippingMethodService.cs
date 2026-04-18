using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;
using B2X.Store.Core.Store.Entities;
using B2X.Store.Core.Store.Interfaces;

namespace B2X.Store.Application.Store.Services;

public interface IShippingMethodService
{
    Task<ShippingMethod?> GetShippingMethodByIdAsync(Guid shippingMethodId, CancellationToken cancellationToken = default);
    Task<ShippingMethod?> GetShippingMethodByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default);
    Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default);
    Task<ShippingMethod> CreateShippingMethodAsync(ShippingMethod method, CancellationToken cancellationToken = default);
    Task<ShippingMethod> UpdateShippingMethodAsync(ShippingMethod method, CancellationToken cancellationToken = default);
    Task DeleteShippingMethodAsync(Guid shippingMethodId, CancellationToken cancellationToken = default);
}

public class ShippingMethodService : IShippingMethodService
{
    private readonly IShippingMethodRepository _shippingMethodRepository;

    public ShippingMethodService(IShippingMethodRepository shippingMethodRepository)
    {
        _shippingMethodRepository = shippingMethodRepository;
    }

    public async Task<ShippingMethod?> GetShippingMethodByIdAsync(Guid shippingMethodId, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetByIdAsync(shippingMethodId, cancellationToken);
    }

    public async Task<ShippingMethod?> GetShippingMethodByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetMethodsForCountryAsync(storeId, countryCode, cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetByCarrierAsync(carrier, storeId, cancellationToken);
    }

    public async Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default)
    {
        return await _shippingMethodRepository.GetCheapestMethodAsync(storeId, countryCode, weight, cancellationToken);
    }

    public async Task<ShippingMethod> CreateShippingMethodAsync(ShippingMethod method, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(method.Code))
            throw new ArgumentException("Shipping method code is required", nameof(method.Code));

        var existing = await _shippingMethodRepository.GetByCodeAsync(method.Code, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Shipping method with code '{method.Code}' already exists");

        method.Id = Guid.NewGuid();
        method.CreatedAt = DateTime.UtcNow;
        method.UpdatedAt = DateTime.UtcNow;
        method.Code = method.Code.ToUpper();

        await _shippingMethodRepository.AddAsync(method, cancellationToken);
        return method;
    }

    public async Task<ShippingMethod> UpdateShippingMethodAsync(ShippingMethod method, CancellationToken cancellationToken = default)
    {
        var existing = await _shippingMethodRepository.GetByIdAsync(method.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException($"Shipping method with ID '{method.Id}' not found");

        method.UpdatedAt = DateTime.UtcNow;
        method.CreatedAt = existing.CreatedAt;
        method.Code = method.Code.ToUpper();

        await _shippingMethodRepository.UpdateAsync(method, cancellationToken);
        return method;
    }

    public async Task DeleteShippingMethodAsync(Guid shippingMethodId, CancellationToken cancellationToken = default)
    {
        var method = await _shippingMethodRepository.GetByIdAsync(shippingMethodId, cancellationToken);
        if (method == null)
            throw new InvalidOperationException($"Shipping method with ID '{shippingMethodId}' not found");

        await _shippingMethodRepository.DeleteAsync(method, cancellationToken);
    }
}


