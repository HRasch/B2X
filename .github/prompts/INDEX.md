# 🎯 Development Cycle Prompts - Quick Reference

**Last Updated**: 30. Dezember 2025  
**Status**: ✅ All Core Prompts Created

---

## 📋 Quick Navigation

Use these prompts to trigger different phases of the development cycle:

### 🚀 Feature Development Cycle

#### 1. **START_FEATURE** - Initiate New Feature
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

#### 2. **REQUIREMENTS_ANALYSIS** - Detailed Analysis
**When to use**: Breaking down requirements across multiple domains
**Who runs it**: @Backend, @Frontend, @Security, @Architect → @SARAH consolidation
**Output**: Unified spec, technical analysis, implementation plan

---

#### 3. **SPRINT_CYCLE** - Sprint Management
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

#### 4. **CODE_REVIEW** - Code Quality Gate
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

#### 5. **RUN_TESTS** - Quality Assurance
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

#### 6. **BUG_ANALYSIS** - Bug Investigation
**When to use**: Bug reported, production incident, or test failure
**Who runs it**: @TechLead, @Backend, @Frontend
**Output**: Root cause analysis, fix implementation, verification

```
@TechLead: /bug-analysis
Severity: [P0-critical | P1-high | P2-medium | P3-low]
Component: [backend | frontend | infrastructure]
```

---

### 🔐 Security & Compliance

#### 7. **SECURITY_AUDIT** - Security Review
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

#### 8. **CREATE_ADR** - Architecture Decision Record
**When to use**: Major architectural decision, technology choice, pattern selection
**Who runs it**: @Architect
**Output**: Documented decision with rationale, alternatives, consequences

---

### 📦 Deployment & Release

#### 9. **DEPLOY** - Deployment & Release Management
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

#### 10. **FEATURE_HANDOVER** - Completion & Handover
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
- [x] CODE_REVIEW - Quality gate
- [x] RUN_TESTS - QA sign-off
- [x] SECURITY_AUDIT - Security review
- [x] BUG_ANALYSIS - Bug resolution
- [x] CREATE_ADR - Architecture decisions
- [x] DEPLOY - Deployment management
- [x] FEATURE_HANDOVER - Documentation & release
- [x] SPRINT_CYCLE - Sprint management

**Status**: ✅ All core prompts documented and ready for use

---

**Last Updated**: 30. Dezember 2025  
**Next Review**: 15. Januar 2026
