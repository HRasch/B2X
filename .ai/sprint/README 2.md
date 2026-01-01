# Sprint / Iteration Documents

Naming convention: `SPR-[NUMBER]-[SHORT_NAME]` (e.g. `SPR-001-sprint-template`).

Purpose:
- Store sprint plans, iteration notes, retrospectives, and status summaries.

Guidelines:
- Use `SPR-` prefix for all sprint/iteration documents.
- Fill the YAML header with `docid`, `title`, `owner`, and `status`.
- Link back to relevant ADRs (`ADR-*`), Guidelines (`GL-*`) and Workflows (`WF-*`).

Templates:
- `SPR-001` is the canonical Sprint / Iteration Template. Copy it when creating a new sprint document and update the `docid` and `title`.

Storage:
- Place completed sprints under `.ai/sprint/` following the naming convention.
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
