// NVShop abstraction types for enventa ERP integration
// These types define the contracts for HTTP communication between B2X (.NET 10)
// and the ERP Connector (.NET Framework 4.8)
//
// Architecture:
// - B2X.ERP.Abstractions (.NET 10): DTOs and interfaces for HTTP communication
// - B2X.ERP (.NET 10): Receives data via HTTP/REST from the connector
// - ERP Connector (.NET 4.8): Separate project with FSGlobalPool, FSUtil, and actual enventa DAL

namespace NVShop.Data.NV.Model;

/// <summary>
/// Abstraction for enventa database context.
/// The actual implementation (FSGlobalPool, FSUtil) lives in the .NET 4.8 ERP Connector.
/// </summary>
public interface INVContext : IDisposable
{
    IQueryable<NVArticle> Articles { get; }
    IQueryable<NVCustomer> Customers { get; }
    IQueryable<NVOrder> Orders { get; }
    IQueryable<NVAddress> Addresses { get; }
}

/// <summary>
/// Article entity from enventa ERP.
/// </summary>
public class NVArticle
{
    // Core identifiers
    public int ArticleId { get; set; }

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }

    // Basic info
    public string Name { get; set; } = string.Empty;
    public string? ArticleNumber { get; set; }
    public string ArticleState { get; set; } = string.Empty;
    public string? ArticleType { get; set; }
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

/// <summary>
/// Customer entity from enventa ERP.
/// </summary>
public class NVCustomer
{
    // Core identifiers
    public string CustomerNumber { get; set; } = string.Empty;

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }

    // Company info
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

/// <summary>
/// Order entity from enventa ERP.
/// </summary>
public class NVOrder
{
    // Core identifiers
    public string OrderNumber { get; set; } = string.Empty;

    // Sync tracking
    public int? RowId { get; set; }
    public byte[]? RowVersion { get; set; }

    // Customer reference
    public string CustomerNumber { get; set; } = string.Empty;
    public string? CustomerId { get; set; }

    // Order details
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; }

    // Addresses
    public NVAddress? ShippingAddress { get; set; }
    public NVAddress? BillingAddress { get; set; }

    // Line items
    public List<NVOrderItem> Items { get; set; } = [];

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

/// <summary>
/// Order line item from enventa ERP.
/// </summary>
public class NVOrderItem
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

/// <summary>
/// Address entity from enventa ERP.
/// </summary>
public class NVAddress
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

/// <summary>
/// eGate IcECArticle interface for e-commerce article operations.
/// </summary>
#pragma warning disable CA1715 // Identifiers should have correct prefix
public interface IcECArticle
#pragma warning restore CA1715 // Identifiers should have correct prefix
{
    int ArticleId { get; }
    string Name { get; }
    string ArticleState { get; }
    bool NoECommerce { get; }
    Dictionary<string, object>? Attributes { get; }
}
