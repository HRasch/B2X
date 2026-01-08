// <copyright file="ErpActor.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using System.Threading.Channels;
using B2X.ERP.Core;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Thread-safe Actor for serialized ERP access.
/// Ensures single-threaded execution of all ERP operations for a specific tenant.
/// Critical for enventa Trade ERP which is NOT thread-safe.
/// </summary>
/// <remarks>
/// This Actor pattern implementation uses a Channel{T} as a message queue.
/// A single background worker processes operations sequentially, ensuring
/// that the underlying ERP connection is never accessed concurrently.
/// </remarks>
public sealed class ErpActor : IAsyncDisposable
{
    private readonly Channel<IErpOperation> _operationQueue;
    private readonly Task _workerTask;
    private readonly CancellationTokenSource _shutdownCts;
    private readonly ILogger<ErpActor> _logger;
    private readonly TenantContext _tenant;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isDisposed;
    private bool _isInitialized;
    private int _processedCount;
    private int _errorCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpActor"/> class.
    /// </summary>
    /// <param name="tenant">The tenant context for this actor.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="queueCapacity">Maximum number of queued operations (default 1000).</param>
    public ErpActor(TenantContext tenant, ILogger<ErpActor> logger, int queueCapacity = 1000)
    {
        _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _operationQueue = Channel.CreateBounded<IErpOperation>(new BoundedChannelOptions(queueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

        _shutdownCts = new CancellationTokenSource();
        _workerTask = Task.Run(() => ProcessOperationsAsync(_shutdownCts.Token));

        _logger.LogInformation(
            "ErpActor created for tenant {TenantId} with queue capacity {Capacity}",
            tenant.TenantId,
            queueCapacity);
    }

    /// <summary>
    /// Gets the tenant context for this actor.
    /// </summary>
    public TenantContext Tenant => _tenant;

    /// <summary>
    /// Gets the number of operations currently in the queue.
    /// </summary>
    public int QueuedOperations => _operationQueue.Reader.Count;

    /// <summary>
    /// Gets the total number of processed operations.
    /// </summary>
    public int ProcessedOperations => _processedCount;

    /// <summary>
    /// Gets the total number of failed operations.
    /// </summary>
    public int FailedOperations => _errorCount;

    /// <summary>
    /// Gets a value indicating whether the actor is ready for operations.
    /// </summary>
    public bool IsReady => _isInitialized && !_isDisposed;

    /// <summary>
    /// Enqueues an operation for execution on the single-threaded worker.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>Task that completes when the operation finishes.</returns>
    public async Task<TResult> EnqueueAsync<TResult>(ErpOperation<TResult> operation)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ArgumentNullException.ThrowIfNull(operation);

        if (!_tenant.TenantId.Equals(operation.Tenant.TenantId, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Operation tenant {operation.Tenant.TenantId} does not match actor tenant {_tenant.TenantId}");
        }

        _logger.LogDebug(
            "Enqueueing operation {OperationId} for tenant {TenantId}",
            operation.OperationId,
            _tenant.TenantId);

        await _operationQueue.Writer.WriteAsync(operation, operation.CancellationToken).ConfigureAwait(false);

        return await operation.ResultSource.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Initializes the ERP connection for this actor.
    /// Call this before enqueueing operations if lazy initialization is needed.
    /// </summary>
    /// <param name="initializeFunc">Async function to initialize the ERP connection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task InitializeAsync(
        Func<TenantContext, CancellationToken, Task> initializeFunc,
        CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return;
        }

        await _initLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_isInitialized)
            {
                return;
            }

            _logger.LogInformation("Initializing ERP connection for tenant {TenantId}", _tenant.TenantId);

            await initializeFunc(_tenant, cancellationToken).ConfigureAwait(false);

            _isInitialized = true;

            _logger.LogInformation("ERP connection initialized for tenant {TenantId}", _tenant.TenantId);
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// Background worker that processes operations sequentially.
    /// </summary>
    private async Task ProcessOperationsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ErpActor worker started for tenant {TenantId}", _tenant.TenantId);

        try
        {
            await foreach (var operation in _operationQueue.Reader.ReadAllAsync(cancellationToken))
            {
                await ProcessSingleOperationAsync(operation).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("ErpActor worker stopped for tenant {TenantId}", _tenant.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "ErpActor worker crashed for tenant {TenantId}", _tenant.TenantId);
            throw;
        }
    }

    /// <summary>
    /// Processes a single operation with error handling.
    /// No reflection - uses typed ExecuteAndCompleteAsync method.
    /// </summary>
    private async Task ProcessSingleOperationAsync(IErpOperation operation)
    {
        var startTime = DateTimeOffset.UtcNow;

        _logger.LogDebug(
            "Processing operation {OperationId} for tenant {TenantId}",
            operation.OperationId,
            _tenant.TenantId);

        // Execute operation and obtain the result. The worker will
        // increment counters before completing the caller's Task to avoid
        // races where callers resume before statistics are updated.
        try
        {
            var result = await operation.ExecuteOperationAsync(operation.CancellationToken).ConfigureAwait(false);

            var duration = DateTimeOffset.UtcNow - startTime;

            // Operation executed successfully - increment processed count BEFORE completing the caller
            Interlocked.Increment(ref _processedCount);

            _logger.LogDebug(
                "Operation {OperationId} completed in {Duration}ms for tenant {TenantId}",
                operation.OperationId,
                duration.TotalMilliseconds,
                _tenant.TenantId);

            // Complete the typed result for waiting callers
            try
            {
                operation.CompleteWithResult(result);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _errorCount);
                operation.CompleteWithException(ex);
                _logger.LogError(ex, "Failed to complete result for operation {OperationId} on tenant {TenantId}", operation.OperationId, _tenant.TenantId);
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _errorCount);
            _logger.LogError(ex, "Operation {OperationId} crashed for tenant {TenantId}", operation.OperationId, _tenant.TenantId);

            try
            {
                operation.CompleteWithException(ex);
            }
            catch
            {
                // Swallow - nothing we can do if completion fails
            }
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

        _logger.LogInformation(
            "Disposing ErpActor for tenant {TenantId}. Processed: {Processed}, Errors: {Errors}",
            _tenant.TenantId,
            _processedCount,
            _errorCount);

        // Signal shutdown
        _operationQueue.Writer.Complete();
        await _shutdownCts.CancelAsync().ConfigureAwait(false);

        // Wait for worker to finish (with timeout)
        try
        {
            await _workerTask.WaitAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            _logger.LogWarning("ErpActor worker did not shut down gracefully for tenant {TenantId}", _tenant.TenantId);
        }

        _shutdownCts.Dispose();
        _initLock.Dispose();
    }
}
