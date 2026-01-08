---
docid: UNKNOWN-123
title: AdminErrorLogging
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
title: Admin Frontend Error Logging Design
owner: @SARAH
---

Ziel
----
Fehler und Performance-Anomalien aus dem Admin/Management-Frontend zuverlässig erfassen, für Analyse verfügbar machen (Admin UI) und sicher an Backend/Observability weiterleiten.

Anforderungen
------------
- Erfassen von JS-Fehlern, unhandled promise rejections, Vue runtime errors und optional user-reproduced traces
- Kontext: Tenant, UserId, Route, Browser, App-Version, optional form state (ohne PII)
- Aggregation & Suche: Admin UI bietet Log-Viewer mit Filter (time range, level, tenant, user, route)
- Datenschutz: PII-Redaction und opt-out per tenant setting
- Performance: Batch-sende-Modus, rate-limiting, backoff
- Reliability: retry + local queue when offline

Architektur
------------
- Client-side: `errorLogger` service in Admin app
  - captures errors, enriches with RequestContext, queues and batches sends to backend endpoint `/api/admin/client-logs`
  - exposes `installVueHandler()` and `installGlobalHandlers()` utilities
- Server-side: dedicated ingestion endpoint validates, redacts PII, persists to DB or forwards to central logging (Elasticsearch, Seq, Application Insights)
- Admin UI: `LogsView` to list, filter and group logs; uses backend search API supporting pagination and aggregation
- Observability: metrics for client logs received, ingestion errors, queue sizes

API Contract
------------
- POST `/api/admin/client-logs`
  - Body: { tenantId, userId, level, message, stack, route, component, meta, timestamp }
  - Auth: Admin role required to submit from Admin app; token required for server-to-server ingestion

Security & Privacy
------------------
- Redact fields marked as PII before storage (emails, credit-card-like patterns)
- Limit stored payload length, strip large form values by default
- Store consent/opt-out per tenant and drop or anonymize logs accordingly

Operational
-----------
- Provide a `replay` mode for frontend: capture and attach console logs for reproduction (opt-in)
- Provide a dashboard with ingestion rate, errors, and slow client-side traces
- Add retention policy (ILM) if storing in Elasticsearch

Minimal Implementation Plan
---------------------------
1. Add `errorLogger` client service in `frontend/Admin/src/services/errorLogger.ts`.
2. Add backend endpoint scaffold `/api/admin/client-logs` (separate task).
3. Add simple `LogsView.vue` in Admin to query logs API and display entries.
4. Add unit tests for redaction and batching logic.
