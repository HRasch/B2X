# ⚙️ Aspire Orchestration Reference

**Audience**: DevOps, Backend leads  
**Purpose**: .NET Aspire setup, service orchestration, local development  
**Framework**: .NET Aspire, .NET 10, C#

---

## Quick Start: Local Development

```bash
# Start Aspire Orchestration
cd backend/Orchestration
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Aspire Dashboard opens at http://localhost:15500
# Services auto-start with In-Memory database
```

All services available instantly:
- 📊 **Aspire Dashboard**: http://localhost:15500
- 🛒 **Store Gateway**: http://localhost:6000
- 🔧 **Admin Gateway**: http://localhost:6100

---

## What is Aspire?

**.NET Aspire** is Microsoft's cloud-native orchestration framework:

- ✅ Local orchestration (replaces Docker Compose for dev)
- ✅ Service discovery & networking
- ✅ Structured logging & observability
- ✅ Health checks & readiness probes
- ✅ Environment variable management
- ✅ Dashboard for monitoring

**Why Aspire over Docker?**
- Faster startup (no containers)
- Native .NET debugging (F5 in VS Code)
- Instant hot reload
- Built-in observability

---

## Project Structure

```
backend/
├── Orchestration/
│   └── B2Connect.Orchestration.csproj    ← START HERE
│       └── Program.cs                     ← Service definitions
│
├── BoundedContexts/
│   ├── Store/
│   │   ├── API/
│   │   │   └── B2Connect.Store.csproj    ← Store Gateway
│   │   ├── Catalog/
│   │   │   └── B2Connect.Catalog.csproj
│   │   ├── CMS/
│   │   │   └── B2Connect.CMS.csproj
│   │   ├── Theming/
│   │   │   └── B2Connect.Theming.csproj
│   │   ├── Localization/
│   │   │   └── B2Connect.Localization.csproj
│   │   └── Search/
│   │       └── B2Connect.Search.csproj
│   │
│   ├── Admin/
│   │   └── API/
│   │       └── B2Connect.Admin.csproj    ← Admin Gateway
│   │
│   └── Shared/
│       ├── Identity/
│       │   └── B2Connect.Identity.csproj
│       └── Tenancy/
│           └── B2Connect.Tenancy.csproj
│
├── ServiceDefaults/
│   └── B2Connect.ServiceDefaults.csproj  ← Shared config
│
└── shared/
    ├── B2Connect.Shared.Core/
    ├── B2Connect.Shared.Infrastructure/
    ├── B2Connect.Shared.Messaging/
    └── B2Connect.Shared.Search/
```

---

## Program.cs: Aspire Configuration

**File**: `backend/Orchestration/Program.cs`

```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// ===== DATABASES =====
// Shared PostgreSQL for all contexts
var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume();

var storeDb = postgres.AddDatabase("store-db");
var adminDb = postgres.AddDatabase("admin-db");

// ===== CACHE =====
var redis = builder
    .AddRedis("redis")
    .WithRedisInsight();  // UI at http://localhost:8001

// ===== SEARCH =====
var elasticsearch = builder
    .AddElasticsearch("elasticsearch")
    .WithDashboard();  // Kibana at http://localhost:5601

// ===== MESSAGING =====
var rabbitmq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();  // UI at http://localhost:15672

// ===== SHARED SERVICES =====
builder
    .AddProject<Projects.B2Connect_Identity>("identity")
    .WithReference(storeDb)
    .WithReference(redis);

builder
    .AddProject<Projects.B2Connect_Tenancy>("tenancy")
    .WithReference(storeDb)
    .WithReference(redis);

// ===== STORE CONTEXT =====
// Catalog Service
builder
    .AddProject<Projects.B2Connect_Catalog>("catalog")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithReference(elasticsearch)
    .WithHttpsEndpoint(port: 6001, name: "catalog-service");

// CMS Service
builder
    .AddProject<Projects.B2Connect_CMS>("cms")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithReference(elasticsearch)
    .WithHttpsEndpoint(port: 6002, name: "cms-service");

// Theming Service
builder
    .AddProject<Projects.B2Connect_Theming>("theming")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 6003, name: "theming-service");

// Localization Service
builder
    .AddProject<Projects.B2Connect_Localization>("localization")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 6004, name: "localization-service");

// Search Service
builder
    .AddProject<Projects.B2Connect_Search>("search")
    .WithReference(elasticsearch)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 6005, name: "search-service");

// Store Gateway (PUBLIC API)
var storeGateway = builder
    .AddProject<Projects.B2Connect_Store>("store-gateway")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithReference(elasticsearch)
    .WithHttpEndpoint(port: 6000, name: "http")
    .WithHttpsEndpoint(name: "https");

// ===== ADMIN CONTEXT =====
// Admin Gateway (PROTECTED API)
var adminGateway = builder
    .AddProject<Projects.B2Connect_Admin>("admin-gateway")
    .WithReference(adminDb)
    .WithReference(redis)
    .WithHttpEndpoint(port: 6100, name: "http")
    .WithHttpsEndpoint(name: "https");

// ===== BUILD & RUN =====
var app = builder.Build();
await app.RunAsync();
```

---

## Service Configuration

### Service Definition (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Database": {
    "Provider": "inmemory"
  },
  "Caching": {
    "Enabled": true,
    "DefaultTtl": "01:00:00"
  },
  "Search": {
    "Enabled": true,
    "IndexPrefix": "dev-"
  }
}
```

### Service Defaults (ServiceDefaults/Program.cs)

```csharp
using Aspire.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace B2Connect.ServiceDefaults;

public static class Extensions
{
    /// <summary>
    /// Add default Aspire services to all microservices
    /// </summary>
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder)
    {
        // Resilience (Polly)
        builder.Services.AddHttpClientResilienceHandler()
            .WithRetry(3)
            .WithTimeout(TimeSpan.FromSeconds(30));

        // Service discovery
        builder.Services.AddServiceDiscovery();

        // Health checks
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), 
                      tags: new[] { "live" });

        // Observability
        builder.ConfigureOpenTelemetry();

        return builder;
    }

    /// <summary>
    /// Configure OpenTelemetry for all services
    /// </summary>
    private static IHostApplicationBuilder ConfigureOpenTelemetry(
        this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    /// Configure database connection (PostgreSQL or InMemory)
    /// </summary>
    public static IHostApplicationBuilder AddDatabase(
        this IHostApplicationBuilder builder,
        string connectionStringName = "DefaultConnection")
    {
        var configuration = builder.Configuration;
        
        builder.Services.AddScoped(sp =>
        {
            var connection = configuration.GetConnectionString(connectionStringName);
            return new DbContextOptions<ApplicationDbContext>(
                builder: opts => opts.UseNpgsql(connection));
        });

        return builder;
    }

    /// <summary>
    /// Configure caching (Redis or InMemory)
    /// </summary>
    public static IHostApplicationBuilder AddCaching(
        this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        if (configuration.GetValue<bool>("Caching:Enabled"))
        {
            builder.Services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = configuration.GetConnectionString("Redis");
            });
        }
        else
        {
            builder.Services.AddMemoryCache();
        }

        return builder;
    }
}
```

---

## Environment Variables

### Development (Aspire In-Memory)

```bash
# backend/Orchestration/.env.local
ASPNETCORE_ENVIRONMENT=Development
Database__Provider=inmemory
DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true
ASPIRE_ALLOW_UNSECURED_TRANSPORT=true
```

### Production (Real Services)

```bash
# backend/Orchestration/.env.production
ASPNETCORE_ENVIRONMENT=Production
Database__Provider=postgresql
Database__ConnectionString=Server=prod-postgres;...
Redis__ConnectionString=prod-redis:6379
Elasticsearch__Nodes=https://prod-elasticsearch:9200
RabbitMQ__HostName=prod-rabbitmq
RabbitMQ__UserName=admin
RabbitMQ__Password=***
```

---

## Health Checks

All services expose health endpoints:

```csharp
// In service Program.cs
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

**Usage**:
```bash
curl http://localhost:6000/health
curl http://localhost:6100/health
curl http://localhost:6001/health  # Catalog
```

Response:
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "redis": "Healthy",
    "elasticsearch": "Healthy"
  }
}
```

---

## Service Discovery (In Aspire)

Services automatically discover each other via `IHttpClientFactory`:

```csharp
// In Store.API
public class CatalogClient
{
    private readonly HttpClient _http;
    
    public CatalogClient(IHttpClientFactory factory)
    {
        // Aspire resolves "catalog" to http://catalog:6001
        _http = factory.CreateClient("catalog");
    }
    
    public async Task<List<Product>> GetProducts()
    {
        var response = await _http.GetAsync("/api/products");
        return await response.Content.ReadAsAsync<List<Product>>();
    }
}

// Register in Program.cs
builder.Services
    .AddHttpClient<CatalogClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("http://catalog");
    });
```

---

## Observability: Aspire Dashboard

**Access**: http://localhost:15500

**Features**:
- 📊 Real-time metrics (CPU, memory, requests/sec)
- 🔍 Distributed tracing (see request flow across services)
- 📝 Structured logs (search by level, service, correlation ID)
- 🏥 Health checks status
- 📡 Service dependencies graph

**Example Trace View**:
```
Request: POST /api/products
├─ Store.API (50ms)
│  ├─ Identity validation (10ms)
│  ├─ Database query (25ms)
│  └─ Redis cache (5ms)
├─ Catalog service (30ms)
│  ├─ Database query (20ms)
│  └─ Search reindex (10ms)
└─ Response (20ms)
Total: 80ms
```

---

## Common Tasks

### Add a New Service

1. **Create project**:
```bash
dotnet new globaljson --sdk-version 10.0.0
mkdir backend/BoundedContexts/MyContext/MyService
dotnet new webapi -n B2Connect.MyService -o backend/BoundedContexts/MyContext/MyService
```

2. **Add to Orchestration/Program.cs**:
```csharp
builder
    .AddProject<Projects.B2Connect_MyService>("my-service")
    .WithReference(storeDb)
    .WithReference(redis)
    .WithHttpsEndpoint(port: 6010, name: "http");
```

3. **Reference ServiceDefaults**:
```csharp
// In MyService/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();

// Add specific services
builder.Services.AddDatabase();
builder.Services.AddCaching();
```

### Change Port

```csharp
// In Orchestration/Program.cs
builder
    .AddProject<Projects.B2Connect_Catalog>("catalog")
    .WithHttpsEndpoint(port: 6011);  // Changed from 6001
```

### Add External Service (PostgreSQL)

```csharp
var postgres = builder
    .AddPostgres("postgres", port: 5432)
    .WithDataVolume(name: "postgres-data")
    .WithEnvironment("POSTGRES_PASSWORD", "dev-password");

var myDb = postgres.AddDatabase("my-service-db");

builder
    .AddProject<Projects.B2Connect_MyService>("my-service")
    .WithReference(myDb);  // Aspire injects connection string
```

### Debug a Single Service

```bash
# Start only Orchestration (all services)
cd backend/Orchestration
dotnet run

# In VS Code, F5 and select "Orchestration"
# All services start, but you can attach debugger to any
```

---

## Troubleshooting

### Port Already in Use
```bash
# Find process using port 6000
lsof -i :6000

# Kill it
kill -9 <PID>
```

### Service Won't Connect to Redis
```bash
# Check Redis is running
curl http://localhost:6379

# Check connection string
echo $REDIS_CONNECTIONSTRING

# Restart Redis
docker restart redis-aspire
```

### Database Migrations Not Applied
```bash
# Run migrations manually
cd backend/BoundedContexts/Store/Catalog
dotnet ef database update

# Or reset in-memory
ASPNETCORE_ENVIRONMENT=Development Database__Provider=inmemory dotnet run
```

### Can't Access Dashboard
```bash
# Ensure Aspire is running
curl http://localhost:15500

# Check firewall
sudo ufw allow 15500

# Restart Aspire
cd backend/Orchestration
dotnet run
```

---

## Performance Tuning

### Caching Strategy
```csharp
// In appsettings.Development.json
{
  "Caching": {
    "DefaultTtl": "01:00:00",  // 1 hour for dev
    "SearchTtl": "00:05:00",   // 5 min for search
    "ProductTtl": "06:00:00"   // 6 hours for products
  }
}
```

### Connection Pooling
```csharp
// In ServiceDefaults
builder.Services.AddNpgsql<ApplicationDbContext>(
    options =>
    {
        options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
        options.CommandTimeout(30);
    });
```

### Request Timeout
```csharp
// In ServiceDefaults
builder.Services.AddHttpClientResilienceHandler()
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRetry(3, TimeSpan.FromMilliseconds(500));
```

---

## References

- [.NET Aspire Docs](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Service Discovery](https://learn.microsoft.com/en-us/dotnet/aspire/service-discovery/)
- [DDD Bounded Contexts](DDD_BOUNDED_CONTEXTS_REFERENCE.md)
- [Wolverine Pattern Reference](WOLVERINE_PATTERN_REFERENCE.md)

---

*Updated: 30. Dezember 2025*  
*Framework: .NET Aspire orchestration*
