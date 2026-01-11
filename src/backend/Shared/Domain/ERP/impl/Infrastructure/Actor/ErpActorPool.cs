// <copyright file="ErpActorPool.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2X.ERP.Core;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Pool for managing ERP actors per tenant.
/// </summary>
public class ErpActorPool : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, ErpActor> _actors = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ErpActorPool> _logger;

    /// <summary>
    /// Gets the number of active actors.
    /// </summary>
    public int ActiveActorCount => _actors.Count;

    /// <summary>
    /// Gets the active tenant IDs.
    /// </summary>
    public IReadOnlyCollection<string> ActiveTenants => _actors.Keys.ToList().AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpActorPool"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public ErpActorPool(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<ErpActorPool>();
    }

    /// <summary>
    /// Gets or creates an actor for the specified tenant.
    /// </summary>
    /// <param name="tenantContext">The tenant context.</param>
    /// <returns>The ERP actor for the tenant.</returns>
    public async Task<ErpActor> GetOrCreateActorAsync(TenantContext tenantContext)
    {
        if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));

        return await Task.FromResult(_actors.GetOrAdd(tenantContext.TenantId, _ =>
        {
            var logger = _loggerFactory.CreateLogger<ErpActor>();
            return new ErpActor(tenantContext, logger);
        }));
    }

    /// <summary>
    /// Removes an actor for the specified tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <returns>A task representing the removal operation.</returns>
    public async Task RemoveActorAsync(string tenantId)
    {
        if (_actors.TryRemove(tenantId, out var actor))
        {
            await actor.DisposeAsync();
        }
    }

    /// <summary>
    /// Executes an operation using the appropriate actor.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> ExecuteAsync<T>(ErpOperation<T> operation)
    {
        var actor = await GetOrCreateActorAsync(operation.TenantContext);
        return await actor.EnqueueAsync(operation);
    }

    /// <summary>
    /// Gets statistics for all actors.
    /// </summary>
    /// <returns>A dictionary of tenant IDs to actor statistics.</returns>
    public IReadOnlyDictionary<string, ActorStatistics> GetStatistics()
    {
        var stats = new Dictionary<string, ActorStatistics>();
        foreach (var kvp in _actors)
        {
            stats[kvp.Key] = new ActorStatistics
            {
                TenantId = kvp.Key,
                ProcessedOperations = kvp.Value.ProcessedOperations,
                FailedOperations = kvp.Value.FailedOperations,
                QueuedOperations = kvp.Value.QueuedOperations,
                IsReady = kvp.Value.IsReady
            };
        }
        return stats;
    }

    /// <summary>
    /// Disposes the actor pool.
    /// </summary>
    /// <returns>A value task representing the dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        foreach (var actor in _actors.Values)
        {
            await actor.DisposeAsync();
        }
        _actors.Clear();
    }
}

/// <summary>
/// Statistics for an actor.
/// </summary>
public class ActorStatistics
{
    /// <summary>
    /// Gets the tenant ID.
    /// </summary>
    public string TenantId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of processed operations.
    /// </summary>
    public int ProcessedOperations { get; init; }

    /// <summary>
    /// Gets the number of failed operations.
    /// </summary>
    public int FailedOperations { get; init; }

    /// <summary>
    /// Gets the number of queued operations.
    /// </summary>
    public int QueuedOperations { get; init; }

    /// <summary>
    /// Gets a value indicating whether the actor is ready.
    /// </summary>
    public bool IsReady { get; init; }
}