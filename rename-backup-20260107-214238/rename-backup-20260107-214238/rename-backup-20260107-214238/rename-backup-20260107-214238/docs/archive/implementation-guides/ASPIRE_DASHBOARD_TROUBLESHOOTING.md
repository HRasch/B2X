# Aspire Dashboard - Services nicht sichtbar? L�sung!

## ?? Problem: Services werden im Aspire Dashboard nicht angezeigt

Das ist ein h�ufiges Problem. Hier sind die **Top 5 L�sungen**:

---

## ? L�sung 1: `IsAspireProjectResource` �berpr�fen

**Das h�ufigste Problem!** Alle Service-Projekte m�ssen diese Eigenschaft haben.

### �berpr�fen Sie jedes Service-Projekt:

```bash
# Windows PowerShell
Get-Content backend\Domain\Identity\B2X.Identity.API.csproj | Select-String "IsAspireProjectResource"

# Linux/Mac
grep "IsAspireProjectResource" backend/Domain/Identity/B2X.Identity.API.csproj
```

### ? Korrekt:
```xml
<PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsAspireProjectResource>true</IsAspireProjectResource>
</PropertyGroup>
```

### ? Falsch:
```xml
<PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <!-- Missing IsAspireProjectResource! -->
</PropertyGroup>
```

**Fix:** F�gen Sie zu allen Service-Projekten hinzu:
```xml
<IsAspireProjectResource>true</IsAspireProjectResource>
```

---

## ? L�sung 2: AppHost-Pfade �berpr�fen

Die Pfade zu den Service-Projekten m�ssen relativ zur AppHost-Position sein.

### �berpr�fen Sie AppHost/Program.cs:

```csharp
// ? Korrekt (relativ zur AppHost)
var authService = builder
    .AddProject("auth-service", "../backend/Domain/Identity/B2X.Identity.API.csproj")
    .WithHttpEndpoint(port: 7002, ...)

// ? Falsch
var authService = builder
    .AddProject("auth-service", "backend/Domain/Identity/B2X.Identity.API.csproj")
```

**Struktur:**
```
AppHost/
    ??? Program.cs
    ??? B2X.AppHost.csproj

backend/
    ??? Domain/
        ??? Identity/
            ??? B2X.Identity.API.csproj  <- 2 Ebenen hoch (../..)
```

---

## ? L�sung 3: AppHost Build Errors

Der AppHost selbst muss fehlerfrei kompilieren.

```bash
# �berpr�fen Sie auf Fehler
dotnet build AppHost -c Debug

# Sollte mit "Erfolgreich" enden
# Nicht: "1 Fehler, 0 Warnungen"
```

**H�ufige Fehler:**
- Missing Project References
- Invalid Project Paths
- Extension Method not found

---

## ? L�sung 4: Service-Startup-Reihenfolge

Services m�ssen in der richtigen Reihenfolge starten. Infrastructure Services zuerst!

### �berpr�fen Sie docker-compose.yml:

```yaml
# ? Korrekt - Infrastruktur zuerst
services:
  postgres:
    ...
  
  redis:
    ...
  
  identity-service:
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
```

### Logs ansehen:
```bash
# Alle Logs
docker-compose logs

# Nur Service
docker-compose logs identity-service

# Real-time
docker-compose logs -f catalog-service
```

---

## ? L�sung 5: AppHost mit ausf�hrlicher Ausgabe starten

L�uft der AppHost? Gibt es Fehler beim Startup?

```bash
# Terminal 1: Mit detaillierten Logs
cd AppHost
dotnet run --verbosity detailed

# Sollte zeigen:
# - Service registriert
# - Ports listened
# - Health checks
```

### Expected Output:
```
info: Aspire.Hosting.DistributedApplication[0]
      Starting project "postgres"
info: Aspire.Hosting.DistributedApplication[0]
      Starting project "redis"
info: Aspire.Hosting.DistributedApplication[0]
      Starting project "auth-service"
...
Listening on http://localhost:15500
```

---

## ?? Diagnostics durchf�hren

Ich habe ein Diagnose-Skript erstellt. F�hren Sie aus:

### Windows PowerShell:
```powershell
.\scripts\diagnose.ps1
```

### Linux/Mac:
```bash
bash scripts/diagnose.sh
```

Das Script �berpr�ft:
- ? Alle .csproj Dateien existieren
- ? IsAspireProjectResource ist gesetzt
- ? Docker Container laufen
- ? Ports sind offen
- ? AppHost kompiliert

---

## ?? Complete Checklist

Bevor Sie ein Support-Ticket �ffnen, �berpr�fen Sie:

- [ ] `IsAspireProjectResource>true` in ALL Service .csproj Dateien
- [ ] Pfade in AppHost/Program.cs sind korrekt (../backend/...)
- [ ] `dotnet build AppHost` erfolgreich
- [ ] `docker-compose ps` zeigt healthy Services
- [ ] `AppHost` l�uft: `cd AppHost && dotnet run`
- [ ] http://localhost:15500 laden Sie im Browser
- [ ] Browser Console (F12) hat keine Fehler
- [ ] Services starten in korrekter Reihenfolge (Infra zuerst)

---

## ?? Service-Setup Vorlage

Falls Sie neue Services hinzuf�gen, benutzen Sie diese Vorlage:

### 1. .csproj updaten:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsAspireProjectResource>true</IsAspireProjectResource>
  </PropertyGroup>
</Project>
```

### 2. In AppHost/Program.cs hinzuf�gen:
```csharp
var newService = builder
    .AddProject("new-service", "../backend/Domain/NewService/B2X.NewService.API.csproj")
    .WithHttpEndpoint(port: 7010, targetPort: 7010, name: "new-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7010")
    .WithPostgresConnection(newServiceDb)  // Falls n�tig
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithOpenTelemetry();
```

### 3. In docker-compose.yml hinzuf�gen:
```yaml
new-service:
  build:
    context: .
    dockerfile: backend/Domain/NewService/Dockerfile
  container_name: B2X-new-service
  ports:
    - "7010:7010"
  depends_on:
    postgres:
      condition: service_healthy
```

---

## ?? Quick Fix f�r h�ufiges Problem

95% der F�lle: `IsAspireProjectResource` fehlt!

```bash
# Schnelle �berpr�fung aller Services:
for file in backend/Domain/*/B2X.*.API.csproj; do
    if ! grep -q "IsAspireProjectResource" "$file"; then
        echo "? FEHLT in: $file"
    fi
done
```

**Fix:**
```bash
# Alle Service-Projekte �berpr�fen und ggfs. updaten
```

---

## ?? Support

Wenn Sie immer noch Probleme haben:

1. F�hren Sie das Diagnose-Skript aus: `.\scripts\diagnose.ps1`
2. Kopieren Sie die gesamte Ausgabe
3. Schauen Sie in: `docs/ASPIRE_FRONTEND_INTEGRATION.md`
4. �berpr�fen Sie die Logs: `docker-compose logs`

---

## ?? H�ufige Fehlermeldungen

### "Service wird nicht im Dashboard angezeigt"
? **L�sung 1**: IsAspireProjectResource �berpr�fen

### "Project file not found"
? **L�sung 2**: AppHost-Pfade �berpr�fen

### "Service startet nicht"
? **L�sung 4**: docker-compose logs overpr�fen

### "Port already in use"
? Siehe: `scripts/kill-all-services.sh`

---

**Denken Sie daran:** Das Aspire Dashboard ist sehr sensitiv gegen�ber Startproblemen. Wenn Services nicht angezeigt werden, ist fast immer einer der 5 Punkte oben das Problem! ??
