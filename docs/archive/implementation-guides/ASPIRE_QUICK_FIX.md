# ? Schnelle Lösung: Aspire Dashboard zeigt Services nicht

## ?? Schnelle Schritte (3 Minuten)

### 1. Alle Prozesse killen
```powershell
# Windows
.\scripts\kill-all-services.sh  # oder manuell:
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
docker-compose down
```

### 2. Clean Build
```bash
dotnet clean B2Connect.slnx
dotnet build AppHost -c Release
```

### 3. Fresh Start
```bash
# Terminal 1: AppHost mit Verbose Output
cd AppHost
dotnet run --verbosity diagnostic

# WARTEN Sie, bis Sie sehen:
# "Listening on http://localhost:15500"
# DANN öffnen Sie den Browser
```

### 4. Browser öffnen
```
http://localhost:15500
```

---

## ?? Wenn das nicht funktioniert, überprüfen Sie:

### Check 1: Ports sind offen?
```powershell
# Windows
netstat -ano | findstr "15500 7002 7003"

# Sollte zeigen:
# LISTENING ... 15500  (Aspire Dashboard)
# LISTENING ... 7002   (Auth Service)
```

### Check 2: AppHost Console Output
Der AppHost sollte **ohne Fehler** starten. Suchen Sie nach:

```
? "Starting project \"postgres\""
? "Starting project \"redis\""
? "Starting project \"auth-service\""
? "Listening on http://localhost:15500"

? "Error", "Exception", "Failed"
```

### Check 3: Docker Services überprüfen
```bash
# Alle Container sehen
docker-compose ps

# Sollte zeigen:
# b2connect-postgres     Up (healthy)
# b2connect-redis        Up (healthy)
# b2connect-rabbitmq     Up
# ...
```

### Check 4: Service-Logs überprüfen
```bash
# Spezifischer Service
docker-compose logs auth-service

# Real-time folgen
docker-compose logs -f catalog-service | head -50
```

---

## ?? Die 5 häufigsten Ursachen & Lösungen

| Problem | Symptom | Lösung |
|---------|---------|--------|
| **Services nicht registriert** | "No services in dashboard" | Überprüfen `IsAspireProjectResource>true` in allen .csproj |
| **AppHost startet nicht** | "Failed to start AppHost" | Überprüfen Kompilierungsfehler: `dotnet build AppHost` |
| **Ports belegt** | "Port 15500 already in use" | Killen Sie alte Prozesse: `netstat -ano \| findstr 15500` |
| **Docker nicht laufen** | "Cannot connect to Docker" | Starten Sie Docker Desktop |
| **Netzwerk-Fehler** | "Cannot reach services" | Überprüfen Sie docker network: `docker network inspect b2connect` |

---

## ?? Debug Ausgabe aktivieren

Für maximale Verbosity:

```bash
cd AppHost
dotnet run --verbosity diagnostic 2>&1 | Tee-Object -FilePath debug.log
```

Dann überprüfen Sie `debug.log` auf Fehler.

---

## ?? Factory Reset (Letzter Ausweg)

Wenn gar nichts funktioniert:

```bash
# WARNUNG: Das löscht ALLES!
docker-compose down -v
docker system prune -a --volumes
docker network prune

# Neu starten
docker-compose up -d
cd AppHost && dotnet run
```

---

## ? Verifikationschecklist

Bevor Sie aufgeben, überprüfen Sie:

- [ ] `dotnet build AppHost` kompiliert **ohne Fehler**
- [ ] Alle Service .csproj haben `<IsAspireProjectResource>true</IsAspireProjectResource>`
- [ ] `docker-compose ps` zeigt **healthy** Containers
- [ ] `netstat -ano | findstr 15500` zeigt port LISTENING
- [ ] `dotnet run` im AppHost Verzeichnis startet **ohne Fehler**
- [ ] Browser kann `http://localhost:15500` laden (keine 404 oder Connection refused)
- [ ] Browser F12 Console zeigt **keine Fehler**

---

## ?? Video Steps (Was Sie sehen sollten)

1. **AppHost Start:**
   ```
   Listening on http://localhost:15500
   ```

2. **Browser Dashboard:**
   - Aspire Dashboard lädt ?
   - Titelleiste: "Aspire Dashboard" ?
   - Services-Liste sichtbar ?
   - Grüne Health-Icons neben Services ?

3. **Wenn Services NICHT sichtbar sind:**
   - Überprüfen Sie AppHost Console auf Fehler
   - Services benötigen 30-60 Sekunden zum starten
   - Drücken Sie F5 um zu refreshen

---

## ?? Wenn Sie immer noch Hilfe brauchen:

1. Führen Sie aus: `.\scripts\diagnose.ps1`
2. Kopieren Sie **GESAMTE** Ausgabe
3. Kopieren Sie **GESAMTE** AppHost Console Output
4. Öffnen Sie ein Issue auf GitHub mit:
   - diagnose.ps1 Ausgabe
   - AppHost Console Output
   - Screenshot vom Browser F12 Console
   - `docker-compose ps` Output

---

## ?? Pro-Tipps

### Tip 1: Mehrere Terminals gleichzeitig
```bash
# Terminal 1
cd AppHost && dotnet run

# Terminal 2 (während Terminal 1 läuft)
docker-compose logs -f

# Terminal 3
curl http://localhost:7002/health  # Test einzelner Service
```

### Tip 2: Health Checks überprüfen
```bash
# Alle Services
for port in 7002 7003 7004 7005 7008 8000 8080; do
    curl -s http://localhost:$port/health | jq .
done
```

### Tip 3: VS Code Integration
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "AppHost",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/AppHost/bin/Debug/net10.0/B2Connect.AppHost.dll",
            "args": [],
            "cwd": "${workspaceFolder}/AppHost",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
    ]
}
```

---

**Für detaillierte Infos, siehe:** `docs/ASPIRE_DASHBOARD_TROUBLESHOOTING.md`
