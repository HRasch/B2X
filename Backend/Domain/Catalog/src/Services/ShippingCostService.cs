using B2X.Catalog.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Services;

/// <summary>
/// Service to calculate shipping costs per country and method
/// Implements PAngV compliance: Shipping costs visible before checkout
/// </summary>
public class ShippingCostService
{
    private readonly ILogger<ShippingCostService> _logger;

    // Hardcoded shipping methods - in production, would be in database
    private static readonly List<ShippingMethod> ShippingMethods = new()
    {
        new ShippingMethod
        {
            Id = "dhl-express",
            Name = "DHL Express",
            Provider = "DHL",
            BaseCost = 4.99m,
            Description = "Express delivery (1-2 business days)",
            EstimatedDaysMin = 1,
            EstimatedDaysMax = 2,
            MaxWeight = 30m,
            IsActive = true,
        },
        new ShippingMethod
        {
            Id = "dpd-standard",
            Name = "DPD Standard",
            Provider = "DPD",
            BaseCost = 3.99m,
            Description = "Standard delivery (3-5 business days)",
            EstimatedDaysMin = 3,
            EstimatedDaysMax = 5,
            MaxWeight = 30m,
            IsActive = true,
        },
        new ShippingMethod
        {
            Id = "postnl-standard",
            Name = "PostNL Standard",
            Provider = "PostNL",
            BaseCost = 5.99m,
            Description = "PostNL Standard (2-4 business days)",
            EstimatedDaysMin = 2,
            EstimatedDaysMax = 4,
            MaxWeight = 20m,
            IsActive = true,
        },
    };

    // Country-specific shipping cost adjustments (in EUR)
    private static readonly Dictionary<string, decimal> CountryShippingMultipliers = new(StringComparer.Ordinal)
    {
        // Germany
        { "DE", 1.0m },
        // EU Countries - 1.5x multiplier
        { "AT", 1.5m },
        { "BE", 1.5m },
        { "BG", 1.5m },
        { "HR", 1.5m },
        { "CY", 1.5m },
        { "CZ", 1.5m },
        { "DK", 1.5m },
        { "EE", 1.5m },
        { "ES", 1.5m },
        { "FR", 1.5m },
        { "GR", 1.5m },
        { "HU", 1.5m },
        { "IE", 1.5m },
        { "IT", 1.5m },
        { "LV", 1.5m },
        { "LT", 1.5m },
        { "LU", 1.5m },
        { "NL", 1.5m },
        { "PL", 1.5m },
        { "PT", 1.5m },
        { "RO", 1.5m },
        { "SE", 1.5m },
        { "SI", 1.5m },
        { "SK", 1.5m },
        // Switzerland
        { "CH", 2.0m },
        // UK
        { "GB", 2.0m },
    };

    public ShippingCostService(ILogger<ShippingCostService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get available shipping methods for a destination country with costs
    /// </summary>
    public async Task<GetShippingMethodsResponse> GetShippingMethodsAsync(
        string destinationCountry,
        decimal? totalWeight = null,
        decimal? orderTotal = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching shipping methods for country: {Country}, Weight: {Weight}kg",
            destinationCountry, totalWeight ?? 0);

        try
        {
            // Validate country
            if (string.IsNullOrWhiteSpace(destinationCountry))
            {
                return new GetShippingMethodsResponse
                {
                    Success = false,
                    Message = "Destination country is required",
                };
            }

            var countryCode = destinationCountry.ToUpperInvariant();
            var multiplier = GetCountryMultiplier(countryCode);

            // Get available methods
            var availableMethods = ShippingMethods
                .Where(m => m.IsActive && (totalWeight == null || m.MaxWeight == null || totalWeight <= m.MaxWeight))
                .ToList();

            if (availableMethods.Count == 0)
            {
                _logger.LogWarning("No available shipping methods for country {Country} with weight {Weight}kg",
                    countryCode, totalWeight ?? 0);

                return new GetShippingMethodsResponse
                {
                    Success = false,
                    Message = $"Shipping not available to {countryCode}",
                };
            }

            // Calculate costs with country multiplier
            var methodDtos = availableMethods
                .Select(m => new ShippingMethodDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Provider = m.Provider,
                    Description = m.Description,
                    Cost = Math.Round(m.BaseCost * multiplier, 2),
                    EstimatedDaysMin = m.EstimatedDaysMin,
                    EstimatedDaysMax = m.EstimatedDaysMax,
                    CurrencyCode = "EUR",
                })
                .ToList();

            _logger.LogInformation("Found {Count} shipping methods for {Country}",
                methodDtos.Count, countryCode);

            return new GetShippingMethodsResponse
            {
                Success = true,
                Methods = methodDtos,
                Message = "Shipping methods retrieved successfully",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching shipping methods for country: {Country}",
                destinationCountry);

            return new GetShippingMethodsResponse
            {
                Success = false,
                Message = "Error retrieving shipping methods",
            };
        }
    }

    /// <summary>
    /// Calculate total cost including selected shipping method
    /// </summary>
    public async Task<decimal> CalculateTotalAsync(
        decimal subtotal,
        string shippingMethodId,
        string destinationCountry,
        decimal? totalWeight = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating total: Subtotal={Subtotal}, Method={Method}, Country={Country}",
            subtotal, shippingMethodId, destinationCountry);

        // Get shipping cost
        var shippingResponse = await GetShippingMethodsAsync(
            destinationCountry, totalWeight, subtotal, cancellationToken).ConfigureAwait(false);

        if (!shippingResponse.Success)
        {
            _logger.LogWarning("Failed to get shipping cost: {Message}", shippingResponse.Message);
            return subtotal; // Fallback: return subtotal without shipping
        }

        var selectedMethod = shippingResponse.Methods
            .FirstOrDefault(m => string.Equals(m.Id, shippingMethodId, StringComparison.Ordinal));

        if (selectedMethod == null)
        {
            _logger.LogWarning("Shipping method {Method} not found", shippingMethodId);
            return subtotal;
        }

        var total = Math.Round(subtotal + selectedMethod.Cost, 2);

        _logger.LogInformation("Total calculated: {Total} (Subtotal: {Subtotal} + Shipping: {Shipping})",
            total, subtotal, selectedMethod.Cost);

        return total;
    }

    /// <summary>
    /// Get cost multiplier for country
    /// </summary>
    private decimal GetCountryMultiplier(string countryCode)
    {
        if (CountryShippingMultipliers.TryGetValue(countryCode, out var multiplier))
        {
            return multiplier;
        }

        _logger.LogWarning("Country {Country} not found, using 2.0x multiplier", countryCode);
        return 2.0m; // Default for unknown countries
    }

    /// <summary>
    /// Get free shipping threshold for country
    /// </summary>
    public decimal GetFreeShippingThreshold(string destinationCountry)
    {
        return destinationCountry?.ToUpperInvariant() switch
        {
            "DE" => 50.00m,
            "AT" or "BE" or "NL" => 75.00m,
            "FR" or "IT" or "ES" => 100.00m,
            _ => 150.00m,
        };
    }
}
