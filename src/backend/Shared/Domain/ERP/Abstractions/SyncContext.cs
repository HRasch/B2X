// <copyright file="SyncContext.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents the context for a data synchronization operation.
/// </summary>
public class SyncContext
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last sync timestamp.
    /// </summary>
    public DateTimeOffset? LastSyncTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the batch size for synchronization.
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether this is a full sync or incremental.
    /// </summary>
    public bool FullSync { get; set; }

    /// <summary>
    /// Gets or sets additional sync parameters.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}