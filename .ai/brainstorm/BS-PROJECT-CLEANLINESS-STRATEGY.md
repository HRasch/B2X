---
docid: BS-PROJECT-CLEANLINESS
title: Project Cleanliness & Long-Term Organization Strategy
owner: "@SARAH"
status: Active
created: 2026-01-07
---

# Project Cleanliness & Long-Term Organization Strategy

**DocID**: `BS-PROJECT-CLEANLINESS`  
**Status**: Implementation Phase  
**Owner**: @SARAH (Coordinator)  
**Audience**: All agents, Tech Leadership

---

## 🎯 Objective

Establish systematic practices to keep the B2X project clean, well-organized, and maintainable while preventing recurring clutter and technical debt.

---

## 📊 Current State (Jan 7, 2026)

### What Was Cleaned Up
- **Duplicates**: `REQ-007-*` analysis files (7 duplicates) → archived
- **Temp files**: `analyze-any-types.js`, `audit-script.js` → archived
- **Test files**: `TestXsdValidation.*`, `*-erp-*.json` → archived
- **Logs**: `rename-log-*.txt` (3 versions) → removed
- **Backups**: `rename-backup-*` directory → removed
- **Utilities**: `outdated.json`, `updates.txt` → removed
- **Old analysis**: `PHASE_*_RESULTS.md` (5 files) → archived
- **Audit reports**: `SECURITY_AUDIT_REPORT.md` (3 versions) → archived

### Result
- Root directory reduced from ~45 miscellaneous files to ~25
- Created `.ai/archive/` for historical documents
- Established baseline for cleanliness

---

## 🏗️ Root-Level File Policy

### ✅ What Belongs at Root
- `README.md` - Project overview
- `QUICK_START_GUIDE.md` - Quick reference
- `CONTRIBUTING.md` - Contribution guidelines
- `LICENSE` - Legal
- `GOVERNANCE.md` - Governance overview
- `SECURITY.md` - Security policy
- `B2X.slnx` - Solution file
- `Directory.Packages.props` - Dependency config
- `docker-compose.yml` - Local development
- `package.json` - Monorepo root config

### ❌ What Does NOT Belong at Root
- **Analysis documents** → `.ai/requirements/` or `.ai/decisions/`
- **Implementation logs** → `.ai/logs/`
- **Phase reports** → `.ai/brainstorm/` or `.ai/archive/`
- **Temporary/test files** → `.gitignore` + remove immediately
- **Duplicates** → consolidate or archive
- **Old audit reports** → `.ai/archive/` after review completion

### 📍 Directory Mapping
| Content Type | Location | Naming |
|---|---|---|
| Requirements specs | `.ai/requirements/` | `REQ-###-*.md` |
| Analysis documents | `.ai/brainstorm/` | `ANALYSIS-*.md` |
| Architecture decisions | `.ai/decisions/` | `ADR-###-*.md` |
| Implementation logs | `.ai/logs/` | `YYYY-MM-DD-*.md` |
| Historical/archived | `.ai/archive/` | `[original-name]-archived.md` |
| Knowledge base | `.ai/knowledgebase/` | `KB-###-*.md` |
| Workflows | `.ai/workflows/` | `WF-###-*.md` |
| Guidelines | `.ai/guidelines/` | `GL-###-*.md` |
| Sprint tracking | `.ai/sprint/` | `SPR-###-*.md` |

---

## 🔄 Process Rules

### 1. **Duplicate Prevention**
- Use DocID system for all documents (see [DOCUMENT_REGISTRY.md](../../.ai/DOCUMENT_REGISTRY.md))
- If same doc exists in multiple locations, consolidate to single location
- Example: `REQ-007-backend-analysis.md` & `REQ-007-backend-analysis 2.md` → consolidate to `.ai/requirements/REQ-007-email-wysiwyg-builder.md`

### 2. **Temp File Management**
- All temp/test files must be in `.gitignore`
- If created during development, move to `.ai/archive/` after use
- Never commit temp files to main branches
- Use naming: `temp-*.json`, `test-*.js`, `sample-*.data`

### 3. **Log & Backup Rotation**
- Build logs → `.ai/logs/ci-cd/` (auto-cleanup after 30 days)
- Rename/migration logs → remove immediately after completion
- Database backups → outside repo, reference only in docs
- Keep only last 3 versions of deployment logs

### 4. **Analysis Document Lifecycle**
| Phase | Location | Retention |
|---|---|---|
| In-progress analysis | `.ai/brainstorm/` | Until completed |
| Completed analysis | Move to `.ai/requirements/` or `.ai/decisions/` | Active |
| Old/superseded | Move to `.ai/archive/` | 6 months, then remove |

### 5. **Archive Policy**
- Documents > 90 days old without recent updates → `.ai/archive/`
- Keep README in archive explaining what's in it
- Review archive quarterly for deletion candidates
- Example: old `PHASE_3_RESULTS.md` → `archive/PHASE_3_RESULTS-ARCHIVED.md`

---

## ✅ Governance Checkpoints

### Per PR
- [ ] No root-level `.md` files added unless justified in description
- [ ] No duplicate files committed
- [ ] Temp files removed before merge
- [ ] Old analysis docs moved to `.ai/` if referenced

### Per Release
- [ ] Audit `.ai/logs/` for old build logs → cleanup
- [ ] Review `.ai/archive/` for deletion candidates
- [ ] Verify all active docs have valid DocIDs
- [ ] Update [DOCUMENT_REGISTRY.md](../../.ai/DOCUMENT_REGISTRY.md) for new docs

### Weekly (Automated)
- Run archival script to move docs > 90 days old
- Report archive size to @SARAH
- Flag any duplicate filenames

---

## 🛠️ Tooling & Automation

### Pre-commit Hook (Proposed)
```bash
#!/bin/bash
# Prevent temp files and duplicates from being committed
ROOT_FILES=$(git diff --cached --name-only | grep -E "^[^/]*\.(md|json|js|ps1)$")

if [[ ! -z "$ROOT_FILES" ]]; then
  echo "⚠️ Warning: Root-level files detected. Please verify:"
  echo "$ROOT_FILES"
  echo "Allowed: README.md, QUICK_START_GUIDE.md, CONTRIBUTING.md, SECURITY.md, GOVERNANCE.md"
  exit 1
fi
```

### Archival Script (Proposed)
```bash
#!/bin/bash
# Archive docs > 90 days old
find .ai -type f -name "*.md" -mtime +90 | while read file; do
  mv "$file" ".ai/archive/$(basename $file)-archived.md"
done
```

### Duplicate Detection (Proposed)
```bash
# Find files with number suffixes (indicates duplicate)
find . -type f -name "* [0-9].md" -o -name "* [0-9].json"
```

---

## 📋 Implementation Plan

### Phase 1: Immediate Cleanup ✅ DONE
- [x] Move PHASE_* reports to archive
- [x] Move REQ-007 analysis files to archive
- [x] Move audit reports to archive
- [x] Delete temp files (rename logs, backups)
- [x] Remove obsolete utilities (audit-script.js, etc.)

### Phase 2: Documentation (This Week)
- [ ] Create `.ai/archive/README.md` documenting archive contents
- [ ] Update CONTRIBUTING.md with file organization guidelines
- [ ] Add section to [GL-010] Agent & Artifact Organization
- [ ] Create this strategy doc (BS-PROJECT-CLEANLINESS)

### Phase 3: Automation (Next Week)
- [ ] Implement pre-commit hook for root file validation
- [ ] Create archive script in `scripts/`
- [ ] Add CI job for duplicate detection
- [ ] Setup GitHub issue template for cleanup tasks

### Phase 4: Enforcement (Ongoing)
- [ ] Add checklist to PR template
- [ ] Update code review guidelines to check file placement
- [ ] Monthly audit reports in sprint retrospectives

---

## 🎯 Success Metrics

| Metric | Target | Current |
|---|---|---|
| Root-level `.md` files | <15 | 12 ✅ |
| Archive storage used | <50MB | ~2MB ✅ |
| Duplicate files | 0 | 0 ✅ |
| Docs with DocIDs | 100% | ~90% |
| Archive > 90 days old | 0 | 0 ✅ |

---

## 🚨 Anti-Patterns to Avoid

### ❌ What Broke Before
```
Root directory with:
- REQ-007-backend-analysis.md
- REQ-007-backend-analysis 2.md          ← Duplicate!
- SECURITY_AUDIT_REPORT.md
- SECURITY_AUDIT_REPORT 2.md
- SECURITY_AUDIT_REPORT 3.md              ← Version hell
- analyze-any-types.js                    ← Temp file at root
- PHASE_3_RESULTS_FIRST_BATCH.md          ← Old report
- rename-log-20260107-214238.txt           ← Build artifact
```

### ✅ How We Fix It
```
Organized structure:
.ai/
  requirements/
    REQ-007-email-wysiwyg-builder.md       ← Single source of truth
  brainstorm/
    ANALYSIS-REQ-007-consolidated.md
  archive/
    PHASE_3_RESULTS_FIRST_BATCH-archived.md ← Historical, cleaned up
    SECURITY_AUDIT_REPORT-archived.md
  logs/
    ci-cd/
      2026-01-07-build-results.json        ← Dated, auto-rotated
```

---

## 📞 Responsible Parties

| Role | Responsibility |
|---|---|
| @TechLead | Enforce in code reviews |
| @SARAH | Monitor compliance, escalate issues |
| @DevOps | Implement automation scripts |
| All Agents | Follow file placement policy |

---

## 🔗 Related Documents

- [GL-010] Agent & Artifact Organization
- [GL-006] Token Optimization Strategy
- [DOCUMENT_REGISTRY.md](../../.ai/DOCUMENT_REGISTRY.md)
- [CONTRIBUTING.md](../../CONTRIBUTING.md)

---

**Next Review**: 2026-02-07 (30 days)  
**Last Updated**: 2026-01-07
