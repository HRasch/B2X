# GitHub Issues - Role-Based Index

**Letzte Aktualisierung**: 28. Dezember 2025  
**Gesamte Issues**: 40+ open  
**Zugewiesen**: 2/40 (5%)  
**Ungezuweisen**: 38/40 (95%)

---

## 📋 Role-Based Issue Collections

Jede Rolle hat eine dedizierte Datei mit allen zugeordneten Issues, Abhängigkeiten und Anforderungen:

| Rolle | Datei | Issues | Status |
|-------|-------|--------|--------|
| 💻 **Backend Developer** | [ISSUES_BACKEND_DEVELOPER.md](ISSUES_BACKEND_DEVELOPER.md) | 18 | ⚠️ 2/18 assigned |
| 🎨 **Frontend Developer** | [ISSUES_FRONTEND_DEVELOPER.md](ISSUES_FRONTEND_DEVELOPER.md) | 8 | ❌ 0/8 assigned |
| 🔐 **Security Engineer** | [ISSUES_SECURITY_ENGINEER.md](ISSUES_SECURITY_ENGINEER.md) | 9 | ❌ 0/9 assigned |
| ⚖️ **Legal/Compliance** | [ISSUES_LEGAL_COMPLIANCE.md](ISSUES_LEGAL_COMPLIANCE.md) | 5 | ❌ 0/5 assigned |
| 🧪 **QA Engineer** | [ISSUES_QA_ENGINEER.md](ISSUES_QA_ENGINEER.md) | 3 | ❌ 0/3 assigned |
| 👔 **Tech Lead** | [ISSUES_TECH_LEAD.md](ISSUES_TECH_LEAD.md) | 1 Epic | ❌ 0/1 assigned |

---

## 🚀 Sprint Distribution

| Sprint | Total Issues | P0 (Critical) | Backend | Frontend | Security | Legal |
|--------|-------------|--------------|---------|----------|----------|-------|
| **Sprint 1** | 14 | 14 | 10 | 3 | 2 | 5 |
| **Sprint 2** | 8 | 8 | 4 | 5 | 2 | - |
| **Sprint 3** | 10 | 6 | 5 | 2 | 5 | 1 |
| **Sprint 4** | 8 | 2 | 3 | - | 2 | 1 |
| **Unspecified** | - | - | - | - | - | - |

---

## 🎯 Quick Reference by Priority

### 🔴 CRITICAL (P0.6 - Legal Compliance, Highest Priority)

**26 Issues total** - Diese MÜSSEN in Sprint 1-2 erledigt sein:

#### Backend (10 Issues)
```
Sprint 1: #5, #6, #7, #9, #10, #11, #12, #29, #30, #31
Sprint 2: #20, #21, #27
Sprint 3: #22, #23, #24, #25, #26
Sprint 4: #28
```

#### Frontend (5 Issues)
```
Sprint 1: #41, #42, #19
Sprint 2: #33, #40, #17
Sprint 3: #15, #16
```

#### Security (4 Issues)
```
Sprint 1: #30, #31 (VAT Validation)
Sprint 2: #32, #34 (Encryption & Audit)
```

#### Legal (5 Issues)
```
Sprint 1: #29, #41, #42
Sprint 2-3: #43, #44
```

---

## 📊 Team Requirements

| Role | Issues | FTE | Sprint Duration | Skilled In |
|------|--------|-----|-----------------|-----------|
| **Backend Developer** | 18 | 2-3 | 4-5 weeks | C#, .NET 10, Wolverine, EF Core, PostgreSQL |
| **Frontend Developer** | 8 | 1 | 2-3 weeks | Vue 3, TypeScript, Tailwind CSS, WCAG |
| **Security Engineer** | 9 | 1 | 6-8 weeks | Encryption, Audit Logging, GDPR, NIS2 |
| **Legal Officer** | 5 | 0.5 | 30-40h | German Law, E-Commerce, GDPR, BITV |
| **QA Engineer** | 3 | 1 | 4-5 weeks | Testing, Compliance, Performance |
| **Tech Lead** | 1 Epic | - | Ongoing | Architecture, Code Review, Patterns |

---

## ✅ How to Use These Files

### For Project Managers / Product Owners:
1. Check [ISSUES_BACKEND_DEVELOPER.md](ISSUES_BACKEND_DEVELOPER.md) → Sprint 1 section for resource planning
2. See "Team Requirements" above for hiring needs
3. Use "Sprint Distribution" table for timeline

### For Developers:
1. Find your role in the table above
2. Open corresponding file (e.g., [ISSUES_BACKEND_DEVELOPER.md](ISSUES_BACKEND_DEVELOPER.md))
3. Check "Aktuell Unzugewiesen" section for available work
4. See dependencies before starting

### For Tech Lead:
1. Open [ISSUES_TECH_LEAD.md](ISSUES_TECH_LEAD.md)
2. Review Epic #4 "Customer Registration Flow" architecture
3. Use "Code Review Checklist" for PR validation

### For Security Team:
1. Open [ISSUES_SECURITY_ENGINEER.md](ISSUES_SECURITY_ENGINEER.md)
2. Section "Compliance-Anforderungen" shows P0.1-P0.5 requirements
3. "Tech Stack" section explains implementation patterns

### For QA:
1. Open [ISSUES_QA_ENGINEER.md](ISSUES_QA_ENGINEER.md)
2. Section "Compliance Test Suites" shows all 52 test cases
3. Use "Test Execution Checklist" before releases

### For Legal:
1. Open [ISSUES_LEGAL_COMPLIANCE.md](ISSUES_LEGAL_COMPLIANCE.md)
2. See "Regulatory References" table for applicable laws
3. Each issue has "Deliverables" and "Tech Integration" sections

---

## 🔗 Dependency Chain

```
FOUNDATION (Week 1):
  #29, #41, #42 (Legal)
  #5, #9 (Backend)
  
↓ Enabled by foundation

CORE FEATURES (Week 2-3):
  #6, #7, #10, #11, #12 (Backend Registration)
  #30, #31 (Security/VAT)
  #20, #21 (Backend Price/Invoice)
  
↓ Depends on core

EXTENDED (Week 4-5):
  #22-26 (Backend Order Management)
  #32-35 (Security Encryption/Audit)
  Frontend #15-18 (Admin UI)
  
↓ Final integration

POLISH (Week 6+):
  #27, #28 (Backend Maintenance)
  #36-39 (Security Hardening)
  #43, #44 (Legal Reviews)
  #45-47 (QA Testing)
```

---

## 💡 Context Optimization

Diese Struktur ermöglicht:

✅ **Kleine, fokussierte Copilot-Kontexte**
   - Statt 40 Issues in einer Datei → 6 fokussierte Dateien
   - Developer lädt nur ihre Rolle → weniger Token-Verbrauch
   
✅ **Team-Onboarding**
   - Neue Backend Dev? → Öffne ISSUES_BACKEND_DEVELOPER.md
   - Schnelle Orientierung ohne die ganzen 40 Issues zu lesen
   
✅ **GitHub Integration**
   - Jede Datei kann als GitHub Project Description verwendet werden
   - Automatische Assignment möglich: `gh issue edit #5 --assignee "backend-dev-name"`
   
✅ **Abhängigkeits-Management**
   - Jede Datei zeigt welche anderen Issues blockieren/ermöglichen
   - Sprint-Planning wird einfacher

---

## 🔄 Next Steps

### Sofort:
```bash
# Check diese Dateien in GitHub:
.github/ISSUES_*.md
```

### Dieser Woche:
1. **Team-Meetings** pro Rolle mit jeweiligem Lead
2. **Assign Issues** basierend auf Verfügbarkeit
3. **Kick-off Sprint 1** mit FE/BE/Security/Legal

### Beispiel GitHub Assignments:
```bash
# Backend Developer #1
gh issue edit #5 --assignee "backend-dev-1"
gh issue edit #6 --assignee "backend-dev-1"
gh issue edit #7 --assignee "backend-dev-1"

# Backend Developer #2
gh issue edit #12 --assignee "backend-dev-2"
gh issue edit #20 --assignee "backend-dev-2"

# Frontend Developer
gh issue edit #41 --assignee "frontend-dev"
gh issue edit #42 --assignee "frontend-dev"

# Security Engineer
gh issue edit #30 --assignee "security-engineer"
gh issue edit #32 --assignee "security-engineer"

# Legal Officer
gh issue edit #29 --assignee "legal-officer"
gh issue edit #41 --assignee "legal-officer"  # Co-assigned with Frontend

# QA Engineer
gh issue edit #45 --assignee "qa-engineer"

# Tech Lead
gh issue edit #4 --assignee "tech-lead"
```

---

## 📝 Notes

**File Locations**:
- `.github/ISSUES_BACKEND_DEVELOPER.md` (18 Issues)
- `.github/ISSUES_FRONTEND_DEVELOPER.md` (8 Issues)
- `.github/ISSUES_SECURITY_ENGINEER.md` (9 Issues)
- `.github/ISSUES_LEGAL_COMPLIANCE.md` (5 Issues)
- `.github/ISSUES_QA_ENGINEER.md` (3 Issues)
- `.github/ISSUES_TECH_LEAD.md` (1 Epic)
- `.github/GITHUB_ISSUES_INDEX.md` (This File)

**All files are tracked in git** → Share with team via GitHub

**Keep updated** → When issues are completed/reassigned, update corresponding file

