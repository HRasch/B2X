using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using B2X.Api.Models.Erp;
using B2X.Api.Validation;
using Microsoft.Extensions.Logging;

namespace B2X.Api.Validation;

public class ErpSpecificValidator : IDataValidator<ErpData>
{
    private readonly ILogger<ErpSpecificValidator> _logger;
    private readonly IErpConnector _connector;
    private readonly IValidationRulesProvider _rulesProvider;

    public ErpSpecificValidator(
        ILogger<ErpSpecificValidator> logger,
        IErpConnector connector,
        IValidationRulesProvider rulesProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connector = connector ?? throw new ArgumentNullException(nameof(connector));
        _rulesProvider = rulesProvider ?? throw new ArgumentNullException(nameof(rulesProvider));
    }

    public async Task<ValidationResult> ValidateAsync(ErpData data, CancellationToken ct = default)
    {
        var results = new List<ValidationResult>();

        // ERP-specific business rules
        results.AddRange(await ValidateErpBusinessRulesAsync(data, ct));

        // Connector-specific validations
        results.AddRange(await _connector.ValidateDataAsync(data, ct));

        // Custom tenant rules
        results.AddRange(await ValidateTenantRulesAsync(data, ct));

        var hasErrors = results.Any(r => r.Severity >= ValidationSeverity.Error);
        var hasWarnings = results.Any(r => r.Severity == ValidationSeverity.Warning);

        // Return a summary result
        return new ValidationResult
        {
            IsValid = !hasErrors,
            Code = hasErrors ? "ERP_VALIDATION_FAILED" : hasWarnings ? "ERP_VALIDATION_WARNING" : "ERP_VALIDATION_PASSED",
            Message = hasErrors ? "ERP-specific validation failed" : hasWarnings ? "ERP-specific validation passed with warnings" : "ERP-specific validation passed",
            Severity = hasErrors ? ValidationSeverity.Error : hasWarnings ? ValidationSeverity.Warning : ValidationSeverity.Warning
        };
    }

    public async Task<IEnumerable<ValidationResult>> ValidateCollectionAsync(
        IEnumerable<ErpData> items,
        CancellationToken ct = default)
    {
        var allResults = new List<ValidationResult>();

        foreach (var item in items)
        {
            ct.ThrowIfCancellationRequested();
            var result = await ValidateAsync(item, ct);
            allResults.Add(result);
        }

        return allResults;
    }

    private async Task<IEnumerable<ValidationResult>> ValidateErpBusinessRulesAsync(ErpData data, CancellationToken ct)
    {
        var results = new List<ValidationResult>();

        // Basic validation rules
        if (string.IsNullOrWhiteSpace(data.Id))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "MISSING_ID",
                Message = "ID is required",
                FieldPath = "Id",
                Severity = ValidationSeverity.Error
            });
        }

        if (data.Price.HasValue && data.Price.Value < 0)
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "INVALID_PRICE",
                Message = "Price cannot be negative",
                FieldPath = "Price",
                Severity = ValidationSeverity.Error
            });
        }

        // ERP-specific rules (e.g., SKU format for enventa Trade)
        if (data.ErpType == "enventa" && !string.IsNullOrEmpty(data.Sku))
        {
            if (!Regex.IsMatch(data.Sku, @"^[A-Z]{2}\d{6}$"))
            {
                results.Add(new ValidationResult
                {
                    IsValid = false,
                    Code = "INVALID_ENVENTA_SKU_FORMAT",
                    Message = "SKU must match enventa format: AA123456",
                    FieldPath = "Sku",
                    Severity = ValidationSeverity.Error
                });
            }
        }

        // Additional ERP rules...

        return results;
    }

    private async Task<IEnumerable<ValidationResult>> ValidateTenantRulesAsync(ErpData data, CancellationToken ct)
    {
        var rules = await _rulesProvider.GetErpSpecificRulesAsync(data.TenantId, data.ErpType, ct);
        var results = new List<ValidationResult>();

        foreach (var rule in rules)
        {
            var ruleResult = await rule.ValidateAsync(data, ct);
            if (!ruleResult.IsValid)
            {
                results.Add(ruleResult);
            }
        }

        return results;
    }
}

public interface IErpConnector
{
    Task<IEnumerable<ValidationResult>> ValidateDataAsync(ErpData data, CancellationToken ct = default);
    string ErpType { get; }
}
