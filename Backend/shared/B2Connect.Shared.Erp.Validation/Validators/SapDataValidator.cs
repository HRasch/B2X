// <copyright file="SapDataValidator.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2Connect.Shared.Erp.Validation.Validators;

/// <summary>
/// Validator for SAP ERP data formats.
/// </summary>
public class SapDataValidator : BaseErpDataValidator
{
    private const string SapMaterialPattern = @"^\d{18}$";
    private const string SapCustomerPattern = @"^\d{10}$";

    /// <inheritdoc />
    public override string ErpType => "sap";

    /// <inheritdoc />
    public override bool IsValidSku(string sku)
        => ValidatePattern(sku, SapMaterialPattern);

    /// <inheritdoc />
    public override bool IsValidCustomerNumber(string customerNumber)
        => ValidatePattern(customerNumber, SapCustomerPattern);

    /// <inheritdoc />
    protected override string GetSkuValidationMessage()
        => "Material number must be 18 digits";

    /// <inheritdoc />
    protected override string GetCustomerNumberValidationMessage()
        => "Customer number must be 10 digits";
}
