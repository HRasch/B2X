# Sprint 1 - GitHub Project Board

**Created**: 28. Dezember 2025  
**Sprint Duration**: 28.12.2025 - 24.01.2026 (4 Wochen)  
**Team Size**: 9 Developer  
**Sprint Goal**: P0.6 E-Commerce Legal Compliance Foundation  
**Board Link**: https://github.com/b2connect-dev/b2connect-platform/projects/1

---

## 📊 Board Status Overview

| Column | Issues | Story Points | Progress |
|--------|--------|--------------|----------|
| **Backlog** | 4 | - | Future work |
| **Ready for Sprint** | 14 | 60 | ✅ Assigned |
| **In Progress** | 2 | 10 | 🔄 Active |
| **In Review** | 0 | 0 | ⏳ Pending |
| **Done** | 0 | 0 | - |

---

## 🎯 Sprint 1 Board Layout (Kanban)

### Column 1: BACKLOG (Future - Week 3+)

```
📌 Feature Ideas
├── #39: GraphQL API Support
├── #40: Redis Caching Layer
├── #43: Elasticsearch Full-Text Search
└── #44: Multi-Currency Support
```

**Owner**: Backlog Grooming (backend_1 + Product Team)

---

### Column 2: READY FOR SPRINT (Sprint Planning)

#### 🔐 Security & Foundation (P0.2, P0.3)
```
├── #30: VAT-ID Validation via VIES API
│   ├── Assignee: @HRasch (Lead), @DavidKeller (Review)
│   ├── Story Points: 8
│   ├── Priority: 🔴 P0
│   ├── Dependencies: None
│   └── Status: Ready
│
├── #31: Reverse Charge Logic (B2B)
│   ├── Assignee: @HRasch
│   ├── Story Points: 5
│   ├── Priority: 🔴 P0
│   ├── Dependencies: #30
│   └── Status: Ready
│
└── #32: Invoice Encryption (AES-256)
    ├── Assignee: @DavidKeller
    ├── Story Points: 8
    ├── Priority: 🔴 P0
    ├── Dependencies: #20
    └── Status: Ready
```

#### 👤 User Registration (F1.1)
```
├── #5: Wolverine HTTP Handler for Registration
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 5
│   ├── Priority: 🔴 P0
│   ├── Dependencies: None
│   └── Status: Ready - Implementation starts 02.01.2026
│
├── #6: Email Verification & Confirmation
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 3
│   ├── Priority: 🔴 P0
│   ├── Dependencies: #5
│   └── Status: Ready
│
├── #7: JWT Token Generation & Refresh
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 5
│   ├── Priority: 🔴 P0
│   ├── Dependencies: #5
│   └── Status: Ready
│
├── #9: Multi-Tenant Isolation (Registration)
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 3
│   ├── Priority: 🔴 P0
│   ├── Dependencies: #5
│   └── Status: Ready
│
├── #10: Password Policy Enforcement
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 2
│   ├── Priority: 🟡 P1
│   ├── Dependencies: #5
│   └── Status: Ready
│
├── #11: Failed Login Lockout (5+ attempts)
│   ├── Assignee: @MaxMueller
│   ├── Story Points: 3
│   ├── Priority: 🟡 P1
│   ├── Dependencies: #5
│   └── Status: Ready
│
└── #12: Session Timeout (15 min inactivity)
    ├── Assignee: @MaxMueller
    ├── Story Points: 2
    ├── Priority: 🟡 P1
    ├── Dependencies: #7
    └── Status: Ready
```

#### 💰 Pricing & Invoicing (F1.3)
```
├── #20: Price Calculation Service (B2C/B2B)
│   ├── Assignee: @LisaSchmidt
│   ├── Story Points: 8
│   ├── Priority: 🔴 P0
│   ├── Dependencies: #30, #31
│   └── Status: Ready
│
├── #21: Shipping Cost Calculation
│   ├── Assignee: @LisaSchmidt
│   ├── Story Points: 5
│   ├── Priority: 🔴 P0
│   ├── Dependencies: None
│   └── Status: Ready
│
├── #27: Return Label Generation
│   ├── Assignee: @LisaSchmidt
│   ├── Story Points: 5
│   ├── Priority: 🟡 P1
│   ├── Dependencies: #20
│   └── Status: Ready
│
└── #29: Invoice Generation & Storage (10-Year)
    ├── Assignee: @LisaSchmidt + @JuliaHoffmann (Legal Review)
    ├── Story Points: 8
    ├── Priority: 🔴 P0
    ├── Dependencies: #20, #32
    └── Status: Ready
```

#### 📖 Legal & Compliance (F1.4)
```
├── #41: AGB & Widerrufsbelehrung (Frontend)
│   ├── Assignee: @AnnaWeber + @JuliaHoffmann (Content)
│   ├── Story Points: 5
│   ├── Priority: 🔴 P0
│   ├── Dependencies: None
│   └── Status: Ready - UI Design Complete
│
└── #42: Datenschutzerklärung & Impressum
    ├── Assignee: @AnnaWeber + @JuliaHoffmann (Content)
    ├── Story Points: 3
    ├── Priority: 🔴 P0
    ├── Dependencies: #41
    └── Status: Ready
```

#### 🎨 Frontend Components (F1.1)
```
└── #19: Base Button Component (accessible)
    ├── Assignee: @AnnaWeber
    ├── Story Points: 2
    ├── Priority: 🟡 P1
    ├── Dependencies: None
    └── Status: Ready - Design Approved
```

---

### Column 3: IN PROGRESS 🔄

```
Epic #4: Customer Registration Flow
├── Owner: @HRasch (Epic Lead)
├── Status: Analysis Phase
├── Started: 28.12.2025
├── Target Complete: 17.01.2026 (3 weeks)
└── Linked Issues:
    ├── #5 (Handler) - @MaxMueller
    ├── #6 (Verification) - @MaxMueller
    ├── #7 (JWT) - @MaxMueller
    ├── #9 (Tenancy) - @MaxMueller
    ├── #41 (Legal UI) - @AnnaWeber
    └── #42 (Legal Docs) - @AnnaWeber

Issue #30: VAT-ID Validation
├── Owner: @HRasch + @DavidKeller (Security Review)
├── Status: Architecture Design
├── Started: 28.12.2025
├── Blocked By: None
├── Blocks: #31, #20
└── Current Work:
    └── 📝 Designing VIES API integration
```

---

### Column 4: IN REVIEW ⏳

```
(None yet - Review phase begins on 04.01.2026 after first features complete)
```

---

### Column 5: DONE ✅

```
(Sprint just started - no completed items yet)
(Target: 12-15 issues by 24.01.2026)
```

---

## 📋 Sprint 1 Timeline & Milestones

### Week 1: 28.12.2025 - 03.01.2026 (Analysis & Setup)
```
28.12 (Mon): Sprint Planning
             - Team assigned to issues
             - Architecture review (#30, #4)
             - Environment setup
             
29-30.12 (Tue-Wed): Holiday (skip)

02.01 (Thu): Development Starts
             - @MaxMueller: #5 Handler implementation
             - @HRasch: #30 VAT Validation design
             - @AnnaWeber: #41, #42 UI layout
             - @LisaSchmidt: #20 Price logic
             
03.01 (Fri): First Review
             - Architecture review meeting
             - Progress status: 15-20% complete
             - Target: 5-8 story points completed
```

### Week 2: 04.01.2026 - 10.01.2026 (Development)
```
Target Velocity: 15-20 story points
Expected Completions:
  - #5, #6, #19 (Registration basics)
  - #20, #21 (Pricing)
  - #41, #42 (Legal UI)
  
Parallel Work:
  - Security review of #30, #31
  - Test framework setup (#45)
```

### Week 3: 11.01.2026 - 17.01.2026 (Integration & Testing)
```
Target Velocity: 15-20 story points
Expected Completions:
  - #30, #31 (VAT fully tested)
  - #29 (Invoice generation)
  - #32 (Encryption)
  - #12, #11, #10 (Auth policies)
  
Testing Phase Begins:
  - Unit tests (80%+ coverage)
  - Integration tests
  - Security review (@DavidKeller)
```

### Week 4: 18.01.2026 - 24.01.2026 (Final & Release)
```
Target Velocity: 10-15 story points (cleanup)
Expected Completions:
  - Remaining issues
  - Bug fixes
  - Documentation
  - Sprint Review & Retrospective on 24.01
  
Go/No-Go Decision: 24.01.2026 (Phase 1 Entry Gate)
```

---

## 👥 Team Roles & Responsibilities

### 🧑‍💼 Tech Lead & Architecture (1)
**@HRasch** - Senior Backend Developer + Tech Lead
- Responsible for: Epic #4, Issues #30, #31 (VAT)
- Capacity: 40h/week (managing + coding)
- Key Activities:
  - Architecture reviews (Epic #4)
  - Code reviews (all PRs)
  - Risk management & escalations
  - Daily standup lead
- Blocking Authority: Yes (architecture decisions)

### 👨‍💻 Backend Team (2)
**@MaxMueller** - Mid-Level Backend Developer
- Responsible for: Issues #5-#12 (Registration flow)
- Capacity: 40h/week
- Skills: Wolverine patterns, CQRS, fluentValidation
- Starts: 02.01.2026

**@LisaSchmidt** - Backend Developer
- Responsible for: Issues #20, #21, #27, #29 (Pricing & Invoicing)
- Capacity: 40h/week
- Skills: EF Core, payment integrations, tax logic
- Starts: 02.01.2026

### 👩‍💻 Frontend Team (2)
**@AnnaWeber** - Frontend Developer
- Responsible for: Issues #41, #42, #19 (Legal UI, Components)
- Capacity: 40h/week
- Skills: Vue 3, Composition API, Tailwind CSS
- Starts: 02.01.2026

**@TomBauer** - Frontend Developer
- Responsible for: Admin dashboard prep (#17, #18)
- Capacity: 30h/week (part support)
- Skills: Vue 3, forms, state management
- Starts: 11.01.2026

### 🔐 Security Engineer (1)
**@DavidKeller** - Security Specialist
- Responsible for: Issues #30, #31 (Security Review), #32 (Encryption)
- Capacity: 20h/week (code review + implementation)
- Key Activities:
  - Security code review (#30, #31)
  - Encryption implementation (#32)
  - Threat modeling
- Blocking Authority: Yes (security)

### ⚖️ Legal/Compliance Officer (1)
**@JuliaHoffmann** - Legal Specialist
- Responsible for: Issues #29, #41, #42 (Legal content)
- Capacity: 20h/week (content + review)
- Key Activities:
  - AGB content (#41)
  - Privacy policy (#42)
  - Invoice legal requirements (#29)
  - Compliance review
- Blocking Authority: Yes (legal decisions)

### 🧪 QA Engineer (1)
**@ThomasKrause** - QA Automation
- Responsible for: Testing framework, compliance tests
- Capacity: 20h/week (Week 2+ focus)
- Key Activities:
  - Test framework setup (#45)
  - 15 E-Commerce Legal Tests
  - Coverage monitoring
  - Regression testing
- Starts: 04.01.2026

### ⚙️ DevOps Engineer (1)
**@SandraBerg** - DevOps/Infrastructure (backup support)
- Capacity: 10h/week (on-demand)
- Key Activities:
  - CI/CD pipeline
  - Test environment
  - Performance monitoring
- Blocking Authority: No (unless infrastructure)

---

## 📊 Burndown Chart (Expected)

```
Story Points Remaining (Target 60 points)

60 ├─ [BACKLOG START]
   │ 
50 ├─  ╲
   │    ╲    (Week 1: Analysis, Setup)
40 ├─     ╲╲
   │        ╲╲  (Week 2: Development)
30 ├─         ╲╲╲
   │            ╲╲╲ (Week 3: Integration)
20 ├─              ╲
   │                ╲  (Week 4: Final)
10 ├─                 ╲
   │                   ╲___
0  └─────────────────────────
   W1   W2   W3   W4   Goal
```

**Ideal Path**: Linear decline (12-15 points/week)  
**Actual Path**: TBD (tracked daily)

---

## 🎯 Success Metrics

| Metric | Target | Owner | Check |
|--------|--------|-------|-------|
| Velocity | 50+ story points | @HRasch | Weekly |
| Code Coverage | > 80% | @ThomasKrause | Daily |
| PR Review Time | < 4 hours | @HRasch | Daily |
| Test Pass Rate | 100% | @ThomasKrause | CI/CD |
| Zero High/Critical Bugs | 100% | @DavidKeller | Sprint Review |
| Documentation | 100% complete | @HRasch | Sprint End |

---

## ⚠️ Risk Register

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| VAT-ID API Outages (#30) | Blocks checkout | Medium | Use mock + fallback |
| Legal Review Delays (#41, #42) | Blocks release | Low | @JuliaHoffmann assigned early |
| Integration Issues | Delay testing | Medium | Daily integration testing |
| Performance Problems | Blocks Go/No-Go | Low | Load testing from Week 2 |

---

## 📞 Communication & Escalation

### Daily Standup
- **Time**: 09:00 CET
- **Participants**: All 9 team members
- **Duration**: 15 minutes
- **Format**: 
  - What did you do yesterday?
  - What are you doing today?
  - Any blockers?

### Issue Updates
- **Frequency**: Daily (via GitHub comments)
- **Format**: 
  ```
  Status: @mention if blocked
  Progress: X% complete
  Next Steps: What's next
  ```

### Code Review
- **Target Time**: < 4 hours
- **Minimum Reviewers**: 1 (2 for security/legal)
- **Approval Required**: All comments resolved

### Escalation Path
```
Issue Blocker
  ↓
Daily Standup (9:00)
  ↓
Tech Lead Review (9:30)
  ↓
Architectural Review (if needed, 10:00)
  ↓
C-Level Escalation (if critical)
```

---

## 📝 Issue Lifecycle Process

### 1️⃣ READY State
```
Issue created in Backlog
  ↓
Requirements clear? → Accept Criteria defined
  ↓
Dependencies documented
  ↓
Move to "Ready for Sprint"
```

### 2️⃣ IN PROGRESS State
```
Developer assigned
  ↓
Comment: "Starting work on [date]"
  ↓
Create feature branch (git)
  ↓
Daily progress comments
  ↓
70% done? → Create Pull Request
```

### 3️⃣ IN REVIEW State
```
PR created
  ↓
Code review (1+ reviewers)
  ↓
Security review (if needed)
  ↓
Legal review (if needed)
  ↓
All checks pass? → Approve
  ↓
Merge to develop
```

### 4️⃣ DONE State
```
Merged to develop
  ↓
CI/CD tests pass
  ↓
Comment: "Completed on [date]"
  ↓
Move to "Done" column
  ↓
Verify in staging environment
```

---

## 🔗 Related Documentation

- **Team Roles**: [.github/TEAM_MEMBERS.md](.github/TEAM_MEMBERS.md)
- **Sprint Planning**: [SPRINT_1_KICKOFF.md](../SPRINT_1_KICKOFF.md)
- **Issue Index**: [.github/GITHUB_ISSUES_INDEX.md](.github/GITHUB_ISSUES_INDEX.md)
- **Backend Guide**: [.github/ISSUES_BACKEND_DEVELOPER.md](.github/ISSUES_BACKEND_DEVELOPER.md)
- **Frontend Guide**: [.github/ISSUES_FRONTEND_DEVELOPER.md](.github/ISSUES_FRONTEND_DEVELOPER.md)

---

## ✅ Board Verification Checklist

Before Sprint Start (28.12.2025):

- [x] Project board created (GitHub Projects v2)
- [x] 14 issues added to "Ready for Sprint" column
- [x] Team members assigned
- [x] Dependencies documented
- [x] Estimate (story points) set for all issues
- [x] Acceptance criteria clear
- [x] Risk register reviewed
- [x] Success metrics defined

---

**Last Updated**: 28. Dezember 2025  
**Next Review**: 02. Januar 2026 (Sprint Start)  
**Board Owner**: @HRasch

