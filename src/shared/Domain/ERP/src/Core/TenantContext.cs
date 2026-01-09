// <copyright file="TenantContext.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

namespace B2X.ERP.Core;

/// <summary>
/// Contains tenant-specific context for ERP operations.
/// All ERP provider operations require a valid tenant context.
/// </summary>
public sealed record TenantContext
{
    /// <summary>
    /// Gets the unique identifier for the tenant.
    /// </summary>
    public required string TenantId { get; init; }

    /// <summary>
    /// Gets the tenant name for display purposes.
    /// </summary>
    public required string TenantName { get; init; }

    /// <summary>
    /// Gets the correlation ID for request tracing.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets the user ID making the request.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets the timestamp when the context was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets additional metadata for the request.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
