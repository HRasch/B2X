---
docid: UNKNOWN-187
title: Requirements Consolidation.Prompt
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
agent: SARAH
description: Konsolidierung aller Agent-Analysen für eine Anforderung
---

# Requirements Consolidation

Konsolidiere alle Agent-Analysen für die Anforderung.

## Input
Lese alle Analyse-Dokumente in `.ai/requirements/REQ-{{req_id}}-*-analysis.md`

## Konsolidierungs-Framework

### 1. Sammle alle Perspektiven
- ProductOwner Analyse
- TechLead Analyse
- Backend Analyse
- Frontend Analyse
- Security Analyse
- QA Analyse
- DevOps Analyse

### 2. Identifiziere Konsens
Punkte, bei denen alle Agenten übereinstimmen.

### 3. Identifiziere Konflikte
| # | Konflikt | Agent A | Agent B | Beschreibung |
|---|----------|---------|---------|--------------|
| 1 | ...      | ...     | ...     | ...          |

### 4. Löse Konflikte
Für jeden Konflikt:
- Analysiere beide Perspektiven
- Gewichte nach Projekt-Prioritäten
- Gib begründete Empfehlung

### 5. Aggregiere Risiken
| Risiko | Quellen | Gesamtschwere | Empfohlene Mitigation |
|--------|---------|---------------|----------------------|
| ...    | ...     | ...           | ...                  |

### 6. Berechne Gesamtaufwand
| Domain | Schätzung | Konfidenz |
|--------|-----------|-----------|
| Backend | ... | ... |
| Frontend | ... | ... |
| ...    | ... | ... |
| **Gesamt** | **...** | **...** |

### 7. Finale Empfehlung
- [ ] ✅ Proceed - Anforderung kann wie spezifiziert umgesetzt werden
- [ ] ⚠️ Proceed with Adjustments - Anpassungen empfohlen
- [ ] ❓ Needs Clarification - Weitere Klärung erforderlich
- [ ] ❌ Not Recommended - Umsetzung nicht empfohlen

## Output Format

```markdown
# REQ-{{req_id}}: [Titel] - Konsolidierte Analyse

## Executive Summary
[2-3 Sätze Gesamtbild]

## Analyse-Übersicht

### Konsens-Punkte
- ✅ [Punkt 1]
- ✅ [Punkt 2]

### Gelöste Konflikte
| # | Konflikt | Entscheidung | Begründung |
|---|----------|--------------|------------|
| 1 | ...      | ...          | ...        |

### Offene Punkte (Klärungsbedarf)
- ❓ [Offener Punkt 1]
- ❓ [Offener Punkt 2]

## Risiko-Matrix
| Risiko | Schwere | Wahrscheinlichkeit | Mitigation |
|--------|---------|-------------------|------------|
| ...    | Hoch/Mittel/Niedrig | Hoch/Mittel/Niedrig | ... |

## Aufwandsschätzung
| Bereich | Schätzung | Konfidenz | Anmerkungen |
|---------|-----------|-----------|-------------|
| Backend | M | Hoch | ... |
| Frontend | S | Mittel | ... |
| DevOps | S | Hoch | ... |
| **Gesamt** | **M** | **Mittel** | |

## Empfehlung
**[PROCEED / ADJUST / CLARIFY / REJECT]**

[Ausführliche Begründung der Empfehlung]

## Nächste Schritte
1. [ ] [Schritt 1]
2. [ ] [Schritt 2]
3. [ ] [Schritt 3]

## Beteiligte Agenten
- @ProductOwner: [Status der Analyse]
- @TechLead: [Status der Analyse]
- @Backend: [Status der Analyse]
- @Frontend: [Status der Analyse]
- @Security: [Status der Analyse]
- @QA: [Status der Analyse]
- @DevOps: [Status der Analyse]

---
Konsolidiert: [Datum] | Agent: @SARAH
Status: Final / Draft
```

---
Speichere in: `.ai/requirements/REQ-{{req_id}}-consolidated.md`
