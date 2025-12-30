# Admin Frontend - Login E2E Tests

## Übersicht

Diese E2E-Tests validieren den kompletten Login-Flow im Admin-Frontend, einschließlich:
- Tenant-Auflösung (X-Tenant-ID Header)
- JWT-Token-Authentifizierung
- Sicherheitsvalidierung (Tenant Spoofing Prevention)
- Fehlerbehandlung (Network Errors, 401 Unauthorized)

## Test-Kategorien

### 1. UI/UX Tests

#### `should display login form with all required elements`
- **Ziel**: Verifiziert, dass alle Login-Formularelemente vorhanden sind
- **Testet**:
  - Email-Eingabefeld
  - Passwort-Eingabefeld
  - "Sign In" Button
  - "Remember Me" Checkbox

#### `should display demo credentials in footer`
- **Ziel**: Verifiziert Demo-Credential-Anzeige für Entwickler
- **Testet**: Footer-Bereich mit Demo-Account-Informationen

---

### 2. Tenant Resolution Tests

#### `should initialize default tenant ID in localStorage on page load`
- **Ziel**: Verifiziert automatische Initialisierung des Default Tenant
- **Testet**:
  - localStorage wird mit `00000000-0000-0000-0000-000000000001` initialisiert
  - Tenant ID ist sofort verfügbar
- **Wichtig**: Dies ist das Development-Fallback gemäß Security-Specs (CVE-2025-002)

#### `should include X-Tenant-ID header in all authenticated API requests`
- **Ziel**: Verifiziert Multi-Tenant-Isolation auf API-Ebene
- **Testet**:
  - Alle API-Requests enthalten `X-Tenant-ID` Header
  - Tenant ID entspricht dem gespeicherten Wert
- **Sicherheit**: Verhindert Cross-Tenant-Datenlecks (VUL-2025-005)

---

### 3. Authentication Tests

#### `should show error message with invalid credentials`
- **Ziel**: Verifiziert Fehlerbehandlung bei falschen Credentials
- **Testet**:
  - Login wird abgelehnt
  - User bleibt auf Login-Page
- **Sicherheit**: Keine detaillierten Fehlerinformationen (VUL-2025-004)

#### `should successfully login with valid credentials and JWT validation`
- **Ziel**: Verifiziert erfolgreichen Login-Flow
- **Testet**:
  - Login-Request enthält `X-Tenant-ID` Header
  - Redirect zu Dashboard nach erfolgreicher Authentifizierung
  - JWT-Token wird validiert

#### `should store JWT token and tenant ID in localStorage after successful login`
- **Ziel**: Verifiziert Token-Speicherung
- **Testet**:
  - `authToken` wird in localStorage gespeichert
  - `refreshToken` wird in localStorage gespeichert
  - `tenantId` bleibt konsistent (Default Tenant)

#### `should include Authorization header with JWT token in authenticated requests`
- **Ziel**: Verifiziert JWT-Propagierung zu Backend
- **Testet**:
  - Alle API-Requests enthalten `Authorization: Bearer <token>` Header
  - Token entspricht dem gespeicherten Wert

---

### 4. Security Tests

#### `should prevent tenant ID spoofing by validating JWT claim`
- **Ziel**: Verifiziert Schutz gegen Tenant ID Manipulation (CVE-2025-001)
- **Testet**:
  - User kann `tenantId` in localStorage nicht manipulieren
  - Backend validiert Tenant ID gegen JWT-Claim
  - Fehlschlag führt zu 403 Forbidden
- **Kritisch**: Dies ist die wichtigste Security-Maßnahme im Multi-Tenant-System

#### `should redirect to login on 401 unauthorized`
- **Ziel**: Verifiziert Session-Timeout-Handling
- **Testet**:
  - 401-Response löscht Tokens aus localStorage
  - Automatischer Redirect zu Login-Page
- **Sicherheit**: Verhindert unautorisierten Zugriff

#### `should clear sensitive data on logout`
- **Ziel**: Verifiziert Bereinigung sensibler Daten
- **Testet**:
  - `authToken` wird bei Logout gelöscht
  - `refreshToken` wird bei Logout gelöscht
- **Sicherheit**: Verhindert Token-Diebstahl nach Logout

---

### 5. Error Handling Tests

#### `should handle network errors gracefully during login`
- **Ziel**: Verifiziert Robustheit bei Netzwerkfehlern
- **Testet**:
  - Login-Request-Fehler wird abgefangen
  - User bleibt auf Login-Page
  - Keine unbehandelte Exception

#### `should validate email format before submission`
- **Ziel**: Verifiziert Client-seitige Validierung
- **Testet**:
  - HTML5-Validierung verhindert ungültige Email-Formate
  - Form wird nicht submitted bei Validierungsfehlern

---

## Test-Ausführung

### Voraussetzungen

```bash
# 1. Backend muss laufen (Aspire Orchestration)
cd /Users/holger/Documents/Projekte/B2Connect/AppHost
dotnet run

# 2. Admin Frontend muss laufen
cd /Users/holger/Documents/Projekte/B2Connect/frontend/Admin
npm install
npm run dev
```

### Alle Tests ausführen

```bash
cd /Users/holger/Documents/Projekte/B2Connect/frontend/Admin

# Alle E2E-Tests
npm run test:e2e

# Nur Login-Tests
npx playwright test auth.spec.ts

# Mit UI
npx playwright test auth.spec.ts --ui

# Im Debug-Modus
npx playwright test auth.spec.ts --debug
```

### Einzelne Tests ausführen

```bash
# Einzelner Test
npx playwright test auth.spec.ts -g "should prevent tenant ID spoofing"

# Mit Headed Browser
npx playwright test auth.spec.ts -g "should successfully login" --headed

# Mit Trace-Viewer
npx playwright test auth.spec.ts --trace on
```

---

## Test-Konfiguration

Siehe [playwright.config.ts](../../playwright.config.ts):
- **baseURL**: `http://localhost:5174`
- **Timeout**: 30 Sekunden
- **Retries**: 2 (nur in CI)
- **Workers**: 4 (lokal), 1 (CI)
- **Browser**: Chromium (Desktop Chrome)

---

## Sicherheits-Validierung

Diese Tests decken folgende Security Requirements ab:

| CVE/VUL ID | Beschreibung | Test |
|------------|--------------|------|
| **CVE-2025-001** | JWT Tenant Validation | `should prevent tenant ID spoofing` |
| **CVE-2025-002** | Development Fallback Security | `should initialize default tenant ID` |
| **VUL-2025-004** | Generic Error Messages | `should show error message with invalid credentials` |
| **VUL-2025-005** | Tenant Isolation via Headers | `should include X-Tenant-ID header` |
| **VUL-2025-007** | Tenant Ownership Validation | `should prevent tenant ID spoofing` |

---

## Expected Test Results

**Alle 13 Tests sollten bestehen:**

```
✓ should display login form with all required elements
✓ should initialize default tenant ID in localStorage on page load
✓ should show error message with invalid credentials
✓ should successfully login with valid credentials and JWT validation
✓ should store JWT token and tenant ID in localStorage after successful login
✓ should include X-Tenant-ID header in all authenticated API requests
✓ should include Authorization header with JWT token in authenticated requests
✓ should redirect to login on 401 unauthorized
✓ should prevent tenant ID spoofing by validating JWT claim
✓ should handle network errors gracefully during login
✓ should validate email format before submission
✓ should clear sensitive data on logout
✓ should display demo credentials in footer

13 passed (45s)
```

---

## Troubleshooting

### Test schlägt fehl: "Timeout waiting for dashboard"

**Problem**: Login-Request dauert zu lange oder schlägt fehl

**Lösung**:
1. Prüfen ob Backend läuft: `curl http://localhost:7002/health`
2. Prüfen ob Admin-Gateway läuft: `curl http://localhost:8080/health`
3. Logs überprüfen: `docker logs <container-id>`

### Test schlägt fehl: "X-Tenant-ID header not found"

**Problem**: Tenant ID wird nicht in API-Requests gesendet

**Lösung**:
1. Prüfen ob `tenantId` in localStorage gesetzt ist
2. Prüfen `src/services/client.ts` Request Interceptor
3. Browser DevTools Network Tab überprüfen

### Test schlägt fehl: "Tenant ID spoofing not prevented"

**Problem**: Backend validiert Tenant ID nicht gegen JWT

**Lösung**:
1. Prüfen Backend `TenantContextMiddleware.cs`
2. Verifizieren JWT enthält `tenant_id` Claim
3. Environment muss Production sein (nicht Development mit Fallback)

---

## Nächste Schritte

### Erweiterungen
1. **Multi-Tenant Login Test**: Test mit verschiedenen Tenant IDs
2. **Session Timeout Test**: Test automatischer Token-Refresh
3. **MFA Test**: Test 2-Faktor-Authentifizierung (wenn implementiert)
4. **Accessibility Test**: WCAG 2.1 Compliance für Login-Form

### Performance Tests
1. **Load Test**: 100 simultane Logins (k6 oder Artillery)
2. **Response Time Test**: Login < 500ms
3. **Token Validation Test**: JWT-Validierung < 50ms

---

## Referenzen

- [SECURITY_FIXES_IMPLEMENTATION.md](../../../SECURITY_FIXES_IMPLEMENTATION.md)
- [SECURITY_QUICK_REFERENCE.md](../../../SECURITY_QUICK_REFERENCE.md)
- [TENANT_RESOLUTION_GUIDE.md](../../../TENANT_RESOLUTION_GUIDE.md)
- [APPLICATION_SPECIFICATIONS.md](../../../docs/APPLICATION_SPECIFICATIONS.md)

---

**Letzte Aktualisierung**: 28. Dezember 2025  
**Autor**: AI Coding Agent  
**Test Coverage**: 13 Tests (100% Login Flow)
