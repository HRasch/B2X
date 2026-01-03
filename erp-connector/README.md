# B2Connect ERP Connector

**.NET Framework 4.8 Windows Service** for connecting B2Connect to enventa Trade ERP.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          B2Connect (.NET 10)                            │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────────┐  │
│  │  ErpQueryBuilder │ → │  IErpHttpClient │ → │ HTTP Requests        │  │
│  │  (Type-safe)     │    │  (Abstraction)  │    │ /api/articles/query │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼ HTTP (localhost:5080)
┌─────────────────────────────────────────────────────────────────────────┐
│                   B2Connect.ErpConnector (.NET 4.8)                     │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────────┐  │
│  │ OWIN Web API 2  │ → │ EnventaErpService│ → │ EnventaUtil/Scope   │  │
│  │ (Self-hosted)   │    │ (Business Logic) │    │ (Connection Pool)   │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────────┘  │
│                                                          │              │
│                              ┌────────────────────────────┤              │
│                              │                            │              │
│                              ▼                            ▼              │
│                   ┌─────────────────────┐    ┌─────────────────────┐    │
│                   │ EnventaActorPool    │    │ EnventaGlobalFactory│    │
│                   │ (Thread-safe)       │    │ (Pool Management)   │    │
│                   └─────────────────────┘    └─────────────────────┘    │
│                                                          │              │
│                                                          ▼              │
│                                              ┌─────────────────────┐    │
│                                              │ enventa Trade ERP   │    │
│                                              │ (IcECArticle, etc.) │    │
│                                              └─────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────┘
```

## Infrastructure Pattern (based on eGate)

The infrastructure is based on the proven **eGate NVShop.Data.FS.shared** patterns:

```
┌───────────────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                               │
├───────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  EnventaUtil (static entry point)                                     │
│       │                                                               │
│       ▼                                                               │
│  EnventaScope (disposable, per-operation)                             │
│       │                                                               │
│       ▼                                                               │
│  EnventaGlobalFactory (singleton, manages pools)                      │
│       │                                                               │
│       ▼                                                               │
│  EnventaGlobalPool (per-identity, connection pool)                    │
│       │                                                               │
│       ▼                                                               │
│  IEnventaGlobalObjectFactory (creates actual ERP connections)         │
│                                                                       │
└───────────────────────────────────────────────────────────────────────┘
```

### Key Classes

| Class | eGate Equivalent | Purpose |
|-------|------------------|---------|
| `EnventaUtil` | `FSUtil` | Static helper methods, creates scopes |
| `EnventaScope` | `FSScope` | Disposable scope for ERP operations |
| `EnventaGlobalFactory` | `FSGlobalFactory` | Manages connection pools per identity |
| `EnventaGlobalPool` | `FSGlobalPool` | Thread-safe connection pool |
| `EnventaIdentity` | `NVIdentity` | Identity with Name, Password, BusinessUnit |
| `EnventaGlobalContext` | `FSGlobalContext` | Wrapper for ERP global objects |

### Usage Pattern

```csharp
// Service uses EnventaUtil for all ERP operations
public class EnventaErpService
{
    private readonly EnventaUtil _util;

    public EnventaErpService(IEnventaIdentityProvider identityProvider)
    {
        _util = new EnventaUtil(identityProvider);
    }

    public ArticleDto GetArticle(string articleId)
    {
        // Scope acquires connection from pool, returns on dispose
        using (var scope = _util.Scope())
        {
            var article = scope.Create<IcECArticle>();
            article.Load(articleId);
            return MapToDto(article);
        }
    }
}
```

## Why .NET Framework 4.8?

The enventa Trade ERP uses proprietary assemblies (`FSUtil.dll`, `IcECArticle.dll`, etc.) that:
- Are compiled for .NET Framework 4.x
- Are **NOT thread-safe** (require single-threaded access)
- Cannot be loaded directly into .NET 10 (different runtime)

This connector acts as a **bridge** between modern B2Connect and legacy enventa ERP.

## Actor Pattern for Thread Safety

Since enventa ERP is not thread-safe, we use the **Actor Pattern**:

```csharp
// Each tenant gets a dedicated actor with serialized access
var actor = EnventaActorPool.Instance.GetOrCreateActor(tenantId);

// Operations are queued and executed sequentially
var result = await actor.ExecuteAsync<ArticleDto>(async () => {
    // This runs on a single thread per tenant
    // Uses scope pattern for connection management
    using (var scope = _util.Scope())
    {
        return scope.Create<IcECArticle>().Load(articleNumber);
    }
});
```

Benefits:
- ✅ Thread-safe access to non-thread-safe ERP
- ✅ Tenant isolation (each tenant has own actor)
- ✅ Connection pooling per identity (reduces connection overhead)
- ✅ Automatic operation queuing
- ✅ No race conditions or deadlocks

## API Endpoints

### Health
- `GET /api/health` - Full health check with ERP connectivity
- `GET /api/health/ping` - Simple liveness probe

### Articles
- `GET /api/articles/{articleNumber}` - Get single article
- `POST /api/articles/query` - Query articles with specification
- `POST /api/articles/sync` - Bulk sync articles to B2Connect

### Customers
- `GET /api/customers/{customerNumber}` - Get single customer
- `POST /api/customers/query` - Query customers with specification

### Orders
- `GET /api/orders/{orderNumber}` - Get single order
- `POST /api/orders` - Create new order in ERP

## Installation

### Prerequisites
- Windows Server 2016+ or Windows 10+
- .NET Framework 4.8 Runtime
- enventa Trade ERP installed and configured

### Build
```powershell
# Open in Visual Studio 2022 or build via MSBuild
msbuild B2Connect.ErpConnector.sln /p:Configuration=Release
```

### Install as Windows Service
```powershell
# Navigate to build output
cd bin\Release

# Install service (run as Administrator)
B2Connect.ErpConnector.exe install

# Start service
B2Connect.ErpConnector.exe start

# View service status
sc query "B2ConnectErpConnector"
```

### Uninstall
```powershell
# Stop and uninstall
B2Connect.ErpConnector.exe stop
B2Connect.ErpConnector.exe uninstall
```

## Configuration

Edit `App.config` before deployment:

```xml
<appSettings>
  <!-- HTTP API -->
  <add key="BaseAddress" value="http://localhost:5080" />
  
  <!-- enventa ERP Connection -->
  <add key="Enventa:ConnectionString" value="Server=ERPSERVER;Database=ENVENTA;Integrated Security=True" />
  <add key="Enventa:LicenseServer" value="LICENSESERVER:1234" />
  <add key="Enventa:BasePath" value="C:\enventa\base" />
  
  <!-- Actor Pool -->
  <add key="ActorPool:MaxActorsPerTenant" value="1" />
  <add key="ActorPool:OperationTimeoutSeconds" value="30" />
</appSettings>
```

## Logging

Logs are written to:
- **Console** (when running interactively)
- **File**: `logs/erp-connector.log` (rolling daily, 7-day retention)

Log levels can be configured in `App.config`:
```xml
<nlog>
  <rules>
    <logger name="*" minlevel="Info" writeTo="console,file" />
  </rules>
</nlog>
```

## Development

### Running Locally
```powershell
# Run as console application (not as service)
B2Connect.ErpConnector.exe
```

### Testing
```powershell
# Health check
curl http://localhost:5080/api/health

# Query articles
curl -X POST http://localhost:5080/api/articles/query `
  -H "Content-Type: application/json" `
  -H "X-Tenant-Id: tenant1" `
  -d '{"category":"Schrauben","skip":0,"take":100}'
```

## Integration with B2Connect

B2Connect uses `IErpHttpClient` to communicate:

```csharp
// In B2Connect (.NET 10)
var specification = ArticleSpecification.Create()
    .WithCategory("Schrauben")
    .ActiveOnly()
    .WithPriceRange(10, 100)
    .Build();

var articles = await erpHttpClient.QueryArticlesAsync(specification);
```

This translates to HTTP requests to the connector.

## Security Considerations

⚠️ **Production Deployment**:
- Run behind firewall, not exposed to internet
- Use HTTPS with valid certificate
- Implement authentication (API keys or JWT)
- Restrict to known B2Connect server IPs
- Regular security updates

## Troubleshooting

### Service Won't Start
1. Check Windows Event Viewer for errors
2. Verify .NET Framework 4.8 installed
3. Check App.config settings
4. Run as console to see startup errors

### Connection to ERP Fails
1. Verify enventa ERP is running
2. Check license server connectivity
3. Verify connection string
4. Check enventa DLLs are accessible

### Performance Issues
1. Monitor actor queue depth
2. Check ERP response times
3. Consider increasing timeout values
4. Review logging for slow operations

## License

Copyright © NissenVelten Software GmbH 2026. All rights reserved.

See [KB-021] for enventa Trade ERP integration details.
See [ADR-023] for ERP Plugin Architecture decision.
