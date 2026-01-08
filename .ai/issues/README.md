---
docid: UNKNOWN-159
title: README
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# Issue Collaboration

Zentrale Ablage für Issue-spezifische Zusammenarbeit.

## Verzeichnis-Struktur

```
.ai/issues/
├── README.md                    ← Du bist hier
└── {issue-id}/                  ← Pro Issue ein Verzeichnis
    ├── context.md               ← Issue-Kontext & Requirements
    ├── analysis.md              ← Agent-Analysen
    ├── decisions.md             ← Entscheidungen zum Issue
    ├── progress.md              ← Fortschritts-Tracking (Team → SARAH → GitHub)
    └── notes.md                 ← Arbeitsnotizen
```

## Issue-Verzeichnis erstellen

Bei neuem Issue:
```
.ai/issues/ISSUE-123/
├── context.md      # Was ist das Problem/Feature?
├── analysis.md     # Agent-Perspektiven
├── decisions.md    # Getroffene Entscheidungen
└── notes.md        # Laufende Notizen
```

## context.md Template

```markdown
# ISSUE-123: [Titel]

## Beschreibung
[Problem/Feature Beschreibung]

## Akzeptanzkriterien
- [ ] Kriterium 1
- [ ] Kriterium 2

## Betroffene Bereiche
- [ ] Backend
- [ ] Frontend
- [ ] Security
- [ ] DevOps

## Links
- GitHub Issue: #123
- Related: ISSUE-XXX
```

## analysis.md Template

```markdown
# Analysen für ISSUE-123

## @Backend
[Backend-Perspektive]

## @Frontend
[Frontend-Perspektive]

## @Security
[Security-Perspektive]

## Konsolidierung (@SARAH)
[Zusammenfassung & Empfehlung]
```

## Workflow

```
1. Issue erstellt → Verzeichnis anlegen
   @SARAH: "Erstelle Issue-Verzeichnis für ISSUE-123"

2. Kontext erfassen
   @ProductOwner: Füllt context.md

3. Analysen sammeln
   @Backend, @Frontend, etc.: Ergänzen analysis.md

4. Entscheidungen dokumentieren
   @TechLead/@SARAH: Dokumentiert in decisions.md

5. Arbeiten
   Alle: Nutzen notes.md für Koordination

6. Abschluss
   Issue-Verzeichnis bleibt als Dokumentation
```

## Namenskonvention

- GitHub Issues: `GH-123/`
- Interne Issues: `INT-123/`
- Bugs: `BUG-123/`
- Features: `FEAT-123/`
