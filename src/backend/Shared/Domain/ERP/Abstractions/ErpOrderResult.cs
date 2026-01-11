// <copyright file="ErpOrderResult.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents the result of creating an order in the ERP system.
/// </summary>
public class ErpOrderResult
{
    /// <summary>
    /// Gets or sets whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the ERP order ID.
    /// </summary>
    public string ErpOrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional data from the ERP system.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Gets or sets any error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
