---
docid: COLLAB-006
title: AGENT_ARTIFACT_RESPONSIBILITY
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# 📋 Agent Responsibility Delegation - `.ai/` Folder Management

**Status**: ✅ ACTIVE  
**Date**: 30. Dezember 2025  
**Scope**: All agents responsible for managing `.ai/` artifacts in their domain

---

## Overview

Each agent is **responsible for creating, organizing, and maintaining** artifacts in the `.ai/` folder related to their domain expertise. This is not optional documentation—it's core to how agents manage and share knowledge.

---

## Agent Responsibility Matrix

### Core Teams

| Agent | Domain | `.ai/` Folder | Artifacts Owned |
|-------|--------|---------------|-----------------|
| **@ProductOwner** | Requirements & Features | `requirements/` `handovers/` | Feature specs, user stories, requirements analysis, feature documentation, acceptance criteria |
| **@Architect** | System Design & Decisions | `decisions/` | Architecture Decision Records (ADRs), design patterns, system design docs, technology choices, scalability analysis |
| **@ScrumMaster** | Process & Sprint Management | `sprint/` `status/` | Sprint plans, daily standups, velocity tracking, task status, retrospectives, blockers, team metrics |
| **@Security** | Security & Compliance | `compliance/` | Security audits, compliance checklists, vulnerability reports, threat modeling, incident reports, remediation plans |
| **@Legal** | Legal Compliance | `compliance/` | Legal compliance documents, GDPR reviews, contractual analysis, policy updates, compliance tracking |
| **@TechLead** | Code Quality & Knowledge | `knowledgebase/` `decisions/` | Technical guides, best practices, code patterns, performance analysis, mentoring docs, architectural notes |

### Implementation Teams

| Agent | Domain | `.ai/` Folder | Artifacts Owned |
|-------|--------|---------------|-----------------|
| **@Backend** | Backend Implementation | `decisions/` `knowledgebase/` | Backend architecture decisions, API documentation, data model docs, service contracts, implementation guides |
| **@Frontend** | Frontend Implementation | `decisions/` `knowledgebase/` | Frontend architecture decisions, component documentation, state management docs, design system specs, UI patterns |
| **@QA** | Testing & Quality | `logs/` | Test reports, test plans, QA findings, test automation docs, test coverage reports |
| **@DevOps** | Infrastructure & Deployment | `config/` `logs/` | Infrastructure configuration, deployment logs, monitoring setup, CI/CD documentation, runbooks |

### Governance & Coordination

| Agent | Domain | `.ai/` Folder | Artifacts Owned |
|-------|--------|---------------|-----------------|
| **@SARAH** | Coordination & Governance | `collaboration/` `templates/` `workflows/` | Coordination framework, GitHub templates, workflow orchestration, agent coordination docs, governance records |

### Cross-Domain & Issue-Specific

| Role | Domain | `.ai/` Folder | Artifacts Owned |
|------|--------|---------------|-----------------|
| **Issue Owner** | Issue-Specific Work | `issues/{issue-id}/` | Issue-specific collaboration, progress notes, blockers, decisions, design discussions, learnings |

---

## Artifact Types & Locations

### `.ai/requirements/` - Feature Specifications
**Owner**: @ProductOwner  
**Type**: Living specifications that evolve throughout development

**Include**:
- Feature requirements and acceptance criteria
- User stories and use cases
- Design mockups/wireframes (if UI-heavy)
- Requirements analysis documents
- Risk assessment
- Handover documentation

**Example Structure**:
```
requirements/
├── FEATURE-001-store-shipping/
│   ├── specification.md (User stories + AC)
│   ├── requirements-analysis.md (@ProductOwner + domain experts)
│   ├── design-mockups/ (if applicable)
│   └── handover.md (feature documentation)
├── FEATURE-002-payment-gateway/
│   └── ...
```

**Update Frequency**: As requirements change, throughout sprint

---

### `.ai/decisions/` - Architecture & Design Decisions
**Owner**: @Architect (with input from domain teams)  
**Type**: Architecture Decision Records (ADRs) + implementation notes

**Include**:
- ADRs for significant technical decisions
- Design patterns applied
- Technology choices and rationale
- Scalability and performance considerations
- Trade-offs made
- Implementation-specific architecture notes

**Example Structure**:
```
decisions/
├── ADR-001-microservices-architecture.md (@Architect)
├── ADR-002-caching-strategy.md (@Architect + @Backend)
├── ADR-003-state-management.md (@Frontend)
├── backend-architecture-notes.md (@Backend)
└── frontend-component-hierarchy.md (@Frontend)
```

**Update Frequency**: When major decisions are made, refactor notes added during implementation

---

### `.ai/sprint/` - Sprint Planning & Tracking
**Owner**: @ScrumMaster  
**Type**: Active sprint documentation

**Include**:
- Sprint plans (goal, capacity, breakdown)
- Daily standups
- Velocity tracking
- Risk assessment
- Completion status
- Retrospective notes

**Example Structure**:
```
sprint/
├── SPRINT-12-jan-6-17/
│   ├── plan.md (@ScrumMaster)
│   ├── daily-standups.md (updated daily)
│   ├── velocity-tracking.md (updated daily)
│   ├── blockers.md (active issues)
│   ├── risks.md (risk management)
│   └── retrospective.md (end of sprint)
├── SPRINT-11-dec-20-30/
│   └── ... (completed sprint)
```

**Update Frequency**: Daily during sprint, finalized at sprint end

---

### `.ai/compliance/` - Security & Legal Compliance
**Owner**: @Security (primary), @Legal (secondary)  
**Type**: Compliance artifacts

**Include**:
- Security audits and findings
- Compliance checklists
- Vulnerability reports
- Threat modeling documents
- Remediation plans
- GDPR/legal compliance docs
- Compliance tracking

**Example Structure**:
```
compliance/
├── security-audit-2025-01.md (@Security)
├── vulnerability-reports/ (@Security)
│   ├── VULN-001-xss-injection.md
│   └── remediation-plan.md
├── gdpr-compliance-checklist.md (@Legal + @Security)
└── compliance-tracking.md (@Security)
```

**Update Frequency**: As audits happen, continuously as vulnerabilities found/fixed

---

### `.ai/knowledgebase/` - Technical Knowledge
**Owner**: @TechLead (with contributions from all teams)  
**Type**: Technical guidance and best practices

**Include**:
- Technical guides and best practices
- Code patterns and examples
- Performance analysis
- Troubleshooting runbooks
- API documentation (conceptual)
- Testing strategies
- Architecture patterns

**Example Structure**:
```
knowledgebase/
├── backend-patterns.md (@Backend + @TechLead)
├── frontend-components.md (@Frontend + @TechLead)
├── testing-strategies.md (@QA + @TechLead)
├── performance-optimization.md (@TechLead)
└── troubleshooting/
    ├── common-backend-issues.md
    └── common-frontend-issues.md
```

**Update Frequency**: As team discovers patterns, continuously improved

---

### `.ai/config/` - Infrastructure Configuration
**Owner**: @DevOps  
**Type**: Infrastructure and deployment config

**Include**:
- Infrastructure-as-Code documentation
- Deployment configurations
- Environment setup guides
- CI/CD pipeline configuration
- Secrets management
- Monitoring setup

**Example Structure**:
```
config/
├── kubernetes-deployment.md (@DevOps)
├── ci-cd-pipeline.md (@DevOps)
├── environment-setup.md (@DevOps)
├── monitoring-setup.md (@DevOps)
└── secrets-management.md (@DevOps)
```

**Update Frequency**: As infrastructure changes, continuously maintained

---

### `.ai/logs/` - Execution Logs & Reports
**Owner**: Agent responsible for the operation  
**Type**: Operational logs and reports

**Include**:
- Test execution reports
- Build logs (summary)
- Deployment logs (summary)
- Performance test results
- Security scan results
- Release notes

**Example Structure**:
```
logs/
├── test-reports/
│   ├── test-run-2025-01-15.md (@QA)
│   └── coverage-report-2025-01-15.md (@QA)
├── deployment-logs/
│   ├── production-deployment-2025-01-10.md (@DevOps)
│   └── staging-deployment-2025-01-12.md (@DevOps)
└── security-scans/
    ├── sonarqube-scan-2025-01-15.md (@Security)
    └── dependency-audit-2025-01-15.md (@Security)
```

**Update Frequency**: After each operation (test run, deployment, scan)

---

### `.ai/collaboration/` - Coordination Framework
**Owner**: @SARAH  
**Type**: Cross-team coordination

**Include**:
- Agent coordination records
- Decision logs
- Escalation tracking
- Team communication summaries
- Workflow documentation
- Process improvements

**Example Structure**:
```
collaboration/
├── AGENT_COORDINATION.md (@SARAH)
├── decision-log.md (@SARAH)
├── escalations-2025-01.md (@SARAH)
└── process-improvements.md (@SARAH)
```

**Update Frequency**: Continuously as coordination happens

---

### `.ai/issues/{issue-id}/` - Issue-Specific Work
**Owner**: Issue owner + contributors  
**Type**: Collaboration on specific GitHub issues

**Include**:
- Analysis and investigation notes
- Design discussions
- Implementation notes
- Test cases
- Blockers and solutions
- Learnings

**Example Structure**:
```
issues/
├── GITHUB-123-feature-name/
│   ├── analysis.md (issue owner)
│   ├── design-discussion.md (all contributors)
│   ├── implementation-notes.md (@Backend/@Frontend)
│   ├── test-plan.md (@QA)
│   └── learnings.md (final retrospective)
```

**Update Frequency**: Throughout issue lifecycle, finalized when issue closes

---

## How Agents Manage Artifacts

### Creating Artifacts

```markdown
When you start work on something in your domain:

1. Create/update artifact in your `.ai/` folder
2. Use clear, descriptive filenames
3. Include metadata (owner, date, status)
4. Cross-reference related artifacts
5. Make it discoverable (table of contents, links)
```

**Example**:
```bash
# @ProductOwner starting feature work
.ai/requirements/FEATURE-001-shipping/
├── specification.md          (Who: @ProductOwner, When: 2025-01-06)
├── analysis.md              (Team input)
└── handover.md             (Final documentation)
```

### Organizing Artifacts

**Guidelines**:
- Group by feature/theme when possible
- Use consistent naming (descriptive + ID)
- Create README files for navigation
- Link to related artifacts
- Keep structure shallow (2-3 levels max)

**Good Structure**:
```
requirements/
├── FEATURE-001-shipping/
│   ├── README.md (navigation)
│   ├── specification.md
│   └── analysis.md
└── FEATURE-002-payment/
    └── ...
```

### Updating Artifacts

**Live Document Principle**:
- Not created once and forgotten
- Updated as project evolves
- Reflect current state
- Add learnings during implementation
- Clean up at sprint end

**Update Schedule**:
```
Sprint Planning → Create artifact
During Sprint → Update continuously
Sprint Review → Finalize, add retrospective
Next Sprint → Archive or consolidate
```

### Collaborating on Artifacts

**When multiple agents manage same artifact**:
1. Agree on owner (primary agent)
2. Other agents add sections to same file
3. Use clear headers/sections
4. Cross-reference in collaboration folder

**Example** (`.ai/compliance/gdpr-checklist.md`):
```markdown
# GDPR Compliance Checklist

## Security Requirements (@Security)
- [ ] Encryption at rest
- [ ] Encryption in transit
...

## Legal Requirements (@Legal)
- [ ] Privacy notice
- [ ] Lawful basis
...
```

---

## Agent Responsibilities Checklist

### At Sprint Start
- [ ] Create/update artifact for your domain work
- [ ] Link to related artifacts
- [ ] Define structure and ownership
- [ ] Add to team's awareness (share links)

### During Sprint
- [ ] Update artifacts with progress
- [ ] Add learnings and decisions
- [ ] Link to related issues/PRs
- [ ] Collaborate with other agents on shared artifacts

### At Sprint End
- [ ] Finalize artifacts
- [ ] Add retrospective notes
- [ ] Archive if complete
- [ ] Consolidate lessons learned
- [ ] Update knowledgebase with patterns discovered

### Continuous
- [ ] Keep artifacts current (no stale docs)
- [ ] Remove outdated information
- [ ] Cross-link related artifacts
- [ ] Make artifacts discoverable

---

## Benefits of Agent-Driven Artifact Management

✅ **Domain Experts Own Knowledge** - Agents maintain artifacts in their area  
✅ **Living Documentation** - Artifacts evolve with project, not static  
✅ **Better Knowledge Transfer** - Implicit knowledge captured explicitly  
✅ **Accountability** - Clear owner for each artifact  
✅ **Organizational Learning** - Patterns discovered, documented, shared  
✅ **Decision Traceability** - Why decisions made, documented in ADRs  
✅ **Onboarding** - New team members find artifacts in `.ai/`  
✅ **Scalability** - Knowledge scales with team as artifacts grow

---

## Examples in Action

### Example 1: Feature Development
```
Timeline: Sprint 12 (Jan 6-17)

Week 1:
- @ProductOwner creates `.ai/requirements/FEATURE-001-shipping/specification.md`
- @Architect creates `.ai/decisions/ADR-001-shipping-design.md`
- @Backend adds `.ai/decisions/backend-shipping-architecture.md`
- @Frontend adds `.ai/decisions/frontend-shipping-ui.md`
- @ScrumMaster tracks in `.ai/sprint/SPRINT-12/plan.md`

Week 2:
- Agents update with daily progress in sprint file
- Implementation notes added to decision files
- Tests documented in backend/frontend files
- @ProductOwner adds to `.ai/handovers/shipping-feature.md`

Sprint End:
- All artifacts finalized
- Learnings added
- Retrospective notes added to sprint file
- Archive or link from next sprint
```

### Example 2: Security Audit
```
Timeline: Mid-sprint security review

Day 1:
- @Security creates `.ai/compliance/security-audit-jan-2025.md`
- Audit findings documented
- Vulnerabilities listed with severity

Days 2-4:
- @Backend fixes backend issues, documents in `.ai/decisions/`
- @Frontend fixes frontend issues, documents in `.ai/decisions/`
- @Security adds remediation tracking

Day 5:
- @Security updates compliance status
- Creates `.ai/logs/security-scan-results-2025-01-15.md`
- Marks audit complete

Next Audit:
- Reference previous audit for context
- Track improvement over time
```

---

## Support & Questions

**I don't know what to document?**
→ If it's in your domain and others should know it, document it

**My artifact is growing large?**
→ Split into multiple files with clear table of contents

**Should I ask permission?**
→ No—create artifacts in your assigned folder freely

**How do I know what other agents created?**
→ Check `.ai/` folder structure, or ask in collaboration channel

---

## Summary

**Each agent is responsible for managing artifacts in their domain.**

This is not:
- ❌ Optional documentation
- ❌ One-time writing
- ❌ Someone else's job

This is:
- ✅ Core agent responsibility
- ✅ Living documentation you own
- ✅ Knowledge capture and sharing
- ✅ How the project learns and grows

**Start creating and managing `.ai/` artifacts today!**

---

**Last Updated**: 30. Dezember 2025  
**Status**: ✅ Active delegation model in place
