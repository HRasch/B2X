---
docid: WF-010
title: Documentation Maintenance & Quality Workflow
category: Workflows
type: Process
status: Active
created: 2026-01-08
owner: "@DocMaintainer"
---

# WF-010: Documentation Maintenance & Quality Workflow

**Purpose**: Regular maintenance of documentation system to ensure quality, compliance, and organization

**Owner**: @DocMaintainer  
**Frequency**: Daily (registry), Weekly (audits), Monthly (compliance), Quarterly (strategy)

---

## 📅 Maintenance Schedule

### Daily: Registry Update
**When**: Every commit that touches `.ai/` or `.github/`  
**Owner**: Automated (pre-commit hook) + @DocMaintainer

```
Process:
1. Pre-commit hook runs: docs-validation.sh
2. Validates:
   ✓ DocID format correct (PREFIX-NNN)
   ✓ No duplicate DocIDs
   ✓ YAML metadata complete
   ✓ Frontmatter valid
   ✓ Registry entry exists
3. If validation fails → Commit blocked
4. If validation passes → Commit proceeds
5. @DocMaintainer reviews in batch (end of day)
   - Confirms new DocIDs are appropriate
   - Verifies categorization correct
   - Notes any issues for weekly review
```

**Time**: ~5 minutes per 10 commits

---

### Weekly: Link & Metadata Audit
**When**: Every Monday 9 AM CET  
**Owner**: Automated (CI/CD) + @DocMaintainer

#### Step 1: Automated Link Check (Monday 9 AM)
```bash
# .github/workflows/docs-audit.yml - weekly-link-check
markdown-link-check on all .md files in:
  - .ai/
  - .github/
  - docs/

Output: Report of broken links (if any)
Sent to: @DocMaintainer via GitHub issue
```

#### Step 2: Metadata Validation (Monday 9 AM)
```bash
# .github/workflows/docs-audit.yml - weekly-metadata-check
Validate all docs have:
  ✓ docid: field
  ✓ title: field
  ✓ status: field
  ✓ created: field (YYYY-MM-DD)
  ✓ owner: field

Output: List of docs missing fields
Sent to: @DocMaintainer via GitHub issue
```

#### Step 3: @DocMaintainer Reviews (Monday 10-11 AM)
```
1. Review link checker report
   - For each broken link:
     a. Determine root cause (moved file? typo? dead external?)
     b. If internal: Update reference or notify document owner
     c. If external: Update or remove link
     d. Create issue if needed (assign to document author)

2. Review metadata violations
   - For each incomplete doc:
     a. Add missing metadata if obvious
     b. If not obvious: Notify document author
     c. Create issue for author to fix

3. Identify stale docs
   - Find docs older than 6 months without updates
   - Verify they're still current with owner
   - Archive if no longer maintained

4. Generate weekly summary
   - X broken links fixed
   - X metadata issues resolved
   - X stale docs reviewed
   - Status: Green / Yellow / Red
   - Post summary to Slack/GitHub (if issues exist)
```

**Time**: ~30 minutes

---

### Monthly: Compliance & Segregation Audit
**When**: First Monday of month, 9 AM CET  
**Owner**: Automated (CI/CD) + @DocMaintainer

#### Step 1: Segregation Validator (Auto)
```bash
# .github/workflows/docs-audit.yml - monthly-segregation-check
Check all [USERDOC-*] docs:
  ✓ USERDOC-STORE-* files do NOT link to USERDOC-ADMIN-* or USERDOC-MGMT-*
  ✓ USERDOC-ADMIN-* files do NOT link to USERDOC-STORE-* or USERDOC-MGMT-*
  ✓ USERDOC-MGMT-* files do NOT link to USERDOC-STORE-* or USERDOC-ADMIN-*

For each violation found:
  ✓ Report file path + line number
  ✓ Identify violating link
  ✓ Block merge until fixed (CI fails)

Output: Segregation audit report
Sent to: @DocMaintainer via GitHub issue
```

#### Step 2: YAML Compliance Check (Auto)
```bash
# .github/workflows/docs-audit.yml - monthly-yaml-check
For all USERDOC-* files, verify:
  ✓ system: field populated (store | admin | management)
  ✓ audience.systems: list populated
  ✓ audience.exclude_roles: list populated
  ✓ system_access flags set correctly

Output: List of docs with missing/incorrect fields
Sent to: @DocMaintainer via GitHub issue
```

#### Step 3: Coverage Report (Auto)
```bash
# .github/workflows/docs-audit.yml - monthly-coverage-report
Calculate metrics:
  ✓ Total docs: X
  ✓ Docs with DocID: Y (Y% coverage)
  ✓ Docs without DocID: Z (Z%)
  ✓ By category:
    - GL-*: XX docs
    - KB-*: XX docs
    - WF-*: XX docs
    - USERDOC-STORE-*: XX docs
    - USERDOC-ADMIN-*: XX docs
    - USERDOC-MGMT-*: XX docs
    - Other: XX docs
  ✓ Link health: XX% links valid
  ✓ Compliance: XX% follow GL-052 rules

Output: Metrics dashboard (CSV/JSON)
Sent to: @DocMaintainer + @SARAH
```

#### Step 4: @DocMaintainer Review (First Monday 10 AM-12 PM)
```
1. Review segregation violations
   - For each cross-system link:
     a. Understand why it exists
     b. Either: Remove link OR explain exception
     c. If exception needed: Create security exception document
     d. Notify document owner

2. Review YAML compliance issues
   - Add missing fields where clear
   - Ask owners to fill in where ambiguous
   - Create issues for non-compliance

3. Analyze coverage metrics
   - Is DocID coverage improving? (Track trend)
   - Which categories are under-documented?
   - Are any systems lacking docs?
   - Recommend improvements

4. Generate monthly report
   - Segregation violations: X (target: 0)
   - YAML compliance: Y% (target: 100%)
   - DocID coverage: Z% (target: >90%)
   - Link health: W% (target: 100%)
   - Trend: Improving / Stable / Declining
   - Send to @SARAH + team
   - Create issues for violations (assign owners)
```

**Time**: ~1 hour

---

### Quarterly: Strategic Audit & Planning
**When**: First Monday of Q (Jan, Apr, Jul, Oct), 9 AM CET  
**Owner**: @DocMaintainer + @SARAH

#### Step 1: Full Documentation Inventory
```
1. List ALL documentation by category:
   .ai/
   ├─ guidelines/ → GL-*
   ├─ knowledgebase/ → KB-*
   ├─ workflows/ → WF-*
   ├─ requirements/ → REQ-*
   ├─ decisions/ → ADR-*
   ├─ templates/ → TPL-*
   ├─ sales/ → DOCS-SALES-*
   └─ status/ → STATUS-*

   .github/
   ├─ agents/ → AGT-*
   ├─ prompts/ → PRM-*
   └─ instructions/ → INS-*

   docs/
   ├─ user/
   │  ├─ store/ → USERDOC-STORE-*
   │  ├─ admin/ → USERDOC-ADMIN-*
   │  └─ management/ → USERDOC-MGMT-*
   └─ developer/ → DEVDOC-*

2. Count by category (track trend from previous quarter)
3. Identify gaps (categories with few docs)
4. Identify orphans (docs without DocID, owner, or recent updates)
```

#### Step 2: Coverage Analysis
```
Metrics to track:
✓ Total docs: X (↑/↓ from last quarter)
✓ Docs with DocID: Y% (↑/↓ from last quarter)
✓ Link health: Z% (↑/↓ from last quarter)
✓ GL-052 compliance: W% (↑/↓ from last quarter)
✓ Average doc age: D days (↑/↓ = docs getting older/fresher)
✓ Stale docs (>6mo old): X (↑/↓ from last quarter)

Create trend analysis (improving or declining?)
```

#### Step 3: Compliance Assessment
```
Security & Access Control:
✓ Cross-system segregation: X violations (trend?)
✓ YAML completeness: Y% (trend?)
✓ Access logs audit: (if logging implemented)
✓ System-specific doc completeness:
  - Store docs: Complete? Y/N
  - Admin docs: Complete? Y/N
  - Management docs: Complete? Y/N

Recommend improvements
```

#### Step 4: Recommendations & Planning
```
For Next Quarter:
1. Priority improvements (top 3)
   Example: "Increase KB coverage to 100 docs (currently 67)"
   
2. New standards to adopt
   Example: "Require example code in all API docs"
   
3. New automation to implement
   Example: "Add access logging for doc retrieval"
   
4. Documentation initiatives
   Example: "Migrate legacy docs to new framework"
   
5. Resource needs
   Example: "Request 4 hours/week for automated checks"
```

#### Step 5: Present to @SARAH
```
Quarterly Review Meeting (1 hour):
- Share metrics & trends
- Present recommendations
- Discuss next quarter priorities
- Align with broader documentation goals
- Get approval for major changes
- Update strategic direction if needed
```

**Time**: 2-3 hours (spread across week)

---

## 🛠️ Automation Setup

### Pre-Commit Hook
**File**: `scripts/docs-validation.sh`

```bash
#!/bin/bash
# Pre-commit hook: Validate documentation

echo "🔍 Validating documentation..."

# 1. DocID format check
echo "  ✓ Checking DocID format..."
# Validate all docs have docid: field matching PREFIX-NNN pattern

# 2. Duplicate DocID check
echo "  ✓ Checking for duplicate DocIDs..."
# Ensure no two docs have same DocID

# 3. Registry entry check
echo "  ✓ Checking registry entries..."
# Verify every docid: field has entry in DOCUMENT_REGISTRY.md

# 4. Cross-system link prevention
echo "  ✓ Checking system segregation..."
# Verify no [USERDOC-STORE-*] links to [USERDOC-ADMIN-*], etc.

# 5. YAML validation
echo "  ✓ Validating YAML metadata..."
# Ensure required fields present (docid, title, status, created, owner)

if [ $? -eq 0 ]; then
    echo "✅ Documentation validation passed"
    exit 0
else
    echo "❌ Documentation validation failed"
    exit 1
fi
```

### CI/CD Workflows
**File**: `.github/workflows/docs-audit.yml`

```yaml
name: Documentation Audit

on:
  schedule:
    - cron: '0 9 * * 1'  # Every Monday 9 AM CET
  workflow_dispatch:

jobs:
  weekly-audit:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      # Weekly: Link check
      - name: Check links
        run: npm install -g markdown-link-check
             markdown-link-check .ai/**/*.md .github/**/*.md docs/**/*.md
      
      # Weekly: Metadata validation
      - name: Validate metadata
        run: |
          scripts/validate-metadata.sh
          
      # Monthly: Segregation check (on first Monday)
      - name: Check segregation
        if: github.event.schedule == '0 9 1 * 1'
        run: scripts/validate-segregation.sh
        
      # Create issues for failures
      - name: Report issues
        if: failure()
        run: |
          # Create GitHub issue for @DocMaintainer
          gh issue create \
            --title "Documentation Audit Issues" \
            --body "See CI logs for details" \
            --assignee @DocMaintainer \
            --label docs
```

---

## 📊 Key Metrics to Track

### Daily
- New docs added (count)
- Pre-commit validations passed/failed (%)

### Weekly
- Broken links found/fixed (count, trend)
- Metadata issues found/fixed (count, trend)
- Stale docs identified (count)

### Monthly
- Cross-system link violations (count, target: 0)
- YAML compliance (%, target: 100%)
- DocID coverage (%, target: >90%)
- Link health (%, target: 100%)

### Quarterly
- Total docs by category
- Documentation gaps
- Average doc age
- Trend (improving/declining)
- Coverage by system (Store, Admin, Management)

---

## 🎯 Success Criteria

| Metric | Target | Frequency | Owner |
|--------|--------|-----------|-------|
| **Link Health** | 100% | Weekly | Automated |
| **DocID Coverage** | >95% | Monthly | @DocMaintainer |
| **GL-052 Compliance** | 100% | Monthly | Automated |
| **YAML Completeness** | 100% | Monthly | Automated |
| **Broken Links Fixed** | Within 1 week | Weekly | @DocMaintainer |
| **Registry Up-to-Date** | 100% | Daily | Automated |

---

## 📋 Checklist: Setting Up This Workflow

- [ ] Create `scripts/docs-validation.sh` (pre-commit hook)
- [ ] Create `scripts/validate-metadata.sh` (metadata checker)
- [ ] Create `scripts/validate-segregation.sh` (GL-052 validator)
- [ ] Create `.github/workflows/docs-audit.yml` (CI/CD jobs)
- [ ] Install `markdown-link-check` as dependency
- [ ] Set up pre-commit hook in git config
- [ ] Brief @DocMaintainer on weekly/monthly processes
- [ ] Create Slack channel for audit reports (optional)
- [ ] Schedule calendar reminders for quarterly reviews

---

## 📞 Escalation Path

**For @DocMaintainer to escalate**:
- Link health < 95% → Notify document owners (no escalation needed)
- Link health < 80% → Notify @SARAH
- Cross-system violations detected → Notify @SARAH (security concern)
- Coverage declining → Discuss with @SARAH (needs investigation)
- Major structural issues → Open issue, assign to relevant agent
- Policy questions → Propose to @SARAH for approval

---

## 🔄 Related Documentation

- [AGT-016] @DocMaintainer Agent Definition
- [GL-049] Documentation Framework (5 categories)
- [GL-050] Documentation Structure in docs/
- [GL-052] Role-Based Documentation Access Control
- [DOCUMENT_REGISTRY.md] All DocID entries

---

**Version**: 1.0  
**Created**: 2026-01-08  
**Maintained by**: @DocMaintainer + @SARAH