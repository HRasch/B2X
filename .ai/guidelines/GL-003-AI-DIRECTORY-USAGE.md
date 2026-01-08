---
docid: GL-062
title: GL 003 AI DIRECTORY USAGE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# .ai Directory Usage Guidelines

**Purpose:** Define what belongs in `.ai/` and what doesn't  
**Scope:** All content in `.ai/` directory  
**Principle:** `.ai/` is reserved for AI agent work only  
**Impact:** Token optimization, cleaner organization, faster context

---

## Core Principle

```
.ai/ directory = AGENT WORK ONLY

Everything in .ai/ must be:
✅ Created BY agents (automated)
✅ Used BY agents (in workflows/delegations)
✅ FOR agent decision-making
✅ Relevant to agent operations

Nothing else belongs here.
```

---

## What BELONGS in .ai/

### Essential Agent Content

**✅ Workflows & Processes**
- `.ai/workflows/` — Agent execution workflows
- `.ai/collaboration/` — Agent coordination patterns
- `.ai/processes/` — Operational processes

**✅ Decision Support**
- `.ai/decisions/` — Architecture Decision Records
- `.ai/analysis/` — Technical analysis for decision-making
- `.ai/research/` — Research supporting agent work

**✅ Requirements & Issue Coordination**
- `.ai/requirements/` — Agent coordination on requirements before issue creation
- `.ai/issues/{issue-id}/` — Agent analysis and coordination for specific issues
- `.ai/logs/` — Execution logs, performance metrics
- `.ai/status/` — Task completion tracking
- `.ai/permissions/` — Agent capability definitions
- `.ai/config/` — Agent configuration
- `.ai/knowledgebase/` — AI-optimized domain knowledge, patterns, best practices

### ✅ .ai/templates/

```
Purpose: GitHub issue templates used by agents for issue creation
Contains: GitHub issue, PR, feature request, bug report templates
Examples:
- github-issue.md
- github-pr.md
- github-feature-request.md
- github-bug.md

Owner: Agents creating issues/PRs
Updated: When template format changes
Usage: Agents reference when creating GitHub issues/PRs
```

**✅ Issue-Specific Work**
- `.ai/issues/{issue-id}/` — Agent analysis and coordination for specific issues
- `.ai/sprint/` — Sprint-specific agent tasks

---

## What DOES NOT Belong in .ai/

### ❌ Human-Only Content

```
DON'T PUT IN .ai/:
❌ Team documentation (use /docs/)
❌ User guides (use /docs/)
❌ Meeting notes (use /docs/ or separate folder)
❌ Blog posts (use /docs/blog/)
❌ Marketing content (use /marketing/)
❌ Legal documents (use /legal/)
❌ HR/Admin (use separate folder)
❌ Project planning (use /planning/ or tools)
❌ General tutorials (use /docs/learning/)
❌ Learning materials (use /docs/learning/)

NOTE: AI-optimized domain knowledge for decision-making DOES belong in .ai/knowledgebase/
NOTE: Requirements coordination DOES belong in .ai/requirements/ (before issue creation)
```

### ❌ Non-Critical Content

```
DON'T PUT IN .ai/:
❌ Archived/historical data (use /archive/)
❌ Backup files (use /backups/)
❌ Experimental code (use /experimental/)
❌ TODO lists (use tasks/issues)
❌ Random notes (use personal docs)
❌ Development scratchpad (use temp folder)
❌ Examples that aren't operational (use /examples/)
```

### ❌ Duplicate Content

```
DON'T PUT IN .ai/:
❌ Code duplicating /src/
❌ Docs duplicating /docs/
❌ Instructions duplicating .github/instructions/
❌ Prompts duplicating .github/prompts/
❌ Configs duplicating root config files
```

---

## Decision Tree: Does This Belong in .ai/?

```
┌─ Is this created/used BY agents?
│
├─ YES → Does it support agent decisions/work?
│       ├─ YES → Put in .ai/ ✅
│       └─ NO → This is human content → DON'T put in .ai/ ❌
│
└─ NO → Is this for human consumption?
        ├─ YES → Put in /docs/ or appropriate folder ❌
        └─ NO → Where should this go?
```

---

## Directory Structure: What Goes Where

### ✅ .ai/knowledgebase/

```
Purpose: AI-optimized domain knowledge for informed decision-making
Contains:
- Architecture patterns agents should know
- Best practices for agent decisions
- Domain knowledge about the system
- Design patterns and approaches
- Tools & technology stack decisions

Examples:
- architecture/ — Architectural patterns & decisions
- best-practices/ — Proven approaches for agent work
- domain-knowledge/ — System/domain expertise
- patterns/ — Design patterns, protocols
- tools-and-tech/ — Technology decisions agents need

Owner: Agents using for decision support
Updated: When new knowledge/patterns/decisions added
Usage: Agents load this context for informed decisions
```

### ✅ .ai/collaboration/

```
Purpose: Multi-agent coordination
Contains: How agents work together
Examples:
- AGENT_COORDINATION.md
- handoff-patterns.md
- escalation-procedures.md

Owner: Agents coordinating
Updated: When coordination changes
```

### ✅ .ai/guidelines/

```
Purpose: Rules for agent behavior
Contains: Operating guidelines, standards
Examples:
- COMMUNICATION-OVERVIEW.md
- AGENT-REMOVAL-GUIDE.md
- SECURITY-CHECKLIST.md

Owner: Agents following rules
Updated: When rules change
Note: This is operational, not tutorial
```

### ✅ .ai/decisions/

```
Purpose: Decision records for agents
Contains: ADRs, analysis, recommendations
Examples:
- ARCH-001-microservices-vs-monolithic.md
- TECH-045-node-upgrade-analysis.md
- DESIGN-089-database-migration.md

Owner: Agents making decisions
Updated: When decisions are made
```

### ✅ .ai/requirements/

```
Purpose: Requirements analysis and agent coordination BEFORE issue creation
Contains: Agent alignment on requirements, specifications, analysis
Examples:
- requirements.md
- feature-specifications.md
- coordination-notes.md

Owner: Agents coordinating on requirements
Updated: During requirements analysis phase
Lifecycle: Agents align → coordinate → GitHub issue created → FILE DELETED
Cleanup: Requirements file REMOVED once GitHub issue is created
Usage: Temporary coordination space until issue conversion
```

### ✅ .ai/issues/{issue-id}/

```
Purpose: Multi-agent coordination and analysis for specific issues
Contains: Issue-specific research, analysis, handoff documents
Examples:
- .ai/issues/FEAT-456/requirements.md
- .ai/issues/BUG-789/root-cause-analysis.md
- .ai/issues/ARCH-123/solution-design.md
- .ai/issues/FEAT-456/progress.md (team coordination)

Owner: Agents working on issues, coordinating between teams
Updated: During issue investigation and implementation
Usage: Agents sync work, document findings, coordinate handoffs
```

### ✅ .ai/logs/

```
Purpose: Execution logs and metrics
Contains: Performance data, audit trails
Examples:
- delegations-2025-12.md
- subagent-performance-week50.md
- deployment-log-2025-12-30.md

Owner: Agents recording work
Updated: During operations
```

### ❌ .ai/docs/

```
DON'T CREATE THIS
Human documentation goes in /docs/
Examples that might tempt you:
- General tutorials → /docs/learning/
- User guides → /docs/user-guide/
- Contributing guidelines → /docs/contributing/

NOTE: Domain knowledge for AI decision-making GOES IN .ai/knowledgebase/
```

### ❌ .ai/personal/

```
DON'T CREATE THIS
Personal notes/scratchpads don't belong
These should go:
- Personal workspace (outside repo)
- Temporary folder (if needed)
- Issue comments (for discussion)
```

### ✅ .ai/workflows/

```
Purpose: Agent execution workflows
Contains: Step-by-step process workflows
Examples:
- code-review.workflow.md
- feature-deployment.workflow.md
- security-audit.workflow.md

Owner: Agents executing these
Updated: During normal operations
```

---

## Content Guidelines by Type

### Decision Records (✅ BELONGS)

```markdown
# Decision Records in .ai/decisions/

These MUST be in .ai/:
- Architecture Decision Records (ADRs)
- Technical analysis supporting decisions
- Alternatives evaluated
- Recommendations from analysis

These should NOT be in .ai/:
- Meeting notes about decisions (→ /docs/)
- Historical context (→ /docs/history/)
- Stakeholder discussion (→ /docs/)
```

### Analysis & Research (✅ MAYBE)

```markdown
# Research in .ai/

Research BELONGS if:
✅ AI-optimized domain knowledge (architecture patterns, best practices)
✅ Done TO support agent decision-making
✅ Referenced in agent workflows
✅ Part of issue investigation
✅ Provides knowledge agents need for informed decisions

Research does NOT belong if:
❌ General human learning/understanding
❌ Educational purpose only
❌ Archival/historical
❌ Not referenced by any agent
→ Put in /docs/ instead
```

### Logs & Metrics (✅ BELONGS)

```markdown
# Logs in .ai/logs/

Logs BELONG if:
✅ Agent execution logs
✅ Delegation audit trails
✅ Performance metrics
✅ Operational data

Logs do NOT belong if:
❌ Development logs (→ /dev-logs/)
❌ Infrastructure logs (→ /infra-logs/)
❌ Historical archives (→ /archive/)
```

### Guidelines & Standards (✅ BELONGS)

```markdown
# Guidelines in .ai/guidelines/

Guidelines BELONG if:
✅ Rules for agent behavior
✅ Process standards for agents
✅ Agent communication patterns
✅ Agent decision frameworks

Guidelines do NOT belong if:
❌ User/team documentation (→ /docs/)
❌ Development standards (→ /docs/dev/)
❌ General best practices (→ /docs/best-practices/)
```

---

## KI-Optimierte Speicherung: Schreibrichtlinien für .ai/

**Kritisches Prinzip:** Jede Datei in `.ai/` wird in den Agent-Kontext geladen. Schreiben Sie präzise und zielorientiert.

### Speicherformat-Standards

**✅ DO: KI-optimierte Schreibweise**

```
Format als:
- Prägnante Markdown
- Strukturierte Sections (h2/h3 max)
- Bullet-Listen statt Absätze
- Tabellen für Daten
- Code-Beispiele (nur notwendig)
- Links zu Details (kein Copy-Paste)

Stil:
- Aktiv
- Keine Füllwörter
- Eine Idee pro Zeile
- Parallele Struktur
- Keine Wiederholung

Token-Ziele:
- Guidelines: 200-500 Tokens
- Decisions: 300-800 Tokens
- Analyze: 500-1000 Tokens
- Workflows: 400-1200 Tokens
```

**✅ DO: Markdown-Effizienz nutzen**

```markdown
EFFIZIENT:

# Decision: Datenbank-Wahl

## Kontext
- Skalierung: 1M Nutzer
- Read/Write: 100:1
- Konsistenz: Strong

## Optionen
| DB | Vorteile | Nachteile | Kosten |
|---|---|---|---|
| PostgreSQL | ACID, Scaling | Single-region | Niedrig |
| DynamoDB | Global, schnell | Limited queries | Mittel |

## Entscheidung
PostgreSQL (erfüllt alle Anforderungen, niedrigste Kosten)

---

INEFFIZIENT (Nicht machen):

# Datenbank-Technologie Entscheidung

Dieses Dokument behandelt die wichtige Entscheidung, die wir
bezüglich der Datenbanktechnologie treffen müssen. Es gibt viele
Überlegungen bei der Auswahl einer Datenbank, einschließlich
Performance, Kosten, Skalierbarkeit und Zuverlässigkeit.
Unser Team hat erhebliche Zeit damit verbracht, verschiedene 
Optionen zu recherchieren und zu evaluieren...

[100+ weitere Wörter]
```

### Speicherrichtlinien nach Content-Typ

**Guidelines & Standards**

```
Ziel: 200-400 Tokens
Format:
- Titel + Zweck (1 Zeile)
- Do's/Don'ts (Bullets)
- Beispiele (1-2)
- Decision Tree (einfach)

Struktur:
# Richtlinie-Name
Zweck: [1 Satz]
Do's: [3-5 Items]
Don'ts: [3-5 Items]
Beispiel: [1 Szenario]
Decision Tree: [Flowchart]
```

**Decisions (ADRs)**

```
Ziel: 300-600 Tokens
Format:
- Titel (Problem/Lösung)
- Kontext (3-5 Key Facts)
- Optionen (Tabelle)
- Entscheidung (Was + Warum)
- Aktion (Umsetzung)

Vermeiden: Lange Diskussionen, historischer Kontext, verworfene Alternativen
```

**Workflows**

```
Ziel: 500-1000 Tokens
Format:
- Titel (was es tut)
- Wann verwenden (Trigger)
- Schritte (nummeriert, klar)
- Ein-/Ausgaben
- Fehlerbehandlung
- Erfolgskriterien

Vermeiden: Tutorials, Background Info, Erklärungen
```

**Analyse & Research**

```
Ziel: 500-1000 Tokens
Format:
- Frage/Problem (1 Zeile)
- Kernerkenntnisse (3-5 Bullets)
- Empfehlung
- Implementierungsschritte
- Erfolgsmessungen

Vermeiden: Ausführliches Research-Prozess, verworfene Hypothesen
```

**Requirements**

```
Ziel: 200-500 Tokens pro Anforderung
Format:
- Titel (klar & spezifisch)
- User Story: "As [Rolle] I want [Aktion] so [Nutzen]"
- Acceptance Criteria (3-5 Items)
- Abhängigkeiten (wenn vorhanden)
- Notizen (non-blocking Info)

Vermeiden: Lange Beschreibungen, detaillierte Workflows
```

### Kompression-Techniken

**1. Tabellen statt Prosa**

```
AUSFÜHRLICH (100+ Tokens):
Die erste Option ist Option A. Option A hat mehrere Vorteile...
Die zweite Option ist Option B. Option B hat auch Vorteile...

EFFIZIENT (20 Tokens):
| Option | Vorteil | Nachteil | Kosten |
|--------|---------|----------|--------|
| A | [+] | [-] | [$$] |
| B | [+] | [-] | [$] |
```

**2. Strukturierte Listen**

```
AUSFÜHRLICH (50+ Tokens):
Wir müssen mehrere Faktoren beachten. Erst Performance, dann...

EFFIZIENT (15 Tokens):
Faktoren:
- Performance (1M Nutzer erforderlich)
- Kosten (Budget: 5k$/Monat)
- Team-Expertise (Go-Team)
```

**3. Link zu Details, keine Duplikation**

```
FALSCH (350 Tokens in .ai/ + Duplikat in /docs/):
.ai/decisions/ARCH-001.md (vollständige 300-Token Decision)
/docs/architecture/detailed-analysis.md (Duplikat)

RICHTIG (50 Tokens in .ai/ + Detail in /docs/):
.ai/decisions/ARCH-001.md:
  Entscheidung: PostgreSQL
  Warum: [1-2 Gründe]
  → Siehe /docs/architecture/db-analysis.md für Vollanalyse
```

**4. Standardinformation abkürzen**

```
AUSFÜHRLICH:
Dies ist eine Anforderung für das Authentifizierungssystem, 
die beschreibt...

EFFIZIENT:
REQ-AUTH-001: User-Login via Email/Passwort
- Acceptance: Gültige Email akzeptiert, ungültige abgelehnt
- Dependencies: Keine
```

**5. Code statt Beschreibung**

```
AUSFÜHRLICH (100+ Tokens):
Die Konfiguration hat mehrere Settings:
- Die Datenbank-URL Environment-Variable sollte...
- Das Timeout sollte auf... gesetzt sein

EFFIZIENT (30 Tokens):
# Config
```yaml
DB_URL: postgres://localhost
TIMEOUT: 30s
```
```

### KI-optimierte Templates

**Decision Record Template (300-400 Tokens)**

```markdown
# [Titel: Problemstellung]

**Kontext:** [3-5 Key Constraints]
**Entscheidung:** [Was wir gewählt haben]
**Warum:** [2-3 Kerngründe]

| Option | Vorteile | Nachteile | Fitness |
|--------|----------|-----------|---------|
| [A] | [+] | [-] | [%] |
| [B] | [+] | [-] | [%] |

**Aktion:** [Nächster Schritt]
```

**Guideline Template (200-300 Tokens)**

```markdown
# [Regelname]

**Wann:** [Trigger/Bedingung]
**Do's:**
- [Aktion 1]
- [Aktion 2]

**Don'ts:**
- [Anti-Pattern 1]
- [Anti-Pattern 2]

**Beispiel:** [1 Szenario]
```

**Workflow Template (400-600 Tokens)**

```markdown
# [Workflow-Name]

**Trigger:** [Wann ausführen]
**Owner:** @[Agent]

## Schritte
1. [Klare Aktion]
2. [Klare Aktion]
3. [Klare Aktion]

**Input:** [Erforderliche Daten]
**Output:** [Ergebnis-Format]
**Error:** [Wenn X, dann Y]
**Erfolg:** [Wie verifizieren]
```

### Qualitäts-Metriken

**Ziele für .ai/-Inhalte:**

```
Guideline:
- Länge: <400 Tokens ✅
- Klarheit: In <1 Min erfassbar ✅
- Actionable: Klarer nächster Schritt ✅

Decision:
- Länge: <600 Tokens ✅
- Vergleich: Tabellenformat ✅
- Begründung: 2+ Gründe ✅

Workflow:
- Länge: <1000 Tokens ✅
- Schritte: 5-15 klare Aktionen ✅
- Erfolg: Messbares Ergebnis ✅

Analysis:
- Länge: <1000 Tokens ✅
- Erkenntnisse: 3-5 Key Points ✅
- Empfehlung: Klarer nächster Schritt ✅
```

### Häufige Ineffizienzen vermeiden

**❌ Über-Dokumentation**

```
FALSCH:
---
title: Communication Guidelines
description: Dieses Dokument beschreibt, wie verschiedene Arten
von Agenten in unserem System miteinander kommunizieren. 
Kommunikation ist wichtig, weil sie es Agenten ermöglicht...

[2000 Tokens Narrative]

RICHTIG:
---
title: Agent Communication

Kanäle:
- Direkt: Einfache Tasks (<10 min)
- Routed: Komplex (via SARAH)
Details: COMMUNICATION-OVERVIEW.md
```

**❌ Redundante Sections**

```
FALSCH:
# Datenbank Decision

## Executive Summary
Wir mussten eine Datenbank wählen...

## Einleitung
Das folgende ist unsere Datenbank-Entscheidung...

## Background
Datenbanken sind wichtig für...

## Kontext
Unsere Anforderungen waren...

RICHTIG:
# Datenbank Decision
Kontext: 1M Nutzer, 100:1 Read/Write, Strong Consistency
Entscheidung: PostgreSQL
Warum: Erfüllt alle Anforderungen, niedrigste Kosten
```

**❌ Narrative statt Struktur**

```
FALSCH:
Während unserer Analyse fanden wir heraus, dass die erste Option,
die wir zuerst evaluiert haben, einige Vorteile hatte im Vergleich
zu den anderen Optionen, die wir auch evaluiert haben. Nach
sorgfältiger Überlegung der Faktoren entschieden wir...

RICHTIG:
| Option | Vorteile | Fitness |
|--------|----------|---------|
| A | [+] [+] | 95% |
| B | [+] | 70% |
Entscheidung: A
```

### Implementierungs-Checkliste

Vor Veröffentlichung von Content in `.ai/`:

```
□ Ist dies die prägnanteste Form?
  - Könnte es eine Tabelle sein?
  - Könnten Narrative Bullets sein?
  - Kann es kürzer sein?

□ Dupliziert es etwas?
  - Ist dies auch in /docs/?
  - Können wir linken statt duplizieren?
  - Längere Version löschen?

□ Ist es optimal strukturiert?
  - Header klar (h2/h3)?
  - Sections scanbar?
  - Key Info zuerst?

□ Gehört es in .ai/?
  - Von Agenten verwendet? JA/NEIN
  - Essentiell für Entscheidungen? JA/NEIN
  - In Kontext geladen? JA/NEIN

□ Token schätzen (~4 chars pro Token)
  - Guideline: <400 Tokens ✅
  - Decision: <600 Tokens ✅
  - Workflow: <1000 Tokens ✅
  - Analysis: <1000 Tokens ✅

Alle JA → Ready für .ai/ ✅
```

---

## Token Optimization Strategy

### Why This Matters

```
.ai/ content is frequently loaded into agent context

Context size:
- Small .ai/: Faster context loading, more room for code
- Bloated .ai/: Wastes tokens, slower execution, less context

Example:
- Keeping 1 MB non-essential docs in .ai/
- = ~250 tokens per agent execution
- 100 executions/day
- = 25,000 wasted tokens/day
- = 750,000 tokens/month
```

### Optimization Rules

**1. Keep .ai/ Lean**
```
Target: Only agent-essential content
Audit quarterly:
- Is this still used by agents?
- Is this supporting decisions?
- Can this move to /docs/?
```

**2. Archive Aggressively**
```
Move to /archive/ if:
- Not used in 30+ days
- Obsolete (decisions made, analysis done)
- Historical/reference only
- Closed issues (move to /archive/issues/)
- Converted requirements (DELETE after GitHub issue created)
```

**3. Reference, Don't Duplicate**
```
Good:
├─ .ai/decisions/ARCH-001.md (decision)
└─ Links to /docs/architecture/ for context

Bad:
├─ .ai/decisions/ARCH-001.md
├─ .ai/reference/architecture-overview.md
├─ .ai/docs/architecture-guide.md
└─ .ai/learning/architecture-tutorial.md
```

**4. Compress & Consolidate**
```
Instead of:
- .ai/logs/delegation-1.md
- .ai/logs/delegation-2.md
- .ai/logs/delegation-3.md
(100 files)

Use:
- .ai/logs/delegations-2025-12.md
(1 file with summaries)
```

---

## Content Checklist

Before putting something in `.ai/`, ask:

```
□ Is this created/used BY agents?
  If NO → Don't put in .ai/

□ Does this support agent decision-making?
  If NO → Don't put in .ai/

□ Is this referenced in agent workflows?
  If NO → Consider if it belongs

□ Will agents need this in their context?
  If NO → Use /docs/ instead

□ Is this the only/best place for this?
  If NO → Move to appropriate folder

□ Will this still be relevant in 30 days?
  If NO → Don't create it in .ai/

Special cases:
□ Is this a requirement? → DELETE when GitHub issue created
□ Is this analysis done? → Archive after decision made
□ Is this issue closed? → Archive to /archive/issues/

All YES → OK to put in .ai/ ✅
```

---

## Common Mistakes

### ❌ Mistake 1: Dumping Everything in .ai/

```
WRONG:
.ai/
├── guidelines/
│   ├── COMMUNICATION-OVERVIEW.md (✅ OK)
│   ├── how-to-use-slack.md (❌ NO)
│   ├── team-processes.md (❌ NO)
│   └── AGENT-REMOVAL-GUIDE.md (✅ OK)

RIGHT:
.ai/
├── guidelines/
│   ├── COMMUNICATION-OVERVIEW.md (✅ Agent communication)
│   └── AGENT-REMOVAL-GUIDE.md (✅ Agent operation)

/docs/
├── team/
│   ├── how-to-use-slack.md
│   └── team-processes.md
```

### ❌ Mistake 2: Keeping Converted Requirements in .ai/requirements/

```
WRONG:
.ai/requirements/
├── feature-auth.md (GitHub issue #123 created)
├── bug-cache.md (GitHub issue #456 created)
├── feature-api.md (GitHub issue #789 created)
└── ... (requirements for issues already created)

These files waste space and tokens!

RIGHT:
.ai/requirements/
├── feature-notifications.md (still being coordinated)
└── enhancement-performance.md (still being analyzed)

When issue is created:
1. GitHub issue created (#456: Bug: Cache performance)
2. .ai/requirements/bug-cache.md → DELETED
3. Work continues in .ai/issues/BUG-456/
```

### ❌ Mistake 3: Keeping Archived Data in .ai/

```
WRONG:
.ai/issues/
├── FEAT-123/ (active issue)
│   └── analysis.md
├── FEAT-456/ (closed 3 months ago)
│   └── analysis.md
├── BUG-789/ (closed 6 months ago)
│   └── analysis.md
└── ... (30 more closed issues)

RIGHT:
.ai/issues/
└── FEAT-123/ (only active)
    └── analysis.md

/archive/
└── issues/
    ├── FEAT-456/
    └── BUG-789/
```

### ❌ Mistake 4: Duplicating Content

```
WRONG:
.github/
└── instructions/
    └── backend.instructions.md

.ai/
└── guidelines/
    └── backend-guide.md (same content!)

/docs/
└── backend/
    └── backend-overview.md (same again!)

RIGHT:
.github/
└── instructions/
    └── backend.instructions.md (source of truth)

.ai/
└── [nothing, agents reference .github/instructions/]

/docs/
└── backend/
    └── backend-overview.md (different: for users)
```

---

## Migration Guide: Cleaning Up .ai/

If you have non-essential content in `.ai/`:

### Step 1: Audit

```
□ List all files in .ai/
□ For each file:
  - Is this used by agents? (YES/NO)
  - Is this essential? (YES/NO)
  - When was it last updated? (Date)
  - Where should it go? (Location)
```

### Step 2: Categorize

```
KEEP (Agent-essential):
- .ai/guidelines/COMMUNICATION-OVERVIEW.md
- .ai/workflows/code-review.workflow.md
- .ai/decisions/ARCH-001.md
- .ai/requirements/active-requirements.md

DELETE (Converted to Issues):
- .ai/requirements/finished-feature.md (GitHub issue #456 created)
- .ai/requirements/resolved-bug.md (GitHub issue #789 created)

MOVE (Not essential):
- Team docs → /docs/
- User guides → /docs/
- Learning materials → /docs/learning/

ARCHIVE (Obsolete):
- Closed issue analysis → /archive/issues/
- Old logs → /archive/logs/
- Historic decisions → /archive/decisions/
```

### Step 3: Execute Migration

```
1. Create target folders (/docs/, /archive/)
2. Move files to new locations
3. Update all cross-references
4. Delete from .ai/
5. Verify nothing broken
6. Document changes
```

---

## What Belongs Where: Quick Reference

| Content | .ai/ | /docs/ | .github/ | Other |
|---------|------|--------|----------|-------|
| **Agent workflow** | ✅ | ❌ | ❌ | ❌ |
| **Agent guideline** | ✅ | ❌ | ❌ | ❌ |
| **Decision record** | ✅ | (link) | ❌ | ❌ |
| **Requirements coordination** | ✅ | ❌ | ❌ | ❌ |
| **Issue analysis & coordination** | ✅ | ❌ | ❌ | ❌ |
| **GitHub issue templates** | ✅ | ❌ | ❌ | ❌ |
| **Domain knowledge (AI)** | ✅ | ❌ | ❌ | ❌ |
| **Design patterns (AI)** | ✅ | ❌ | ❌ | ❌ |
| **Issue analysis** | ✅ | ❌ | ❌ | ❌ |
| **Execution log** | ✅ | ❌ | ❌ | ❌ |
| **Team process** | ❌ | ✅ | ❌ | ❌ |
| **User guide** | ❌ | ✅ | ❌ | ❌ |
| **Learning material** | ❌ | ✅ | ❌ | ❌ |
| **Code instructions** | ❌ | ❌ | ✅ | ❌ |
| **API docs** | ❌ | ✅ | ❌ | /docs/api/ |
| **Archived data** | ❌ | ❌ | ❌ | /archive/ |

---

## Review Checklist

Use this monthly to keep `.ai/` clean:

```
Monthly .ai/ Review:
□ Any files >30 days old and not referenced?
□ Any requirements with GitHub issues created? (DELETE)
□ Any completed issues still in .ai/issues/? (Archive)
□ Any duplicated content in .ai/ and /docs/?
□ Any human-only content in .ai/?
□ Any archived issues still in .ai/issues/?
□ Total .ai/ size reasonable? (< 50 MB is good)
□ Any .ai/ subdirectories unused?
□ All active decisions in .ai/decisions/?
□ Recent logs archived if > 30 days?
```

---

## Implementation

**Effective immediately:** This guideline applies to all new content.

**Retroactive:** Existing content will be audited and migrated quarterly.

**Responsibility:** @SARAH ensures compliance during content creation/delegation.

---

## Summary

```
PRINCIPLE: .ai/ = AGENT WORK ONLY

✅ PUT IN .ai/:
- Workflows agents execute
- Guidelines agents follow
- Decisions agents make
- Analysis supporting decisions
- Operational logs/metrics
- Issue-specific work

❌ DON'T PUT IN .ai/:
- Team/user documentation
- Learning materials
- Meeting notes
- Marketing/legal content
- Duplicated content
- Archived/obsolete data
- Non-agent-relevant content

BENEFIT: Token savings, cleaner org, better performance
```

---

## Related Documents

- [.github/copilot-instructions.md](../../.github/copilot-instructions.md) — Copilot guidelines
- [.ai/guidelines/](../guidelines/) — All agent guidelines
- [COMMUNICATION-OVERVIEW.md](COMMUNICATION-OVERVIEW.md) — Agent communication

---

**Created:** 30.12.2025  
**Owner:** @SARAH  
**Status:** ✅ ACTIVE

Token savings through smart organization! 🚀
