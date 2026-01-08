---
docid: TPL-009
title: Github Bug
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
name: 🐛 Bug Report
about: Issue-Template für Bug Reports
title: "[BUG] [TITLE]"
labels: ["bug"]
---

## 🐛 Bug Summary

**Kurzbeschreibung:**
[Prägnante Beschreibung des Bugs]

---

## 📍 Where Does It Happen?

### Environment
- **OS:** [Windows / macOS / Linux]
- **Browser/App:** [Chrome / Firefox / Mobile App / etc.]
- **Version:** [Version number]

### Steps to Reproduce
1. [Schritt 1]
2. [Schritt 2]
3. [Schritt 3]

### Expected Behavior
[Was sollte passieren?]

### Actual Behavior
[Was passiert stattdessen?]

### Screenshot/Video
[Wenn möglich, Screenshot oder Video der Fehler]

---

## 📊 Impact Assessment

### Severity
- [ ] 🔴 **Critical** - System down, data loss, security issue
- [ ] 🟠 **High** - Major feature broken, workaround difficult
- [ ] 🟡 **Medium** - Feature partially broken, workaround exists
- [ ] 🟢 **Low** - Minor issue, cosmetic, edge case

### Frequency
- [ ] 🔴 Always reproducible
- [ ] 🟠 Often (>50%)
- [ ] 🟡 Sometimes (10-50%)
- [ ] 🟢 Rarely (<10%)

### Users Affected
[Wie viele Benutzer sind betroffen?]

---

## 🔍 Initial Analysis

### @Backend - Backend Perspective
**Possible Cause:**
[Vermutete Ursache]

**Affected Services:**
- [ ]

**Investigation Needed:**
- [ ]

---

### @Frontend - Frontend Perspective
**UI Issues:**
[Frontend-relevante Punkte]

**Browser Compatibility:**
[In welchen Browsern tritt es auf?]

---

### @Security - Security Perspective
**Security Impact:**
[Ist das ein Security Issue?]

**Data Exposure:**
[Ja / Nein - Details]

---

### @QA - Testing Perspective
**Test Case:**
[Wie kann das automatisiert/getestet werden?]

**Regression Risk:**
[Hoch / Mittel / Niedrig]

---

## 🔗 Related Issues

- Related to: #XXX
- Duplicate of: #XXX

---

## 📝 Notes

[Zusätzliche Informationen]

---

## 🚀 Workflow

1. **Bug reported** → `.ai/issues/{issue-id}/` angelegt
2. **Agents analysieren** → Root cause analysis
3. **Fix implementiert** → PR linked zu diesem Bug
4. **Testing** → QA verifies fix
5. **Deployed** → GitHub Comment mit Status

---

*Template Version: 1.0 | Last Updated: 2025-12-30*
