// <copyright file="OrderCommands.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Commands;

/// <summary>
/// Command to get a single order.
/// </summary>
public record GetOrderCommand(string TenantId, string OrderNumber);

/// <summary>
/// Command to query orders with filtering.
/// </summary>
public record QueryOrdersCommand(string TenantId, QueryRequest Query);

/// <summary>
/// Command to sync orders with delta changes.
/// </summary>
public record SyncOrdersCommand(string TenantId, DeltaSyncRequest Request);

/// <summary>
/// Command to batch write orders.
/// </summary>
public record BatchWriteOrdersCommand(string TenantId, IEnumerable<OrderDto> Orders);
