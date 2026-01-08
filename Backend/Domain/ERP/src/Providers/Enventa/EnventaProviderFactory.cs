// <copyright file="EnventaProviderFactory.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Contracts;
using B2X.ERP.Core;
using B2X.ERP.Infrastructure.Actor;
using B2X.ERP.Models;
using B2X.ERP.Providers.Fake;
using B2X.ERP.Services;
using Microsoft.Extensions.Logging;

namespace B2X.ERP.Providers.Enventa;

/// <summary>
/// Factory for creating enventa Trade ERP provider instances.
/// For Mac development, creates fake implementations.
/// Real implementation will be built on Windows with .NET Framework 4.8.
/// </summary>
public sealed class EnventaProviderFactory : IProviderFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<EnventaProviderFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnventaProviderFactory"/> class.
    /// </summary>
    /// <param name="loggerFactory">Logger factory.</param>
    public EnventaProviderFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<EnventaProviderFactory>();
    }

    /// <inheritdoc/>
    public async Task<ProviderInstance> CreateProviderAsync(
        TenantContext tenant,
        ProviderConfiguration configuration,
        ErpActor actor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(actor);

        _logger.LogInformation(
            "Creating FAKE enventa provider for tenant {TenantId} (Mac development)",
            tenant.TenantId);

        // Use fake implementations for Mac development
        // Real implementation will be built on Windows with .NET Framework 4.8
        var fakeErpProvider = new FakeErpProvider();
        var fakePimProvider = new FakePimProvider();
        var fakeCrmProvider = new FakeCrmProvider();

        await Task.CompletedTask.ConfigureAwait(false);

        return new ProviderInstance
        {
            Configuration = configuration,
            PimProvider = fakePimProvider,
            CrmProvider = fakeCrmProvider,
            ErpProvider = fakeErpProvider,
            ConnectionState = ProviderConnectionState.Connected,
            LastActivity = DateTimeOffset.UtcNow
        };
    }

    /// <inheritdoc/>
    public string ProviderType => "enventa-fake";

    /// <inheritdoc/>
    public bool SupportsCapability(ProviderCapability capability)
    {
        return capability switch
        {
            ProviderCapability.Products => true,
            ProviderCapability.Customers => true,
            ProviderCapability.Orders => true,
            ProviderCapability.Pricing => true,
            ProviderCapability.Stock => true,
            ProviderCapability.Categories => true,
            ProviderCapability.Contacts => true,
            ProviderCapability.Activities => true,
            ProviderCapability.Streaming => true,
            ProviderCapability.BatchOperations => true,
            _ => false
        };
    }
}
