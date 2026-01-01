# 🧪 B2Connect TestMode

Der TestMode ist ein leistungsstarkes Debugging- und Auto-Fix-System für die B2Connect Admin-Anwendung. Er überwacht alle Browser-Aktionen in Echtzeit und behebt automatisch erkannte Fehler.

## 🚀 Aktivierung

### Automatische Aktivierung
Der TestMode wird automatisch aktiviert wenn:
- Die Anwendung im Development-Modus läuft (`import.meta.env.DEV`)
- Die URL den Parameter `?testmode` enthält
- `localStorage.testModeEnabled = 'true'` gesetzt ist

### Manuelle Aktivierung
```javascript
import { getTestMode } from '@/utils/testMode'

const testMode = getTestMode()
testMode?.enable()
```

### Debug Panel
- **Tastenkombination**: `Ctrl + Shift + T` um das Debug-Panel ein-/auszublenden
- **Position**: Rechts unten im Browser
- **Features**:
  - Live Action-Monitoring
  - Statistiken und Erfolgsraten
  - Auto-Fix Aktivierung/Deaktivierung
  - Log-Export Funktion

## 🔧 Auto-Fix Regeln

Der TestMode erkennt und behebt automatisch folgende Probleme:

### 1. Navigation-Fehler (weiße Seiten)
- **Erkennung**: Fehlgeschlagene Navigation-Aktionen
- **Fix**: Router-Reset und Seiten-Reload
- **Trigger**: Back-Button Navigation, direkte URL-Änderungen

### 2. Authentifizierungsfehler (401/403)
- **Erkennung**: HTTP 401/403 Fehler in API-Calls
- **Fix**: Automatischer Redirect zu Login-Seite, Token-Cleanup
- **Trigger**: Abgelaufene Sessions, ungültige Tokens

### 3. API-Timeouts
- **Erkennung**: API-Calls länger als 10 Sekunden
- **Fix**: Retry mit Cache-Bypass
- **Trigger**: Langsame Netzwerkverbindungen, Server-Timeouts

### 4. Router-Link Probleme
- **Erkennung**: Fehlgeschlagene Router-Link Klicks
- **Fix**: Component-Key Reset für Router-View Re-render
- **Trigger**: Vue Router State-Desync

### 5. Form-Validierung Fehler
- **Erkennung**: Fehlgeschlagene Form-Submits
- **Fix**: Automatischer Fokus auf invalide Felder
- **Trigger**: Erforderliche Felder nicht ausgefüllt

### 6. Netzwerkfehler
- **Erkennung**: NetworkError Exceptions
- **Fix**: Connectivity-Check und Retry
- **Trigger**: Verlorene Internetverbindung

### 7. JavaScript Null-Reference Fehler
- **Erkennung**: "Cannot read properties of null" Fehler
- **Fix**: Force Component Re-render
- **Trigger**: Vue.js Component State Issues

## 📊 Monitoring Features

### Echtzeit-Überwachung
- **Click Events**: Alle Klicks werden mit Selektor und Erfolg verfolgt
- **Navigation**: Route-Änderungen und Browser-History Events
- **API Calls**: Response-Zeiten und HTTP-Status-Codes
- **Errors**: JavaScript Errors und unhandled Promises

### Performance Metrics
- **Success Rate**: Prozentsatz erfolgreicher Aktionen
- **Average API Response**: Durchschnittliche API-Antwortzeiten
- **Error Count**: Anzahl der Fehler pro Session

### Visuelle Indikatoren
- **TestMode Badge**: Roter "TEST MODE" Indikator oben rechts
- **Error Highlighting**: Fehlerhafte Elemente werden rot umrandet
- **Success Highlighting**: Erfolgreiche Aktionen werden grün markiert

## 🛠️ Konfiguration

```typescript
interface TestModeConfig {
  enabled: boolean           // TestMode aktivieren/deaktivieren
  autoFix: boolean          // Auto-Fix aktivieren/deaktivieren
  logLevel: 'error' | 'warn' | 'info' | 'debug'  // Logging Level
  visualIndicators: boolean // Visuelle Indikatoren anzeigen
  performanceMonitoring: boolean // Performance-Monitoring aktivieren
}

// Konfiguration ändern
const testMode = getTestMode()
testMode?.updateConfig({
  autoFix: false,  // Auto-Fix deaktivieren
  logLevel: 'debug' // Mehr Logging
})
```

## 📝 Logging & Debugging

### Console Logs
Alle TestMode-Aktivitäten werden in der Browser-Konsole geloggt:
```
[TestMode info] 2024-01-15T10:30:45.123Z: Action recorded: click
[TestMode info] 2024-01-15T10:30:45.456Z: Auto-Fix angewendet: Navigation-Fehler behoben durch Router-Reset
```

### Log Export
- Im Debug-Panel: "Export Log" Button
- Erstellt JSON-Datei mit allen Actions, Config und Timestamps
- Nützlich für detaillierte Fehleranalyse

## 🔒 Sicherheit

- **Development Only**: TestMode ist nur in Development-Builds verfügbar
- **No Production Data**: Keine sensiblen Daten werden geloggt
- **Local Storage Only**: Alle Daten bleiben im Browser
- **Auto Cleanup**: Actions werden nach 100 Einträgen automatisch bereinigt

## 🚀 Verwendung in Tests

```typescript
// In E2E Tests
import { getTestMode } from '@/utils/testMode'

test('should handle navigation errors', async ({ page }) => {
  // TestMode aktivieren
  await page.evaluate(() => {
    const testMode = window.getTestMode?.()
    testMode?.enable()
  })

  // Test durchführen
  await page.click('.router-link')
  await page.goBack()

  // TestMode Statistiken prüfen
  const stats = await page.evaluate(() => {
    const testMode = window.getTestMode?.()
    return {
      actions: testMode?.getActions().length,
      errors: testMode?.getActions().filter(a => !a.success).length
    }
  })

  expect(stats.errors).toBe(0) // Keine Fehler erwartet
})
```

## 🐛 Bekannte Einschränkungen

1. **Cross-Origin Requests**: API-Calls zu anderen Domains werden nicht überwacht
2. **Service Worker**: Service Worker Requests werden nicht getrackt
3. **Memory Usage**: Bei langen Sessions kann der Memory-Verbrauch ansteigen
4. **Performance Impact**: TestMode fügt minimalen Overhead hinzu (~1-2ms pro Action)

## 🔄 Roadmap

- [ ] Integration mit Playwright für automatische Test-Generierung
- [ ] Machine Learning für Fehler-Pattern-Erkennung
- [ ] Performance Profiling Integration
- [ ] Custom Fix Rules API für Entwickler
- [ ] TestMode für Production (read-only Mode)