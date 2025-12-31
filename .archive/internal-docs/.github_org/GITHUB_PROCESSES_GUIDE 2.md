# GitHub Processes & PR Documentation Guide

**Purpose**: Central guide for GitHub-based processes and PR documentation  
**Version**: 1.0  
**Updated**: 29. Dezember 2025  
**Location**: `.github/GITHUB_PROCESSES_GUIDE.md`

---

## 📁 Process Documentation Structure

### `.github/processes/` Directory
**Location**: `.github/processes/`  
**Purpose**: Central repository for all development and collaboration processes

**Contents**:
```
.github/processes/
├── PROCESSES_INDEX.md                          (process documentation index)
├── PR_DOCUMENTATION_TEMPLATE.md                (template guide for PR docs)
├── CONTRIBUTING.md                             (contribution guidelines)
├── RETROSPECTIVE_PROTOCOL.md                   (sprint retrospective process)
├── DEVELOPMENT_PROCESS_INDEX.md                (complete development guide)
├── DEVELOPMENT_PROCESS_FRAMEWORK.md            (frameworks & patterns)
├── DEVELOPMENT_PROCESS_COMPLETE.md             (comprehensive documentation)
├── DEVELOPMENT_PROCESS_UPDATES.md              (recent process updates)
└── DEVELOPMENT_PROCESS_VISUAL_GUIDE.md         (visual process flows)
```

**Key Files**:
- **PROCESSES_INDEX.md**: Start here for process overview
- **PR_DOCUMENTATION_TEMPLATE.md**: Template for creating PR docs
- **RETROSPECTIVE_PROTOCOL.md**: Sprint retrospective facilitation
- **CONTRIBUTING.md**: Contribution and collaboration guidelines

---

### `/pr/{issueid}/` Directory
**Location**: `/pr/{issueid}/` (replace {issueid} with GitHub issue number)  
**Purpose**: Issue-specific PR documentation

**Structure** (Example `/pr/123/`):
```
/pr/123/
├── README.md                       (directory overview)
├── ISSUE_SUMMARY.md                (issue context & acceptance criteria)
├── IMPLEMENTATION_PLAN.md          (detailed implementation steps)
├── PULL_REQUEST_TEMPLATE.md        (PR description template)
├── TESTING_CHECKLIST.md            (test verification steps)
└── REVIEW_NOTES.md                 (reviewer feedback - added during review)
```

**Standard Files**:
| File | Purpose | When Created | 
|------|---------|--------------|
| ISSUE_SUMMARY.md | Issue context, criteria, dependencies | Issue creation |
| IMPLEMENTATION_PLAN.md | Detailed steps, estimates, testing strategy | Sprint planning |
| PULL_REQUEST_TEMPLATE.md | PR description, changes, testing status | PR creation |
| TESTING_CHECKLIST.md | Unit/integration/manual test results | QA phase |
| REVIEW_NOTES.md | Reviewer comments, approval status | Code review |

---

## 🔄 Workflow: From Issue to Merged PR

```
1. Issue Created
   └─→ /pr/{issueid}/ISSUE_SUMMARY.md
       (Issue context, acceptance criteria, dependencies)

2. Sprint Planning
   └─→ /pr/{issueid}/IMPLEMENTATION_PLAN.md
       (Detailed implementation steps, time estimates)

3. Development & PR Creation
   └─→ /pr/{issueid}/PULL_REQUEST_TEMPLATE.md
       (PR description, changes made, testing status)
       └─→ GitHub PR #[N] references /pr/{issueid}/

4. QA & Testing
   └─→ /pr/{issueid}/TESTING_CHECKLIST.md
       (Unit/integration/manual test results)

5. Code Review
   └─→ /pr/{issueid}/REVIEW_NOTES.md
       (Reviewer comments, requested changes, approval)

6. Merge & Archive
   └─→ Move /pr/{issueid}/ to docs/archive/pr-documentation/{issueid}/
       (Keep last 10 active PRs in /pr/ for reference)
```

---

## �� Creating PR Documentation

### Step 1: Create Issue Directory
```bash
mkdir -p /pr/123
```

### Step 2: Create ISSUE_SUMMARY.md
```markdown
# Issue #123: [Title]

**Status**: Ready for Development
**Epic**: [Epic Name]
**Priority**: [High/Medium/Low]

## Description
[Issue description from GitHub]

## Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2

## Related Issues
- Issue #X - [title]
```

### Step 3: Create IMPLEMENTATION_PLAN.md
```markdown
# Implementation Plan - Issue #123

**Owner**: [Developer]
**Timeline**: [Dates]
**Effort**: [Hours]

## Objective
[Clear objective]

## Implementation Steps
### Step 1: [Title]
- [ ] Subtask
- [ ] Subtask

## Testing Strategy
[Testing approach]

## Success Criteria
- [ ] Criterion 1
```

### Step 4: Create PULL_REQUEST_TEMPLATE.md
```markdown
# Pull Request #[PR Number]

**Issue**: Closes #123
**Title**: [PR Title]

## Changes
- [ ] Feature/Fix 1
- [ ] Feature/Fix 2

## Testing
- [ ] Unit tests added
- [ ] Manual testing done

## Checklist
- [ ] Code review complete
- [ ] Tests passing
- [ ] Documentation updated
```

### Step 5: Create TESTING_CHECKLIST.md
```markdown
# Testing Checklist - Issue #123

## Unit Tests
- [ ] Test 1: [Result]
- [ ] Test 2: [Result]

## Integration Tests
- [ ] Test 1: [Result]

## Manual Testing
- [ ] Scenario 1: [Result]

## Acceptance Criteria Verification
- [ ] Criterion 1: ✅ Passed

## Sign-off
**Tester**: [Name]
**Status**: ✅ APPROVED
```

### Step 6: Update REVIEW_NOTES.md (During Code Review)
```markdown
# Code Review Notes - Issue #123

**PR**: #[PR Number]
**Reviewer**: [Name]
**Date**: [Date]

## ✅ Approved
- [Item 1]
- [Item 2]

## 🔄 Requested Changes
### Change 1: [Title]
**Reason**: [Why]
**Suggestion**: [Fix]
**Status**: [✅ Resolved / ⏳ Pending]

## Final Status
**Approved**: ✅ YES
**Reviewer**: [Name], [Date]
```

---

## 🔗 Referencing PR Documentation

### In GitHub PR Description
```markdown
## PR Details

See `/pr/123/` directory for complete documentation:
- Issue Summary: `/pr/123/ISSUE_SUMMARY.md`
- Implementation Plan: `/pr/123/IMPLEMENTATION_PLAN.md`
- Testing Results: `/pr/123/TESTING_CHECKLIST.md`
- Code Review: `/pr/123/REVIEW_NOTES.md`

## Changes
[PR-specific changes]

## Testing
[Testing summary]
```

### In GitHub Issue Comments
```markdown
PR #[N] created for this issue.

Documentation available at:
- Plan: `/pr/[ID]/IMPLEMENTATION_PLAN.md`
- Testing: `/pr/[ID]/TESTING_CHECKLIST.md`
```

---

## 📊 Process Documentation Organization

### Development Processes (`.github/processes/`)
- **DEVELOPMENT_PROCESS_INDEX.md**: Complete workflow from issue to production
- **DEVELOPMENT_PROCESS_FRAMEWORK.md**: Design patterns, architecture decisions
- **DEVELOPMENT_PROCESS_COMPLETE.md**: Comprehensive implementation guide
- **DEVELOPMENT_PROCESS_UPDATES.md**: Recent changes and improvements
- **DEVELOPMENT_PROCESS_VISUAL_GUIDE.md**: Visual representations

### Contribution & Review
- **CONTRIBUTING.md**: How to contribute, coding standards, review process
- **RETROSPECTIVE_PROTOCOL.md**: Sprint retrospectives, lessons learned
- **PR_DOCUMENTATION_TEMPLATE.md**: Templates for PR documentation

### Related Links
- **GitHub Issues**: GitHub issue templates in `.github/ISSUE_TEMPLATE/`
- **Pull Request**: Pull request template in `.github/pull_request_template.md`
- **Copilot Instructions**: Agent coordination in `.github/copilot-instructions*.md`

---

## 🎯 Best Practices

### Creating PR Documentation
1. ✅ **Start with ISSUE_SUMMARY.md**
   - Clarify requirements before development
   - Set clear acceptance criteria
   - Identify dependencies early

2. ✅ **Keep IMPLEMENTATION_PLAN.md Updated**
   - Document actual progress vs. estimate
   - Note blockers and decisions
   - Link to code commits

3. ✅ **Complete TESTING_CHECKLIST.md**
   - Document all test results
   - Verify acceptance criteria
   - Get QA sign-off before PR

4. ✅ **Collect REVIEW_NOTES.md During Review**
   - Document reviewer comments
   - Track requested changes
   - Record approval and signature

### Maintenance
1. **After Merge**: Archive PR docs to `docs/archive/pr-documentation/{issueid}/`
2. **Monthly Review**: Keep last 10 active PRs in `/pr/` for reference
3. **Quarterly**: Consolidate learnings into process documentation

---

## 📈 Metrics & Tracking

**Track Per PR**:
- Time from issue created → implementation plan: Target <1 day
- Time from plan → PR created: Target <5 days
- Time from PR → approved: Target <2 days
- Review cycles (iterations): Target <2 cycles
- Acceptance criteria: Pass/fail ratio (target ≥95% pass)

**Track Per Process**:
- Document update frequency: Quarterly minimum
- Team feedback integration: Per retrospective
- Process improvement impact: Measured via sprint metrics

---

## 🚀 Getting Started

### For New Team Members
1. Read [.github/processes/PROCESSES_INDEX.md](.github/processes/PROCESSES_INDEX.md)
2. Review [.github/processes/CONTRIBUTING.md](.github/processes/CONTRIBUTING.md)
3. Check example PR docs: `/pr/` (last 3 PRs for reference)
4. Create first PR documentation using templates

### For Team Leads
1. Review [.github/processes/DEVELOPMENT_PROCESS_INDEX.md](.github/processes/DEVELOPMENT_PROCESS_INDEX.md)
2. Facilitate sprint using [.github/processes/RETROSPECTIVE_PROTOCOL.md](.github/processes/RETROSPECTIVE_PROTOCOL.md)
3. Review PR documentation completeness in code reviews
4. Archive completed PR docs quarterly

### For Scrum Master
1. Maintain [.github/processes/PROCESSES_INDEX.md](.github/processes/PROCESSES_INDEX.md)
2. Update [.github/processes/PR_DOCUMENTATION_TEMPLATE.md](.github/processes/PR_DOCUMENTATION_TEMPLATE.md) based on learnings
3. Run retrospectives using [.github/processes/RETROSPECTIVE_PROTOCOL.md](.github/processes/RETROSPECTIVE_PROTOCOL.md)
4. Archive PR docs: Move completed to `docs/archive/pr-documentation/`

---

## 📚 Complete File Reference

### Process Documentation Files
| File | Purpose | Location |
|------|---------|----------|
| PROCESSES_INDEX.md | Process overview & navigation | `.github/processes/` |
| PR_DOCUMENTATION_TEMPLATE.md | PR documentation templates | `.github/processes/` |
| CONTRIBUTING.md | Contribution guidelines | `.github/processes/` |
| RETROSPECTIVE_PROTOCOL.md | Sprint retrospective process | `.github/processes/` |
| DEVELOPMENT_PROCESS_*.md | Complete development processes | `.github/processes/` |

### PR Documentation Files
| File | Purpose | Location |
|------|---------|----------|
| README.md | PR directory overview | `/pr/` |
| ISSUE_SUMMARY.md | Issue context & criteria | `/pr/{issueid}/` |
| IMPLEMENTATION_PLAN.md | Implementation steps | `/pr/{issueid}/` |
| PULL_REQUEST_TEMPLATE.md | PR description | `/pr/{issueid}/` |
| TESTING_CHECKLIST.md | Test verification | `/pr/{issueid}/` |
| REVIEW_NOTES.md | Reviewer feedback | `/pr/{issueid}/` |

---

## 🔗 Quick Links

- **Processes Index**: [.github/processes/PROCESSES_INDEX.md](.github/processes/PROCESSES_INDEX.md)
- **Contributing Guide**: [.github/processes/CONTRIBUTING.md](.github/processes/CONTRIBUTING.md)
- **Development Guide**: [.github/processes/DEVELOPMENT_PROCESS_INDEX.md](.github/processes/DEVELOPMENT_PROCESS_INDEX.md)
- **PR Documentation**: [/pr/README.md](/pr/README.md)
- **Retrospective Guide**: [.github/processes/RETROSPECTIVE_PROTOCOL.md](.github/processes/RETROSPECTIVE_PROTOCOL.md)

---

**Maintained By**: Scrum Master  
**Last Updated**: 29. Dezember 2025  
**Next Review**: After first full sprint using new structure

