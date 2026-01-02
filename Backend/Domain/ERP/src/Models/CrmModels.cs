// <copyright file="CrmModels.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2Connect.ERP.Models;

/// <summary>
/// Customer information from CRM system.
/// </summary>
public sealed record CrmCustomer
{
    public required string Id { get; init; }
    public required string CustomerNumber { get; init; }
    public required string Name { get; init; }
    public string? CompanyName { get; init; }
    public string? TaxId { get; init; }
    public string? VatId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public string? Website { get; init; }
    public CrmCustomerType CustomerType { get; init; }
    public string? CustomerGroupId { get; init; }
    public string? CustomerGroupName { get; init; }
    public string? PriceGroupId { get; init; }
    public string? SalesRepId { get; init; }
    public string? PaymentTerms { get; init; }
    public decimal? CreditLimit { get; init; }
    public string? Currency { get; init; }
    public string? Language { get; init; }
    public bool IsActive { get; init; }
    public bool IsBlocked { get; init; }
    public string? BlockedReason { get; init; }
    public CrmAddress? BillingAddress { get; init; }
    public CrmAddress? ShippingAddress { get; init; }
    public IReadOnlyList<CrmAddress>? Addresses { get; init; }
    public IReadOnlyList<CrmContact>? Contacts { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Customer type classification.
/// </summary>
public enum CrmCustomerType
{
    Business,
    Individual,
    Government,
    NonProfit,
    Reseller,
    Distributor
}

/// <summary>
/// Contact person.
/// </summary>
public sealed record CrmContact
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string FullName => $"{FirstName} {LastName}";
    public string? Salutation { get; init; }
    public string? Title { get; init; }
    public string? Position { get; init; }
    public string? Department { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Mobile { get; init; }
    public string? Fax { get; init; }
    public bool IsPrimary { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<string>? Roles { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
}

/// <summary>
/// Customer address.
/// </summary>
public sealed record CrmAddress
{
    public required string Id { get; init; }
    public CrmAddressType Type { get; init; }
    public string? Name { get; init; }
    public string? Company { get; init; }
    public string? Street1 { get; init; }
    public string? Street2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public required string CountryCode { get; init; }
    public string? CountryName { get; init; }
    public string? Phone { get; init; }
    public bool IsDefault { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Address type.
/// </summary>
public enum CrmAddressType
{
    Billing,
    Shipping,
    Both,
    Other
}

/// <summary>
/// Customer activity/interaction.
/// </summary>
public sealed record CrmActivity
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public string? ContactId { get; init; }
    public required CrmActivityType Type { get; init; }
    public required string Subject { get; init; }
    public string? Description { get; init; }
    public CrmActivityStatus Status { get; init; }
    public CrmActivityPriority Priority { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? AssignedToId { get; init; }
    public string? AssignedToName { get; init; }
    public string? RelatedOrderId { get; init; }
    public string? RelatedQuoteId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
}

/// <summary>
/// Activity type.
/// </summary>
public enum CrmActivityType
{
    Call,
    Email,
    Meeting,
    Task,
    Note,
    Quote,
    Order,
    Support,
    Other
}

/// <summary>
/// Activity status.
/// </summary>
public enum CrmActivityStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled,
    Deferred
}

/// <summary>
/// Activity priority.
/// </summary>
public enum CrmActivityPriority
{
    Low,
    Normal,
    High,
    Urgent
}

/// <summary>
/// Customer-specific pricing.
/// </summary>
public sealed record CrmPricing
{
    public required string CustomerId { get; init; }
    public required string ProductId { get; init; }
    public required decimal Price { get; init; }
    public required string Currency { get; init; }
    public decimal? DiscountPercent { get; init; }
    public int? MinQuantity { get; init; }
    public DateTimeOffset? ValidFrom { get; init; }
    public DateTimeOffset? ValidUntil { get; init; }
    public string? PriceListId { get; init; }
    public string? PriceListName { get; init; }
}

/// <summary>
/// Customer order history entry.
/// </summary>
public sealed record CrmOrderHistory
{
    public required string OrderId { get; init; }
    public required string OrderNumber { get; init; }
    public required string CustomerId { get; init; }
    public DateTimeOffset OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public required string Currency { get; init; }
    public string? Status { get; init; }
    public int ItemCount { get; init; }
    public IReadOnlyList<CrmOrderHistoryItem>? Items { get; init; }
}

/// <summary>
/// Order history line item.
/// </summary>
public sealed record CrmOrderHistoryItem
{
    public required string ProductId { get; init; }
    public required string Sku { get; init; }
    public string? Name { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
}
