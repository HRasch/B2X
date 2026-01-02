// <copyright file="SyncService.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using Microsoft.Extensions.Logging;

namespace B2Connect.ERP.Services;

/// <summary>
/// Fake implementation of <see cref="ISyncService"/> for Mac development.
/// Real implementation will use gRPC to communicate with ERP provider containers.
/// </summary>
public sealed class SyncService : ISyncService
{
    private readonly ILogger<SyncService> _logger;
    private readonly Dictionary<string, DateTimeOffset> _watermarks = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public SyncService(ILogger<SyncService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<SyncResult> SyncArticlesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Syncing articles for tenant {TenantId} (Mac development)", tenant.TenantId);

        await Task.Delay(100, cancellationToken).ConfigureAwait(false); // Simulate work

        var syncId = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;

        // Update watermark
        _watermarks[$"{tenant.TenantId}:articles"] = now;

        return new SyncResult(
            syncId,
            SyncOperationState.Completed,
            1500, // Fake: 1500 articles processed
            0,    // No failures
            now,
            Array.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<SyncResult> SyncAttributesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        IEnumerable<string>? articleIds = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Syncing attributes for tenant {TenantId} (Mac development)", tenant.TenantId);

        await Task.Delay(150, cancellationToken).ConfigureAwait(false); // Simulate work

        var syncId = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;

        // Update watermark
        _watermarks[$"{tenant.TenantId}:attributes"] = now;

        return new SyncResult(
            syncId,
            SyncOperationState.Completed,
            30000, // Fake: 30,000 attributes processed
            0,     // No failures
            now,
            Array.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<SyncResult> SyncImagesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        IEnumerable<string>? articleIds = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Syncing images for tenant {TenantId} (Mac development)", tenant.TenantId);

        await Task.Delay(200, cancellationToken).ConfigureAwait(false); // Simulate work

        var syncId = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;

        // Update watermark
        _watermarks[$"{tenant.TenantId}:images"] = now;

        return new SyncResult(
            syncId,
            SyncOperationState.Completed,
            2500, // Fake: 2,500 images processed
            0,    // No failures
            now,
            Array.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<SyncResult> SyncCategoriesAsync(
        Core.TenantContext tenant,
        DateTimeOffset? since = null,
        bool includeTree = true,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Syncing categories for tenant {TenantId} (Mac development)", tenant.TenantId);

        await Task.Delay(50, cancellationToken).ConfigureAwait(false); // Simulate work

        var syncId = Guid.NewGuid().ToString();
        var now = DateTimeOffset.UtcNow;

        // Update watermark
        _watermarks[$"{tenant.TenantId}:categories"] = now;

        return new SyncResult(
            syncId,
            SyncOperationState.Completed,
            150, // Fake: 150 categories processed
            0,   // No failures
            now,
            Array.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<string> StartFullSyncAsync(
        Core.TenantContext tenant,
        SyncMode mode = SyncMode.Incremental,
        bool forceRestart = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Starting full sync for tenant {TenantId}, mode {Mode} (Mac development)",
            tenant.TenantId, mode);

        await Task.Delay(300, cancellationToken).ConfigureAwait(false); // Simulate work

        var syncId = Guid.NewGuid().ToString();
        return syncId;
    }

    /// <inheritdoc/>
    public async Task<SyncStatus> GetSyncStatusAsync(
        Core.TenantContext tenant,
        string? syncId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Getting sync status for tenant {TenantId} (Mac development)", tenant.TenantId);

        await Task.Delay(10, cancellationToken).ConfigureAwait(false); // Simulate work

        var id = syncId ?? Guid.NewGuid().ToString();
        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        return new SyncStatus(
            id,
            SyncOperationState.Completed,
            new SyncOperationProgress(32000, 32000, 0, "categories", "final"),
            startedAt,
            DateTimeOffset.UtcNow,
            null);
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset?> GetLastSyncWatermarkAsync(
        Core.TenantContext tenant,
        string entityType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Getting watermark for tenant {TenantId}, entity {EntityType} (Mac development)",
            tenant.TenantId, entityType);

        await Task.Delay(5, cancellationToken).ConfigureAwait(false); // Simulate work

        var key = $"{tenant.TenantId}:{entityType}";
        return _watermarks.TryGetValue(key, out var watermark) ? watermark : null;
    }

    /// <inheritdoc/>
    public async Task UpdateSyncWatermarkAsync(
        Core.TenantContext tenant,
        string entityType,
        DateTimeOffset watermark,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("FAKE: Updating watermark for tenant {TenantId}, entity {EntityType} (Mac development)",
            tenant.TenantId, entityType);

        await Task.Delay(5, cancellationToken).ConfigureAwait(false); // Simulate work

        var key = $"{tenant.TenantId}:{entityType}";
        _watermarks[key] = watermark;
    }
}