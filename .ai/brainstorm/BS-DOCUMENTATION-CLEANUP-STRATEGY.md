---
docid: BS-DOCUMENTATION-CLEANUP-STRATEGY
title: Documentation Cleanup Strategy
owner: "@DocMaintainer"
status: Brainstorm
created: 2026-01-08
---

# Documentation Cleanup Strategy

**DocID**: `BS-DOCUMENTATION-CLEANUP-STRATEGY`  
**Status**: Brainstorm Phase  
**Owner**: @DocMaintainer  
**Audience**: @SARAH, @TechLead, All Agents

---

## 🎯 Objective

Develop a systematic strategy to clean up, organize, and maintain the B2X project's documentation, ensuring it's current, accessible, and free of clutter while preserving historical value.

---

## 📊 Current State Assessment (Jan 8, 2026)

### Documentation Inventory
- **Total .md files**: ~1090
- **Root level docs**: ~25 (after recent cleanup)
- **.ai/ directory**: ~800+ files across subdirectories
- **docs/ directory**: ~200+ files
- **Backend docs**: ~50+ files
- **Frontend docs**: ~20+ files

### Identified Issues
1. **Duplicates**: Multiple versions of same documents (e.g., audit reports, analysis docs)
2. **Outdated content**: Legacy guides not updated for current architecture
3. **Poor organization**: Docs scattered across root, docs/, .ai/ without clear structure
4. **Missing DocIDs**: Many docs lack proper DocID references
5. **Archive gaps**: Historical docs not properly archived
6. **Inconsistent naming**: Mixed conventions (camelCase, kebab-case, spaces)

### Recent Cleanup Progress
- Root directory cleaned (Phase 1 of BS-PROJECT-CLEANLINESS completed)
- Archive system established (.ai/archive/)
- Basic file placement policy defined

---

## 🏗️ Cleanup Strategy Framework

### Phase 1: Documentation Audit (1-2 weeks)
**Goal**: Complete inventory and categorization of all documentation

#### Steps:
1. **Full Inventory Scan**
   - Use automated script to catalog all .md files
   - Record: path, size, last modified, DocID status
   - Output: `docs/audit/documentation-inventory-2026-01.csv`

2. **Content Categorization**
   - **Active**: Currently referenced/used
   - **Deprecated**: Superseded but may be referenced
   - **Archive**: Historical, no longer needed
   - **Duplicate**: Multiple versions of same content

3. **Quality Assessment**
   - DocID compliance (reference DOCUMENT_REGISTRY.md)
   - Link validity (broken internal/external links)
   - Content freshness (last updated vs. relevance)

#### Tools:
- Custom audit script: `scripts/docs-audit.sh`
- Link checker: `npm install -g markdown-link-check`
- Duplicate detector: `fdupes` or custom diff script

### Phase 2: Consolidation & Organization (2-3 weeks)
**Goal**: Eliminate duplicates, fix structure, update content

#### Steps:
1. **Duplicate Resolution**
   - Identify duplicate files (name patterns, content diff)
   - Consolidate to single source of truth
   - Update all references to point to consolidated version
   - Move duplicates to archive with redirect notes

2. **Structure Standardization**
   - Apply consistent naming: `PREFIX-NNN-short-description.md`
   - Move misplaced docs to correct directories
   - Update .ai/ subdirectories per GL-010 guidelines

3. **Content Updates**
   - Flag outdated docs for review/update
   - Add missing DocIDs to DOCUMENT_REGISTRY.md
   - Fix broken links and references

#### Directory Structure Target:
```
.ai/
├── knowledgebase/     # KB-*
├── guidelines/        # GL-*
├── decisions/         # ADR-*
├── requirements/      # REQ-*
├── workflows/         # WF-*
├── brainstorm/        # BS-*
├── sprint/           # SPR-*
├── handovers/        # FH-*
├── compliance/       # CMP-*
├── logs/             # Implementation logs
└── archive/          # Historical docs

docs/
├── guides/           # User/developer guides
├── api/              # API documentation
├── architecture/     # System architecture
├── features/         # Feature documentation
└── user-guides/      # End-user guides
```

### Phase 3: Automation & Maintenance (1-2 weeks)
**Goal**: Prevent future clutter with automated processes

#### Steps:
1. **Pre-commit Hooks**
   - Block commits with root-level .md files (except allowed list)
   - Require DocID for new docs in .ai/
   - Check for duplicate filenames

2. **CI/CD Integration**
   - Automated link checking on PRs
   - Documentation freshness monitoring
   - Archive rotation (90+ days → archive)

3. **Maintenance Scripts**
   - `scripts/docs-cleanup.sh`: Automated cleanup tasks
   - `scripts/docs-audit.sh`: Monthly audit reports
   - `scripts/docs-archive.sh`: Move old docs to archive

#### Automation Rules:
- Docs > 90 days unmodified → flag for review
- Broken links → create issues automatically
- Missing DocIDs → warn in PR checks

### Phase 4: Governance & Monitoring (Ongoing)
**Goal**: Ensure long-term compliance and continuous improvement

#### Steps:
1. **PR Template Updates**
   - Add documentation checklist
   - Require DocID for new docs
   - Link validation confirmation

2. **Review Process Integration**
   - Code reviews check documentation placement
   - Tech leads verify DocID compliance
   - @DocMaintainer audits quarterly

3. **Metrics & Reporting**
   - Monthly documentation health report
   - Track: total docs, duplicates, broken links, archive size
   - Dashboard in PROJECT_DASHBOARD.md

---

## 📋 Detailed Action Plan

### Immediate Actions (This Week)
- [ ] Create documentation audit script
- [ ] Run initial inventory scan
- [ ] Identify top 10 duplicate clusters
- [ ] Create BS-DOCUMENTATION-CLEANUP-STRATEGY.md (this doc)

### Short-term (1-2 weeks)
- [ ] Consolidate identified duplicates
- [ ] Fix critical broken links
- [ ] Add missing DocIDs for active docs
- [ ] Update CONTRIBUTING.md with documentation guidelines

### Medium-term (2-4 weeks)
- [ ] Implement pre-commit hooks
- [ ] Create maintenance scripts
- [ ] Update PR templates
- [ ] Train team on new processes

### Long-term (Ongoing)
- [ ] Monthly audit reports
- [ ] Quarterly archive cleanup
- [ ] Annual documentation review

---

## 🛠️ Required Tools & Scripts

### Audit Script (`scripts/docs-audit.sh`)
```bash
#!/bin/bash
# Generate comprehensive documentation inventory

OUTPUT="docs/audit/documentation-inventory-$(date +%Y-%m).csv"
echo "Path,Size,LastModified,DocID,Status" > "$OUTPUT"

find . -name "*.md" -type f | while read file; do
    size=$(stat -f%z "$file" 2>/dev/null || stat -c%s "$file")
    mtime=$(stat -f%Sm -t "%Y-%m-%d" "$file" 2>/dev/null || stat -c%y "$file" | cut -d' ' -f1)
    docid=$(grep -o "docid: [A-Z0-9-]*" "$file" | head -1 | cut -d' ' -f2)
    status="unknown"
    
    # Determine status based on path and content
    if [[ "$file" == *".ai/archive/"* ]]; then status="archived"
    elif [[ -z "$docid" ]]; then status="no-docid"
    else status="active"; fi
    
    echo "\"$file\",\"$size\",\"$mtime\",\"$docid\",\"$status\"" >> "$OUTPUT"
done

echo "Audit complete: $OUTPUT"
```

### Cleanup Script (`scripts/docs-cleanup.sh`)
```bash
#!/bin/bash
# Automated documentation cleanup

# Find docs > 90 days old
find .ai -name "*.md" -mtime +90 -not -path "*/archive/*" | while read file; do
    echo "Archiving: $file"
    mv "$file" ".ai/archive/$(basename "$file" .md)-archived.md"
done

# Find potential duplicates
find . -name "*.md" | sed 's|.*/||' | sort | uniq -d | while read dup; do
    echo "Duplicate basename: $dup"
    find . -name "$dup" -type f
done
```

### Pre-commit Hook (`.git/hooks/pre-commit`)
```bash
#!/bin/bash
# Prevent documentation policy violations

# Check for root-level .md files
ROOT_MD=$(git diff --cached --name-only | grep -E "^[^/]*\.md$")
ALLOWED="README\.md|QUICK_START_GUIDE\.md|CONTRIBUTING\.md|LICENSE|GOVERNANCE\.md|SECURITY\.md"

if echo "$ROOT_MD" | grep -v -E "$ALLOWED" >/dev/null; then
    echo "❌ Root-level .md files detected. Only these are allowed:"
    echo "$ALLOWED"
    echo "Files found: $ROOT_MD"
    exit 1
fi

# Check for missing DocIDs in .ai/
AI_MD=$(git diff --cached --name-only | grep "^\.ai/.*\.md$")
for file in $AI_MD; do
    if ! grep -q "docid:" "$file"; then
        echo "❌ Missing DocID in: $file"
        exit 1
    fi
done

echo "✅ Documentation checks passed"
```

---

## 📊 Success Metrics

| Metric | Target | Current | Tracking |
|---|---|---|---|
| Total .md files | <800 | ~1090 | Monthly audit |
| Docs with DocIDs | 100% | ~90% | PR checks |
| Broken links | 0 | Unknown | CI/CD |
| Duplicate files | 0 | Unknown | Audit script |
| Archive size | <100MB | ~2MB | Quarterly |

---

## 🚨 Risk Mitigation

### Potential Issues
1. **Accidental Deletion**: Always archive, never delete immediately
2. **Broken References**: Update all links when moving docs
3. **Content Loss**: Backup before bulk operations
4. **Team Resistance**: Communicate benefits, provide training

### Contingency Plans
- **Rollback**: Archive operations are reversible
- **Recovery**: Git history preserves all changes
- **Escalation**: @SARAH for policy conflicts
- **Gradual Rollout**: Phase implementation to minimize disruption

---

## 📞 Responsible Parties

| Role | Responsibility |
|---|---|
| @DocMaintainer | Lead audit, implement scripts, maintain registry |
| @SARAH | Coordinate phases, resolve conflicts, monitor compliance |
| @TechLead | Review technical docs, enforce in code reviews |
| @DevOps | Implement CI/CD integration, maintain scripts |
| All Agents | Follow documentation policies, update DocIDs |

---

## 🔗 Related Documents

- [BS-PROJECT-CLEANLINESS] Project Cleanliness Strategy
- [GL-010] Agent & Artifact Organization
- [DOCUMENT_REGISTRY.md] Document Registry
- [CONTRIBUTING.md] Contribution Guidelines

---

## 📝 Next Steps

1. **Immediate**: Create audit script and run initial scan
2. **Week 1**: Analyze results, identify priorities
3. **Week 2**: Begin consolidation of high-priority duplicates
4. **Week 3**: Implement automation and update processes
5. **Ongoing**: Monitor and maintain

---

**Brainstorm Completed**: 2026-01-08  
**Next Review**: 2026-01-15  
**Implementation Start**: 2026-01-09</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\brainstorm\BS-DOCUMENTATION-CLEANUP-STRATEGY.md