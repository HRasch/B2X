// <copyright file="OrderEvents.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;

namespace B2Connect.ERP.Events;

/// <summary>
/// Event raised when orders are synced.
/// </summary>
public record OrdersSyncedEvent(string TenantId, DeltaSyncResponse<OrderDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when orders are batch written.
/// </summary>
public record OrdersBatchWrittenEvent(string TenantId, BatchWriteResponse<OrderDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when an order is accessed.
/// </summary>
public record OrderAccessedEvent(string TenantId, string OrderNumber, DateTime Timestamp);
