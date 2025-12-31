# Agent Completion Protocol

## Zweck
Agenten melden SARAH nach Abschluss einer Operation den Status. Ermöglicht koordinierte Folgeoperationen.

## Realistisches Modell (VS Code Copilot)

```
⚠️ Keine direkte Agent-zu-Agent Kommunikation möglich!

Workflow:
1. User aktiviert Agent → Agent führt aus
2. Agent schreibt Completion in .ai/status/
3. User wechselt zu @SARAH
4. SARAH liest Status-Files → koordiniert nächste Schritte
```

## Completion Status File

**Location:** `.ai/status/current-task.md`

**Format:**
```markdown
# Current Task Status

## Letzte Completion
- **Agent:** @Backend
- **Operation:** API für User-Authentication implementiert
- **Zeit:** 2025-12-30 14:30
- **Status:** ✅ Complete
- **Artifacts:**
  - `src/api/auth.ts` (neu)
  - `src/services/authService.ts` (geändert)

## Pending Actions
- [ ] @Frontend: Auth-Integration
- [ ] @Security: Auth-Review
- [ ] @QA: Test-Cases erstellen

## Blocked
- (keine)
```

## Agent Completion Template

Nach jeder Operation fügt der Agent hinzu:

```markdown
## [Agent] Completion - [Timestamp]
**Operation:** [Was wurde gemacht]
**Status:** ✅ Complete | ⚠️ Partial | ❌ Blocked
**Files:** [Geänderte/Erstellte Files]
**Next:** [Empfohlene Folgeaktion für SARAH]
**Blocker:** [Falls vorhanden]
```

## SARAH Coordination Response

SARAH liest Completions und antwortet mit:

```markdown
## SARAH Coordination - [Timestamp]
**Received:** [Agent] completion für [Operation]
**Next Steps:**
1. @[Agent]: [Nächste Aufgabe]
2. @[Agent]: [Parallele Aufgabe]
**Priority:** [High/Medium/Low]
```

## Workflow-Beispiel

```
┌─────────────────────────────────────────────────────────┐
│ 1. User → @Backend: "Implementiere Auth-API"            │
│    Backend: Implementiert, schreibt Status-File         │
│                                                         │
│ 2. User → @SARAH: "Was ist der nächste Schritt?"        │
│    SARAH: Liest .ai/status/, empfiehlt:                 │
│    → @Security für Auth-Review                          │
│    → @Frontend für Integration (parallel möglich)       │
│                                                         │
│ 3. User → @Security: "Review Auth-API"                  │
│    Security: Reviewed, schreibt Completion              │
│                                                         │
│ 4. User → @SARAH: "Status?"                             │
│    SARAH: Security ✅, empfiehlt @QA für Tests          │
└─────────────────────────────────────────────────────────┘
```

## Quick Completion (Chat)

Für einfache Operationen - Agent endet mit:

```
✅ **Done:** [Operation]
📁 **Files:** [file1], [file2]
➡️ **Next:** @[Agent] für [Task]
```

SARAH kann darauf direkt im Chat reagieren.

## Status-Tracking Commands

User kann SARAH fragen:
- "Was ist der aktuelle Status?"
- "Welche Operationen sind pending?"
- "Was ist der nächste Schritt?"
- "Wer ist blockiert?"

SARAH liest `.ai/status/` und gibt Übersicht.
