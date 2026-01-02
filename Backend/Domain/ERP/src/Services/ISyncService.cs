// <copyright file="ISyncService.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Grpc;

namespace B2Connect.ERP.Services;

/// <summary>
/// Service for synchronizing master data from ERP systems.
/// Implements hybrid strategy: sync master data + live customer queries.
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Synchronizes articles incrementally since last sync.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="since">Sync watermark (null for full sync).</param>
    /// <param name="batchSize">Batch size for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync result with statistics.</returns>
    Task<SyncResult> SyncArticlesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes article attributes incrementally.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="since">Sync watermark.</param>
    /// <param name="articleIds">Optional: sync attributes for specific articles.</param>
    /// <param name="batchSize">Batch size for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync result with statistics.</returns>
    Task<SyncResult> SyncAttributesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        IEnumerable<string>? articleIds = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes images and media assets.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="since">Sync watermark.</param>
    /// <param name="articleIds">Optional: sync images for specific articles.</param>
    /// <param name="batchSize">Batch size for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync result with statistics.</returns>
    Task<SyncResult> SyncImagesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        IEnumerable<string>? articleIds = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes category hierarchy.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="since">Sync watermark.</param>
    /// <param name="includeTree">Include full category tree.</param>
    /// <param name="batchSize">Batch size for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync result with statistics.</returns>
    Task<SyncResult> SyncCategoriesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        bool includeTree = true,
        int batchSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts full synchronization (staged: Articles → Attributes → Images → Categories).
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="mode">Sync mode (full or incremental).</param>
    /// <param name="forceRestart">Force restart if already running.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync operation ID for status tracking.</returns>
    Task<string> StartFullSyncAsync(
        Core.TenantContext tenant,
        SyncMode mode = SyncMode.Incremental,
        bool forceRestart = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a sync operation.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="syncId">Sync operation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sync status with progress information.</returns>
    Task<SyncStatus> GetSyncStatusAsync(
        Core.TenantContext tenant,
        string? syncId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last sync watermark for an entity type.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="entityType">Entity type ("articles", "attributes", etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Last sync timestamp.</returns>
    Task<DateTimeOffset?> GetLastSyncWatermarkAsync(
        Core.TenantContext tenant,
        string entityType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the sync watermark for an entity type.
    /// </summary>
    /// <param name="tenant">Tenant context.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="watermark">New watermark timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateSyncWatermarkAsync(
        Core.TenantContext tenant,
        string entityType,
        DateTimeOffset watermark,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Sync operation mode.
/// </summary>
public enum SyncMode
{
    /// <summary>
    /// Full synchronization (complete dataset).
    /// </summary>
    Full,

    /// <summary>
    /// Incremental synchronization (only changed records).
    /// </summary>
    Incremental
}

/// <summary>
/// Result of a sync operation.
/// </summary>
public record SyncResult(
    string SyncId,
    SyncOperationState State,
    long RecordsProcessed,
    long RecordsFailed,
    DateTimeOffset? WatermarkUpdated,
    IReadOnlyList<string> Errors);

/// <summary>
/// Status of a sync operation.
/// </summary>
public record SyncStatus(
    string SyncId,
    SyncOperationState State,
    SyncOperationProgress Progress,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string? ErrorMessage);

/// <summary>
/// Sync operation state.
/// </summary>
public enum SyncOperationState
{
    /// <summary>
    /// Operation is unspecified.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Operation is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Operation is running.
    /// </summary>
    Running,

    /// <summary>
    /// Operation completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Operation failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Progress information for a sync operation.
/// </summary>
public record SyncOperationProgress(
    long TotalRecords,
    long ProcessedRecords,
    long FailedRecords,
    string CurrentEntity,
    string CurrentBatch);
