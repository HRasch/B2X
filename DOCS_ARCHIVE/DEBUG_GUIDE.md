# Quick Start - Wenn die launch.json Fehler wirft

## Problem: "Format uri ('{0}') must contain exactly one substitution placeholder."

Diese Fehlermeldung kommt von VS Code Debugging-Konfigurationen.

## Sofort-Lösungen

### 1. VS Code komplett neu starten
```bash
# Terminal beenden
killall Code

# VS Code wieder öffnen
open /Applications/Visual\ Studio\ Code.app
```

### 2. Im Projekt öffnen
```bash
cd /Users/holger/Documents/Projekte/B2Connect
code .
```

### 3. Launch Config auswählen
- `Cmd+Shift+D` - Debug Tab öffnen
- "Full Stack" aus Dropdown wählen
- `F5` drücken

### 4. Wenn immer noch Fehler

**Option A: Manuell starten**
```bash
# Terminal 1: Frontend
cd frontend && npm run dev

# Terminal 2: Backend
cd backend/services/AppHost && dotnet run
```

**Option B: VS Code Extensions Problem**
```bash
# Alle VS Code Extensions deaktivieren
# Settings → Extensions → Disable all

# Oder C# Extension neu installieren
# Extension ID: ms-dotnettools.csharp
```

**Option C: Kompletter Reset**
```bash
# Lösche alle VS Code Dateien
rm -rf .vscode

# Recreate minimal setup
mkdir -p .vscode
cat > .vscode/launch.json << 'EOF'
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Frontend",
      "type": "node",
      "request": "launch",
      "runtimeExecutable": "npm",
      "runtimeArgs": ["run", "dev"],
      "cwd": "${workspaceFolder}/frontend",
      "console": "integratedTerminal"
    }
  ]
}
EOF
```

## Aktueller Setup

Die aktuelle `.vscode/launch.json` ist **minimal und sauber**:
- ✅ Nur essenzielle Felder
- ✅ Kein `serverReadyAction` (häufige Fehlerquelle)
- ✅ Kein komplexes Pattern-Matching
- ✅ Einfache Konfiguration

## Wenn der Fehler immer noch kommt

**Der Fehler könnte von woanders kommen:**

1. **C# Extension Bug** - Versuche zu aktualisieren
2. **Workspace Settings** - Überprüfe `workspace.json`
3. **Global VS Code Settings** - Überprüfe `~/.config/Code/settings.json`

**Debug-Modus:**
```bash
# Starte VS Code mit Debug Info
code --log debug

# Oder mit trace
code --log trace
```

## Alternativer Workflow (ohne VS Code Debugging)

Falls VS Code Debugging Probleme macht, nutze diese Alternative:

### Terminal 1: Frontend
```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend
npm run dev
# Öffnet http://localhost:3000
```

### Terminal 2: Backend
```bash
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost
dotnet run
# Öffnet http://localhost:15500 (Aspire Dashboard)
```

**Das funktioniert genauso gut wie VS Code Debugging!**

## Konfiguration überprüfen

```bash
# Zeige aktuelle launch.json
cat .vscode/launch.json | grep -i "serverReady\|uriFormat"

# Sollte NICHTS ausgeben - das wäre das Problem

# Wenn leer, ist Konfiguration in Ordnung
echo "✅ launch.json ist sauber"
```

---

**Wenn nichts davon funktioniert:** Nutze einfach das manuelle Terminal Setup oben - das ist am zuverlässigsten!
