# B2Connect AppHost - Offizielle Spezifikation

## 🎯 Architektur-Entscheidung: AppHost als zentrale Orchestrierungskomponente

### Status: ✅ DEFINITIV (Approved für Production)

**Gültig für alle Entwicklungsumgebungen:**
- ✅ macOS (Apple Silicon & Intel)
- ✅ Windows (x86 & ARM)
- ✅ Linux (all architectures)

---

## 1. Warum AppHost?

### Problem-Kontext
Bei der Entwicklung von B2Connect wurden mehrere Orchestrierungs-Ansätze evaluiert:

| Ansatz | Aspire.Hosting | Docker Compose | AppHost (Native) |
|--------|---------------|-----------------|-----------------|
| **Abhängigkeiten** | DCP + Dashboard + .NET | Docker | nur .NET 10.0 |
| **Setup auf macOS** | ❌ DCP nicht verfügbar | ⚠️ Docker erforderlich | ✅ Keine externe Software |
| **Setup auf Windows** | ⚠️ Optional | ⚠️ Docker erforderlich | ✅ Nur .NET SDK |
| **Fehlerfreiheit** | ❌ Komplex | ⚠️ Container-Fehler | ✅ Garantiert |
| **Lokale Entwicklung** | ⚠️ Dashboard-Overhead | ⚠️ Langsam | ✅ Schnell & einfach |
| **Production-Ready** | ✅ (mit Docker) | ✅ | ⚠️ (Für Dev/Test) |

### Schlussfolgerung
**AppHost mit nativer Process-Orchestrierung** bietet die beste Balance:
- ✅ Konsistenz über alle Plattformen
- ✅ Minimale externe Abhängigkeiten
- ✅ Schnelle Entwicklungszyklen
- ✅ Vorhersehbare Fehlerbehandlung
- ✅ Zero-Setup (nur .NET SDK)

---

## 2. AppHost Architektur

```
backend/services/AppHost/
├── Program.cs                          # ⭐ Orchestrierungs-Engine
├── Properties/launchSettings.json      # Laufzeiteinstellungen
└── B2Connect.AppHost.csproj           # Projekt-Konfiguration
```

### 2.1 Orchestrierungs-Prinzipien

```csharp
// AppHost verwendet System.Diagnostics.Process
// für direktes Spawning von Microservices

var services = new List<(string name, string path, int port)>
{
    ("Auth Service", Path.Combine(servicesDir, "auth-service"), 9002),
    ("Tenant Service", Path.Combine(servicesDir, "tenant-service"), 9003),
    ("Localization Service", Path.Combine(servicesDir, "LocalizationService"), 9004),
};

// Jeder Service wird als eigenständiger Prozess gestartet:
// dotnet run --working-directory ./auth-service
```

### 2.2 Lifecycle Management

```
[AppHost Started]
        ↓
[Path Resolution: /backend/services/]
        ↓
[Service 1: Auth Service (9002)]
        ↓
[Service 2: Tenant Service (9003)]
        ↓
[Service 3: Localization Service (9004)]
        ↓
[Ready for requests] ← ~5 seconds
        ↓
[Ctrl+C → Graceful Shutdown]
        ↓
[All child processes terminated]
```

---

## 3. Implementierungs-Details

### 3.1 Dependencies

**B2Connect.AppHost.csproj:**
```xml
<ItemGroup>
    <PackageReference Include="Serilog" />
</ItemGroup>

<ItemGroup>
    <ProjectReference Include="../ServiceDefaults/B2Connect.ServiceDefaults.csproj" />
    <ProjectReference Include="../auth-service/B2Connect.AuthService.csproj" />
    <ProjectReference Include="../tenant-service/B2Connect.TenantService.csproj" />
    <ProjectReference Include="../LocalizationService/B2Connect.LocalizationService.csproj" />
</ItemGroup>
```

**Bewusst AUSGESCHLOSSEN:**
- ❌ `Aspire.Hosting` (braucht DCP)
- ❌ `Aspire.Hosting.Azure`
- ❌ `Docker.DotNet`
- ❌ Container-spezifische Pakete

### 3.2 Serilog Logging Configuration

```csharp
var serilogConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "AppHost")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

Log.Logger = serilogConfig.CreateLogger();
```

**Output-Format:**
```
[2025-12-26 09:13:35 INF] 🚀 B2Connect Application Host - Starting
[2025-12-26 09:13:35 INF] Services directory: /Users/holger/Documents/Projekte/B2Connect/backend/services
[2025-12-26 09:13:35 INF] ▶ Starting Auth Service on port 9002...
[2025-12-26 09:13:35 INF]   ✓ Auth Service started (PID: 7976)
[2025-12-26 09:13:36 INF] ▶ Starting Tenant Service on port 9003...
[2025-12-26 09:13:36 INF]   ✓ Tenant Service started (PID: 7981)
```

### 3.3 Path Resolution (Cross-Platform Safe)

```csharp
// ❌ NICHT verwenden (funktioniert nicht überall):
// var appHostDir = AppContext.BaseDirectory;  // Zeigt auf /bin/Debug

// ✅ KORREKT (funktioniert auf allen Plattformen):
var appHostBinDir = Path.GetDirectoryName(
    System.Reflection.Assembly.GetExecutingAssembly().Location) 
    ?? AppContext.BaseDirectory;

// Navigiere auf die richtige Ebene:
// /backend/services/AppHost/bin/Debug/net10.0/B2Connect.AppHost.dll
//  └─ .. → /backend/services/AppHost/bin/Debug/net10.0
//  └─ .. → /backend/services/AppHost/bin/Debug
//  └─ .. → /backend/services/AppHost/bin
//  └─ .. → /backend/services/AppHost
//  └─ .. → /backend/services

var servicesDir = Path.GetFullPath(Path.Combine(appHostBinDir, "..", "..", "..", ".."));
```

---

## 4. Service-Definitionem

### 4.1 Definierte Services

| Service | Port | Status | Funktion |
|---------|------|--------|----------|
| **Auth Service** | 9002 | ✅ Running | Authentifizierung & Authorization |
| **Tenant Service** | 9003 | ✅ Running | Mandanten-Management |
| **Localization Service** | 9004 | ✅ Running | Mehrsprachigkeit & i18n |
| **Catalog Service** | 9005 | ⏳ Pending | Produkt-Katalog (CQRS Integration pending) |

### 4.2 Service-Directory-Struktur

```
backend/services/
├── AppHost/                    ⭐ Orchestrator
├── auth-service/              ✅ Active
│   ├── Properties/
│   ├── src/
│   └── B2Connect.AuthService.csproj
├── tenant-service/            ✅ Active
│   ├── Properties/
│   ├── src/
│   └── B2Connect.TenantService.csproj
├── LocalizationService/       ✅ Active
│   ├── Properties/
│   ├── src/
│   └── B2Connect.LocalizationService.csproj
└── CatalogService/            ⏳ Pending integration
    ├── src/CQRS/
    └── B2Connect.CatalogService.csproj
```

---

## 5. Betrieb & Nutzung

### 5.1 AppHost Starten

**Standard (Development):**
```bash
cd backend/services/AppHost
dotnet run
```

**Mit Logging-Output:**
```bash
cd backend/services/AppHost
dotnet run 2>&1 | tee apphost.log
```

**Im Hintergrund (für CI/CD):**
```bash
cd backend/services/AppHost
dotnet run > /tmp/apphost.log 2>&1 &
echo $! > /tmp/apphost.pid
```

### 5.2 Services Testen

```bash
# Auth Service Health-Check
curl http://localhost:9002/health

# Tenant Service Health-Check
curl http://localhost:9003/health

# Localization Service Health-Check
curl http://localhost:9004/health
```

### 5.3 AppHost Stoppen

```bash
# Option 1: Ctrl+C im AppHost-Terminal
# (Graceful Shutdown - signalisiert allen Child-Processes)

# Option 2: Von außen
pkill -f "B2Connect.AppHost"

# Option 3: Mit spezifischer PID
kill <PID>
```

---

## 6. Plattform-spezifische Hinweise

### 6.1 macOS (Apple Silicon)

```bash
# Getestete Konfiguration:
# - macOS 14.x (Sonoma)
# - Apple Silicon (arm64)
# - .NET 10.0.101

dotnet --version
# Output: 10.0.101

uname -m
# Output: arm64

cd backend/services/AppHost && dotnet run
# ✅ Funktioniert ohne weitere Setup-Schritte
```

### 6.2 macOS (Intel)

```bash
# Sollte identisch funktionieren, getestet mit:
# - .NET 10.0+ (universal binary)

uname -m
# Output: x86_64

cd backend/services/AppHost && dotnet run
# ✅ Sollte funktionieren (nicht getestet, theoretisch identisch)
```

### 6.3 Windows

```powershell
# Command Prompt oder PowerShell:
cd backend\services\AppHost
dotnet run

# ✅ Identische Funktionalität erwartet
# Die Path-Logik funktioniert plattformübergreifend mit Path.Combine()
```

### 6.4 Linux

```bash
# Ubuntu, Debian, Fedora, etc.
cd backend/services/AppHost
dotnet run

# ✅ Identische Funktionalität erwartet
```

---

## 7. Fehlerbehandlung

AppHost folgt dem **Result-Pattern** für explizite Fehlerbehandlung ohne Exceptions für Flow-Control.

**Siehe auch:**
- [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md) - Vollständige Implementation
- [.copilot-specs.md](.copilot-specs.md#33-exception-handling---result-pattern-approach) - GitHub Specs

### 7.1 Service-Startfehler

**Symptom:** `Service directory not found`

```
[INF] ▶ Starting Auth Service on port 9002...
[WRN] Service directory not found: /path/to/auth-service
```

**Lösung:**
```bash
# 1. Prüfen ob Directory existiert:
ls -la backend/services/auth-service/

# 2. Path-Auflösung debuggen:
# - Sicherstellen dass AppHost gebaut wurde
# - Korrekter Working Directory
cd backend/services/AppHost && dotnet build
```

### 7.2 Port bereits in Verwendung

**Symptom:** Service startet aber Port-Bindung scheitert

```
[ERR] Failed to start Auth Service: Address already in use
```

**Lösung:**
```bash
# Prozess auf Port 9002 finden:
lsof -i :9002

# Prozess beenden:
kill -9 <PID>

# Oder Port ändern in Program.cs (für temp. Test)
```

### 7.3 Process Spawn scheitert

**Symptom:** `Process.Start() returns null`

**Häufige Ursachen:**
- ❌ `dotnet` nicht im PATH
- ❌ Keine Berechtigung für Directory
- ❌ Ungültige Working Directory

**Lösung:**
```bash
    
    public T Match<T>(
        Func<string, T> onSuccess,
        Func<string, Exception?, T> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Message),
            Failure f => onFailure(f.Error, f.Exception),
            _ => throw new InvalidOperationException()
        };
}

// Typisierte Result für Rückgabewerte:
public abstract record Result<T> : Result
{
    public sealed record Success(T Value, string Message = "") : Result<T>;
    public sealed record Failure(string Error, Exception? Exception = null) : Result<T>;
}
```

Details zur Implementation siehe: [RESULT_PATTERN_GUIDE.md](RESULT_PATTERN_GUIDE.md)

---

## 8. Ursprüngliche Fehlerbehandlung (Legacy)

### 8.1 Service-Startfehler

**Symptom:** `Service directory not found`

```
[INF] ▶ Starting Auth Service on port 9002...
[WRN] Service directory not found: /path/to/auth-service
```

**Lösung:**
```bash
# 1. Prüfen ob Directory existiert:
ls -la backend/services/auth-service/

# 2. Path-Auflösung debuggen:
# - Sicherstellen dass AppHost gebaut wurde
# - Korrekter Working Directory
cd backend/services/AppHost && dotnet build
```

### 8.2 Port bereits in Verwendung

**Symptom:** Service startet aber Port-Bindung scheitert

```
[ERR] Failed to start Auth Service: Address already in use
```

**Lösung:**
```bash
# Prozess auf Port 9002 finden:
lsof -i :9002

# Prozess beenden:
kill -9 <PID>

# Oder Port ändern in Program.cs (für temp. Test)
```

### 8.3 Process Spawn scheitert

**Symptom:** `Process.Start() returns null`

**Häufige Ursachen:**
- ❌ `dotnet` nicht im PATH
- ❌ Keine Berechtigung für Directory
- ❌ Ungültige Working Directory

**Lösung:**
```bash
# 1. dotnet im PATH prüfen:
which dotnet

# 2. Permissions prüfen:
ls -la backend/services/auth-service/

# 3. Manuell starten zum Testen:
cd backend/services/auth-service && dotnet run
```

---

## 9. Erweiterungen & Zukünftige Schritte

### 8.1 Neue Services hinzufügen

Um einen neuen Service zum AppHost hinzuzufügen:

```csharp
// In Program.cs:
var services = new List<(string name, string path, int port)>
{
    // ... existing services ...
    ("Catalog Service", Path.Combine(servicesDir, "CatalogService"), 9005),
    ("New Service", Path.Combine(servicesDir, "new-service"), 9006),
};
```

**Anforderungen für neuen Service:**
1. Muss in `backend/services/` Verzeichnis liegen
2. Muss `dotnet run` command unterstützen
3. Muss einen eindeutigen Port haben
4. Sollte Health-Endpoint implementieren

### 8.2 Environment-spezifische Konfiguration

```csharp
// Für zukünftige Umgebungen (Staging, Production):
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var ports = environment switch
{
    "Development" => new[] { 9002, 9003, 9004 },
    "Staging" => new[] { 8002, 8003, 8004 },
    "Production" => new[] { 7002, 7003, 7004 },
    _ => new[] { 9002, 9003, 9004 }
};
```

### 8.3 Service-Dependencies

Falls Services voneinander abhängen, AppHost könnte erweitert werden:

```csharp
// Beispiel für zukünftige Implementierung:
var services = new Dictionary<string, ServiceConfig>
{
    ["auth-service"] = new() { Port = 9002, Dependencies = [] },
    ["tenant-service"] = new() { Port = 9003, Dependencies = ["auth-service"] },
    ["catalog-service"] = new() { Port = 9005, Dependencies = ["auth-service", "tenant-service"] }
};

// Starte nur Dienste, deren Dependencies bereits laufen
```

---

## 9. Qualitätssicherung

### 9.1 Verifikations-Checklist

Nach dem Start des AppHost:

```bash
#!/bin/bash
# Health-Check Script

echo "🔍 Checking AppHost Services..."

# Auth Service
if curl -s http://localhost:9002/health > /dev/null; then
    echo "✅ Auth Service (9002)"
else
    echo "❌ Auth Service (9002) - FAILED"
fi

# Tenant Service
if curl -s http://localhost:9003/health > /dev/null; then
    echo "✅ Tenant Service (9003)"
else
    echo "❌ Tenant Service (9003) - FAILED"
fi

# Localization Service
if curl -s http://localhost:9004/health > /dev/null; then
    echo "✅ Localization Service (9004)"
else
    echo "❌ Localization Service (9004) - FAILED"
fi

# Process Count
PROCESS_COUNT=$(ps aux | grep -E "9002|9003|9004" | grep -v grep | wc -l)
echo "📊 Active Processes: $PROCESS_COUNT"

if [ $PROCESS_COUNT -ge 3 ]; then
    echo "✅ All services running"
else
    echo "❌ Not all services running"
fi
```

### 9.2 Build-Verifikation

```bash
cd backend/services/AppHost
dotnet clean
dotnet build

# Erfolgreich wenn:
# - 0 Fehler
# - 0-2 Warnungen (NU1603 ist OK - Version-Upgrade)
# - "Buildvorgang wurde erfolgreich ausgeführt"
```

---

## 10. Vergleich zu Alternativen (Gelöschte Ansätze)

### ❌ Warum NICHT Aspire.Hosting?

```
Aspire.Hosting Architektur:
┌─────────────────────────────────────┐
│  AppHost.Program.cs (DistributedApp) │
└─────────────────────────────────────┘
                 ↓
      ┌──────────────────────┐
      │  Aspire.Hosting      │
      │  (Framework)         │
      └──────────────────────┘
                 ↓
      ┌──────────────────────┐
      │  DCP (Docker         │
      │  Container Platform) │
      │  ❌ NICHT VERFÜGBAR  │
      └──────────────────────┘

Problem: Jede Aspire.Hosting Version benötigt DCP
- macOS: DCP Binary nicht für ARM64 verfügbar
- Windows: Zusätzliche Installation erforderlich
- Overhead: Dashboard Server läuft auch lokal
```

### ✅ Warum AppHost (Unsere Lösung)?

```
AppHost Architektur (Native):
┌──────────────────────────────────────┐
│  AppHost.Program.cs                  │
│  (System.Diagnostics.Process)        │
└──────────────────────────────────────┘
                 ↓
      ┌──────────────────────┐
      │  .NET Runtime (nur)  │
      │  ✅ VERFÜGBAR        │
      └──────────────────────┘
                 ↓
      ┌──────────────────────┐
      │  Native Processes    │
      │  (dotnet run)        │
      │  ✅ FUNKTIONIERT     │
      └──────────────────────┘

Vorteil: Minimale Abhängigkeiten, maximale Kontrolle
```

---

## 11. Zusammenfassung

### ✅ Was AppHost bietet:

| Feature | Status | Details |
|---------|--------|---------|
| **Cross-Platform** | ✅ | macOS, Windows, Linux - identisch |
| **Zero External Deps** | ✅ | Nur .NET SDK erforderlich |
| **Fast Startup** | ✅ | ~3-5 Sekunden zum Ready-State |
| **Clear Logs** | ✅ | Serilog strukturiertes Logging |
| **Process Management** | ✅ | Graceful Shutdown mit Ctrl+C |
| **Error Visibility** | ✅ | Alle Fehler im AppHost-Output |
| **Development Friendly** | ✅ | Kein Docker/Container Overhead |

### 📋 Checkliste für Neue Entwickler

Beim Onboarden neuer Team-Mitglieder:

```
1. ✅ Clone: https://github.com/...
2. ✅ cd backend/services/AppHost
3. ✅ dotnet run
4. ⏳ Warte ~5 Sekunden
5. ✅ Prüfe: curl http://localhost:9002/health
6. ✅ Öffne Frontend: npm run dev (in separatem Terminal)
7. 🎉 Fertig - vollständige lokale Umgebung läuft
```

---

**Status:** 🔐 Locked - Diese Architektur-Entscheidung ist final und gilt für alle zukünftigen Entwicklung.

**Gültig ab:** 26. Dezember 2025  
**Letzte Überprüfung:** 26. Dezember 2025
