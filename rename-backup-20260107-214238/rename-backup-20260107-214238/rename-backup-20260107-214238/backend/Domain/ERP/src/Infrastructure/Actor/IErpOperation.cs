// <copyright file="IErpOperation.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Core;

namespace B2X.ERP.Infrastructure.Actor;

/// <summary>
/// Base interface for ERP operations that must be serialized through the Actor.
/// </summary>
public interface IErpOperation
{
    /// <summary>
    /// Unique identifier for the operation.
    /// </summary>
    Guid OperationId { get; }

    /// <summary>
    /// Tenant context for the operation.
    /// </summary>
    TenantContext Tenant { get; }

    /// <summary>
    /// Timeout for the operation.
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// Cancellation token for the operation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Executes the operation and returns the result. The Actor worker is
    /// responsible for completing the typed ResultSource after counting
    /// processed/failed operations to ensure statistics are updated before
    /// callers observing the ResultSource resume.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token from the worker.</param>
    /// <returns>Task that completes with the operation result (boxed).</returns>
    Task<object?> ExecuteOperationAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Completes the operation with the provided result (worker calls this after counting).
    /// </summary>
    /// <param name="result">Boxed result value for the typed ResultSource.</param>
    void CompleteWithResult(object? result);

    /// <summary>
    /// Completes the operation with an exception (worker calls this after counting on error).
    /// </summary>
    /// <param name="ex">The exception to set.</param>
    void CompleteWithException(Exception ex);
}

/// <summary>
/// Typed ERP operation with result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IErpOperation<TResult> : IErpOperation, IErpOperationWithStatus
{
    /// <summary>
    /// Task completion source for the result.
    /// </summary>
    TaskCompletionSource<TResult> ResultSource { get; }
}

/// <summary>
/// Interface for checking operation completion status.
/// </summary>
public interface IErpOperationWithStatus
{
    /// <summary>
    /// Gets a value indicating whether the operation has failed.
    /// </summary>
    bool HasFailed { get; }

    /// <summary>
    /// Gets a value indicating whether the operation has completed successfully.
    /// </summary>
    bool HasSucceeded { get; }
}
