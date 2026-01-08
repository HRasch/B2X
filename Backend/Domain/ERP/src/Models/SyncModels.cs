// <copyright file="SyncModels.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Models;

/// <summary>
/// Product change event for synchronization.
/// </summary>
public sealed record ProductChangeEvent
{
    public required string ProductId { get; init; }
    public required ChangeType ChangeType { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
    public string? ChangedBy { get; init; }
    public IReadOnlyList<string>? ChangedFields { get; init; }
    public PimProduct? Product { get; init; }
}

/// <summary>
/// Customer change event for synchronization.
/// </summary>
public sealed record CustomerChangeEvent
{
    public required string CustomerId { get; init; }
    public required ChangeType ChangeType { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
    public string? ChangedBy { get; init; }
    public IReadOnlyList<string>? ChangedFields { get; init; }
    public CrmCustomer? Customer { get; init; }
}

/// <summary>
/// Stock change event.
/// </summary>
public sealed record StockChangeEvent
{
    public required string ProductId { get; init; }
    public int OldQuantity { get; init; }
    public int NewQuantity { get; init; }
    public int Delta => NewQuantity - OldQuantity;
    public DateTimeOffset ChangedAt { get; init; }
    public string? WarehouseId { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Price change event.
/// </summary>
public sealed record PriceChangeEvent
{
    public required string ProductId { get; init; }
    public string? CustomerId { get; init; }
    public decimal OldPrice { get; init; }
    public decimal NewPrice { get; init; }
    public required string Currency { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
    public DateTimeOffset? EffectiveFrom { get; init; }
    public string? PriceListId { get; init; }
}

/// <summary>
/// Type of change.
/// </summary>
public enum ChangeType
{
    Created,
    Updated,
    Deleted,
    Restored
}

/// <summary>
/// Entity type for synchronization.
/// </summary>
public enum SyncEntityType
{
    Product,
    Category,
    Customer,
    Contact,
    Pricing,
    Stock,
    Order
}

/// <summary>
/// Synchronization response.
/// </summary>
public sealed record SyncResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required bool HasMore { get; init; }
    public string? NextChangeToken { get; init; }
    public DateTimeOffset SyncTimestamp { get; init; }
    public int? RemainingItems { get; init; }
}

/// <summary>
/// Full sync progress tracking.
/// </summary>
public sealed record SyncProgress
{
    public required Guid SyncId { get; init; }
    public required SyncEntityType EntityType { get; init; }
    public required SyncStatus Status { get; init; }
    public int TotalItems { get; init; }
    public int ProcessedItems { get; init; }
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public double ProgressPercent => TotalItems > 0 ? (ProcessedItems * 100.0 / TotalItems) : 0;
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<SyncError>? Errors { get; init; }
}

/// <summary>
/// Sync status.
/// </summary>
public enum SyncStatus
{
    Pending,
    Running,
    Completed,
    CompletedWithErrors,
    Failed,
    Cancelled
}

/// <summary>
/// Sync error details.
/// </summary>
public sealed record SyncError
{
    public required string EntityId { get; init; }
    public required string ErrorCode { get; init; }
    public required string Message { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public bool IsRetryable { get; init; }
    public int RetryCount { get; init; }
}
