// <copyright file="ErpActor.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using B2X.ERP.Core;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Actor for processing ERP operations sequentially.
/// </summary>
public class ErpActor : IAsyncDisposable
{
    private readonly TenantContext _tenantContext;
    private readonly ILogger<ErpActor> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<object> _operationQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private bool _isInitialized;
    private bool _disposed;

    /// <summary>
    /// Gets the number of processed operations.
    /// </summary>
    public int ProcessedOperations { get; private set; }

    /// <summary>
    /// Gets the number of failed operations.
    /// </summary>
    public int FailedOperations { get; private set; }

    /// <summary>
    /// Gets the number of queued operations.
    /// </summary>
    public int QueuedOperations => _operationQueue.Count;

    /// <summary>
    /// Gets a value indicating whether the actor is ready.
    /// </summary>
    public bool IsReady => _isInitialized;

    /// <summary>
    /// Gets the tenant context.
    /// </summary>
    public TenantContext Tenant => _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpActor"/> class.
    /// </summary>
    /// <param name="tenantContext">The tenant context.</param>
    /// <param name="logger">The logger.</param>
    public ErpActor(TenantContext tenantContext, ILogger<ErpActor> logger)
    {
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Initializes the actor.
    /// </summary>
    /// <param name="initializeAction">The initialization action.</param>
    /// <returns>A task representing the initialization operation.</returns>
    public async Task InitializeAsync(Func<TenantContext, CancellationToken, Task> initializeAction)
    {
        if (_isInitialized)
        {
            return;
        }

        await initializeAction(_tenantContext, _cts.Token);
        _isInitialized = true;
    }

    /// <summary>
    /// Enqueues an operation for execution.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="operation">The operation to enqueue.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> EnqueueAsync<T>(ErpOperation<T> operation, CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ErpActor));
        }

        if (!_isInitialized)
        {
            throw new InvalidOperationException("Actor is not initialized.");
        }

        if (!string.Equals(operation.TenantContext.TenantId, _tenantContext.TenantId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Operation tenant '{operation.TenantContext.TenantId}' does not match actor tenant '{_tenantContext.TenantId}'.");
        }

        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            var result = await operation.ExecuteAsync(cancellationToken);
            ProcessedOperations++;
            return result;
        }
        catch (Exception ex)
        {
            FailedOperations++;
            _logger.LogError(ex, "Operation failed for tenant {TenantId}", _tenantContext.TenantId);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Disposes the actor.
    /// </summary>
    /// <returns>A value task representing the dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;
        _cts.Cancel();
        _cts.Dispose();
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
