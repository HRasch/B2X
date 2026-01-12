# B2X Aspire Orchestration Specification

## Overview

B2X uses **.NET Aspire** as the central orchestration and service management platform. Aspire provides automated service discovery, health checking, observability, and lifecycle management for all microservices in the B2X ecosystem.

---

## ⚠️ .NET 10 Compatibility Notice (January 2026)

**Current Configuration**: Aspire 13.1.0 with .NET 10.0 target framework

**Known Issue**: `Aspire.Hosting` package (13.1.0) only ships `net8.0` assemblies, causing runtime `FileNotFoundException` when AppHost targets `net10.0`.

**Workaround Applied** (in `B2X.AppHost.csproj`):
```xml
<PropertyGroup>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  <AssetTargetFallback>$(AssetTargetFallback);net8.0</AssetTargetFallback>
</PropertyGroup>
```

**Status**: Fix expected in Aspire 13.2. See [dotnet/aspire#13611](https://github.com/dotnet/aspire/issues/13611)

---

## 1. Aspire Architecture

### Service Topology

```
┌─────────────────────────────────────────────────────┐
│        B2X Aspire Orchestration Host           │
│              (AppHost - Port 15500)                 │
│                                                     │
│  ┌─────────────────────────────────────────────┐   │
│  │  Service Registry & Discovery              │   │
│  │  - Service Name Resolution                 │   │
│  │  - Port Assignment Management              │   │
│  │  - Environment Variable Injection          │   │
│  └─────────────────────────────────────────────┘   │
│                                                     │
│  ┌─────────────────────────────────────────────┐   │
│  │  Lifecycle Management                       │   │
│  │  - Service Startup Orchestration           │   │
│  │  - Health Check Monitoring                 │   │
│  │  - Restart Policies                        │   │
│  │  - Dependency Ordering                     │   │
│  └─────────────────────────────────────────────┘   │
│                                                     │
│  ┌─────────────────────────────────────────────┐   │
│  │  Observability Stack                        │   │
│  │  - Logs (Console + Structured)             │   │
│  │  - Metrics (Prometheus Compatible)         │   │
│  │  - Traces (OpenTelemetry)                  │   │
│  │  - Real-time Dashboard                     │   │
│  └─────────────────────────────────────────────┘   │
│                                                     │
│  ┌─────────────────────────────────────────────┐   │
│  │  Service Dependencies                       │   │
│  │                                             │   │
│  │  API Gateway → {Auth, Tenant, Shop}        │   │
│  │  Auth Service → PostgreSQL                 │   │
│  │  Tenant Service → PostgreSQL                │   │
│  │  Shop Service → {PostgreSQL, Redis}        │   │
│  │  Order Service → {PostgreSQL, RabbitMQ}    │   │
│  │  Procurement Gateway → {API, PostgreSQL}   │   │
│  └─────────────────────────────────────────────┘   │
│                                                     │
└─────────────────────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
    Logs Panel      Metrics Panel   Traces Panel
    (Port 15500)    (Port 15500)    (Port 15500)
```

---

## 2. Managed Services

### Service Registry

| Service | Port | Framework | Dependencies | Status |
|---------|------|-----------|--------------|--------|
| **API Gateway** | 5000 | ASP.NET Core 8 | - | Core |
| **Auth Service** | 5001 | ASP.NET Core 8 | PostgreSQL | Critical |
| **Tenant Service** | 5002 | ASP.NET Core 8 | PostgreSQL | Critical |
| **Shop Service** | 5003 | ASP.NET Core 8 | PostgreSQL, Redis | Business |
| **Order Service** | 5004 | ASP.NET Core 8 | PostgreSQL, RabbitMQ | Business |
| **Procurement Gateway** | 5005 | ASP.NET Core 8 | PostgreSQL, API Gateway | Integration |
| **Catalog Service** | 5006 | ASP.NET Core 8 | PostgreSQL | Business |
| **Inventory Service** | 5007 | ASP.NET Core 8 | PostgreSQL | Business |
| **Supplier Service** | 5010 | ASP.NET Core 8 | PostgreSQL | Integration |

### Resource Dependencies

| Resource | Type | Host | Port | Purpose |
|----------|------|------|------|---------|
| PostgreSQL | Database | localhost | 5432 | Primary data storage, tenant isolation |
| RabbitMQ | Message Broker | localhost | 5672 | Event publishing, message queues |
| Redis | Cache | localhost | 6379 | Session storage, cache layer |
| ElasticSearch | Search | localhost | 9200 | Full-text search, logging |

---

## 3. Aspire Orchestration Features

### 3.1 Service Discovery

**Automatic Service Registration:**
```csharp
// In AppHost Program.cs
var apiGateway = builder
    .AddProject<Projects.B2X_ApiGateway>("api-gateway")
    .WithHttpEndpoint(port: 5000);

var authService = builder
    .AddProject<Projects.B2X_AuthService>("auth-service")
    .WithHttpEndpoint(port: 5001)
    .WithReference(postgres);  // Automatic dependency injection
```

**Service Resolution:**
- Each service gets environment variables for upstream services
- DNS-like resolution: `http://service-name:port`
- Automatic port assignment for inter-service communication

### 3.2 Health Checks

**Automatic Health Monitoring:**

Each service registered with `AddServiceDefaults()` automatically gets:

1. **Liveness Probe** (`/health/live`)
   - Is the service running?
   - TCP connection test
   
2. **Readiness Probe** (`/health/ready`)
   - Is the service ready to accept requests?
   - Database connectivity check
   - Dependency availability check

3. **Startup Probe** (`/health/startup`)
   - Has the service completed initialization?
   - Migration execution
   - Warm-up tasks

**Health Check Endpoint:**
```bash
GET http://localhost:5000/health
```

Response:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "database": { "status": "Healthy" },
    "service": { "status": "Healthy" }
  }
}
```

### 3.3 Lifecycle Management

**Service Startup Order:**

Aspire respects dependency ordering:

1. **PostgreSQL** (foundational resource)
2. **RabbitMQ** (foundational resource)
3. **Auth Service** (depends on PostgreSQL)
4. **Tenant Service** (depends on PostgreSQL)
5. **Shop Service** (depends on PostgreSQL, Redis)
6. **Order Service** (depends on PostgreSQL, RabbitMQ)
7. **Procurement Gateway** (depends on PostgreSQL, API Gateway)
8. **API Gateway** (depends on Auth, Tenant)
9. **Frontend** (depends on API Gateway)

**Configuration in AppHost:**
```csharp
var authService = builder
    .AddProject<Projects.B2X_AuthService>("auth-service")
    .WithReference(postgres)  // Wait for postgres
    .WaitFor(postgres);        // Explicit wait condition
```

### 3.4 Environment Variable Management

**Automatic Injection:**

When a service references another service:
```csharp
var authService = builder.AddProject("auth-service");
var apiGateway = builder
    .AddProject("api-gateway")
    .WithReference(authService);  // Injects as env var
```

The API Gateway automatically gets:
```
ASPNETCORE_AUTH_SERVICE_URL=http://auth-service:5001
ASPNETCORE_AUTH_SERVICE_HTTP_ENDPOINT_0=http://auth-service:5001
```

**Usage in Service:**
```csharp
var authUrl = builder.Configuration["ASPNETCORE_AUTH_SERVICE_URL"];
// or via dependency injection
services.AddHttpClient("auth", (sp, client) => {
    var url = sp.GetService<IConfiguration>()["ASPNETCORE_AUTH_SERVICE_URL"];
    client.BaseAddress = new Uri(url);
});
```

---

## 4. Dashboard & Observability

### 4.1 Aspire Dashboard

**Access:**
- URL: `http://localhost:15500`
- Port: 15500
- Protocol: HTTP

**Features:**

#### Resources View
- Lists all services and resources
- Current port assignments
- Running/Stopped status
- Last startup time

#### Logs View
- Real-time logs from all services
- Structured logging (JSON)
- Log level filtering
- Search & grep functionality

#### Metrics View
- CPU usage per service
- Memory consumption
- HTTP request metrics
- Custom business metrics

#### Traces View
- Distributed tracing with OpenTelemetry
- Request flow visualization
- Performance analysis
- Dependency mapping

### 4.2 Health Status Indicators

```
🟢 Healthy    - Service running, health checks passing
🟡 Degraded   - Service running, health checks failing
🔴 Unhealthy  - Service not responding
⚫ Starting    - Service initializing
⚪ Stopped     - Service intentionally stopped
```

---

## 5. Service Configuration

### 5.1 Service Defaults

All services implement `AddServiceDefaults()`:

```csharp
// In each service's Program.cs
builder.Host.AddServiceDefaults();
app.UseServiceDefaults();
```

This provides:
- Automatic health checks
- Structured logging
- Metrics collection
- Tracing setup
- CORS handling
- **Rate limiting** (required by UseServiceDefaults)

#### ⚠️ Important: AddServiceDefaults Overloads

**The ServiceDefaults library provides TWO extension method overloads** - ensure you use the correct one:

| Builder Type | Method | Usage Pattern |
|--------------|--------|---------------|
| `IHostBuilder` | `builder.Host.AddServiceDefaults()` | Generic .NET Host |
| `IHostApplicationBuilder` | `builder.AddServiceDefaults()` | WebApplication.CreateBuilder() |

**Both overloads MUST maintain feature parity** including:
- Health checks (`AddHealthChecks`)
- Rate limiting (`AddRateLimiter`)
- Service discovery
- OpenTelemetry

**Common Error**: If you see `InvalidOperationException: Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddRateLimiter'`, this means the service is using an `AddServiceDefaults()` overload that doesn't register rate limiting, but `UseServiceDefaults()` calls `app.UseRateLimiter()`.

**Exit Code -532462766** (0xE0434352) typically indicates this misconfiguration.

See [KB-LESSONS] for full diagnosis and resolution details.

### 5.2 Startup Configuration

**launchSettings.json Template:**
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:PORT",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### 5.3 Dependency Injection

**In AppHost (Program.cs):**

```csharp
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("B2X-db");

var redis = builder.AddRedis("redis");

var apiGateway = builder
    .AddProject<Projects.B2X_ApiGateway>("api-gateway")
    .WithReference(authService)
    .WithReference(tenantService)
    .WithHttpEndpoint(port: 5000);

var authService = builder
    .AddProject<Projects.B2X_AuthService>("auth-service")
    .WithReference(postgres)
    .WithHttpEndpoint(port: 5001);
```

---

## 6. Startup Procedures

### 6.1 Local Development

**Automatic Startup (Recommended):**
```bash
# Terminal: Run Full Stack from VS Code
Cmd+Shift+D → "Full Stack" → F5
```

This automatically:
1. Builds all projects
2. Starts AppHost (Aspire orchestrator)
3. Mounts all services in correct order
4. Opens Aspire Dashboard
5. Injects environment variables

**Manual Startup:**
```bash
# Terminal: Aspire Host
cd backend/services/AppHost
dotnet run

# This starts all dependent services automatically
```

### 6.2 Container Deployment

**Docker Compose (Future):**
```yaml
services:
  apphost:
    image: B2X:aspire
    ports:
      - "15500:15500"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - postgres
      - rabbitmq
```

**Kubernetes (Future):**
- Helm charts for Aspire services
- StatefulSets for databases
- ConfigMaps for environment variables
- Service discovery via DNS

---

## 7. Health Check Flow

### Request Flow with Health Checks

```
┌────────────────────────────────────────────────┐
│  Frontend sends request to API Gateway         │
│  http://localhost:5000/api/users               │
└────────────┬─────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────┐
│  API Gateway                                   │
│  - Checks own health ✅                         │
│  - Propagates tenant context                  │
│  - Routes to Auth Service                      │
└────────────┬─────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────┐
│  Auth Service                                  │
│  - Checks own health ✅                         │
│  - Verifies JWT token                         │
│  - Returns auth info                          │
└────────────┬─────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────┐
│  Response returned to Frontend                │
│  - Status 200 OK with auth data               │
│  - All services healthy during request        │
└────────────────────────────────────────────────┘
```

### Health Check Monitoring (Aspire Dashboard)

```
Every 30 seconds:

1. Aspire queries /health endpoint on each service
2. Services check their dependencies
3. Dashboard updates with status
4. Alerts if unhealthy
5. Logs any failures
```

---

## 8. Troubleshooting

### Service Shows "Unhealthy"

**Diagnosis Steps:**

1. Check logs in Aspire Dashboard
2. Verify database connectivity
3. Test health endpoint manually:
   ```bash
   curl http://localhost:5001/health
   ```
4. Check environment variables
5. Verify port availability

**Common Causes:**

| Issue | Solution |
|-------|----------|
| Database not accessible | Ensure PostgreSQL is running on port 5432 |
| Environment variables missing | Check AppHost service registration |
| Port already in use | Kill existing process: `kill -9 $(lsof -ti:PORT)` |
| Service crash on startup | Check service logs in dashboard |
| Slow startup | Increase health check timeout |

### Service Exits with Code -532462766 or 1

**Exit Code -532462766** (0xE0434352) = CLR unhandled exception
**Exit Code 1** = General error

**Most Common Cause**: Missing service registration for middleware used in `UseServiceDefaults()`.

**Diagnosis Steps:**
1. Open Aspire Dashboard (http://localhost:15500)
2. Click on the failing service
3. Go to **Console** tab
4. Look for `System.InvalidOperationException` with message about missing services

**Common Error Messages:**

| Error | Missing Registration | Fix |
|-------|---------------------|-----|
| `IServiceCollection.AddRateLimiter` | Rate limiter not registered | Add `AddRateLimiter()` in `AddServiceDefaults()` |
| `IServiceCollection.AddHealthChecks` | Health checks not registered | Add `AddHealthChecks()` in `AddServiceDefaults()` |

**Root Cause**: The `IHostApplicationBuilder.AddServiceDefaults()` overload may be missing registrations that `UseServiceDefaults()` expects.

**Quick Fix**: Verify both `AddServiceDefaults()` overloads in `B2X.ServiceDefaults/Extensions.cs` have matching service registrations.

### File Lock Errors During Build (CS2012/MSB3026/MSB3027)

**Errors**:
- `CS2012: Cannot open 'X.dll' for writing -- The process cannot access the file`
- `MSB3027: Could not copy "apphost.exe" to "B2X.*.exe". The file is being used by another process.`

**Cause**: This is a **known MSBuild issue** ([dotnet/sdk#9585](https://github.com/dotnet/sdk/issues/9585) - open since 2018). Race conditions occur during parallel builds when multiple projects reference shared libraries.

**Prevention Strategy** (Implemented):

1. **Build Tasks Use Limited Parallelism**:
   - All VS Code build tasks now use `-m:2` instead of `-m`
   - This limits parallel MSBuild workers to 2 (vs unlimited)
   - Balances build speed with stability

2. **Pre-Build Check Script**:
   ```powershell
   # Check for locked files before building
   .\scripts\pre-build-check.ps1
   
   # Auto-clean locked files and kill processes
   .\scripts\pre-build-check.ps1 -AutoClean
   ```

3. **Manual Recovery** (when locks occur):
   ```powershell
   # Windows: Kill all dotnet processes
   Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
   
   # Linux/macOS
   pkill -9 dotnet
   
   # Wait 3 seconds
   Start-Sleep 3
   
   # Clean specific obj folder if needed
   Remove-Item "path/to/project/obj" -Recurse -Force
   
   # Rebuild with limited parallelism
   dotnet build -m:2
   ```

**Key Insight**: The `-maxcpucount:1` or `-m:2` workaround is used by many teams, including those at Microsoft. Full parallelism (`-m`) is known to cause race conditions.

### File Lock Errors During Aspire Startup/Rebuild (CS2012 - Runtime)

**Error**: `CS2012: Cannot open 'B2X.Shared.Kernel.dll' for writing -- The process cannot access the file`

**Context**: This happens when **Aspire is already running** and you try to rebuild a project. This is **different** from the parallel build race condition above.

**Root Cause**: When using `dotnet run` to start AppHost, Aspire doesn't automatically stop and restart individual services during rebuild. The running process holds locks on DLLs.

**Reference**: [dotnet/aspire#4981](https://github.com/dotnet/aspire/issues/4981) - "Cannot rebuild projects due to exe file being locked" (reported by Steve Sanderson, Microsoft)

**Official Answer from David Fowler** (Aug 2025):
> "When you use `dotnet run` it's expected. When you use Visual Studio or VS Code now and rebuild a specific project, it will restart that project without intervention."

**Solutions** (in order of preference):

1. **Use VS Code's Integrated Debugging** (Recommended):
   - Run AppHost from VS Code (F5 or Ctrl+F5)
   - VS Code coordinates builds and restarts services automatically
   - This is how the Aspire team intended development to work

2. **Use `aspire run` CLI** (Alternative):
   ```bash
   # Instead of: dotnet run --project AppHost
   aspire run
   ```
   - The Aspire CLI handles hot reload better than `dotnet run`

3. **Use `dotnet watch`** (For hot reload):
   ```bash
   dotnet watch --project src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj
   ```
   - Enables hot reload with automatic service restart

4. **Dashboard-Based Restart** (Manual workaround):
   - Open Aspire Dashboard (http://localhost:15500)
   - Stop the specific service you want to rebuild
   - Build the project
   - Start the service again

5. **Stop All and Rebuild** (Last resort):
   ```powershell
   # Stop Aspire completely
   Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
   
   # Clean and rebuild
   .\scripts\pre-build-check.ps1 -AutoClean
   dotnet build src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj -m:2
   
   # Restart Aspire
   dotnet run --project src/backend/Infrastructure/Hosting/AppHost/B2X.AppHost.csproj
   ```

**Two Types of File Locks**:
| Type | Cause | Solution |
|------|-------|----------|
| Build-time race | Parallel MSBuild workers | Use `-m:2` flag |
| Runtime lock | Aspire holding DLLs | Use VS Code F5 or `aspire run` |

### Service Not Registered

**Check:**
1. Is service project referenced in AppHost.csproj?
2. Does service have `AddServiceDefaults()`?
3. Are dependencies declared in AppHost?

**Fix:**
```csharp
// In AppHost/Program.cs
var myService = builder
    .AddProject<Projects.MyServiceNamespace>("my-service")
    .WithHttpEndpoint(port: 5xxx);
```

---

## 9. Best Practices

### 1. Service Design
- ✅ Implement health checks in every service
- ✅ Use service discovery for inter-service communication
- ✅ Inject dependencies via AppHost
- ✅ Use structured logging (Serilog)

### 2. Configuration
- ✅ Never hardcode service URLs
- ✅ Use environment variables from AppHost
- ✅ Implement graceful shutdown handlers
- ✅ Set appropriate health check timeouts

### 3. Monitoring
- ✅ Watch Aspire Dashboard during development
- ✅ Check logs for warnings early
- ✅ Monitor health checks regularly
- ✅ Set alerts for unhealthy services

### 4. Testing
- ✅ Test inter-service communication
- ✅ Verify health checks work
- ✅ Test with missing dependencies
- ✅ Simulate service failures

---

## 10. Aspire vs. Manual Management

### Why Aspire?

| Feature | Manual | Aspire |
|---------|--------|--------|
| Service startup | Multiple terminals | Single command |
| Port assignment | Manual tracking | Automatic |
| Dependency ordering | Manual sequencing | Automatic |
| Health checks | Manual setup | Built-in |
| Environment variables | Manual injection | Automatic |
| Service discovery | Manual URL hardcoding | Automatic DNS |
| Logging aggregation | Multiple log streams | Unified dashboard |
| Metrics collection | Manual instrumentation | Built-in |
| Dashboard | Third-party tools | Native |

---

## 11. Future Roadmap

### Phase 1 (Current)
- ✅ Local development with Aspire
- ✅ Service discovery
- ✅ Health checks
- ✅ Dashboard observability

### Phase 2 (Q1 2026)
- Docker containers for each service
- Kubernetes deployment files
- Production health checks
- Distributed tracing

### Phase 3 (Q2 2026)
- Service mesh (Istio)
- Advanced traffic management
- Circuit breakers
- Resilience policies

### Phase 4 (Q3 2026)
- Multi-region deployment
- Service federation
- Global load balancing
- Disaster recovery

---

## Conclusion

Aspire provides a modern, integrated platform for managing B2X's microservices ecosystem. It handles the complexity of service orchestration, allowing developers to focus on business logic rather than infrastructure concerns.

**Key Takeaway:** All B2X services are now managed through a single, unified orchestration platform (Aspire AppHost), providing automatic discovery, health monitoring, and observability for the entire system.

---

**Document Version:** 1.0  
**Last Updated:** December 25, 2025  
**Status:** Active
