// <copyright file="ErpModels.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Models;

/// <summary>
/// ERP Order for submission.
/// </summary>
public sealed record ErpOrder
{
    public required string Id { get; init; }
    public string? OrderNumber { get; init; }
    public required string CustomerId { get; init; }
    public required string CustomerNumber { get; init; }
    public DateTimeOffset OrderDate { get; init; }
    public DateTimeOffset? RequestedDeliveryDate { get; init; }
    public required ErpOrderStatus Status { get; init; }
    public string? Reference { get; init; }
    public string? Comments { get; init; }
    public required ErpOrderAddress BillingAddress { get; init; }
    public required ErpOrderAddress ShippingAddress { get; init; }
    public required IReadOnlyList<ErpOrderLine> Lines { get; init; }
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public required string Currency { get; init; }
    public string? PaymentMethod { get; init; }
    public string? PaymentTerms { get; init; }
    public string? ShippingMethod { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Order status in ERP.
/// </summary>
public enum ErpOrderStatus
{
    Draft,
    Submitted,
    Confirmed,
    Processing,
    PartiallyShipped,
    Shipped,
    Delivered,
    Completed,
    Cancelled,
    OnHold
}

/// <summary>
/// Order line item.
/// </summary>
public sealed record ErpOrderLine
{
    public required int LineNumber { get; init; }
    public required string ProductId { get; init; }
    public required string Sku { get; init; }
    public string? Name { get; init; }
    public required int Quantity { get; init; }
    public int? QuantityShipped { get; init; }
    public int? QuantityBackordered { get; init; }
    public required decimal UnitPrice { get; init; }
    public decimal? DiscountPercent { get; init; }
    public decimal? DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public required decimal TotalPrice { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset? ExpectedShipDate { get; init; }
}

/// <summary>
/// Order address.
/// </summary>
public sealed record ErpOrderAddress
{
    public string? Name { get; init; }
    public string? Company { get; init; }
    public string? Street1 { get; init; }
    public string? Street2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public required string CountryCode { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
}

/// <summary>
/// Order creation request.
/// </summary>
public sealed record OrderRequest
{
    public required string CustomerId { get; init; }
    public required string CustomerNumber { get; init; }
    public string? Reference { get; init; }
    public string? Comments { get; init; }
    public DateTimeOffset? RequestedDeliveryDate { get; init; }
    public required ErpOrderAddress BillingAddress { get; init; }
    public required ErpOrderAddress ShippingAddress { get; init; }
    public required IReadOnlyList<OrderRequestLine> Lines { get; init; }
    public string? PaymentMethod { get; init; }
    public string? ShippingMethod { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Order request line item.
/// </summary>
public sealed record OrderRequestLine
{
    public required string ProductId { get; init; }
    public required string Sku { get; init; }
    public required int Quantity { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Order submission result.
/// </summary>
public sealed record OrderResult
{
    public required string OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public required ErpOrderStatus Status { get; init; }
    public DateTimeOffset? EstimatedDeliveryDate { get; init; }
    public string? ConfirmationNumber { get; init; }
    public IReadOnlyList<OrderLineResult>? LineResults { get; init; }
    public IReadOnlyList<string>? Warnings { get; init; }
}

/// <summary>
/// Individual line result.
/// </summary>
public sealed record OrderLineResult
{
    public required int LineNumber { get; init; }
    public required string ProductId { get; init; }
    public bool IsAvailable { get; init; }
    public int? QuantityAvailable { get; init; }
    public int? QuantityBackordered { get; init; }
    public DateTimeOffset? ExpectedAvailableDate { get; init; }
    public decimal? ConfirmedPrice { get; init; }
}

/// <summary>
/// Document types supported by ERP.
/// </summary>
public enum ErpDocumentType
{
    Invoice,
    DeliveryNote,
    PackingSlip,
    OrderConfirmation,
    Quote,
    CreditNote,
    ProformaInvoice
}

/// <summary>
/// Document from ERP system.
/// </summary>
public sealed record ErpDocument
{
    public required string Id { get; init; }
    public required string DocumentNumber { get; init; }
    public required ErpDocumentType Type { get; init; }
    public required string CustomerId { get; init; }
    public string? OrderId { get; init; }
    public DateTimeOffset DocumentDate { get; init; }
    public decimal? TotalAmount { get; init; }
    public string? Currency { get; init; }
    public string? DownloadUrl { get; init; }
    public byte[]? Content { get; init; }
    public string? ContentType { get; init; }
    public string? FileName { get; init; }
}
