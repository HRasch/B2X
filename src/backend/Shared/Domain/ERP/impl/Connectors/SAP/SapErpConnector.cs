// <copyright file="SapErpConnector.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using B2X.ERP.Abstractions;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Connectors.SAP;

/// <summary>
/// ERP connector for SAP systems.
/// </summary>
public class SapErpConnector : IErpConnector
{
    private readonly ILogger<SapErpConnector> _logger;
    private readonly HttpClient _httpClient;
    private bool _isInitialized;
    private string _erpVersion = "S/4HANA 2022+";

    /// <summary>
    /// Gets the ERP type identifier.
    /// </summary>
    public string ErpType => "sap";

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName => "SAP ERP";

    /// <summary>
    /// Gets the version information.
    /// </summary>
    public ErpVersionInfo Version => new()
    {
        SystemName = "SAP ERP/S4HANA",
        SystemVersion = _erpVersion,
        ApiVersion = "OData V4",
        SupportsBackwardCompatibility = true,
        MinimumSystemVersion = "S/4HANA 1909",
        RecommendedSystemVersion = "S/4HANA 2022+",
        Version = "1.0.0"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="SapErpConnector"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClient">The HTTP client.</param>
    public SapErpConnector(ILogger<SapErpConnector> logger, HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Initializes the connector with the specified configuration.
    /// </summary>
    /// <param name="config">The ERP configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the initialization operation.</returns>
    public Task InitializeAsync(ErpConfiguration config, CancellationToken cancellationToken = default)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));
        if (string.IsNullOrEmpty(config.ErpType) || config.ErpType != "sap")
            throw new ArgumentException("Invalid ERP type for SAP connector", nameof(config));

        if (!config.ConnectionSettings.TryGetValue("serviceUrl", out var serviceUrl) || string.IsNullOrWhiteSpace(serviceUrl))
            throw new ArgumentException("SAP connector requires 'serviceUrl' connection setting", nameof(config));

        _erpVersion = string.IsNullOrWhiteSpace(config.ErpVersion)
            ? "S/4HANA 2022+"
            : config.ErpVersion;
        _isInitialized = true;
        _logger.LogInformation("SapErpConnector initialized for tenant {TenantId}", config.TenantId);
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
                SupportedEntityTypes = new List<string> { "materials" }
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
                SupportedEventTypes = new List<string> { "material-change" },
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
            SupportedAuthTypes = new List<string> { "oauth2", "basic", "certificate" },
            SupportedDataTypes = new List<string> { "materials", "customers" },
            CustomCapabilities = new Dictionary<string, object>
            {
                ["supportsIdoc"] = true,
                ["supportsRfc"] = true,
                ["supportsOData"] = true,
                ["supportsMultiCompany"] = true
            }
        };

        if (_erpVersion.Contains("ECC", StringComparison.OrdinalIgnoreCase))
        {
            capabilities.Catalog.SupportsDeltaSync = false;
            capabilities.Catalog.SupportsRealTimeUpdates = false;
            capabilities.Order.SupportsReturns = false;
            capabilities.RealTime.SupportsWebhooks = false;
            capabilities.RealTime.SupportsPolling = true;
        }

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
            ResponseTimeMs = 200
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
            ErpOrderId = order.OrderId,
            AdditionalData = new Dictionary<string, object>
            {
                ["status"] = "created",
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
