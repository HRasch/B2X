using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2Connect.Shared.ErpConnector.Models;

namespace B2Connect.Shared.ErpConnector.Interfaces;

/// <summary>
/// Core interface for ERP system adapters.
/// All ERP connectors must implement this interface.
/// </summary>
public interface IErpAdapter
{
    /// <summary>
    /// Unique identifier for the ERP system type.
    /// </summary>
    string ErpType { get; }

    /// <summary>
    /// Human-readable name of the ERP system.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Version of the adapter.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Supported features by this adapter.
    /// </summary>
    ErpFeatures SupportedFeatures { get; }

    /// <summary>
    /// Initialize the adapter with tenant-specific configuration.
    /// </summary>
    Task InitializeAsync(ErpConfiguration configuration);

    /// <summary>
    /// Test connection to the ERP system.
    /// </summary>
    Task<ErpConnectionResult> TestConnectionAsync();

    /// <summary>
    /// Get articles from the ERP system.
    /// </summary>
    Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleQuery query);

    /// <summary>
    /// Get customers from the ERP system.
    /// </summary>
    Task<IEnumerable<CustomerDto>> GetCustomersAsync(CustomerQuery query);

    /// <summary>
    /// Get orders from the ERP system.
    /// </summary>
    Task<IEnumerable<OrderDto>> GetOrdersAsync(OrderQuery query);

    /// <summary>
    /// Create a new order in the ERP system.
    /// </summary>
    Task<OrderResult> CreateOrderAsync(OrderDto order);

    /// <summary>
    /// Update an existing order in the ERP system.
    /// </summary>
    Task<OrderResult> UpdateOrderAsync(OrderDto order);

    /// <summary>
    /// Dispose resources used by the adapter.
    /// </summary>
    Task DisposeAsync();
}

/// <summary>
/// Features supported by an ERP adapter.
/// </summary>
[Flags]
public enum ErpFeatures
{
    None = 0,
    Articles = 1 << 0,
    Customers = 1 << 1,
    Orders = 1 << 2,
    RealTimeSync = 1 << 3,
    BatchSync = 1 << 4,
    CustomFields = 1 << 5,
    MultiCompany = 1 << 6
}

/// <summary>
/// Result of ERP connection test.
/// </summary>
public class ErpConnectionResult
{
    public bool IsConnected { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ResponseTime { get; set; }
}

/// <summary>
/// Result of order operations.
/// </summary>
public class OrderResult
{
    public bool Success { get; set; }
    public string? OrderNumber { get; set; }
    public string? ErrorMessage { get; set; }
}