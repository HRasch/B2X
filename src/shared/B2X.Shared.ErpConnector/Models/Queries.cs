using System;
using System.Collections.Generic;

namespace B2X.Shared.ErpConnector.Models;

/// <summary>
/// Query parameters for article retrieval.
/// </summary>
public class ArticleQuery
{
    /// <summary>
    /// Filter by article IDs.
    /// </summary>
    public IEnumerable<string>? ArticleIds { get; set; }

    /// <summary>
    /// Filter by article states.
    /// </summary>
    public IEnumerable<ArticleState>? States { get; set; }

    /// <summary>
    /// Search text for article name or description.
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Modified after this date.
    /// </summary>
    public DateTime? ModifiedAfter { get; set; }

    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Number of results to skip.
    /// </summary>
    public int? Offset { get; set; }
}

/// <summary>
/// Query parameters for customer retrieval.
/// </summary>
public class CustomerQuery
{
    /// <summary>
    /// Filter by customer numbers.
    /// </summary>
    public IEnumerable<string>? CustomerNumbers { get; set; }

    /// <summary>
    /// Search text for company name or contact.
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Number of results to skip.
    /// </summary>
    public int? Offset { get; set; }
}

/// <summary>
/// Query parameters for order retrieval.
/// </summary>
public class OrderQuery
{
    /// <summary>
    /// Filter by order numbers.
    /// </summary>
    public IEnumerable<string>? OrderNumbers { get; set; }

    /// <summary>
    /// Filter by customer numbers.
    /// </summary>
    public IEnumerable<string>? CustomerNumbers { get; set; }

    /// <summary>
    /// Filter by order status.
    /// </summary>
    public IEnumerable<OrderStatus>? Statuses { get; set; }

    /// <summary>
    /// Orders created after this date.
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Orders created before this date.
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Maximum number of results to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Number of results to skip.
    /// </summary>
    public int? Offset { get; set; }
}
