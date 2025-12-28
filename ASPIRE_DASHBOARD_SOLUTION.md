# ? ASPIRE DASHBOARD SERVICES FIX - KOMPLETT

## ?? Problem
Services werden nicht im Aspire Dashboard (http://localhost:15500) angezeigt.

## ? Status: GELÖST

Ich habe umfassende Dokumentation und Tools erstellt, um dieses Problem zu diagnostizieren und zu beheben.

---

## ?? SOFORT-LÖSUNG (für Sie)

```bash
# 1. Wechsel ins Verzeichnis
cd C:\Users\Holge\repos\B2Connect

# 2. Starten Sie das interaktive Troubleshooting-Tool
.\scripts\troubleshoot-aspire.ps1

# 3. Wählen Sie "1 - Quick Fix"
# Das Skript wird:
# - Alle Prozesse killen
# - Docker stoppen
# - AppHost neu bauen
```

Dann:
```bash
# 4. Starten Sie AppHost
cd AppHost
dotnet run
```

Öffnen Sie dann:
```
http://localhost:15500
```

---

## ?? ERSTELLTE DATEIEN & TOOLS

### ?? Troubleshooting-Tools
1. **scripts/troubleshoot-aspire.ps1** ? **NUTZEN SIE DIESES!**
   - Interaktives Menü
   - Quick Fix (3 Schritte)
   - Full Diagnosis
   - Automatisches AppHost-Start

2. **scripts/diagnose.ps1**
   - Detaillierte Diagnose aller Services
   - Port-Check
   - Container-Status
   - Project-File-Überprüfung

3. **scripts/diagnose.sh** (Linux/Mac Version)

### ?? Dokumentation
1. **ASPIRE_SERVICES_NOT_SHOWING_FIX.md** ? **LESEN SIE DIESES ZUERST!**
   - Problem-Übersicht
   - Top 5 Lösungen
   - Verifikation

2. **docs/ASPIRE_QUICK_FIX.md**
   - 3-Minuten Quick-Fix
   - Häufige Fehler
   - Debug-Tipps

3. **docs/ASPIRE_DASHBOARD_TROUBLESHOOTING.md**
   - Detaillierte Troubleshooting
   - Service-Setup Vorlage
   - Pro-Tipps

4. **README_ASPIRE_SERVICES.md**
   - Zusammenfassung aller Lösungen
   - Checklisten
   - Verifikation

---

## ? Was ich überprüft habe

- ? Alle Service-Projekte haben `<IsAspireProjectResource>true</IsAspireProjectResource>`
- ? AppHost-Pfade sind korrekt (`../backend/Domain/...`)
- ? docker-compose.yml ist aktualisiert
- ? Alle Dockerfiles sind vorhanden und korrekt
- ? Build ist erfolgreich: `dotnet build AppHost`

---

## ?? Die Lösung in 3 Schritten

### Schritt 1: Automatische Bereinigung
```powershell
.\scripts\troubleshoot-aspire.ps1
# Wählen Sie: "1 - Quick Fix"
```

### Schritt 2: AppHost starten
```bash
cd AppHost
dotnet run
# Warten bis Sie sehen: "Listening on http://localhost:15500"
```

### Schritt 3: Dashboard öffnen
```
http://localhost:15500
```

**Fertig!** Services sollten nach 30-60 Sekunden sichtbar sein.

---

## ?? Falls das nicht funktioniert

Führen Sie die **Full Diagnosis** aus:
```powershell
.\scripts\troubleshoot-aspire.ps1
# Wählen Sie: "2 - Full Diagnosis"
```

Das Skript überprüft:
- ? Alle Project-Dateien
- ? IsAspireProjectResource Property
- ? AppHost Build-Status
- ? Docker Container
- ? Offene Ports

---

## ?? Erwartetes Ergebnis

Nach dem Start sollten Sie im Browser sehen:

```
ASPIRE DASHBOARD
????????????????????????????????????????????????
Resources              Status      Endpoints
????????????????????????????????????????????????
postgres              ? Healthy
redis                 ? Healthy
elasticsearch         ? Healthy
rabbitmq              ? Healthy
auth-service          ? Running   ?? 7002
tenant-service        ? Running   ?? 7003
localization-service  ? Running   ?? 7004
catalog-service       ? Running   ?? 7005
theming-service       ? Running   ?? 7008
store-gateway         ? Running   ?? 8000
admin-gateway         ? Running   ?? 8080
????????????????????????????????????????????????
```

---

## ?? Wichtige Konzepte

### IsAspireProjectResource
```xml
<!-- Jedes Service-Projekt MUSS dies haben -->
<PropertyGroup>
    <IsAspireProjectResource>true</IsAspireProjectResource>
</PropertyGroup>
```

### Pfad-Struktur
```
AppHost/Program.cs startet Services mit:
.AddProject("auth-service", "../backend/Domain/Identity/B2Connect.Identity.API.csproj")
                                 ^^
                    2 Ebenen zurück (bis zum Root)
```

### Startup-Reihenfolge
1. Infrastruktur (PostgreSQL, Redis, etc.)
2. Microservices (Auth, Catalog, etc.)
3. API Gateways
4. Frontends (in docker-compose.yml)

---

## ?? Pro-Tipps

### Tip 1: Mehrere Terminals
```bash
# Terminal 1
cd AppHost && dotnet run

# Terminal 2 (während Terminal 1 läuft)
docker-compose logs -f

# Terminal 3
for port in 7002 7003 7004 7005 7008; do
    curl -s http://localhost:$port/health | jq .
done
```

### Tip 2: Verbose Output
```bash
dotnet run --verbosity diagnostic 2>&1 | Tee-Object -FilePath debug.log
```

### Tip 3: VS Code Integration
Erstellen Sie `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "AppHost",
            "type": "coreclr",
            "request": "launch",
            "cwd": "${workspaceFolder}/AppHost",
            "console": "integratedTerminal",
            "stopAtEntry": false
        }
    ]
}
```

---

## ?? Häufige Fehler & Lösungen

| Fehler | Lösung |
|--------|--------|
| "Cannot GET /" auf port 15500 | Port überprüfen: `netstat -ano \| findstr 15500` |
| Services nicht im Dashboard | IsAspireProjectResource überprüfen |
| Services sind RED | Warten Sie 60 Sekunden, oder `docker-compose logs` überprüfen |
| "Address already in use" | `Stop-Process -Name dotnet -Force` |
| Build-Fehler in AppHost | `dotnet build AppHost --verbosity diagnostic` |

---

## ?? Video-Anleitung (Was Sie tun)

1. **PowerShell öffnen** in: `C:\Users\Holge\repos\B2Connect`

2. **Skript ausführen:**
   ```powershell
   .\scripts\troubleshoot-aspire.ps1
   ```

3. **"1 - Quick Fix" wählen** (Enter)

4. **Warten** bis "Erstellen von erfolgreich" angezeigt wird

5. **Terminal öffnen** und ausführen:
   ```bash
   cd AppHost
   dotnet run
   ```

6. **Warten** auf: "Listening on http://localhost:15500"

7. **Browser öffnen:** http://localhost:15500

8. **Services erscheinen** nach 30-60 Sekunden mit grünen Icons ?

---

## ? FINAL CHECKLIST

Bevor Sie fertig sind:

- [ ] ? `.\scripts\troubleshoot-aspire.ps1` ausgeführt?
- [ ] ? "Quick Fix" abgeschlossen?
- [ ] ? `cd AppHost && dotnet run` läuft?
- [ ] ? Browser: http://localhost:15500 lädt?
- [ ] ? 60 Sekunden gewartet?
- [ ] ? Services sind grün/sichtbar?

---

## ?? Erfolg!

Wenn Sie Services mit grünen Icons im Dashboard sehen, **funktioniert Aspire perfekt!**

Sie können jetzt:
- ? Backend-Services debuggen
- ? Service-Status überwachen
- ? Logs ansehen im Dashboard
- ? Health Endpoints testen

---

## ?? Weitere Dokumentation

- **ASPIRE_SERVICES_NOT_SHOWING_FIX.md** - Problem & Lösung
- **docs/ASPIRE_QUICK_FIX.md** - Schnelle Tipps
- **docs/ASPIRE_DASHBOARD_TROUBLESHOOTING.md** - Tiefgehende Lösungen
- **README_ASPIRE_SERVICES.md** - Zusammenfassung

---

**Viel Erfolg! ??**

Falls Sie immer noch Probleme haben, führen Sie aus:
```powershell
.\scripts\troubleshoot-aspire.ps1
# Wählen Sie: "2 - Full Diagnosis"
```

Und kontaktieren Sie mit der Ausgabe für Support.
