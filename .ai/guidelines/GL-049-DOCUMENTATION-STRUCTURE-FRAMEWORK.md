---
docid: GL-085
title: GL 049 DOCUMENTATION STRUCTURE FRAMEWORK
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-049
title: Documentation Structure Framework - Clear Category Separation
owner: "@DocMaintainer"
status: Active
created: 2026-01-08
---

# Documentation Structure Framework

**DocID**: `GL-049`  
**Purpose**: Define clear separation between five document categories  
**Owner**: @DocMaintainer  
**Updated**: 2026-01-08

---

## 🎯 Five Core Categories

The B2X documentation is organized into five distinct categories, each with specific purposes, locations, and governance rules.

---

## 1. 📚 KNOWLEDGEBASE (KB)

**Purpose**: Reference material, technical deep-dives, best practices, tool guides

**Location**: `.ai/knowledgebase/`

**DocID Format**: `KB-###` (e.g., `KB-055-security-mcp-best-practices.md`)

**Characteristics**:
- Long-lived, reference material
- No expiration date
- Updated when technology/patterns change
- Searchable knowledge repository

**Subdirectories**:
```
.ai/knowledgebase/
├── architecture/          # KB-ARCH-*
├── patterns/              # KB-PAT-*
├── best-practices/        # KB-BP-*
├── tools-and-tech/        # KB-TOOL-*
├── dependency-updates/    # KB-DEP-*
└── operations/            # KB-OPS-*
```

**Examples**:
- [KB-055] Security MCP Best Practices
- [KB-006] Wolverine Patterns
- [KB-020] ArchUnitNET
- [KB-023] Email Template Best Practices

**Access**: Reference by DocID in code comments, PRs, ADRs

---

## 2. 📊 STATUS & DASHBOARDS

**Purpose**: Current state, metrics, health checks, execution tracking

**Location**: `.ai/status/` (new dedicated directory)

**DocID Format**: `STATUS-###` (e.g., `STATUS-PROJECT-CLEANUP-PHASE-2.md`)

**Characteristics**:
- Time-sensitive, regularly updated
- Shows current state at specific point in time
- Includes metrics, progress indicators
- Used for decision-making
- Archive after project milestone completion

**Types**:
- Project status reports
- Cleanup execution tracking
- CI/CD pipeline metrics
- Health dashboards
- Sprint progress

**Examples**:
- `STATUS-PROJECT-CLEANUP-PHASE-2.md` - Completion tracking
- `STATUS-REFACTOR-STRATEGY.md` - Refactoring progress
- `STATUS-API-COMPLIANCE-2026.md` - API audit results

**Retention**: Keep current phase active, archive previous phases

---

## 3. 🤝 COLLABORATION

**Purpose**: Joint work, shared context, team coordination

**Location**: `.ai/collaboration/`

**DocID Format**: `COLLAB-###` (e.g., `COLLAB-001-sprint-planning.md`)

**Characteristics**:
- Active collaboration documents
- Shared workspaces for multiple agents
- Discussion, decision-making, consensus
- Time-limited (until decision is made)

**Types**:
- Multi-agent analysis documents
- Code review discussions
- Planning sessions
- Team coordination notes
- Decision boards

**Examples**:
- `PROMPTS_INDEX.md` - Shared prompt reference
- Sprint planning breakdowns
- Multi-team architecture reviews
- Consolidated analysis (multiple agents' input)

**Retention**: Keep active collab docs, archive after decision; reference outcomes in ADR/REQ

---

## 4. 📖 DOCUMENTATION

**Purpose**: User guides, developer guides, system documentation

**Location**: `docs/`

**DocID Format**: `DOC-###` (e.g., `DOC-005-development-guide.md`)

**Characteristics**:
- Stable, maintained documentation
- Covers processes, architectures, systems
- For end users and developers
- Version-controlled alongside code

**Subdirectories**:
```
docs/
├── guides/                # User/Developer guides
├── api/                   # API documentation
├── architecture/          # System architecture docs
├── features/              # Feature documentation
└── user-guides/           # End-user documentation
```

**Examples**:
- [DOC-001] Quick Start Guide
- [DOC-005] Development Guide
- CONTRIBUTING.md
- Architecture decision explanations

**Governance**:
- Requires PR review
- Auto-generated from code (OpenAPI, etc.)
- Versioned with releases

---

## 5. 💡 PLANS / IDEAS (Brainstorm & Strategy)

**Purpose**: Future planning, strategic initiatives, brainstorming

**Location**: `.ai/brainstorm/`

**DocID Format**: `BS-###` or `PLAN-###` (e.g., `BS-REFACTOR-001`, `PLAN-PILOT-001`)

**Characteristics**:
- Not yet approved/decided
- Strategic thinking and exploration
- May spawn ADRs, REQs, or GLs
- Time-limited (until approved or rejected)

**Types**:
- Refactoring efficiency strategies
- Future architecture explorations
- Pilot programs and experiments
- Communication plans
- Long-term planning documents

**Examples**:
- [BS-PROJECT-CLEANLINESS] Project Cleanliness Strategy
- [BS-DOCUMENTATION-CLEANUP-STRATEGY] Documentation Cleanup
- [PLAN-PILOT-001] Pilot Refactoring Candidates

**Transition Path**:
- Brainstorm → Approved → Implementation
- BS-* → ADR-* (if architectural decision)
- BS-* → WF-* (if becomes workflow)
- BS-* → GL-* (if becomes guideline)
- BS-* → ARCHIVE (if rejected)

---

## 📍 Directory Structure (Post-Cleanup)

```
.ai/
├── knowledgebase/         # 📚 KB-* (permanent reference)
│   ├── architecture/
│   ├── patterns/
│   ├── best-practices/
│   ├── tools-and-tech/
│   ├── dependency-updates/
│   └── operations/
├── status/                # 📊 STATUS-* (current state, metrics)
├── collaboration/         # 🤝 COLLAB-* (joint work)
├── decisions/             # ⚙️ ADR-* (approved decisions)
├── guidelines/            # 📋 GL-* (enforced practices)
├── requirements/          # 📝 REQ-* (specifications)
├── brainstorm/            # 💡 BS-*, PLAN-* (ideas & strategy)
├── workflows/             # 🔄 WF-* (processes)
├── handovers/             # 📦 FH-* (feature completion)
├── compliance/            # ✅ CMP-* (compliance)
├── archive/               # 📦 Historical docs
└── logs/                  # 📋 Execution logs

docs/
├── guides/                # 📖 DOC-* (user/dev guides)
├── api/
├── architecture/
├── features/
└── user-guides/

.github/
├── instructions/          # INS-* (path-specific rules)
├── agents/                # AGT-* (agent definitions)
└── prompts/               # PRM-* (system prompts)
```

---

## 🎯 Category Comparison Matrix

| Aspect | KB | Status | Collaboration | Documentation | Plans/Ideas |
|--------|-----|--------|----------------|---------------|------------|
| **Purpose** | Reference | Metrics | Joint work | User/dev guides | Future thinking |
| **Location** | `.ai/knowledgebase/` | `.ai/status/` | `.ai/collaboration/` | `docs/` | `.ai/brainstorm/` |
| **DocID** | `KB-###` | `STATUS-###` | `COLLAB-###` | `DOC-###` | `BS-###, PLAN-###` |
| **Lifespan** | Permanent | Active project | Until decision | Maintained | Until approved |
| **Update Freq.** | Infrequent | Regular | Active/ongoing | As needed | As needed |
| **Archival** | Rarely | After milestone | After decision | Never | If rejected |
| **Audience** | Developers | Team | Collaborators | Users/devs | Strategy team |
| **Approved** | ✓ Yes | ✓ Yes | ◐ Partial | ✓ Yes | ✗ No (yet) |

---

## 🔄 Document Lifecycle

### Typical Journey:
```
💡 BRAINSTORM (BS-*)
    ↓
[Decision Point]
    ├→ APPROVED → 🎯 ADR-* or GL-* (permanent)
    ├→ REQUIRES WORK → 🤝 COLLABORATION
    └→ REJECTED → 📦 ARCHIVE
    
🤝 COLLABORATION (COLLAB-*)
    ↓
[Consensus Reached]
    ├→ APPROVED → Move to appropriate category
    └→ NEEDS REVISION → Stay in collaboration
    
📝 REQ-* (Requirements)
    ↓
[Implementation Complete] → FH-* (Handover)
    ↓
📖 DOC-* (Documentation)
    ↓
[Stable] → Maintained in docs/
```

---

## ✅ Migration Strategy (Post-Cleanup)

### Phase 1: Create Status Directory
- Move `STATUS-*` from `.ai/brainstorm/` → `.ai/status/`
- Example: `BS-PROJECT-CLEANUP-PHASE-2` → `STATUS-PROJECT-CLEANUP-PHASE-2`

### Phase 2: Identify Collaboration Docs
- Create `.ai/collaboration/` subdirectories for project-specific collabs
- Move multi-agent analysis docs there

### Phase 3: Update DOCUMENT_REGISTRY
- Add `STATUS` prefix registry
- Add `COLLAB` prefix registry
- Update location references

### Phase 4: Update GL-010
- [GL-010] Agent & Artifact Organization updated with new categories
- Clear ownership for each category

---

## 📋 Governance Rules by Category

### Knowledgebase (KB)
- **Review**: @TechLead validates accuracy
- **Approval**: GitHub Copilot maintains
- **Update**: When technology changes or best practices evolve
- **Archival**: Only if content becomes obsolete

### Status
- **Creation**: Authorized by @SARAH
- **Update Frequency**: Regular (weekly/daily depending on type)
- **Archival**: When project milestone completed
- **Retention**: Current + previous version only

### Collaboration
- **Creation**: Any agent can start
- **Review**: Requires consensus from collaborating agents
- **Conclusion**: Document decision outcome in appropriate category (ADR/REQ/GL)
- **Archival**: After decision made

### Documentation
- **Location**: `docs/` only
- **Review**: @TechLead, relevant domain experts
- **Approval**: Required for merge to main
- **Maintenance**: Ongoing, tied to code changes

### Plans/Ideas
- **Creation**: Any agent (usually @SARAH, @Architect)
- **Review**: Stakeholders per topic
- **Approval**: @SARAH gates advancement to implementation
- **Archival**: If rejected or superseded

---

## 🔗 Cross-Referencing

Use DocIDs consistently:

```markdown
**Knowledgebase Reference**:
See [KB-055] for security best practices

**Status Reference**:
Progress tracked in [STATUS-PROJECT-CLEANUP-PHASE-2]

**Collaboration Reference**:
Initial analysis in [COLLAB-001-sprint-planning]

**Documentation Reference**:
Details in [DOC-005] Development Guide

**Plans Reference**:
Strategy defined in [BS-REFACTOR-001]
```

---

## 📊 Success Metrics

- **Category Clarity**: 100% of docs can be categorized without ambiguity
- **Discoverability**: Docs found via DocID within 2 clicks
- **Zero Duplicates**: Across categories
- **Clear Paths**: Documents flow through lifecycle without confusion
- **Stakeholder Alignment**: Agents understand category purposes

---

## 🔗 Related Documents

- [GL-010] Agent & Artifact Organization
- [BS-PROJECT-CLEANLINESS] Project Cleanliness Strategy
- [BS-DOCUMENTATION-CLEANUP-STRATEGY] Documentation Cleanup Implementation
- [DOCUMENT_REGISTRY.md] Document Registry

---

**Effective Date**: 2026-01-08  
**Last Updated**: 2026-01-08  
**Next Review**: 2026-02-08 (after cleanup Phase 1 completion)