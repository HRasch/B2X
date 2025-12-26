# B2Connect Aspire Launch Guide

## VS Code Debug Configurations

Die Anwendung wurde auf echte **.NET Aspire Orchestration** umgestellt. Alle Services werden über den zentralen AppHost verwaltet.

### 🚀 Quick Start

**1. Aspire AppHost starten (empfohlen)**
```
Debug → Run and Debug → 🚀 Aspire AppHost (Orchestration)
```

Dies startet alle Services zentral:
- PostgreSQL (automatisch)
- Redis (automatisch)
- Catalog Service (Port 9001)
- Auth Service (Port 9002)
- Tenant Service (Port 9003)
- Localization Service (Port 9004)
- Search Service (Port 9005)

**2. Frontend separat starten (optional)**
```
Debug → Run and Debug → 🎨 Frontend (Port 5173)
```

**3. Full Stack (empfohlen)**
```
Debug → Run and Debug → Full Stack (Aspire + Frontend)
```

Startet AppHost + Frontend zusammen und stoppt beide beim Beenden.

---

## Verfügbare Konfigurationen

| Konfiguration | Beschreibung |
|---|---|
| 🚀 Aspire AppHost (Orchestration) | Zentraler Application Host - alle Services |
| 🎨 Frontend (Port 5173) | Customer Frontend (Vue) |
| 👨‍💼 Admin Frontend (Port 5174) | Admin Frontend (Vue) |
| Full Stack (Aspire + Frontend) | AppHost + Customer Frontend |
| Full Stack (Aspire + Admin Frontend) | AppHost + Admin Frontend |
| Full Stack with All Services | AppHost + beide Frontends |

---

## Aspire Service Architecture

```
┌─────────────────────────────────────────────┐
│     .NET Aspire Application Host            │
│     (AppHost - Port 15500)                  │
├─────────────────────────────────────────────┤
│                                              │
│  Services (Auto-Orchestration):             │
│  ├─ Catalog Service (9001)                  │
│  ├─ Auth Service (9002)                     │
│  ├─ Tenant Service (9003)                   │
│  ├─ Localization Service (9004)             │
│  └─ Search Service (9005)                   │
│                                              │
│  Infrastructure:                            │
│  ├─ PostgreSQL (auto-managed)               │
│  └─ Redis (auto-managed)                    │
│                                              │
│  Frontend Apps:                             │
│  ├─ Frontend (Port 5173)                    │
│  └─ Admin Frontend (Port 5174)              │
└─────────────────────────────────────────────┘
```

---

## Wichtige Änderungen

✅ **AppHost.csproj** - Verwendet jetzt `Aspire.Hosting` statt reguläres WebApplication
✅ **Program.cs** - DistributedApplication API für zentrale Orchestration
✅ **launch.json** - Neue Debug-Konfigurationen für Aspire
✅ **Vite Configs** - Port-Verwaltung über Umgebungsvariablen

---

## Debugging

### AppHost Debuggen
1. `Debug → Run and Debug → 🚀 Aspire AppHost`
2. Breakpoints in `Program.cs` setzen
3. Services werden automatisch mit Debugger gestartet

### Service Debuggen (einzeln)
Services laufen über Aspire, können aber auch einzeln gestartet werden:
```bash
cd backend/services/CatalogService
dotnet run
```

### Frontend Debuggen
```
Debug → 🎨 Frontend (Port 5173)
```
Oder im Browser DevTools debuggen.

---

## Ports & Endpoints

| Service | Port | Endpoint |
|---|---|---|
| Aspire Dashboard | 15500 | http://localhost:15500 |
| Catalog Service | 9001 | http://localhost:9001 |
| Auth Service | 9002 | http://localhost:9002 |
| Tenant Service | 9003 | http://localhost:9003 |
| Localization | 9004 | http://localhost:9004 |
| Search Service | 9005 | http://localhost:9005 |
| Frontend | 5173 | http://localhost:5173 |
| Admin Frontend | 5174 | http://localhost:5174 |
| PostgreSQL | 5432 | postgres:postgres@localhost |
| Redis | 6379 | localhost:6379 |

---

## Fehlerbehebung

### "Project not found" Error
→ Services müssen gebaut sein: `dotnet build` im `/backend` Verzeichnis

### Port bereits in Verwendung
→ Alte Prozesse beenden: `pkill -f dotnet` oder Task `🛑 Stop Services` nutzen

### Service startet nicht
→ Logs prüfen in `logs/` Verzeichnis oder im Terminal der Aspire Session

### Datenbank-Fehler
→ PostgreSQL läuft automatisch, aber Migrations brauchen ggf. `dotnet ef database update`

---

## Tasks vs. Debug Configurations

**Tasks** (über `npm run` / Shell):
- `✅ Full Startup (Backend + Frontend)` - Shell-basiert
- `🚀 Backend Aspire (aspire-start.sh)` - Script-basiert

**Debug Configurations** (Debug-Modus mit Breakpoints):
- `Full Stack (Aspire + Frontend)` - Mit vollständigem Debugger
- `🚀 Aspire AppHost (Orchestration)` - Nur AppHost mit Debugger

**Empfehlung:** Debug Configurations für Entwicklung nutzen!

