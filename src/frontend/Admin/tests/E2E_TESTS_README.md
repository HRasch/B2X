# E2E Tests für Admin Frontend

Umfassende End-to-End Tests für das B2X Admin Frontend mit Playwright.

## Test-Übersicht

### 1. **auth.spec.ts** - Authentifizierung

- Login mit gültigen/ungültigen Credentials
- Token Management
- Session Persistence
- Dashboard Navigation nach Login

### 2. **cms.spec.ts** - CMS/Pages Management

- Pages Liste laden
- API Response Validierung (`/api/layout/pages`)
- Navigation zu Page Details
- Suchfunktionalität
- API-Routing Validierung

### 3. **shop.spec.ts** - Catalog/Products Management

- Products Liste laden
- Categories Liste laden
- API Response Validierung (`/api/v1/products`, `/api/v1/categories`)
- Navigation zu Product Details
- Filterung und Suche
- API-Routing Validierung

### 4. **integration.spec.ts** - API Gateway Integration

- API Gateway Routing (`/api/v1/*` → CatalogService)
- API Gateway Routing (`/api/layout/*` → LayoutService)
- Paginated Responses
- Fehlerbehandlung
- Dark Mode Support
- 404 Error Prevention auf allen Admin Pages

### 5. **performance.spec.ts** - Performance & Zuverlässigkeit

- Seitenladezeit (< 10 Sekunden)
- API Response Zeit (< 5 Sekunden)
- Rapid Navigation Handling
- Network Slowdown Recovery
- Session Persistence
- Large Dataset UI Responsiveness
- API Contract Validation

## Ausführung

### Alle Tests

```bash
npm run e2e
```

### Tests mit UI (interaktiv)

```bash
npm run e2e:ui
```

### Tests im Debug-Modus

```bash
npm run e2e:debug
```

### Spezifische Test Suite

```bash
npx playwright test tests/e2e/cms.spec.ts
npx playwright test tests/e2e/shop.spec.ts
npx playwright test tests/e2e/integration.spec.ts
```

### Tests mit Report

```bash
npx playwright test
npx playwright show-report
```

## Voraussetzungen

1. **Backend läuft**
   - API Gateway auf Port 6000
   - CatalogService auf Port 9001
   - LayoutService auf Port 9005
   - Auth Service auf Port 9002

2. **Frontend läuft**
   - Admin Frontend auf Port 5174: `npm run dev`
   - Oder Playwright startet es automatisch (siehe playwright.config.ts)

3. **Test-Credentials**
   - Email: `admin@example.com`
   - Passwort: `password`

## Test-Konfiguration

Siehe `playwright.config.ts`:

- Base URL: `http://localhost:5174`
- Screenshots: Bei Fehlern
- Videos: Bei Fehlern
- Traces: Beim ersten Versuch
- Reporter: HTML Report
- Timeouts: 10 Sekunden (default)

## Key Test Scenarios

### ✅ API Gateway Routing

```typescript
/api/v1/products     → CatalogService:9001
/api/layout/pages    → LayoutService:9005
/api/v1/categories   → CatalogService:9001
```

### ✅ Frontend Routes

- `/catalog/products` → Produkte Liste
- `/catalog/categories` → Kategorien Liste
- `/catalog/brands` → Marken Liste
- `/cms/pages` → Seiten Liste

### ✅ Error Handling

- 404 Prevention auf allen Admin Pages
- Network Failure Recovery
- Graceful Error Display

### ✅ Performance Targets

- Page Load: < 10s
- API Response: < 5s
- No UI Freezing

## Troubleshooting

### Tests schlagen fehl mit "Unable to connect"

- Stelle sicher, dass Backend läuft: `dotnet run` im AppHost
- Stelle sicher, dass Frontend läuft: `npm run dev`
- Prüfe Port-Konfigurationen in `playwright.config.ts`

### "404 Not Found" Fehler

- Prüfe API Gateway Routing in `appsettings.json`
- Verifiziere Services in AppHost starten
- Check CatalogService (9001) und LayoutService (9005)

### Tests sind langsam

- Erhöhe Timeouts in `playwright.config.ts`
- Prüfe Network Performance
- Überprüfe Backend Performance

### Login schlägt fehl

- Verifiziere Auth Service läuft (Port 9002)
- Prüfe Test-Credentials: `admin@example.com` / `password`
- Prüfe localStorage für authToken

## CI/CD Integration

Für automatisierte Tests in CI:

```bash
npm run e2e  # Läuft alle Tests
# oder
CI=true npm run e2e  # Mit CI-Konfiguration
```

Playwright konfiguriert sich automatisch für CI (see `playwright.config.ts`):

- `forbidOnly: !!process.env.CI`
- `retries: process.env.CI ? 2 : 0`
- `workers: process.env.CI ? 1 : undefined`

## Test-Abdeckung

Aktuell abgedeckt:

- ✅ Authentication Flow
- ✅ CMS Management
- ✅ Catalog Management
- ✅ API Gateway Integration
- ✅ Performance Metrics
- ✅ Error Handling
- ✅ Dark Mode Styling

Zukünftig:

- ⏳ Job Queue Management
- ⏳ User Management
- ⏳ Store Frontend Tests
- ⏳ Search Functionality
- ⏳ Localization/i18n

## Reporting

Nach Testlauf wird ein HTML Report erstellt:

```bash
npx playwright show-report
```

Report enthält:

- Test Results (✅/❌)
- Screenshots (bei Fehlern)
- Videos (bei Fehlern)
- Execution Traces
- Timing Information

## Tipps

1. **Lokal entwickeln**: Nutze `npm run e2e:ui` für interaktives Testing
2. **Debugging**: Nutze `npm run e2e:debug` für Breakpoints
3. **Spezifische Tests**: Nutze `.only()` zum Fokussieren auf ein Test
4. **Test-Isolation**: Nutze `beforeEach()` für Setup/Teardown
5. **Assertions**: Nutze `expect()` für aussagekräftige Fehlermeldungen

## Links

- [Playwright Documentation](https://playwright.dev/)
- [Playwright API](https://playwright.dev/docs/api/class-page)
- [Test Configuration](playwright.config.ts)
