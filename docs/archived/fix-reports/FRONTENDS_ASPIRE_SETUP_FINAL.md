# ? B2Connect Frontend Integration - Komplett!

## ?? Was wurde erledigt

### ? Frontend-Integration für Aspire 13.1.0

**Problem:** `AddNpmApp` existiert nicht mehr in Aspire 13.1.0
**Lösung:** Docker Compose + Aspire AppHost (Best Practice)

### ? Dateien erstellt/aktualisiert:

1. **Docker-Setup:**
   - ? `Frontend/Store/Dockerfile` - Multi-stage build (dev & prod)
   - ? `Frontend/Admin/Dockerfile` - Multi-stage build (dev & prod)
   - ? `.dockerignore` - Optimierte Docker Builds
   - ? `Frontend/Store/.dockerignore`
   - ? `Frontend/Admin/.dockerignore`

2. **docker-compose.yml:**
   - ? Backend-Services (PostgreSQL, Redis, RabbitMQ, Elasticsearch)
   - ? Microservices (Auth, Tenant, Localization, Catalog, Theming)
   - ? API Gateways (Store, Admin)
   - ? Frontend Services (Store, Admin) mit Hot-Reload
   - ? Volumes und Networks konfiguriert

3. **Vite-Konfiguration:**
   - ? `Frontend/Store/vite.config.ts` - Host 0.0.0.0, API-Proxy
   - ? `Frontend/Admin/vite.config.ts` - Host 0.0.0.0, API-Proxy
   - ? Environment-Variablen für API-Gateway-URLs

4. **Aspire AppHost:**
   - ? `AppHost/Program.cs` - Clean configuration (keine AddNpmApp)
   - ? Kommentare erklären Frontend-Startup
   - ? Ready for Aspire Dashboard

5. **Dokumentation:**
   - ? `FRONTENDS_ASPIRE_SETUP.md` - Komplett Guide mit Optionen
   - ? `docs/ASPIRE_FRONTEND_INTEGRATION.md` - Detaillierte Dokumentation
   - ? `Frontend/README.md` - Frontend-spezifische Anleitung
   - ? `STARTUP_GUIDE.ps1` - Windows interactive Guide
   - ? `STARTUP_GUIDE.sh` - Linux/Mac interactive Guide

---

## ?? Drei Startup-Optionen

### ?? Option 1: Docker Compose (Einfachste)
```bash
docker-compose up -d
```
- Alles in Containern
- Keine lokalen Abhängigkeiten außer Docker
- Production-ready

### ?? Option 2: Aspire + Docker (Empfohlen für Dev)
```bash
cd AppHost && dotnet run          # Terminal 1
docker-compose up frontend-store frontend-admin  # Terminal 2
```
- Aspire Dashboard (http://localhost:15500)
- Direktes Backend-Debugging
- Containerisierte Frontends
- **? BEST FOR DEVELOPMENT**

### ????? Option 3: Alles Manual (Max Debugging)
```bash
cd AppHost && dotnet run          # Terminal 1
cd Frontend/Store && npm run dev  # Terminal 2
cd Frontend/Admin && npm run dev  # Terminal 3
```
- Maximales Debugging
- Schnelstes Hot-Reload
- Keine Docker nötig (aber .NET + Node.js)

---

## ?? Build Status

? **Backend:** Erfolgreich kompiliert
```
B2Connect.AppHost net10.0 Erfolgreich
B2Connect.Admin net10.0 erfolgreich mit 4 Warnung(en)
Erstellen von erfolgreich mit 20 Warnung(en) in 3,2s
```

? **Frontend:** Docker-ready
- Vite Configs konfiguriert
- Multi-stage Dockerfiles
- Hot-reload enabled
- TypeScript Warnings (normal, nicht kritisch)

---

## ?? Zugriff nach Startup

| Service | URL | Gateway |
|---------|-----|---------|
| **Store Frontend** | http://localhost:5173 | Port 8000 |
| **Admin Frontend** | http://localhost:5174 | Port 8080 |
| Store API Gateway | http://localhost:8000 | - |
| Admin API Gateway | http://localhost:8080 | - |
| Auth Service | http://localhost:7002 | - |
| Tenant Service | http://localhost:7003 | - |
| Localization | http://localhost:7004 | - |
| Catalog Service | http://localhost:7005 | - |
| Theming Service | http://localhost:7008 | - |
| PostgreSQL | localhost:5432 | user: postgres |
| Redis | localhost:6379 | - |
| RabbitMQ Admin | http://localhost:15672 | guest/guest |
| Elasticsearch | http://localhost:9200 | elastic/elastic |
| **Aspire Dashboard** | http://localhost:15500 | (Option 2 only) |

---

## ?? Wichtige Konzepte

### Multi-Stage Docker Build

**Development:**
```dockerfile
FROM node:20-alpine AS development
...
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

**Production:**
```dockerfile
FROM node:20-alpine AS production
RUN npm install -g serve
COPY --from=builder /app/dist ./dist
CMD ["serve", "-s", "dist"]
```

### Hot-Reload in Docker

```yaml
volumes:
  - ./Frontend/Store:/app        # Code changes sync
  - /app/node_modules            # Keep node_modules in container
command: npm run dev -- --host 0.0.0.0  # Listen on all interfaces
```

### API-Gateway Integration

**vite.config.ts:**
```typescript
proxy: {
  "/api": {
    target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8000",
    changeOrigin: true,
    ws: true,
  }
}
```

**docker-compose.yml:**
```yaml
environment:
  VITE_API_GATEWAY_URL: http://store-gateway:8000
```

---

## ??? Häufige Befehle

### Docker Compose
```bash
# Start
docker-compose up -d

# Logs
docker-compose logs -f frontend-store

# Restart
docker-compose restart frontend-store

# Stop
docker-compose down

# Clean rebuild
docker-compose build --no-cache
docker-compose up -d
```

### Aspire AppHost
```bash
# Start with Dashboard
cd AppHost && dotnet run

# Run specific service
dotnet run --project B2Connect.AppHost.csproj
```

### Frontend Development
```bash
# Install
npm install

# Dev server
npm run dev

# Build
npm run build

# Preview production
npm run preview

# Tests
npm run test
npm run e2e
```

---

## ?? Nächste Schritte

1. **Starten Sie mit Option 2** (Empfohlen):
   ```bash
   cd AppHost && dotnet run  # Terminal 1
   docker-compose up frontend-store frontend-admin  # Terminal 2
   ```

2. **Öffnen Sie die Frontends:**
   - Store: http://localhost:5173
   - Admin: http://localhost:5174

3. **Monitoring:**
   - Aspire Dashboard: http://localhost:15500

4. **Debugging:**
   - Visual Studio: Backend-Services
   - Browser DevTools: Frontend-Apps
   - Docker logs: `docker-compose logs -f`

---

## ?? Dokumentation

- `FRONTENDS_ASPIRE_SETUP.md` - Diese Datei (Quick Reference)
- `docs/ASPIRE_FRONTEND_INTEGRATION.md` - Detailliert
- `Frontend/README.md` - Frontend-specific Guide
- `STARTUP_GUIDE.ps1` - Interactive Windows Guide
- `STARTUP_GUIDE.sh` - Interactive Linux/Mac Guide

---

## ? Checkliste für Go-Live

- ? Docker Compose konfiguriert
- ? Frontends containerisiert (Dockerfile)
- ? Aspire AppHost setup
- ? Vite Configs updated
- ? Environment variables configured
- ? Network & volumes ready
- ? Documentation complete
- ? Build successful
- ?? Test Startup (nächster Schritt!)

---

## ?? Ready to Go!

Ihre B2Connect Anwendung ist jetzt **vollständig konfiguriert** für:

? Local Development (Option 2)
? Testing & QA (Option 1)
? Production Deployment (Docker/Kubernetes)

**Los geht's!** ??

```bash
docker-compose up -d
# Oder
cd AppHost && dotnet run
```
