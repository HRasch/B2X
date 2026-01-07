# B2Connect Technical Documentation - Aspire Update

## Documentation Changes & Additions (Dec 25, 2025)

### New Documents

1. **[aspire-orchestration-specs.md](aspire-orchestration-specs.md)**
   - Complete .NET Aspire specification
   - Service topology and registry
   - Orchestration features explained
   - Health check system details
   - Dashboard usage guide
   - Troubleshooting procedures

2. **[ASPIRE_IMPLEMENTATION_SUMMARY.md](../ASPIRE_IMPLEMENTATION_SUMMARY.md)**
   - High-level implementation overview
   - Architecture changes explained
   - Service management procedures
   - Quick reference guide
   - Benefits summary

3. **[UNHEALTHY_SERVICES.md](../UNHEALTHY_SERVICES.md)**
   - Root cause analysis for unhealthy services
   - Health check debugging
   - Service configuration reference
   - Best practices

### Updated Documents

1. **[architecture.md](architecture.md)**
   - Added "Service Orchestration with .NET Aspire" section
   - Explained AppHost role
   - Service registration procedures
   - Health check strategy
   - Benefits comparison table

2. **[STARTUP_GUIDE.md](../STARTUP_GUIDE.md)**
   - Updated with Aspire startup instructions
   - Removed old manual setup steps
   - Added Aspire Dashboard documentation
   - New health check procedures

### Reference Documents

- **[SOCKET_ERRORS.md](../SOCKET_ERRORS.md)** - Socket/CORS troubleshooting
- **[SOCKET_FIXES.md](../SOCKET_FIXES.md)** - Technical details of fixes
- **[DEBUG_GUIDE.md](../DEBUG_GUIDE.md)** - VS Code debugging setup

---

## Key Architecture Changes

### Service Orchestration

All services now controlled via **AppHost (Aspire)**:

```
AppHost (Port 15500)
├─ PostgreSQL (managed resource)
├─ RabbitMQ (managed resource)
├─ API Gateway (Port 5000)
├─ Auth Service (Port 5001)
├─ Tenant Service (Port 5002)
├─ Shop Service (Port 5003)
├─ Order Service (Port 5004)
├─ Procurement Gateway (Port 5005)
├─ Catalog Service (Port 5006)
├─ Inventory Service (Port 5007)
└─ Supplier Service (Port 5010)
```

### Service Discovery

- **Automatic**: Services discover each other through Aspire
- **DNS-like**: `http://service-name:port`
- **Environment Variables**: Automatically injected
- **No Hardcoding**: URLs generated at runtime

### Health Checks

- **Automatic**: Built-in `/health` endpoints
- **Monitoring**: Aspire checks every 30 seconds
- **Dashboard**: Real-time status display
- **Levels**: Liveness, Readiness, Startup probes

---

## Technical Specifications

### Service Registry

| Service | Port | Framework | Health Check |
|---------|------|-----------|--------------|
| API Gateway | 5000 | .NET 8 | ✅ Implemented |
| Auth Service | 5001 | .NET 8 | ✅ Implemented |
| Tenant Service | 5002 | .NET 8 | ✅ Implemented |
| Shop Service | 5003 | .NET 8 | ✅ Ready |
| Order Service | 5004 | .NET 8 | ✅ Ready |
| Procurement Gateway | 5005 | .NET 8 | ✅ Ready |
| Catalog Service | 5006 | .NET 8 | ✅ Ready |
| Inventory Service | 5007 | .NET 8 | ✅ Ready |
| Supplier Service | 5010 | .NET 8 | ✅ Ready |

### Launch Settings Files (New)

Created `Properties/launchSettings.json` for:
- ✅ API Gateway (Port 5000)
- ✅ Auth Service (Port 5001)
- ✅ Tenant Service (Port 5002)

### Configuration Files

**Modified:**
- `backend/services/api-gateway/Program.cs` - Added CORS
- `backend/services/AppHost/Program.cs` - Simplified health checks

**Created:**
- `backend/services/api-gateway/Properties/launchSettings.json`
- `backend/services/auth-service/Properties/launchSettings.json`
- `backend/services/tenant-service/Properties/launchSettings.json`

---

## Startup Procedures

### Development (Local)

**Via VS Code (Recommended):**
```
Debug Tab (Cmd+Shift+D) → "Full Stack" → F5
```

**Via Terminal:**
```bash
cd backend/services/AppHost
dotnet run
```

**Automated Setup:**
```bash
./start-all.sh
```

### Dashboard Access

**Aspire Dashboard:**
- URL: `http://localhost:15500`
- Shows: Services, Logs, Metrics, Traces
- Real-time: Updates as services run

---

## Health Check Implementation

### Automatic Health Endpoints

Each service implements via `AddServiceDefaults()`:

```
GET /health/live     → Is service running?
GET /health/ready    → Ready for requests?
GET /health/startup  → Initialization done?
GET /health          → Combined status
```

### Status Indicators

```
🟢 Healthy   - All checks passing
🟡 Degraded  - Some checks failing
🔴 Unhealthy - Service not responding
⚫ Starting   - Initialization in progress
```

### Manual Verification

```bash
# Check individual services
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health

# Check AppHost
curl http://localhost:15500/api/health
```

---

## Environment Variable Management

### Automatic Injection

When services reference each other in AppHost:

```csharp
var authService = builder
    .AddProject("auth-service")
    .WithHttpEndpoint(port: 5001);

var apiGateway = builder
    .AddProject("api-gateway")
    .WithReference(authService);  // Auto-injects env vars
```

**Generated Environment Variables:**
```
ASPNETCORE_AUTH_SERVICE_URL=http://auth-service:5001
ASPNETCORE_AUTH_SERVICE_HTTP_ENDPOINT_0=http://auth-service:5001
```

### Usage in Services

```csharp
// No hardcoding needed
var authUrl = configuration["ASPNETCORE_AUTH_SERVICE_URL"];
// or via HttpClient factory
var client = httpClientFactory.CreateClient("auth");
```

---

## Logging & Observability

### Structured Logging

All services use Serilog:
```csharp
builder.Host.UseSerilog((context, config) =>
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration)
);
```

### Log Aggregation

**Via Aspire Dashboard:**
- Combined logs from all services
- Filter by service, level, timestamp
- Search functionality
- Structured log viewing

### Metrics Collection

**Prometheus-compatible:**
- CPU per service
- Memory usage
- HTTP request metrics
- Custom business metrics

### Distributed Tracing

**OpenTelemetry Integration:**
- Request flow visualization
- Service-to-service tracing
- Performance analysis
- Dependency mapping

---

## Troubleshooting Guide

### Services Show Unhealthy

**Root Cause:** Service dependencies not available
**Solution:** See [UNHEALTHY_SERVICES.md](../UNHEALTHY_SERVICES.md)

### Socket Errors

**Root Cause:** CORS/Connection issues
**Solution:** See [SOCKET_ERRORS.md](../SOCKET_ERRORS.md)

### Service Won't Start

**Steps:**
1. Check logs in Aspire Dashboard
2. Verify database connectivity
3. Check port availability: `lsof -i :PORT`
4. Review service launchSettings.json

### Can't Connect Between Services

**Steps:**
1. Verify services are registered in AppHost
2. Check environment variables in Dashboard
3. Test health endpoint: `curl http://localhost:PORT/health`
4. Check network logs in Dashboard

---

## Best Practices

✅ **Always use Aspire** for local development  
✅ **Monitor Dashboard** during development  
✅ **Check Health Endpoints** when troubleshooting  
✅ **Use Structured Logging** for diagnostics  
✅ **Let Aspire Inject** environment variables  
✅ **Never Hardcode** service URLs  
✅ **Implement Health Checks** in new services  
✅ **Use Service Defaults** in all services  

---

## Migration From Manual Setup

### Old Process
```bash
# Multiple terminals needed
T1: npm run dev                    (Frontend)
T2: dotnet run                     (API Gateway)
T3: dotnet run                     (Auth Service)
T4: dotnet run                     (Tenant Service)
T5: docker-compose up              (Databases)
Manual: Check logs across terminals
Manual: Manage URLs and ports
```

### New Process
```bash
# Single command
cd backend/services/AppHost && dotnet run

# Or VS Code
Cmd+Shift+D → "Full Stack" → F5

Dashboard: All logs, metrics, traces in one place
Automatic: Service discovery and health checks
```

---

## Future Roadmap

### Phase 1 ✅ (Complete)
- Local development with Aspire
- Service discovery
- Health checks
- Dashboard

### Phase 2 (Q1 2026)
- Docker containers
- Kubernetes deployment
- Production health checks
- CI/CD integration

### Phase 3 (Q2 2026)
- Service mesh (Istio)
- Advanced traffic management
- Resilience patterns
- Circuit breakers

### Phase 4 (Q3 2026)
- Multi-region deployment
- Global load balancing
- Disaster recovery
- Service federation

---

## Documentation Index

### Architecture & Design
- [Full Architecture Documentation](architecture.md)
- [Aspire Orchestration Specification](aspire-orchestration-specs.md)
- [Implementation Summary](../ASPIRE_IMPLEMENTATION_SUMMARY.md)

### Startup & Configuration
- [Startup Guide](../STARTUP_GUIDE.md)
- [Debug Guide](../DEBUG_GUIDE.md)
- [Health Check Procedures](../UNHEALTHY_SERVICES.md)

### Troubleshooting
- [Socket Errors Guide](../SOCKET_ERRORS.md)
- [Socket Fixes Reference](../SOCKET_FIXES.md)
- [Service Configuration](../UNHEALTHY_SERVICES.md)

### API & Integration
- [API Specifications](api-specifications.md)
- [Shop Platform Specs](shop-platform-specs.md)
- [Procurement Gateway Specs](procurement-gateway-specs.md)

---

## Summary

B2Connect now has a **modern, unified service orchestration platform** powered by **.NET Aspire**. All services are:

- **Centrally managed** through AppHost
- **Automatically discovered** by other services
- **Continuously monitored** for health
- **Fully observable** via Dashboard
- **Production-grade** ready

This eliminates the complexity of manual service coordination while providing enterprise-grade observability and reliability.

---

**Last Updated:** December 25, 2025  
**Status:** Aspire Orchestration Complete  
**Next Review:** Q1 2026
