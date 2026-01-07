// <copyright file="OrderQueries.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Queries;

/// <summary>
/// Query to get a single order.
/// </summary>
public record GetOrderQuery(string TenantId, string OrderNumber);

/// <summary>
/// Query to get orders by IDs.
/// </summary>
public record GetOrdersByIdsQuery(string TenantId, IEnumerable<string> OrderNumbers);

/// <summary>
/// Query to get orders with cursor pagination.
/// </summary>
public record QueryOrdersQuery(string TenantId, QueryRequest Query);
