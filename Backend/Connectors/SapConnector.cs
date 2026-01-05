using System.Threading;
using System.Threading.Tasks;
using B2Connect.Api.Validation;
using B2Connect.Api.Models.Erp;
using Microsoft.Extensions.Logging;

namespace B2Connect.Api.Connectors;

public class SapConnector : IErpConnector
{
    private readonly ILogger<SapConnector> _logger;
    private readonly ISapApiClient _apiClient;

    public string ErpType => "sap";

    public SapConnector(
        ILogger<SapConnector> logger,
        ISapApiClient apiClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<IEnumerable<ValidationResult>> ValidateDataAsync(ErpData data, CancellationToken ct = default)
    {
        var results = new List<ValidationResult>();

        // SAP-specific material number validation
        if (!string.IsNullOrEmpty(data.Sku) && !IsValidSapMaterialNumber(data.Sku))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "INVALID_SAP_MATERIAL",
                Message = "Material number must be 18 digits",
                FieldPath = "Sku",
                Severity = ValidationSeverity.Error
            });
        }

        // Validate against SAP BAPI
        var sapValidation = await _apiClient.ValidateMaterialAsync(data.Sku, ct);
        if (!sapValidation.IsValid)
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "SAP_VALIDATION_FAILED",
                Message = sapValidation.Message,
                FieldPath = "Sku",
                Severity = ValidationSeverity.Error
            });
        }

        return results;
    }

    private bool IsValidSapMaterialNumber(string sku)
    {
        return sku.Length == 18 && long.TryParse(sku, out _);
    }
}

public interface ISapApiClient
{
    Task<SapValidationResult> ValidateMaterialAsync(string materialNumber, CancellationToken ct = default);
}

public class SapValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}