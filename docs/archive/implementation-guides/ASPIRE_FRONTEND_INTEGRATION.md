# B2Connect Frontend Integration mit Aspire 13.1.0

## ?? Situation

Sie verwenden **Aspire 13.1.0**, in der `AddNpmApp` nicht mehr existiert. Das ist okay! Es gibt mehrere Ansätze:

## ? Empfohlener Ansatz: Docker Compose + Separate Aspire AppHost

Dies ist die **beste Lösung** für Ihre Konfiguration:

### 1?? Option A: Docker Compose für Alles (Einfachste Lösung)

```bash
# Startet ALLES in Docker: Backend + Frontend
docker-compose up -d
```

**Vorteile:**
- Reproduzierbare Umgebung
- Isoliert von lokaler Umgebung
- Keine .NET oder Node.js Installation nötig (außer Docker)
- Hot-reload funktioniert in Containern

**Nachteile:**
- Docker muss laufen
- Weniger direktes Debugging von Backend

### 2?? Option B: Aspire AppHost + Docker Compose (Hybrid)

```bash
# Terminal 1: Backend mit Aspire Orchestration
cd AppHost
dotnet run

# Terminal 2: Frontends in Docker (optional)
docker-compose up frontend-store frontend-admin
```

**Vorteile:**
- Aspire Dashboard für Service-Monitoring
- Backend direkt debuggbar
- Frontends in Docker
- Best of both worlds

### 3?? Option C: Alles Manual (Für lokale Entwicklung)

```bash
# Terminal 1: Aspire AppHost (orchestriert Backend)
cd AppHost
dotnet run

# Terminal 2: Frontend Store
cd Frontend/Store
npm install
npm run dev

# Terminal 3: Frontend Admin
cd Frontend/Admin
npm install
npm run dev
```

**Vorteile:**
- Maximales Debugging und Kontrolle
- Hot-reload sehr schnell
- Keine Docker-Abhängigkeit

**Nachteile:**
- Node.js Installation erforderlich
- Manual starten

## ?? Warum es AppHost in Aspire 13.1.0 nicht gibt

Aspire entfernte `AddNpmApp` in Version 13+ wegen:

1. **Komplexität**: Vite braucht Node.js Installation
2. **Best Practices**: Frontends sollten separat verwaltet werden
3. **Alternatives**: Docker/Docker Compose ist der Standard

## ?? Vergleichstabelle

| Ansatz | Komplexität | Debugging | Production-Ready | Empfohlen |
|--------|-------------|-----------|------------------|-----------|
| Docker Compose (alles) | Niedrig | Schwierig | ? Sehr gut | ? |
| Aspire + Docker | Mittel | Gut | ? Gut | ? |
| Manual (alles) | Hoch | Sehr gut | ? Nein | Dev-Only |

## ?? Schnellstart (Empfohlen)

### Setup einmalig:

```bash
# Clone und Setup
git clone https://github.com/HRasch/B2Connect.git
cd B2Connect

# Stellen Sie sicher, dass Docker läuft
docker ps

# (Optional) Bauen Sie die Images vor:
docker-compose build
```

### Tägliche Nutzung:

```bash
# Alles starten
docker-compose up -d

# Oder für Development mit Aspire:
cd AppHost && dotnet run

# Frontends (wenn nicht in Docker):
cd Frontend/Store && npm run dev
cd Frontend/Admin && npm run dev
```

## ?? Zugriff

- **Store Frontend**: http://localhost:5173
- **Admin Frontend**: http://localhost:5174
- **Store API Gateway**: http://localhost:8000
- **Admin API Gateway**: http://localhost:8080
- **Aspire Dashboard** (wenn AppHost läuft): http://localhost:15500

## ??? Alternative: Custom Aspire Extension

Falls Sie Frontends doch in Aspire orchestrieren wollen, können Sie eine Custom Extension erstellen:

```csharp
// AppHost/Extensions/ViteExtensions.cs
namespace B2Connect.Aspire.Extensions;

public static class ViteExtensions
{
    public static IResourceBuilder<ExecutableResource> AddViteApp(
        this IDistributedApplicationBuilder builder,
        string name,
        string projectPath,
        int port)
    {
        var workingDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", projectPath);
        
        return builder
            .AddExecutable(
                name: name,
                command: "npm",
                workingDirectory: workingDirectory,
                args: ["run", "dev", "--", "--host", "0.0.0.0", "--port", port.ToString()])
            .WithHttpEndpoint(
                port: port,
                targetPort: port,
                name: name,
                isProxied: false)
            .WithEnvironment("VITE_API_GATEWAY_URL", $"http://localhost:{GetGatewayPort(name)}")
            .WithEnvironment("NODE_ENV", "development");
    }

    private static int GetGatewayPort(string name) => 
        name.Contains("store") ? 8000 : 8080;
}

// Usage in AppHost/Program.cs:
var frontendStore = builder
    .AddViteApp("frontend-store", "../../Frontend/Store", 5173);

var frontendAdmin = builder
    .AddViteApp("frontend-admin", "../../Frontend/Admin", 5174);
```

## ?? Environment-Variablen

Vite liest Variablen mit `VITE_` Präfix:

**Frontend/Store/vite.config.ts:**
```typescript
proxy: {
  "/api": {
    target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8000",
  }
}
```

**In docker-compose.yml:**
```yaml
environment:
  VITE_API_GATEWAY_URL: http://store-gateway:8000
  NODE_ENV: development
```

## ?? Docker-spezifisch

### Services Bauen:
```bash
docker-compose build --no-cache
```

### Logs ansehen:
```bash
docker-compose logs -f frontend-store
docker-compose logs -f frontend-admin
```

### Einzelne Service neustarten:
```bash
docker-compose restart frontend-store
```

## ?? Weitere Ressourcen

- [Aspire Dokumentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Docker Compose Referenz](https://docs.docker.com/compose/compose-file/)
- [Vite Konfiguration](https://vitejs.dev/config/)
- [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)

## ?? Best Practices für Ihre Situation

1. **Development**: Nutzen Sie **Option C** (Manual) oder **Option B** (Hybrid)
   - Schnellestes Feedback
   - Einfacheres Debugging
   
2. **Testing**: Nutzen Sie **Option A** (Docker Compose)
   - Genau wie Production
   - Reproduzierbar
   
3. **Production**: Nutzen Sie **Docker/Kubernetes**
   - Scalierbar
   - Isoliert

## ?? Häufige Probleme

### Problem: "Cannot GET /" auf Port 5173

**Lösung:**
- Stellen Sie sicher, dass `npm run dev` läuft
- Überprüfen Sie, dass Port 5173 nicht belegt ist: `netstat -ano | findstr :5173`

### Problem: Frontend kann Backend nicht erreichen

**Lösung:**
- Überprüfen Sie `VITE_API_GATEWAY_URL`
- Überprüfen Sie, dass Gateway läuft: `curl http://localhost:8000/health`
- Check Docker network: `docker network inspect b2connect`

### Problem: Hot-reload funktioniert nicht

**Lösung:**
- Stellen Sie sicher, dass Vite auf `0.0.0.0` lauscht (nicht `localhost`)
- Überprüfen Sie Dockerfile: `CMD npm run dev -- --host 0.0.0.0`

---

**Zusammenfassung**: Ihre Konfiguration mit **docker-compose.yml** ist bereits optimal für Aspire 13.1.0! Starten Sie einfach mit:

```bash
docker-compose up -d
```

Oder für Development:

```bash
cd AppHost && dotnet run  # Terminal 1: Backend
# Terminal 2 & 3: Frontends laufen bereits in docker-compose
```
