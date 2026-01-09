using System;
using System.Net.Http;
using B2X.ERP.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.ERP.Connectors.SAP;

/// <summary>
/// Factory for creating SAP ERP connector instances.
/// </summary>
public class SapErpAdapterFactory : IErpAdapterFactory
{
    /// <inheritdoc />
    public string ErpType => "sap";

    /// <inheritdoc />
    public IErpConnector CreateConnector(TenantContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Create service provider for this tenant's scope
        var services = new ServiceCollection();

        // Register HTTP client with tenant-specific configuration
        services.AddHttpClient<SapErpConnector>(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(5);
            // Additional SAP-specific headers can be configured here
        });

        // Register logging
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<SapErpConnector>();
    }

    /// <inheritdoc />
    public ErpConfigurationSchema GetConfigurationSchema()
    {
        return new ErpConfigurationSchema
        {
            ErpType = "sap",
            DisplayName = "SAP ERP/S4HANA",
            Description = "Enterprise resource planning system for large organizations with comprehensive business process support.",
            Version = "1.0.0",
            SupportedVersions = new[]
            {
                "SAP ECC 6.0",
                "S/4HANA 1610",
                "S/4HANA 1709",
                "S/4HANA 1809",
                "S/4HANA 1909",
                "S/4HANA 2020",
                "S/4HANA 2021",
                "S/4HANA 2022",
                "S/4HANA 2023"
            },
            RequiredSettings = new Dictionary<string, ErpSettingDefinition>
            {
                ["serviceUrl"] = new ErpSettingDefinition
                {
                    Key = "serviceUrl",
                    DisplayName = "SAP Service URL",
                    Type = "string",
                    Required = true,
                    Description = "Base URL for SAP OData services (e.g., https://sap-system.company.com)"
                },
                ["systemId"] = new ErpSettingDefinition
                {
                    Key = "systemId",
                    DisplayName = "SAP System ID",
                    Type = "string",
                    Required = true,
                    Description = "SAP system identifier (SID)"
                },
                ["client"] = new ErpSettingDefinition
                {
                    Key = "client",
                    DisplayName = "SAP Client",
                    Type = "string",
                    Required = true,
                    Description = "SAP client number (e.g., 100)"
                }
            },
            OptionalSettings = new Dictionary<string, ErpSettingDefinition>
            {
                ["companyCode"] = new ErpSettingDefinition
                {
                    Key = "companyCode",
                    DisplayName = "Company Code",
                    Type = "string",
                    Required = false,
                    DefaultValue = "1000",
                    Description = "Default SAP company code for operations"
                },
                ["salesOrg"] = new ErpSettingDefinition
                {
                    Key = "salesOrg",
                    DisplayName = "Sales Organization",
                    Type = "string",
                    Required = false,
                    DefaultValue = "1000",
                    Description = "Default sales organization for order processing"
                },
                ["batchSize"] = new ErpSettingDefinition
                {
                    Key = "batchSize",
                    DisplayName = "Batch Size",
                    Type = "number",
                    Required = false,
                    DefaultValue = 1000,
                    Description = "Maximum batch size for bulk operations"
                },
                ["timeout"] = new ErpSettingDefinition
                {
                    Key = "timeout",
                    DisplayName = "Timeout (seconds)",
                    Type = "number",
                    Required = false,
                    DefaultValue = 300,
                    Description = "Timeout for SAP API calls in seconds"
                }
            },
            AuthenticationTypes = new[]
            {
                new ErpAuthenticationType
                {
                    Type = "oauth2",
                    DisplayName = "OAuth 2.0",
                    Description = "Modern OAuth 2.0 authentication with SAP Authorization Server"
                },
                new ErpAuthenticationType
                {
                    Type = "basic",
                    DisplayName = "Basic Authentication",
                    Description = "Username/password authentication (use only in secure environments)"
                },
                new ErpAuthenticationType
                {
                    Type = "certificate",
                    DisplayName = "X.509 Certificate",
                    Description = "Mutual TLS authentication with client certificates"
                }
            }
        };
    }
}
