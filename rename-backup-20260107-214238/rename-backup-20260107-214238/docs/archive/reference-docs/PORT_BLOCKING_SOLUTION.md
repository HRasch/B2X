# Port-Blockierung Problem & Lösung

## Problem

Wenn Aspire Orchestration über VS Code Tasks gestartet wird (`isBackground: true`), werden die Prozesse **detached** vom Terminal. Das bedeutet:

1. **Terminal schließen stoppt KEINE Services**
2. Child-Prozesse (dotnet, node) laufen weiter
3. Ports bleiben belegt (7002, 7003, 7004, 7005, 7008, 8000, 8080, 5173, 5174, 15500)
4. Neustart schlägt fehl mit "Port already in use"

## Ursache

```
Terminal (Ctrl+C) → Beendet nur Parent Shell
    ↓
Aspire Process (detached) → Läuft weiter
    ↓
Child Services (dotnet/node) → Laufen alle weiter
```

**Background-Tasks** in VS Code Tasks werden mit `nohup` ähnlich gestartet und überleben Terminal-Schließung.

## Lösung 1: Cleanup-Script verwenden

```bash
# Alle Services stoppen
./scripts/kill-all-services.sh

# Oder in VS Code: Cmd+Shift+P → "Tasks: Run Task" → "kill-all-services"
```

Das Script:
- Stoppt Aspire Orchestration
- Killt alle .NET Backend-Services
- Killt Frontend Vite Dev Server
- Gibt alle Ports frei (7002-8080, 5173, 5174, 15500)

## Lösung 2: VS Code Tasks

### Stoppen

```
Cmd+Shift+P → Tasks: Run Task → kill-all-services
```

### Neustarten

```
Cmd+Shift+P → Tasks: Run Task → restart-all-services
```

Dieser Task:
1. Stoppt alle Services mit `kill-all-services`
2. Wartet bis Ports frei sind
3. Startet Aspire Orchestration neu

## Lösung 3: Manuelle Prozess-Terminierung

```bash
# Finde Prozesse auf bestimmten Ports
lsof -i :15500
lsof -i :7002
lsof -i :8080

# Töte Prozess auf Port
lsof -ti:15500 | xargs kill -9

# Töte alle B2X Prozesse
pkill -f "B2X"
pkill -f "Aspire"
```

## Lösung 4: Process Groups (Best Practice)

**Problem**: VS Code Tasks starten Prozesse ohne Process Group

**Future Fix**: Eigenes Start-Script erstellen:

```bash
#!/bin/bash
# scripts/start-aspire.sh

# Trap SIGTERM/SIGINT und leite an Child-Prozesse weiter
trap 'kill -TERM $ASPIRE_PID; exit 0' SIGTERM SIGINT

dotnet run --project backend/Orchestration/B2X.Orchestration.csproj &
ASPIRE_PID=$!

wait $ASPIRE_PID
```

Dann in tasks.json:
```json
{
  "label": "backend-start",
  "command": "${workspaceFolder}/scripts/start-aspire.sh"
}
```

## Prävention

### 1. Vor jedem Start cleanup ausführen

```bash
./scripts/kill-all-services.sh && dotnet run --project backend/Orchestration/B2X.Orchestration.csproj
```

### 2. Alias in .zshrc/.bashrc

```bash
# ~/.zshrc
alias b2c-kill="~/Documents/Projekte/B2X/scripts/kill-all-services.sh"
alias b2c-start="b2c-kill && cd ~/Documents/Projekte/B2X && dotnet run --project backend/Orchestration/B2X.Orchestration.csproj"
```

### 3. Pre-launch Task in VS Code

In `.vscode/tasks.json`:
```json
{
  "label": "backend-start",
  "dependsOn": ["kill-all-services"],
  "dependsOrder": "sequence"
}
```

## Überwachung

### Aktive Ports prüfen

```bash
# Alle B2X Ports
lsof -i :7002 -i :7003 -i :7004 -i :7005 -i :7008 -i :8000 -i :8080 -i :5173 -i :5174 -i :15500
```

### Prozess-Baum anzeigen

```bash
pstree | grep -E "(dotnet|node|Aspire)"
```

### Port-Status Dashboard

```bash
watch -n 2 'lsof -i :15500 -i :8080 -i :7002 | grep LISTEN'
```

## Debugging

### Wenn Port immer noch belegt ist

```bash
# Zeige Prozess-Details
lsof -i :7002

# Output:
# COMMAND   PID  USER   FD   TYPE  DEVICE SIZE/OFF NODE NAME
# dotnet  98581 holger  123u IPv6 0x... TCP localhost:7002 (LISTEN)

# Töte Prozess
kill -9 98581

# Oder force kill allen dotnet Prozessen
pkill -9 dotnet
```

### Wenn MSBuild Ports blockiert

```bash
# MSBuild Worker stoppen
pkill -f "MSBuild.dll"
pkill -f "VBCSCompiler"

# .NET Build Cache löschen
dotnet build-server shutdown
```

## Bekannte Issues

1. **Docker Desktop**: Manchmal belegt Docker Port 5173/5174 wenn Container auf Host-Netzwerk laufen
   ```bash
   docker ps
   docker stop <container_id>
   ```

2. **Zombie Processes**: Wenn Parent-Prozess crashed, bleiben Children als Zombies
   ```bash
   ps aux | grep defunct
   ```

3. **VS Code Extension Server**: Manchmal hält DevKit Extension Prozesse offen
   ```bash
   pkill -f "Microsoft.VisualStudio"
   ```

## Best Practice für neue Entwickler

```bash
# 1. Repository klonen
git clone <repo>
cd B2X

# 2. Cleanup ausführen (falls vorige Session crashed)
./scripts/kill-all-services.sh

# 3. Services starten
dotnet run --project backend/Orchestration/B2X.Orchestration.csproj

# 4. Bei Problemen: Force-Kill und restart
Ctrl+C
./scripts/kill-all-services.sh
dotnet run --project backend/Orchestration/B2X.Orchestration.csproj
```

---

**Zusammenfassung**: Verwende **immer** das Cleanup-Script vor dem Neustart, um Port-Konflikte zu vermeiden.
