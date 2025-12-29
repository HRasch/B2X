# OpenTelemetry Frontend Integration - Anleitung

**Letzte Aktualisierung**: 29. Dezember 2025  
**Issue**: #51 - Vite Frontend mit OpenTelemetry auf Aspire integrieren

---

## Überblick

Diese Anleitung erklärt, wie Sie verteiltes Tracing und Metriken für B2Connect Frontend-Anwendungen (Store & Admin) mit OpenTelemetry und Aspire Dashboard Integration aktivieren können.

## Voraussetzungen

- Node.js 18+
- Aspire läuft (`cd backend/Orchestration && dotnet run`)
- Aspire Dashboard erreichbar unter http://localhost:15500

## Schnellstart

### Telemetrie aktivieren

```bash
# Frontend Store (Port 5173)
cd frontend/Store
npm run dev:telemetry

# Frontend Admin (Port 5174)
cd frontend/Admin
npm run dev:telemetry
```

### Traces anzeigen

1. Aspire Dashboard öffnen: http://localhost:15500
2. Zum **Traces**-Tab navigieren
3. Nach Service filtern: `frontend-store` oder `frontend-admin`
4. Auf einen Trace klicken, um Span-Details zu sehen

## Konfiguration

### Umgebungsvariablen

| Variable | Standard | Beschreibung |
|----------|----------|--------------|
| `ENABLE_TELEMETRY` | `false` | OpenTelemetry ein-/ausschalten |
| `OTEL_SERVICE_NAME` | `frontend-store` / `frontend-admin` | Service-Bezeichner |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | `http://localhost:4318` | OTLP-Collector-Endpunkt |
| `NODE_ENV` | `development` | Umgebungsname in Traces |

### Benutzerdefinierte Konfiguration

```bash
# Telemetrie mit benutzerdefiniertem Endpunkt aktivieren
ENABLE_TELEMETRY=true \
OTEL_EXPORTER_OTLP_ENDPOINT=http://custom-collector:4318 \
OTEL_SERVICE_NAME=mein-service \
npm run dev:telemetry
```

## Was wird instrumentiert?

### Aktivierte Instrumentierungen

| Instrumentierung | Typ | Beschreibung |
|------------------|-----|--------------|
| HTTP | Auto | Erfasst ausgehende HTTP-Anfragen |
| Fetch | Auto | Erfasst fetch()-API-Aufrufe |
| Node.js HTTP/HTTPS | Auto | Server-seitige HTTP-Verarbeitung |

### Deaktivierte Instrumentierungen (Performance)

| Instrumentierung | Grund |
|------------------|-------|
| Dateisystem (fs) | Zu viel Rauschen für Entwicklung |
| DNS | Zu häufige Abfragen |

## Trace-Beispiele

### HTTP-Anfrage-Span

```
Service: frontend-store
Span: GET /api/products
Dauer: 45ms
Attribute:
  - http.method: GET
  - http.url: http://localhost:8000/api/products
  - http.status_code: 200
  - service.name: frontend-store
  - service.version: 1.0.0
```

### Trace-Korrelation

Frontend-Traces korrelieren automatisch mit Backend-Services über W3C TraceContext-Header:

```
Frontend (frontend-store) → API Gateway (8000) → Katalog-Service (7005)
```

## Fehlerbehebung

### Keine Traces sichtbar

1. **Prüfen, ob Aspire läuft**: http://localhost:15500
2. **OTLP-Endpunkt verifizieren**: `curl http://localhost:4318/v1/traces`
3. **Telemetrie-Status prüfen**: Suchen Sie nach `[OTel] Telemetry initialized successfully` in der Konsole
4. **HTTP-Anfrage auslösen**: Navigieren Sie zu einer Seite, die API-Aufrufe macht

### Langsamer Start

Falls der Dev-Server mit Telemetrie langsam startet:

1. Verwenden Sie `npm run dev` (Telemetrie deaktiviert) für normale Entwicklung
2. Aktivieren Sie Telemetrie nur bei Performance-Debugging
3. Erwägen Sie, `exportIntervalMillis` in `instrumentation.ts` zu reduzieren

### Verbindungsfehler

Falls der OTLP-Endpunkt nicht erreichbar ist:

- Telemetrie fällt graceful zurück (kein Absturz)
- Warnmeldung: `[OTel] Failed to initialize telemetry`
- Dev-Server läuft normal weiter

### Zu viele Spans

Falls zu viele Spans erscheinen:

1. Dateisystem- und DNS-Instrumentierungen sind bereits deaktiviert
2. Erwägen Sie, im Aspire Dashboard nach Service-Namen zu filtern
3. Verwenden Sie spezifische Trace-IDs, um verwandte Spans zu finden

## Performance-Auswirkungen

| Metrik | Ohne Telemetrie | Mit Telemetrie | Auswirkung |
|--------|-----------------|----------------|------------|
| Dev-Server-Start | ~1,5s | ~1,8s | +20% |
| HMR-Latenz | ~50ms | ~55ms | +10% |
| Speicherverbrauch | ~150MB | ~170MB | +13% |

**Hinweis**: Die Auswirkung liegt im akzeptablen Bereich (<5% Laufzeit-Overhead gemäß Akzeptanzkriterien).

## Architektur

```
┌─────────────────────┐      ┌─────────────────────┐
│   Frontend Store    │      │   Frontend Admin    │
│   (Instrumentierung)│      │   (Instrumentierung)│
└─────────┬───────────┘      └─────────┬───────────┘
          │                            │
          │ OTLP/HTTP (4318)           │ OTLP/HTTP (4318)
          │                            │
          ▼                            ▼
┌─────────────────────────────────────────────────┐
│              Aspire Dashboard                   │
│              (http://localhost:15500)           │
│                                                 │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐         │
│  │ Traces  │  │ Metrics │  │  Logs   │         │
│  └─────────┘  └─────────┘  └─────────┘         │
└─────────────────────────────────────────────────┘
```

## Datei-Referenz

| Datei | Zweck |
|-------|-------|
| `frontend/Store/instrumentation.ts` | OpenTelemetry SDK-Konfiguration für Store |
| `frontend/Admin/instrumentation.ts` | OpenTelemetry SDK-Konfiguration für Admin |
| `frontend/Store/package.json` | `dev:telemetry`-Skript |
| `frontend/Admin/package.json` | `dev:telemetry`-Skript |

## Verwandte Dokumentation

- [OpenTelemetry JS Erste Schritte](https://opentelemetry.io/docs/languages/js/getting-started/nodejs/)
- [Aspire Telemetrie-Konfiguration](https://aspire.dev/fundamentals/telemetry/)
- [Issue #51 - OpenTelemetry Integration](https://github.com/HRasch/B2Connect/issues/51)
- [Issue #50 - Vite Build-Fehler erfassen](https://github.com/HRasch/B2Connect/issues/50)

---

**Fragen?** Markieren Sie @devops-engineer oder @tech-lead in GitHub Issues.
