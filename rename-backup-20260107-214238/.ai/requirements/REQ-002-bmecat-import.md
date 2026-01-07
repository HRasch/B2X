---
docid: REQ-002
title: BMEcat Katalog Import
owner: @ProductOwner
status: Draft
created: 2026-01-03
---

# REQ-002: BMEcat Katalog Import

## User Story
Als Händler möchte ich BMEcat-Kataloge von Handelsverbänden importieren können, damit ich Produktstammdaten effizient synchronisiere. Die importierten Daten müssen identifizierbar bleiben, um Kataloge bei Bedarf wieder entfernen zu können.

## Business Context
Handelsverbände stellen Produktkataloge häufig im BMEcat-Format zur Verfügung. Dies ermöglicht eine standardisierte Integration von Lieferanten-Daten in das PIM-System.

## Akzeptanzkriterien
- [ ] BMEcat 1.2/2005 Standard wird unterstützt
- [ ] Upload via API (POST /api/catalogs/import/bmecat) möglich
- [ ] XML-Validierung mit aussagekräftigen Fehlermeldungen
- [ ] Katalog-Identifizierung durch SupplierID + CatalogID + Timestamp
- [ ] Progress-Tracking für große Kataloge (>10k Produkte)
- [ ] Löschung ganzer Kataloge möglich (DELETE /api/catalogs/{catalogId})
- [ ] Tenant-Isolation: Kataloge sind tenant-spezifisch
- [ ] Audit-Log für Import- und Lösch-Operationen

## Nicht-Funktionale Anforderungen
- Performance: Import von 100k Produkten < 5 Minuten
- Zuverlässigkeit: 99.9% Erfolgsrate bei validen Katalogen
- Sicherheit: Input-Validation, keine Code-Injection möglich
- Compliance: GDPR-konforme Löschung

## Abhängigkeiten
- Catalog-Domain Service
- File-Upload-Infrastruktur
- Tenant-Isolation Framework
- Audit-Logging System

## Risiken
- Komplexität der BMEcat-XML-Struktur
- Performance bei großen Katalogen
- Schema-Validierung Overhead
- Datenkonsistenz bei Updates

## Priorität
P1 - Wichtiges B2B-Feature für Katalog-Management

## Schätzung
Story Points: 13 (Backend: 8, Frontend: 3, Tests: 2)

---
**Agent**: @ProductOwner  
**Status**: Initial Requirements - Ready for Multi-Agent Analysis

---

## Multi-Agent Analysis

### @Backend Analysis
**Machbarkeit**: Hoch - BMEcat ist standardisiertes XML-Format, gute .NET-Unterstützung

**Betroffene Services**:
- Catalog.API: Neuer Import-Endpoint
- Catalog.Domain: Neue Entities für Katalog-Metadaten
- Catalog.Application: Import-Handler mit Wolverine

**API Design**:
```csharp
POST /api/v1/catalogs/import/bmecat
- Content-Type: multipart/form-data
- Body: file (BMEcat XML)
- Response: ImportJobId für Progress-Tracking
```

**Datenmodell-Änderungen**:
- Neue Tabelle: `catalog_imports` (id, supplier_id, catalog_id, imported_at, tenant_id)
- Neue Tabelle: `catalog_products` (id, catalog_import_id, supplier_aid, data)
- Foreign Key Beziehungen für Löschbarkeit

**Technische Herausforderungen**:
- XML-Parsing Performance für große Dateien
- Schema-Validation (XSD)
- Memory-Management bei 100k+ Produkten

**Aufwandsschätzung**: 8 Story Points
**Empfehlung**: Proceed - Gut integrierbar in bestehende Catalog-Architektur

### @Frontend Analysis
**Betroffene Components**:
- Catalog Management Page: Neue Import-Section
- File Upload Component: Drag & Drop für XML
- Progress Indicator: Real-time Status für lange Imports

**UX Considerations**:
- Klare Fehlermeldungen bei invaliden Dateien
- Progress-Bar für große Uploads
- Confirmation Dialog für Löschung

**State Management**:
- Pinia Store für Import-Jobs
- Reactive Updates für Progress

**Aufwandsschätzung**: 3 Story Points
**Empfehlung**: Proceed - Erweiterung bestehender Catalog-UI

### @Security Analysis
**Sicherheitsimplikationen**:
- XML External Entity (XXE) Attack Prevention
- File Upload Size Limits (max 100MB)
- Input Validation: Nur gültiges BMEcat XML
- Tenant Isolation: Kataloge tenant-spezifisch

**Datenidentifizierung**:
- Composite Key: `{tenant_id}-{supplier_id}-{catalog_id}-{timestamp}`
- Audit Trail: Alle Import/Lösch-Operationen loggen
- Soft Delete: Markierung als deleted, Hard Delete nach Retention

**Compliance-Relevanz**:
- GDPR: Recht auf Löschung ganzer Kataloge
- Data Minimization: Nur notwendige Felder importieren

**Empfohlene Maßnahmen**:
- Server-side XML Validation
- Rate Limiting für Import-Requests
- Encryption at Rest für sensitive Daten

**Security Sign-off Required**: Ja
**Empfehlung**: Proceed mit Security Review vor Merge

### @Architect Analysis
**System Design Impact**:
- Adapter Pattern: BMEcat als erster konkreter Adapter
- Async Processing: Background Jobs für große Imports
- Service Boundaries: Catalog-Service erweitert, kein neuer Service

**Architektur Patterns**:
- Command Pattern: ImportBmecatCommand
- Repository Pattern: CatalogImportRepository
- Observer Pattern: Progress Notifications

**Scalability Considerations**:
- Horizontal Scaling: Stateless Import-Handler
- Database Indexing: catalog_import_id für schnelle Löschung
- Caching: Import-Status in Redis

**Risiken**:
- Performance bei sehr großen Katalogen
- Schema-Evolution bei BMEcat-Versionen

**Empfehlung**: Proceed - Passt gut in Onion Architecture

### @QA Analysis
**Testbarkeit**: Hoch - XML-basierte Imports gut testbar

**Vorgeschlagene Testszenarien**:
- Unit: XML Parser, Validation Rules
- Integration: End-to-End Import mit Test-Katalogen
- Performance: 10k/100k Produkte Import-Zeit
- Security: XXE Attack Prevention
- Edge Cases: Ungültige XML, Duplikate, Encoding

**Automatisierbarkeit**: Hoch - XML-Fixtures einfach zu erstellen

**Akzeptanzkriterien (ergänzt)**:
- [ ] Import-Erfolg bei validen BMEcat 1.2 Dateien
- [ ] Aussagekräftige Fehler bei invaliden Dateien
- [ ] Katalog-Löschung entfernt alle zugehörigen Produkte
- [ ] Tenant-Isolation: Kataloge nicht cross-tenant sichtbar

### @DevOps Analysis
**Deployment Impact**:
- Neue API-Endpoints: Kein Breaking Change
- Database Migration: Neue Tabellen
- Storage: XML-Dateien in Blob Storage (optional)

**Monitoring Requirements**:
- Import-Duration Metrics
- Success/Failure Rates
- Storage Usage für Kataloge

**Infrastructure Changes**:
- File Upload Limits in Nginx/Reverse Proxy
- Background Job Queue (falls async)
- Backup Strategy für Katalog-Daten

**Skalierungs-Überlegungen**:
- Stateless API: Horizontal Scaling möglich
- Database: Indexing für catalog_import_id

### @Enventa Analysis
**ERP Integration Impact**:
- Potenzielle Synchronisation: BMEcat → Enventa Produkte
- Mapping: BMEcat ARTICLE → IcECArticle
- Actor Pattern: Single-threaded für Enventa-Integration

**Conflict Resolution**:
- Bei Duplikaten: Version-basiertes Update
- Tenant Isolation: Enventa per BusinessUnit

**Aufwand für Integration**: Separates Feature (nicht in diesem Scope)

---

## Konsolidierte Empfehlung (@SARAH)
**Status**: Proceed mit folgenden Anpassungen

**Konsens-Punkte**:
- Hoch priorisiert für B2B-Katalog-Management
- Adapter-Pattern als Basis für weitere Formate
- Tenant-Isolation und Löschbarkeit kritisch

**Identifizierte Konflikte**:
- Async vs Sync Processing: @Architect empfiehlt Async für Performance, @Backend sieht Sync als simpler

**SARAH Empfehlung**: Async Processing für Imports >1k Produkte, Sync für kleinere

**Risiken (priorisiert)**:
1. Performance bei großen Katalogen → Mitigation: Async + Progress Tracking
2. XML Security → Mitigation: Strict Validation + Size Limits

**Gesamtaufwand**: 13 Story Points (Backend 8, Frontend 3, Tests 2)

**Nächste Schritte**:
1. ADR für BMEcat Import Architecture erstellen ✅ DONE
2. Implementation in Catalog-Service starten
3. Security Review vor Merge

---

## Finale Spezifikation (@ProductOwner)

### User Story (Final)
Als Händler möchte ich BMEcat-Kataloge von Handelsverbänden importieren können, damit ich Produktstammdaten effizient synchronisiere. Die importierten Daten bleiben identifizierbar für spätere Löschung.

### Akzeptanzkriterien (Final)
- [ ] BMEcat 1.2/2005 Standard wird unterstützt
- [ ] Upload via API (POST /api/v1/catalogs/import/bmecat) möglich
- [ ] XML-Validierung mit aussagekräftigen Fehlermeldungen
- [ ] Katalog-Identifizierung durch {tenant_id}-{supplier_id}-{catalog_id}-{timestamp}
- [ ] Progress-Tracking für große Kataloge (>1k Produkte) via WebSocket
- [ ] Löschung ganzer Kataloge möglich (DELETE /api/v1/catalogs/{catalogId})
- [ ] Tenant-Isolation: Kataloge sind tenant-spezifisch
- [ ] Audit-Log für Import- und Lösch-Operationen
- [ ] Async Processing für Kataloge >1k Produkte

### Technische Spezifikation
- **Adapter Pattern**: BmecatImportAdapter in Catalog.Application
- **Database**: Neue Tabellen catalog_imports, catalog_products
- **Security**: XSD Validation, XXE Prevention, 100MB Size Limit
- **Performance**: <5 Minuten für 100k Produkte
- **API**: RESTful mit Wolverine Command Handler

### Abhängigkeiten (Final)
- Catalog-Domain Service ✅
- File-Upload-Infrastruktur ✅
- Tenant-Isolation Framework ✅
- Audit-Logging System ✅
- Background Job Processing (für Async)

### Risiken & Mitigation (Final)
| Risiko | Wahrscheinlichkeit | Impact | Mitigation |
|--------|-------------------|--------|------------|
| Performance bei großen Katalogen | Mittel | Hoch | Async Processing + Progress Tracking |
| XML Security Vulnerabilities | Niedrig | Hoch | Strict Validation + Security Review |
| Datenkonsistenz bei Fehlern | Mittel | Mittel | Transaction Scopes + Error Recovery |
| Schema-Evolution | Niedrig | Mittel | Version Handling in Adapter |

### Story Points: 13
- Backend Implementation: 8 SP
- Frontend Integration: 3 SP
- Testing & Security: 2 SP

---

## GitHub Issue (Planning)
**Issue Created**: #BME-001 - Implement BMEcat Catalog Import

**Labels**: feature, backend, frontend, security, P1

**Tasks**:
- [ ] Create ICatalogImportAdapter interface
- [ ] Implement BmecatImportAdapter with XML parsing
- [ ] Add database migrations for catalog tables
- [ ] Create ImportBmecatCommand and handler
- [ ] Add XSD validation and security measures
- [ ] Implement async processing with progress tracking
- [ ] Create REST API endpoint
- [ ] Add frontend upload component with progress UI
- [ ] Create comprehensive tests
- [ ] Security review and penetration testing

**Assignee**: @Backend (Lead), @Frontend, @QA, @Security

---

**Status**: ✅ Finalized - Ready for Implementation  
**Next**: Start development in next sprint</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/requirements/REQ-002-bmecat-import.md