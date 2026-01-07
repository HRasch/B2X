# VS Code Launch-Konfiguration für B2X

## 🚀 Verfügbare Launch-Konfigurationen

### Backend - InMemory Database

**🚀 Aspire AppHost (InMemory)** ⭐ EMPFOHLEN
- Startet den kompletten Aspire AppHost mit InMemory-Datenbank
- Projekt: `backend/services/Orchestration` (B2X.Orchestration.csproj)
- Keine PostgreSQL/Docker nötig
- Alle Services automatisch orchestriert
- **Shortcut**: `F5` → Wähle "🚀 Aspire AppHost (InMemory)"
- **Ports**:
  - AppHost/Dashboard: http://localhost:9000
  - Auth Service: http://localhost:9002
  - Tenant Service: http://localhost:9003
  - Localization Service: http://localhost:9004
  - Catalog Service: http://localhost:9001 (wenn aktiv)

### Backend + Frontend (Parallel)

**🚀 Aspire AppHost + Frontend (InMemory)** ⭐⭐ AM BESTEN!
- Startet AppHost + Frontend gleichzeitig
- InMemory-Datenbank
- Ideal für vollständige lokale Entwicklung
- **Shortcut**: `F5` → Wähle "🚀 Aspire AppHost + Frontend (InMemory)"
- **Ports**:
  - Frontend: http://localhost:5173
  - AppHost: http://localhost:9000

### Frontend

**Frontend (npm dev)**
- Startet nur das Frontend mit Vite DevServer
- **Shortcut**: `F5` → Wähle "Frontend (npm dev)"
- **Port**: http://localhost:5173

### Tests

**Frontend Tests**
- Führt Vitest Unit-Tests aus
- **Shortcut**: `F5` → Wähle "Frontend Tests"

**Backend Tests (.NET)**
- Führt xUnit Tests aus
- **Shortcut**: `F5` → Wähle "Backend Tests (.NET)"

---

## 📋 Umgebungsvariablen

### Verfügbare Variablen in launch.json

```json
{
  "Database__Provider": "inmemory",  // InMemory aktivieren
  "ASPNETCORE_ENVIRONMENT": "Development",
  "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS": "true"  // Dashboard ohne Auth
}
```

### Ändern zur PostgreSQL

In `.vscode/launch.json` die InMemory-Konfiguration anpassen:

```json
"env": {
  "Database__Provider": "postgresql",  // ← Ändern zu "postgresql"
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

---

## 🔧 Available Tasks (Ctrl+Shift+B)

| Task | Zweck |
|------|-------|
| `build-apphost` | Build Aspire AppHost |
| `build-backend` | Build gesamtes Backend |
| `test-backend` | Führe Backend-Tests aus |
| `test-frontend` | Führe Frontend-Tests aus |
| `dev-frontend` | Starte Frontend Dev-Server |
| `lint-frontend` | Frontend Linting |

---

## 🎯 Schnellstart

### 1. Einfaches Setup (nur Backend)
```
1. F5
2. Wähle "🚀 Aspire AppHost (InMemory)"
3. Wartet bis "Now listening on: ..."
4. AppHost-Dashboard öffnet sich automatisch
```

### 2. Vollständiges Setup (Backend + Frontend)
```
1. F5
2. Wähle "🚀 Aspire AppHost + Frontend (InMemory)"
3. Beide Anwendungen starten parallel
4. Frontend öffnet sich automatisch auf http://localhost:5173
5. AppHost-Dashboard auf http://localhost:9000
```

### 3. Tests ausführen
```
Ctrl+Shift+B
Wähle:
- "test-backend" für Backend-Tests
- "test-frontend" für Frontend-Tests
```

---

## 🐛 Debugging

### Breakpoints setzen
1. Klicke auf die Zeilennummer links im Editor
2. Ein roter Punkt erscheint
3. Starte die Konfiguration mit F5
4. Code pausiert bei Breakpoints

### Debug Konsole
- **Ctrl+Shift+Y** öffnet die Debug-Konsole
- Dort können Variablen inspiziert werden
- Kommandos können ausgeführt werden

### Watched Expressions
- Im Debug-Panel "Watch" hinzufügen
- Werte werden während des Debuggings aktualisiert

---

## 📊 Service-Ports Übersicht

| Service | Port | URL |
|---------|------|-----|
| AppHost/Dashboard | 9000 | http://localhost:9000 |
| Catalog Service | 9001 | http://localhost:9001 |
| Auth Service | 9002 | http://localhost:9002 |
| Tenant Service | 9003 | http://localhost:9003 |
| Localization Service | 9004 | http://localhost:9004 |
| Frontend | 5173 | http://localhost:5173 |
| Admin Frontend | 5174 | http://localhost:5174 |

---

## ✅ Checkliste

- [x] `.vscode/launch.json` konfiguriert
- [x] `.vscode/tasks.json` konfiguriert
- [x] `.vscode/settings.json` konfiguriert
- [x] `.vscode/extensions.json` mit Empfehlungen
- [x] InMemory-Database-Unterstützung
- [x] Compound-Konfigurationen für Parallel-Start
- [x] Auto-Open Browser-Konfiguration

---

## 🆘 Fehlersuche

### AppHost startet nicht
```bash
cd backend/services/AppHost
dotnet build
dotnet run
```

### Frontend startet nicht
```bash
cd frontend
npm install
npm run dev
```

### InMemory-Datenbank wird nicht verwendet
- Prüfe ob `Database__Provider: inmemory` in launch.json gesetzt ist
- Wende "Clean" an: **Ctrl+Shift+P** → "C#: Clean Up Workspace"

### Port bereits in Verwendung
```powershell
# Windows: Finde Prozess auf Port 9000
netstat -ano | findstr :9000

# Kill Prozess
taskkill /PID <PID> /F
```

---

**Zuletzt aktualisiert**: 2025-12-26
