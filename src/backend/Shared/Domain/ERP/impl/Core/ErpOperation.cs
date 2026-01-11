// <copyright file="ErpOperation.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.ERP.Core;

/// <summary>
/// Represents an ERP operation.
/// </summary>
public class ErpOperation
{
    /// <summary>
    /// Creates a new ERP operation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="tenantContext">The tenant context.</param>
    /// <param name="operation">The operation function.</param>
    /// <param name="timeout">The operation timeout.</param>
    /// <returns>A new ERP operation.</returns>
    public static ErpOperation<T> Create<T>(TenantContext tenantContext, Func<CancellationToken, Task<T>> operation, TimeSpan? timeout = null)
    {
        return new ErpOperation<T>(tenantContext, operation, timeout);
    }
}

/// <summary>
/// Represents an ERP operation.
/// </summary>
/// <typeparam name="T">The result type.</typeparam>
public class ErpOperation<T>
{
    private readonly Func<CancellationToken, Task<T>> _operation;
    private readonly TenantContext _tenantContext;
    private readonly TimeSpan? _timeout;

    /// <summary>
    /// Gets the tenant context.
    /// </summary>
    public TenantContext TenantContext => _tenantContext;

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan? Timeout => _timeout;

    internal ErpOperation(TenantContext tenantContext, Func<CancellationToken, Task<T>> operation, TimeSpan? timeout = null)
    {
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        _timeout = timeout;
    }

    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_timeout.HasValue)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout.Value);
            return await _operation(cts.Token);
        }
        return await _operation(cancellationToken);
    }
}