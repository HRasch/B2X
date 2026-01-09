// <copyright file="IErpConnectorRegistry.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System.Collections.Generic;
using B2X.Shared.Erp.Core.Models;

namespace B2X.Shared.Erp.Core.Interfaces;

/// <summary>
/// Registry for managing available ERP connector factories.
/// Central point for ERP connector discovery and instantiation (ADR-034).
/// </summary>
public interface IErpConnectorRegistry
{
    /// <summary>
    /// Registers an ERP adapter factory.
    /// </summary>
    /// <param name="factory">The factory to register.</param>
    void Register(IErpAdapterFactory factory);

    /// <summary>
    /// Gets a registered factory by ERP type.
    /// </summary>
    /// <param name="erpType">The ERP type identifier.</param>
    /// <returns>The factory if found, otherwise null.</returns>
    IErpAdapterFactory? GetFactory(string erpType);

    /// <summary>
    /// Gets all registered ERP adapter factories.
    /// </summary>
    /// <returns>Collection of all registered factories.</returns>
    IEnumerable<IErpAdapterFactory> GetAllFactories();

    /// <summary>
    /// Gets all supported ERP types.
    /// </summary>
    /// <returns>Collection of ERP type identifiers.</returns>
    IEnumerable<string> GetSupportedErpTypes();

    /// <summary>
    /// Gets connector metadata for all registered ERPs.
    /// </summary>
    /// <returns>Collection of connector metadata.</returns>
    IEnumerable<ErpConnectorMetadata> GetConnectorMetadata();

    /// <summary>
    /// Checks if an ERP type is supported.
    /// </summary>
    /// <param name="erpType">The ERP type to check.</param>
    /// <returns>True if supported, otherwise false.</returns>
    bool IsSupported(string erpType);
}
