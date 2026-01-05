// <copyright file="ErpDtos.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace B2Connect.Shared.Erp.Core.Models;

/// <summary>
/// Represents an order to be created in the ERP system.
/// </summary>
public class ErpOrder
{
    /// <summary>
    /// Gets or sets the external order reference.
    /// </summary>
    public string OrderReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer ID in the ERP.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order date.
    /// </summary>
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the order items.
    /// </summary>
    public List<ErpOrderItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the shipping address.
    /// </summary>
    public ErpAddress? ShippingAddress { get; set; }

    /// <summary>
    /// Gets or sets the billing address.
    /// </summary>
    public ErpAddress? BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets additional order notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets custom fields.
    /// </summary>
    public Dictionary<string, string> CustomFields { get; set; } = new();
}

/// <summary>
/// Represents an order line item.
/// </summary>
public class ErpOrderItem
{
    /// <summary>
    /// Gets or sets the item position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the article/SKU.
    /// </summary>
    public string ArticleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure.
    /// </summary>
    public string Unit { get; set; } = "PCE";

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage.
    /// </summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Represents an address.
/// </summary>
public class ErpAddress
{
    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;
}

/// <summary>
/// Represents customer data from the ERP system.
/// </summary>
public class ErpCustomerData
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer number.
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact name.
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the billing address.
    /// </summary>
    public ErpAddress? BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the shipping addresses.
    /// </summary>
    public List<ErpAddress> ShippingAddresses { get; set; } = new();

    /// <summary>
    /// Gets or sets the price group.
    /// </summary>
    public string? PriceGroup { get; set; }

    /// <summary>
    /// Gets or sets the credit limit.
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Gets or sets custom fields.
    /// </summary>
    public Dictionary<string, string> CustomFields { get; set; } = new();
}

/// <summary>
/// Context for catalog synchronization.
/// </summary>
public class SyncContext
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is a full sync.
    /// </summary>
    public bool IsFullSync { get; set; }

    /// <summary>
    /// Gets or sets the last sync timestamp.
    /// </summary>
    public DateTimeOffset? LastSyncTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the batch size.
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the sync scope.
    /// </summary>
    public SyncScope Scope { get; set; } = SyncScope.All;

    /// <summary>
    /// Gets or sets progress callback.
    /// </summary>
    public Action<SyncProgress>? OnProgress { get; set; }
}

/// <summary>
/// Scope of synchronization.
/// </summary>
[Flags]
public enum SyncScope
{
    /// <summary>
    /// No scope.
    /// </summary>
    None = 0,

    /// <summary>
    /// Articles/products.
    /// </summary>
    Articles = 1 << 0,

    /// <summary>
    /// Categories.
    /// </summary>
    Categories = 1 << 1,

    /// <summary>
    /// Pricing.
    /// </summary>
    Pricing = 1 << 2,

    /// <summary>
    /// Inventory/stock.
    /// </summary>
    Inventory = 1 << 3,

    /// <summary>
    /// All data.
    /// </summary>
    All = Articles | Categories | Pricing | Inventory
}

/// <summary>
/// Progress information for sync operations.
/// </summary>
public class SyncProgress
{
    /// <summary>
    /// Gets or sets the current item count.
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// Gets or sets the total item count.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the current phase.
    /// </summary>
    public string Phase { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the percentage complete.
    /// </summary>
    public double PercentComplete => Total > 0 ? (double)Current / Total * 100 : 0;
}

/// <summary>
/// Tenant context for ERP operations.
/// </summary>
public class TenantContext
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant name.
    /// </summary>
    public string TenantName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ERP configuration.
    /// </summary>
    public ErpConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// Gets or sets the correlation ID for tracing.
    /// </summary>
    public string? CorrelationId { get; set; }
}

/// <summary>
/// Metadata about an ERP connector.
/// </summary>
public class ErpConnectorMetadata
{
    /// <summary>
    /// Gets or sets the ERP type.
    /// </summary>
    public string ErpType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target framework.
    /// </summary>
    public string TargetFramework { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capabilities.
    /// </summary>
    public string[] Capabilities { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// Gets or sets whether this connector is available.
    /// </summary>
    public bool IsAvailable { get; set; } = true;
}
