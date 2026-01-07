// <copyright file="ErpConnectionFactory.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using NVShop.Data.NV.Model;

namespace B2X.Domain.ERP.Infrastructure;

/// <summary>
/// Factory for creating ERP connections.
/// In the HTTP architecture, this factory creates HTTP client connections
/// rather than direct database connections. Direct database access is handled
/// by the ERP Connector (.NET Framework 4.8) which exposes HTTP endpoints.
/// </summary>
public class ErpConnectionFactory : IErpConnectionFactory
{
    private readonly ILogger<ErpConnectionFactory> _logger;
    private readonly ErpConnectionOptions _options;

    public ErpConnectionFactory(
        ILogger<ErpConnectionFactory> logger,
        ErpConnectionOptions? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? new ErpConnectionOptions();
    }

    public Task<IErpConnection> CreateConnectionAsync(string tenantId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(tenantId))
        {
            throw new ArgumentException("Tenant ID cannot be null or empty", nameof(tenantId));
        }

        // In the HTTP architecture, direct database connections are not created here.
        // The ERP Connector (.NET Framework 4.8) handles database access and exposes HTTP endpoints.
        // This factory is kept for interface compatibility and may be used for HTTP client setup.
        _logger.LogWarning(
            "CreateConnectionAsync called for tenant {TenantId}. Direct connections are handled by ERP Connector.",
            tenantId);

        throw new NotImplementedException(
            "Direct ERP connections are not supported. Use IErpClient for HTTP communication with the ERP Connector.");
    }
}

/// <summary>
/// Options for ERP connection factory.
/// </summary>
public class ErpConnectionOptions
{
    public string? DefaultConnectionString { get; set; }
    public int DefaultCommandTimeout { get; set; } = 30;
    public bool EnablePooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int MinPoolSize { get; set; } = 5;
    public TimeSpan ConnectionLifetime { get; set; } = TimeSpan.FromMinutes(5);
}

/// <summary>
/// Tenant-specific ERP configuration.
/// </summary>
public class TenantErpConfiguration
{
    public string TenantId { get; set; } = string.Empty;
    public string? ConnectionString { get; set; }
    public int? CommandTimeout { get; set; }
    public bool EnablePooling { get; set; } = true;
}
