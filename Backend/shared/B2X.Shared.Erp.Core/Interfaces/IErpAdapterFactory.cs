// <copyright file="IErpAdapterFactory.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using B2X.Shared.Erp.Core.Models;

namespace B2X.Shared.Erp.Core.Interfaces;

/// <summary>
/// Factory interface for creating ERP connector instances.
/// Used by the pluggable ERP connector framework (ADR-034).
/// </summary>
public interface IErpAdapterFactory
{
    /// <summary>
    /// Gets the ERP type identifier (e.g., "enventa", "sap", "dynamics").
    /// </summary>
    string ErpType { get; }

    /// <summary>
    /// Gets the display name for this ERP adapter.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the version of this adapter factory.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Creates a new ERP connector instance for the given tenant context.
    /// </summary>
    /// <param name="context">The tenant context.</param>
    /// <returns>A new ERP connector instance.</returns>
    IErpConnector CreateConnector(TenantContext context);

    /// <summary>
    /// Gets the configuration schema required by this ERP adapter.
    /// </summary>
    /// <returns>The configuration schema.</returns>
    ErpConfigurationSchema GetConfigurationSchema();

    /// <summary>
    /// Gets the supported features and capabilities of this adapter.
    /// </summary>
    /// <returns>The supported features.</returns>
    ErpFeatures GetSupportedFeatures();
}
