// <copyright file="IErpOperation.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;

namespace B2Connect.ERP.Infrastructure.Actor;

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
    /// Executes the operation and completes the result source.
    /// This method is called by the Actor worker thread.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token from the worker.</param>
    /// <returns>Task that completes when the operation is done.</returns>
    Task ExecuteAndCompleteAsync(CancellationToken cancellationToken);
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
