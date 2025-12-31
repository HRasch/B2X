# Sprint Coordination

Zentrale Ablage für Sprint-Planung und -Tracking.

## Verzeichnis-Struktur

```
.ai/sprint/
├── README.md                    ← Du bist hier
├── current.md                   ← Aktueller Sprint
├── backlog.md                   ← Product Backlog
└── archive/                     ← Abgeschlossene Sprints
    └── sprint-001.md
```

## Current Sprint File

**Location:** `.ai/sprint/current.md`

```markdown
# Sprint [Number] - [Name]

**Zeitraum:** [Start] - [Ende]
**Sprint Goal:** [Ziel in einem Satz]

## Sprint Backlog
| ID | Story | Points | Status | Owner |
|----|-------|--------|--------|-------|
| US-001 | ... | 3 | ✅ Done | @Backend |
| US-002 | ... | 5 | 🔄 In Progress | @Frontend |
| US-003 | ... | 2 | ⏳ To Do | - |

## Velocity
- Committed: [X] Points
- Completed: [Y] Points

## Impediments
- [ ] [Impediment 1] - @ScrumMaster

## Daily Notes
### [Date]
- [Update]
```

## Workflow

```
Sprint Planning:
  @ProductOwner → Priorisiert Backlog
  @ScrumMaster  → Erstellt current.md
  @DevelopmentTeam → Committed Stories

Daily:
  Agents → Updaten Status in current.md

Sprint Review:
  @ProductOwner → Review completed items

Sprint Retro:
  @ScrumMaster → Dokumentiert Learnings
  @SARAH → Konsolidiert in Guidelines
```

## Befehle

| Befehl | Agent | Aktion |
|--------|-------|--------|
| "Sprint starten" | @ScrumMaster | Erstellt current.md |
| "Backlog priorisieren" | @ProductOwner | Updated backlog.md |
| "Sprint Status" | @ScrumMaster | Zeigt current.md |
| "Sprint abschließen" | @ScrumMaster | Archiviert, neuer Sprint |
