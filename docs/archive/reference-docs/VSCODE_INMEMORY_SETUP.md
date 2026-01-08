# VS Code Launch-Konfiguration für InMemory-Datenbank

> Schnelles Setup für Entwicklung ohne Datenbank-Installation

## 🚀 Quick Start

Die Launch-Konfiguration wurde aktualisiert. Du findest jetzt im Debug-Menü (F5):

### Neue InMemory-Konfigurationen

1. **🚀 Aspire AppHost (Orchestration) - InMemory** (Empfohlen!)
   - Startet alle Services mit In-Memory-Datenbank
   - Keine PostgreSQL/Docker nötig
   - Ideal für schnelle Entwicklung

2. **Catalog Service (Debug) - InMemory**
   - Nur Catalog Service mit In-Memory
   - Für isolierte Service-Tests

### Compound Launch-Konfigurationen (für mehrere Services gleichzeitig)

3. **Full Stack (Aspire + Frontend) - InMemory 🚀** (Beliebt!)
   - AppHost + Customer Frontend
   - Beides startet mit InMemory-DB

4. **Full Stack (Aspire + Admin Frontend) - InMemory 🚀** (Beliebt!)
   - AppHost + Admin Dashboard
   - Beides startet mit InMemory-DB

5. **Full Stack (All Services + Both Frontends) - InMemory 🚀**
   - Komplett Stack mit beiden Frontends
   - Alles mit InMemory-DB

## 📋 Verfügbare Konfigurationen

### Backend Services (InMemory)
- `🚀 Aspire AppHost (Orchestration) - InMemory` - AppHost mit InMemory
- `Catalog Service (Debug) - InMemory` - Nur Catalog Service

### Backend Services (PostgreSQL - Original)
- `🚀 Aspire AppHost (Orchestration)` - AppHost mit PostgreSQL
- `Catalog Service (Debug)` - Catalog mit PostgreSQL
- `AppHost (Debug) - Legacy` - Legacy-Konfiguration

### Frontend Services
- `🎨 Frontend (Port 5173)` - Customer App
- `👨‍💼 Admin Frontend (Port 5174)` - Admin Dashboard
- `Frontend Tests (Vitest)` - Test-Runner
- `E2E Tests (Playwright)` - End-to-End Tests

## ⚙️ Umgebungsvariablen

### Automatisch gesetzt für InMemory-Konfigurationen:
```json
{
  "ASPNETCORE_ENVIRONMENT": "Development",
  "Database__Provider": "inmemory"
}
```

### Was passiert mit InMemory:
- ✅ Datenbank wird im RAM erstellt
- ✅ Keine PostgreSQL erforderlich
- ✅ Startup ist schneller (~2-3 Sekunden)
- ✅ Jeder Neustart = saubere Datenbank
- ⚠️ Daten gehen verloren beim Neustart

## 🎯 Empfohlener Workflow

### Schnelle Entwicklung (Ohne Datenbank)
```
1. Öffne Debug-Panel (Strg+Shift+D)
2. Wähle: "Full Stack (Aspire + Frontend) - InMemory 🚀"
3. Drücke F5
4. Frontend öffnet sich auf http://localhost:5173
5. Fertig! 🎉
```

### Mit PostgreSQL (Für fortgeschrittene)
```
1. PostgreSQL/Docker starten
2. Wähle: "Full Stack (Aspire + Frontend)"
3. Drücke F5
```

### Nur Backend testen
```
1. Wähle: "🚀 Aspire AppHost (Orchestration) - InMemory"
2. Drücke F5
3. Services starten auf http://localhost:15500
```

## 🔧 Services und ihre Ports

| Service | Port | Typ |
|---------|------|-----|
| AppHost | 15500 | API Gateway |
| Auth Service | 9002 | Service |
| Tenant Service | 9003 | Service |
| Localization Service | 9004 | Service |
| Catalog Service | 9001 | Service (wenn separat) |
| Frontend | 5173 | Web |
| Admin Frontend | 5174 | Web |

## 📝 Konfigurationsdateien

### Neue Dateien hinzugefügt:
- `backend/services/LocalizationService/appsettings.Development.json` - InMemory-Config
- `backend/services/auth-service/appsettings.Development.json` - Development-Logging
- `backend/services/tenant-service/appsettings.Development.json` - Development-Logging

### Umgebungsvariablen-Überschreibung:
```
Database__Provider=inmemory    # InMemory aktivieren
Database__Provider=PostgreSQL  # PostgreSQL aktivieren (Standard)
```

## 💡 Tipps & Tricks

### Problem: "Database provider not recognized"
```
→ Stelle sicher, dass die richtige appsettings.Development.json vorhanden ist
→ Überprüfe die Umgebungsvariablen
```

### Problem: "Port is already in use"
```
→ Laufen noch Services vom letzten Start?
→ netstat -tlnp | grep :15500  (Linux/Mac)
→ Oder einfach VS Code neustarten
```

### Problem: "InMemory Database not persisting"
```
→ Das ist normal! InMemory-Datenbank ist flüchtig
→ Beim Neustart gehen alle Daten verloren
→ Das ist für Entwicklung gewünscht
```

### Debug-Tipps:
```csharp
// In Program.cs sieht man welcher Provider verwendet wird:
// "Database provider: InMemory" oder "Database provider: PostgreSQL"

// Im Debug Output suchen nach:
// "🚀 B2X Application Host - Starting"
```

## 🔄 Switch zwischen InMemory und PostgreSQL

### Option 1: Über VS Code Launch-Konfiguration
```
Debug-Panel → Dropdown → Gewünschte Konfiguration wählen → F5
```

### Option 2: Über Umgebungsvariable
```bash
# InMemory
export Database__Provider=inmemory
dotnet run

# PostgreSQL
export Database__Provider=PostgreSQL
dotnet run
```

### Option 3: appsettings.json bearbeiten
```json
// In appsettings.Development.json
"Database": {
  "Provider": "inmemory"  // oder "PostgreSQL"
}
```

## 📚 Weitere Ressourcen

- [VS Code Debug Documentation](https://code.visualstudio.com/docs/editor/debugging)
- [Entity Framework InMemory](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/)
- [.NET Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration)

---

**Letzte Aktualisierung:** 26. Dezember 2025

*Viel Spaß beim Entwickeln mit B2X! 🚀*
