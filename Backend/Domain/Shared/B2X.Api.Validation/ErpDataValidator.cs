using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using B2X.Api.Models.Erp;
using Microsoft.Extensions.Logging;

namespace B2X.Api.Validation;

public class ErpDataValidator : IDataValidator<ErpData>
{
    private readonly ILogger<ErpDataValidator> _logger;
    private readonly IValidationRulesProvider _rulesProvider;

    public ErpDataValidator(
        ILogger<ErpDataValidator> logger,
        IValidationRulesProvider rulesProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rulesProvider = rulesProvider ?? throw new ArgumentNullException(nameof(rulesProvider));
    }

    public async Task<ValidationResult> ValidateAsync(ErpData data, CancellationToken ct = default)
    {
        var results = new List<ValidationResult>();

        // Business rules validation
        results.AddRange(await ValidateBusinessRulesAsync(data, ct));

        // Format validation
        results.AddRange(ValidateFormatRules(data));

        // Data integrity validation
        results.AddRange(await ValidateDataIntegrityAsync(data, ct));

        var hasErrors = results.Any(r => r.Severity >= ValidationSeverity.Error);
        var hasWarnings = results.Any(r => r.Severity == ValidationSeverity.Warning);

        return new ValidationResult
        {
            IsValid = !hasErrors,
            Code = hasErrors ? "VALIDATION_FAILED" : hasWarnings ? "VALIDATION_WARNINGS" : "VALIDATION_PASSED",
            Message = hasErrors ? "Data validation failed" : hasWarnings ? "Data validation passed with warnings" : "Data validation passed",
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

    private async Task<IEnumerable<ValidationResult>> ValidateBusinessRulesAsync(ErpData data, CancellationToken ct)
    {
        var results = new List<ValidationResult>();
        var rules = await _rulesProvider.GetRulesForTenantAsync(data.TenantId, ct);

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

    private List<ValidationResult> ValidateFormatRules(ErpData data)
    {
        var results = new List<ValidationResult>();

        // Email validation
        if (!string.IsNullOrEmpty(data.Email) && !IsValidEmail(data.Email))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "INVALID_EMAIL_FORMAT",
                Message = "Email format is invalid",
                FieldPath = "Email",
                Severity = ValidationSeverity.Error
            });
        }

        // SKU uniqueness check (simplified - would check DB)
        if (string.IsNullOrEmpty(data.Sku))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "MISSING_SKU",
                Message = "SKU is required",
                FieldPath = "Sku",
                Severity = ValidationSeverity.Error
            });
        }

        return results;
    }

    private async Task<IEnumerable<ValidationResult>> ValidateDataIntegrityAsync(ErpData data, CancellationToken ct)
    {
        var results = new List<ValidationResult>();

        // Check FK references (simplified - would query DB)
        if (!await ReferenceExistsAsync(data.CategoryId, "categories", ct))
        {
            results.Add(new ValidationResult
            {
                IsValid = false,
                Code = "INVALID_CATEGORY_REFERENCE",
                Message = "Category reference does not exist",
                FieldPath = "CategoryId",
                Severity = ValidationSeverity.Error
            });
        }

        return results;
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        return emailRegex.IsMatch(email);
    }

    private async Task<bool> ReferenceExistsAsync(string referenceId, string table, CancellationToken ct)
    {
        // Placeholder - implement actual DB check
        await Task.Delay(1, ct); // Simulate async DB call
        return !string.IsNullOrEmpty(referenceId);
    }
}

public interface IValidationRulesProvider
{
    Task<IEnumerable<IValidationRule<ErpData>>> GetRulesForTenantAsync(string tenantId, CancellationToken ct = default);
    Task<IEnumerable<IValidationRule<ErpData>>> GetErpSpecificRulesAsync(
        string tenantId,
        string erpType,
        CancellationToken ct = default);
}

public interface IValidationRule<T>
{
    Task<ValidationResult> ValidateAsync(T data, CancellationToken ct = default);
}
