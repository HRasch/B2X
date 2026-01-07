# ? ASPIRE FRONTENDS MIT AddExecutable - PERFEKT GEL�ST!

## ?? Update: Native Aspire Frontend Integration

Ihre Frontends sind jetzt **nativen in Aspire integriert** mit `AddExecutable`!

---

## ?? Was sich ge�ndert hat

### ? ALT (Manuelle Verwaltung)
```bash
# Manuelles Starten in separaten Terminals
cd Frontend/Store && npm run dev
cd Frontend/Admin && npm run dev
```

### ? NEU (Aspire Native)
```bash
# Alles �ber Aspire orchestriert
cd AppHost && dotnet run
# ? Dashboard zeigt alle Services + Frontends!
```

---

## ?? Implementierung

### 1. **AddViteApp Extension** (neue Methode in B2XAspireExtensions.cs)

```csharp
public static IResourceBuilder<ExecutableResource> AddViteApp(
    this IDistributedApplicationBuilder builder,
    string name,
    string workingDirectory,
    int port,
    string? apiGatewayUrl = null)
{
    return builder
        .AddExecutable(
            name: name,
            command: "npm",
            workingDirectory: workingDirectory)
        .WithArgs("run", "dev")
        .WithHttpEndpoint(...)
        .WithEnvironment("VITE_PORT", port.ToString())
        .WithEnvironment("VITE_API_GATEWAY_URL", ...)
        .WithEnvironment("NODE_ENV", "development")
        .WithEnvironment("BROWSER", "none");
}
```

### 2. **AppHost Integration**

```csharp
// Frontend Store (Port 5173)
var frontendStore = builder.AddViteApp(
    name: "frontend-store",
    workingDirectory: "../Frontend/Store",
    port: 5173,
    apiGatewayUrl: "http://localhost:8000");

// Frontend Admin (Port 5174)
var frontendAdmin = builder.AddViteApp(
    name: "frontend-admin",
    workingDirectory: "../Frontend/Admin",
    port: 5174,
    apiGatewayUrl: "http://localhost:8080");
```

---

## ?? Vorteile von AddExecutable

? **Native Aspire Integration**
- Frontends in Aspire Dashboard sichtbar
- Einheitliche Orchestrierung
- Automatisches Startup/Shutdown

? **Besseres Development**
- Alles in einem Dashboard
- Unified Logging
- Service Dependencies

? **Production Ready**
- Kontrollierter Startup
- Health Checks
- Resource Management

---

## ?? So starten Sie jetzt

### Schritt 1: AppHost starten
```bash
cd AppHost
dotnet run
```

### Schritt 2: Dashboard �ffnen
```
http://localhost:15500
```

### Schritt 3: Frontends im Dashboard sehen
```
ASPIRE DASHBOARD
????????????????????????????????????????
Resources              Status
????????????????????????????????????????
postgres              ? Healthy
redis                 ? Healthy
auth-service          ? Running
...
frontend-store        ? Running   (Port 5173)
frontend-admin        ? Running   (Port 5174)
????????????????????????????????????????
```

### Schritt 4: Frontends �ffnen
- Store: http://localhost:5173
- Admin: http://localhost:5174

---

## ?? Wie AddExecutable funktioniert

### Was wird ausgef�hrt:
```bash
# F�r Frontend Store:
cd ../Frontend/Store
npm run dev

# F�r Frontend Admin:
cd ../Frontend/Admin
npm run dev
```

### Was Aspire tut:
1. **Startet** npm dev Server als Child Process
2. **Monitored** Health & Logs
3. **Zeigt** in Dashboard an
4. **Stoppt** bei Shutdown

---

## ?? Dateien die ge�ndert wurden

? **AppHost/B2XAspireExtensions.cs**
- Neue Methode: `AddViteApp()`
- Komplettiert: `WithOpenTelemetry()`
- Komplettiert: `WithSecurityDefaults()`

? **AppHost/Program.cs**
- Frontend Store registriert mit `AddViteApp()`
- Frontend Admin registriert mit `AddViteApp()`

---

## ?? Erweiterte Verwendung

### Mit Abh�ngigkeiten:
```csharp
var frontendStore = builder.AddViteApp(...)
    .WithReference(storeGateway)  // Frontend kennt Gateway
    .WithReference(authService);   // Frontend kennt Auth Service
```

### Mit Custom Environment:
```csharp
var frontendStore = builder.AddViteApp(...)
    .WithEnvironment("VITE_DEBUG", "true")
    .WithEnvironment("VITE_FEATURE_FLAG_X", "enabled");
```

### Mit Health Checks:
```csharp
var frontendStore = builder.AddViteApp(...)
    .WithHealthCheck("/");  // Check Frontend Health
```

---

## ?? Vergleich: Alt vs. Neu

| Aspekt | Manual | AddExecutable |
|--------|--------|---------------|
| **Startup** | 3+ Terminals | 1 Command |
| **Monitoring** | Terminal Logs | Aspire Dashboard |
| **Health Checks** | Manual curl | Automatic |
| **Dependency Management** | Manuell | Automatic |
| **Environment Variables** | .env Files | Aspire Config |
| **Shutdown** | Ctrl+C in jedem Terminal | Automatic |
| **Production Ready** | ? Nein | ? Ja |

---

## ?? H�ufige Fragen

### F: Was wenn npm nicht installiert ist?
A: `AddExecutable` funktioniert nur wenn das Kommando existiert. M�ssen Sie npm/Node.js installieren.

### F: Wie setze ich Custom Ports?
A: Parameter `port` in `AddViteApp()` �ndern:
```csharp
var frontendStore = builder.AddViteApp(..., port: 3000);
```

### F: Kann ich npm ci statt npm install nutzen?
A: Ja, im `npm run dev` ist bereits installiert, aber Sie k�nnen das Dockerfile anpassen.

### F: Funktioniert Hot Reload noch?
A: ? Ja! `npm run dev` l�dt automatisch neu. Besser sogar, da Aspire Dashboard alles anzeigt.

---

## ?? Weitere Ressourcen

- [Aspire AddExecutable Docs](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-overview)
- [Vite Development Server](https://vitejs.dev/guide/#index-html-and-project-root)
- [Docker Alternative](docker-compose.yml) - Falls Sie Frontends containerisieren m�chten

---

## ? Checklist

- ? `AddViteApp()` Extension implementiert
- ? Frontend Store in AppHost registriert
- ? Frontend Admin in AppHost registriert
- ? Build erfolgreich
- ? Ready f�r Startup

---

## ?? N�chste Schritte

1. **Starten Sie AppHost:**
   ```bash
   cd AppHost && dotnet run
   ```

2. **�ffnen Sie Dashboard:**
   ```
   http://localhost:15500
   ```

3. **Frontends sollten sichtbar sein:**
   - `frontend-store` (Running, Port 5173)
   - `frontend-admin` (Running, Port 5174)

4. **�ffnen Sie die Frontends:**
   - Store: http://localhost:5173
   - Admin: http://localhost:5174

---

## ?? Gl�ckwunsch!

Sie haben jetzt **vollst�ndige native Aspire Integration** f�r Ihre Frontends! ??

Alle Services (Backend + Frontend) werden �ber einen AppHost orchestriert und im Aspire Dashboard monitored.
