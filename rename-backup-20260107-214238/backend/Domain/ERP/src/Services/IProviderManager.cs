// <copyright file="IProviderManager.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Contracts;
using B2X.ERP.Core;

namespace B2X.ERP.Services;

/// <summary>
/// Manages ERP/PIM/CRM provider lifecycle and access.
/// </summary>
public interface IProviderManager
{
    /// <summary>
    /// Gets a PIM provider for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The PIM provider instance.</returns>
    Task<IPimProvider> GetPimProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a CRM provider for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The CRM provider instance.</returns>
    Task<ICrmProvider> GetCrmProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an ERP provider for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ERP provider instance.</returns>
    Task<IErpProvider> GetErpProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a provider is available and healthy for the tenant.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Health status of the provider.</returns>
    Task<ProviderHealthStatus> CheckHealthAsync(TenantContext tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets provider configuration for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provider configuration.</returns>
    Task<ProviderConfiguration?> GetConfigurationAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases provider resources for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReleaseProviderAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics for all active providers.
    /// </summary>
    /// <returns>Dictionary of tenant ID to provider statistics.</returns>
    IReadOnlyDictionary<string, ProviderStatistics> GetStatistics();
}

/// <summary>
/// Provider health status.
/// </summary>
public sealed record ProviderHealthStatus
{
    public required string TenantId { get; init; }
    public required bool IsHealthy { get; init; }
    public required ProviderConnectionState ConnectionState { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset LastChecked { get; init; }
    public TimeSpan? ResponseTime { get; init; }
    public IReadOnlyDictionary<string, bool>? ComponentHealth { get; init; }
}

/// <summary>
/// Provider connection state.
/// </summary>
public enum ProviderConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Degraded,
    Failed
}

/// <summary>
/// Individual provider capability for capability checking.
/// </summary>
public enum ProviderCapability
{
    Products,
    Customers,
    Orders,
    Pricing,
    Stock,
    Categories,
    Contacts,
    Activities,
    Streaming,
    BatchOperations,
    RealTimeSync,
    DocumentDownload,
    PriceCalculation,
    StockReservation
}

/// <summary>
/// Provider configuration for a tenant.
/// </summary>
public sealed record ProviderConfiguration
{
    public required string TenantId { get; init; }
    public required string ProviderType { get; init; }
    public string? EndpointUrl { get; init; }
    public string? ContainerImage { get; init; }
    public IReadOnlyDictionary<string, string>? Settings { get; init; }
    public ProviderCapabilities Capabilities { get; init; }
    public RetryConfiguration? RetryConfig { get; init; }
    public TimeSpan? DefaultTimeout { get; init; }
    public int? MaxConcurrentOperations { get; init; }
    public bool IsEnabled { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
}

/// <summary>
/// Provider capabilities flags.
/// </summary>
[Flags]
public enum ProviderCapabilities
{
    None = 0,
    Pim = 1,
    Crm = 2,
    Erp = 4,
    Streaming = 8,
    BatchOperations = 16,
    RealTimeSync = 32,
    DocumentDownload = 64,
    PriceCalculation = 128,
    StockReservation = 256,
    All = Pim | Crm | Erp | Streaming | BatchOperations | RealTimeSync | DocumentDownload | PriceCalculation | StockReservation
}

/// <summary>
/// Retry configuration.
/// </summary>
public sealed record RetryConfiguration
{
    public int MaxRetries { get; init; } = 3;
    public TimeSpan InitialDelay { get; init; } = TimeSpan.FromSeconds(1);
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(30);
    public double BackoffMultiplier { get; init; } = 2.0;
    public bool RetryOnTimeout { get; init; } = true;
}

/// <summary>
/// Provider statistics.
/// </summary>
public sealed record ProviderStatistics
{
    public required string TenantId { get; init; }
    public required string ProviderType { get; init; }
    public ProviderConnectionState ConnectionState { get; init; }
    public long TotalOperations { get; init; }
    public long SuccessfulOperations { get; init; }
    public long FailedOperations { get; init; }
    public double SuccessRate => TotalOperations > 0 ? (SuccessfulOperations * 100.0 / TotalOperations) : 0;
    public TimeSpan AverageResponseTime { get; init; }
    public TimeSpan MaxResponseTime { get; init; }
    public int QueuedOperations { get; init; }
    public DateTimeOffset LastActivity { get; init; }
    public DateTimeOffset? LastError { get; init; }
    public string? LastErrorMessage { get; init; }
}
