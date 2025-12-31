---
name: AI Agent PR
about: Pull Request Template mit Multi-Agent Review
title: "[PR] [TITLE]"
labels: ["needs-review"]
---

## 📌 PR Summary

**Was wurde gemacht?**
[Beschreibung der Änderungen]

**Ticket/Issue:**
Closes #XXX

---

## 🎯 Type of Change

- [ ] 🐛 Bug Fix
- [ ] ✨ New Feature
- [ ] 📚 Documentation
- [ ] ♻️ Refactoring
- [ ] ⚡ Performance
- [ ] 🔒 Security

---

## 📝 Changes

### Files Changed
- `path/to/file.ts`
- `path/to/file.tsx`

### Summary
[Kurze Zusammenfassung der Änderungen]

---

## ✅ Pre-Review Checklist

### Code Quality
- [ ] Code follows style guidelines (`.github/instructions/*.instructions.md`)
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] No console logs left

### Testing
- [ ] Unit tests added/updated
- [ ] Integration tests pass
- [ ] No regressions detected

### Security
- [ ] No secrets committed
- [ ] Input validation present
- [ ] OWASP principles followed

### Documentation
- [ ] API docs updated (if applicable)
- [ ] README updated (if applicable)
- [ ] Type definitions complete

---

## 👥 Required Reviews

### @Backend Review (if backend changes)
- [ ] API contracts correct
- [ ] Database queries optimized
- [ ] Error handling proper
- [ ] Tests sufficient

**@Backend:** ❓ Awaiting review

---

### @Frontend Review (if frontend changes)
- [ ] Component props typed
- [ ] Accessibility standards met
- [ ] Performance optimized
- [ ] Responsive design tested

**@Frontend:** ❓ Awaiting review

---

### @Security Review (if security-relevant)
- [ ] Input sanitization present
- [ ] Auth/AuthZ correct
- [ ] Data protection adequate
- [ ] No vulnerabilities introduced

**@Security:** ❓ Awaiting review

---

### @QA Review
- [ ] Test scenarios comprehensive
- [ ] Test data realistic
- [ ] Coverage adequate
- [ ] Manual testing plan provided

**@QA:** ❓ Awaiting review

---

### @TechLead Review
- [ ] Architecture follows ADRs
- [ ] Code quality high
- [ ] Technical debt addressed/documented
- [ ] Maintainability good

**@TechLead:** ❓ Awaiting review

---

## 📊 Metrics (if applicable)

- Bundle Size: ±X KB
- Performance: +X% / -X%
- Test Coverage: XX%

---

## 🔄 Deployment

### Deployment Type
- [ ] Zero-downtime
- [ ] Requires migration
- [ ] Feature flag needed
- [ ] Manual steps required

### Deployment Checklist
- [ ] Database migrations tested
- [ ] Feature flags configured
- [ ] Monitoring prepared
- [ ] Rollback plan documented

---

## 📝 Notes

[Zusätzliche Notizen für Reviewer]

---

## 🚀 Merge Requirements

- [ ] All reviews approved
- [ ] All checks passing
- [ ] No conflicts
- [ ] Squash & merge strategy applied

---

*Template Version: 1.0 | Last Updated: 2025-12-30*
