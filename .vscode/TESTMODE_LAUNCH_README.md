# 🧪 TestMode Launch-Konfigurationen

VS Code Launch-Konfigurationen für das TestMode-System der B2Connect Admin-Anwendung.

## 🚀 Verfügbare Konfigurationen

### **🧪 Frontend TestMode (Admin)**
Startet den Admin-Frontend mit aktiviertem TestMode.

**Features:**
- Automatische TestMode-Aktivierung
- Browser öffnet sich automatisch mit `?testmode=true`
- Dev-Server auf Port 5174
- Vollständige TestMode-Funktionalität verfügbar

**Verwendung:**
1. Öffnen Sie die Run & Debug Sidebar (Ctrl+Shift+D)
2. Wählen Sie "🧪 Frontend TestMode (Admin)"
3. Drücken Sie F5 oder klicken Sie auf den Play-Button
4. Browser öffnet sich automatisch mit aktiviertem TestMode

### **🧪 Frontend TestMode (Store)**
Startet den Store-Frontend mit aktiviertem TestMode.

**Features:**
- Gleiche Funktionalität wie Admin TestMode
- Dev-Server auf Port 5175
- Für Store-spezifisches Testing

### **🧪 TestMode Demo Runner**
Führt die automatisierte TestMode-Demo aus.

**Features:**
- Startet automatisch den Frontend-Dev-Server
- Führt systematische Tests durch
- Demonstriert alle TestMode-Features
- Erstellt detaillierte Logs

## 🔧 Compound-Konfigurationen

### **🧪 Full Stack + TestMode**
Startet sowohl das Backend (Full Stack) als auch den Admin-Frontend mit TestMode.

**Verwendung:**
- Vollständige Entwicklungsumgebung mit TestMode
- Backend + Frontend gleichzeitig debuggen
- Alle Services verfügbar

### **🧪 TestMode + Demo**
Startet Frontend TestMode und führt automatisch die Demo aus.

**Verwendung:**
- Automatisierte TestMode-Präsentation
- Perfekt für Demos und Schulungen

## 📋 Voraussetzungen

### System Requirements
- Node.js 18+
- VS Code mit folgenden Extensions:
  - MS-VSCode.vscode-json
  - Vue.volar
  - ms-dotnettools.csharp

### Projekt Setup
```bash
# Stelle sicher, dass Dependencies installiert sind
cd frontend/Admin && npm install
cd ../Store && npm install

# Backend Dependencies (für Full Stack)
dotnet restore
```

## 🎯 TestMode Features in Launch Config

### Automatische Aktivierung
- `VITE_TESTMODE_ENABLED=true` Environment Variable
- URL Parameter `?testmode=true` wird automatisch hinzugefügt
- Keine manuelle Aktivierung nötig

### Debug Panel
- Nach Start: `Ctrl + Shift + T` drücken um Debug Panel zu öffnen
- Live-Monitoring aller Browser-Aktionen
- Statistiken und Auto-Fix Controls

### Auto-Fix Engine
- 7 vordefinierte Auto-Fix-Regeln
- Automatische Behebung bekannter Fehler
- Navigation-Fehler, Auth-Issues, API-Timeouts, etc.

## 🔍 Debugging Features

### Breakpoints
- Setzen Sie Breakpoints im TestMode-Code
- Debuggen Sie die Auto-Fix-Logik
- Überwachen Sie Browser-Events

### Console Logs
- Alle TestMode-Aktivitäten werden geloggt
- Detaillierte Fehlerberichte
- Performance-Metriken

### Hot Reload
- Änderungen am TestMode-Code werden automatisch neu geladen
- Kein Neustart des Debuggers nötig

## 📊 Monitoring & Analyse

### Live Statistics
- Success Rate aller Aktionen
- API Response Times
- Error Count und Types
- Performance-Metriken

### Log Export
- Vollständige Action-History exportieren
- JSON-Format für weitere Analyse
- Debugging-Informationen

## 🚨 Troubleshooting

### "Vite nicht gefunden"
```bash
# Installiere Dependencies
cd frontend/Admin && npm install
```

### "Port bereits belegt"
```bash
# Finde und beende Prozesse auf Port 5174
lsof -ti:5174 | xargs kill -9
```

### "TestMode wird nicht aktiviert"
- Prüfen Sie Browser Console auf Fehler
- Stellen Sie sicher, dass `VITE_TESTMODE_ENABLED=true` gesetzt ist
- Öffnen Sie die URL mit `?testmode=true`

### "Backend nicht verfügbar"
- Für Full Stack: Starten Sie zuerst nur "🚀 Full Stack"
- Warten Sie bis Aspire Dashboard verfügbar ist
- Dann starten Sie TestMode separat

## 🎬 Demo-Szenario

1. **Starten Sie "🧪 TestMode + Demo"**
2. **Browser öffnet sich automatisch**
3. **Demo führt systematische Tests durch:**
   - Navigation Testing
   - Error Simulation
   - Auto-Fix Demonstration
   - Performance Monitoring
4. **Debug Panel zeigt Live-Results**
5. **Log Export für Analyse**

## 📝 Keyboard Shortcuts

- `F5`: Start selected configuration
- `Shift+F5`: Stop debugging
- `Ctrl+Shift+T`: Toggle TestMode Debug Panel (im Browser)
- `Ctrl+Shift+D`: Open Run & Debug sidebar

## 🔗 Related Files

- `frontend/Admin/src/utils/testMode.ts` - TestMode Core Logic
- `frontend/Admin/src/components/common/TestModeDebug.vue` - Debug UI
- `frontend/Admin/testmode-demo.js` - Demo Script
- `frontend/Admin/testmode-control.sh` - Control Script
- `frontend/Admin/TESTMODE_README.md` - Vollständige Dokumentation