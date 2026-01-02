// <copyright file="IErpProvider.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Models;

namespace B2Connect.ERP.Contracts;

/// <summary>
/// Core ERP provider interface for standard ERP operations.
/// Implementations must be thread-safe and handle the single-threading
/// constraint of legacy ERPs like enventa Trade via Actor pattern.
/// </summary>
public interface IErpProvider : IDisposable
{
    /// <summary>
    /// Gets the provider type identifier.
    /// </summary>
    string ProviderType { get; }

    /// <summary>
    /// Gets the provider version.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets a value indicating whether the provider is connected and operational.
    /// </summary>
    bool IsConnected { get; }

    // ========================================
    // Single Operations (for simple queries)
    // ========================================

    /// <summary>
    /// Gets a single product by ID.
    /// </summary>
    Task<ProviderResult<PimProduct>> GetProductAsync(
        string productId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single customer by ID.
    /// </summary>
    Task<ProviderResult<CrmCustomer>> GetCustomerAsync(
        string customerId,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Creates a new order in the ERP system.
    /// </summary>
    Task<ProviderResult<ErpOrder>> CreateOrderAsync(
        OrderRequest request,
        TenantContext context,
        CancellationToken ct = default);

    // ========================================
    // Bulk Operations (to reduce chatty interfaces)
    // ========================================

    /// <summary>
    /// Gets multiple products by IDs in a single operation.
    /// More efficient than multiple GetProductAsync calls.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<PimProduct>>> GetProductsAsync(
        IEnumerable<string> productIds,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets multiple customers by IDs in a single operation.
    /// </summary>
    Task<ProviderResult<IReadOnlyList<CrmCustomer>>> GetCustomersAsync(
        IEnumerable<string> customerIds,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Creates multiple orders in a batch operation.
    /// </summary>
    Task<ProviderResult<BatchResult>> CreateOrdersAsync(
        IEnumerable<OrderRequest> requests,
        TenantContext context,
        CancellationToken ct = default);

    // ========================================
    // Paged Operations (for .NET 4.8 compatibility)
    // ========================================

    /// <summary>
    /// Gets products with paging support.
    /// Use instead of streaming for .NET Framework 4.8 compatibility.
    /// </summary>
    Task<ProviderResult<PagedResult<PimProduct>>> GetProductsPagedAsync(
        ProductFilter filter,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Gets customers with paging support.
    /// </summary>
    Task<ProviderResult<PagedResult<CrmCustomer>>> GetCustomersPagedAsync(
        CustomerFilter filter,
        int pageSize,
        string? continuationToken,
        TenantContext context,
        CancellationToken ct = default);

    // ========================================
    // Sync Operations (for bulk data transfer)
    // ========================================

    /// <summary>
    /// Synchronizes products from the ERP system.
    /// </summary>
    Task<ProviderResult<SyncResult>> SyncProductsAsync(
        SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Synchronizes customers from the ERP system.
    /// </summary>
    Task<ProviderResult<SyncResult>> SyncCustomersAsync(
        SyncRequest request,
        IProgress<SyncProgress>? progress,
        TenantContext context,
        CancellationToken ct = default);

    // ========================================
    // Health & Connection
    // ========================================

    /// <summary>
    /// Performs a health check on the ERP connection.
    /// </summary>
    Task<ProviderResult<HealthCheckResult>> HealthCheckAsync(
        TenantContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Initializes the provider connection.
    /// </summary>
    Task<ProviderResult> InitializeAsync(
        TenantContext context,
        CancellationToken ct = default);
}

/// <summary>
/// Filter for product queries.
/// </summary>
public sealed record ProductFilter
{
    public string? CategoryId { get; init; }
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public DateTimeOffset? ModifiedSince { get; init; }
    public IReadOnlyList<string>? ProductIds { get; init; }
    public bool IncludePrices { get; init; } = true;
    public bool IncludeStock { get; init; } = true;
    public bool IncludeImages { get; init; } = false;
}

/// <summary>
/// Filter for customer queries.
/// </summary>
public sealed record CustomerFilter
{
    public string? SearchTerm { get; init; }
    public string? CustomerGroup { get; init; }
    public bool? IsActive { get; init; }
    public DateTimeOffset? ModifiedSince { get; init; }
    public IReadOnlyList<string>? CustomerIds { get; init; }
}

/// <summary>
/// Request for synchronization operations.
/// </summary>
public sealed record SyncRequest
{
    public SyncMode Mode { get; init; } = SyncMode.Incremental;
    public DateTimeOffset? Since { get; init; }
    public int BatchSize { get; init; } = 1000;
    public bool IncludeDeleted { get; init; } = false;
    public IReadOnlyList<string>? EntityIds { get; init; }
}

/// <summary>
/// Synchronization mode.
/// </summary>
public enum SyncMode
{
    /// <summary>
    /// Full synchronization of all data.
    /// </summary>
    Full,

    /// <summary>
    /// Incremental synchronization of changed data only.
    /// </summary>
    Incremental,

    /// <summary>
    /// Delta synchronization using timestamps.
    /// </summary>
    Delta
}

/// <summary>
/// Health check result for provider status.
/// </summary>
public sealed record HealthCheckResult
{
    public bool IsHealthy { get; init; }
    public string Status { get; init; } = string.Empty;
    public TimeSpan ResponseTime { get; init; }
    public IReadOnlyDictionary<string, object> Details { get; init; } = new Dictionary<string, object>();
}
