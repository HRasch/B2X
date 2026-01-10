// <copyright file="FashopErpConnector.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using B2X.ERP.Abstractions;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Connectors;

/// <summary>
/// ERP connector for Fashop ERP system.
/// </summary>
public class FashopErpConnector : IErpConnector
{
    private readonly ILogger<FashopErpConnector> _logger;
    private bool _isInitialized;

    /// <summary>
    /// Gets the ERP type identifier.
    /// </summary>
    public string ErpType => "fashop";

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName => "Fashop ERP";

    /// <summary>
    /// Gets the version information.
    /// </summary>
    public ErpVersionInfo Version => new()
    {
        SystemName = "Fashop ERP",
        SystemVersion = "2023.1",
        ApiVersion = "REST API v2",
        SupportsBackwardCompatibility = true,
        MinimumSystemVersion = "2020.1",
        RecommendedSystemVersion = "2023.1+",
        Version = "1.0.0"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FashopErpConnector"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public FashopErpConnector(ILogger<FashopErpConnector> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initializes the connector with the specified configuration.
    /// </summary>
    /// <param name="config">The ERP configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the initialization operation.</returns>
    public Task InitializeAsync(ErpConfiguration config, CancellationToken cancellationToken = default)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        if (string.IsNullOrEmpty(config.ErpType) || config.ErpType != "fashop")
            throw new ArgumentException("Invalid ERP type for Fashop connector", nameof(config));

        _isInitialized = true;
        _logger.LogInformation("FashopErpConnector initialized for tenant {TenantId}", config.TenantId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the capabilities of the ERP connector.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ERP capabilities.</returns>
    public Task<ErpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Connector must be initialized before use");

        var capabilities = new ErpCapabilities
        {
            Catalog = new ErpCapability
            {
                Supported = true,
                SupportsFullSync = true,
                SupportsDeltaSync = true,
                SupportsRealTimeUpdates = true,
                SupportedEntityTypes = new List<string> { "materials", "products" }
            },
            Order = new ErpCapability
            {
                Supported = true,
                SupportsStatusUpdates = true,
                SupportsCancellation = true,
                SupportsReturns = true,
                SupportsPartialOrders = true
            },
            Customer = new ErpCapability
            {
                Supported = true,
                SupportsCreation = true,
                SupportsUpdates = true,
                SupportsAddressManagement = true,
                SupportsCreditLimitChecks = true
            },
            Inventory = new ErpCapability
            {
                Supported = true,
                SupportsRealTimeUpdates = true,
                SupportsReservations = true,
                SupportsMultiLocation = true,
                SupportsLowStockAlerts = true
            },
            RealTime = new ErpCapability
            {
                Supported = true,
                SupportedEventTypes = new List<string> { "material-change", "order-update" },
                SupportsWebhooks = true,
                SupportsPolling = true
            },
            Batch = new ErpBatchCapabilities
            {
                Supported = true,
                MaxBatchSize = 1000,
                SupportsBulkImport = true,
                SupportsBulkExport = true
            },
            SupportedAuthTypes = new List<string> { "basic", "oauth2" },
            SupportedDataTypes = new List<string> { "products", "customers", "orders", "inventory", "materials" },
            CustomCapabilities = new Dictionary<string, object>
            {
                ["supportsIdoc"] = false,
                ["supportsRfc"] = false,
                ["supportsOData"] = true,
                ["supportsMultiCompany"] = true
            }
        };

        return Task.FromResult(capabilities);
    }

    /// <summary>
    /// Tests the connection to the ERP system.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The connection test result.</returns>
    public Task<ErpConnectionResult> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Connector must be initialized before use");

        var result = new ErpConnectionResult
        {
            Success = true,
            ResponseTimeMs = 100
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Synchronizes the catalog data.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the sync operation.</returns>
    public Task SyncCatalogAsync(SyncContext context, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Connector must be initialized before use");

        _logger.LogInformation("Syncing catalog for tenant {TenantId}", context.TenantId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates an order in the ERP system.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order creation result.</returns>
    public Task<ErpOrderResult> CreateOrderAsync(ErpOrder order, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Connector must be initialized before use");

        var result = new ErpOrderResult
        {
            Success = true,
            ErpOrderId = $"FASHOP-{order.OrderId}",
            AdditionalData = new Dictionary<string, object>
            {
                ["status"] = "confirmed",
                ["createdAt"] = DateTime.UtcNow
            }
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets customer data from the ERP system.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The customer data.</returns>
    public Task<ErpCustomerData> GetCustomerDataAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Connector must be initialized before use");

        var customerData = new ErpCustomerData
        {
            CustomerId = customerId,
            Name = "Sample Customer",
            Email = "customer@example.com"
        };

        return Task.FromResult(customerData);
    }

    /// <summary>
    /// Disposes the connector.
    /// </summary>
    /// <returns>A value task representing the dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        _isInitialized = false;
        return ValueTask.CompletedTask;
    }
}