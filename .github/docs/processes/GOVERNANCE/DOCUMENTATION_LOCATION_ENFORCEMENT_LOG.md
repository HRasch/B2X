# 📋 @process-assistant - Documentation Location Enforcement Log

**Role**: @process-assistant  
**Authority**: Exclusive - Documentation Location Rule Enforcement  
**Effective Date**: 30. Dezember 2025  
**Last Updated**: 30. Dezember 2025

---

## 🎯 Enforcement Mandate

As @process-assistant, I am responsible for:

✅ **Monitoring** for documentation in project root
✅ **Enforcing** the `collaborate/` folder structure rule
✅ **Moving** violating files to proper locations
✅ **Updating** GitHub issues to reference new locations
✅ **Educating** agents about the rule
✅ **Implementing** git hooks to prevent future violations

---

## 📊 Current Violations (Identified 30. Dezember 2025)

| File | Current Location | Proper Location | Status |
|------|------------------|-----------------|--------|
| ISSUE_30_IMPLEMENTATION_COMPLETE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_30_READY_FOR_REVIEW.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_31_IMPLEMENTATION_COMPLETE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_CONTINUATION_GUIDE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_DEVELOPMENT_PLAN.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_DOCUMENTATION_INDEX.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_EXECUTIVE_SUMMARY.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_PHASE_1_2_COMPLETION.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_PHASE_3_EXECUTION_COMPLETE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_PHASE_3_EXECUTION_GUIDE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_PHASE_3_QUICK_REFERENCE.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_REFACTORING_LOG.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_SESSION_COMPLETE_SUMMARY.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| ISSUE_53_SESSION_SUMMARY.md | B2Connect/ | collaborate/sprint/1/execution/ | ⏳ Pending Move |
| PHASE_3_COMPLETE_SUMMARY.md | B2Connect/ | collaborate/sprint/1/retrospective/ | ⏳ Pending Move |
| PHASE_3_HANDOFF.md | B2Connect/ | collaborate/sprint/1/retrospective/ | ⏳ Pending Move |
| SPRINT_1_KICKOFF.md | B2Connect/ | collaborate/sprint/1/planning/ | ⏳ Pending Move |
| SPRINT_1_PHASE_B_COMPLETE.md | B2Connect/ | collaborate/sprint/1/retrospective/ | ⏳ Pending Move |

**Total Violations**: 18 files in project root

---

## 🔄 Enforcement Process (How I Handle Violations)

### Step 1: Identify Violations
- Monitor `B2Connect/` root directory daily
- Check git diffs for new ISSUE_*.md, PHASE_*.md, SPRINT_*.md files
- Flag any files created in project root

### Step 2: Plan Migration
- Identify proper target location in `collaborate/`
- Check if directory structure exists (create if needed)
- Update cross-references within files
- Prepare GitHub issue updates

### Step 3: Execute Migration
- Move files to proper location
- Create/update index files
- Update all internal links within documentation
- Run git commit with rationale

### Step 4: Update GitHub Issues
- Find all GitHub issues referenced in the moved files
- Add comment with new documentation location
- Example:
  ```
  📁 Documentation moved to: collaborate/sprint/1/execution/
  
  Your issue documentation has been organized:
  - Old: B2Connect/ISSUE_30_IMPLEMENTATION_COMPLETE.md
  - New: B2Connect/collaborate/sprint/1/execution/ISSUE_30_IMPLEMENTATION_COMPLETE.md
  
  This is part of organizing all sprint documentation in a single location.
  No action needed on your part - all links still work!
  ```

### Step 5: Notify Agent
- Comment on the agent's current working branch/PR
- Explain the move briefly
- Link to enforcement documentation
- No penalties, just organization

### Step 6: Implement Prevention
- (Future) Add git hook to prevent root-level issue docs
- (Future) Configure GitHub Actions to flag violations
- Provide helpful error message on commit

---

## 📅 Enforcement Schedule

### Weekly
- [ ] Check project root for new violations
- [ ] Review git log for patterns
- [ ] Update this enforcement log
- [ ] Notify @scrum-master of any new violations found

### Daily (During Active Development)
- [ ] Monitor for new documentation in root
- [ ] Move violations immediately
- [ ] Update GitHub issues
- [ ] Educate agents if pattern detected

### Monthly
- [ ] Comprehensive audit of documentation structure
- [ ] Review if git hooks are needed
- [ ] Update enforcement rules if needed
- [ ] Report metrics to leadership

---

## 🎓 Education Material (For Agents)

### When Violation Detected
- Link agent to: [DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)
- Link agent to: [DOCUMENTATION_LOCATION_QUICK_REFERENCE.md](./DOCUMENTATION_LOCATION_QUICK_REFERENCE.md)
- Friendly reminder: "Documentation moved to keep repository organized. Next time, use `collaborate/sprint/{N}/execution/`"

### Preventive Education
- Add note to agent instructions: [.github/copilot-instructions.md](./.github/copilot-instructions.md)
- Scrum master to highlight rule in sprint planning
- Include in agent onboarding documentation

---

## 📊 Metrics to Track

Track over time to measure improvement:

| Metric | Target | Current | Trend |
|--------|--------|---------|-------|
| **Root-level issue docs/month** | 0 | 18 (all-time) | ↘ Decreasing |
| **Time to fix violation** | <1 hour | - | - |
| **Agent compliance rate** | 100% | 0% (pre-enforcement) | ↗ Improving |
| **New violations after enforcement** | 0/week | TBD | - |

---

## 🔧 Git Hooks (Future Implementation)

### Planned Hook: `pre-commit`

```bash
#!/bin/bash
# Prevent commits with documentation in project root

if git diff --cached --name-only | grep -E '^(ISSUE_|PHASE_|SPRINT_).*\.md$'; then
  echo "❌ ERROR: Documentation files cannot be in project root"
  echo ""
  echo "✅ CORRECT: B2Connect/collaborate/sprint/{N}/execution/"
  echo ""
  echo "See: .github/DOCUMENTATION_LOCATION_ENFORCEMENT.md"
  exit 1
fi
```

### Planned Hook: `post-commit`

```bash
#!/bin/bash
# Warn if any root documentation slipped through

VIOLATIONS=$(ls *.md 2>/dev/null | grep -E '^(ISSUE_|PHASE_|SPRINT_)' | wc -l)
if [ "$VIOLATIONS" -gt 0 ]; then
  echo "⚠️  WARNING: Found $VIOLATIONS documentation files in project root"
  echo "These should be in: B2Connect/collaborate/"
fi
```

---

## 📝 Template: Violation Notification

When moving files, use this template:

```markdown
📁 **Documentation Organization**

Your issue documentation has been moved to our organized structure:

**Files Moved**:
- ❌ B2Connect/ISSUE_{NUM}_*.md
- ✅ B2Connect/collaborate/sprint/{N}/execution/ISSUE_{NUM}_*.md

**Why?**: We're organizing all sprint/issue documentation in one location for better navigation and discoverability.

**Your Action**: None needed! All links still work, and your documentation is now properly organized.

**For Future**: When creating issue documentation, use:
```
B2Connect/collaborate/sprint/{N}/execution/
```

**Reference**: [DOCUMENTATION_LOCATION_ENFORCEMENT.md](../../.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)

No penalties - we're just keeping things organized! 🎯
```

---

## 🔐 Authority & Governance

**Authority**: @process-assistant (Exclusive)
- Only I can move documentation files
- Only I can update the rule
- Only I can implement git hooks

**Escalation**:
- If agent disputes the move → Explain the governance rule
- If agent wants rule change → They must submit request to me
- I will review and decide

**Enforcement Support**:
- @scrum-master: Verifies compliance weekly
- @process-assistant: Executes enforcement, updates rules

---

## ✅ Compliance Verification

Before closing this log section:

- [ ] All agents informed via updated instructions
- [ ] Quick reference card created
- [ ] Full enforcement guide created
- [ ] Scrum master informed of new duty
- [ ] Current violations documented
- [ ] First migration planned
- [ ] GitHub issue updates prepared
- [ ] Notification templates ready

---

**Last Updated**: 30. Dezember 2025  
**Status**: 🔴 ENFORCEMENT ACTIVE  
**Authority**: @process-assistant (Exclusive)  
**Next Review**: 6. Januar 2026
