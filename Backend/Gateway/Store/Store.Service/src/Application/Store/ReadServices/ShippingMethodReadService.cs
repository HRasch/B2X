using B2X.Store.Core.Common.Entities;
using B2X.Store.Core.Common.Interfaces;
using B2X.Store.Core.Store.Entities;
using B2X.Store.Core.Store.Interfaces;

namespace B2X.Store.Application.Store.ReadServices;

/// <summary>
/// Read-only service for ShippingMethod queries
/// Optimized for public API access with no write operations
/// </summary>
public interface IShippingMethodReadService
{
    Task<ShippingMethod?> GetShippingMethodByIdAsync(Guid shippingMethodId, CancellationToken cancellationToken = default);
    Task<ShippingMethod?> GetShippingMethodByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default);
    Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default);
    Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default);
    Task<ICollection<string>> GetAvailableCarriersAsync(Guid storeId, CancellationToken cancellationToken = default);
    Task<decimal> CalculateShippingCostAsync(Guid storeId, string countryCode, Guid shippingMethodId, decimal weight, CancellationToken cancellationToken = default);
    Task<int> GetShippingMethodCountAsync(Guid storeId, CancellationToken cancellationToken = default);
}

public class ShippingMethodReadService : IShippingMethodReadService
{
    private readonly IShippingMethodRepository _shippingMethodRepository;
    private readonly ILogger<ShippingMethodReadService> _logger;

    public ShippingMethodReadService(IShippingMethodRepository shippingMethodRepository, ILogger<ShippingMethodReadService> logger)
    {
        _shippingMethodRepository = shippingMethodRepository;
        _logger = logger;
    }

    public async Task<ShippingMethod?> GetShippingMethodByIdAsync(Guid shippingMethodId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shipping method by ID: {ShippingMethodId}", shippingMethodId);
        return await _shippingMethodRepository.GetByIdAsync(shippingMethodId, cancellationToken);
    }

    public async Task<ShippingMethod?> GetShippingMethodByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shipping method by code: {Code}", code);
        var method = await _shippingMethodRepository.GetByCodeAsync(code, cancellationToken);

        if (method == null)
            _logger.LogWarning("Shipping method not found with code: {Code}", code);

        return method;
    }

    public async Task<ICollection<ShippingMethod>> GetActiveMethodsAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching active shipping methods for store: {StoreId}", storeId);
        return await _shippingMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetMethodsForCountryAsync(Guid storeId, string countryCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shipping methods for country: {CountryCode}, store: {StoreId}", countryCode, storeId);
        return await _shippingMethodRepository.GetMethodsForCountryAsync(storeId, countryCode, cancellationToken);
    }

    public async Task<ICollection<ShippingMethod>> GetByCarrierAsync(string carrier, Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shipping methods by carrier: {Carrier}, store: {StoreId}", carrier, storeId);
        return await _shippingMethodRepository.GetByCarrierAsync(carrier, storeId, cancellationToken);
    }

    public async Task<ShippingMethod?> GetCheapestMethodAsync(Guid storeId, string countryCode, decimal weight, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finding cheapest shipping method for store: {StoreId}, country: {CountryCode}, weight: {Weight}kg", storeId, countryCode, weight);
        return await _shippingMethodRepository.GetCheapestMethodAsync(storeId, countryCode, weight, cancellationToken);
    }

    public async Task<ICollection<string>> GetAvailableCarriersAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching available carriers for store: {StoreId}", storeId);
        var methods = await _shippingMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);

        return methods
            .Where(m => !string.IsNullOrWhiteSpace(m.Carrier))
            .Select(m => m.Carrier!)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<decimal> CalculateShippingCostAsync(Guid storeId, string countryCode, Guid shippingMethodId, decimal weight, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating shipping cost for store: {StoreId}, country: {CountryCode}, method: {ShippingMethodId}, weight: {Weight}kg",
            storeId, countryCode, shippingMethodId, weight);

        var method = await _shippingMethodRepository.GetByIdAsync(shippingMethodId, cancellationToken);
        if (method == null || method.StoreId != storeId)
        {
            _logger.LogWarning("Shipping method not found or does not belong to store");
            throw new InvalidOperationException("Shipping method not found or does not belong to this store");
        }

        // Validate weight constraints
        if (method.MaximumWeight.HasValue && weight > method.MaximumWeight)
        {
            throw new InvalidOperationException($"Package weight {weight}kg exceeds maximum allowed {method.MaximumWeight}kg for this shipping method");
        }

        // Calculate base cost
        var baseCost = method.BaseCost;
        var weightCost = weight * (method.CostPerKg ?? 0);
        var totalCost = baseCost + weightCost;

        _logger.LogInformation("Calculated shipping cost: {TotalCost} (base: {BaseCost}, weight: {WeightCost})",
            totalCost, baseCost, weightCost);

        return totalCost;
    }

    public async Task<int> GetShippingMethodCountAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting active shipping methods for store: {StoreId}", storeId);
        var methods = await _shippingMethodRepository.GetActiveMethodsAsync(storeId, cancellationToken);
        return methods.Count;
    }
}


