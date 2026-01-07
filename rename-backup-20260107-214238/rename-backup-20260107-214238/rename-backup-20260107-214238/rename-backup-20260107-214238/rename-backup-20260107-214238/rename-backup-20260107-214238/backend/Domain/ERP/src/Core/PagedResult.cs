// <copyright file="PagedResult.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Models;

namespace B2Connect.ERP.Core;

/// <summary>
/// Represents a paged result for ERP operations.
/// Used for .NET Framework 4.8 compatibility instead of IAsyncEnumerable.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed record PagedResult<T>
{
    public PagedResult(
        IReadOnlyList<T> items,
        string? continuationToken,
        int totalCount = -1)
    {
        Items = items;
        ContinuationToken = continuationToken;
        TotalCount = totalCount;
        HasMore = !string.IsNullOrEmpty(continuationToken);
    }

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the continuation token for the next page.
    /// Null if no more pages.
    /// </summary>
    public string? ContinuationToken { get; }

    /// <summary>
    /// Gets the total count of items (if known, -1 otherwise).
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets a value indicating whether there are more pages.
    /// </summary>
    public bool HasMore { get; }

    /// <summary>
    /// Gets the count of items in the current page.
    /// </summary>
    public int PageCount => Items.Count;

    /// <summary>
    /// Creates an empty result with no items.
    /// </summary>
    public static PagedResult<T> Empty() => new(Array.Empty<T>(), null, 0);
}

/// <summary>
/// Result of a batch/bulk operation.
/// </summary>
public sealed record BatchResult
{
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public int TotalCount => SuccessCount + FailureCount;
    public IReadOnlyList<BatchError> Errors { get; init; } = Array.Empty<BatchError>();
    public TimeSpan Duration { get; init; }
    public bool IsFullySuccessful => FailureCount == 0;
}

/// <summary>
/// Represents an error for a single item in a batch operation.
/// </summary>
public sealed record BatchError(string ItemId, string Code, string Message);

/// <summary>
/// Result of a synchronization operation.
/// </summary>
public sealed record SyncResult
{
    public int Created { get; init; }
    public int Updated { get; init; }
    public int Deleted { get; init; }
    public int Skipped { get; init; }
    public int Failed { get; init; }
    public int TotalProcessed => Created + Updated + Deleted + Skipped + Failed;
    public IReadOnlyList<Models.SyncError> Errors { get; init; } = Array.Empty<Models.SyncError>();
    public TimeSpan Duration { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
}
