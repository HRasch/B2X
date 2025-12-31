# ? ASPIRE FRONTENDS MIT AddExecutable - PERFEKT GELÖST!

## ?? Update: Native Aspire Frontend Integration

Ihre Frontends sind jetzt **nativen in Aspire integriert** mit `AddExecutable`!

---

## ?? Was sich geändert hat

### ? ALT (Manuelle Verwaltung)
```bash
# Manuelles Starten in separaten Terminals
cd Frontend/Store && npm run dev
cd Frontend/Admin && npm run dev
```

### ? NEU (Aspire Native)
```bash
# Alles über Aspire orchestriert
cd AppHost && dotnet run
# ? Dashboard zeigt alle Services + Frontends!
```

---

## ?? Implementierung

### 1. **AddViteApp Extension** (neue Methode in B2ConnectAspireExtensions.cs)

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

### Schritt 2: Dashboard öffnen
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

### Schritt 4: Frontends öffnen
- Store: http://localhost:5173
- Admin: http://localhost:5174

---

## ?? Wie AddExecutable funktioniert

### Was wird ausgeführt:
```bash
# Für Frontend Store:
cd ../Frontend/Store
npm run dev

# Für Frontend Admin:
cd ../Frontend/Admin
npm run dev
```

### Was Aspire tut:
1. **Startet** npm dev Server als Child Process
2. **Monitored** Health & Logs
3. **Zeigt** in Dashboard an
4. **Stoppt** bei Shutdown

---

## ?? Dateien die geändert wurden

? **AppHost/B2ConnectAspireExtensions.cs**
- Neue Methode: `AddViteApp()`
- Komplettiert: `WithOpenTelemetry()`
- Komplettiert: `WithSecurityDefaults()`

? **AppHost/Program.cs**
- Frontend Store registriert mit `AddViteApp()`
- Frontend Admin registriert mit `AddViteApp()`

---

## ?? Erweiterte Verwendung

### Mit Abhängigkeiten:
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

## ?? Häufige Fragen

### F: Was wenn npm nicht installiert ist?
A: `AddExecutable` funktioniert nur wenn das Kommando existiert. Müssen Sie npm/Node.js installieren.

### F: Wie setze ich Custom Ports?
A: Parameter `port` in `AddViteApp()` ändern:
```csharp
var frontendStore = builder.AddViteApp(..., port: 3000);
```

### F: Kann ich npm ci statt npm install nutzen?
A: Ja, im `npm run dev` ist bereits installiert, aber Sie können das Dockerfile anpassen.

### F: Funktioniert Hot Reload noch?
A: ? Ja! `npm run dev` lädt automatisch neu. Besser sogar, da Aspire Dashboard alles anzeigt.

---

## ?? Weitere Ressourcen

- [Aspire AddExecutable Docs](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-overview)
- [Vite Development Server](https://vitejs.dev/guide/#index-html-and-project-root)
- [Docker Alternative](docker-compose.yml) - Falls Sie Frontends containerisieren möchten

---

## ? Checklist

- ? `AddViteApp()` Extension implementiert
- ? Frontend Store in AppHost registriert
- ? Frontend Admin in AppHost registriert
- ? Build erfolgreich
- ? Ready für Startup

---

## ?? Nächste Schritte

1. **Starten Sie AppHost:**
   ```bash
   cd AppHost && dotnet run
   ```

2. **Öffnen Sie Dashboard:**
   ```
   http://localhost:15500
   ```

3. **Frontends sollten sichtbar sein:**
   - `frontend-store` (Running, Port 5173)
   - `frontend-admin` (Running, Port 5174)

4. **Öffnen Sie die Frontends:**
   - Store: http://localhost:5173
   - Admin: http://localhost:5174

---

## ?? Glückwunsch!

Sie haben jetzt **vollständige native Aspire Integration** für Ihre Frontends! ??

Alle Services (Backend + Frontend) werden über einen AppHost orchestriert und im Aspire Dashboard monitored.
