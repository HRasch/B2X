// <copyright file="TransactionalErpOperation.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;

namespace B2Connect.ERP.Infrastructure.Transactions;

/// <summary>
/// ERP operation that executes within a transaction scope.
/// For enventa, this wraps operations in FSUtil.CreateScope().
/// </summary>
/// <typeparam name="TResult">Type of the operation result.</typeparam>
public sealed class TransactionalErpOperation<TResult> : Actor.IErpOperation<TResult>
{
    private readonly Func<IErpTransactionScope, CancellationToken, Task<TResult>> _operation;
    private readonly IErpTransactionScopeFactory _scopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionalErpOperation{TResult}"/> class.
    /// </summary>
    /// <param name="tenant">The tenant context.</param>
    /// <param name="scopeFactory">Factory for creating transaction scopes.</param>
    /// <param name="operation">The operation to execute within the transaction.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public TransactionalErpOperation(
        TenantContext tenant,
        IErpTransactionScopeFactory scopeFactory,
        Func<IErpTransactionScope, CancellationToken, Task<TResult>> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        OperationId = Guid.NewGuid();
        Timeout = timeout ?? TimeSpan.FromSeconds(60); // Longer timeout for transactions
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
    public async Task<object?> ExecuteOperationAsync(CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            CancellationToken);

        linkedCts.CancelAfter(Timeout);

        await using var scope = await _scopeFactory.CreateScopeAsync(linkedCts.Token).ConfigureAwait(false);

        try
        {
            var result = await _operation(scope, linkedCts.Token).ConfigureAwait(false);
            await scope.CommitAsync(linkedCts.Token).ConfigureAwait(false);
            return (object?)result!;
        }
        catch
        {
            // Rollback on any exception
            if (scope.IsActive && !scope.IsCommitted)
            {
                await scope.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public void CompleteWithResult(object? result)
    {
        ResultSource.TrySetResult((TResult)result!);
    }

    /// <inheritdoc/>
    public void CompleteWithException(Exception ex)
    {
        ResultSource.TrySetException(ex);
    }
}

/// <summary>
/// Factory for creating transactional ERP operations.
/// </summary>
public static class TransactionalErpOperation
{
    /// <summary>
    /// Creates a new transactional ERP operation.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="scopeFactory">Transaction scope factory.</param>
    /// <param name="operation">The operation to execute within the transaction.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A new transactional operation.</returns>
    public static TransactionalErpOperation<TResult> Create<TResult>(
        TenantContext tenant,
        IErpTransactionScopeFactory scopeFactory,
        Func<IErpTransactionScope, CancellationToken, Task<TResult>> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return new TransactionalErpOperation<TResult>(
            tenant,
            scopeFactory,
            operation,
            timeout,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new transactional ERP operation without return value.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="scopeFactory">Transaction scope factory.</param>
    /// <param name="operation">The operation to execute within the transaction.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A new transactional operation.</returns>
    public static TransactionalErpOperation<bool> Create(
        TenantContext tenant,
        IErpTransactionScopeFactory scopeFactory,
        Func<IErpTransactionScope, CancellationToken, Task> operation,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return new TransactionalErpOperation<bool>(
            tenant,
            scopeFactory,
            async (scope, ct) =>
            {
                await operation(scope, ct).ConfigureAwait(false);
                return true;
            },
            timeout,
            cancellationToken);
    }
}
