# B2X Service Port Plan

**DocID**: `ARCH-PORTS-001`  
**Title**: Service Port Assignments and Planning  
**Owner**: @DevOps  
**Status**: Active  
**Created**: 2026-01-11  
**Last Updated**: 2026-01-11  

---

## Overview

This document outlines the port assignments for all B2X services in development and production environments. The port plan ensures no conflicts and follows standard conventions for microservices architecture.

### Key Principles

- **Development**: Fixed ports for external-facing services, dynamic ports for internal services via Aspire Service Discovery
- **Production**: Service discovery via Kubernetes/Docker Compose, no fixed ports required
- **Security**: External ports use standard ranges (8000-8999), internal services use dynamic assignment
- **Scalability**: Ports support horizontal scaling and load balancing

---

## Infrastructure Services

| Service | Development Port | Production Port | Protocol | Description |
|---------|------------------|-----------------|----------|-------------|
| **PostgreSQL** | 5432 | 5432 | TCP | Primary database |
| **Redis** | 6379 | 6379 | TCP | Cache and session store |
| **Elasticsearch** | 9200 | 9200 | HTTP | Search and analytics |
| **RabbitMQ** | 5672 | 5672 | AMQP | Message queue |
| **RabbitMQ Management** | 15672 | 15672 | HTTP | Management UI |

---

## API Gateways (External Facing)

| Service | Development Port | Production Port | Protocol | Frontend | Description |
|---------|------------------|-----------------|----------|----------|-------------|
| **Reverse Proxy** | 5000 | 80/443 | HTTP/HTTPS | All | Main entry point, tenant routing |
| **Store Gateway** | 8000 | Service Discovery | HTTP | Store (5173) | Public read-only endpoints |
| **Admin Gateway** | 8080 | Service Discovery | HTTP | Admin (5174) | Admin/management endpoints |

---

## Microservices (Internal)

| Service | Development Port | Production Port | Protocol | Database | Description |
|---------|------------------|-----------------|----------|----------|-------------|
| **Auth Service** | Dynamic (Aspire) | Service Discovery | HTTP | auth | Identity, JWT, Passkeys |
| **Tenant Service** | Dynamic (Aspire) | Service Discovery | HTTP | tenant | Multi-tenancy management |
| **Localization Service** | Dynamic (Aspire) | Service Discovery | HTTP | localization | i18n, translations |
| **Catalog Service** | Dynamic (Aspire) | Service Discovery | HTTP | catalog | Product catalog, search |
| **Variants Service** | Dynamic (Aspire) | Service Discovery | HTTP | store | Product variants/SKUs |
| **Categories Service** | Dynamic (Aspire) | Service Discovery | HTTP | store | Product categories |
| **Theming Service** | Dynamic (Aspire) | Service Discovery | HTTP | layout | UI themes, customization |
| **Smart Data Integration** | Dynamic (Aspire) | Service Discovery | HTTP | smartdataintegration | ERP integration |
| **Monitoring Service** | Dynamic (Aspire) | Service Discovery | HTTP | monitoring | Health monitoring, logging |

---

## Management & Tools

| Service | Development Port | Production Port | Protocol | Description |
|---------|------------------|-----------------|----------|----------|-------------|
| **MCP Server** | 8090 | Service Discovery | HTTP | AI assistant for management |
| **Seeding API** | 8095 | Service Discovery | HTTP | Test data management |
| **Aspire Dashboard** | 15500 | N/A | HTTP | Development orchestration UI |

---

## Frontend Applications

| Application | Development Port | Production Port | Protocol | Gateway | Description |
|-------------|------------------|-----------------|----------|---------|-------------|
| **Store Frontend** | 5173 | 80/443 | HTTP | 8000 | Customer-facing e-commerce |
| **Admin Frontend** | 5174 | 80/443 | HTTP | 8080 | Admin/management interface |
| **Management Frontend** | 5175 | 80/443 | HTTP | 8080 | Tenant management portal |

---

## Development Environment Setup

### Port Ranges
- **Infrastructure**: 5000-6999 (PostgreSQL, Redis, etc.)
- **Gateways**: 8000-8999 (Store: 8000, Admin: 8080)
- **Frontends**: 5173-5175 (Vite dev servers)
- **Tools**: 15000+ (Aspire: 15500, MCP: 8090, Seeding: 8095)
- **Internal Services**: Dynamic (assigned by Aspire)

### Environment Variables
```bash
# Frontend environment variables
VITE_API_GATEWAY_URL=http://localhost:8000  # Store frontend
VITE_API_GATEWAY_URL=http://localhost:8080  # Admin/Management frontends
VITE_MCP_SERVER_URL=http://localhost:8090   # Management frontend

# Backend environment variables
ASPNETCORE_URLS=http://localhost:8000       # Store gateway
ASPNETCORE_URLS=http://localhost:8080       # Admin gateway
ASPNETCORE_URLS=http://localhost:8090       # MCP server
```

---

## Production Deployment Considerations

### Kubernetes Service Types
- **LoadBalancer**: Reverse Proxy (port 80/443)
- **ClusterIP**: Internal services (no external ports)
- **NodePort**: Development/debugging only

### Ingress Configuration
```yaml
# Example ingress for reverse proxy
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: b2x-reverse-proxy
spec:
  rules:
  - host: store.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: reverse-proxy
            port:
              number: 80
```

### Service Discovery
- **Development**: Aspire Service Discovery (automatic)
- **Production**: Kubernetes DNS or service mesh (Istio/Linkerd)
- **No hardcoded ports** in production configurations

---

## Port Conflict Resolution

### Common Conflicts
1. **PostgreSQL**: Default 5432, conflict with local installations
   - Solution: Use Docker container or change local PostgreSQL port

2. **Redis**: Default 6379, conflict with local installations
   - Solution: Use Docker container or change local Redis port

3. **Elasticsearch**: Default 9200, conflict with local installations
   - Solution: Use Docker container or change local Elasticsearch port

### Testing Ports
```bash
# Check if ports are available
netstat -ano | findstr :8000
netstat -ano | findstr :8080

# Kill processes using ports (Windows)
Stop-Process -Id (Get-NetTCPConnection -LocalPort 8000).OwningProcess -Force
```

---

## Monitoring and Health Checks

### Health Check Endpoints
All services expose `/health` endpoints on their assigned ports.

### Port Monitoring
- **Development**: Aspire Dashboard shows service status
- **Production**: Kubernetes liveness/readiness probes
- **Logging**: Structured logs include port binding information

---

## Future Considerations

### IPv6 Support
- All ports support both IPv4 and IPv6
- No changes required for dual-stack deployment

### Service Mesh Integration
- Istio/Linkerd can provide dynamic port management
- Service discovery becomes mesh-native

### Load Balancing
- External ports (5000, 8000, 8080) support load balancers
- Internal services scale horizontally without port conflicts

---

## Change History

| Date | Change | Author |
|------|--------|--------|
| 2026-01-11 | Initial port plan created | @DevOps |
| 2026-01-11 | Added production considerations | @DevOps |

---

**Related Documents**:
- [ARCH-001] Project Structure
- [ARCH-011] Hosting Infrastructure
- [WF-014] Deployment Guide</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\docs\architecture\components\PORT_ASSIGNMENTS.md