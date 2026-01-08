---
docid: TPL-016
title: TPL REQ ANALYSIS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: TPL-REQ-ANALYSIS
title: "Templates: Requirements Analysis v2.0"
owner: "@SARAH"
status: "Active"
created: "2026-01-07"
---

# 📋 Requirements Analysis Templates v2.0

**Für PRM-010: Requirements Analysis Prompt**

---

## 🏷️ Template: Anforderungs-Kategorisierung

```yaml
# Anforderungs-Kategorisierung für REQ-XXX
Kategorie: [TRIVIAL | STANDARD | KOMPLEX]

Begründung:
- Geschätzter Gesamtaufwand: [X] Stunden
- Komplexität: [Niedrig | Mittel | Hoch]
- Stakeholder-Impact: [1-3 Teams | 4+ Teams]
- Technische Risiken: [Niedrig | Mittel | Hoch]

Agents beteiligt:
- TRIVIAL: ProductOwner, Backend, Frontend
- STANDARD: ^^ + TechLead, Security
- KOMPLEX: ^^ + QA, DevOps, UX

Analyse-Dauer: [30min | 90min | 3-4h]
Parallelisierung: [Ja | Teilweise | Nein]
```

---

## 🔗 Template: Cross-Requirement-Matrix

```markdown
# Cross-Requirement-Matrix für REQ-XXX

## Abhängigkeiten

### Blockiert von (MUSS fertig sein)
- [ ] REQ-YYY: [Kurzbeschreibung] - Status: [Offen | In Arbeit | Fertig]

### Baut auf (bereits verfügbar)
- [ ] REQ-ZZZ: [Kurzbeschreibung] - Verfügbar seit: [Datum]

### Beeinflusst (Koordination nötig)
- [ ] REQ-AAA: [Kurzbeschreibung] - Impact: [Hoch | Mittel | Niedrig]
- [ ] REQ-BBB: [Kurzbeschreibung] - Impact: [Hoch | Mittel | Niedrig]

### Parallel möglich (keine Abhängigkeit)
- [ ] REQ-CCC: [Kurzbeschreibung] - Team: [@Agent]

## System-Impact

### Services
- [ ] ServiceName: [Änderungstyp - Neu/Modifiziert/Entfernt]

### Datenbanken
- [ ] Schema: [Tabelle.Feld] - Änderung: [Neu/Index/FK/etc]
- [ ] Migration: [Up/Down Skript erforderlich]

### APIs
- [ ] Endpoint: [GET/POST/PUT/DELETE] /api/path - [Neu/Modifiziert]
- [ ] Version: [Breaking Change | Backward Compatible]

### UI-Komponenten
- [ ] ComponentName: [Neu/Modifiziert] - Impact: [Hoch | Mittel | Niedrig]
- [ ] Theme/Design: [Änderungen nötig]

## Koordinations-Plan

### Mit anderen Teams
- [ ] @Backend: [Was koordinieren] - Wann: [Sprint X]
- [ ] @Frontend: [Was koordinieren] - Wann: [Sprint X]
- [ ] @DevOps: [Was koordinieren] - Wann: [Sprint X]

### Mit ProductOwner
- [ ] Stakeholder absprechen: [Datum]
- [ ] Priorität bestätigen: [Datum]
```

---

## 📝 Template: Change-Log

```markdown
# Change-Log für REQ-XXX

| Version | Datum | Änderung | Verursacher | Impact | Status |
|---------|-------|----------|-------------|--------|--------|
| v1.0    | 2026-01-07 | Initial Requirements | @ProductOwner | Baseline | ✅ Approved |
| v1.1    | 2026-01-08 | Scope erweitert um X | @TechLead | +2h Aufwand | ⏳ Review |
| v1.2    | 2026-01-09 | Security-Requirements hinzugefügt | @Security | +1h Review | ✅ Approved |
| v2.0    | 2026-01-10 | Major Rewrite nach Stakeholder-Feedback | @ProductOwner | Scope Change | ⏳ Review |

## Aktuelle Version: v[X.X]

## Wichtige Änderungen seit Baseline:
- [ ] Änderung 1: [Beschreibung] - Impact: [Hoch|Mittel|Niedrig]
- [ ] Änderung 2: [Beschreibung] - Impact: [Hoch|Mittel|Niedrig]
```

---

## 🎯 Template: Use-Case-Decomposition (@QA)

```markdown
# Use-Case-Decomposition für REQ-XXX

## Übersicht
**Titel**: [Kurzer, beschreibender Titel]
**Komplexität**: [Einfach | Mittel | Komplex]
**Priorität**: [Kritisch | Hoch | Mittel | Niedrig]

## Primary Actor
**Rolle**: [z.B. "Eingeloggter Benutzer", "Administrator", "API-Client"]
**Persona**: [Link zu Persona-Dokument oder kurze Beschreibung]

## Goal
**In Context**: [Was will der Actor erreichen?]
**Level**: [User-Goal | Subfunction]

## Preconditions
- [ ] System ist verfügbar
- [ ] Actor ist authentifiziert [falls relevant]
- [ ] [Spezifische Bedingung für diese Anforderung]
- [ ] [Weitere Preconditions]

## Main Success Scenario
1. Actor [Aktion 1]
2. System [Response 1]
3. Actor [Aktion 2]
4. System [Response 2]
5. Actor erreicht Goal

## Alternative Flows

### Alternative A: [Beschreibung der Alternative]
1. Actor [alternative Aktion]
2. System [alternative Response]
3. Fortsetzung im Main Scenario bei Schritt [X]

### Exception E1: [Fehlerbedingung]
1. System erkennt [Problem]
2. System zeigt [Fehlermeldung]
3. Actor kann [Recovery Action] durchführen
4. Fortsetzung bei Schritt [X] oder Abbruch

## Postconditions
**Success**: [Systemzustand nach erfolgreichem Use-Case]
- [ ] Daten gespeichert
- [ ] Benachrichtigungen versendet
- [ ] Audit-Log aktualisiert

**Failure**: [Systemzustand nach fehlgeschlagenem Use-Case]
- [ ] Transaktion zurückgerollt
- [ ] Fehler geloggt
- [ ] Benutzer informiert

## Business Rules
- [ ] Rule 1: [Beschreibung]
- [ ] Rule 2: [Beschreibung]

## Performance Requirements
- [ ] Response Time: < [X] Sekunden
- [ ] Concurrent Users: [X] gleichzeitig
- [ ] Data Volume: [X] Records/Operation

## Test Scenarios (abgeleitet)
- [ ] Happy Path: [Beschreibung]
- [ ] Alternative A: [Beschreibung]
- [ ] Exception E1: [Beschreibung]
- [ ] Edge Case: [Beschreibung]
```

---

## 👥 Template: @UX Integration

```markdown
# UX Analysis für REQ-XXX

## User Journey Mapping

### Current State
1. User [aktuelle Aktion] → [Erfahrung] → [Pain Point]
2. User [aktuelle Aktion] → [Erfahrung] → [Pain Point]
3. User erreicht [aktuelles Ziel] mit [Effort Level]

### Proposed State (nach Implementation)
1. User [neue Aktion] → [verbesserte Erfahrung] → [Gain]
2. User [neue Aktion] → [verbesserte Erfahrung] → [Gain]
3. User erreicht [neues Ziel] mit [reduziertem Effort]

## Persona Impact

### Primary Persona: [Name]
**Demographics**: [Alter, Rolle, Erfahrung]
**Goals**: [Was will diese Persona erreichen?]
**Pain Points**: [Aktuelle Probleme]
**Impact Assessment**: [Wie stark betroffen? Hoch/Mittel/Niedrig]

### Secondary Personas
- [Persona 2]: [Impact Level] - [Begründung]
- [Persona 3]: [Impact Level] - [Begründung]

## Accessibility Requirements
- [ ] WCAG 2.1 Level AA Compliance
- [ ] Keyboard Navigation
- [ ] Screen Reader Support
- [ ] Color Contrast (4.5:1 minimum)
- [ ] Focus Management
- [ ] Error Handling (clear messages)

## Design System Compliance
- [ ] Uses approved components: [Liste]
- [ ] Follows spacing guidelines: [Ja/Nein]
- [ ] Color palette: [Primary/Secondary/Accent]
- [ ] Typography: [Approved fonts/sizes]
- [ ] Responsive breakpoints: [Mobile/Tablet/Desktop]

## Empathy Mapping

### Says (was sagt der User)
- "Ich wünschte, [Problem]"
- "Ich brauche [Feature] um [Ziel]"

### Thinks (was denkt der User)
- "Das ist zu kompliziert"
- "Warum kann ich nicht einfach [Aktion]?"

### Does (was macht der User)
- [Aktuelle Workarounds]
- [Alternative Tools/Prozesse]

### Feels (wie fühlt sich der User)
- [Frustration mit aktueller Lösung]
- [Erleichterung mit neuer Lösung]

## Validation Questions
- [ ] Ist die Lösung intuitiv?
- [ ] Reduziert sie kognitive Last?
- [ ] Ist sie zugänglich für alle User?
- [ ] Passt sie zum Gesamt-Design?
```

---

## 📊 Template: Value-Scoring (@ProductOwner)

```markdown
# Value-Scoring für REQ-XXX

## Business Value Assessment

### Value Score (1-10)
**Score**: [X]/10

**Begründung**:
- Business Impact: [Beschreibung] (+X Punkte)
- User Satisfaction: [Beschreibung] (+X Punkte)
- Competitive Advantage: [Beschreibung] (+X Punkte)
- Revenue Potential: [Beschreibung] (+X Punkte)

### Effort Score (1-10)
**Score**: [X]/10

**Begründung**:
- Development Complexity: [Beschreibung] (+X Punkte)
- Testing Requirements: [Beschreibung] (+X Punkte)
- Deployment Risk: [Beschreibung] (+X Punkte)
- Team Coordination: [Beschreibung] (+X Punkte)

### Risk Score (1-10)
**Score**: [X]/10

**Begründung**:
- Technical Risk: [Beschreibung] (+X Punkte)
- Business Risk: [Beschreibung] (+X Punkte)
- Timeline Risk: [Beschreibung] (+X Punkte)
- Dependency Risk: [Beschreibung] (+X Punkte)

## Prioritization Quadrant

### Quadrant: [HIGH-VALUE/LOW-EFFORT | HIGH-VALUE/HIGH-EFFORT | LOW-VALUE/LOW-EFFORT | LOW-VALUE/HIGH-EFFORT]

### Action Recommendation:
- [ ] **SOFORT** (High Value + Low Effort)
- [ ] **PLANEN** (High Value + High Effort)
- [ ] **NICE-TO-HAVE** (Low Value + Low Effort)
- [ ] **SKIP** (Low Value + High Effort)

## ROI Calculation

### Estimated Benefits
- **Quantitative**: [€X savings | X% efficiency gain | X new customers]
- **Qualitative**: [Better UX | Reduced support tickets | Competitive edge]

### Estimated Costs
- **Development**: [X] Personentage
- **Testing**: [X] Personentage
- **Deployment**: [X] Personentage
- **Maintenance**: [X] Personentage/Jahr

### ROI: [X]% | Payback Period: [X] Monate

## MoSCoW Prioritization

### Must Have (Kritisch für Erfolg)
- [ ] [Kriterium 1]

### Should Have (Wichtig, aber nicht kritisch)
- [ ] [Kriterium 2]

### Could Have (Nice-to-have)
- [ ] [Kriterium 3]

### Won't Have (Nicht in diesem Scope)
- [ ] [Kriterium 4]
```

---

## 🔄 Template: Konsolidierung (@TechLead)

```markdown
# Konsolidierte Analyse für REQ-XXX

## Executive Summary
[2-3 Sätze Gesamt-Einschätzung]

## Kategorie & Scope
- **Kategorie**: [TRIVIAL | STANDARD | KOMPLEX]
- **Gesamtaufwand**: [X] Stunden
- **Timeline**: [X] Wochen
- **Risiko-Level**: [Niedrig | Mittel | Hoch]

## Agent-Analysen Summary

### @ProductOwner
- **Value Score**: [X]/10
- **Priorität**: [SOFORT | PLANEN | NICE-TO-HAVE | SKIP]
- **Business Impact**: [Hoch | Mittel | Niedrig]

### @Backend
- **Komplexität**: [XS | S | M | L | XL]
- **Technische Risiken**: [Liste]
- **Architektur-Impact**: [Hoch | Mittel | Niedrig]

### @Frontend
- **UI-Impact**: [Hoch | Mittel | Niedrig]
- **UX-Verbesserungen**: [Liste]
- **Komplexität**: [XS | S | M | L | XL]

### @Security
- **Security-Risiken**: [Hoch | Mittel | Niedrig]
- **Compliance-Requirements**: [Liste]
- **Sign-off erforderlich**: [Ja | Nein]

### @QA
- **Testbarkeit**: [Gut | Mittel | Schwierig]
- **Regression-Risiko**: [Hoch | Mittel | Niedrig]
- **Automatisierung möglich**: [Ja | Teilweise | Nein]

### @DevOps
- **Deployment-Impact**: [Hoch | Mittel | Niedrig]
- **Infrastructure-Changes**: [Liste]
- **Monitoring erforderlich**: [Ja | Nein]

### @UX (falls beteiligt)
- **User-Impact**: [Hoch | Mittel | Niedrig]
- **Accessibility**: [WCAG Compliant | Needs Work]
- **Persona-Fit**: [Gut | Mittel | Schlecht]

## Cross-Requirement-Impact
- **Blockiert von**: [Liste]
- **Beeinflusst**: [Liste]
- **Koordination nötig**: [Ja | Nein]

## Gesamtrisiken & Mitigation

| Risiko | Wahrscheinlichkeit | Impact | Mitigation | Owner |
|--------|-------------------|--------|------------|-------|
| [Risiko 1] | [Hoch|Mittel|Niedrig] | [Hoch|Mittel|Niedrig] | [Plan] | [@Agent] |
| [Risiko 2] | [Hoch|Mittel|Niedrig] | [Hoch|Mittel|Niedrig] | [Plan] | [@Agent] |

## Empfehlung
[PROCEED | ADJUST | CLARIFY | REJECT]

**Begründung**: [2-3 Sätze]

## Next Steps
1. [ ] [Sofortige Aktion]
2. [ ] [Nächster Meilenstein]
3. [ ] [Follow-up]

## Change-Log
- v1.0: [Datum] - Initial Analysis
- v[X.X]: [Datum] - [Letzte Änderung]
```