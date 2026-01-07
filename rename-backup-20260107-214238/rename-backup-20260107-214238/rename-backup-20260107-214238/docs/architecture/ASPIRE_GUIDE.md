# Aspire Orchestration & AppHost Guide

## Overview

.NET Aspire is a cloud-ready stack for building observable, production-ready distributed applications. In B2X, Aspire orchestrates all microservices (Catalog Service, Auth Service, Search Service, Order Service) and provides development tooling.

**Key Resources:**
- Aspire Dashboard: `http://localhost:9000` (when running)
- AppHost: `backend/services/AppHost/Program.cs`
- See `.copilot-specs.md` Section 20 for detailed architecture

## Quick Start

### Start Aspire with Debugging
```bash
# VS Code: F5 → "AppHost (Debug)" or "Full Stack (AppHost + Admin Frontend)"
# Or via terminal:
cd backend/services/AppHost
dotnet run
```

### Services Orchestrated

| Service | Port | Type | Status Endpoint |
|---------|------|------|-----------------|
| AppHost | 9000 | Gateway/Dashboard | http://localhost:9000 |
| Catalog Service | 9001 | gRPC/REST | http://localhost:9001 |
| Auth Service | 9002 | REST | http://localhost:9002 |
| Search Service | 9003 | REST | http://localhost:9003 |
| Order Service | 9004 | REST | http://localhost:9004 |

## Configuration

### Environment Variables

Set in `aspire-start.sh` or VS Code launch config:

```bash
# Feature Flags
ELASTICSEARCH_ENABLED=true          # Enable search indexing
EVENTVALIDATION_ENABLED=true        # Enable event validation
LOCALIZATION_ENABLED=true           # Enable i18n support
ASPIRE_DASHBOARD_ENABLED=true       # Show Aspire dashboard

# Service Ports
CATALOG_SERVICE_PORT=9001
AUTH_SERVICE_PORT=9002
SEARCH_SERVICE_PORT=9003
ORDER_SERVICE_PORT=9004

# Environment
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:9000
```

### AppHost Program.cs Pattern

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Redis cache
var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithPersistence();

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithInitializationSql("init.sql");

// Add services
builder.AddProject<Projects.CatalogService>("catalogservice")
    .WithReference(postgres)
    .WithReference(redis)
    .WithEnvironment("CATALOG_SERVICE_PORT", "9001");

builder.AddProject<Projects.AuthService>("authservice")
    .WithReference(postgres)
    .WithEnvironment("AUTH_SERVICE_PORT", "9002");

// Launch
await builder.Build().RunAsync();
```

## Debugging

### Via VS Code

1. **F5** → Select "AppHost (Debug)"
2. Set breakpoints in `AppHost/Program.cs`
3. Step through service initialization
4. Open Aspire Dashboard when ready

### Via Terminal with Watch Mode

```bash
cd backend/services/AppHost
dotnet watch run
```

Automatically rebuilds and restarts on file changes.

## Dashboard Features

**When Aspire is running** (http://localhost:9000):

- **Resources**: View all running services, databases, caches
- **Logs**: Real-time logs from each service
- **Traces**: Distributed trace visualization (OpenTelemetry)
- **Metrics**: Performance metrics (CPU, memory, requests)
- **Environment**: View env variables and configuration

## Common Tasks

### Add a New Microservice

1. Create service project: `backend/services/NewService/`
2. Add to AppHost Program.cs:
   ```csharp
   builder.AddProject<Projects.NewService>("newservice")
       .WithReference(postgres)
       .WithEnvironment("PORT", "9005");
   ```
3. Run `dotnet run` from AppHost
4. Service is now orchestrated

### Check Service Health

```bash
curl http://localhost:9001/health    # Catalog
curl http://localhost:9002/health    # Auth
curl http://localhost:9003/health    # Search
curl http://localhost:9004/health    # Order
```

### View Logs for Specific Service

In Aspire Dashboard → Resources → Click service → Logs tab

### Rebuild Just One Service

```bash
cd backend/services/CatalogService
dotnet build
```

Then restart Aspire with `dotnet run` from AppHost.

## Troubleshooting

### "AppHost won't start"
- Check port 9000 is available: `lsof -i :9000`
- Kill existing process: `pkill -f B2X.AppHost`
- Clean build: `dotnet clean && dotnet build`

### "Service not responding"
- Check Aspire Dashboard → Resources for failures
- View logs in dashboard
- Run service standalone to debug:
  ```bash
  cd backend/services/CatalogService
  dotnet run --no-build
  ```

### "Port conflicts"
Change in `aspire-start.sh` or AppHost Program.cs:
```csharp
.WithEnvironment("CATALOG_SERVICE_PORT", "9010")  // Use 9010 instead
```

## Best Practices

**DO:**
- Use Aspire Dashboard to monitor services
- Review logs in dashboard before debugging code
- Use `WithReference()` for service dependencies
- Enable persistence for development data
- Set meaningful service names in AppHost

**DON'T:**
- Run services independently in production (use Aspire)
- Ignore service startup errors
- Hard-code service URLs (use service references)
- Leave orphaned processes running

## References

- [Official Aspire Docs](https://learn.microsoft.com/en-us/dotnet/aspire/)
- `.copilot-specs.md` Section 20 (Architecture & Aspire)
- `VSCODE_ASPIRE_CONFIG.md` (Debug configurations)
- `DEBUG_QUICK_REFERENCE.md` (Quick launch options)
