---
docid: UNKNOWN-186
title: Requirements Analysis.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: SARAH
description: Multi-Agent Requirements Analysis - Koordinierte Anforderungsanalyse (v2.0)
---

# Requirements Analysis v2.0

Analysiere die folgende Anforderung aus deiner Domain-Perspektive.

## Anforderung
{{requirement_description}}

## 🚀 NEU: Anforderungs-Kategorisierung (v2.0)

**Bestimme die Größe der Anforderung:**

### 🟢 TRIVIAL (< 4h Gesamtaufwand)
- Bugfix, kleine Feature, Konfiguration
- **Agents beteiligt**: @ProductOwner, @Backend, @Frontend
- **Analyse-Dauer**: 30 Minuten
- **Parallelisierung**: Ja (alle gleichzeitig)

### 🟡 STANDARD (4-20h Gesamtaufwand)
- Neue Feature, API-Änderung, UI-Update
- **Agents beteiligt**: @ProductOwner, @Backend, @Frontend, @TechLead, @Security
- **Analyse-Dauer**: 90 Minuten
- **Parallelisierung**: Ja (alle gleichzeitig)

### 🔴 KOMPLEX (20-80h Gesamtaufwand)
- Neue Service, Architektur-Change, Multi-Team-Impact
- **Agents beteiligt**: ALLE Agents (@ProductOwner, @Backend, @Frontend, @TechLead, @Security, @QA, @DevOps, @UX)
- **Analyse-Dauer**: 3-4 Stunden
- **Parallelisierung**: Teilweise (Domain-Gruppen parallel, dann Konsolidierung)

## ⚡ NEU: Parallelisierung (v2.0)

**WICHTIG**: Alle Agents starten GLEICHZEITIG, nicht sequentiell!

**Koordination**:
- @SARAH startet alle relevanten Agents gleichzeitig
- Jeder Agent arbeitet unabhängig an seiner Domain-Analyse
- @TechLead konsolidiert nach 60-90 Minuten
- Rate-Limit-Schutz: Max 2 Agents gleichzeitig bei Bedarf

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

**Für @UX:**
- User Journey Mapping
- Persona Impact Assessment
- Accessibility Requirements
- Design System Compliance
- User Experience Validation
- Empathy Mapping (Pain Points, Gains)

**Für @QA:**
- Testbarkeit
- Testszenarien (Happy Path + Edge Cases)
- Automatisierungspotential
- Regressions-Risiko
- Akzeptanzkriterien Ergänzungen
- **NEU**: Use-Case-Decomposition (für KOMPLEX Anforderungen)

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

## 🚀 NEU: Cross-Requirement-Matrix (v2.0)

**Für STANDARD & KOMPLEX Anforderungen:**

### Abhängigkeiten
- **Blockiert von**: [REQ-XXX] (noch nicht fertig)
- **Baut auf**: [REQ-YYY] (bereits verfügbar)
- **Beeinflusst**: [REQ-ZZZ] (Koordination nötig)
- **Parallel möglich**: [REQ-AAA] (keine Abhängigkeit)

### System-Impact
- **Services**: [Liste betroffener Services]
- **Datenbanken**: [Schema-Änderungen]
- **APIs**: [Neue/Modifizierte Endpoints]
- **UI-Komponenten**: [Betroffene Bereiche]

## 📝 NEU: Change-Log (v2.0)

**Während der Analyse auftretende Änderungen dokumentieren:**

| Version | Datum | Änderung | Grund | Impact |
|---------|-------|----------|-------|--------|
| v1.0    | YYYY-MM-DD | Initial | - | - |
| v1.1    | YYYY-MM-DD | Scope erweitert | Stakeholder-Feedback | +2h Aufwand |

## 🎯 NEU: Use-Case-Decomposition (v2.0)

**Für KOMPLEX Anforderungen (@QA führt):**

### Primary Actor
[Wer ist der Haupt-Nutzer?]

### Preconditions
- [ ] Bedingung 1
- [ ] Bedingung 2

### Main Success Scenario
1. User tut X
2. System macht Y
3. User sieht Z

### Alternative Flows
- **Alternative A**: Wenn Bedingung W, dann...
- **Exception E**: Bei Fehler F, dann...

### Postconditions
- [ ] Ergebnis erreicht
- [ ] System im konsistenten Zustand

## Output Format

```markdown
## [Agent] Analyse für REQ-XXX (v2.0)

### Zusammenfassung
[1-2 Sätze]

### Kategorie
[🟢 TRIVIAL | 🟡 STANDARD | 🔴 KOMPLEX]

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

<!-- NEU: Für STANDARD/KOMPLEX -->
### Cross-Requirement-Impact
- **Blockiert von**: [Liste]
- **Beeinflusst**: [Liste]
- **System-Impact**: [Services/APIs/UI]

<!-- NEU: Für KOMPLEX -->
### Use-Case (falls relevant)
- **Actor**: [Primärer Nutzer]
- **Main Flow**: [Schritte 1-2-3]
- **Alternatives**: [Ausnahmen]
```

---
Speichere Analyse in: `.ai/requirements/REQ-{{req_id}}-{{agent}}-analysis.md`
