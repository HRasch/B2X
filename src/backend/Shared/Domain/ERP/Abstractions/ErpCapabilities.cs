// <copyright file="ErpCapabilities.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents the capabilities of an ERP connector.
/// </summary>
public class ErpCapabilities
{
    /// <summary>
    /// Gets or sets the catalog capabilities.
    /// </summary>
    public ErpCapability Catalog { get; set; } = new();

    /// <summary>
    /// Gets or sets the order capabilities.
    /// </summary>
    public ErpCapability Order { get; set; } = new();

    /// <summary>
    /// Gets or sets the customer capabilities.
    /// </summary>
    public ErpCapability Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets the inventory capabilities.
    /// </summary>
    public ErpCapability Inventory { get; set; } = new();

    /// <summary>
    /// Gets or sets the real-time capabilities.
    /// </summary>
    public ErpCapability RealTime { get; set; } = new();

    /// <summary>
    /// Gets or sets the batch processing capabilities.
    /// </summary>
    public ErpBatchCapabilities Batch { get; set; } = new();

    /// <summary>
    /// Gets or sets the supported authentication types.
    /// </summary>
    public List<string> SupportedAuthTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the supported data types.
    /// </summary>
    public List<string> SupportedDataTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets custom capabilities.
    /// </summary>
    public Dictionary<string, object> CustomCapabilities { get; set; } = new();
}

/// <summary>
/// Represents a specific ERP capability.
/// </summary>
public class ErpCapability
{
    /// <summary>
    /// Gets or sets whether the capability is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Gets or sets whether full synchronization is supported.
    /// </summary>
    public bool SupportsFullSync { get; set; }

    /// <summary>
    /// Gets or sets whether delta synchronization is supported.
    /// </summary>
    public bool SupportsDeltaSync { get; set; }

    /// <summary>
    /// Gets or sets whether real-time updates are supported.
    /// </summary>
    public bool SupportsRealTimeUpdates { get; set; }

    /// <summary>
    /// Gets or sets the supported entity types.
    /// </summary>
    public List<string> SupportedEntityTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets whether status updates are supported.
    /// </summary>
    public bool SupportsStatusUpdates { get; set; }

    /// <summary>
    /// Gets or sets whether cancellation is supported.
    /// </summary>
    public bool SupportsCancellation { get; set; }

    /// <summary>
    /// Gets or sets whether returns are supported.
    /// </summary>
    public bool SupportsReturns { get; set; }

    /// <summary>
    /// Gets or sets whether partial orders are supported.
    /// </summary>
    public bool SupportsPartialOrders { get; set; }

    /// <summary>
    /// Gets or sets whether creation is supported.
    /// </summary>
    public bool SupportsCreation { get; set; }

    /// <summary>
    /// Gets or sets whether updates are supported.
    /// </summary>
    public bool SupportsUpdates { get; set; }

    /// <summary>
    /// Gets or sets whether address management is supported.
    /// </summary>
    public bool SupportsAddressManagement { get; set; }

    /// <summary>
    /// Gets or sets whether credit limit checks are supported.
    /// </summary>
    public bool SupportsCreditLimitChecks { get; set; }

    /// <summary>
    /// Gets or sets whether reservations are supported.
    /// </summary>
    public bool SupportsReservations { get; set; }

    /// <summary>
    /// Gets or sets whether multi-location is supported.
    /// </summary>
    public bool SupportsMultiLocation { get; set; }

    /// <summary>
    /// Gets or sets whether low stock alerts are supported.
    /// </summary>
    public bool SupportsLowStockAlerts { get; set; }

    /// <summary>
    /// Gets or sets the supported event types.
    /// </summary>
    public List<string> SupportedEventTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets whether webhooks are supported.
    /// </summary>
    public bool SupportsWebhooks { get; set; }

    /// <summary>
    /// Gets or sets whether polling is supported.
    /// </summary>
    public bool SupportsPolling { get; set; }
}

/// <summary>
/// Represents batch processing capabilities.
/// </summary>
public class ErpBatchCapabilities
{
    /// <summary>
    /// Gets or sets whether batch processing is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Gets or sets the maximum batch size.
    /// </summary>
    public int MaxBatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether bulk import is supported.
    /// </summary>
    public bool SupportsBulkImport { get; set; }

    /// <summary>
    /// Gets or sets whether bulk export is supported.
    /// </summary>
    public bool SupportsBulkExport { get; set; }
}