using System;
using System.Collections.Generic;

namespace B2Connect.Shared.ErpConnector.Models;

/// <summary>
/// Article data transfer object.
/// </summary>
public class ArticleDto
{
    /// <summary>
    /// Unique article identifier.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;

    /// <summary>
    /// Article name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Article description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Article state.
    /// </summary>
    public ArticleState State { get; set; }

    /// <summary>
    /// Article price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Stock quantity.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Unit of measure.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Whether article is excluded from e-commerce.
    /// </summary>
    public bool NoECommerce { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Custom fields specific to ERP system.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Article states.
/// </summary>
public enum ArticleState
{
    /// <summary>
    /// Article is active and available.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Article is inactive.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Article is discontinued.
    /// </summary>
    Discontinued = 2
}

/// <summary>
/// Customer data transfer object.
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Unique customer number.
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Contact person name.
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Street address.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// City.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Whether customer is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Custom fields specific to ERP system.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Order data transfer object.
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Unique order number.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer number.
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Order date.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Order status.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Total order amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Order items.
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Custom fields specific to ERP system.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Order item data transfer object.
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Article ID.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;

    /// <summary>
    /// Quantity ordered.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Line total.
    /// </summary>
    public decimal LineTotal { get; set; }

    /// <summary>
    /// Custom fields specific to ERP system.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Order statuses.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is draft.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Order is confirmed.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Order is in progress.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Order is shipped.
    /// </summary>
    Shipped = 3,

    /// <summary>
    /// Order is delivered.
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Order is cancelled.
    /// </summary>
    Cancelled = 5
}