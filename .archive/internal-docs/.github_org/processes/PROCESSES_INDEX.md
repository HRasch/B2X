# Process Documentation Index

**Location**: `.github/processes/`  
**Purpose**: Central repository for all development, review, and collaboration processes  
**Last Updated**: 29. Dezember 2025

---

## �� Process Categories

### Development Processes
- [DEVELOPMENT_PROCESS_INDEX.md](./DEVELOPMENT_PROCESS_INDEX.md) - Complete development workflow
- [DEVELOPMENT_PROCESS_FRAMEWORK.md](./DEVELOPMENT_PROCESS_FRAMEWORK.md) - Framework and patterns
- [DEVELOPMENT_PROCESS_COMPLETE.md](./DEVELOPMENT_PROCESS_COMPLETE.md) - Comprehensive guide
- [DEVELOPMENT_PROCESS_UPDATES.md](./DEVELOPMENT_PROCESS_UPDATES.md) - Recent updates and changes
- [DEVELOPMENT_PROCESS_VISUAL_GUIDE.md](./DEVELOPMENT_PROCESS_VISUAL_GUIDE.md) - Visual process flows

### Contribution & Review
- [CONTRIBUTING.md](./CONTRIBUTING.md) - Contribution guidelines
- [RETROSPECTIVE_PROTOCOL.md](./RETROSPECTIVE_PROTOCOL.md) - Sprint retrospective process

---

## 🔄 Related Documentation

**PR Documentation**: See [`/pr/{issueid}/`](../../pr/) for issue-specific pull request documentation  
**Role-Based Guides**: See [`.github/docs/roles/`](../docs/roles/) for role-specific processes  
**Copilot Instructions**: See [`.github/`](../) for AI agent coordination

---

## 📖 Quick Reference

### Running a Sprint
1. Check [DEVELOPMENT_PROCESS_FRAMEWORK.md](./DEVELOPMENT_PROCESS_FRAMEWORK.md) for sprint structure
2. Follow the workflow outlined in [DEVELOPMENT_PROCESS_COMPLETE.md](./DEVELOPMENT_PROCESS_COMPLETE.md)
3. Conduct retrospectives using [RETROSPECTIVE_PROTOCOL.md](./RETROSPECTIVE_PROTOCOL.md)

### Contributing Code
1. Review [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines
2. Follow development process in [DEVELOPMENT_PROCESS_INDEX.md](./DEVELOPMENT_PROCESS_INDEX.md)
3. See `/pr/{issueid}/` for issue-specific PR documentation

### Creating a Pull Request
1. Follow steps in [DEVELOPMENT_PROCESS_UPDATES.md](./DEVELOPMENT_PROCESS_UPDATES.md)
2. Reference issue ID in PR title: `Fix issue #123`
3. Store PR-specific docs in `/pr/123/` directory

---

## 📁 Directory Structure

```
.github/processes/
├── PROCESSES_INDEX.md                      (this file)
├── CONTRIBUTING.md                         (contribution guidelines)
├── RETROSPECTIVE_PROTOCOL.md               (sprint retrospectives)
├── DEVELOPMENT_PROCESS_INDEX.md            (complete guide)
├── DEVELOPMENT_PROCESS_FRAMEWORK.md        (framework & patterns)
├── DEVELOPMENT_PROCESS_COMPLETE.md         (comprehensive documentation)
├── DEVELOPMENT_PROCESS_UPDATES.md          (recent updates)
└── DEVELOPMENT_PROCESS_VISUAL_GUIDE.md     (visual process flows)

/pr/
├── {issueid}/
│   ├── ISSUE_SUMMARY.md                    (issue context & acceptance criteria)
│   ├── IMPLEMENTATION_PLAN.md               (detailed implementation steps)
│   ├── PULL_REQUEST_TEMPLATE.md             (PR description template)
│   ├── TESTING_CHECKLIST.md                 (test verification steps)
│   └── REVIEW_NOTES.md                      (reviewer feedback & decisions)
```

---

## ✨ Purpose of Each Document

| Document | Purpose | Audience |
|----------|---------|----------|
| **CONTRIBUTING.md** | How to contribute to the project | All developers |
| **DEVELOPMENT_PROCESS_INDEX.md** | Complete development workflow reference | Dev team |
| **DEVELOPMENT_PROCESS_FRAMEWORK.md** | Patterns, best practices, frameworks | Dev team |
| **DEVELOPMENT_PROCESS_COMPLETE.md** | Comprehensive implementation guide | Dev team leads |
| **DEVELOPMENT_PROCESS_UPDATES.md** | Recent process changes and updates | All team |
| **DEVELOPMENT_PROCESS_VISUAL_GUIDE.md** | Visual representations of processes | Visual learners |
| **RETROSPECTIVE_PROTOCOL.md** | Sprint retrospective facilitation | Scrum master, team leads |

---

## 🔗 Key Links

- **Quick Start**: [QUICK_START_GUIDE.md](../../QUICK_START_GUIDE.md)
- **Project Dashboard**: [PROJECT_DASHBOARD.md](../../PROJECT_DASHBOARD.md)
- **GitHub Configuration**: [`.github/`](../)
- **PR Templates**: [`.github/ISSUE_TEMPLATE/`](../ISSUE_TEMPLATE/)

---

## 📝 Creating Issue-Specific Documentation

When working on issue #123:

```bash
# Create issue-specific PR directory
mkdir -p /pr/123

# Create standard PR documentation
cat > /pr/123/ISSUE_SUMMARY.md << 'ISSUE'
# Issue #123: [Title]
...
ISSUE
```

Standard files for `/pr/{issueid}/`:
- `ISSUE_SUMMARY.md` - Issue context, acceptance criteria
- `IMPLEMENTATION_PLAN.md` - Detailed implementation steps
- `PULL_REQUEST_TEMPLATE.md` - PR description template
- `TESTING_CHECKLIST.md` - Test verification checklist
- `REVIEW_NOTES.md` - Reviewer feedback (added during PR review)

---

**Maintained By**: Scrum Master  
**Last Review**: 29. Dezember 2025  
**Next Review**: After Sprint 4 completion
