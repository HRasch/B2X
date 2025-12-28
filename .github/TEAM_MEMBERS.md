# B2Connect Team - Fiktive Entwickler

**GitHub Organization**: b2connect-dev  
**Team Struktur**: 7 Rollen, 9 Entwickler  
**Status**: Active Development Sprint 1

---

## 👥 Team-Übersicht

### Backend-Team (3 Developer)

| Name | GitHub-ID | Rolle | Fokus | Status |
|------|-----------|-------|-------|--------|
| Backend Developer #1 | `backend_1` | Senior Backend Dev + Tech Lead | VAT, Tax Logic, Architecture | ✅ Active |
| Backend Developer #2 | `backend_2` | Backend Developer | User Registration, Auth | ⏳ Zuweisbar |
| Backend Developer #3 | `backend_3` | Backend Developer | Orders, Invoicing, E-Commerce | ⏳ Zuweisbar |

### Frontend-Team (2 Developer)

| Name | GitHub-ID | Rolle | Fokus | Status |
|------|-----------|-------|-------|--------|
| Frontend Developer #1 | `frontend_1` | Frontend Developer | Store Frontend, Components | ⏳ Zuweisbar |
| Frontend Developer #2 | `frontend_2` | Frontend Developer | Admin Dashboard, Theming | ⏳ Zuweisbar |

### Security & Ops (2 Engineer)

| Name | GitHub-ID | Rolle | Fokus | Status |
|------|-----------|-------|-------|--------|
| **David Keller** | `DavidKeller` | Security Engineer | Encryption, Audit, NIS2 | ⏳ Zuweisbar |
| **Sandra Berg** | `SandraBerg` | DevOps Engineer | Infrastructure, Monitoring | ⏳ Zuweisbar |

### QA & Legal (2 Specialist)

| Name | GitHub-ID | Rolle | Fokus | Status |
|------|-----------|-------|-------|--------|
| **Thomas Krause** | `ThomasKrause` | QA Engineer | Compliance Testing, Coverage | ⏳ Zuweisbar |
| **Julia Hoffmann** | `JuliaHoffmann` | Legal/Compliance Officer | GDPR, E-Commerce Law, Audit | ⏳ Zuweisbar |

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

#### Backend (HRasch + MaxMueller + LisaSchmidt)
- **HRasch** (Lead): #30 (VAT), #31 (Reverse Charge), Architecture (#4)
- **MaxMueller**: #5, #6, #7, #9, #10, #11, #12 (Registration Flow)
- **LisaSchmidt**: #20, #21, #29, #27 (Orders, Invoicing, Returns)

#### Frontend (AnnaWeber + TomBauer)
- **AnnaWeber**: #41, #42, #19 (Components, Legal UI)
- **TomBauer**: #17, #18 (Theme Config, Admin Prep)

#### Security (DavidKeller)
- **DavidKeller**: #30, #31 (Review), #32, #34 (Encryption)

#### QA (ThomasKrause)
- **ThomasKrause**: #45, #46 (Test Framework, E-Commerce Tests)

#### Legal (JuliaHoffmann)
- **JuliaHoffmann**: #29, #41, #42 (Return Policy, Terms, Privacy)

---

## 🎯 GitHub Issue Assignment Commands

```bash
# Backend Developer #1 - Max Müller
gh issue comment #5 --body "@MaxMueller - Zugewiesen für Sprint 1. User Registration Handler. Start: 2026-01-02"
gh issue edit #5 --assignee "MaxMueller" --add-label "sprint-1,in-progress"

gh issue comment #6 --body "@MaxMueller - Email Verification Logic. Abhängig von #5."
gh issue edit #6 --assignee "MaxMueller" --add-label "sprint-1"

# Backend Developer #2 - Lisa Schmidt  
gh issue comment #20 --body "@LisaSchmidt - Zugewiesen für Sprint 1. Price Calculation Service. Start: 2026-01-02"
gh issue edit #20 --assignee "LisaSchmidt" --add-label "sprint-1"

# Frontend Developer #1 - Anna Weber
gh issue comment #41 --body "@AnnaWeber - Zugewiesen für Sprint 1. AGB & Checkbox. Koordination mit @JuliaHoffmann (Legal)"
gh issue edit #41 --assignee "AnnaWeber" --add-label "sprint-1" --assignee "JuliaHoffmann"

# Security - David Keller
gh issue comment #30 --body "@DavidKeller & @HRasch - VAT-ID Validation. Security Review erforderlich. Priority: 🔴 P0"
gh issue edit #30 --assignee "DavidKeller" --assignee "HRasch" --add-label "security-review"

# QA - Thomas Krause
gh issue comment #45 --body "@ThomasKrause - Test Framework Setup. 15 E-Commerce Legal Tests. Start: 2026-01-09"
gh issue edit #45 --assignee "ThomasKrause" --add-label "sprint-2,testing"
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
- **Morgens 9:00 CET**: Backend Team (HRasch, MaxMueller, LisaSchmidt)
- **Morgens 10:00 CET**: Frontend Team (AnnaWeber, TomBauer)
- **Morgens 11:00 CET**: Security & Ops (DavidKeller, SandraBerg)
- **Morgens 14:00 CET**: Sprint Review (alle)

### Pull Request Reviews
- **Backend PRs**: Review by HRasch (Tech Lead)
- **Frontend PRs**: Review by AnnaWeber or TomBauer
- **Security PRs**: Review by DavidKeller + HRasch
- **Test PRs**: Review by ThomasKrause + HRasch

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
| Velocity | 50 Story Points | HRasch |
| Code Coverage | > 80% | ThomasKrause |
| PR Review Time | < 4h | HRasch |
| Test Pass Rate | 100% | ThomasKrause |
| Issue Burndown | Linear | HRasch |

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

