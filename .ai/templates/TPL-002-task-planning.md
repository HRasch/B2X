---
docid: TPL-002
title: Task Planning Template
owner: @SARAH
status: Active
created: 2026-01-09
---

# Task Planning Template (TPL-002)

**Zweck**: Strukturiere Tasks zu Beginn für effiziente, agentenbasierte Umsetzung mit runSubagent und MCP-Services.

## 1. Task-Übersicht
- **Titel**: [Kurze Beschreibung]
- **Beschreibung**: [Detaillierte Anforderungen]
- **Priorität**: P0 | P1 | P2 | P3
- **Deadline**: [Datum]
- **Stakeholder**: [Beteiligte Agenten/Personen]

## 2. Task-Zerlegung (Plan-Phase)
Zerlege in atomare Subtasks mit Schätzung (T-Shirt-Sizing: S/M/L).

| Subtask | Beschreibung | Agent | Aufwand | Abhängigkeiten | MCP-Validierung |
|---------|--------------|-------|---------|----------------|-----------------|
| [Subtask 1] | [Details] | @Backend | S | Keine | Roslyn MCP |
| [Subtask 2] | [Details] | @Frontend | M | Subtask 1 | TypeScript MCP |

## 3. Agent-Zuweisung & Kontext-Optimierung
- **Spezialisierte Agenten**: Weise basierend auf [AGT-*] Registry zu.
- **Kontext-Isolierung**: Lade nur relevante Dateien ([GL-043] Smart Attachments).
- **Token-Budget**: Max. [X] Tokens pro Subtask.

## 4. Ausführung (Act-Phase)
- **runSubagent-Nutzung**: Parallele, isolierte Ausführung.
  ```
  #runSubagent: [Subtask-Beschreibung]
  - [Schritt 1]
  - [Schritt 2]
  - Rückgabe: [Erwartete Ergebnisse]
  ```
- **MCP-Hintergrundarbeiten**: Automatische Validierungen (z.B. Security MCP für Scans).

## 5. Kontrolle (Control-Phase)
- **Metriken**: Token-Verbrauch, Zeit, Fehler-Rate ([GL-046] Token Audit).
- **Qualitätsgates**: Tests, Reviews ([ADR-020] PR Quality Gate).
- **Lessons Learned**: Aktualisiere `.ai/knowledgebase/lessons.md` nach Abschluss.

## 6. Akzeptanzkriterien
- [ ] Task in Subtasks zerlegt.
- [ ] Agenten zugewiesen und kontextoptimiert.
- [ ] MCP-Validierungen integriert.
- [ ] Token-Verbrauch um ≥40% reduziert.
- [ ] Alle Subtasks abgeschlossen und getestet.

## 7. Risiken & Fallbacks
- **Risiko**: [z.B. MCP-Fehler] → Fallback: Manuelle Validierung.
- **Escalation**: Bei Blockaden @SARAH kontaktieren.

**Verwendung**: Kopiere dieses Template für neue Tasks und fülle aus. Speichere als `.ai/tasks/task-[ID]-planning.md`.