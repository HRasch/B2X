// <copyright file="ServiceCollectionExtensions.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Abstractions;
using B2Connect.ERP.Abstractions.Http;
using B2Connect.ERP.Infrastructure.DataAccess;
using B2Connect.Domain.ERP.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;

namespace B2Connect.ERP;

/// <summary>
/// Extension methods for registering ERP services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ERP integration services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddErpIntegration(
        this IServiceCollection services,
        Action<ErpOptions>? configure = null)
    {
        // Configure options
        var options = new ErpOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        // Register HTTP client for ERP communication
        services.TryAddSingleton<IErpClient, ErpHttpClient>();

        // Register ERP service
        services.TryAddSingleton<IErpService, ErpService>();

        return services;
    }

    /// <summary>
    /// Adds Wolverine CQRS integration for ERP operations.
    /// Provides command/query separation while using existing ErpService.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional Wolverine configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddErpWolverineIntegration(
        this IServiceCollection services,
        Action<WolverineOptions>? configure = null)
    {
        // Add Wolverine with default configuration
        services.AddWolverine(opts =>
        {
            // Configure message routing for ERP operations
            opts.Policies.AutoApplyTransactions();

            // Apply custom configuration if provided
            configure?.Invoke(opts);
        });

        return services;
    }

    /// <summary>
    /// Adds data access services for hybrid EF Core + Dapper operations (ADR-025).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.TryAddSingleton<IDapperConnectionFactory>(
            new PostgresDapperConnectionFactory(connectionString));
        return services;
    }
}

/// <summary>
/// Configuration options for ERP integration.
/// </summary>
public sealed class ErpOptions
{
    /// <summary>
    /// Gets or sets the base URL for the ERP Connector HTTP service.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the default timeout for ERP operations (default 30 seconds).
    /// </summary>
    public TimeSpan DefaultOperationTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to enable detailed operation logging (default false).
    /// </summary>
    public bool EnableDetailedLogging { get; set; }
}
