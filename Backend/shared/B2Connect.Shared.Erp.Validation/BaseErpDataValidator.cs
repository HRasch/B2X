// <copyright file="BaseErpDataValidator.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace B2Connect.Shared.Erp.Validation;

/// <summary>
/// Base implementation of ERP data validation with common validation logic.
/// </summary>
public abstract class BaseErpDataValidator : IErpDataValidator
{
    /// <inheritdoc />
    public abstract string ErpType { get; }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ErpValidationResult>> ValidateAsync(
        ErpDataToValidate data,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ErpValidationResult>();

        // Validate SKU
        if (!string.IsNullOrEmpty(data.Sku))
        {
            if (!IsValidSku(data.Sku!))
            {
                results.Add(ErpValidationResult.Error(
                    $"INVALID_{ErpType.ToUpperInvariant()}_SKU",
                    GetSkuValidationMessage(),
                    "Sku"));
            }
        }

        // Validate customer number
        if (!string.IsNullOrEmpty(data.CustomerNumber))
        {
            if (!IsValidCustomerNumber(data.CustomerNumber!))
            {
                results.Add(ErpValidationResult.Error(
                    $"INVALID_{ErpType.ToUpperInvariant()}_CUSTOMER",
                    GetCustomerNumberValidationMessage(),
                    "CustomerNumber"));
            }
        }

        // Allow derived classes to add additional validation
        var additionalResults = await ValidateAdditionalAsync(data, cancellationToken);
        results.AddRange(additionalResults);

        return results;
    }

    /// <inheritdoc />
    public abstract bool IsValidSku(string sku);

    /// <inheritdoc />
    public abstract bool IsValidCustomerNumber(string customerNumber);

    /// <summary>
    /// Gets the validation message for invalid SKU.
    /// </summary>
    /// <returns>The validation message.</returns>
    protected abstract string GetSkuValidationMessage();

    /// <summary>
    /// Gets the validation message for invalid customer number.
    /// </summary>
    /// <returns>The validation message.</returns>
    protected abstract string GetCustomerNumberValidationMessage();

    /// <summary>
    /// Override to add additional validation logic.
    /// </summary>
    /// <param name="data">The data to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Additional validation results.</returns>
    protected virtual Task<IEnumerable<ErpValidationResult>> ValidateAdditionalAsync(
        ErpDataToValidate data,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ErpValidationResult>>(new List<ErpValidationResult>());
    }

    /// <summary>
    /// Helper to validate string against a regex pattern.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="pattern">The regex pattern.</param>
    /// <returns>True if valid, otherwise false.</returns>
    protected static bool ValidatePattern(string value, string pattern)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return Regex.IsMatch(value, pattern);
    }
}
