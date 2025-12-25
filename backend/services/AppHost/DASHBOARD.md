# B2Connect Backend Dashboard

Ein AdminLTE-basiertes Dashboard zur Überwachung aller B2Connect Microservices.

## Verfügbarkeit

- **URL**: http://localhost:9000
- **Port**: 9000

## Funktionen

✨ **Echtzeit Service-Überwachung**
- Überwachung der Verfügbarkeit aller Microservices
- Health-Check Status für jeden Service
- Auto-Refresh alle 10 Sekunden

📊 **Service-Übersicht**
- Anzahl der Microservices
- Anzahl gesunder Services
- Anzahl fehlerhafter Services
- Detaillierte Service-Informationen (Name, Port, Status)

🎨 **AdminLTE Theme**
- Modernes und responsives Design
- Schnelle und intuitive Bedienung
- Mobile-freundliche Oberfläche

## Services

Das Dashboard überwacht automatisch:

| Service | Port | URL |
|---------|------|-----|
| Auth Service | 5001 | http://localhost:5001 |
| Tenant Service | 5002 | http://localhost:5002 |
| API Gateway | 5000 | http://localhost:5000 |

## Funktionalität

### Dashboard Widgets
- **Info Boxen**: Zeigen Statistiken der Services
- **Service Cards**: Visuelle Darstellung des Status jeder Service
- **Detail Tabelle**: Ausführliche Informationen zu jedem Service
- **Action Buttons**: Schneller Zugriff auf Services und Health-Checks

### Health Checks
Das Dashboard führt automatisch Health-Checks durch:
- Jede Service kann unter `/health` abgerufen werden
- Status wird als Badge angezeigt (Healthy/Unhealthy/Unavailable)
- Die Seite wird alle 10 Sekunden aktualisiert

## Architektur

```
AppHost (Port 9000)
├── Dashboard Controller
│   └── Index Action (Service Status Prüfung)
├── API Endpoints
│   └── /api/health (JSON Health Status)
└── Views
    └── Dashboard
        └── Index.cshtml (AdminLTE UI)
```

## API

### GET /api/health
Gibt den Status aller Services im JSON-Format zurück.

**Response:**
```json
[
  {
    "name": "Auth Service",
    "status": "healthy",
    "statusCode": 200
  },
  {
    "name": "Tenant Service",
    "status": "unavailable",
    "statusCode": 0
  },
  {
    "name": "API Gateway",
    "status": "unhealthy",
    "statusCode": 500
  }
]
```

## Starten

```bash
cd /Users/holger/Documents/Projekte/B2Connect/backend/services/AppHost
dotnet run --urls "http://localhost:9000"
```

## Abhängigkeiten

- .NET 10.0+
- Serilog.AspNetCore für Logging
- AdminLTE 3.2.0 (CDN)
- Font Awesome 6.4.0 (CDN)
- Bootstrap 4.6.2 (CDN)
- jQuery 3.6.0 (CDN)

## Hinweise

- Das Dashboard braucht einen laufenden Backend-Service-Stack
- Es wird angenommen, dass die Services auf den Standard-Ports (5000, 5001, 5002) laufen
- Die Ports können in `appsettings.json` konfiguriert werden

---

**Erstellt**: 25. Dezember 2025
**Status**: Production-Ready
