# Gateway Separation - Store vs Admin

## Übersicht

Die API-Gateways wurden getrennt, um eine klare Trennung zwischen Store-Frontend und Admin-Frontend zu gewährleisten:

| Gateway | Port | Frontend | Zweck |
|---------|------|----------|-------|
| **Store Gateway** | 6000 | frontend-store (5173) | Öffentliche, read-only APIs |
| **Admin Gateway** | 6100 | frontend-admin (5174) | Geschützte CRUD APIs |

## Architektur

```
┌─────────────────┐     ┌─────────────────┐
│  frontend-store │     │  frontend-admin │
│   (Port 5173)   │     │   (Port 5174)   │
└────────┬────────┘     └────────┬────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐     ┌─────────────────┐
│  Store Gateway  │     │  Admin Gateway  │
│   (Port 6000)   │     │   (Port 6100)   │
│   - GET only    │     │   - JWT Auth    │
│   - Public      │     │   - Full CRUD   │
└────────┬────────┘     └────────┬────────┘
         │                       │
         └───────────┬───────────┘
                     ▼
         ┌───────────────────────┐
         │   Backend Services    │
         │  - Catalog (9005)     │
         │  - Layout (9006)      │
         │  - Auth (9002)        │
         │  - Localization (9004)│
         │  - Tenancy (9003)     │
         └───────────────────────┘
```

## Store Gateway (Port 6000)

### Erlaubte Endpunkte (nur GET)
- `/api/products/*` - Produktliste und Details
- `/api/categories/*` - Kategorien
- `/api/brands/*` - Marken
- `/api/pages/*` - CMS Seiten
- `/api/templates/*` - Templates
- `/api/media/*` - Medien
- `/api/layout/*` - Layout-Daten
- `/api/localization/*` - Übersetzungen

### CORS
- `http://localhost:3000`
- `http://localhost:5173`

## Admin Gateway (Port 6100)

### Erlaubte Endpunkte (JWT-geschützt)
- `/api/auth/*` - Authentifizierung
- `/api/admin/products/*` - Produktverwaltung (CRUD)
- `/api/admin/categories/*` - Kategorienverwaltung (CRUD)
- `/api/admin/brands/*` - Markenverwaltung (CRUD)
- `/api/admin/attributes/*` - Attributverwaltung
- `/api/admin/pricing/*` - Preisverwaltung
- `/api/admin/discounts/*` - Rabattverwaltung
- `/api/admin/cms/*` - CMS-Verwaltung
- `/api/admin/localization/*` - Lokalisierungsverwaltung
- `/api/admin/tenants/*` - Mandantenverwaltung
- `/api/admin/users/*` - Benutzerverwaltung
- `/api/admin/roles/*` - Rollenverwaltung

### CORS
- `http://localhost:5174`

### JWT-Konfiguration
```json
{
  "Jwt": {
    "Secret": "...",
    "Issuer": "B2X",
    "Audience": "B2X-Admin"
  }
}
```

## VS Code Tasks

Neue Tasks wurden hinzugefügt:
- `store-gateway-start` - Startet Store Gateway
- `admin-gateway-start` - Startet Admin Gateway

## Frontend-Konfiguration

### frontend-store
```typescript
// .env
VITE_API_URL=http://localhost:6000/api
```

### frontend-admin
```typescript
// .env
VITE_API_URL=http://localhost:6100/api
```

## Starten

```bash
# Via Aspire Orchestration (empfohlen)
dotnet run --project backend/services/Orchestration/B2X.Orchestration.csproj

# Oder einzeln:
dotnet run --project backend/services/Gateway.Store/B2X.Gateway.Store.csproj
dotnet run --project backend/services/Gateway.Admin/B2X.Gateway.Admin.csproj
```

## Sicherheitsvorteile

1. **Reduzierte Angriffsfläche**: Store-Gateway exponiert keine Admin-Endpunkte
2. **Klare Trennung**: Store kann nur lesen, Admin hat volle CRUD-Rechte
3. **Separate Auth**: Admin Gateway hat eigene JWT-Validierung
4. **CORS-Isolierung**: Jedes Gateway akzeptiert nur sein Frontend
