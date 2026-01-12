// <copyright file="ErpOrder.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents an order to be created in the ERP system.
/// </summary>
public class ErpOrder
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order lines.
    /// </summary>
    public List<ErpOrderLine> Lines { get; set; } = new();
}

/// <summary>
/// Represents an order line item.
/// </summary>
public class ErpOrderLine
{
    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
