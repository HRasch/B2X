// <copyright file="IErpConnector.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using B2X.Shared.Erp.Core.Models;

namespace B2X.Shared.Erp.Core.Interfaces;

/// <summary>
/// Core ERP connector interface for pluggable ERP integrations.
/// Provides standardized operations across different ERP systems.
/// Implements ADR-034 Multi-ERP Connector Architecture.
/// </summary>
public interface IErpConnector
{
    /// <summary>
    /// Gets the unique identifier for this ERP system type (e.g., "enventa", "sap", "dynamics").
    /// </summary>
    string ErpType { get; }

    /// <summary>
    /// Gets the human-readable display name for the ERP system.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the ERP connector version information.
    /// </summary>
    ErpVersionInfo Version { get; }

    /// <summary>
    /// Initializes the connector with tenant-specific configuration.
    /// </summary>
    /// <param name="config">The ERP configuration for this tenant.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the initialization operation.</returns>
    Task InitializeAsync(ErpConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the capabilities supported by this ERP connector.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ERP capabilities.</returns>
    Task<ErpCapabilities> GetCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the connection to the ERP system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The connection test result.</returns>
    Task<ErpConnectionResult> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the catalog data from the ERP system.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the sync operation.</returns>
    Task SyncCatalogAsync(SyncContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an order in the ERP system.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the order creation.</returns>
    Task<ErpOrderResult> CreateOrderAsync(ErpOrder order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customer data from the ERP system.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer data.</returns>
    Task<ErpCustomerData> GetCustomerDataAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disposes resources used by the connector.
    /// </summary>
    /// <returns>A task representing the dispose operation.</returns>
    ValueTask DisposeAsync();
}
