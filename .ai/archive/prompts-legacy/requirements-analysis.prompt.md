---
docid: UNKNOWN-023
title: Requirements Analysis.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

---
agent: SARAH
description: Multi-Agent Requirements Analysis - Koordinierte Anforderungsanalyse
---

# Requirements Analysis

Analysiere die folgende Anforderung aus deiner Domain-Perspektive.

## Anforderung
{{requirement_description}}

## Analyse-Framework

Führe eine strukturierte Analyse durch und dokumentiere:

### 1. Verständnis-Check
- [ ] Anforderung ist klar und eindeutig
- [ ] Scope ist definiert
- [ ] Abhängigkeiten sind identifiziert

### 2. Domain-spezifische Analyse

**Für @ProductOwner:**
- Business Value Assessment
- User Story Formulierung
- Akzeptanzkriterien
- Priorisierung (MoSCoW)
- Stakeholder Impact

**Für @TechLead:**
- Architektur Impact
- Machbarkeitsanalyse
- Technische Schulden Risiko
- System-Abhängigkeiten
- Langfristige Implikationen

**Für @Backend:**
- Betroffene Services/APIs
- Datenmodell-Änderungen
- Business Logic Komplexität
- Integration Points
- Aufwandsschätzung (T-Shirt Sizes)

**Für @Frontend:**
- UI/UX Implikationen
- Component Impact
- State Management
- Responsive/Accessibility
- Aufwandsschätzung (T-Shirt Sizes)

**Für @Security:**
- Sicherheitsimplikationen
- Auth/AuthZ Änderungen
- Data Protection
- Compliance Relevanz
- Security Sign-off Requirement

**Für @QA:**
- Testbarkeit
- Testszenarien (Happy Path + Edge Cases)
- Automatisierungspotential
- Regressions-Risiko
- Akzeptanzkriterien Ergänzungen

**Für @DevOps:**
- Deployment Impact
- Infrastructure Changes
- Monitoring Requirements
- Skalierungs-Implikationen
- Rollback Strategy

### 3. Risiken & Bedenken
- [ ] Identifizierte Risiken
- [ ] Mitigation Vorschläge

### 4. Offene Fragen
- [ ] Klärungsbedarf

### 5. Empfehlung
- [ ] Proceed as is
- [ ] Proceed with adjustments
- [ ] Needs clarification
- [ ] Not recommended

### 6. Aufwandsschätzung
[XS | S | M | L | XL] + Konfidenz [Niedrig | Mittel | Hoch]

## Output Format

```markdown
## [Agent] Analyse für REQ-XXX

### Zusammenfassung
[1-2 Sätze]

### Details
[Domain-spezifische Punkte]

### Risiken
| Risiko | Schwere | Mitigation |
|--------|---------|------------|
| ...    | ...     | ...        |

### Offene Fragen
- [ ] ...

### Empfehlung
[Proceed/Adjust/Clarify/Reject] - [Begründung]

### Aufwand
[Size] | Konfidenz: [Level]
```

---
Speichere Analyse in: `.ai/requirements/REQ-{{req_id}}-{{agent}}-analysis.md`
