# PR Documentation Directory

**Purpose**: Issue-specific pull request documentation organized by issue ID  
**Structure**: `/pr/{issueid}/`  
**Last Updated**: 29. Dezember 2025

---

## 📁 Directory Structure

```
/pr/
├── README.md                 (this file)
├── 30/                       (Example: Issue #30)
│   ├── ISSUE_SUMMARY.md      (issue context & acceptance criteria)
│   ├── IMPLEMENTATION_PLAN.md (detailed implementation steps)
│   ├── PULL_REQUEST_TEMPLATE.md (PR description template)
│   ├── TESTING_CHECKLIST.md  (test verification steps)
│   └── REVIEW_NOTES.md       (reviewer feedback)
├── 31/
│   ├── ISSUE_SUMMARY.md
│   └── ... (same structure)
```

---

## 🔄 Workflow

### 1️⃣ Issue Creation
Create `/pr/{issueid}/ISSUE_SUMMARY.md` with:
- Issue context and description
- Acceptance criteria
- Related issues and dependencies

### 2️⃣ Planning Phase
Create `/pr/{issueid}/IMPLEMENTATION_PLAN.md` with:
- Detailed implementation steps
- Time estimates
- Testing strategy
- Success criteria

### 3️⃣ Development & PR Creation
Create `/pr/{issueid}/PULL_REQUEST_TEMPLATE.md` with:
- PR description
- Changes made
- Testing completed
- Checklist items

### 4️⃣ Testing Phase
Create `/pr/{issueid}/TESTING_CHECKLIST.md` with:
- Unit test results
- Integration test results
- Manual testing scenarios
- Acceptance criteria verification

### 5️⃣ Code Review
Update `/pr/{issueid}/REVIEW_NOTES.md` with:
- Reviewer comments
- Requested changes
- Approval status
- Reviewer signature

---

## 📋 Standard Files Per Issue

| File | Purpose | Created | Updated |
|------|---------|---------|---------|
| **ISSUE_SUMMARY.md** | Issue context & criteria | Issue creation | PR review |
| **IMPLEMENTATION_PLAN.md** | Detailed steps & estimates | Sprint planning | As needed |
| **PULL_REQUEST_TEMPLATE.md** | PR description | PR creation | Before merge |
| **TESTING_CHECKLIST.md** | Test verification | QA phase | During testing |
| **REVIEW_NOTES.md** | Reviewer feedback | Code review | After approval |

---

## 🚀 Quick Start

### Create PR Documentation for Issue #123

```bash
# Create directory
mkdir -p /pr/123

# Copy templates
cp .github/processes/PR_DOCUMENTATION_TEMPLATE.md /pr/123/

# Create files (use templates as guide)
# 1. ISSUE_SUMMARY.md - Issue context
# 2. IMPLEMENTATION_PLAN.md - Implementation steps
# 3. PULL_REQUEST_TEMPLATE.md - PR description
# 4. TESTING_CHECKLIST.md - Test verification
# 5. REVIEW_NOTES.md - Reviewer feedback

# Reference in GitHub PR
# Link: See /pr/123/ for detailed documentation
```

---

## 📖 Examples

### Example: Issue #30 (Price Transparency)
See `/pr/30/` directory for:
- Issue summary with VAT requirements
- Implementation plan with database schema
- PR template with testing results
- Testing checklist with verification status
- Review notes with approvals

### Template Structure
See [.github/processes/PR_DOCUMENTATION_TEMPLATE.md](.github/processes/PR_DOCUMENTATION_TEMPLATE.md) for:
- File structure and format
- Detailed templates for each file
- Usage instructions
- Best practices

---

## 🔄 Maintenance

### Active PRs
Keep `/pr/{issueid}/` directories for:
- Current sprint PRs (in progress)
- Recent merged PRs (last 10)
- Reference for ongoing development

### Archive Old PRs
Move completed PR documentation to:
- `docs/archive/pr-documentation/{issueid}/`
- Frequency: Monthly review, quarterly archive

### Metrics Tracking
Track per PR:
- Time from issue → implementation plan
- Time from plan → PR creation
- Time from PR → approval
- Review cycles (iterations)

---

## 🎯 Best Practices

1. **Create ISSUE_SUMMARY.md FIRST**
   - Clarify requirements before development
   - Identify dependencies early
   - Set clear acceptance criteria

2. **Link from GitHub**
   - Reference PR docs in PR description
   - Use format: See `/pr/123/PULL_REQUEST_TEMPLATE.md`
   - Helps reviewers understand context

3. **Update During Development**
   - Keep IMPLEMENTATION_PLAN.md in sync with actual progress
   - Update REVIEW_NOTES.md as feedback arrives
   - Document blockers and decisions

4. **Archive After Merge**
   - Move to `docs/archive/pr-documentation/` after merge
   - Keep last 10 active PRs in `/pr/` for reference
   - Link from sprint summary to completed PRs

---

## �� Related Documentation

- **Process Guide**: [.github/processes/PROCESSES_INDEX.md](.github/processes/PROCESSES_INDEX.md)
- **PR Template**: [.github/processes/PR_DOCUMENTATION_TEMPLATE.md](.github/processes/PR_DOCUMENTATION_TEMPLATE.md)
- **Development Process**: [.github/processes/DEVELOPMENT_PROCESS_INDEX.md](.github/processes/DEVELOPMENT_PROCESS_INDEX.md)
- **Contributing**: [.github/processes/CONTRIBUTING.md](.github/processes/CONTRIBUTING.md)

---

**Maintained By**: Scrum Master  
**Last Updated**: 29. Dezember 2025  
**Next Review**: When first PR documentation set created

