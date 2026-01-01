# Fragen & Hinweise Funktion - Business Requirements Document (BRD)

**Owner:** @Administrator
**Effective Date:** 1. Januar 2026
**Status:** Active

## Executive Summary

Implementierung einer "Fragen & Hinweise"-Funktion für Store und Admin-Backend, die automatisch anonymisierte Issues mit relevanten Kontextinformationen erstellt. Diese Funktion ermöglicht es Benutzern, direkt aus der Anwendung heraus Support-Anfragen zu stellen, während sensible Daten automatisch anonymisiert werden.

## Business Objectives

### Primäre Ziele
- **Verbesserte User Experience:** Direkte Support-Möglichkeit ohne Anwendung verlassen
- **Automatisierte Issue-Erstellung:** Reduzierung manueller Support-Aufwände um 70%
- **Datenschutz-Konformität:** Vollständige Anonymisierung sensibler Informationen
- **Kontext-Relevanz:** Automatische Erfassung aller für Problemlösung relevanten Daten

### Sekundäre Ziele
- **Proaktive Problemerkennung:** Identifikation von Mustern und häufigen Problemen
- **Qualitätsverbesserung:** Feedback-gestützte Produktoptimierung
- **Support-Effizienz:** Schnellere Problemlösung durch vollständigen Kontext

## Functional Requirements

### Core Features

#### 1. Fragen & Hinweise Dialog
- **FR-001:** Modal/Dialog für Fragen und Hinweise in Store und Admin
- **FR-002:** Kategorisierung: "Frage", "Problem", "Verbesserungsvorschlag", "Sonstiges"
- **FR-003:** Freitext-Beschreibung mit Zeichenbegrenzung (1000 Zeichen)
- **FR-004:** Optionale Datei-Anhänge (Screenshots, Logs) mit Größenbegrenzung (5MB)

#### 2. Automatische Datenerfassung
- **FR-005:** Browser-Informationen (User-Agent, Screen Resolution, Browser Version)
- **FR-006:** Anwendungs-Kontext (Version, Environment, User Role für Admin)
- **FR-007:** Session-Informationen (Dauer, letzte Aktionen - anonymisiert)
- **FR-008:** Performance-Metriken (Ladezeiten, Fehler-Codes - falls verfügbar)
- **FR-009:** URL und Timestamp der Erfassung

#### 3. Anonymisierung
- **FR-010:** Vollständige Entfernung personenbezogener Daten (Namen, E-Mails, IPs)
- **FR-011:** Hashing von User-IDs für Korrelation ohne Identifizierung
- **FR-012:** Tenant-ID Beibehaltung für Support-Routing (anonymisiert)
- **FR-013:** Entfernung sensibler URL-Parameter und Query-Strings

#### 4. Issue-Erstellung
- **FR-014:** Automatische GitHub Issue Erstellung im B2Connect Repository
- **FR-015:** Standardisierte Issue-Templates basierend auf Kategorie
- **FR-016:** Automatische Label-Zuweisung (support, bug, enhancement, question)
- **FR-017:** Korrelations-ID für Nachverfolgung

#### 5. User Feedback
- **FR-018:** Bestätigung der erfolgreichen Übermittlung
- **FR-019:** Anzeige der Korrelations-ID für Nachverfolgung
- **FR-020:** Optionale E-Mail-Benachrichtigung bei Lösung (anonym)

### User Stories

#### Als Store-Benutzer
- **US-001:** Ich möchte eine Frage zu einem Produkt stellen können
- **US-002:** Ich möchte ein technisches Problem melden können
- **US-003:** Ich möchte einen Verbesserungsvorschlag einreichen können
- **US-004:** Ich möchte Screenshots von Fehlern anhängen können

#### Als Admin-Benutzer
- **US-005:** Ich möchte Systemprobleme mit vollem Kontext melden können
- **US-006:** Ich möchte Verbesserungsvorschläge für Admin-Funktionen machen können
- **US-007:** Ich möchte Performance-Probleme mit Metriken melden können
- **US-008:** Ich möchte API-Fehler mit Request-Details melden können

#### Als Support-Team
- **US-009:** Ich möchte alle relevanten Kontextinformationen automatisch erhalten
- **US-010:** Ich möchte Issues nach Kategorie und Schweregrad filtern können
- **US-011:** Ich möchte Korrelation zwischen Issues erkennen können
- **US-012:** Ich möchte anonymisierte Daten für GDPR-Konformität haben

## Technical Requirements

### Backend Implementation

#### API Endpoints
- **TR-001:** `POST /api/support/feedback` - Feedback/Frage erstellen
- **TR-002:** `GET /api/support/feedback/{correlationId}` - Status abfragen (Admin only)

#### Data Models
- **TR-003:** FeedbackRequest DTO mit Validierung
- **TR-004:** AnonymizedContext mit erfassten Informationen
- **TR-005:** GitHubIssuePayload für API-Integration

#### Anonymization Service
- **TR-006:** IDataAnonymizer Interface mit Implementierung
- **TR-007:** Hashing von User-IDs (SHA256 + Salt)
- **TR-008:** PII Detection und Removal
- **TR-009:** URL Sanitization

#### GitHub Integration
- **TR-010:** IGitHubService Interface für Issue-Erstellung
- **TR-011:** GitHub App Authentication (Personal Access Token)
- **TR-012:** Rate Limiting und Error Handling
- **TR-013:** Issue Template Management

### Frontend Implementation

#### Vue.js Components
- **TR-014:** FeedbackModal.vue für Store und Admin
- **TR-015:** ContextCollector composable für automatische Datenerfassung
- **TR-016:** FileUpload component mit Validierung
- **TR-017:** ConfirmationDialog für erfolgreiche Übermittlung

#### Data Collection
- **TR-018:** Browser API Integration (navigator.userAgent, window.screen)
- **TR-019:** Performance API für Ladezeiten
- **TR-020:** Error Boundary Integration für automatische Fehlererfassung

### Security & Compliance

#### GDPR Compliance
- **TR-021:** Keine Speicherung personenbezogener Daten
- **TR-022:** Anonymisierung vor jeglicher Verarbeitung
- **TR-023:** Recht auf Löschung (Correlation ID basiert)
- **TR-024:** Data Minimization Prinzip

#### Security Requirements
- **TR-025:** Input Validation und Sanitization
- **TR-026:** Rate Limiting pro User/Session
- **TR-027:** CORS Konfiguration für beide Frontends
- **TR-028:** API Key Security für GitHub Integration

### Performance & Scalability

#### Performance Targets
- **TR-029:** <500ms Response Time für Feedback-Submission
- **TR-030:** <2MB Bundle Size Impact auf Frontend
- **TR-031:** <100ms Context Collection Time

#### Scalability Requirements
- **TR-032:** Horizontal Scaling Support
- **TR-033:** Database Connection Pooling
- **TR-034:** Async Processing für GitHub API Calls

## Integration Requirements

### Existing Systems
- **IR-001:** Integration mit bestehender Wolverine CQRS Architecture
- **IR-002:** Tenant-Isolation beibehalten
- **IR-003:** Logging Framework Integration
- **IR-004:** Error Handling Patterns

### External Systems
- **IR-005:** GitHub Issues API Integration
- **IR-006:** Optional: Slack/Discord Webhooks für Notifications
- **IR-007:** Optional: Email Service für User Notifications

## Testing Requirements

### Unit Tests
- **TE-001:** Anonymization Service Tests (100% Coverage)
- **TE-002:** GitHub Service Integration Tests
- **TE-003:** Controller Validation Tests
- **TE-004:** Context Collection Tests

### Integration Tests
- **TE-005:** End-to-End Feedback Submission
- **TE-006:** GitHub Issue Creation Verification
- **TE-007:** Multi-Tenant Isolation Tests
- **TE-008:** Rate Limiting Tests

### E2E Tests
- **TE-009:** Store Frontend Feedback Flow
- **TE-010:** Admin Frontend Feedback Flow
- **TE-011:** File Upload Functionality
- **TE-012:** Error Scenarios

## Deployment & Rollout

### Phase 1: Backend Implementation (Week 1-2)
- Core Services und APIs implementieren
- Anonymization Service entwickeln
- GitHub Integration aufbauen

### Phase 2: Frontend Implementation (Week 3-4)
- Vue.js Components für beide Frontends
- Context Collection implementieren
- UI/UX Design finalisieren

### Phase 3: Testing & QA (Week 5-6)
- Vollständige Test Suite
- Security Audit
- Performance Testing

### Phase 4: Deployment & Monitoring (Week 7-8)
- Staged Rollout (Store first, dann Admin)
- Monitoring Setup
- User Training

## Success Metrics

### Quantitative Metrics
- **SM-001:** 95% User Satisfaction Score
- **SM-002:** <24h Average Response Time
- **SM-003:** 80% Reduction in Manual Support Tickets
- **SM-004:** >99.5% Uptime

### Qualitative Metrics
- **SM-005:** Improved Issue Quality (mehr Kontext)
- **SM-006:** Better Problem Resolution Rate
- **SM-007:** Enhanced User Trust
- **SM-008:** Positive Support Team Feedback

## Risk Assessment

### High Risk
- **RI-001:** GitHub API Rate Limits - Mitigation: Caching und Queueing
- **RI-002:** Data Privacy Compliance - Mitigation: Legal Review vor Deployment
- **RI-003:** User Adoption - Mitigation: Intuitive UI und Training

### Medium Risk
- **RI-004:** Performance Impact - Mitigation: Async Processing
- **RI-005:** False Positives in Anonymization - Mitigation: Extensive Testing

### Low Risk
- **RI-006:** GitHub API Changes - Mitigation: Abstraction Layer
- **RI-007:** Browser Compatibility - Mitigation: Fallback Mechanisms

## Dependencies

### Technical Dependencies
- GitHub Personal Access Token mit Issues-Berechtigung
- Vue.js 3 für Frontend Components
- Wolverine für CQRS Pattern
- Entity Framework für Data Access

### Team Dependencies
- @Backend für API Implementation
- @Frontend für UI Components
- @Security für Anonymization Review
- @DevOps für Deployment Setup
- @QA für Testing Strategy

## Cost Estimation

### Development Costs
- **Backend Development:** 40 Stunden
- **Frontend Development:** 60 Stunden
- **Testing & QA:** 40 Stunden
- **Security Review:** 20 Stunden

### Operational Costs
- **GitHub API Calls:** ~$50/Monat (Enterprise Plan)
- **Storage:** Minimal (nur temporäre Logs)
- **Monitoring:** Existing Infrastructure

### ROI Projection
- **Support Cost Reduction:** 70% (Manual Ticket Handling)
- **Resolution Time:** 50% faster
- **User Satisfaction:** +20% improvement
- **Break-even:** Within 3 months

---

**Diese BRD definiert die vollständigen Anforderungen für die Fragen & Hinweise-Funktion. Nach Approval durch @ProductOwner wird die Implementierung durch @Architect und die entsprechenden Development Teams durchgeführt.**</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/requirements/feedback-function-brd.md