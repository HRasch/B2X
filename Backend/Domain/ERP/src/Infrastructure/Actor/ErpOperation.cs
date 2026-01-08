// <copyright file="ErpOperation.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Core;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Wrapper for ERP operations to be queued in the Actor.
/// Encapsulates the operation function, context, and result mechanism.
/// </summary>
/// <typeparam name="TResult">Type of the operation result.</typeparam>
public sealed class ErpOperation<TResult> : IErpOperation<TResult>
{
    private readonly Func<CancellationToken, Task<TResult>> _operation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpOperation{TResult}"/> class.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="timeout">Optional timeout (default 30 seconds).</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public ErpOperation(
        TenantContext tenant,
        Func<CancellationToken, Task<TResult>> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        OperationId = Guid.NewGuid();
        Timeout = timeout ?? TimeSpan.FromSeconds(30);
        CancellationToken = cancellationToken;
        ResultSource = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public Guid OperationId { get; }

    /// <inheritdoc/>
    public TenantContext Tenant { get; }

    /// <inheritdoc/>
    public TimeSpan Timeout { get; }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc/>
    public TaskCompletionSource<TResult> ResultSource { get; }

    /// <inheritdoc/>
    public bool HasFailed => ResultSource.Task.IsCompleted && ResultSource.Task.IsFaulted;

    /// <inheritdoc/>
    public bool HasSucceeded => ResultSource.Task.IsCompleted && ResultSource.Task.IsCompletedSuccessfully;

    /// <summary>
    /// Gets the timestamp when this operation was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <inheritdoc/>
    /// <summary>
    /// Executes the operation and returns the result as boxed object.
    /// The caller (actor worker) is responsible for completing the typed ResultSource
    /// to ensure counters are updated _before_ completion is signalled to callers.
    /// </summary>
    public async Task<object?> ExecuteOperationAsync(CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            CancellationToken);

        linkedCts.CancelAfter(Timeout);

        try
        {
            var result = await _operation(linkedCts.Token).ConfigureAwait(false);
            return (object?)result!;
        }
        catch (OperationCanceledException) when (linkedCts.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"ERP operation {OperationId} timed out after {Timeout.TotalSeconds} seconds.");
        }
    }

    /// <summary>
    /// Completes the operation with a typed result (called by the actor worker).
    /// </summary>
    /// <param name="result">Boxed result.</param>
    public void CompleteWithResult(object? result)
    {
        ResultSource.TrySetResult((TResult)result!);
    }

    /// <summary>
    /// Completes the operation with an exception (called by the actor worker).
    /// </summary>
    /// <param name="ex">Exception to set.</param>
    public void CompleteWithException(Exception ex)
    {
        ResultSource.TrySetException(ex);
    }

    /// <summary>
    /// Executes the operation with proper error handling.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    private async Task<TResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            CancellationToken);

        linkedCts.CancelAfter(Timeout);

        try
        {
            return await _operation(linkedCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (linkedCts.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"ERP operation {OperationId} timed out after {Timeout.TotalSeconds} seconds.");
        }
    }
}

/// <summary>
/// Factory for creating ERP operations.
/// </summary>
public static class ErpOperation
{
    /// <summary>
    /// Creates a new ERP operation.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A new ErpOperation instance.</returns>
    public static ErpOperation<TResult> Create<TResult>(
        TenantContext tenant,
        Func<CancellationToken, Task<TResult>> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return new ErpOperation<TResult>(tenant, operation, timeout, cancellationToken);
    }

    /// <summary>
    /// Creates a new ERP operation without a return value.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A new ErpOperation instance.</returns>
    public static ErpOperation<bool> Create(
        TenantContext tenant,
        Func<CancellationToken, Task> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return new ErpOperation<bool>(
            tenant,
            async ct =>
            {
                await operation(ct).ConfigureAwait(false);
                return true;
            },
            timeout,
            cancellationToken);
    }
}
