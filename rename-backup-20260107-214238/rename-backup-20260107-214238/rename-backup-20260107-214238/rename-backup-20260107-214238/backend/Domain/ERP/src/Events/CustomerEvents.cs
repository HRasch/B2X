// <copyright file="CustomerEvents.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Events;

/// <summary>
/// Event raised when customers are synced.
/// </summary>
public record CustomersSyncedEvent(string TenantId, DeltaSyncResponse<CustomerDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when customers are batch written.
/// </summary>
public record CustomersBatchWrittenEvent(string TenantId, BatchWriteResponse<CustomerDto> Response, DateTime Timestamp);

/// <summary>
/// Event raised when a customer is accessed.
/// </summary>
public record CustomerAccessedEvent(string TenantId, string CustomerNumber, DateTime Timestamp);
