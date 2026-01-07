// <copyright file="IErpDataValidator.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.Shared.Erp.Validation;

/// <summary>
/// Interface for ERP-specific data validation.
/// </summary>
public interface IErpDataValidator
{
    /// <summary>
    /// Gets the ERP type this validator handles.
    /// </summary>
    string ErpType { get; }

    /// <summary>
    /// Validates ERP data.
    /// </summary>
    /// <param name="data">The data to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of validation results.</returns>
    Task<IEnumerable<ErpValidationResult>> ValidateAsync(
        ErpDataToValidate data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a SKU/article number format.
    /// </summary>
    /// <param name="sku">The SKU to validate.</param>
    /// <returns>True if valid, otherwise false.</returns>
    bool IsValidSku(string sku);

    /// <summary>
    /// Validates a customer number format.
    /// </summary>
    /// <param name="customerNumber">The customer number to validate.</param>
    /// <returns>True if valid, otherwise false.</returns>
    bool IsValidCustomerNumber(string customerNumber);
}

/// <summary>
/// Data to be validated.
/// </summary>
public class ErpDataToValidate
{
    /// <summary>
    /// Gets or sets the SKU/article number.
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Gets or sets the customer number.
    /// </summary>
    public string? CustomerNumber { get; set; }

    /// <summary>
    /// Gets or sets the order reference.
    /// </summary>
    public string? OrderReference { get; set; }

    /// <summary>
    /// Gets or sets additional data fields to validate.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Result of an ERP validation operation.
/// </summary>
public class ErpValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the validation code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field that failed validation.
    /// </summary>
    public string? FieldPath { get; set; }

    /// <summary>
    /// Gets or sets the severity of the validation issue.
    /// </summary>
    public ErpValidationSeverity Severity { get; set; } = ErpValidationSeverity.Error;

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A valid result.</returns>
    public static ErpValidationResult Valid() => new() { IsValid = true };

    /// <summary>
    /// Creates an error validation result.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="fieldPath">The field path.</param>
    /// <returns>An error result.</returns>
    public static ErpValidationResult Error(string code, string message, string? fieldPath = null)
        => new()
        {
            IsValid = false,
            Code = code,
            Message = message,
            FieldPath = fieldPath,
            Severity = ErpValidationSeverity.Error
        };

    /// <summary>
    /// Creates a warning validation result.
    /// </summary>
    /// <param name="code">The warning code.</param>
    /// <param name="message">The warning message.</param>
    /// <param name="fieldPath">The field path.</param>
    /// <returns>A warning result.</returns>
    public static ErpValidationResult Warning(string code, string message, string? fieldPath = null)
        => new()
        {
            IsValid = true,
            Code = code,
            Message = message,
            FieldPath = fieldPath,
            Severity = ErpValidationSeverity.Warning
        };
}

/// <summary>
/// Severity level for validation issues.
/// </summary>
public enum ErpValidationSeverity
{
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Warning - operation can continue.
    /// </summary>
    Warning,

    /// <summary>
    /// Error - operation should not continue.
    /// </summary>
    Error
}
