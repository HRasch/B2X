# B2X Backend Dashboard

Ein AdminLTE-basiertes Dashboard zur Überwachung aller B2X Microservices.

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
| Localization Service | 5003 | http://localhost:5003 |
| API Gateway | 5000 | http://localhost:5000 |

## Funktionalität

### Dashboard Widgets
- **Info Boxen**: Zeigen Statistiken der Services
- **Service Cards**: Visuelle Darstellung des Status jeder Service
- **Detail Tabelle**: Ausführliche Informationen zu jedem Service
- **Action Buttons**: Schneller Zugriff auf Services und Health-Checks
- **Feature-Übersicht**: Status aller integrierten Features (Localization, Entity Localization, etc.)

### Health Checks
Das Dashboard führt automatisch Health-Checks durch:
- Jede Service kann unter `/health` abgerufen werden
- Status wird als Badge angezeigt (Healthy/Unhealthy/Unavailable)
- Die Seite wird alle 10 Sekunden aktualisiert

### Entity Localization Support
- **Zentrale Übersetzungen**: `/api/localization/{category}/{key}` - für UI Strings
- **Entity-basierte Übersetzungen**: `/api/entity-localization/{entityId}/{propertyName}` - für Entitäts-Inhalte
- **JSON-Speicherung**: Übersetzungen werden direkt in Entitäts-Datensätzen als JSON gespeichert
- **Fluent API**: Einfache, typsichere API für Entwickler

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
  },Localization Service",
    "status": "healthy",
    "statusCode": 200
  },
  {
    "name": "API Gateway",
    "status": "unhealthy",
    "statusCode": 500
  }
]
```

### Localization API Endpoints

#### Zentrale Übersetzungen
- `GET /api/localization/{category}/{key}?language=de` - Einzelne Übersetzung abrufen
- `GET /api/localization/category/{category}?language=de` - Gesamte Kategorie abrufen
- `GET /api/localization/languages` - Unterstützte Sprachen auflisten
- `POST /api/localization/{category}/{key}` - Übersetzungen aktualisieren (Admin only)

#### Entity-basierte Übersetzungen
- `GET /api/entity-l

## Features

### ✅ Zentrale Localization Service
- Übersetzungen für UI-Strings (Buttons, Labels, Error Messages)
- Mehrsprachige Unterstützung (8 Sprachen: en, de, fr, es, it, pt, nl, pl)
- In-Memory Caching für Performanz
- Tenant-isoliert
- RESTful API Endpoints

### ✅ Entity Localization System  
- JSON-basierte Übersetzungen direkt in Entitäten
- LocalizedContent Klasse mit Fluent API
- 15+ Extension Methods für Entities
- JSON-Utility für Transformationen
- 5 vordefinierte Entities (Product, ContentPage, MenuItem, FaqEntry, Feature)
- 30+ Unit Tests
- PostgreSQL JSONB Support

### ✅ Dashboard & Monitoring
- Echtzeit Service-Überwachung
- Health-Check Status
- Feature-Übersicht
- Auto-Refresh alle 10 Sekundenocalization/{entityId}/{propertyName}` - Alle Übersetzungen für Property
- `GET /api/entity-localization/{entityId}/{propertyName}/{language}` - Sprachspezifische Übersetzung
- `POST /api/entity-localization/{entityId}/{propertyName}` - Übersetzung setzen
- `PUT /api/entity-localization/{entityId}/{propertyName}` - Alle Übersetzungen aktualisieren (Batch)
- `GET /api/entity-localization/{entityId}` - Alle lokalisierten Properties einer Entity
- `POST /api/entity-localization/{entityId}/validate` - Validiert erforderliche Sprachen,
  {
    "name": "API Gateway",
    "status": "unhealthy",
    "statusCode": 500
  }
]
```

## Starten

```bash
cd /Users/holger/Documents/Projekte/B2X/backend/services/AppHost
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
