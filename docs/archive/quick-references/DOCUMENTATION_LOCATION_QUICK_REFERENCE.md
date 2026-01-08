# 📁 Documentation Location Quick Card

**Print this or keep handy!**

---

## ✅ WHERE TO PUT DOCUMENTATION

### Issue Implementation → `collaborate/sprint/{N}/execution/`
```
Example: ISSUE_30_IMPLEMENTATION_COMPLETE.md
Location: B2X/collaborate/sprint/1/execution/
```

### Sprint Planning → `collaborate/sprint/{N}/planning/`
```
Example: SPRINT_1_KICKOFF.md
Location: B2X/collaborate/sprint/1/planning/
```

### Sprint Retrospective → `collaborate/sprint/{N}/retrospective/`
```
Example: SPRINT_1_RETROSPECTIVE.md
Location: B2X/collaborate/sprint/1/retrospective/
```

### PR Documentation → `collaborate/pr/{PR_NUM}/`
```
Examples: 
  - collaborate/pr/30/design-decisions/
  - collaborate/pr/30/implementation-notes/
  - collaborate/pr/30/review-feedback/
```

### Lessons Learned → `collaborate/lessons-learned/`
```
Example: 2025-12-30-build-first-rule.md
Location: B2X/collaborate/lessons-learned/
```

### Team Agreements → `collaborate/agreements/`
```
Examples: coding-standards.md, process-agreements.md
```

---

## ❌ NEVER CREATE IN PROJECT ROOT

```
Wrong:
  B2X/ISSUE_30_*.md
  B2X/PHASE_3_*.md
  B2X/SPRINT_*.md
```

---

## ✅ ALWAYS UPDATE INDEX

When creating new issue doc in `collaborate/sprint/1/execution/`:

```bash
# Add to index.md:
- [Issue #30: Description](./ISSUE_30_IMPLEMENTATION_COMPLETE.md)
```

---

## 📞 Questions?

- **Where does this doc go?** → Check [DOCUMENTATION_LOCATION_ENFORCEMENT.md](./.github/DOCUMENTATION_LOCATION_ENFORCEMENT.md)
- **Sprint number?** → Check GitHub issue labels
- **Not sure?** → Ask @scrum-master

---

**Enforced by**: @process-assistant  
**Effective**: 30. Dezember 2025
