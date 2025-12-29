# Aspire Dashboard - Services nicht sichtbar? Lösung!

## ?? Problem: Services werden im Aspire Dashboard nicht angezeigt

Das ist ein häufiges Problem. Hier sind die **Top 5 Lösungen**:

---

## ? Lösung 1: `IsAspireProjectResource` überprüfen

**Das häufigste Problem!** Alle Service-Projekte müssen diese Eigenschaft haben.

### Überprüfen Sie jedes Service-Projekt:

```bash
# Windows PowerShell
Get-Content backend\Domain\Identity\B2Connect.Identity.API.csproj | Select-String "IsAspireProjectResource"

# Linux/Mac
grep "IsAspireProjectResource" backend/Domain/Identity/B2Connect.Identity.API.csproj
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

**Fix:** Fügen Sie zu allen Service-Projekten hinzu:
```xml
<IsAspireProjectResource>true</IsAspireProjectResource>
```

---

## ? Lösung 2: AppHost-Pfade überprüfen

Die Pfade zu den Service-Projekten müssen relativ zur AppHost-Position sein.

### Überprüfen Sie AppHost/Program.cs:

```csharp
// ? Korrekt (relativ zur AppHost)
var authService = builder
    .AddProject("auth-service", "../backend/Domain/Identity/B2Connect.Identity.API.csproj")
    .WithHttpEndpoint(port: 7002, ...)

// ? Falsch
var authService = builder
    .AddProject("auth-service", "backend/Domain/Identity/B2Connect.Identity.API.csproj")
```

**Struktur:**
```
AppHost/
    ??? Program.cs
    ??? B2Connect.AppHost.csproj

backend/
    ??? Domain/
        ??? Identity/
            ??? B2Connect.Identity.API.csproj  <- 2 Ebenen hoch (../..)
```

---

## ? Lösung 3: AppHost Build Errors

Der AppHost selbst muss fehlerfrei kompilieren.

```bash
# Überprüfen Sie auf Fehler
dotnet build AppHost -c Debug

# Sollte mit "Erfolgreich" enden
# Nicht: "1 Fehler, 0 Warnungen"
```

**Häufige Fehler:**
- Missing Project References
- Invalid Project Paths
- Extension Method not found

---

## ? Lösung 4: Service-Startup-Reihenfolge

Services müssen in der richtigen Reihenfolge starten. Infrastructure Services zuerst!

### Überprüfen Sie docker-compose.yml:

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

## ? Lösung 5: AppHost mit ausführlicher Ausgabe starten

Läuft der AppHost? Gibt es Fehler beim Startup?

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

## ?? Diagnostics durchführen

Ich habe ein Diagnose-Skript erstellt. Führen Sie aus:

### Windows PowerShell:
```powershell
.\scripts\diagnose.ps1
```

### Linux/Mac:
```bash
bash scripts/diagnose.sh
```

Das Script überprüft:
- ? Alle .csproj Dateien existieren
- ? IsAspireProjectResource ist gesetzt
- ? Docker Container laufen
- ? Ports sind offen
- ? AppHost kompiliert

---

## ?? Complete Checklist

Bevor Sie ein Support-Ticket öffnen, überprüfen Sie:

- [ ] `IsAspireProjectResource>true` in ALL Service .csproj Dateien
- [ ] Pfade in AppHost/Program.cs sind korrekt (../backend/...)
- [ ] `dotnet build AppHost` erfolgreich
- [ ] `docker-compose ps` zeigt healthy Services
- [ ] `AppHost` läuft: `cd AppHost && dotnet run`
- [ ] http://localhost:15500 laden Sie im Browser
- [ ] Browser Console (F12) hat keine Fehler
- [ ] Services starten in korrekter Reihenfolge (Infra zuerst)

---

## ?? Service-Setup Vorlage

Falls Sie neue Services hinzufügen, benutzen Sie diese Vorlage:

### 1. .csproj updaten:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsAspireProjectResource>true</IsAspireProjectResource>
  </PropertyGroup>
</Project>
```

### 2. In AppHost/Program.cs hinzufügen:
```csharp
var newService = builder
    .AddProject("new-service", "../backend/Domain/NewService/B2Connect.NewService.API.csproj")
    .WithHttpEndpoint(port: 7010, targetPort: 7010, name: "new-service", isProxied: false)
    .WithEnvironment("ASPNETCORE_URLS", "http://localhost:7010")
    .WithPostgresConnection(newServiceDb)  // Falls nötig
    .WithRedisConnection(redis)
    .WithRabbitMQConnection(rabbitmq)
    .WithJaegerTracing()
    .WithOpenTelemetry();
```

### 3. In docker-compose.yml hinzufügen:
```yaml
new-service:
  build:
    context: .
    dockerfile: backend/Domain/NewService/Dockerfile
  container_name: b2connect-new-service
  ports:
    - "7010:7010"
  depends_on:
    postgres:
      condition: service_healthy
```

---

## ?? Quick Fix für häufiges Problem

95% der Fälle: `IsAspireProjectResource` fehlt!

```bash
# Schnelle Überprüfung aller Services:
for file in backend/Domain/*/B2Connect.*.API.csproj; do
    if ! grep -q "IsAspireProjectResource" "$file"; then
        echo "? FEHLT in: $file"
    fi
done
```

**Fix:**
```bash
# Alle Service-Projekte überprüfen und ggfs. updaten
```

---

## ?? Support

Wenn Sie immer noch Probleme haben:

1. Führen Sie das Diagnose-Skript aus: `.\scripts\diagnose.ps1`
2. Kopieren Sie die gesamte Ausgabe
3. Schauen Sie in: `docs/ASPIRE_FRONTEND_INTEGRATION.md`
4. Überprüfen Sie die Logs: `docker-compose logs`

---

## ?? Häufige Fehlermeldungen

### "Service wird nicht im Dashboard angezeigt"
? **Lösung 1**: IsAspireProjectResource überprüfen

### "Project file not found"
? **Lösung 2**: AppHost-Pfade überprüfen

### "Service startet nicht"
? **Lösung 4**: docker-compose logs overprüfen

### "Port already in use"
? Siehe: `scripts/kill-all-services.sh`

---

**Denken Sie daran:** Das Aspire Dashboard ist sehr sensitiv gegenüber Startproblemen. Wenn Services nicht angezeigt werden, ist fast immer einer der 5 Punkte oben das Problem! ??
