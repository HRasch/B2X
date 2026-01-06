using System.ComponentModel.DataAnnotations;

namespace B2Connect.ERP.Abstractions;

/// <summary>
/// ERP system version information.
/// </summary>
public class ErpVersionInfo
{
    /// <summary>
    /// The ERP system name (e.g., "SAP", "Oracle EBS", "Microsoft Dynamics").
    /// </summary>
    [Required]
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// The ERP system version (e.g., "S/4HANA 2022", "EBS 12.2", "Dynamics 365 v9.2").
    /// </summary>
    [Required]
    public string SystemVersion { get; set; } = string.Empty;

    /// <summary>
    /// The API version supported by this connector.
    /// </summary>
    public string? ApiVersion { get; set; }

    /// <summary>
    /// Whether this version supports backward compatibility.
    /// </summary>
    public bool SupportsBackwardCompatibility { get; set; }

    /// <summary>
    /// Minimum required ERP system version for this connector.
    /// </summary>
    public string? MinimumSystemVersion { get; set; }

    /// <summary>
    /// Recommended ERP system version.
    /// </summary>
    public string? RecommendedSystemVersion { get; set; }
}

/// <summary>
/// Core ERP connector interface for pluggable ERP integrations.
/// Provides standardized operations across different ERP systems.
/// </summary>
public interface IErpConnector
{
    /// <summary>
    /// Gets the ERP system version information.
    /// </summary>
    ErpVersionInfo Version { get; }

    /// <summary>
    /// Initializes the connector with the provided configuration.
    /// </summary>
    /// <param name="config">The ERP configuration for this tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the initialization operation.</returns>
    Task InitializeAsync(ErpConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the capabilities supported by this ERP connector for the current version.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ERP capabilities.</returns>
    Task<ErpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the catalog data from the ERP system.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the sync operation.</returns>
    Task SyncCatalogAsync(SyncContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an order in the ERP system.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the order creation.</returns>
    Task<ErpOrderResult> CreateOrderAsync(ErpOrder order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customer data from the ERP system.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer data.</returns>
    Task<ErpCustomerData> GetCustomerDataAsync(string customerId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Factory interface for creating ERP connector instances.
/// </summary>
public interface IErpAdapterFactory
{
    /// <summary>
    /// Gets the ERP type identifier (e.g., "sap", "oracle", "dynamics").
    /// </summary>
    string ErpType { get; }

    /// <summary>
    /// Creates a new connector instance for the specified tenant context.
    /// </summary>
    /// <param name="context">The tenant context.</param>
    /// <returns>The ERP connector instance.</returns>
    IErpConnector CreateConnector(TenantContext context);

    /// <summary>
    /// Gets the configuration schema for this ERP type.
    /// </summary>
    /// <returns>The configuration schema.</returns>
    ErpConfigurationSchema GetConfigurationSchema();
}

/// <summary>
/// ERP configuration for a specific tenant and ERP type.
/// </summary>
public class ErpConfiguration
{
    /// <summary>
    /// The ERP type (e.g., "sap", "oracle").
    /// </summary>
    [Required]
    public string ErpType { get; set; } = string.Empty;

    /// <summary>
    /// The ERP system version (optional, for version-specific behavior).
    /// </summary>
    public string? ErpVersion { get; set; }

    /// <summary>
    /// The tenant identifier.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Connection settings specific to the ERP type.
    /// </summary>
    public Dictionary<string, object> ConnectionSettings { get; set; } = new();

    /// <summary>
    /// Authentication settings.
    /// </summary>
    public ErpAuthentication Authentication { get; set; } = new();

    /// <summary>
    /// Synchronization settings.
    /// </summary>
    public ErpSyncSettings SyncSettings { get; set; } = new();

    /// <summary>
    /// Custom configuration properties.
    /// </summary>
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// Authentication settings for ERP connection.
/// </summary>
public class ErpAuthentication
{
    /// <summary>
    /// The authentication type (e.g., "basic", "oauth2", "certificate").
    /// </summary>
    public string Type { get; set; } = "basic";

    /// <summary>
    /// Username for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for basic authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Client ID for OAuth2.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret for OAuth2.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Token endpoint for OAuth2.
    /// </summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>
    /// Certificate path for certificate authentication.
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>
    /// Certificate password.
    /// </summary>
    public string? CertificatePassword { get; set; }
}

/// <summary>
/// Synchronization settings.
/// </summary>
public class ErpSyncSettings
{
    /// <summary>
    /// Batch size for bulk operations.
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Timeout for sync operations in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Whether to enable delta synchronization.
    /// </summary>
    public bool EnableDeltaSync { get; set; } = true;

    /// <summary>
    /// Retry policy settings.
    /// </summary>
    public ErpRetryPolicy RetryPolicy { get; set; } = new();
}

/// <summary>
/// Retry policy for ERP operations.
/// </summary>
public class ErpRetryPolicy
{
    /// <summary>
    /// Maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Initial retry delay in seconds.
    /// </summary>
    public int InitialDelaySeconds { get; set; } = 1;

    /// <summary>
    /// Maximum retry delay in seconds.
    /// </summary>
    public int MaxDelaySeconds { get; set; } = 60;

    /// <summary>
    /// Backoff multiplier.
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;
}

/// <summary>
/// Capabilities supported by an ERP connector.
/// </summary>
/// <summary>
/// Comprehensive ERP capabilities for feature support matrix.
/// </summary>
public class ErpCapabilities
{
    /// <summary>
    /// Catalog management capabilities.
    /// </summary>
    public CatalogCapabilities Catalog { get; set; } = new();

    /// <summary>
    /// Order management capabilities.
    /// </summary>
    public OrderCapabilities Order { get; set; } = new();

    /// <summary>
    /// Customer management capabilities.
    /// </summary>
    public CustomerCapabilities Customer { get; set; } = new();

    /// <summary>
    /// Inventory management capabilities.
    /// </summary>
    public InventoryCapabilities Inventory { get; set; } = new();

    /// <summary>
    /// Real-time synchronization capabilities.
    /// </summary>
    public RealTimeCapabilities RealTime { get; set; } = new();

    /// <summary>
    /// Batch operation capabilities.
    /// </summary>
    public BatchCapabilities Batch { get; set; } = new();

    /// <summary>
    /// Authentication types supported.
    /// </summary>
    public List<string> SupportedAuthTypes { get; set; } = new();

    /// <summary>
    /// Data types supported by this ERP system.
    /// </summary>
    public List<string> SupportedDataTypes { get; set; } = new();

    /// <summary>
    /// Custom capabilities specific to this ERP system.
    /// </summary>
    public Dictionary<string, object> CustomCapabilities { get; set; } = new();
}

/// <summary>
/// Catalog management capabilities.
/// </summary>
public class CatalogCapabilities
{
    /// <summary>
    /// Whether catalog synchronization is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Whether full catalog sync is supported.
    /// </summary>
    public bool SupportsFullSync { get; set; }

    /// <summary>
    /// Whether delta/incremental sync is supported.
    /// </summary>
    public bool SupportsDeltaSync { get; set; }

    /// <summary>
    /// Whether real-time catalog updates are supported.
    /// </summary>
    public bool SupportsRealTimeUpdates { get; set; }

    /// <summary>
    /// Supported catalog entity types.
    /// </summary>
    public List<string> SupportedEntityTypes { get; set; } = new();
}

/// <summary>
/// Order management capabilities.
/// </summary>
public class OrderCapabilities
{
    /// <summary>
    /// Whether order creation is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Whether order status updates are supported.
    /// </summary>
    public bool SupportsStatusUpdates { get; set; }

    /// <summary>
    /// Whether order cancellation is supported.
    /// </summary>
    public bool SupportsCancellation { get; set; }

    /// <summary>
    /// Whether order returns/refunds are supported.
    /// </summary>
    public bool SupportsReturns { get; set; }

    /// <summary>
    /// Whether partial orders are supported.
    /// </summary>
    public bool SupportsPartialOrders { get; set; }
}

/// <summary>
/// Customer management capabilities.
/// </summary>
public class CustomerCapabilities
{
    /// <summary>
    /// Whether customer data retrieval is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Whether customer creation is supported.
    /// </summary>
    public bool SupportsCreation { get; set; }

    /// <summary>
    /// Whether customer updates are supported.
    /// </summary>
    public bool SupportsUpdates { get; set; }

    /// <summary>
    /// Whether customer address management is supported.
    /// </summary>
    public bool SupportsAddressManagement { get; set; }

    /// <summary>
    /// Whether customer credit limit checks are supported.
    /// </summary>
    public bool SupportsCreditLimitChecks { get; set; }
}

/// <summary>
/// Inventory management capabilities.
/// </summary>
public class InventoryCapabilities
{
    /// <summary>
    /// Whether inventory synchronization is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Whether real-time inventory updates are supported.
    /// </summary>
    public bool SupportsRealTimeUpdates { get; set; }

    /// <summary>
    /// Whether inventory reservations are supported.
    /// </summary>
    public bool SupportsReservations { get; set; }

    /// <summary>
    /// Whether multi-location inventory is supported.
    /// </summary>
    public bool SupportsMultiLocation { get; set; }

    /// <summary>
    /// Whether low stock alerts are supported.
    /// </summary>
    public bool SupportsLowStockAlerts { get; set; }
}

/// <summary>
/// Real-time synchronization capabilities.
/// </summary>
public class RealTimeCapabilities
{
    /// <summary>
    /// Whether real-time sync is supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Supported real-time event types.
    /// </summary>
    public List<string> SupportedEventTypes { get; set; } = new();

    /// <summary>
    /// Whether webhooks are supported.
    /// </summary>
    public bool SupportsWebhooks { get; set; }

    /// <summary>
    /// Whether polling-based real-time sync is supported.
    /// </summary>
    public bool SupportsPolling { get; set; }
}

/// <summary>
/// Batch operation capabilities.
/// </summary>
public class BatchCapabilities
{
    /// <summary>
    /// Whether batch operations are supported.
    /// </summary>
    public bool Supported { get; set; }

    /// <summary>
    /// Maximum batch size.
    /// </summary>
    public int? MaxBatchSize { get; set; }

    /// <summary>
    /// Whether bulk imports are supported.
    /// </summary>
    public bool SupportsBulkImport { get; set; }

    /// <summary>
    /// Whether bulk exports are supported.
    /// </summary>
    public bool SupportsBulkExport { get; set; }
}

/// <summary>
/// Context for synchronization operations.
/// </summary>
public class SyncContext
{
    /// <summary>
    /// The tenant identifier.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// The last synchronization watermark.
    /// </summary>
    public string? LastWatermark { get; set; }

    /// <summary>
    /// The synchronization mode.
    /// </summary>
    public SyncMode Mode { get; set; } = SyncMode.Full;

    /// <summary>
    /// Specific entity types to sync.
    /// </summary>
    public List<string> EntityTypes { get; set; } = new();

    /// <summary>
    /// Progress callback.
    /// </summary>
    public Func<SyncProgress, Task>? ProgressCallback { get; set; }
}

/// <summary>
/// Synchronization mode.
/// </summary>
public enum SyncMode
{
    /// <summary>
    /// Full synchronization.
    /// </summary>
    Full,

    /// <summary>
    /// Incremental synchronization.
    /// </summary>
    Incremental,

    /// <summary>
    /// Delta synchronization.
    /// </summary>
    Delta
}

/// <summary>
/// Synchronization progress information.
/// </summary>
public class SyncProgress
{
    /// <summary>
    /// Current phase of synchronization.
    /// </summary>
    public string Phase { get; set; } = string.Empty;

    /// <summary>
    /// Percentage complete (0-100).
    /// </summary>
    public int Percentage { get; set; }

    /// <summary>
    /// Current item being processed.
    /// </summary>
    public string? CurrentItem { get; set; }

    /// <summary>
    /// Total items to process.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Items processed so far.
    /// </summary>
    public int ProcessedItems { get; set; }
}

/// <summary>
/// Order data for ERP creation.
/// </summary>
public class ErpOrder
{
    /// <summary>
    /// Order identifier.
    /// </summary>
    [Required]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Customer identifier.
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Order lines.
    /// </summary>
    public List<ErpOrderLine> Lines { get; set; } = new();

    /// <summary>
    /// Order total.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Currency.
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Order date.
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Order line item.
/// </summary>
public class ErpOrderLine
{
    /// <summary>
    /// Product identifier.
    /// </summary>
    [Required]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Line total.
    /// </summary>
    public decimal LineTotal { get; set; }
}

/// <summary>
/// Result of order creation.
/// </summary>
public class ErpOrderResult
{
    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The ERP order identifier.
    /// </summary>
    public string? ErpOrderId { get; set; }

    /// <summary>
    /// Error message if operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional result data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Customer data from ERP.
/// </summary>
public class ErpCustomerData
{
    /// <summary>
    /// Customer identifier.
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Customer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Address information.
    /// </summary>
    public ErpAddress? Address { get; set; }

    /// <summary>
    /// Additional customer attributes.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = new();
}

/// <summary>
/// Address information.
/// </summary>
public class ErpAddress
{
    /// <summary>
    /// Street address.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// City.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    public string? Country { get; set; }
}

/// <summary>
/// Configuration schema for ERP connectors.
/// </summary>
public class ErpConfigurationSchema
{
    /// <summary>
    /// The ERP type identifier.
    /// </summary>
    public string ErpType { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the ERP system.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Version of the connector.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Supported ERP system versions.
    /// </summary>
    public IEnumerable<string> SupportedVersions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Required configuration settings.
    /// </summary>
    public Dictionary<string, ErpSettingDefinition> RequiredSettings { get; set; } = new();

    /// <summary>
    /// Optional configuration settings.
    /// </summary>
    public Dictionary<string, ErpSettingDefinition> OptionalSettings { get; set; } = new();

    /// <summary>
    /// Supported authentication types.
    /// </summary>
    public IEnumerable<ErpAuthenticationType> AuthenticationTypes { get; set; } = Array.Empty<ErpAuthenticationType>();
}

/// <summary>
/// Definition of an ERP setting.
/// </summary>
public class ErpSettingDefinition
{
    /// <summary>
    /// The setting key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The setting type (string, number, boolean).
    /// </summary>
    public string Type { get; set; } = "string";

    /// <summary>
    /// Whether the setting is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Default value for the setting.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Description of the setting.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// ERP authentication type definition.
/// </summary>
public class ErpAuthenticationType
{
    /// <summary>
    /// The authentication type identifier.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the authentication type.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Tenant context for multi-tenant operations.
/// </summary>
public class TenantContext
{
    /// <summary>
    /// Tenant identifier.
    /// </summary>
    [Required]
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Tenant name.
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// Tenant-specific configuration.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new();

    /// <summary>
    /// User context for the operation.
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// Registry for managing ERP connector factories.
/// </summary>
public interface IErpConnectorRegistry
{
    /// <summary>
    /// Gets all registered connector factories.
    /// </summary>
    IEnumerable<IErpAdapterFactory> GetAllFactories();

    /// <summary>
    /// Gets a connector factory by ERP type.
    /// </summary>
    /// <param name="erpType">The ERP type identifier.</param>
    /// <returns>The connector factory.</returns>
    IErpAdapterFactory GetFactory(string erpType);

    /// <summary>
    /// Checks if a connector factory is registered for the given ERP type.
    /// </summary>
    /// <param name="erpType">The ERP type identifier.</param>
    /// <returns>True if registered, false otherwise.</returns>
    bool IsRegistered(string erpType);
}
