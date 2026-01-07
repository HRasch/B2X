// <copyright file="OrderEvents.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Events;

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
