# B2Connect Aspire Startup Guide

## Overview

B2Connect uses **.NET Aspire** for centralized microservice orchestration. Aspire provides a unified dashboard for monitoring, managing, and debugging all backend services.

## Quick Start

### Start Complete Environment
```bash
# Start both backend (Aspire) and frontend
./start-all.sh

# With custom configuration
./start-all.sh Production Release 5200
```

### Start Backend Only
```bash
# Start .NET Aspire orchestration
./aspire-start.sh

# With custom port
./aspire-start.sh Development Debug 5200
```

### Stop Services
```bash
# Stop all services gracefully
./aspire-stop.sh
```

## Aspire Configuration

### Service Ports

| Service | Port | Purpose |
|---------|------|---------|
| **AppHost** | 5200 | Aspire orchestration & API gateway |
| **Dashboard** | 5500 | Aspire management dashboard |
| **Frontend** | 5173 | Vite dev server (frontend-admin) |

### Environment Variables

**AppHost Configuration:**
```bash
ASPNETCORE_ENVIRONMENT=Development          # dev/staging/production
ASPNETCORE_URLS=http://+:5200              # Service URL binding
DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true  # Dashboard access
```

**Service Discovery:**
- Services auto-register with AppHost
- Health checks run continuously
- Service mesh networking enabled
- Distributed tracing configured

## Directory Structure

```
B2Connect/
├── aspire-start.sh              # Start backend services
├── aspire-stop.sh               # Stop backend services
├── start-all.sh                 # Start complete environment
├── logs/                         # Service logs directory
│   └── apphost.log             # Main orchestration log
├── .pids/                       # Process ID files
│   └── AppHost.pid             # AppHost process ID
└── backend/services/
    └── AppHost/                 # Aspire orchestration project
        ├── Program.cs           # Aspire configuration
        ├── appsettings.json     # Service settings
        └── appsettings.Development.json
```

## Aspire AppHost Features

### Service Orchestration
- Central management of all microservices
- Automatic service discovery
- Health monitoring
- Container management (Docker integration)

### Built-in Dashboard
- Real-time service status
- Performance metrics
- Distributed tracing
- Log aggregation
- Environment management

### Service Integration
Services configured in AppHost:
- **API Gateway** - Central entry point
- **Auth Service** - Authentication/Authorization
- **Tenant Service** - Multi-tenancy management
- **Localization Service** - i18n support
- **Theme Service** - Theme management
- **Layout Service** - Layout management

## Running Services

### Using start-all.sh

```bash
# Start both backend and frontend
./start-all.sh

# With custom environment
./start-all.sh Development Debug 5200
```

**What it does:**
1. Kills old processes
2. Starts Aspire backend on port 5200
3. Starts Vite frontend on port 5173
4. Keeps both running with proper cleanup

### Using aspire-start.sh

```bash
# Start only backend services
./aspire-start.sh

# Development mode
./aspire-start.sh Development Debug

# Production mode
./aspire-start.sh Production Release 5200
```

**What it does:**
1. Validates prerequisites
2. Restores NuGet packages
3. Builds AppHost project
4. Starts Aspire orchestration
5. Provides health check feedback

### Using aspire-stop.sh

```bash
# Stop all services gracefully
./aspire-stop.sh
```

**What it does:**
1. Sends SIGTERM to services
2. Waits 5 seconds for graceful shutdown
3. Force kills remaining processes
4. Cleans up PID files

## Accessing Services

### Development Workflow

**Local Development:**
```
Frontend:              http://localhost:5173
Backend API:           http://localhost:5200/api
Aspire Dashboard:      http://localhost:5500
Health Check:          http://localhost:5200/health
```

**Service Health Endpoints:**
```bash
# Check AppHost health
curl http://localhost:5200/health

# Get service status
curl http://localhost:5200/api/health/detailed
```

### Aspire Dashboard

Access at: **http://localhost:5500**

Features:
- 📊 Service status monitoring
- 📈 Real-time metrics
- 🔍 Distributed tracing
- 📝 Log viewing
- 🔗 Service discovery
- ⚙️ Configuration management

## Troubleshooting

### Port Already in Use

```bash
# Find process using port 5200
lsof -i :5200

# Kill process
kill -9 <PID>

# Use different port
./aspire-start.sh Development Debug 5300
```

### Services Not Starting

```bash
# Check logs
tail -f logs/apphost.log

# Verify .NET installation
dotnet --version

# Restore packages
cd backend/services/AppHost
dotnet restore
```

### Graceful Shutdown Failed

```bash
# Force stop all processes
pkill -f "dotnet run"
pkill -f "dotnet watch"

# Clean up PID files
rm -rf .pids/

# Start fresh
./aspire-stop.sh
./aspire-start.sh
```

## Log Locations

```
logs/
├── apphost.log      # Main orchestration service
├── services.log     # Service startup logs
└── requests.log     # HTTP request logs
```

**View logs in real-time:**
```bash
tail -f logs/apphost.log
```

## Configuration Files

### appsettings.json
```json
{
  "Services": {
    "ApiGateway": "http://localhost:5000",
    "AuthService": "http://localhost:5001"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

## Advanced Configuration

### Custom Service Ports

Edit `backend/services/AppHost/appsettings.json`:
```json
{
  "Services": {
    "ApiGateway": "http://localhost:5000",
    "AuthService": "http://localhost:5001",
    "TenantService": "http://localhost:5002",
    "LocalizationService": "http://localhost:5003"
  }
}
```

### Enable Distributed Tracing

```csharp
// In AppHost Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());
```

### CORS Configuration

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000",
      "https://yourdomain.com"
    ]
  }
}
```

## Performance Tuning

### Optimize Startup Time

```bash
# Use Release build for faster startup
./aspire-start.sh Development Release
```

### Monitor Resource Usage

```bash
# View memory usage
ps aux | grep dotnet

# Monitor CPU usage
top
```

### Connection Pooling

Configured automatically in AppHost with:
- HTTP connection pooling
- Database connection pooling
- Service discovery caching

## Security

### Development Security

- Dashboard access restricted to localhost
- Anonymous access disabled in production
- API key management integrated
- Request/response logging with sanitization

### Production Deployment

```bash
# Build for production
./aspire-start.sh Production Release

# Environment-specific secrets
export ASPNETCORE_ENVIRONMENT=Production
./aspire-start.sh Production Release
```

## Scripting Integration

### Health Check Script

```bash
#!/bin/bash
# Check service health
curl -s http://localhost:5200/health | jq .
```

### Automated Startup

```bash
# Start on system boot
@reboot /path/to/B2Connect/aspire-start.sh Development Debug
```

## Common Commands

```bash
# View running services
ps aux | grep dotnet

# View open ports
lsof -i -P -n | grep LISTEN

# View logs in real-time
tail -f logs/apphost.log

# Restart services
./aspire-stop.sh && sleep 2 && ./aspire-start.sh

# Full environment restart
./aspire-stop.sh && sleep 2 && ./start-all.sh

# Check service status
curl http://localhost:5200/health/ready
```

## FAQ

**Q: How do I access the Aspire dashboard?**
A: Navigate to http://localhost:5500 in your browser.

**Q: Can I run services on different ports?**
A: Yes, pass the port as a parameter: `./aspire-start.sh Development Debug 8000`

**Q: How do I view detailed logs?**
A: Use `tail -f logs/apphost.log` to watch logs in real-time.

**Q: What if a service fails to start?**
A: Check the logs and verify all prerequisites are installed (dotnet SDK).

**Q: How do I switch between Development and Production?**
A: Use `./aspire-start.sh Production Release`

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Service Orchestration Best Practices](https://docs.microsoft.com/aspire)
- [B2Connect Architecture Documentation](./DOCUMENTATION.md)
- [Troubleshooting Guide](./DEBUG_GUIDE.md)

---

**Version:** 2.0  
**Last Updated:** December 25, 2024  
**Maintained by:** B2Connect Development Team
