using System;
using System.Collections.Generic;

namespace B2X.ErpConnector.Models
{
    #region Article DTOs

    public class ArticleDto
    {
        public string ArticleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ArticleState ArticleState { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool NoECommerce { get; set; }
        public DateTime ModifiedAt { get; set; }
    }

    public enum ArticleState
    {
        Active = 0,
        Inactive = 1,
        Discontinued = 2
    }

    #endregion

    #region Customer DTOs

    public class CustomerDto
    {
        public string CustomerNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    #endregion

    #region Order DTOs

    public class OrderDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateOrderRequest
    {
        public string CustomerNumber { get; set; } = string.Empty;
        public List<OrderItemRequest>? Items { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderItemRequest
    {
        public string ArticleId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public enum OrderStatus
    {
        Draft = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }

    #endregion

    #region Query Models

    public class QueryRequest
    {
        public List<QueryFilter>? Filters { get; set; }
        public List<SortField>? Sorting { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }

    public class QueryFilter
    {
        public string PropertyName { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; }
        public object? Value { get; set; }
    }

    public class SortField
    {
        public string PropertyName { get; set; } = string.Empty;
        public SortOrder Order { get; set; }
    }

    public enum FilterOperator
    {
        Equals = 0,
        NotEquals = 1,
        GreaterThan = 2,
        LessThan = 3,
        Contains = 4,
        In = 5,
        IsNull = 6,
        IsNotNull = 7
    }

    public enum SortOrder
    {
        Ascending = 0,
        Descending = 1
    }

    #endregion

    #region Response Models

    public class CursorPageResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
        public int TotalCount { get; set; }
    }

    public class DeltaSyncRequest
    {
        public DateTime? Since { get; set; }
        public string? Cursor { get; set; }
        public int? PageSize { get; set; }
        /// <summary>
        /// Optional category filter for delta sync.
        /// </summary>
        public string? Category { get; set; }
        /// <summary>
        /// Optional business unit filter for multi-business-unit tenants.
        /// </summary>
        public string? BusinessUnit { get; set; }
    }

    public class DeltaSyncResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
        public DateTime? LastModified { get; set; }
        public string? Watermark { get; set; }
        public int TotalCount { get; set; }
    }

    #endregion

    #region Health Check

    public class ConnectorHealthResponse
    {
        public string Status { get; set; } = "Healthy";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0.0";
        public Dictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    }

    #endregion
}
