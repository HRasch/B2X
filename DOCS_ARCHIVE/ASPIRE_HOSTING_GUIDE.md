# B2Connect Aspire Hosting Configuration Guide

## Overview

B2Connect ist nun vollständig für zentrales Hosting über Apache Aspire 10 konfiguriert. Das System verwendet ein verteiltes Microservices-Architektur mit zentralisierter Service-Orchestration.

## Architektur

```
┌─────────────────────────────────────────────────────────┐
│                     AppHost (9000)                      │
│              (Central Orchestrator & Dashboard)         │
│  - Service Discovery & Registry                         │
│  - Centralized Health Checks                            │
│  - Request Routing                                      │
└────────┬──────────┬──────────┬──────────┬───────────────┘
         │          │          │          │
    ┌────▼───┐ ┌───▼────┐ ┌──▼────┐ ┌───▼────┐
    │Gateway │ │  Auth  │ │Tenant │ │  I18n  │
    │(5000)  │ │(5001)  │ │(5002) │ │(5003)  │
    └────┬───┘ └───┬────┘ └──┬────┘ └───┬────┘
         │          │         │         │
    ┌────▼──────────▼─────────▼─────────▼────┐
    │         PostgreSQL (5432)              │
    │     & Redis Cache (6379)               │
    └─────────────────────────────────────────┘
```

## Komponenten

### 1. AppHost (Zentrale Orchestration)
- **Port**: 9000
- **Verantwortung**: Service-Registrierung, Health-Checks, Request-Routing
- **Features**:
  - Automatische Service-Erkennung
  - Zentralisiertes Logging (Serilog)
  - Health-Endpoint mit Diagnostiken
  - Dashboard-UI für Monitoring

### 2. API Gateway
- **Port**: 5000
- **Verantwortung**: Request-Aggregation, Rate-Limiting, Authentication-Weiterleitung

### 3. Auth Service
- **Port**: 5001
- **Verantwortung**: Benutzer-Management, JWT-Token, Session-Handling

### 4. Tenant Service
- **Port**: 5002
- **Verantwortung**: Mandanten-Verwaltung, Isolation, Daten-Partitionierung

### 5. Localization Service
- **Port**: 5003
- **Verantwortung**: Mehrsprachige Unterstützung, Übersetzungen

### 6. Datenbank
- **PostgreSQL 16**: Primäre Datenspeicherung
- **Redis 7**: Caching und Session-Storage

## Deployment-Optionen

### Option 1: Lokale Entwicklung (Bash-Orchestration)

```bash
# Alle Services starten
./aspire-start.sh Development Debug

# Alle Services stoppen
./aspire-stop.sh

# Health-Status prüfen
curl http://localhost:9000/api/health
```

**Voraussetzungen**:
- .NET 10 SDK
- PostgreSQL 16 lokal installiert ODER Docker für PostgreSQL
- Redis 7 lokal installiert ODER Docker für Redis
- Bash Shell

**Vorteile**:
- Schnell lokal nutzbar
- Vollständiges Debugging
- Schnelle Iteration

### Option 2: Docker Compose (Containerisiert)

```bash
# Stack starten
docker-compose -f docker-compose.aspire.yml up -d

# Logs anzeigen
docker-compose -f docker-compose.aspire.yml logs -f

# Stack stoppen
docker-compose -f docker-compose.aspire.yml down
```

**Voraussetzungen**:
- Docker 20.10+
- Docker Compose 2.0+
- 4GB+ RAM verfügbar

**Services**:
- PostgreSQL unter postgres:5432
- Redis unter redis:6379
- API Gateway unter api-gateway:5000
- Auth Service unter auth-service:5001
- Tenant Service unter tenant-service:5002
- Localization Service unter localization-service:5003
- AppHost unter apphost:9000

**Volumes**:
- `postgres-data`: PostgreSQL Datenspeicherung
- `redis-data`: Redis Persistenz

### Option 3: Kubernetes (Production)

```bash
# Namespace erstellen
kubectl create namespace b2connect

# Secrets anlegen
kubectl create secret generic db-secrets \
  --from-literal=postgres-password='your-secure-password' \
  --from-literal=auth-connection-string='Server=postgres;...' \
  -n b2connect

# Deployment anwenden
kubectl apply -f kubernetes/aspire-deployment.yaml

# Status prüfen
kubectl get all -n b2connect

# AppHost-Dashboard aufrufen
kubectl port-forward -n b2connect svc/apphost 9000:9000
```

**Oder mit Helm**:

```bash
# Chart installieren
helm install b2connect ./backend/kubernetes/helm \
  --namespace b2connect \
  --create-namespace \
  --values backend/kubernetes/helm/values.yaml

# Status prüfen
helm status b2connect -n b2connect

# Upgrade
helm upgrade b2connect ./backend/kubernetes/helm \
  --namespace b2connect \
  --values backend/kubernetes/helm/values.yaml
```

**Kubernetes Features**:
- Automatisches Scaling (HPA für Services)
- Health-basierte Pod-Replicas
- Service Discovery durch DNS
- LoadBalancer für AppHost
- StatefulSets für Datenbanken
- RBAC für Sicherheit
- Resource Limits und Requests

## Umgebungskonfiguration

### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "Services": {
    "ApiGateway": "http://localhost:5000",
    "AuthService": "http://localhost:5001",
    "TenantService": "http://localhost:5002",
    "LocalizationService": "http://localhost:5003"
  },
  "HealthCheck": {
    "Enabled": true,
    "Interval": 10000
  }
}
```

### Production (appsettings.Production.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Services": {
    "ApiGateway": "https://api-gateway.b2connect.local",
    "AuthService": "https://auth-service.b2connect.local",
    "TenantService": "https://tenant-service.b2connect.local",
    "LocalizationService": "https://localization-service.b2connect.local"
  },
  "HealthCheck": {
    "Enabled": true,
    "Interval": 30000
  }
}
```

## Service Discovery

### Statische Konfiguration

Services sind in `appsettings.json` definiert:

```csharp
public class ServiceRegistry
{
    public string ApiGateway { get; set; }
    public string AuthService { get; set; }
    public string TenantService { get; set; }
    public string LocalizationService { get; set; }
}
```

### Dynamische Service Discovery (Kubernetes)

Im Kubernetes-Deployment werden Services automatisch entdeckt:

```bash
# Service URLs in Kubernetes
http://api-gateway:5000
http://auth-service:5001
http://tenant-service:5002
http://localization-service:5003
```

## Health Checks

### Endpoint: GET /api/health

**Request**:
```http
GET http://localhost:9000/api/health
```

**Response** (200 OK):
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:45Z",
  "version": "1.0.0",
  "uptime": "512.5 MB",
  "diagnostics": {
    "totalCheckTime": 42,
    "serviceCount": 4,
    "healthyServices": 4
  },
  "services": {
    "apiGateway": {
      "status": "healthy",
      "responseTime": 8,
      "statusCode": 200,
      "error": null
    },
    "authService": {
      "status": "healthy",
      "responseTime": 12,
      "statusCode": 200,
      "error": null
    },
    "tenantService": {
      "status": "healthy",
      "responseTime": 10,
      "statusCode": 200,
      "error": null
    },
    "localizationService": {
      "status": "healthy",
      "responseTime": 12,
      "statusCode": 200,
      "error": null
    }
  }
}
```

**Degraded Status** (200 OK, aber mit Fehlern):
```json
{
  "status": "degraded",
  "services": {
    "authService": {
      "status": "unhealthy",
      "responseTime": 5000,
      "statusCode": null,
      "error": "The operation timed out"
    }
  }
}
```

**Unhealthy Status** (503 Service Unavailable):
```json
{
  "status": "unhealthy",
  "timestamp": "2024-01-15T10:31:00Z",
  "diagnostics": {
    "healthyServices": 2,
    "serviceCount": 4
  }
}
```

## Logging & Observability

### Serilog-Konfiguration

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "B2Connect")
    .Enrich.WithProperty("Environment", "Production")
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/b2connect-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

### Log-Struktur

```
logs/
├── b2connect-2024-01-15.log
├── b2connect-2024-01-14.log
├── apphost/
│   ├── apphost.log
├── api-gateway/
│   ├── api-gateway.log
├── auth-service/
│   ├── auth-service.log
├── tenant-service/
│   ├── tenant-service.log
└── localization-service/
    └── localization-service.log
```

## CORS-Konfiguration

### Development
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

### Production
```csharp
options.AddPolicy("AllowFrontend", policy =>
{
    policy
        .WithOrigins("https://app.b2connect.local", "https://admin.b2connect.local")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});
```

## Performance-Tuning

### Container Resource-Limits

```yaml
# Empfohlene Limits pro Service:
# AppHost: 500m CPU, 512Mi Memory
# Services: 250m CPU, 256Mi Memory
# PostgreSQL: 1000m CPU, 2Gi Memory
# Redis: 500m CPU, 512Mi Memory
```

### Health Check Intervals

- **Development**: 10 Sekunden (schnellere Rückmeldung)
- **Production**: 30 Sekunden (reduzierte Overhead)

### Timeouts

- **Development**: 5 Sekunden
- **Production**: 10 Sekunden

## Sicherheit

### Kubernetes Secrets

```bash
# Secrets erstellen
kubectl create secret generic db-secrets \
  --from-literal=postgres-password='secure-password' \
  --from-literal=auth-connection-string='...' \
  -n b2connect
```

### RBAC

AppHost hat ClusterRole mit Berechtigung für:
- Services listing
- Endpoints viewing
- Deployments monitoring

### TLS/SSL

```yaml
# Kubernetes Ingress mit Cert-Manager
ingress:
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
  tls:
    enabled: true
    secretName: b2connect-tls
```

## Troubleshooting

### Service startet nicht

```bash
# Logs prüfen
docker-compose logs api-gateway

# oder in Kubernetes
kubectl logs -n b2connect deployment/api-gateway
```

### Health Check schlägt fehl

```bash
# AppHost Health-Endpoint prüfen
curl -v http://localhost:9000/api/health

# Spezifischen Service testen
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
```

### Datenbankverbindung fehlgeschlagen

```bash
# PostgreSQL testen
psql -h localhost -U b2connect -d b2connect_db

# Redis testen
redis-cli ping
```

### Service-Entdeckung funktioniert nicht

```bash
# In Kubernetes
kubectl get endpoints -n b2connect
kubectl get services -n b2connect
kubectl get pods -n b2connect -o wide
```

## Migration zwischen Umgebungen

### Von lokal zu Docker

```bash
# Lokal stoppen
./aspire-stop.sh

# Docker starten
docker-compose -f docker-compose.aspire.yml up -d
```

### Von Docker zu Kubernetes

```bash
# Docker-Images erstellen
docker build -t b2connect-apphost ./backend/services/AppHost
docker build -t b2connect-api-gateway ./backend/services/api-gateway
# ... weitere Services

# Images in Registry pushen
docker push your-registry/b2connect-apphost:latest

# Kubernetes Werte aktualisieren
helm upgrade b2connect ./backend/kubernetes/helm \
  --set apphost.image.repository=your-registry/b2connect-apphost
```

## Monitoring & Metriken

### Prometheus Integration (Optional)

```yaml
# ServiceMonitor für Prometheus
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: b2connect-services
  namespace: b2connect
spec:
  selector:
    matchLabels:
      monitoring: "true"
  endpoints:
  - port: metrics
    interval: 30s
```

### Grafana Dashboards

Dashboard JSON-Templates befinden sich in:
- `infrastructure/grafana/dashboards/`

## Häufig gestellte Fragen (FAQ)

**F: Kann ich Services einzeln neu starten?**
A: Ja, mit `docker-compose restart service-name` oder `kubectl rollout restart deployment/service-name`

**F: Wie konfiguriere ich Datenbankverbindungen?**
A: Über Environment-Variablen in `appsettings.*.json` oder Kubernetes Secrets

**F: Wie skaliere ich Services?**
A: In Kubernetes mit `kubectl scale deployment/api-gateway --replicas=5` oder HPA

**F: Wie beobachte ich Performance?**
A: Health-Endpoint für Basis-Checks, Prometheus/Grafana für detaillierte Metriken

**F: Wie sichere ich den produktiven Stack?**
A: RBAC, Network Policies, Secrets Management, TLS Encryption

## Nächste Schritte

1. **CI/CD Integration**: GitHub Actions / GitLab CI für automatisches Deployment
2. **Monitoring**: Prometheus + Grafana Setup
3. **Logging**: ELK Stack oder Loki Integration
4. **Service Mesh**: Istio für erweiterte Netzwerkfunktionen
5. **Backup & Recovery**: PostgreSQL Backups, Disaster Recovery

## Support und Dokumentation

- **API Specs**: [docs/api-specifications.md](docs/api-specifications.md)
- **Architektur**: [docs/architecture.md](docs/architecture.md)
- **Tenant Isolation**: [docs/tenant-isolation.md](docs/tenant-isolation.md)

---

**Version**: 1.0.0  
**Letztes Update**: 2024-01-15  
**Maintainer**: B2Connect Team
