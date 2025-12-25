# VS Code Start-Konfiguration

## 📋 Überblick

Die VS Code Konfiguration wurde für das B2Connect Aspire-Projekt optimiert. Sie bietet mehrere sofort einsatzbereite Debug-, Launch- und Task-Konfigurationen.

## 🚀 Launch-Konfigurationen

### 1. **AppHost (Debug)**
- **Typ**: CoreCLR (.NET)
- **Zweck**: Startet AppHost mit vollständigem Debugger-Support
- **Features**:
  - Source Maps aktiviert
  - Just My Code Debugging
  - Environment: Development
  - Port: 9000
  - Pre-Task: `backend-build`

**Verwendung**:
```
Klick: Run > Start Debugging (oder F5)
Wähle: AppHost (Debug)
```

### 2. **AppHost with Orchestration**
- **Typ**: CoreCLR (.NET)
- **Zweck**: Startet AppHost mit allen Services
- **Features**:
  - Alle Micro services werden über bash-Script gestartet
  - Pre-Task: `aspire-full-start`

**Verwendung**:
```
Klick: Run > Start Debugging
Wähle: AppHost with Orchestration
```

### 3. **Frontend (Debug)**
- **Typ**: Node.js
- **Zweck**: Startet Vue.js Development Server mit Hot Module Replacement (HMR)
- **Features**:
  - API URL: http://localhost:9000
  - Node internals skipped
  - Vite Dev Server

**Verwendung**:
```
Klick: Run > Start Debugging
Wähle: Frontend (Debug)
```

### 4. **Frontend Tests (Vitest)**
- **Typ**: Node.js
- **Zweck**: Führt Unit- und Component-Tests aus
- **Command**: `npm run test -- --reporter=verbose`

### 5. **E2E Tests (Playwright)**
- **Typ**: Node.js
- **Zweck**: Führt End-to-End Tests aus
- **Command**: `npm run e2e`

### 6. **Docker Compose Stack**
- **Typ**: Node.js
- **Zweck**: Startet komplette containerisierte Stack
- **Pre-Task**: `docker-compose-up`

## 🔗 Compound Konfigurationen

### **Full Stack (AppHost + Frontend)**
Startet gleichzeitig:
- AppHost Debugger
- Frontend Dev Server

```
Klick: Run > Start Debugging
Wähle: Full Stack (AppHost + Frontend)
```

### **Full Stack with All Services**
Startet gleichzeitig:
- AppHost mit allen Services (Bash-Orchestration)
- Frontend Dev Server

```
Klick: Run > Start Debugging
Wähle: Full Stack with All Services
```

### **Testing Suite**
Startet gleichzeitig:
- Vitest (Unit & Component Tests)
- Playwright (E2E Tests)

```
Klick: Run > Start Debugging
Wähle: Testing Suite
```

## ⚙️ Tasks

### Build Tasks

| Task | Befehl | Zweck |
|------|--------|-------|
| `backend-restore` | `dotnet restore` | NuGet-Pakete wiederherstellen |
| `backend-build` | `dotnet build --configuration Debug` | Debug-Build |
| `backend-build-release` | `dotnet build --configuration Release` | Release-Build |
| `frontend-install` | `npm install` | Node-Abhängigkeiten installieren |
| `frontend-build` | `npm run build` | Production-Build |

### Run Tasks

| Task | Befehl | Zweck |
|------|--------|-------|
| `aspire-run` | `dotnet run` | AppHost starten |
| `aspire-watch` | `dotnet watch run` | AppHost mit Hot-Reload |
| `aspire-full-start` | `bash aspire-start.sh` | Alle Services starten |
| `docker-compose-up` | `docker-compose ... up -d` | Docker Stack starten |
| `docker-compose-down` | `docker-compose ... down` | Docker Stack stoppen |
| `frontend-dev` | `npm run dev` | Vite Dev Server |

### Test Tasks

| Task | Befehl | Zweck |
|------|--------|-------|
| `frontend-test` | `npm run test` | Unit & Component Tests |
| `frontend-e2e` | `npm run e2e` | E2E Tests |
| `backend-test` | `dotnet test --verbosity=normal` | .NET Tests |

### Utility Tasks

| Task | Befehl | Zweck |
|------|--------|-------|
| `health-check` | `bash deployment-status.sh local` | Service-Health prüfen |
| `clean` | `dotnet clean` + Cleanup | Build-Artefakte löschen |
| `full-stack-prepare` | `npm install --prefix frontend` | Frontend vorbereiten |

## 🎯 Empfohlene Workflows

### Workflow 1: Lokal entwickeln (AppHost + Frontend)

1. **VS Code öffnen** → Run & Debug (Sidebar)
2. **Configuration wählen**: "Full Stack (AppHost + Frontend)"
3. **F5** oder Play-Button drücken
4. **Ergebnis**:
   - AppHost läuft auf http://localhost:9000
   - Frontend läuft auf http://localhost:5173
   - Beide sind mit Debugger verbunden

### Workflow 2: Alle Services starten (Vollständiges System)

1. **VS Code öffnen** → Run & Debug
2. **Configuration wählen**: "Full Stack with All Services"
3. **F5** drücken
4. **Ergebnis**:
   - Alle 5 Microservices laufen
   - PostgreSQL + Redis Datenebene aktiv
   - Frontend Dev Server aktiv

### Workflow 3: Nur Backend debuggen

1. **Run → Start Debugging**
2. **Configuration**: "AppHost (Debug)"
3. **F5**
4. **Breakpoints setzen** in C#-Code und testen

### Workflow 4: Nur Frontend entwickeln

1. **Run → Start Debugging**
2. **Configuration**: "Frontend (Debug)"
3. **F5**
4. **Browser öffnet** automatisch auf http://localhost:5173

### Workflow 5: Tests ausführen

1. **Run → Start Debugging**
2. **Configuration**: "Testing Suite"
3. **F5**
4. **Tests laufen parallel**:
   - Unit Tests (Vitest)
   - E2E Tests (Playwright)

## 🔧 Environment-Variablen

### AppHost
```
ASPNETCORE_ENVIRONMENT = Development
ASPNETCORE_URLS = http://localhost:9000
```

### Frontend
```
VITE_API_URL = http://localhost:9000
```

Diese sind automatisch in den Launch-Konfigurationen gesetzt.

## 📊 Problem Matcher

Problem Matcher helfen VS Code, Fehler in der Output-Konsole zu erkennen:

| Pattern | Erkannt | Verwendet für |
|---------|---------|------------------|
| `$msCompile` | .NET Compiler-Fehler | Backend Build & Tests |
| `$npm` | npm Fehler | Frontend Tasks |
| Custom (Regexp) | Background Task-Status | Aspire Services |

## 🚦 Status Indicators

Während Tasks laufen, sehen Sie in VS Code:

- **Blinking Dot**: Task läuft im Hintergrund
- **Status Bar**: "x Tasks Running"
- **Terminal Output**: Live-Output
- **Problems Panel**: Fehler und Warnungen

## 💡 Tipps & Tricks

### 1. Multiple Debugging Sessions
Sie können mehrere Debug-Sessions gleichzeitig haben:
- AppHost Debug + Frontend Debug
- Unit Tests + E2E Tests
- etc.

### 2. Keyboard Shortcuts
```
F5              - Debugging starten
Shift+F5        - Debugging stoppen
Ctrl+Shift+D    - Debug View öffnen
Ctrl+Shift+`    - Terminal öffnen
Ctrl+Alt+N      - Task ausführen
```

### 3. Conditional Breakpoints
```csharp
// Im Code rechtsklick auf Breakpoint-Punkt → "Edit Breakpoint"
// Bedingung eingeben z.B.: tenant.Id == "xyz"
```

### 4. Watch Expressions
Im Debug Panel → "Watch" → Add Expression:
```
$config.Services.ApiGateway
health.Services.Count
```

### 5. Call Stack Navigation
- Jeden Frame anklicken um zu Code zu springen
- Lokale Variablen sehen
- Expression evaluieren

## 📝 Anpassen der Konfiguration

### Neue Launch-Konfiguration hinzufügen

In `.vscode/launch.json`:
```json
{
  "name": "Custom Config",
  "type": "coreclr",
  "request": "launch",
  "program": "...",
  "args": [],
  "cwd": "${workspaceFolder}",
  "console": "integratedTerminal"
}
```

### Neue Task hinzufügen

In `.vscode/tasks.json`:
```json
{
  "label": "my-custom-task",
  "type": "shell",
  "command": "bash",
  "args": ["my-script.sh"],
  "problemMatcher": [],
  "presentation": {
    "reveal": "always",
    "panel": "new"
  }
}
```

## 🆘 Troubleshooting

### Problem: "Pre-launch task failed"
- **Lösung**: Task manuell ausführen → Terminal öffnen → `npm install` oder `dotnet restore`

### Problem: Port bereits in Verwendung
- **Lösung**: Andere Instanz stoppen oder Port in `appsettings.json` ändern

### Problem: Debugger verbindet nicht
- **Lösung**: 
  1. VS Code neu starten
  2. C# Extension neu laden
  3. Port 9000 prüfen

### Problem: Frontend zeigt alte Version
- **Lösung**: 
  1. Dev Server neustarten (Stop + Start)
  2. `frontend/.vite` löschen
  3. Browser-Cache leeren

### Problem: "dotnet not found"
- **Lösung**: .NET SDK installieren oder PATH prüfen

## 📚 Weitere Ressourcen

- [VS Code Debugging Documentation](https://code.visualstudio.com/docs/editor/debugging)
- [.NET Debugging Guide](https://learn.microsoft.com/dotnet/core/diagnostics/debugging-with-vs-code)
- [Node.js Debugging](https://code.visualstudio.com/docs/nodejs/nodejs-debugging)
- [Launch Configuration Reference](https://code.visualstudio.com/docs/editor/launch-configuration)

---

**Version**: 1.0.0  
**Last Updated**: 2025-01-15  
**Framework**: .NET 10 & Aspire 10 + Vue.js 3
