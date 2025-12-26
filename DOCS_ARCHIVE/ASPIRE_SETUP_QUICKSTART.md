# Aspire Service Discovery Setup - Quick Start Guide

## Overview

This guide explains how to use ASP.NET Aspire for central service discovery in the B2Connect application. Service discovery eliminates hardcoded ports and allows services to find each other automatically.

## Architecture

```
┌────────────────────────────────────────────────┐
│           Development Environment               │
├────────────────────────────────────────────────┤
│                                                │
│  Frontend (5173/5174)                          │
│      ↓                                         │
│  Vite Proxy (env: VITE_API_GATEWAY_URL)       │
│      ↓                                         │
│  API Gateway (port 6000)                       │
│      ↓                                         │
│  ┌─────────────────────────────────────┐      │
│  │ Backend Services (Service Discovery)│      │
│  ├─────────────────────────────────────┤      │
│  │ • Auth Service (5001)               │      │
│  │ • Tenant Service (5002)             │      │
│  │ • Localization Service (5003)       │      │
│  └─────────────────────────────────────┘      │
│                                                │
└────────────────────────────────────────────────┘
```

## Quick Start

### 1. Stop Any Running Services

```bash
bash /Users/holger/Documents/Projekte/B2Connect/stop-services-local.sh
```

### 2. Start All Backend Services with Service Discovery

```bash
bash /Users/holger/Documents/Projekte/B2Connect/start-services-local.sh
```

This script will:
- ✓ Kill any existing processes
- ✓ Start all backend services on their configured ports
- ✓ Verify services are running
- ✓ Display service URLs and configuration

### 3. Start Frontend Services (in separate terminals)

**Option A: Frontend (port 5173)**
```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

**Option B: Admin Frontend (port 5174)**
```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend-admin
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

## Service Ports (Local Development)

| Service | Port | URL |
|---------|------|-----|
| API Gateway | 6000 | http://localhost:6000 |
| Auth Service | 5001 | http://localhost:5001 |
| Tenant Service | 5002 | http://localhost:5002 |
| Localization Service | 5003 | http://localhost:5003 |
| Frontend | 5173 | http://localhost:5173 |
| Admin Frontend | 5174 | http://localhost:5174 |

## How Service Discovery Works

### Service Names (Aspire/Docker)
When running services under Aspire orchestration or Docker, services reference each other using service names:
```
http://api-gateway:8080
http://auth-service:8080
http://tenant-service:8080
```

### Local Development
For local development, services still use these names, but DNS resolution maps them to localhost:
- `http://api-gateway:8080` → `http://localhost:6000`
- `http://auth-service:8080` → `http://localhost:5001`
- `http://tenant-service:8080` → `http://localhost:5002`

Services configure their actual ports in `launchSettings.json`.

## Configuration Files

### Backend Service Discovery (appsettings.json)

All backend services use environment variables for service discovery, with fallbacks to appsettings:

```csharp
var serviceUrl = Environment.GetEnvironmentVariable("SERVICES__AUTHSERVICE")
    ?? config["Services:AuthService"]
    ?? "http://auth-service:8080";
```

### Frontend Vite Configuration (vite.config.ts)

Frontend Vite proxies API calls based on environment variable:

```typescript
proxy: {
  '/api': {
    target: process.env.VITE_API_GATEWAY_URL || 'http://api-gateway:8080',
    changeOrigin: true,
  },
}
```

### Environment Files

**`.env.local`** files in frontend directories set local development variables:
```bash
VITE_API_GATEWAY_URL=http://localhost:6000
```

## Running with Docker/Aspire

When deploying with Docker Compose or Aspire orchestration:

```bash
docker-compose -f backend/docker-compose.aspire.yml up
```

Services automatically discover each other using internal service names - no environment variables needed.

## Troubleshooting

### "Failed to load resource" Error
**Cause**: Vite proxy points to wrong API Gateway URL  
**Solution**: Verify `VITE_API_GATEWAY_URL` environment variable is set:
```bash
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

### Service Not Responding
**Check service logs**:
```bash
tail -f /tmp/b2connect-api-gateway.log
tail -f /tmp/b2connect-auth-service.log
```

**Verify service is running**:
```bash
curl http://localhost:6000/health
```

### Port Already in Use
**Kill existing processes**:
```bash
bash /Users/holger/Documents/Projekte/B2Connect/stop-services-local.sh
```

### Services Can't Find Each Other
**For Docker/containers**: Verify all services are on the same Docker network  
**For local dev**: Check that service names resolve correctly in `appsettings.json`

## Advanced: Running Individual Services

You can start services manually for debugging:

```bash
# Terminal 1: API Gateway
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/api-gateway
dotnet run

# Terminal 2: Auth Service
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/auth-service
dotnet run

# Terminal 3: Tenant Service
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/tenant-service
dotnet run

# Terminal 4: Frontend
cd /Users/holger/Documents/Projekte/B2Connect/frontend
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

## Service Discovery Environment Variables

| Variable | Purpose | Default | Example |
|----------|---------|---------|---------|
| `VITE_API_GATEWAY_URL` | Frontend API Gateway address | `http://api-gateway:8080` | `http://localhost:6000` |
| `SERVICES__APIGATEWAY` | Backend Gateway service name | `http://api-gateway:8080` | - |
| `SERVICES__AUTHSERVICE` | Backend Auth Service name | `http://auth-service:8080` | - |
| `SERVICES__TENANTSERVICE` | Backend Tenant Service name | `http://tenant-service:8080` | - |
| `SERVICES__LOCALIZATIONSERVICE` | Localization Service name | `http://localization-service:8080` | - |

## Testing Service Discovery

### Verify API Gateway
```bash
curl http://localhost:6000/health
```

### Verify Auth Service
```bash
curl http://localhost:5001/health
```

### Test Authentication Flow
```bash
curl -X POST http://localhost:6000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"password"}'
```

## File Locations

- Start Script: `/Users/holger/Documents/Projekte/B2Connect/start-services-local.sh`
- Stop Script: `/Users/holger/Documents/Projekte/B2Connect/stop-services-local.sh`
- Documentation: `/Users/holger/Documents/Projekte/B2Connect/ASPIRE_SERVICE_DISCOVERY.md`

## Next Steps

1. ✅ Configure service discovery in `appsettings.json`
2. ✅ Update frontend Vite to use environment variables
3. ✅ Create startup/shutdown scripts
4. ⏳ Deploy to Docker/Aspire (see docker-compose.aspire.yml)
5. ⏳ Set up CI/CD pipeline for service discovery

## Additional Resources

- [ASP.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Service Discovery Patterns](https://microservices.io/patterns/service-discovery.html)
- [Docker Networking](https://docs.docker.com/engine/network/)

