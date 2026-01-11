---
docid: GL-054
title: Documentation Trust Strategy
owner: @SARAH
status: Active
created: 2026-01-11
---

# GL-054: Documentation Trust Strategy

**Purpose**: Prevent information inconsistency by establishing clear authority hierarchy, automated validation, and propagation rules for documentation.

---

## 1. Trust Hierarchy

Documentation follows a strict authority hierarchy. When conflicts exist, higher-authority sources win.

```
LEVEL 1 - AUTHORITATIVE (Source of Truth)
├── ADRs (Architecture Decision Records)
├── DOCUMENT_REGISTRY.md (DocID assignments)
├── copilot-instructions.md (Master agent behavior)
└── GOVERNANCE.md (Project governance)

LEVEL 2 - DERIVED (Must align with Level 1)
├── KB-* (Knowledgebase articles)
├── GL-* (Guidelines)
├── INS-* (Instructions)
└── WF-* (Workflows)

LEVEL 3 - GENERATED (Auto-update from Level 2)
├── INDEX.md files
├── Quick references
├── Summaries
└── Status dashboards
```

### Conflict Resolution Rules

| Conflict Type | Resolution |
|---------------|------------|
| ADR vs KB article | ADR wins, KB flagged for update |
| GL vs INS | GL wins (GL is policy, INS is implementation) |
| Multiple ADRs | Later ADR with `supersedes` field wins |
| Undocumented vs documented | Documented wins (no tribal knowledge) |

---

## 2. Document Metadata Requirements

### Required Fields (All Documents)

```yaml
---
docid: [PREFIX]-[NUMBER]
title: [Document Title]
owner: @[AgentName]
status: Active | Draft | Deprecated | Archived
created: YYYY-MM-DD
---
```

### Extended Fields (Level 2 Documents)

```yaml
---
docid: KB-054
title: Vue MCP Integration Guide
owner: @Frontend
status: Active
created: 2026-01-11
derives-from:
  - ADR-050  # TypeScript MCP Server decision
  - GL-012   # Frontend Quality Standards
last-verified: 2026-01-11
next-review: 2026-02-11
---
```

### Relationship Types

| Field | Purpose | Example |
|-------|---------|---------|
| `derives-from` | This doc implements/explains these sources | `[ADR-050, GL-012]` |
| `supersedes` | This doc replaces older doc | `ADR-015` |
| `superseded-by` | This doc was replaced | `ADR-029` |
| `related-to` | Related but not dependent | `[KB-053, KB-055]` |

---

## 3. Consistency Validation

### Automated Checks (CI/CD)

1. **DocID Resolution**: All `[DocID]` references must resolve to existing documents
2. **Staleness Detection**: Flag Level 2 docs when their `derives-from` sources change
3. **Orphan Detection**: Find docs without proper metadata
4. **Circular Reference Detection**: Prevent A derives-from B derives-from A

### Validation Script

```bash
# Run consistency check
pwsh scripts/validation/validate-doc-consistency.ps1

# Output:
# ✅ 156 DocID references validated
# ⚠️ 3 stale documents detected (source changed after last-verified)
# ❌ 2 broken references found
```

### PR Quality Gate

```yaml
# All PRs touching .ai/ or docs/ must pass:
- DocID link validation
- Metadata completeness check
- Staleness warning (non-blocking)
```

---

## 4. Change Propagation Workflow

### When AUTHORITATIVE Document Changes

```
ADR-029 Updated
       │
       ▼
┌──────────────────────────────────┐
│ CI: Find all docs with           │
│ derives-from: ADR-029            │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Create GitHub Issues:            │
│ "Review KB-054 - source ADR-029  │
│  was updated on 2026-01-11"      │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Assign to doc owner              │
│ Label: doc-consistency-review    │
└──────────────────────────────────┘
```

### Review Checklist

When reviewing derived docs after source change:

- [ ] Content still aligns with source decision
- [ ] Examples reflect current implementation
- [ ] Update `last-verified` date
- [ ] Update `next-review` date (+30 days)
- [ ] Close consistency review issue

---

## 5. Staleness Indicators

### Document Health Status

| Status | Criteria | Action |
|--------|----------|--------|
| 🟢 Current | `last-verified` within 30 days of source change | None |
| 🟡 Review Needed | Source changed after `last-verified` | Schedule review |
| 🔴 Stale | >60 days since source change, not reviewed | Urgent review |
| ⚫ Orphan | Missing `derives-from` or broken reference | Add metadata |

### Weekly Report

```markdown
## Documentation Consistency Report - Week 2, 2026

### Summary
- Total documents: 156
- 🟢 Current: 142 (91%)
- 🟡 Review Needed: 11 (7%)
- 🔴 Stale: 2 (1%)
- ⚫ Orphan: 1 (1%)

### Action Items
1. KB-054 - Source ADR-050 updated 2026-01-08
2. GL-043 - Source ADR-041 updated 2026-01-10
3. KB-063 - Missing derives-from metadata
```

---

## 6. @DocMaintainer Responsibilities

### Daily
- Monitor CI for consistency check failures
- Triage broken reference alerts

### Weekly
- Generate consistency report
- Create issues for stale documents
- Review orphan documents

### On Document Change
- Verify metadata completeness
- Update DOCUMENT_REGISTRY.md
- Check downstream impacts

### Monthly
- Full consistency audit
- Archive deprecated documents
- Update review schedules

---

## 7. Best Practices

### When Creating New Documents

1. **Check for existing coverage** - Don't duplicate
2. **Identify authoritative source** - What ADR/GL does this derive from?
3. **Add complete metadata** - All required + extended fields
4. **Register in DOCUMENT_REGISTRY** - Before PR merge

### When Updating Documents

1. **Check downstream dependencies** - Who derives from this?
2. **Update `last-verified`** - Proves review happened
3. **Notify dependent doc owners** - If significant change

### When Deprecating Documents

1. **Add `superseded-by`** - Point to replacement
2. **Update status** - `Deprecated` or `Archived`
3. **Update DOCUMENT_REGISTRY** - Mark status change
4. **Keep for history** - Don't delete, move to archive

---

## 8. Integration with Agent System

### Agent Responsibilities

| Agent | Trust Role |
|-------|------------|
| @SARAH | Enforces trust hierarchy, resolves conflicts |
| @DocMaintainer | Maintains consistency, generates reports |
| @Architect | Owns ADRs (Level 1 authority) |
| @TechLead | Owns GLs (Level 1 policy) |
| @CopilotExpert | Owns INS/AGT (Level 2 implementation) |

### Conflict Escalation

```
Conflict Detected
       │
       ▼
@DocMaintainer: Identify conflict
       │
       ▼
Document owners notified
       │
       ▼
If unresolved → @SARAH arbitration
       │
       ▼
Resolution documented in source ADR/GL
```

---

## References

- [DOCUMENT_REGISTRY.md](../DOCUMENT_REGISTRY.md) - DocID registry
- [GL-010](GL-010-AGENT-ARTIFACT-ORGANIZATION.md) - Agent & artifact organization
- [WF-010](../workflows/WF-010-DOCUMENTATION-MAINTENANCE.md) - Documentation maintenance workflow

---

**Maintained by**: @SARAH  
**Review Cycle**: Monthly  
**Last Updated**: 2026-01-11
