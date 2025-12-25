# VS Code Startup-Konfiguration

## 🚀 Quick Start

### Option 1: Keyboard Shortcuts (Schnellste Methode)

| Shortcut | Aktion |
|----------|--------|
| `Ctrl+Shift+F5` | ✅ **Alles starten** (Backend + Frontend) |
| `Ctrl+Shift+F6` | 🚀 Backend nur (Aspire) |
| `Ctrl+Shift+F7` | 🎨 Frontend nur (Vue Dev Server) |
| `Ctrl+Shift+F9` | 🎯 E2E Tests ausführen |
| `Ctrl+Shift+F12` | 🛑 Alle Services stoppen |

### Option 2: Tasks aus VS Code

1. **`Ctrl+Shift+P`** → "Tasks: Run Task" eingeben
2. Gewünschte Task auswählen:
   - ✅ **Full Startup (Backend + Frontend)** - Empfohlen!
   - 🚀 Backend Aspire (aspire-start.sh)
   - 🎨 Frontend Dev (port 5173)
   - 🎯 E2E Tests (Language Selection)
   - 🛑 Stop Services

### Option 3: Terminal

```bash
# Alles starten
bash aspire-start.sh Development Debug  # Terminal 1
cd frontend && npm run dev -- --port 5173  # Terminal 2

# Tests ausführen
cd frontend && npm run e2e -- language-selection.spec.ts

# Alles stoppen
bash aspire-stop.sh
```

---

## 📋 Available Tasks

### Backend Tasks
- **backend-restore** - NuGet Packages wiederherstellen
- **backend-build** - Backend kompilieren
- **backend-test** - .NET Tests ausführen
- **🚀 Backend Aspire (aspire-start.sh)** - Aspire Dev Environment starten

### Frontend Tasks
- **frontend-install** - npm Dependencies installieren
- **frontend-dev** - Vite Dev Server starten
- **frontend-build** - Frontend für Production bauen
- **frontend-test** - Unit Tests ausführen
- **🎨 Frontend Dev (port 5173)** - Dev Server mit Port 5173
- **🎯 E2E Tests (Language Selection)** - Playwright E2E Tests

### Combined Tasks
- **✅ Full Startup (Backend + Frontend)** - Beide Services parallel starten
- **🛑 Stop Services** - Alles sauber beenden

---

## 🎯 Empfohlener Workflow

### Für die Entwicklung:

1. **Starten:**
   ```
   Ctrl+Shift+F5  (oder Tasks: Run Task → Full Startup)
   ```
   Dies startet:
   - Backend auf Port 9000 (AppHost Dashboard)
   - Services auf Ports 5000-5003
   - Frontend auf Port 5173

2. **Entwickeln:**
   - Frontend Code ändern → Automatischer Hot Reload
   - Backend Code ändern → Automatischer Restart (watch mode)

3. **Testen:**
   ```
   Ctrl+Shift+F9  (E2E Tests)
   ```

4. **Stoppen:**
   ```
   Ctrl+Shift+F12
   ```

---

## 🔗 URLs nach dem Start

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| AppHost Dashboard | http://localhost:9000 |
| API Gateway | http://localhost:5000 |
| Auth Service | http://localhost:5001 |
| Tenant Service | http://localhost:5002 |
| Localization Service | http://localhost:5003 |

---

## 💡 Tipps

- **Ctrl+Shift+~** öffnet das Terminal in VS Code für die Ausgabe
- **Ports-View** (in der Activity Bar) zeigt alle Ports
- **Output-Panel** zeigt Logs aller laufenden Tasks
- Tasks können auch über die Command Palette gestartet werden: `Ctrl+Shift+P` → "Tasks: Run Task"

---

## 🔧 Anpassungen

Falls du die Keyboard Shortcuts ändern möchtest:
1. **File → Preferences → Keyboard Shortcuts**
2. Nach "B2Connect" suchen
3. Shortcuts bearbeiten

Oder direkt in `.vscode/keybindings.json` bearbeiten.
