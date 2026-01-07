// <copyright file="ProviderManager.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using B2X.ERP.Contracts;
using B2X.ERP.Core;
using B2X.ERP.Infrastructure.Actor;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Services;

/// <summary>
/// Default implementation of <see cref="IProviderManager"/>.
/// Manages provider lifecycle with Actor pattern for thread-safe ERP access.
/// </summary>
public sealed class ProviderManager : IProviderManager, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, ProviderInstance> _providers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ProviderConfiguration> _configurations = new(StringComparer.OrdinalIgnoreCase);
    private readonly ErpActorPool _actorPool;
    private readonly IProviderFactory _providerFactory;
    private readonly ILogger<ProviderManager> _logger;
    private readonly SemaphoreSlim _providerLock = new(1, 1);
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderManager"/> class.
    /// </summary>
    /// <param name="actorPool">The actor pool for serialized ERP access.</param>
    /// <param name="providerFactory">Factory for creating provider instances.</param>
    /// <param name="logger">Logger instance.</param>
    public ProviderManager(
        ErpActorPool actorPool,
        IProviderFactory providerFactory,
        ILogger<ProviderManager> logger)
    {
        _actorPool = actorPool ?? throw new ArgumentNullException(nameof(actorPool));
        _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<IPimProvider> GetPimProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default)
    {
        var instance = await GetOrCreateProviderAsync(tenant, cancellationToken).ConfigureAwait(false);
        return instance.PimProvider ?? throw new InvalidOperationException(
            $"PIM provider not available for tenant {tenant.TenantId}");
    }

    /// <inheritdoc/>
    public async Task<ICrmProvider> GetCrmProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default)
    {
        var instance = await GetOrCreateProviderAsync(tenant, cancellationToken).ConfigureAwait(false);
        return instance.CrmProvider ?? throw new InvalidOperationException(
            $"CRM provider not available for tenant {tenant.TenantId}");
    }

    /// <inheritdoc/>
    public async Task<IErpProvider> GetErpProviderAsync(TenantContext tenant, CancellationToken cancellationToken = default)
    {
        var instance = await GetOrCreateProviderAsync(tenant, cancellationToken).ConfigureAwait(false);
        return instance.ErpProvider ?? throw new InvalidOperationException(
            $"ERP provider not available for tenant {tenant.TenantId}");
    }

    /// <inheritdoc/>
    public async Task<ProviderHealthStatus> CheckHealthAsync(TenantContext tenant, CancellationToken cancellationToken = default)
    {
        var startTime = DateTimeOffset.UtcNow;

        try
        {
            if (!_providers.TryGetValue(tenant.TenantId, out var instance))
            {
                return new ProviderHealthStatus
                {
                    TenantId = tenant.TenantId,
                    IsHealthy = false,
                    ConnectionState = ProviderConnectionState.Disconnected,
                    LastChecked = startTime
                };
            }

            var componentHealth = new Dictionary<string, bool>();

            // Check PIM health - PIM uses the ERP provider's health check
            // since IPimProvider doesn't have its own health method
            if (instance.ErpProvider != null)
            {
                try
                {
                    var erpHealth = await instance.ErpProvider.HealthCheckAsync(tenant, cancellationToken).ConfigureAwait(false);
                    componentHealth["pim"] = erpHealth.IsSuccess && erpHealth.Value?.IsHealthy == true;
                    componentHealth["crm"] = componentHealth["pim"];
                    componentHealth["erp"] = componentHealth["pim"];
                }
                catch
                {
                    componentHealth["pim"] = false;
                    componentHealth["crm"] = false;
                    componentHealth["erp"] = false;
                }
            }

            var isHealthy = componentHealth.Count > 0 && componentHealth.Values.All(v => v);
            var responseTime = DateTimeOffset.UtcNow - startTime;

            return new ProviderHealthStatus
            {
                TenantId = tenant.TenantId,
                IsHealthy = isHealthy,
                ConnectionState = isHealthy
                    ? ProviderConnectionState.Connected
                    : ProviderConnectionState.Degraded,
                LastChecked = DateTimeOffset.UtcNow,
                ResponseTime = responseTime,
                ComponentHealth = componentHealth
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for tenant {TenantId}", tenant.TenantId);

            return new ProviderHealthStatus
            {
                TenantId = tenant.TenantId,
                IsHealthy = false,
                ConnectionState = ProviderConnectionState.Failed,
                ErrorMessage = ex.Message,
                LastChecked = DateTimeOffset.UtcNow,
                ResponseTime = DateTimeOffset.UtcNow - startTime
            };
        }
    }

    /// <inheritdoc/>
    public Task<ProviderConfiguration?> GetConfigurationAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _configurations.TryGetValue(tenantId, out var config);
        return Task.FromResult(config);
    }

    /// <summary>
    /// Sets or updates provider configuration for a tenant.
    /// </summary>
    /// <param name="configuration">The configuration to set.</param>
    public void SetConfiguration(ProviderConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configurations[configuration.TenantId] = configuration;
        _logger.LogInformation(
            "Updated configuration for tenant {TenantId}, provider type: {ProviderType}",
            configuration.TenantId,
            configuration.ProviderType);
    }

    /// <inheritdoc/>
    public async Task ReleaseProviderAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        if (_providers.TryRemove(tenantId, out var instance))
        {
            _logger.LogInformation("Releasing provider for tenant {TenantId}", tenantId);

            await _actorPool.RemoveActorAsync(tenantId).ConfigureAwait(false);

            if (instance.PimProvider is IAsyncDisposable pimDisposable)
            {
                await pimDisposable.DisposeAsync().ConfigureAwait(false);
            }

            if (instance.CrmProvider is IAsyncDisposable crmDisposable)
            {
                await crmDisposable.DisposeAsync().ConfigureAwait(false);
            }

            if (instance.ErpProvider is IAsyncDisposable erpDisposable)
            {
                await erpDisposable.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, ProviderStatistics> GetStatistics()
    {
        var actorStats = _actorPool.GetStatistics();
        var result = new Dictionary<string, ProviderStatistics>();

        foreach (var (tenantId, instance) in _providers)
        {
            actorStats.TryGetValue(tenantId, out var actorStat);

            result[tenantId] = new ProviderStatistics
            {
                TenantId = tenantId,
                ProviderType = instance.Configuration?.ProviderType ?? "unknown",
                ConnectionState = instance.ConnectionState,
                TotalOperations = instance.TotalOperations,
                SuccessfulOperations = instance.SuccessfulOperations,
                FailedOperations = instance.FailedOperations,
                AverageResponseTime = instance.AverageResponseTime,
                MaxResponseTime = instance.MaxResponseTime,
                QueuedOperations = actorStat?.QueuedOperations ?? 0,
                LastActivity = instance.LastActivity,
                LastError = instance.LastError,
                LastErrorMessage = instance.LastErrorMessage
            };
        }

        return result;
    }

    private async Task<ProviderInstance> GetOrCreateProviderAsync(TenantContext tenant, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        // Fast path: provider already exists
        if (_providers.TryGetValue(tenant.TenantId, out var existing) &&
            existing.ConnectionState == ProviderConnectionState.Connected)
        {
            return existing;
        }

        // Slow path: create new provider
        await _providerLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Double-check after acquiring lock
            if (_providers.TryGetValue(tenant.TenantId, out existing) &&
                existing.ConnectionState == ProviderConnectionState.Connected)
            {
                return existing;
            }

            _logger.LogInformation("Creating provider instance for tenant {TenantId}", tenant.TenantId);

            // Get configuration
            var config = await GetConfigurationAsync(tenant.TenantId, cancellationToken).ConfigureAwait(false);
            if (config == null || !config.IsEnabled)
            {
                throw new InvalidOperationException($"No enabled configuration found for tenant {tenant.TenantId}");
            }

            // Get actor for serialized access
            var actor = await _actorPool.GetOrCreateActorAsync(tenant).ConfigureAwait(false);

            // Create providers via factory
            var instance = await _providerFactory.CreateProviderAsync(tenant, config, actor, cancellationToken)
                .ConfigureAwait(false);

            _providers[tenant.TenantId] = instance;

            _logger.LogInformation(
                "Provider instance created for tenant {TenantId}, type: {ProviderType}",
                tenant.TenantId,
                config.ProviderType);

            return instance;
        }
        finally
        {
            _providerLock.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _logger.LogInformation("Disposing ProviderManager with {Count} active providers", _providers.Count);

        foreach (var tenantId in _providers.Keys.ToList())
        {
            await ReleaseProviderAsync(tenantId).ConfigureAwait(false);
        }

        _providerLock.Dispose();
        await _actorPool.DisposeAsync().ConfigureAwait(false);
    }
}

/// <summary>
/// Factory for creating provider instances.
/// </summary>
public interface IProviderFactory
{
    /// <summary>
    /// Creates a provider instance for the specified tenant.
    /// </summary>
    Task<ProviderInstance> CreateProviderAsync(
        TenantContext tenant,
        ProviderConfiguration configuration,
        ErpActor actor,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Container for provider instances and statistics.
/// </summary>
public sealed class ProviderInstance
{
    public ProviderConfiguration? Configuration { get; init; }
    public IPimProvider? PimProvider { get; init; }
    public ICrmProvider? CrmProvider { get; init; }
    public IErpProvider? ErpProvider { get; init; }
    public ProviderConnectionState ConnectionState { get; set; } = ProviderConnectionState.Connected;
    public long TotalOperations { get; set; }
    public long SuccessfulOperations { get; set; }
    public long FailedOperations { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public TimeSpan MaxResponseTime { get; set; }
    public DateTimeOffset LastActivity { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastError { get; set; }
    public string? LastErrorMessage { get; set; }
}
