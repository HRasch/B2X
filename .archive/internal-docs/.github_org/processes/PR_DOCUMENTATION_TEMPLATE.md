# PR Documentation Template

**Purpose**: Guide for creating issue-specific PR documentation under `/pr/{issueid}/`  
**Location**: `/pr/{issueid}/` (replace {issueid} with GitHub issue number)  
**Created**: 29. Dezember 2025

---

## Directory Structure Example

```
/pr/123/
├── ISSUE_SUMMARY.md              (issue context & acceptance criteria)
├── IMPLEMENTATION_PLAN.md         (detailed implementation steps)
├── PULL_REQUEST_TEMPLATE.md       (PR description template)
├── TESTING_CHECKLIST.md           (test verification steps)
└── REVIEW_NOTES.md                (reviewer feedback - added during PR review)
```

---

## File Templates

### 1. ISSUE_SUMMARY.md

```markdown
# Issue #123: [Issue Title]

**Status**: Ready for Development  
**Created**: [Date]  
**Epic**: [Epic Name]  
**Priority**: [High/Medium/Low]  
**Effort Estimate**: [Hours]  

---

## 📋 Issue Description

[Detailed issue description from GitHub issue]

## ✅ Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## 🔗 Related Issues

- Issue #[related] - [title]
- Issue #[related] - [title]

## 📚 Reference Documentation

- [Link to relevant docs]
- [Link to architectural decision]
```

### 2. IMPLEMENTATION_PLAN.md

```markdown
# Implementation Plan - Issue #123

**Issue**: [Issue Title]  
**Owner**: [Developer Name]  
**Timeline**: [Start Date] - [End Date]  
**Effort**: [Hours]  

---

## 🎯 Objective

[Clear objective statement]

## 📋 Implementation Steps

### Step 1: [Step Title]
**Time**: [Hours]  
**Deliverable**: [What will be delivered]  
```
- [ ] Sub-task 1
- [ ] Sub-task 2
```

### Step 2: [Step Title]
...

## �� Testing Strategy

[Testing approach and test cases]

## 📊 Success Criteria

- [ ] Criterion 1
- [ ] Criterion 2

## 🔄 Dependencies

- [ ] Depends on Issue #[X]
- [ ] Blocks Issue #[Y]
```

### 3. PULL_REQUEST_TEMPLATE.md

```markdown
# Pull Request #[PR Number]

**Issue**: Closes #123  
**Title**: [PR Title]  
**Author**: [Name]  
**Created**: [Date]  

---

## 📝 Description

[Brief description of changes]

## ✅ Changes

- [ ] Feature/Fix 1
- [ ] Feature/Fix 2
- [ ] Documentation updated

## 🧪 Testing

- [ ] Unit tests added
- [ ] Integration tests added
- [ ] Manual testing completed

## 📋 Checklist

- [ ] Code review completed
- [ ] Tests passing
- [ ] Documentation updated
- [ ] No breaking changes
```

### 4. TESTING_CHECKLIST.md

```markdown
# Testing Checklist - Issue #123

**Purpose**: Verify all acceptance criteria met  
**Created**: [Date]  

---

## ✅ Unit Tests

- [ ] [Test 1]
- [ ] [Test 2]
- [ ] [Test 3]

## ✅ Integration Tests

- [ ] [Test 1]
- [ ] [Test 2]

## ✅ Manual Testing

- [ ] [Scenario 1]
- [ ] [Scenario 2]

## ✅ Acceptance Criteria Verification

- [ ] Criterion 1: [Verified/Failed]
- [ ] Criterion 2: [Verified/Failed]

## ✅ Performance

- [ ] Response time: [Time]ms
- [ ] Memory usage: [Value]
- [ ] Load test: [Result]

## 🐛 Known Issues

[Any known issues or limitations]

## ✨ Sign-off

**Tester**: [Name]  
**Date**: [Date]  
**Status**: ✅ APPROVED / ❌ FAILED
```

### 5. REVIEW_NOTES.md

```markdown
# Code Review Notes - Issue #123

**PR**: #[PR Number]  
**Reviewer**: [Reviewer Name]  
**Date**: [Review Date]  

---

## ✅ Approved Items

- [Item 1]
- [Item 2]

## 🔄 Requested Changes

### Change 1: [Title]
**Reason**: [Why this change]  
**Suggestion**: [Suggested fix]  
**Status**: [ ] Pending / [✅] Resolved

### Change 2: [Title]
...

## 💡 Suggestions (Optional)

- Suggestion 1
- Suggestion 2

## 📋 Compliance Check

- [ ] Security review passed
- [ ] Performance acceptable
- [ ] No breaking changes
- [ ] Documentation updated
- [ ] Tests included

## 🎯 Final Status

**Approved**: ✅ YES / ❌ NO  
**Comments**: [Final comments]  
**Reviewer Signature**: [Name], [Date]
```

---

## 🚀 How to Use

### Creating PR Documentation

```bash
# 1. Create issue directory
mkdir -p /pr/123

# 2. Copy templates
cp .github/processes/PR_DOCUMENTATION_TEMPLATE.md /pr/123/

# 3. Fill in templates with actual data
# - Start with ISSUE_SUMMARY.md
# - Create IMPLEMENTATION_PLAN.md
# - Add PULL_REQUEST_TEMPLATE.md when creating PR
# - Create TESTING_CHECKLIST.md during testing phase
# - Add REVIEW_NOTES.md during code review

# 4. Reference in PR description
# Link to `/pr/123/PULL_REQUEST_TEMPLATE.md` in GitHub PR
```

### Maintenance

**After PR Merge**:
- Archive completed PR docs to `docs/archive/pr-documentation/`
- Keep recent PRs (last 10) in `/pr/` for reference
- Link to archived PR docs in relevant documentation

**Review & Update**:
- Monthly: Review active PR directories
- Quarterly: Archive completed PRs
- Annually: Consolidate learnings into process documentation

---

## 📊 Metrics to Track

Per PR Documentation:
- Time from issue → implementation plan
- Time from implementation → PR creation
- Time from PR → approval/merge
- Number of review cycles
- Acceptance criteria: passed/failed ratio

---

## 🔗 References

- [DEVELOPMENT_PROCESS_INDEX.md](./DEVELOPMENT_PROCESS_INDEX.md)
- [CONTRIBUTING.md](./CONTRIBUTING.md)
- [RETROSPECTIVE_PROTOCOL.md](./RETROSPECTIVE_PROTOCOL.md)
- [Pull Request Template](../pull_request_template.md)

---

**Maintained By**: Scrum Master  
**Last Updated**: 29. Dezember 2025
