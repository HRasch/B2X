# @DatabaseSpecialist Integration Plan

## Übersicht
Dieses Dokument beschreibt, wie @DatabaseSpecialist optimal in die Teamprozesse eingebunden werden kann, um Datenbank-Performance und -Optimierung zu maximieren.

## Aktuelle Spezialisierung
- **Database Performance**: Query-Optimierung, Indizierung, Performance-Tuning
- **SQL/NoSQL Optimization**: EF Core SQL-Review, Ausführungsplan-Analyse
- **Unterstützung**: Tests, Entwicklung, Schema-Design

## Optimale Unterstützungsbereiche

### 1. Code Review Integration
**Automatische Einbindung bei DB-bezogenen Änderungen:**
- PRs mit Änderungen in `backend/**/Data/`, `**/repositories/**`, `**/models/**`
- Automatische Benachrichtigung von @DatabaseSpecialist
- Fokus: Query-Optimierung, Indizierung, EF Core SQL-Generierung

### 2. Performance Monitoring
**Proaktive Überwachung:**
- DB-Performance-Metriken in CI/CD-Pipelines
- Automatische Alerts bei Performance-Degradation
- Regelmäßige Performance-Audits (wöchentlich/monatlich)

### 3. Sprint Planning Integration
**Frühe Einbindung:**
- Bei Identifizierung von DB-Aufgaben in Sprint-Planning
- Schema-Design-Reviews vor Implementierung
- Kapazitätsplanung für DB-Optimierungen

### 4. Test Integration
**DB-Performance-Tests:**
- Integration in bestehende Test-Suites
- Automatische Query-Performance-Tests
- Regression-Tests für DB-Änderungen

### 5. Incident Response
**Bei DB-Problemen:**
- Automatische Eskalation bei DB-Performance-Issues
- Root-Cause-Analysis für DB-bezogene Incidents
- Lessons Learned Dokumentation

## Implementierungsplan

### Phase 1: Prozess-Integration (1-2 Wochen)
1. ✅ **Code Review Hooks**: @DatabaseSpecialist-Benachrichtigung in backend.instructions.md integriert
2. ⏳ **Monitoring Setup**: Aufgabe an @DevOps delegiert ([.ai/status/db-monitoring-setup.md](.ai/status/db-monitoring-setup.md))
3. ⏳ **Sprint Template Update**: Aufgabe an @ScrumMaster delegiert ([.ai/status/sprint-template-db-update.md](.ai/status/sprint-template-db-update.md))

### Phase 2: Automatisierung (2-4 Wochen)
1. **CI/CD Integration**: Automatische DB-Performance-Tests
2. **Alert System**: Performance-Degradation-Alerts
3. **Documentation**: DB-Best-Practices in Knowledgebase

### Phase 3: Optimierung (Laufend)
1. **Feedback Loop**: Regelmäßige Reviews der @DatabaseSpecialist-Beiträge
2. **Capability Expansion**: Neue DB-Technologien/Optimierungen
3. **Team Training**: DB-Performance-Best-Practices für alle Entwickler

## Erwartete Vorteile
- **Performance**: 20-30% DB-Performance-Verbesserung
- **Qualität**: Reduzierte DB-bezogene Bugs
- **Effizienz**: Schnellere Identifizierung von Performance-Issues
- **Wissen**: Zentralisierte DB-Expertise im Team

## Metriken
- DB-Query-Performance (durchschnittliche Ausführungszeit)
- Anzahl DB-bezogener Incidents
- @DatabaseSpecialist-Interventionsrate
- Team-Zufriedenheit mit DB-Unterstützung

## Verantwortlichkeiten
- **@SARAH**: Koordination und Prozess-Updates
- **@DatabaseSpecialist**: Expertise-Bereitstellung und Reviews
- **@Backend**: Integration in Entwicklungsprozesse
- **@DevOps**: Monitoring und Alert-Setup

## Nächste Schritte
1. Review dieses Plans mit @TechLead und @Architect
2. Implementierung der Code-Review-Hooks
3. Setup von Performance-Monitoring
4. Training für Team-Mitglieder

**Status**: Phase 1 gestartet - Aufgaben delegiert</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/processes/DATABASE_SPECIALIST_INTEGRATION.md