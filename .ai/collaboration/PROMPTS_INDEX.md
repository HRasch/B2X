---
docid: COLLAB-024
title: PROMPTS_INDEX
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: PRM-INDEX
title: Development Cycle Prompts - Quick Reference
owner: "@SARAH"
status: Active
---

# 🎯 Development Cycle Prompts - Quick Reference

**DocID**: `PRM-INDEX`  
**Last Updated**: 7. Januar 2026  
**Status**: ✅ All Core Prompts Created

See [DOCUMENT_REGISTRY.md](../../.ai/DOCUMENT_REGISTRY.md) for all DocIDs.

---

## 📋 Prompt Registry by DocID

| DocID | Command | Purpose | Owner |
|-------|---------|---------|-------|
| `PRM-001` | `/start-feature` | Feature initiation | @SARAH |
| `PRM-002` | `/code-review` | Code quality gate | @TechLead |
| `PRM-003` | `/run-tests` | QA & testing | @QA |
| `PRM-004` | `/deploy` | Deployment | @DevOps |
| `PRM-005` | `/security-audit` | Security review | @Security |
| `PRM-006` | `/adr-create` | Architecture decisions | @Architect |
| `PRM-007` | `/iteration-cycle` | Sprint management | @ScrumMaster |
| `PRM-008` | `/bug-analysis` | Bug investigation | @TechLead |
| `PRM-009` | `/feature-handover` | Feature completion | @ProductOwner |
| `PRM-010` | `/requirements-analysis` | Requirements analysis | Domain teams |
| `PRM-011` | `/agent-removal` | Agent deactivation | @SARAH |
| `PRM-012` | `/agent-creation` | Agent creation | @SARAH |
| `PRM-013` | `/context-optimization` | Token efficiency | @SARAH |
| `PRM-014` | `/subagent-delegation` | Delegation routing | @SARAH |
| `PRM-015` | `/project-cleanup` | Project cleanup | @SARAH |
| `PRM-016` | `/typescript-review` | TypeScript analysis | @TechLead |
| `PRM-017` | `/bug-null-check` | Quick null reference fix | @TechLead |
| `PRM-018` | `/bug-async-race` | Quick async race fix | @TechLead |
| `PRM-019` | `/bug-type-mismatch` | Quick type mismatch fix | @TechLead |
| `PRM-020` | `/bug-i18n-missing` | Quick i18n key fix | @TechLead |
| `PRM-021` | `/bug-lint-fix` | Quick linting fix | @TechLead |
| `PRM-022` | `/auto-lessons-learned` | Auto-generate lessons | @TechLead |

---

## 📋 Quick Navigation

Use these prompts to trigger different phases of the development cycle:

### 🚀 Feature Development Cycle

#### 1. **START_FEATURE** (`PRM-001`) - Initiate New Feature
**When to use**: You have a new feature request or user story
**Who runs it**: @ProductOwner, @SARAH
**Output**: Feature specification, task breakdown, team assignments

```
@SARAH: /start-feature
Title: [Feature Title]
User Story: [As a..., I want..., so that...]
Priority: P0 | P1 | P2 | P3
```

---

#### 2. **REQUIREMENTS_ANALYSIS** (`PRM-010`) - Detailed Analysis
**When to use**: Breaking down requirements across multiple domains
**Who runs it**: @Backend, @Frontend, @Security, @Architect → @SARAH consolidation
**Output**: Unified spec, technical analysis, implementation plan

---

#### 3. **SPRINT_CYCLE** (`PRM-007`) - Sprint Management
**When to use**: Sprint planning, daily execution, retrospective
**Who runs it**: @ScrumMaster
**Output**: Sprint plan, daily standups, retrospective notes

```
@ScrumMaster: /sprint-cycle
Phase: [planning | execution | retrospective]
Sprint: [Sprint number and dates]
Capacity: [Available story points]
```

---

### 💻 Development & Implementation

#### 4. **CODE_REVIEW** (`PRM-002`) - Code Quality Gate
**When to use**: PR ready for review
**Who runs it**: @TechLead, domain experts, @Security
**Output**: Review approval or change requests

```
@TechLead: /code-review
PR: [Link to PR]
Component: backend | frontend | infrastructure
Priority: critical | standard | minor
```

---

#### 5. **RUN_TESTS** (`PRM-003`) - Quality Assurance
**When to use**: Feature ready for testing or before merge
**Who runs it**: @QA
**Output**: Test report, defect list, sign-off

```
@QA: /run-tests
Component: backend | frontend | all
Scope: unit | integration | e2e | all
Environment: development | staging | production
```

---

#### 6. **BUG_ANALYSIS** (`PRM-008`) - Bug Investigation
**When to use**: Bug reported, production incident, or test failure
**Who runs it**: @TechLead, @Backend, @Frontend
**Output**: Root cause analysis, fix implementation, verification

```
@TechLead: /bug-analysis
Severity: [P0-critical | P1-high | P2-medium | P3-low]
Component: [backend | frontend | infrastructure]
```

#### 6.1 **BUG_NULL_CHECK** (`PRM-017`) - Quick Null Reference Fix
**When to use**: Null reference errors in code
**Who runs it**: @TechLead, @Frontend
**Output**: Defensive null checks applied

```
@TechLead: /bug-null-check
File: [path/to/file.vue]
Variable: [nullVariable]
```

#### 6.2 **BUG_ASYNC_RACE** (`PRM-018`) - Quick Async Race Fix
**When to use**: Race conditions in async operations
**Who runs it**: @TechLead, @Frontend
**Output**: Race condition prevention applied

```
@TechLead: /bug-async-race
Component: [ComponentName]
Operation: [asyncOperation]
```

#### 6.3 **BUG_TYPE_MISMATCH** (`PRM-019`) - Quick Type Mismatch Fix
**When to use**: TypeScript type errors
**Who runs it**: @TechLead, @Frontend
**Output**: Type safety restored

```
@TechLead: /bug-type-mismatch
File: [path/to/file.ts]
Error: [type error message]
```

#### 6.4 **BUG_I18N_MISSING** (`PRM-020`) - Quick i18n Key Fix
**When to use**: Missing translation keys
**Who runs it**: @TechLead, @Frontend
**Output**: Translation keys added

```
@TechLead: /bug-i18n-missing
Key: [missing.key]
Languages: [en, de, fr]
```

#### 6.5 **BUG_LINT_FIX** (`PRM-021`) - Quick Linting Fix
**When to use**: Linting errors need fixing
**Who runs it**: @TechLead, @Frontend
**Output**: Code style corrected

```
@TechLead: /bug-lint-fix
File: [path/to/file.vue]
Rule: [eslint-rule-name]
```

#### 7. **AUTO_LESSONS_LEARNED** (`PRM-022`) - Auto-Generate Lessons
**When to use**: After fixing a bug to capture learnings
**Who runs it**: @TechLead
**Output**: Formatted lesson entry for knowledge base

```
@TechLead: /auto-lessons-learned
Bug: [brief description]
Fix: [what was done]
Lesson: [key takeaway]
```

#### 8. **TYPESCRIPT_REVIEW** (`PRM-016`) - TypeScript Analysis
**When to use**: Code review, refactoring, type safety validation
**Who runs it**: @TechLead, @Frontend
**Output**: Type analysis, symbol usage, recommendations

```
@TechLead: /typescript-review
Component: [frontend | admin | management]
Scope: [file-path | component-name | directory]
Focus: [types | symbols | usage | all]
```

---

### 🔐 Security & Compliance

#### 8. **SECURITY_AUDIT** - Security Review
**When to use**: Before merge, security-sensitive changes, compliance audit
**Who runs it**: @Security
**Output**: Security assessment, vulnerability report, remediation plan

```
@Security: /security-audit
Component: [backend | frontend | infrastructure | all]
Scope: [security-review | compliance-check | penetration | full]
Risk Level: [low | medium | high | critical]
```

---

### 📚 Architecture & Design

#### 9. **CREATE_ADR** - Architecture Decision Record
**When to use**: Major architectural decision, technology choice, pattern selection
**Who runs it**: @Architect
**Output**: Documented decision with rationale, alternatives, consequences

---

### 📦 Deployment & Release

#### 10. **DEPLOY** - Deployment & Release Management
**When to use**: Feature/release ready for deployment
**Who runs it**: @DevOps
**Output**: Deployment confirmation, release notes, rollback procedure

```
@DevOps: /deploy
Environment: [staging | production]
Version: [x.y.z semantic version]
Components: [List what's being deployed]
Type: [feature | hotfix | maintenance]
```

---

#### 11. **FEATURE_HANDOVER** - Completion & Handover
**When to use**: Feature development complete, ready for documentation
**Who runs it**: @ProductOwner, @TechLead
**Output**: Feature documentation, stakeholder handover, deployment readiness

---

## 🔄 Complete Development Workflow

### Typical Feature Lifecycle:

```
START_FEATURE
    ↓
REQUIREMENTS_ANALYSIS
    ↓
SPRINT_CYCLE (Planning)
    ↓
SPRINT_CYCLE (Execution - 1-2 weeks)
    ├→ CODE_REVIEW (daily PRs)
    ├→ RUN_TESTS (throughout)
    ├→ BUG_ANALYSIS (if issues found)
    └→ SECURITY_AUDIT (before merge)
    ↓
SPRINT_CYCLE (Retrospective)
    ↓
FEATURE_HANDOVER
    ↓
DEPLOY
```

---

## 🎯 By Role

### @ProductOwner
- START_FEATURE
- REQUIREMENTS_ANALYSIS
- FEATURE_HANDOVER

### @Backend / @Frontend
- Part of REQUIREMENTS_ANALYSIS
- CODE_REVIEW
- SPRINT_CYCLE
- BUG_ANALYSIS

### @TechLead
- CODE_REVIEW (lead)
- BUG_ANALYSIS (complex cases)
- CREATE_ADR (architecture guidance)

### @QA
- RUN_TESTS
- BUG_ANALYSIS (reporting)
- FEATURE_HANDOVER (sign-off)

### @Security
- SECURITY_AUDIT
- CODE_REVIEW (security aspects)

### @Architect
- REQUIREMENTS_ANALYSIS (architecture aspects)
- CREATE_ADR

### @DevOps
- DEPLOY
- SPRINT_CYCLE (infrastructure tasks)

### @ScrumMaster
- SPRINT_CYCLE (all phases)
- FEATURE_HANDOVER (timeline coordination)

### @SARAH (Coordinator)
- START_FEATURE (coordination)
- REQUIREMENTS_ANALYSIS (consolidation)
- Strategic oversight

---

## 📊 Documentation & Artifacts

Each prompt generates specific artifacts stored in `.ai/`:

### By Prompt:
| Prompt | Output Location | Artifact Type |
|---|---|---|
| START_FEATURE | `.ai/requirements/` | Feature specs |
| REQUIREMENTS_ANALYSIS | `.ai/requirements/` | Consolidated specs |
| SPRINT_CYCLE | `.ai/sprint/` | Planning & tracking |
| CODE_REVIEW | GitHub PRs | Review comments |
| RUN_TESTS | `.ai/logs/` | Test reports |
| BUG_ANALYSIS | `.ai/issues/` | Root cause docs |
| SECURITY_AUDIT | `.ai/decisions/` | Security assessment |
| CREATE_ADR | `.ai/decisions/` | ADR documents |
| DEPLOY | `.ai/logs/` | Deployment logs |
| FEATURE_HANDOVER | `.ai/handovers/` | Feature docs |

---

## 🚨 When to Use What

### You need to start new work
→ **START_FEATURE**

### You're breaking down requirements
→ **REQUIREMENTS_ANALYSIS**

### You're planning sprint work
→ **SPRINT_CYCLE (Planning)**

### You have a PR to review
→ **CODE_REVIEW**

### Feature is ready for testing
→ **RUN_TESTS**

### You found a bug
→ **BUG_ANALYSIS**

### You need to make an architectural decision
→ **CREATE_ADR**

### You need security review
→ **SECURITY_AUDIT**

### Feature is done, ready to go live
→ **FEATURE_HANDOVER**

### Ready to deploy to production
→ **DEPLOY**

---

## 💡 Pro Tips

1. **Chain prompts** - Use multiple prompts in sequence for complex work
2. **Parallel execution** - Many prompts can run in parallel (use tag mentions)
3. **Documentation** - Each prompt auto-generates documentation
4. **Sign-off workflow** - Each prompt includes approval checkpoints
5. **Traceability** - Link prompts via GitHub issues and ADRs

---

## 📞 Need Help?

### For prompt guidance
→ Check individual prompt files in `.github/prompts/`

### For agent coordination
→ Reference [AGENT_TEAM_REGISTRY.md](../AGENT_TEAM_REGISTRY.md)

### For general workflow questions
→ Contact @SARAH (coordinator)

---

## ✅ Prompt Checklist

- [x] START_FEATURE - Feature initiation
- [x] REQUIREMENTS_ANALYSIS - Multi-agent analysis
- [x] CODE_REVIEW - Code gate
- [x] RUN_TESTS - QA sign-off
- [x] SECURITY_AUDIT - Security review
- [x] BUG_ANALYSIS - Bug resolution
- [x] BUG_NULL_CHECK - Quick null reference fix
- [x] BUG_ASYNC_RACE - Quick async race fix
- [x] BUG_TYPE_MISMATCH - Quick type mismatch fix
- [x] BUG_I18N_MISSING - Quick i18n key fix
- [x] BUG_LINT_FIX - Quick linting fix
- [x] AUTO_LESSONS_LEARNED - Auto-generate lessons
- [x] TYPESCRIPT_REVIEW - TypeScript analysis
- [x] CREATE_ADR - Architecture decisions
- [x] DEPLOY - Deployment management
- [x] FEATURE_HANDOVER - Documentation & release
- [x] SPRINT_CYCLE - Sprint management
- [x] AGENT_REMOVAL - Agent deactivation
- [x] AGENT_CREATION - Agent creation
- [x] CONTEXT_OPTIMIZATION - Token efficiency
- [x] SUBAGENT_DELEGATION - Delegation routing
- [x] PROJECT_CLEANUP - Project cleanup

**Status**: ✅ All 22 core prompts documented and ready for use

---

**Last Updated**: 7. Januar 2026  
**Next Review**: 15. Februar 2026
