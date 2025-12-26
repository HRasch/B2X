# Aspire Service Discovery Configuration

## Overview

This application uses ASP.NET Aspire for centralized service discovery. Instead of hardcoding port numbers, services are referenced by their service names, which allows the system to work seamlessly in both local development and containerized environments.

## Service Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    AppHost (Service Orchestrator)             │
│                     (localhost:15500)                         │
│  - Service Discovery                                          │
│  - Health Monitoring                                          │
│  - Configuration Management                                   │
└────────────────┬────────────────────────────────────────────┘
                 │
    ┌────────────┼────────────┬──────────────┐
    │            │            │              │
    ▼            ▼            ▼              ▼
┌─────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐
│   API   │ │   Auth   │ │  Tenant  │ │Localization │
│ Gateway │ │ Service  │ │ Service  │ │  Service    │
│ :8080   │ │  :8080   │ │  :8080   │ │   :8080     │
└────┬────┘ └──────────┘ └──────────┘ └─────────────┘
     │
     │  (Reverse Proxy & Forwarding)
     │
┌────▼──────────────────────────────────┐
│    Frontend (Vite Proxy)               │
│  localhost:5173 (main)                 │
│  localhost:5174 (admin)                │
└──────────────────────────────────────┘
```

## Configuration

### Service Names (Aspire Internal)
- `api-gateway` → Port 8080
- `auth-service` → Port 8080
- `tenant-service` → Port 8080
- `localization-service` → Port 8080

### Local Development (Docker Compose / Aspire)

Services communicate using service names:
```
http://auth-service:8080
http://api-gateway:8080
http://tenant-service:8080
http://localization-service:8080
```

### Frontend Development (Vite Proxy)

For local development, set environment variable:
```bash
VITE_API_GATEWAY_URL=http://localhost:6000
```

This points the Vite proxy to the locally running API Gateway.

## Configuration Files

### Backend Service Configuration

**AppHost - `/backend/services/AppHost/appsettings.json`:**
```json
{
  "Services": {
    "ApiGateway": "http://api-gateway:8080",
    "AuthService": "http://auth-service:8080",
    "TenantService": "http://tenant-service:8080",
    "LocalizationService": "http://localization-service:8080"
  }
}
```

**API Gateway - `/backend/services/api-gateway/appsettings.json`:**
```json
{
  "ReverseProxy": {
    "Clusters": {
      "auth-service": {
        "Destinations": {
          "auth-service-dest": {
            "Address": "http://auth-service:8080"
          }
        }
      }
    }
  }
}
```

**Auth Service - `/backend/services/auth-service/appsettings.json`:**
```json
{
  "Auth": {
    "Authority": "http://auth-service:8080"
  }
}
```

### Frontend Configuration

**Environment Variables (`.env.local`):**
```bash
# Local development
VITE_API_GATEWAY_URL=http://localhost:6000

# Aspire/Docker
VITE_API_GATEWAY_URL=http://api-gateway:8080
```

## How It Works

1. **AppHost Orchestration**
   - Starts all services with dynamic port assignment
   - Provides service discovery endpoints
   - Manages health checks and configuration

2. **Service-to-Service Communication**
   - Services reference each other by name (e.g., `http://auth-service:8080`)
   - Network discovery resolves service names to actual addresses
   - Works across containers, VMs, and local development

3. **Frontend to Backend Communication**
   - Frontend makes requests to `/api/*`
   - Vite proxy forwards to API Gateway (port determined by `VITE_API_GATEWAY_URL`)
   - API Gateway forwards to appropriate backend service

## Running the Application

### Option 1: Local Development with Aspire
```bash
# AppHost will start all services with service discovery
cd backend/services/AppHost
dotnet run

# In another terminal, start frontend
cd frontend
npm run dev

# Set environment variable to connect to local gateway
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

### Option 2: Docker Compose with Service Discovery
```bash
docker-compose up

# Services will communicate using service names
# No port configuration needed
```

### Option 3: Manual Service Startup
```bash
# Terminal 1: API Gateway
cd backend/services/api-gateway
dotnet run --launch-profile https

# Terminal 2: Auth Service
cd backend/services/auth-service
dotnet run --launch-profile https

# Terminal 3: Frontend
cd frontend
VITE_API_GATEWAY_URL=http://localhost:6000 npm run dev
```

## Service Discovery Resolution

### Inside Containers (Docker Network)
```
Client → frontend:5174
  ↓
Vite Proxy
  ↓
API Gateway at http://api-gateway:8080
  ↓
Auth Service at http://auth-service:8080
```

### Local Development
```
Client → localhost:5174
  ↓
Vite Proxy (configured via VITE_API_GATEWAY_URL)
  ↓
API Gateway at http://localhost:6000
  ↓
Auth Service at http://localhost:5001 (or configured address)
```

## Environment Variables

| Variable | Purpose | Default | Example |
|----------|---------|---------|---------|
| `VITE_API_GATEWAY_URL` | Frontend Vite proxy target | `http://api-gateway:8080` | `http://localhost:6000` |
| `SERVICES__APIGATEWAY` | Backend AppHost gateway address | `http://api-gateway:8080` | `http://localhost:6000` |
| `SERVICES__AUTHSERVICE` | Backend AppHost auth address | `http://auth-service:8080` | `http://localhost:5001` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | `Development` | `Production` |

## Health Checks

AppHost provides comprehensive health monitoring:
```bash
curl http://localhost:15500/api/health
```

Returns status of all registered services.

## Troubleshooting

### "Failed to load resource" Error
Check Vite proxy configuration. Ensure `VITE_API_GATEWAY_URL` points to correct address.

### Service Not Found
Verify service names in `appsettings.json` match actual service names in Aspire configuration.

### Port Conflicts
Aspire automatically assigns available ports. Check AppHost dashboard (localhost:15500) for actual ports in use.

## Benefits

✅ **No Hardcoded Ports** - Services are location-independent
✅ **Seamless Scaling** - Add/remove services without configuration changes
✅ **Container Ready** - Works with Docker, Kubernetes, and local development
✅ **Service Isolation** - Each service runs independently
✅ **Dynamic Discovery** - Services register/deregister automatically

