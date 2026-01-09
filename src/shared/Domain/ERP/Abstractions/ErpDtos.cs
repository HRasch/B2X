using System.ComponentModel.DataAnnotations;

namespace B2X.ERP.Abstractions;

// Enums matching eGate patterns
public enum ArticleState
{
    Unknown = 0,
    Active = 1,
    Inactive = 2,
    Discontinued = 3,
    OutOfStock = 4
}

public enum ArticleType
{
    Unknown = 0,
    Standard = 1,
    Service = 2,
    Bundle = 3,
    Variant = 4
}

public enum OrderStatus
{
    Unknown = 0,
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}

public enum EntityType
{
    Unknown = 0,
    Article = 1,
    Customer = 2,
    Order = 3
}

// DTOs matching eGate NVShop.Data.NV.Model structures
public class ArticleDto
{
    // Core identifiers (eGate: ArticleId)
    [Required]
    public int ArticleId { get; set; }

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }
    public DateTime? LastSyncUtc { get; set; }

    // Basic info
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? ArticleNumber { get; set; }
    public ArticleState ArticleState { get; set; }
    public ArticleType ArticleType { get; set; }
    public string? Description { get; set; }
    public string? MatchCode { get; set; }

    // Media
    public string? DefaultImageUrl { get; set; }

    // Identifiers
    public string? Ean { get; set; }
    public string? Gtin { get; set; }

    // E-commerce flags (eGate)
    public bool NoECommerce { get; set; }
    public bool NoShoppingCart { get; set; }
    public bool NoShipping { get; set; }
    public bool NoPickup { get; set; }

    // Grouping
    public string? ArticleGroupId { get; set; }
    public string? DiscountGroupId { get; set; }
    public string? Category { get; set; }

    // Units & Pricing
    public string? SalesUnit { get; set; }
    public decimal? Price { get; set; }
    public decimal? BasePrice { get; set; }
    public string? Currency { get; set; }

    // Stock
    public int? StockQuantity { get; set; }

    // Attributes (extensible)
    public Dictionary<string, object>? Attributes { get; set; }

    // Timestamps
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class CustomerDto
{
    // Core identifiers (eGate: CustomerNumber)
    [Required]
    public string CustomerNumber { get; set; } = string.Empty;

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }
    public DateTime? LastSyncUtc { get; set; }

    // Company info
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }

    // Name fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Contact
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }

    // Address
    public string? Address { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }

    // Tax
    public string? VatNumber { get; set; }

    // Status
    public bool IsActive { get; set; } = true;

    // Financial
    public decimal? CreditLimit { get; set; }
    public decimal? AccountBalance { get; set; }
    public int? PaymentTermsDays { get; set; }

    // Grouping
    public string? CustomerGroup { get; set; }
    public string? SalesRepId { get; set; }

    // Timestamps
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class OrderDto
{
    // Core identifiers (eGate: OrderNumber)
    [Required]
    public string OrderNumber { get; set; } = string.Empty;

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }
    public DateTime? LastSyncUtc { get; set; }

    // Customer reference
    [Required]
    public string CustomerNumber { get; set; } = string.Empty;
    public string? CustomerId { get; set; }

    // Order details
    [Required]
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; }

    // Addresses
    public AddressDto? ShippingAddress { get; set; }
    public AddressDto? BillingAddress { get; set; }

    // Line items
    public List<OrderItemDto> Items { get; set; } = new();

    // Delivery
    public DateTime? RequestedDeliveryDate { get; set; }

    // References
    public string? Reference { get; set; }

    // Payment & Shipping
    public string? PaymentMethod { get; set; }
    public string? ShippingMethod { get; set; }

    // Timestamps
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class OrderItemDto
{
    public int LineNumber { get; set; }
    public string? ArticleId { get; set; }
    public string? ArticleNumber { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal LineTotal { get; set; }
}

public class AddressDto
{
    public string? CompanyName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

// Query structures
public class QueryRequest
{
    public string? TenantId { get; set; }
    public List<QueryFilter> Filters { get; set; } = new();
    public List<SortField> Sorting { get; set; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public bool IncludeAll { get; set; }
    public bool IncludeAttributes { get; set; }
    public List<string>? IncludeFields { get; set; }
    public List<string>? Ids { get; set; }
}

public class QueryFilter
{
    [Required]
    public string PropertyName { get; set; } = string.Empty;
    [Required]
    public FilterOperator Operator { get; set; }
    public object? Value { get; set; }
}

public class SortField
{
    [Required]
    public string PropertyName { get; set; } = string.Empty;
    [Required]
    public SortOrder Order { get; set; }
}

public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    IsNull,
    IsNotNull
}

public enum SortOrder
{
    Ascending,
    Descending
}

// Sync structures
public class SyncRequest
{
    public string? TenantId { get; set; }
    public EntityType EntityType { get; set; }
    public DateTime? Since { get; set; }
    public int? BatchSize { get; set; }
    public string? ContinuationToken { get; set; }
}

// Non-generic SyncResult for backward compatibility
public class SyncResult
{
    public EntityType EntityType { get; set; }
    public bool Success { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int CreatedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public object? Items { get; set; }
    public string? ContinuationToken { get; set; }
    public bool HasMore { get; set; }
    public DateTime SyncTimestamp { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class SyncResult<T>
{
    public EntityType EntityType { get; set; }
    public bool Success { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int CreatedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<T> Items { get; set; } = new();
    public string? ContinuationToken { get; set; }
    public bool HasMore { get; set; }
    public DateTime SyncTimestamp { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> Errors { get; set; } = new();
}

// Batch operations
// Non-generic BatchRequest for backward compatibility
public class BatchRequest
{
    public string? TenantId { get; set; }
    public List<string> Ids { get; set; } = new();
    public object? Items { get; set; }
    public bool ContinueOnError { get; set; }
    public QueryRequest? Filter { get; set; }
}

public class BatchRequest<T>
{
    public string? TenantId { get; set; }
    public List<string> Ids { get; set; } = new();
    public List<T> Items { get; set; } = new();
    public bool ContinueOnError { get; set; }
    public QueryRequest? Filter { get; set; }
}

public class BatchResult<T>
{
    public List<T> Successful { get; set; } = new();
    public List<BatchError> Errors { get; set; } = new();
}

public class BatchError
{
    public int Index { get; set; }
    public string Error { get; set; } = string.Empty;
}

// Get by ID request
public class GetByIdRequest
{
    public string? TenantId { get; set; }
    [Required]
    public object Id { get; set; } = default!;
}
