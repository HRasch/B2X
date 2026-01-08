using System.Threading;
using System.Threading.Tasks;
using B2X.Api.Models.Erp;
using B2X.Api.Validation;
using Microsoft.Extensions.Logging;

namespace B2X.Api.Connectors;

public class EnventaConnector : IErpConnector
{
    private readonly ILogger<EnventaConnector> _logger;
    private readonly IEnventaApiClient _apiClient;

    public string ErpType => "enventa";

    public EnventaConnector(
        ILogger<EnventaConnector> logger,
        IEnventaApiClient apiClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<IEnumerable<ValidationResult>> ValidateDataAsync(ErpData data, CancellationToken ct = default)
    {
        var results = new List<ValidationResult>();

        // ERP-specific SKU format validation
        if (!string.IsNullOrEmpty(data.Sku) && !IsValidEnventaSku(data.Sku))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "INVALID_ENVENTA_SKU",
                Message = "SKU must match enventa format: AA123456",
                FieldPath = "Sku",
                Severity = ValidationSeverity.Error
            });
        }

        // Check product existence in ERP
        if (!await ProductExistsInErpAsync(data.Sku, ct))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "PRODUCT_NOT_FOUND_IN_ERP",
                Message = "Product not found in enventa ERP system",
                FieldPath = "Sku",
                Severity = ValidationSeverity.Error
            });
        }

        // Validate price consistency
        if (data.Price.HasValue && !await IsPriceConsistentAsync(data.Sku, data.Price.Value, ct))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "PRICE_INCONSISTENT",
                Message = "Price does not match ERP system",
                FieldPath = "Price",
                Severity = ValidationSeverity.Warning
            });
        }

        return results;
    }

    private bool IsValidEnventaSku(string sku)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(sku, @"^[A-Z]{2}\d{6}$");
    }

    private async Task<bool> ProductExistsInErpAsync(string sku, CancellationToken ct)
    {
        try
        {
            var product = await _apiClient.GetProductAsync(sku, ct);
            return product != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate product existence in ERP for SKU {Sku}", sku);
            return false;
        }
    }

    private async Task<bool> IsPriceConsistentAsync(string sku, decimal price, CancellationToken ct)
    {
        try
        {
            var erpProduct = await _apiClient.GetProductAsync(sku, ct);
            return erpProduct?.Price == price;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate price consistency for SKU {Sku}", sku);
            return true; // Default to valid on error
        }
    }
}

public interface IEnventaApiClient
{
    Task<ErpProduct?> GetProductAsync(string sku, CancellationToken ct = default);
}

public class ErpProduct
{
    public string Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Name { get; set; } = string.Empty;
}
