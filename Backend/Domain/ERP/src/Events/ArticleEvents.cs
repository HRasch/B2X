// <copyright file="ArticleEvents.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;

namespace B2Connect.ERP.Events;

/// <summary>
/// Event raised when articles are synced.
/// </summary>
public record ArticlesSyncedEvent(string TenantId, DeltaSyncResponse<ArticleDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when articles are batch written.
/// </summary>
public record ArticlesBatchWrittenEvent(string TenantId, BatchWriteResponse<ArticleDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when an article is accessed.
/// </summary>
public record ArticleAccessedEvent(string TenantId, string ArticleId, DateTime Timestamp);