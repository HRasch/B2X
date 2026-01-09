// <copyright file="IErpTransactionScope.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Infrastructure.Transactions;

/// <summary>
/// Interface for ERP transaction scopes.
/// Adapts to enventa's FSUtil.CreateScope() pattern.
/// </summary>
/// <remarks>
/// enventa Trade ERP uses FSUtil.CreateScope() for transactional operations:
/// <code>
/// using var scope = FSUtil.CreateScope();
/// // ... operations ...
/// scope.Commit();
/// </code>
/// This interface abstracts that pattern for .NET 10 compatibility.
/// </remarks>
public interface IErpTransactionScope : IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the transaction is active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets a value indicating whether the transaction has been committed.
    /// </summary>
    bool IsCommitted { get; }

    /// <summary>
    /// Gets a value indicating whether the transaction has been rolled back.
    /// </summary>
    bool IsRolledBack { get; }

    /// <summary>
    /// Commits the transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Factory for creating ERP transaction scopes.
/// </summary>
public interface IErpTransactionScopeFactory
{
    /// <summary>
    /// Creates a new transaction scope.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new transaction scope.</returns>
    Task<IErpTransactionScope> CreateScopeAsync(CancellationToken cancellationToken = default);
}
