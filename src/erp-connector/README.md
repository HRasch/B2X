# B2X ERP Connector Framework

**Pluggable ERP Integration Framework** for B2X multi-tenant SaaS platform. Supports extensible ERP connectors with standardized interfaces, secure distribution, and tenant self-service management.

## Overview

The B2X ERP Connector Framework provides a pluggable architecture for integrating with multiple ERP systems. Built on ADR-034 Multi-ERP Connector Architecture, it enables seamless addition of new ERP connectors without modifying the core framework.

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

### Performance Monitoring & Benchmarking

The ERP Connector includes comprehensive performance monitoring and benchmarking capabilities to track and optimize ERP operations.

#### Features

- **📊 Real-time Metrics**: Track execution times, throughput, and error rates for all ERP operations
- **⏱️ Operation Timing**: Automatic timing of database queries, API calls, and AI processing
- **📈 Performance Reports**: Periodic console reports with detailed statistics
- **🏃 Benchmarking Mode**: Dedicated benchmarking mode for performance testing
- **🔍 Tenant-specific Metrics**: Separate metrics tracking per tenant for multi-tenant analysis

#### Performance Monitoring Configuration

```xml
<!-- App.config -->
<add key="PerformanceMonitoring:Enabled" value="true" />
<add key="PerformanceMonitoring:ReportingInterval" value="00:05:00" />
```

#### Benchmarking Mode

Run performance benchmarks using the `--benchmark` command-line argument:

```bash
# Run performance benchmarks
./B2X.ErpConnector.exe --benchmark

# Example output:
=== ERP Connector Performance Benchmark ===
AI processing enabled for benchmarking using ollama provider
Starting performance benchmarks...
Benchmarking GetArticle operations (10 iterations)...
Benchmarking QueryArticles operations (5 iterations)...
AI benchmarking skipped - requires proper AI service configuration

=== PERFORMANCE METRICS ===
Session Time: 45.23s
GetArticle_benchmark-tenant: 10 calls, avg=125.3ms, min=98.2ms, max=156.7ms, 0.22 calls/sec, error=0.00%
QueryArticles_benchmark-tenant: 5 calls, avg=234.1ms, min=198.5ms, max=289.3ms, 0.11 calls/sec, error=0.00%
```

#### Monitored Operations

| Operation | Description | Metrics Tracked |
|-----------|-------------|-----------------|
| `GetArticle_{tenant}` | Single article retrieval | Response time, success rate |
| `QueryArticles_{tenant}` | Article queries with filtering | Query time, result count, pagination |
| `AI_ProcessTenantData_{tenant}_{operation}` | AI data processing | Processing time, token usage |
| `AI_ValidateErpData_{tenant}` | AI data validation | Validation time, accuracy metrics |

#### Metrics Export

Performance metrics can be exported in JSON format for analysis:

```json
{
  "SessionTimeSeconds": 45.23,
  "Operations": [
    {
      "OperationName": "GetArticle_tenant1",
      "TotalCalls": 150,
      "SuccessfulCalls": 148,
      "FailedCalls": 2,
      "AverageDurationMs": 125.3,
      "MinDurationMs": 98.2,
      "MaxDurationMs": 156.7,
      "TotalDurationMs": 18795.0,
      "CallsPerSecond": 3.31,
      "ErrorRate": 0.013
    }
  ]
}
```

#### ERP Connector CLI Integration

The ERP Connector integrates with the B2X Administration CLI for comprehensive benchmarking and monitoring:

```bash
# Run ERP connector benchmarks
B2X-admin metrics benchmark --service erp-connector --duration 60 --concurrency 5

# View ERP connector metrics
B2X-admin metrics view --service erp-connector --time-range 24h

# Configure ERP connector monitoring
B2X-admin metrics config set monitoring.enabled true
B2X-admin metrics config set metrics.collection_interval_seconds 30

# View active alerts
B2X-admin metrics alerts list --service erp-connector
```

#### ERP Connector-Specific Metrics

| Metric | Description | Collection Method |
|--------|-------------|-------------------|
| `GetArticle` | Article retrieval performance | Automatic timing |
| `QueryArticles` | Article query performance | Automatic timing |
| `AI_ProcessTenantData` | AI processing performance | Automatic timing |
| `ConnectionPool` | Database connection usage | Pool monitoring |
| `ActorPool` | Thread actor utilization | Pool monitoring |

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
│                 B2X Core Services                      │
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
B2X erp list-supported

# Output example:
Available ERP Connectors:
- enventa (v2.1.0) - enventa Trade ERP (.NET Framework 4.8)
- craft-punchout (v1.0.0) - Craft Software Punchout (.NET 8.0)
- sap (v1.0.0) - SAP ERP/S4HANA (.NET 8.0) [Coming Soon]
```

#### 2. Download Connector

```bash
# Download the enventa connector
B2X erp download-connector --erp-type enventa --version latest

# Verify download
B2X erp list-installed
```

#### 3. Configure Connector

```bash
# Interactive configuration wizard
B2X erp configure-connector --erp-type enventa --interactive

# Or configure with specific settings
B2X erp configure-connector --erp-type enventa \
  --connection-string "Server=ERP-SERVER;Database=ENVENTA;Integrated Security=True" \
  --license-server "LICENSE-SERVER:1234" \
  --base-path "C:\enventa\base"
```

#### 4. Test Connection

```bash
# Test ERP connectivity
B2X erp test-connection --erp-type enventa

# Check connector status
B2X erp status-connector --erp-type enventa
```

#### 5. Start Services

```bash
# Start the ERP connector service
B2X erp start-connector --erp-type enventa

# Enable automatic startup
B2X erp enable-autostart --erp-type enventa
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
B2X erp download-connector --erp-type <erp-type>
B2X erp configure-connector --erp-type <erp-type> --interactive
B2X erp test-connection --erp-type <erp-type>
B2X erp start-connector --erp-type <erp-type>
```

## Development Guide for New Connectors

### 1. Create Connector Project

```bash
# Use the connector template
dotnet new B2X-erp-connector -n MyErpConnector
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
    <PackageId>B2X.Erp.MyErpConnector</PackageId>
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
   B2X erp export-config --erp-type enventa > enventa-config-backup.json
   ```

2. **Stop Legacy Connector**
   ```bash
   # Stop old service
   net stop "B2XErpConnector"
   ```

3. **Install New Framework**
   ```bash
   # Download and configure new connector
   B2X erp download-connector --erp-type enventa --version 2.1.0
   B2X erp configure-connector --erp-type enventa --config-file enventa-config-backup.json
   ```

4. **Migrate Data (if needed)**
   - Catalog data automatically resyncs
   - Order history preserved in B2X core
   - Customer data remains in ERP

5. **Start New Connector**
   ```bash
   B2X erp start-connector --erp-type enventa
   B2X erp enable-autostart --erp-type enventa
   ```

6. **Verify Integration**
   ```bash
   B2X erp test-connection --erp-type enventa
   B2X erp status-connector --erp-type enventa
   ```

#### Compatibility

- **API Compatibility**: 100% backward compatible with existing B2X integrations
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
B2X erp health-check

# Detailed diagnostics
B2X erp diagnostics --erp-type enventa
```

### Common Issues

**Connection Failures:**
```bash
# Test connectivity
B2X erp test-connection --erp-type enventa --verbose

# Check logs
B2X erp logs --erp-type enventa --tail 100
```

**Performance Issues:**
```bash
# Monitor performance metrics
B2X erp metrics --erp-type enventa

# Adjust connection pool
B2X erp configure-connector --erp-type enventa --max-connections 10
```

**Update Connectors:**
```bash
# Check for updates
B2X erp check-updates

# Update specific connector
B2X erp update-connector --erp-type enventa --version latest
```

## Development & Contribution

### Building from Source
```bash
# Clone repository
git clone https://github.com/B2X/erp-connector-framework.git
cd erp-connector-framework

# Build framework
dotnet build B2X.Erp.Framework.sln

# Run tests
dotnet test B2X.Erp.Framework.sln
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
- Documentation: [B2X ERP Framework Docs](https://docs.B2X.com/erp)
- Issues: [GitHub Issues](https://github.com/B2X/erp-connector-framework/issues)
- Enterprise Support: support@B2X.com

**References:**
- [ADR-034] Multi-ERP Connector Architecture
- [KB-021] enventa Trade ERP Integration Guide
