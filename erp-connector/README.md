# B2Connect ERP Connector Framework

**Pluggable ERP Integration Framework** for B2Connect multi-tenant SaaS platform. Supports extensible ERP connectors with standardized interfaces, secure distribution, and tenant self-service management.

## Overview

The B2Connect ERP Connector Framework provides a pluggable architecture for integrating with multiple ERP systems. Built on ADR-034 Multi-ERP Connector Architecture, it enables seamless addition of new ERP connectors without modifying the core framework.

### Key Features

- **🔌 Pluggable Architecture**: Add new ERP systems via standardized adapters
- **🏢 Multi-Tenant Ready**: Complete tenant isolation and secure configuration
- **📦 Self-Service Downloads**: Tenant-admins can download and configure connectors independently
- **🔒 Enterprise Security**: End-to-end encryption, audit logging, and compliance
- **⚡ Performance Optimized**: Connection pooling, caching, and async operations
- **🛠️ Developer Friendly**: Standardized interfaces and development kits
- **🤖 AI-Powered Processing**: Local AI integration with Ollama and LMStudio for intelligent data validation and correction

### AI Processing Capabilities

The ERP Connector Framework includes built-in AI processing capabilities for tenant data:

- **🔍 Intelligent Data Validation**: AI-powered validation of ERP data quality and consistency
- **🔧 Automatic Data Correction**: AI suggestions for fixing data quality issues
- **📊 Smart Data Processing**: General AI processing for data transformation and enrichment
- **🏠 Local AI Support**: Uses client-side Ollama or LMStudio for privacy and performance
- **🏢 Tenant Isolation**: All AI processing respects tenant boundaries and data isolation

#### Supported AI Providers

| Provider | Type | Endpoint | Features |
|----------|------|----------|----------|
| **Ollama** | Local | `http://localhost:11434` | DeepSeek Coder, Qwen models, full local control |
| **LMStudio** | Local | `http://localhost:1234` | OpenAI-compatible API, model switching |

#### AI Configuration

```xml
<!-- App.config -->
<add key="AI:Enabled" value="true" />
<add key="AI:PreferredProvider" value="ollama" />
<add key="AI:Ollama:Endpoint" value="http://localhost:11434" />
<add key="AI:LMStudio:Endpoint" value="http://localhost:1234" />
```

### Supported ERP Systems

| ERP System | Status | Framework | Capabilities |
|------------|--------|-----------|--------------|
| **enventa Trade ERP** | ✅ Production Ready | .NET Framework 4.8 | Catalog Sync, Orders, Customers |
| **enventa Fashop ERP** | ✅ Phase 4 Complete | .NET Framework 4.8 | Retail Catalog, POS Orders, Customer Profiles |
| **SAP ERP/S4HANA** | 🚧 Planned | .NET 8.0 | Full Integration Suite |
| **Microsoft Dynamics** | 🚧 Planned | .NET 8.0 | Modern APIs |
| **Oracle E-Business Suite** | 🚧 Planned | .NET 8.0 | Legacy Integration |
| **Craft Software Punchout** | ✅ Available | .NET 8.0 | Multi-Format Support |

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                 ERP Connector Framework                     │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              Core Framework Services               │    │
│  │  • IErpConnector Interface                         │    │
│  │  • Authentication & Multi-Tenancy                  │    │
│  │  • Audit Logging & Health Monitoring               │    │
│  │  • Secure Download & Version Control               │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐           │
│  │ enventa     │ │   SAP       │ │  Oracle     │           │
│  │ Connector   │ │ Connector   │ │ Connector   │ ...       │
│  │ (.NET 4.8)  │ │ (.NET 8.0)  │ │ (.NET 8.0) │           │
│  └─────────────┘ └─────────────┘ └─────────────┘           │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼ HTTP/HTTPS (Tenant-Isolated)
┌─────────────────────────────────────────────────────────────┐
│                 B2Connect Core Services                      │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐            │
│  │   Catalog   │ │   Orders    │ │   Search    │            │
│  │  Service    │ │  Service    │ │  Service    │            │
│  └─────────────┘ └─────────────┘ └─────────────┘            │
└─────────────────────────────────────────────────────────────┘
```

## IErpConnector Interface

All ERP connectors implement the standardized `IErpConnector` interface for consistent integration:

```csharp
public interface IErpConnector
{
    // Initialize connector with tenant-specific configuration
    Task InitializeAsync(ErpConfiguration config);

    // Get ERP capabilities and supported features
    Task<ErpCapabilities> GetCapabilitiesAsync();

    // Synchronize catalog data (articles/products)
    Task SyncCatalogAsync(SyncContext context);

    // Create orders in the ERP system
    Task<ErpOrderResult> CreateOrderAsync(ErpOrder order);

    // Retrieve customer data
    Task<ErpCustomerData> GetCustomerDataAsync(string customerId);
}
```

### ErpCapabilities Structure

```csharp
public class ErpCapabilities
{
    public bool SupportsCatalogSync { get; set; }
    public bool SupportsOrderCreation { get; set; }
    public bool SupportsRealTimeQueries { get; set; }
    public bool SupportsCustomerData { get; set; }
    public bool RequiresConnectionPooling { get; set; }
    public string[] SupportedDataFormats { get; set; }
}
```

### ErpConfiguration Structure

```csharp
public class ErpConfiguration
{
    public string TenantId { get; set; }
    public string ErpType { get; set; }
    public Dictionary<string, string> ConnectionSettings { get; set; }
    public Dictionary<string, string> AuthenticationSettings { get; set; }
    public ErpSyncSettings SyncSettings { get; set; }
}
```

## Setup Instructions

### For Tenant Administrators

#### 1. Discover Supported ERPs

```bash
# List all available ERP connectors
b2connect erp list-supported

# Output example:
Available ERP Connectors:
- enventa (v2.1.0) - enventa Trade ERP (.NET Framework 4.8)
- craft-punchout (v1.0.0) - Craft Software Punchout (.NET 8.0)
- sap (v1.0.0) - SAP ERP/S4HANA (.NET 8.0) [Coming Soon]
```

#### 2. Download Connector

```bash
# Download the enventa connector
b2connect erp download-connector --erp-type enventa --version latest

# Verify download
b2connect erp list-installed
```

#### 3. Configure Connector

```bash
# Interactive configuration wizard
b2connect erp configure-connector --erp-type enventa --interactive

# Or configure with specific settings
b2connect erp configure-connector --erp-type enventa \
  --connection-string "Server=ERP-SERVER;Database=ENVENTA;Integrated Security=True" \
  --license-server "LICENSE-SERVER:1234" \
  --base-path "C:\enventa\base"
```

#### 4. Test Connection

```bash
# Test ERP connectivity
b2connect erp test-connection --erp-type enventa

# Check connector status
b2connect erp status-connector --erp-type enventa
```

#### 5. Start Services

```bash
# Start the ERP connector service
b2connect erp start-connector --erp-type enventa

# Enable automatic startup
b2connect erp enable-autostart --erp-type enventa
```

### For enventa Trade ERP

**Prerequisites:**
- Windows Server 2016+ or Windows 10/11
- .NET Framework 4.8 Runtime
- enventa Trade ERP installed and licensed
- Network access to ERP database and license server

**Configuration Parameters:**
- `ConnectionString`: Database connection string
- `LicenseServer`: License server address (host:port)
- `BasePath`: enventa installation base path
- `BusinessUnit`: Tenant-specific business unit
- `MaxConnections`: Connection pool size (default: 5)

**Example Configuration:**
```json
{
  "erpType": "enventa",
  "connectionSettings": {
    "ConnectionString": "Server=ERP-PROD;Database=ENVENTA;Integrated Security=True",
    "LicenseServer": "license.enventa.local:1234",
    "BasePath": "C:\\enventa\\trade\\base"
  },
  "syncSettings": {
    "BatchSize": 1000,
    "MaxConcurrentOperations": 1,
    "SyncIntervalMinutes": 15
  }
}
```

### For Future ERP Systems

Setup follows the same pattern for all supported ERPs:

```bash
# Generic setup workflow
b2connect erp download-connector --erp-type <erp-type>
b2connect erp configure-connector --erp-type <erp-type> --interactive
b2connect erp test-connection --erp-type <erp-type>
b2connect erp start-connector --erp-type <erp-type>
```

## Development Guide for New Connectors

### 1. Create Connector Project

```bash
# Use the connector template
dotnet new b2connect-erp-connector -n MyErpConnector
cd MyErpConnector
```

### 2. Implement IErpConnector

```csharp
public class MyErpConnector : IErpConnector
{
    public async Task InitializeAsync(ErpConfiguration config)
    {
        // Validate configuration
        // Establish connections
        // Initialize caches
    }

    public async Task<ErpCapabilities> GetCapabilitiesAsync()
    {
        return new ErpCapabilities
        {
            SupportsCatalogSync = true,
            SupportsOrderCreation = true,
            SupportsRealTimeQueries = false,
            RequiresConnectionPooling = true
        };
    }

    public async Task SyncCatalogAsync(SyncContext context)
    {
        // Implement catalog synchronization
        // Use context.TenantId for isolation
        // Report progress via context.ProgressCallback
    }

    public async Task<ErpOrderResult> CreateOrderAsync(ErpOrder order)
    {
        // Create order in ERP
        // Return result with ERP order number
    }

    public async Task<ErpCustomerData> GetCustomerDataAsync(string customerId)
    {
        // Retrieve customer data
    }
}
```

### 3. Implement IErpAdapterFactory

```csharp
public class MyErpAdapterFactory : IErpAdapterFactory
{
    public string ErpType => "myerp";

    public IErpConnector CreateConnector(TenantContext context)
    {
        return new MyErpConnector();
    }

    public ErpConfigurationSchema GetConfigurationSchema()
    {
        return new ErpConfigurationSchema
        {
            RequiredSettings = new[] { "ApiUrl", "ApiKey" },
            OptionalSettings = new[] { "TimeoutSeconds" },
            AuthenticationMethods = new[] { "ApiKey", "OAuth2" }
        };
    }
}
```

### 4. Register Connector

```csharp
// In connector startup
services.AddErpConnector<MyErpAdapterFactory>();
```

### 5. Testing

```csharp
// Use mock framework for testing
var mockConnector = new MockErpConnector();
var result = await mockConnector.SyncCatalogAsync(context);
Assert.IsTrue(result.Success);
```

### 6. Packaging and Distribution

```xml
<!-- Connector.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <PackageId>B2Connect.Erp.MyErpConnector</PackageId>
    <Version>1.0.0</Version>
  </PropertyGroup>
</Project>
```

## Migration Notes

### From Legacy enventa Connector

The pluggable framework replaces the monolithic enventa-specific connector with a standardized, extensible architecture.

#### Key Changes

**Architecture:**
- ✅ **Before**: Single-purpose enventa connector
- ✅ **After**: Pluggable framework with enventa adapter

**Setup:**
- ✅ **Before**: Manual installation and configuration
- ✅ **After**: Self-service CLI commands with guided wizards

**Multi-Tenancy:**
- ✅ **Before**: Limited tenant isolation
- ✅ **After**: Complete tenant separation with secure contexts

**Extensibility:**
- ✅ **Before**: Adding new ERPs required framework changes
- ✅ **After**: New ERPs added via standardized adapters

#### Migration Steps

1. **Backup Current Configuration**
   ```bash
   # Export current settings
   b2connect erp export-config --erp-type enventa > enventa-config-backup.json
   ```

2. **Stop Legacy Connector**
   ```bash
   # Stop old service
   net stop "B2ConnectErpConnector"
   ```

3. **Install New Framework**
   ```bash
   # Download and configure new connector
   b2connect erp download-connector --erp-type enventa --version 2.1.0
   b2connect erp configure-connector --erp-type enventa --config-file enventa-config-backup.json
   ```

4. **Migrate Data (if needed)**
   - Catalog data automatically resyncs
   - Order history preserved in B2Connect core
   - Customer data remains in ERP

5. **Start New Connector**
   ```bash
   b2connect erp start-connector --erp-type enventa
   b2connect erp enable-autostart --erp-type enventa
   ```

6. **Verify Integration**
   ```bash
   b2connect erp test-connection --erp-type enventa
   b2connect erp status-connector --erp-type enventa
   ```

#### Compatibility

- **API Compatibility**: 100% backward compatible with existing B2Connect integrations
- **Data Compatibility**: All existing data mappings preserved
- **Performance**: Improved with connection pooling and async operations

## API Endpoints

### Health & Status
- `GET /api/health` - Framework health check
- `GET /api/health/{erp-type}` - ERP-specific health check
- `GET /api/status/{erp-type}` - Detailed connector status

### Catalog Operations
- `POST /api/{erp-type}/catalog/sync` - Trigger catalog synchronization
- `GET /api/{erp-type}/catalog/status` - Sync status and progress

### Order Operations
- `POST /api/{erp-type}/orders` - Create new order
- `GET /api/{erp-type}/orders/{orderId}` - Get order status

### Customer Operations
- `GET /api/{erp-type}/customers/{customerId}` - Get customer data

### AI Processing Endpoints
- `POST /api/ai/validate` - Validate ERP data using AI
- `POST /api/ai/correct` - Correct ERP data using AI suggestions
- `POST /api/ai/process` - General AI processing for tenant data

#### AI Validation Example
```bash
curl -X POST http://localhost:5081/api/ai/validate \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "Article data to validate..."
  }'
```

#### AI Correction Example
```bash
curl -X POST http://localhost:5081/api/ai/correct \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "data": "Original article data...",
    "errors": "Validation error messages..."
  }'
```

## Security Considerations

### Authentication
- Tenant-scoped API keys for all operations
- OAuth2 flows for interactive setup
- Certificate-based auth for high-security ERPs

### Data Protection
- End-to-end TLS encryption
- Tenant-specific encryption keys
- Secure credential storage with rotation

### Distribution Security
- Code-signed binaries
- Integrity verification on download
- Automated security scanning

## Monitoring & Troubleshooting

### Health Monitoring
```bash
# Check all connectors
b2connect erp health-check

# Detailed diagnostics
b2connect erp diagnostics --erp-type enventa
```

### Common Issues

**Connection Failures:**
```bash
# Test connectivity
b2connect erp test-connection --erp-type enventa --verbose

# Check logs
b2connect erp logs --erp-type enventa --tail 100
```

**Performance Issues:**
```bash
# Monitor performance metrics
b2connect erp metrics --erp-type enventa

# Adjust connection pool
b2connect erp configure-connector --erp-type enventa --max-connections 10
```

**Update Connectors:**
```bash
# Check for updates
b2connect erp check-updates

# Update specific connector
b2connect erp update-connector --erp-type enventa --version latest
```

## Development & Contribution

### Building from Source
```bash
# Clone repository
git clone https://github.com/b2connect/erp-connector-framework.git
cd erp-connector-framework

# Build framework
dotnet build B2Connect.Erp.Framework.sln

# Run tests
dotnet test B2Connect.Erp.Framework.sln
```

### Adding New ERP Connectors
1. Follow the development guide above
2. Submit pull request with comprehensive tests
3. Include configuration schema and documentation
4. Ensure compliance with security requirements

### Testing Strategy
- Unit tests for all connector implementations
- Integration tests with mock ERPs
- Performance tests for large catalogs
- Security penetration testing

## License & Support

**License:** Copyright © NissenVelten Software GmbH 2026. All rights reserved.

**Support:**
- Documentation: [B2Connect ERP Framework Docs](https://docs.b2connect.com/erp)
- Issues: [GitHub Issues](https://github.com/b2connect/erp-connector-framework/issues)
- Enterprise Support: support@b2connect.com

**References:**
- [ADR-034] Multi-ERP Connector Architecture
- [KB-021] enventa Trade ERP Integration Guide
