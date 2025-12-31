# B2Connect Documentation Map

**Quick Navigation for All Documentation**

---

## 📂 Directory Structure

### Root Level (`/B2Connect/`)
**Essential files only - 13 files total**
```
✅ README.md                          Project overview
✅ QUICK_START_GUIDE.md              Navigation hub (START HERE)
✅ .ai/knowledgebase/governance.md                      Decision-making framework
✅ PROJECT_DASHBOARD.md              Metrics & KPIs
✅ ACCESSIBILITY_COMPLIANCE_REPORT.md WCAG 2.1 AA audit status
✅ AGENT_SYSTEM_GETTING_STARTED.md   AI agent introduction
✅ COPILOT_INSTRUCTIONS_QUICK_REFERENCE.md Quick command reference
✅ COPILOT_INSTRUCTIONS_SETUP.md     Setup instructions
✅ ISSUE_TEMPLATE_STORE_LEGAL_COMPLIANCE.md GitHub issue template
✅ SPRINT_1_KICKOFF.md               Launch day reference
✅ SPRINT_3_COMPLETION_SUMMARY.md    Latest status snapshot
✅ SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md ⭐ NEXT SESSION GUIDE
✅ SPRINT_3_PHASE_2_EXECUTIVE_SUMMARY.md Phase 2 metrics
```

### `.github/` Folder
```
copilot-instructions.md         ← Comprehensive AI guide (CORE)
copilot-instructions-*.md       ← Role-specific instructions
copilot-instructions-quickstart.md ← 5-min overview

/docs/roles/                    ← Role-specific quick guides
├── backend-developer.md
├── frontend-developer.md
├── qa-engineer.md
├── devops-engineer.md
├── security-engineer.md
├── product-owner.md
├── tech-lead.md
└── legal-compliance.md

/docs/processes/                ← Operational processes
├── git-workflow.md
├── deployment.md
└── ...

/agents/                        ← AI agent configurations (30+ agents)
```

### `docs/` Folder
```
/archive/                       ← Historical documentation
├── backlog-refinement/         (3 files)
├── sprint-1-2/                 (18 files)
├── erp-provider/               (3 files)
├── development-process/        (6 files)
├── copilot-optimization/       (4 files)
└── ...                         (42 total archived files)

/compliance/                    ← Compliance testing
├── P0.6_ECOMMERCE_LEGAL_TESTS.md
├── P0.7_AI_ACT_TESTS.md
├── P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
├── P0.9_ERECHNUNG_TESTS.md
├── COMPLIANCE_TESTING_EXAMPLES.md
└── ...

/architecture/                  ← System design
├── DDD_BOUNDED_CONTEXTS.md
├── ASPIRE_GUIDE.md
├── ONION_ARCHITECTURE.md
├── WOLVERINE_HTTP_ENDPOINTS.md
└── ...

/security/                      ← Security patterns
├── AUDIT_LOGGING_IMPLEMENTATION.md
├── ENCRYPTION_PATTERNS.md
├── EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md
└── ...

/user-guides/                   ← User-facing documentation
├── en/                         (English)
│   ├── getting-started.md
│   ├── account-security.md
│   ├── checkout-process.md
│   └── ...
└── de/                         (German)
    ├── erste-schritte.md
    ├── kontosicherheit.md
    ├── checkout-prozess.md
    └── ...
```

---

## 🎯 Finding Information

### By Role
| Role | Start Here |
|------|-----------|
| Backend Developer | [.github/docs/roles/backend-developer.md](./../docs/roles/backend-developer.md) |
| Frontend Developer | [.github/docs/roles/frontend-developer.md](./../docs/roles/frontend-developer.md) |
| QA Engineer | [.github/docs/roles/qa-engineer.md](./../docs/roles/qa-engineer.md) |
| DevOps Engineer | [.github/docs/roles/devops-engineer.md](./../docs/roles/devops-engineer.md) |
| Security Engineer | [.github/docs/roles/security-engineer.md](./../docs/roles/security-engineer.md) |
| Product Owner | [.github/docs/roles/product-owner.md](./../docs/roles/product-owner.md) |
| Tech Lead | [.github/docs/roles/tech-lead.md](./../docs/roles/tech-lead.md) |
| Legal/Compliance | [.github/docs/roles/legal-compliance.md](./../docs/roles/legal-compliance.md) |

### By Topic
| Topic | Location |
|-------|----------|
| Architecture | `docs/architecture/` |
| Compliance Testing | `docs/compliance/` |
| Security Patterns | `docs/security/` |
| User Guides (EN/DE) | `docs/user-guides/{en,de}/` |
| AI Agents | `.github/agents/` |
| Operational Processes | `.github/docs/processes/` |
| Historical Docs | `docs/archive/` |

### By Phase
| Phase | Guide |
|-------|-------|
| Sprint 3 Phase 2.3 (Current) | [SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md](./../../SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md) |
| Phase 2.3 Metrics | [SPRINT_3_PHASE_2_EXECUTIVE_SUMMARY.md](./../../SPRINT_3_PHASE_2_EXECUTIVE_SUMMARY.md) |
| Overall Project Status | [SPRINT_3_COMPLETION_SUMMARY.md](./../../SPRINT_3_COMPLETION_SUMMARY.md) |
| Launch Reference | [SPRINT_1_KICKOFF.md](./../../SPRINT_1_KICKOFF.md) |

---

## 📖 Reading Order

### For New Team Members (30 minutes)
1. [QUICK_START_GUIDE.md](./../../QUICK_START_GUIDE.md) (5 min)
2. Your role guide in `.github/docs/roles/` (10 min)
3. [README.md](./../../README.md) (10 min)
4. [copilot-instructions-quickstart.md](../copilot-instructions-quickstart.md) (5 min)

### For Active Development (before each sprint)
1. [SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md](./../../SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md) (5 min)
2. [PROJECT_DASHBOARD.md](./../../PROJECT_DASHBOARD.md) (5 min)
3. Your role-specific guide in `.github/docs/roles/` (10 min)
4. Relevant architecture docs in `docs/architecture/` (15 min)

### For Historical Context
- `docs/archive/` contains all previous sprint documentation
- Use for understanding project evolution and decisions made

---

## 🔄 Documentation Updates

**Last Cleanup**: 29. December 2025
- ✅ 42 files archived to `docs/archive/`
- ✅ 8 role guides consolidated in `.github/docs/roles/`
- ✅ Root directory reduced to 13 essential files
- ✅ Navigation centralized in `QUICK_START_GUIDE.md`

**Next Review**: 15. January 2026

---

## 📋 Checklist for Team Members

Before starting work:
- [ ] Read your role guide in `.github/docs/roles/`
- [ ] Review current phase guide (SPRINT_3_*.md)
- [ ] Check PROJECT_DASHBOARD.md for metrics
- [ ] Know how to find archived context in `docs/archive/`

---

**Navigation Hub**: [QUICK_START_GUIDE.md](./../../QUICK_START_GUIDE.md)  
**Comprehensive Guide**: [copilot-instructions.md](../copilot-instructions.md)  
**Next Session**: [SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md](./../../SPRINT_3_PHASE_2_CONTINUATION_GUIDE.md)
