// <copyright file="CustomerCommands.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Abstractions;
using B2X.ERP.Abstractions.Http;

namespace B2X.ERP.Commands;

/// <summary>
/// Command to get a single customer.
/// </summary>
public record GetCustomerCommand(string TenantId, string CustomerNumber);

/// <summary>
/// Command to query customers with filtering.
/// </summary>
public record QueryCustomersCommand(string TenantId, QueryRequest Query);

/// <summary>
/// Command to sync customers with delta changes.
/// </summary>
public record SyncCustomersCommand(string TenantId, DeltaSyncRequest Request);

/// <summary>
/// Command to batch write customers.
/// </summary>
public record BatchWriteCustomersCommand(string TenantId, IEnumerable<CustomerDto> Customers);
