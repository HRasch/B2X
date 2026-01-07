using System;
using System.Collections.Generic;
using B2Connect.ERP.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.ERP.Connectors;

/// <summary>
/// Factory for creating Fashop ERP connector instances.
/// </summary>
public class FashopErpAdapterFactory : IErpAdapterFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FashopErpAdapterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public string ErpType => "fashop";

    /// <inheritdoc />
    public IErpConnector CreateConnector(TenantContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Create connector using DI
        var connector = _serviceProvider.GetRequiredService<FashopErpConnector>();

        // Note: Actual initialization happens in the connector's InitializeAsync method
        // with the full ErpConfiguration

        return connector;
    }

    /// <inheritdoc />
    public ErpConfigurationSchema GetConfigurationSchema()
    {
        return new ErpConfigurationSchema
        {
            ErpType = "fashop",
            DisplayName = "enventa Fashop ERP",
            Description = "Retail-focused ERP system with POS integration and product variants",
            Version = "1.0.0",
            RequiredSettings = new Dictionary<string, ErpSettingDefinition>
            {
                ["connection.server"] = new ErpSettingDefinition
                {
                    Key = "connection.server",
                    DisplayName = "Server Address",
                    Type = "string",
                    Required = true,
                    Description = "Fashop server hostname or IP address"
                },
                ["connection.database"] = new ErpSettingDefinition
                {
                    Key = "connection.database",
                    DisplayName = "Database Name",
                    Type = "string",
                    Required = true,
                    Description = "Fashop database name"
                },
                ["connection.businessUnit"] = new ErpSettingDefinition
                {
                    Key = "connection.businessUnit",
                    DisplayName = "Business Unit",
                    Type = "string",
                    Required = true,
                    Description = "enventa BusinessUnit identifier"
                }
            },
            OptionalSettings = new Dictionary<string, ErpSettingDefinition>
            {
                ["connection.timeout"] = new ErpSettingDefinition
                {
                    Key = "connection.timeout",
                    DisplayName = "Connection Timeout (seconds)",
                    Type = "number",
                    Required = false,
                    DefaultValue = 30,
                    Description = "Connection timeout in seconds"
                },
                ["sync.batchSize"] = new ErpSettingDefinition
                {
                    Key = "sync.batchSize",
                    DisplayName = "Sync Batch Size",
                    Type = "number",
                    Required = false,
                    DefaultValue = 100,
                    Description = "Number of records to sync in each batch"
                }
            },
            AuthenticationTypes = new[]
            {
                new ErpAuthenticationType
                {
                    Type = "basic",
                    DisplayName = "Username/Password",
                    Description = "Standard username and password authentication"
                }
            }
        };
    }
}
