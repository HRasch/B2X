// <copyright file="EnventaDataValidator.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.Shared.Erp.Validation.Validators;

/// <summary>
/// Validator for enventa Trade ERP data formats.
/// </summary>
public class EnventaDataValidator : BaseErpDataValidator
{
    private const string EnventaSkuPattern = @"^[A-Z]{2}\d{6}$";
    private const string EnventaCustomerPattern = @"^\d{5,10}$";

    /// <inheritdoc />
    public override string ErpType => "enventa";

    /// <inheritdoc />
    public override bool IsValidSku(string sku)
        => ValidatePattern(sku, EnventaSkuPattern);

    /// <inheritdoc />
    public override bool IsValidCustomerNumber(string customerNumber)
        => ValidatePattern(customerNumber, EnventaCustomerPattern);

    /// <inheritdoc />
    protected override string GetSkuValidationMessage()
        => "SKU must match enventa format: AA123456 (2 uppercase letters followed by 6 digits)";

    /// <inheritdoc />
    protected override string GetCustomerNumberValidationMessage()
        => "Customer number must be 5-10 digits";
}
