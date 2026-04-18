// <copyright file="ErpCapabilities.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;

namespace B2X.Shared.Erp.Core.Models;

/// <summary>
/// Describes the capabilities supported by an ERP connector.
/// </summary>
public class ErpCapabilities
{
    /// <summary>
    /// Gets or sets whether catalog synchronization is supported.
    /// </summary>
    public bool SupportsCatalogSync { get; set; }

    /// <summary>
    /// Gets or sets whether order creation is supported.
    /// </summary>
    public bool SupportsOrderCreation { get; set; }

    /// <summary>
    /// Gets or sets whether real-time queries are supported.
    /// </summary>
    public bool SupportsRealTimeQueries { get; set; }

    /// <summary>
    /// Gets or sets whether customer data retrieval is supported.
    /// </summary>
    public bool SupportsCustomerData { get; set; }

    /// <summary>
    /// Gets or sets whether connection pooling is required.
    /// </summary>
    public bool RequiresConnectionPooling { get; set; }

    /// <summary>
    /// Gets or sets whether the connector is single-threaded only.
    /// </summary>
    public bool IsSingleThreaded { get; set; }

    /// <summary>
    /// Gets or sets whether batch operations are supported.
    /// </summary>
    public bool SupportsBatchOperations { get; set; }

    /// <summary>
    /// Gets or sets the supported data formats.
    /// </summary>
    public string[] SupportedDataFormats { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the maximum batch size.
    /// </summary>
    public int MaxBatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum concurrent connections.
    /// </summary>
    public int MaxConcurrentConnections { get; set; } = 1;
}

/// <summary>
/// Flags representing ERP features.
/// </summary>
[Flags]
public enum ErpFeatures
{
    /// <summary>
    /// No features supported.
    /// </summary>
    None = 0,

    /// <summary>
    /// Article/product management.
    /// </summary>
    Articles = 1 << 0,

    /// <summary>
    /// Customer management.
    /// </summary>
    Customers = 1 << 1,

    /// <summary>
    /// Order management.
    /// </summary>
    Orders = 1 << 2,

    /// <summary>
    /// Real-time synchronization.
    /// </summary>
    RealTimeSync = 1 << 3,

    /// <summary>
    /// Batch synchronization.
    /// </summary>
    BatchSync = 1 << 4,

    /// <summary>
    /// Custom field support.
    /// </summary>
    CustomFields = 1 << 5,

    /// <summary>
    /// Multi-company support.
    /// </summary>
    MultiCompany = 1 << 6,

    /// <summary>
    /// Pricing support.
    /// </summary>
    Pricing = 1 << 7,

    /// <summary>
    /// Inventory management.
    /// </summary>
    Inventory = 1 << 8,

    /// <summary>
    /// Punchout catalog support.
    /// </summary>
    Punchout = 1 << 9,

    /// <summary>
    /// All features.
    /// </summary>
    All = Articles | Customers | Orders | RealTimeSync | BatchSync | CustomFields | MultiCompany | Pricing | Inventory | Punchout
}
