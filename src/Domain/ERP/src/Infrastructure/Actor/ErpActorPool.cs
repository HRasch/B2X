// <copyright file="ErpActorPool.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using B2X.ERP.Core;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Pool of ErpActors, one per tenant.
/// Manages the lifecycle of actors and provides thread-safe access.
/// </summary>
public sealed class ErpActorPool : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, ErpActor> _actors = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ErpActorPool> _logger;
    private readonly int _actorQueueCapacity;
    private readonly SemaphoreSlim _createLock = new(1, 1);
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpActorPool"/> class.
    /// </summary>
    /// <param name="loggerFactory">Logger factory for creating actor loggers.</param>
    /// <param name="actorQueueCapacity">Queue capacity for each actor (default 1000).</param>
    public ErpActorPool(ILoggerFactory loggerFactory, int actorQueueCapacity = 1000)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<ErpActorPool>();
        _actorQueueCapacity = actorQueueCapacity;

        _logger.LogInformation("ErpActorPool initialized with queue capacity {Capacity}", actorQueueCapacity);
    }

    /// <summary>
    /// Gets the number of active actors in the pool.
    /// </summary>
    public int ActiveActorCount => _actors.Count;

    /// <summary>
    /// Gets the tenants that have active actors.
    /// </summary>
    public IReadOnlyCollection<string> ActiveTenants => _actors.Keys.ToList().AsReadOnly();

    /// <summary>
    /// Gets or creates an actor for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <returns>The actor for the tenant.</returns>
    public async Task<ErpActor> GetOrCreateActorAsync(TenantContext tenant)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ArgumentNullException.ThrowIfNull(tenant);

        // Fast path: actor already exists and is not disposed
        if (_actors.TryGetValue(tenant.TenantId, out var existingActor))
        {
            // Return existing actor (even if not initialized yet - it will be initialized on first use)
            return existingActor;
        }

        // Slow path: create new actor
        await _createLock.WaitAsync().ConfigureAwait(false);
        try
        {
            // Double-check after acquiring lock
            if (_actors.TryGetValue(tenant.TenantId, out existingActor))
            {
                return existingActor;
            }

            var actorLogger = _loggerFactory.CreateLogger<ErpActor>();
            var newActor = new ErpActor(tenant, actorLogger, _actorQueueCapacity);

            if (!_actors.TryAdd(tenant.TenantId, newActor))
            {
                // Another thread added an actor, dispose ours and return theirs
                await newActor.DisposeAsync().ConfigureAwait(false);
                if (_actors.TryGetValue(tenant.TenantId, out var otherActor))
                {
                    return otherActor;
                }

                // Extremely rare: actor was added and removed concurrently
                // Retry by recursion (will acquire lock again)
                return await GetOrCreateActorAsync(tenant).ConfigureAwait(false);
            }

            _logger.LogInformation(
                "Created new ErpActor for tenant {TenantId}. Total actors: {Count}",
                tenant.TenantId,
                _actors.Count);

            return newActor;
        }
        finally
        {
            _createLock.Release();
        }
    }

    /// <summary>
    /// Executes an operation through the appropriate tenant's actor.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<TResult> ExecuteAsync<TResult>(ErpOperation<TResult> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var actor = await GetOrCreateActorAsync(operation.Tenant).ConfigureAwait(false);
        return await actor.EnqueueAsync(operation).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes and disposes an actor for a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    public async Task RemoveActorAsync(string tenantId)
    {
        if (_actors.TryRemove(tenantId, out var actor))
        {
            _logger.LogInformation(
                "Removing ErpActor for tenant {TenantId}. Processed: {Processed}, Errors: {Errors}",
                tenantId,
                actor.ProcessedOperations,
                actor.FailedOperations);

            await actor.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets statistics for all actors in the pool.
    /// </summary>
    /// <returns>Dictionary of tenant ID to actor statistics.</returns>
    public IReadOnlyDictionary<string, ActorStatistics> GetStatistics()
    {
        return _actors.ToDictionary(
            kvp => kvp.Key,
            kvp => new ActorStatistics
            {
                TenantId = kvp.Key,
                QueuedOperations = kvp.Value.QueuedOperations,
                ProcessedOperations = kvp.Value.ProcessedOperations,
                FailedOperations = kvp.Value.FailedOperations,
                IsReady = kvp.Value.IsReady
            });
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _logger.LogInformation("Disposing ErpActorPool with {Count} active actors", _actors.Count);

        var disposeTasks = _actors.Values.Select(actor => actor.DisposeAsync().AsTask());
        await Task.WhenAll(disposeTasks).ConfigureAwait(false);

        _actors.Clear();
        _createLock.Dispose();

        _logger.LogInformation("ErpActorPool disposed");
    }
}

/// <summary>
/// Statistics for an actor.
/// </summary>
public sealed record ActorStatistics
{
    public required string TenantId { get; init; }
    public int QueuedOperations { get; init; }
    public int ProcessedOperations { get; init; }
    public int FailedOperations { get; init; }
    public bool IsReady { get; init; }
}
