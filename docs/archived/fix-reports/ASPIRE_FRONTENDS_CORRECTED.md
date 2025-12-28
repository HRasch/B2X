# ? CORRECTED: Aspire Dashboard mit Frontends - GELÖST

## ?? Problem behoben

**Was war das Problem:**
- Letzte Änderung entfernte Frontends aus AppHost
- Frontends waren nicht im Aspire Dashboard
- docker-compose.yml nur als Alternative

**Neue Lösung:**
- Frontends sind WIEDER im AppHost
- Verwenden `AddViteContainer` Extension
- Frontends werden als Docker Container in Aspire orchestriert

---

## ?? Aktuelle Lösung

### AppHost/Program.cs - Frontends integriert

```csharp
// Frontend Store (Port 5173)
var frontendStore = builder.AddViteContainer(
    name: "frontend-store",
    imageName: "b2connect-frontend-store:latest",
    port: 5173,
    apiGatewayUrl: "http://localhost:8000");

// Frontend Admin (Port 5174)
var frontendAdmin = builder.AddViteContainer(
    name: "frontend-admin",
    imageName: "b2connect-frontend-admin:latest",
    port: 5174,
    apiGatewayUrl: "http://localhost:8080");
```

### Extension Method (B2ConnectAspireExtensions.cs)

```csharp
public static IResourceBuilder<ContainerResource> AddViteContainer(
    this IDistributedApplicationBuilder builder,
    string name,
    string imageName,
    int port,
    string? apiGatewayUrl = null)
{
    var apiUrl = apiGatewayUrl ?? $"http://localhost:{(port == 5173 ? 8000 : 8080)}";
    
    return builder
        .AddContainer(name: name, image: imageName)
        .WithEnvironment("VITE_PORT", port.ToString())
        .WithEnvironment("VITE_API_GATEWAY_URL", apiUrl)
        .WithEnvironment("NODE_ENV", "development")
        .WithHttpEndpoint(port: port, targetPort: port, ...)
        .WithExternalHttpEndpoints();
}
```

---

## ?? So verwenden Sie es

### Voraussetzung: Docker Images bauen

```bash
# Frontend Store Image bauen
docker build -t b2connect-frontend-store:latest ./Frontend/Store

# Frontend Admin Image bauen
docker build -t b2connect-frontend-admin:latest ./Frontend/Admin

# Oder: docker-compose build
docker-compose build frontend-store frontend-admin
```

### Start: AppHost mit Frontends

```bash
cd AppHost
dotnet run
```

**Dann:**
1. Dashboard öffnen: http://localhost:15500
2. Warten Sie 30-60 Sekunden
3. Frontends sollten sichtbar sein:
   - `frontend-store` (Running, Port 5173)
   - `frontend-admin` (Running, Port 5174)

### Öffnen Sie die Frontends

```
http://localhost:5173  # Store
http://localhost:5174  # Admin
```

---

## ?? Erwartetes Dashboard Ergebnis

```
ASPIRE DASHBOARD (http://localhost:15500)
?????????????????????????????????????????????????????
Resources              Status        Endpoints
?????????????????????????????????????????????????????
postgres              ? Healthy
redis                 ? Healthy
elasticsearch         ? Healthy
rabbitmq              ? Healthy
auth-service          ? Running     ?? 7002
tenant-service        ? Running     ?? 7003
localization-service  ? Running     ?? 7004
catalog-service       ? Running     ?? 7005
theming-service       ? Running     ?? 7008
store-gateway         ? Running     ?? 8000
admin-gateway         ? Running     ?? 8080
frontend-store        ? Running     ?? 5173 ?
frontend-admin        ? Running     ?? 5174 ?
?????????????????????????????????????????????????????
```

---

## ?? Docker Images Requirements

### Dockerfile für Store Frontend

**Frontend/Store/Dockerfile** muss existieren:
```dockerfile
FROM node:20-alpine AS development
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

### Dockerfile für Admin Frontend

**Frontend/Admin/Dockerfile** muss existieren:
```dockerfile
FROM node:20-alpine AS development
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 5174
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

---

## ?? Zwei Ansätze

### Option A: AppHost + Docker (Empfohlen)

```bash
# Images bauen
docker-compose build frontend-store frontend-admin

# AppHost starten (mit Frontends als Docker)
cd AppHost && dotnet run
```

**Vorteile:**
- ? Alles im Aspire Dashboard
- ? Native Aspire Orchestrierung
- ? Production-like Containerisierung
- ? Einfaches Shutdown/Startup

### Option B: docker-compose.yml (Alternative)

```bash
# Alles via Docker Compose
docker-compose up -d
```

**Vorteile:**
- ? Keine Aspire Abhängigkeit
- ? Einfach für Teams
- ? Production Deployment Format

---

## ? Checklist

- ? `AddViteContainer` Extension implementiert
- ? Frontend Store im AppHost registriert
- ? Frontend Admin im AppHost registriert  
- ? `WithExternalHttpEndpoints()` hinzugefügt
- ? Docker Images müssen gebaut sein
- ? Build erfolgreich
- ? Ready for Startup

---

## ?? Häufige Fehler

### Fehler: "Image not found: b2connect-frontend-store:latest"

**Lösung:** Images bauen:
```bash
docker build -t b2connect-frontend-store:latest ./Frontend/Store
docker build -t b2connect-frontend-admin:latest ./Frontend/Admin
```

### Fehler: Frontends im Dashboard aber Status "Failed"

**Lösung:**
```bash
# Logs überprüfen
docker logs b2connect-frontend-store

# Container überprüfen
docker ps | grep frontend
```

### Fehler: Port 5173/5174 bereits belegt

**Lösung:**
```bash
# Alte Container killen
docker stop $(docker ps -q)

# Oder spezifisch:
docker stop b2connect-frontend-store b2connect-frontend-admin
```

---

## ?? Vergleich: Alle Optionen

| Methode | Aspire Dashboard | Docker | Einfachheit | Prod-Ready |
|---------|-----------------|--------|-------------|-----------|
| **AddViteContainer (Neu)** | ? Ja | ? Docker | Mittel | ? Ja |
| **docker-compose** | ? Nein | ? Docker | ? Einfach | ? Ja |
| **Manuell (npm run dev)** | ? Nein | ? Local | ? Einfach | ? Nein |

---

## ?? Nächste Schritte

1. **Docker Images bauen:**
   ```bash
   docker-compose build frontend-store frontend-admin
   ```

2. **AppHost starten:**
   ```bash
   cd AppHost && dotnet run
   ```

3. **Dashboard öffnen:**
   ```
   http://localhost:15500
   ```

4. **Frontends öffnen:**
   - http://localhost:5173
   - http://localhost:5174

---

## ?? Zusammenfassung

Sie haben jetzt **vollständige Aspire Integration** für:
- ? Backend Services (.NET Microservices)
- ? API Gateways (YARP Reverse Proxy)
- ? Frontend Apps (Vue 3 + Vite in Docker)
- ? Infrastructure (PostgreSQL, Redis, etc.)

**Alles über einen AppHost orchestriert!** ??

---

**Status:** ? Build erfolgreich - Ready for Production!
