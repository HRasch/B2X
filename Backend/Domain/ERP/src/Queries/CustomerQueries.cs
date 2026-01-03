// <copyright file="CustomerQueries.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;

namespace B2Connect.ERP.Queries;

/// <summary>
/// Query to get a single customer.
/// </summary>
public record GetCustomerQuery(string TenantId, string CustomerNumber);

/// <summary>
/// Query to get customers by IDs.
/// </summary>
public record GetCustomersByIdsQuery(string TenantId, IEnumerable<string> CustomerNumbers);

/// <summary>
/// Query to get customers with cursor pagination.
/// </summary>
public record QueryCustomersQuery(string TenantId, QueryRequest Query);