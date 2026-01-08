---
docid: UNKNOWN-016
title: Dependency Upgrade Research.Prompt
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

# Dependency Upgrade Research Workflow

## Ziel
Recherchiere eine neuere Software-Version, fasse Änderungen zusammen und aktualisiere die Knowledgebase.

## Workflow-Phasen

### Phase 1: Recherche (User startet)
```
User: @Architect "Recherchiere Package X von v1.0 zu v2.0"
→ Task: fetch_webpage(package documentation, changelog, migration guide)
→ Output: Rohtext aller gefundenen Informationen
```

### Phase 2: Analyse & Zusammenfassung (Agent)
```
Input: Rohtext aus Recherche
Aufgaben:
  1. Breaking Changes identifizieren & priorisieren
  2. Neue Features & Capabilities auflisten
  3. Performance-Improvements dokumentieren
  4. Deprecations & Removals erfassen
  5. Migration-Anleitung extrahieren
  6. Security-Fixes dokumentieren

Output: `.ai/knowledgebase/{software}/{version}.md`
```

### Phase 3: Token-Optimierung
```
Richtlinien:
  ✓ Bullets statt Prosa
  ✓ Tabellen für Vergleiche
  ✓ Code-Beispiele nur für kritische Changes
  ✓ Links statt vollständiger Dokumentation
  ✓ Kurze, prägnante Beschreibungen (<50 Wörter pro Item)
  
Format:
  - Heading-Hierachie: # Version > ## Category > ### Item
  - Max 1 KB pro kritisches Feature
  - Redundanzen eliminieren
```

### Phase 4: Agent-Integration
```
Inhalte nach Agent-Perspektive:
  - @Architect: System Design Impact, Integration Patterns
  - @Backend: API Changes, Data Model Changes
  - @Frontend: UI Component Updates, State Management
  - @Security: Security Improvements, CVE Fixes
  - @DevOps: Deployment Changes, Dependency Updates
  - @QA: Breaking Changes, Migration Tests
  - @TechLead: Code Quality Impacts, Best Practices
```

### Phase 5: Index-Update
```
Update: `.ai/knowledgebase/INDEX.md`
  1. Software hinzufügen/updaten
  2. Verfügbare Versionen auflisten
  3. Tags vergeben (breaking-changes, security, performance)
  4. Related-Links aktualisieren
```

## Template: Dependency-Summary

```markdown
# {Software} {old-version} → {new-version}

## Quick Summary
[1-2 Sätze: Was ändert sich kritisch?]

## Breaking Changes
| Change | Impact | Migration |
|--------|--------|-----------|
| ... | ... | ... |

## New Features
- Feature 1: [Kurzbeschreibung]
- Feature 2: [Kurzbeschreibung]

## Security Fixes
- CVE-XXXX: [Beschreibung]

## Migration Checklist
- [ ] Breaking Changes prüfen
- [ ] Code updaten
- [ ] Tests anpassen
- [ ] Deployment testen

## Agent-Specific Notes

### @Architect
[System Design Implikationen]

### @Backend
[API/Data Changes]

### @Security
[Security Improvements]

### @DevOps
[Deployment Changes]
```

## Automation
```
Workflow in `.ai/workflows/`:
  1. User gibt Software + Versionen ein
  2. Researcher (Agent oder Subagent) fetcht Doku
  3. Agent schreibt Zusammenfassung
  4. SARAH validiert & updated INDEX
  5. Alle Agents haben Zugriff via INDEX
```

## Best Practices
- Keep Summaries DRY (Don't Repeat Yourself)
- Link zu Official Docs, nicht Content kopieren
- Focuse auf Impact & Action Items
- Tagsbasiert durchsuchbar halten
