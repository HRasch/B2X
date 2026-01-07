using System;
using System.Collections.Generic;
using System.Linq;
using B2Connect.ERP.Abstractions;

namespace B2Connect.ERP.Connectors;

/// <summary>
/// Registry for managing ERP connector factories.
/// </summary>
public class ErpConnectorRegistry : IErpConnectorRegistry
{
    private readonly IEnumerable<IErpAdapterFactory> _factories;

    public ErpConnectorRegistry(IEnumerable<IErpAdapterFactory> factories)
    {
        _factories = factories ?? throw new ArgumentNullException(nameof(factories));
    }

    /// <inheritdoc />
    public IEnumerable<IErpAdapterFactory> GetAllFactories()
    {
        return _factories;
    }

    /// <inheritdoc />
    public IErpAdapterFactory GetFactory(string erpType)
    {
        if (string.IsNullOrEmpty(erpType))
        {
            throw new ArgumentNullException(nameof(erpType));
        }

        var factory = _factories.FirstOrDefault(f =>
            f.ErpType.Equals(erpType, StringComparison.OrdinalIgnoreCase));

        if (factory == null)
        {
            throw new InvalidOperationException($"No connector factory registered for ERP type '{erpType}'. " +
                $"Available types: {string.Join(", ", _factories.Select(f => f.ErpType))}");
        }

        return factory;
    }

    /// <inheritdoc />
    public bool IsRegistered(string erpType)
    {
        if (string.IsNullOrEmpty(erpType))
        {
            return false;
        }

        return _factories.Any(f => f.ErpType.Equals(erpType, StringComparison.OrdinalIgnoreCase));
    }
}
