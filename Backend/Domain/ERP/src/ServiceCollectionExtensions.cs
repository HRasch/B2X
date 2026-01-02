// <copyright file="ServiceCollectionExtensions.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Infrastructure.Actor;
using B2Connect.ERP.Providers.Enventa;
using B2Connect.ERP.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // Register Actor pool as singleton (manages all tenant actors)
        services.TryAddSingleton<ErpActorPool>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
            return new ErpActorPool(loggerFactory, options.ActorQueueCapacity);
        });

        // Register provider manager
        services.TryAddSingleton<IProviderManager, ProviderManager>();

        // Register sync service for master data synchronization
        services.TryAddSingleton<ISyncService, SyncService>();

        return services;
    }

    /// <summary>
    /// Adds the enventa Trade ERP provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEnventaProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<IProviderFactory, EnventaProviderFactory>();
        return services;
    }

    /// <summary>
    /// Adds a custom provider factory.
    /// </summary>
    /// <typeparam name="TFactory">The provider factory type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCustomProviderFactory<TFactory>(this IServiceCollection services)
        where TFactory : class, IProviderFactory
    {
        services.AddSingleton<IProviderFactory, TFactory>();
        return services;
    }
}

/// <summary>
/// Configuration options for ERP integration.
/// </summary>
public sealed class ErpOptions
{
    /// <summary>
    /// Gets or sets the capacity of the operation queue for each Actor (default 1000).
    /// </summary>
    public int ActorQueueCapacity { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the default timeout for ERP operations (default 30 seconds).
    /// </summary>
    public TimeSpan DefaultOperationTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum concurrent providers (default 100).
    /// </summary>
    public int MaxConcurrentProviders { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to enable detailed operation logging (default false).
    /// </summary>
    public bool EnableDetailedLogging { get; set; }

    /// <summary>
    /// Gets or sets the retry configuration for failed operations.
    /// </summary>
    public RetryConfiguration RetryConfig { get; set; } = new();
}
