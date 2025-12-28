# B2Connect Team - Fiktive Entwickler

**GitHub Organization**: b2connect-dev  
**Team Struktur**: 7 Rollen, 9 Entwickler  
**Status**: Active Development Sprint 1

---

## 👥 Team-Übersicht

### Backend-Team (3 Developer)

| Role | GitHub-ID | Position | Fokus | Status |
|------|-----------|----------|-------|--------|
| Backend Lead | `@backend-lead` | Senior Backend Dev + Tech Lead | VAT, Tax Logic, Architecture | ✅ Active |
| Backend Dev | `@backend-dev-auth` | Backend Developer | User Registration, Auth | ⏳ Zuweisbar |
| Backend Dev | `@backend-dev-commerce` | Backend Developer | Orders, Invoicing, E-Commerce | ⏳ Zuweisbar |

### Frontend-Team (2 Developer)

| Role | GitHub-ID | Position | Fokus | Status |
|------|-----------|----------|-------|--------|
| Frontend Dev | `@frontend-dev-store` | Frontend Developer | Store Frontend, Components | ⏳ Zuweisbar |
| Frontend Dev | `@frontend-dev-admin` | Frontend Developer | Admin Dashboard, Theming | ⏳ Zuweisbar |

### Security & Ops (2 Engineer)

| Role | GitHub-ID | Position | Fokus | Status |
|------|-----------|----------|-------|--------|
| Security Engineer | `@security-engineer` | Security Engineer | Encryption, Audit, NIS2 | ⏳ Zuweisbar |
| DevOps Engineer | `@devops-engineer` | DevOps Engineer | Infrastructure, Monitoring | ⏳ Zuweisbar |

### QA & Legal (2 Specialist)

| Role | GitHub-ID | Position | Fokus | Status |
|------|-----------|----------|-------|--------|
| QA Engineer | `@qa-engineer` | QA Engineer | Compliance Testing, Coverage | ⏳ Zuweisbar |
| Legal/Compliance | `@legal-officer` | Legal/Compliance Officer | GDPR, E-Commerce Law, Audit | ⏳ Zuweisbar |

---

## 🔑 Zugangsdaten (Fiktiv)

```
GitHub Organization: https://github.com/b2connect-dev
Repository: b2connect/b2connect-platform
Main Branch: main
Development: develop
```

---

## 📋 Issue Assignment Scheme

### Sprint 1 - Woche 1 (28.12.2025 - 03.01.2026)

#### Backend (@backend-lead + @backend-dev-auth + @backend-dev-commerce)
- **@backend-lead** (Lead): #30 (VAT), #31 (Reverse Charge), Architecture (#4)
- **@backend-dev-auth**: #5, #6, #7, #9, #10, #11, #12 (Registration Flow)
- **@backend-dev-commerce**: #20, #21, #29, #27 (Orders, Invoicing, Returns)

#### Frontend (@frontend-dev-store + @frontend-dev-admin)
- **@frontend-dev-store**: #41, #42, #19 (Components, Legal UI)
- **@frontend-dev-admin**: #17, #18 (Theme Config, Admin Prep)

#### Security (@security-engineer)
- **@security-engineer**: #30, #31 (Review), #32, #34 (Encryption)

#### QA (@qa-engineer)
- **@qa-engineer**: #45, #46 (Test Framework, E-Commerce Tests)

#### Legal (@legal-officer)
- **@legal-officer**: #29, #41, #42 (Return Policy, Terms, Privacy)

---

## 🎯 GitHub Issue Assignment Commands (Rollenbasiert)

```bash
# Backend Developer (Auth) - @backend-dev-auth
gh issue comment #5 --body "@backend-dev-auth - Zugewiesen für Sprint 1. User Registration Handler. Start: 2026-01-02"
gh issue edit #5 --assignee "backend-dev-auth" --add-label "sprint-1,in-progress"

gh issue comment #6 --body "@backend-dev-auth - Email Verification Logic. Abhängig von #5."
gh issue edit #6 --assignee "backend-dev-auth" --add-label "sprint-1"

# Backend Developer (Commerce) - @backend-dev-commerce
gh issue comment #20 --body "@backend-dev-commerce - Zugewiesen für Sprint 1. Price Calculation Service. Start: 2026-01-02"
gh issue edit #20 --assignee "backend-dev-commerce" --add-label "sprint-1"

# Frontend Developer (Store) - @frontend-dev-store
gh issue comment #41 --body "@frontend-dev-store - Zugewiesen für Sprint 1. AGB & Checkbox. Koordination mit @legal-officer"
gh issue edit #41 --assignee "frontend-dev-store" --add-label "sprint-1" --assignee "legal-officer"

# Backend Lead & Security Engineer - @backend-lead @security-engineer
gh issue comment #30 --body "@backend-lead & @security-engineer - VAT-ID Validation. Security Review erforderlich. Priority: 🔴 P0"
gh issue edit #30 --assignee "backend-lead" --assignee "security-engineer" --add-label "security-review"

# QA Engineer - @qa-engineer
gh issue comment #45 --body "@qa-engineer - Test Framework Setup. 15 E-Commerce Legal Tests. Start: 2026-01-09"
gh issue edit #45 --assignee "qa-engineer" --add-label "sprint-2,testing"
```

---

## 📊 Sprint Board Struktur

### Column: Backlog
- #39, #40 (Future work)

### Column: Sprint 1 - Ready
- #5, #6, #7, #9, #10, #11, #12 (Registration)
- #30, #31 (VAT)
- #20, #21 (Price/Invoice)
- #41, #42 (Legal UI)

### Column: In Progress
- #4 (Epic - HRasch)
- #30 (VAT - HRasch + DavidKeller)

### Column: In Review
- (None yet)

### Column: Done
- (None yet - Sprint just starting)

---

## 🔄 Team Koordination

### Daily Standups (15 min)
- **Morgens 9:00 CET**: Backend Team (backend_1, backend_2, backend_3)
- **Morgens 10:00 CET**: Frontend Team (frontend_1, frontend_2)
- **Morgens 11:00 CET**: Security & Ops (security_1, devops_1)
- **Morgens 14:00 CET**: Sprint Review (alle)

### Pull Request Reviews
- **Backend PRs**: Review by backend_1 (Tech Lead)
- **Frontend PRs**: Review by frontend_1 or frontend_2
- **Security PRs**: Review by security_1 + backend_1
- **Test PRs**: Review by qa_1 + backend_1

### Issue Blocking Chain
```
Legal (#29, #41, #42)
  ↓ Enables
Backend Registration (#5-#12)
  ↓ Enables
Frontend Registration UI
  ↓ Enables
Full Feature Testing (#45)
```

---

## 📝 Communication Channels

- **GitHub Issues**: Technical discussions + Progress updates
- **Pull Requests**: Code review + feedback
- **Project Board**: Sprint tracking + status overview
- **Code Comments**: Architecture decisions + patterns

---

## 🎯 Performance Metrics (Sprint 1 Target)

| Metric | Target | Owner |
|--------|--------|-------|
| Velocity | 50 Story Points | backend_1 |
| Code Coverage | > 80% | qa_1 |
| PR Review Time | < 4h | backend_1 |
| Test Pass Rate | 100% | qa_1 |
| Issue Burndown | Linear | backend_1 |

---

## ✅ Onboarding Checklist (für neue Team-Member)

```bash
# 1. Access vorbereiten
gh auth login
gh repo clone b2connect-dev/b2connect-platform

# 2. Lokale Umgebung
./DEVELOPMENT_ENVIRONMENT_SETUP.md

# 3. Issue Context laden
# - Backend Dev: .github/ISSUES_BACKEND_DEVELOPER.md
# - Frontend Dev: .github/ISSUES_FRONTEND_DEVELOPER.md
# - etc.

# 4. First Issue zuweisen
# - Warten auf Team-Zuweisung
# - oder: gh issue list --assignee @me

# 5. PR Process
# - Fork → Branch → Commit → Push → PR
# - Warten auf Review
# - Merge nach Approval
```

