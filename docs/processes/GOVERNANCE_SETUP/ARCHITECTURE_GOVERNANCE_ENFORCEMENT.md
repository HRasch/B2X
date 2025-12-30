# 🔒 Architecture Governance Enforcement

**Effective**: 29. Dezember 2025  
**Enforcer**: @process-assistant  
**Authority**: @software-architect  
**Status**: ✅ ACTIVE

---

## 📋 Enforcement Rules

### Rule 1: Who Controls Estimations?

**ONLY @software-architect** can modify:
- `docs/architecture/ESTIMATIONS_AND_CAPACITY.md`
- Timeline projections
- Cost estimates
- Team capacity requirements
- Scaling plans

**When**: During issue review when development starts  
**NOT allowed**: Mid-sprint changes, changes without issue review  
**Process**: Issue → Product Owner → @software-architect validation → Development start

### Rule 2: Who Controls Design Decisions?

**ONLY @software-architect** can:
- Create/modify Architecture Decision Records (ADRs)
- Update `docs/architecture/DESIGN_DECISIONS.md`
- Approve architectural changes

**When**: BEFORE implementation starts  
**Process**: Issue identified → @software-architect creates ADR → @tech-lead reviews → Implementation approved  
**NOT allowed**: Architectural changes without documented ADR, changes after code written

### Rule 3: Who Controls Scope?

**ONLY @software-architect** can modify:
- `docs/architecture/SOFTWARE_DEFINITION.md`
- Feature scope (IN/OUT lists)
- System constraints
- Non-functional requirements

**When**: During quarterly reviews or when scope shift identified  
**Process**: Change identified → @product-owner confirms → @software-architect documents → All stakeholders notified  
**NOT allowed**: Scope creep without documentation

### Rule 4: What @process-assistant Enforces?

**Daily Checks**:
- ✅ Commit logs reference architecture docs when relevant
- ✅ ADRs are linked in PRs affecting architecture
- ✅ Only @software-architect commits to architecture docs

**Weekly Monitoring**:
- ✅ Architecture docs are current (no stale ADRs)
- ✅ Design decisions are being followed in code
- ✅ No unauthorized changes to estimations

**Monthly Actions**:
- ✅ Review new ADRs for completeness
- ✅ Verify @software-architect is maintaining docs
- ✅ Check for scope violations

**Quarterly Reviews**:
- ✅ Full SOFTWARE_DEFINITION.md review
- ✅ Estimations accuracy check against actuals
- ✅ Design decisions effectiveness assessment
- ✅ Update documents with learnings

---

## 🚨 Violation Handling

### Level 1: Unauthorized Estimation Change

**Detection**: Git diff shows change to `ESTIMATIONS_AND_CAPACITY.md` by non-@software-architect user  
**Action**: 
1. Revert change immediately
2. Notify @software-architect (comment on commit)
3. Log violation

**Message to Agent**:
> "Estimations cannot be modified mid-sprint or by @[user]. Changes must occur during issue review when development starts. Please wait for next issue review, or contact @software-architect to request an out-of-cycle change with justification."

### Level 2: Unauthorized Design Decision Change

**Detection**: Git diff shows change to `DESIGN_DECISIONS.md` by non-@software-architect user  
**Action**:
1. Revert change immediately
2. Notify @software-architect
3. Require ADR for the proposal

**Message to Agent**:
> "Design decisions cannot be changed without an ADR. Please create an ADR documenting your proposal, have @tech-lead review it, and resubmit with approval."

### Level 3: Scope Creep

**Detection**: Features in code not listed in `SOFTWARE_DEFINITION.md` IN section  
**Action**:
1. Flag in code review
2. Require SOFTWARE_DEFINITION.md update before merge
3. Document decision rationale

**Message to Developer**:
> "This feature is not in the documented scope (SOFTWARE_DEFINITION.md). Please either: 1) Add to scope with @software-architect approval, or 2) Remove the feature. Cannot merge undocumented scope changes."

### Level 4: Repeated Violations

**Action**:
1. Escalate to @tech-lead
2. Document pattern
3. Require training on governance

---

## ✅ Change Request Process

### For Estimations (e.g., Timeline, Cost, Capacity)

```
Step 1: During Issue Review
  │
  ├─ @product-owner creates issue
  ├─ Describes what changed and why
  └─ Requests @software-architect validation

Step 2: @software-architect Validates
  │
  ├─ Assess impact on timelines/costs
  ├─ Update ESTIMATIONS_AND_CAPACITY.md
  ├─ Document rationale in commit message
  └─ Commit to architecture branch

Step 3: Development Starts
  │
  └─ Estimations are locked (no changes mid-sprint)

Step 4: Post-Sprint
  │
  └─ Actual vs. estimated review in retrospective
```

### For Design Decisions (New ADR)

```
Step 1: Architecture Question Identified
  │
  └─ Create GitHub issue describing problem

Step 2: @software-architect Creates ADR
  │
  ├─ Writes ADR (problem, options, decision, rationale, consequences)
  ├─ Links to issue
  └─ Requests @tech-lead review

Step 3: @tech-lead Reviews
  │
  ├─ Validates decision is sound
  ├─ Checks consequences are understood
  └─ Approves (or requests revisions)

Step 4: Development Proceeds
  │
  └─ Implementation follows ADR
```

### For Scope Changes

```
Step 1: Quarterly Review
  │
  ├─ @software-architect reviews SOFTWARE_DEFINITION.md
  ├─ @product-owner provides feedback
  └─ Compare actual vs. planned scope

Step 2: Changes Documented
  │
  ├─ Add/remove features from IN/OUT lists
  ├─ Update constraints if applicable
  └─ Commit to architecture branch

Step 3: Stakeholder Notification
  │
  └─ Announce scope changes in team meeting
```

---

## 📊 Governance Metrics (Track Monthly)

| Metric | Target | Owner |
|--------|--------|-------|
| Unauthorized architectural changes | 0 | @process-assistant |
| ADRs before implementation | 100% | @software-architect |
| Estimations accuracy (vs actual) | ±10% | @software-architect |
| Documentation currency | 100% current | @software-architect |
| Scope conformance | <5% creep | @product-owner |
| Process adherence | 95%+ | @process-assistant |

---

## 🎯 @Software-Architect Responsibilities

### Daily
- [ ] Monitor GitHub for PRs affecting architecture
- [ ] Review new ADRs in progress
- [ ] Answer architecture questions

### Weekly
- [ ] Update estimations if scope changes are approved
- [ ] Finalize ADRs ready for implementation
- [ ] Report any scope violations to @product-owner

### Monthly
- [ ] Review accuracy of previous month's estimations
- [ ] Assess if design decisions are being followed
- [ ] Plan next quarter's architecture work

### Quarterly
- [ ] Full SOFTWARE_DEFINITION.md review
- [ ] Update ESTIMATIONS_AND_CAPACITY.md with learnings
- [ ] Report on architecture health
- [ ] Plan next quarter's architecture priorities

---

## 🔗 Related Documents

- [ARCHITECTURAL_DOCUMENTATION_STANDARDS.md](./docs/architecture/ARCHITECTURAL_DOCUMENTATION_STANDARDS.md) - Overall standards
- [SOFTWARE_DEFINITION.md](./docs/architecture/SOFTWARE_DEFINITION.md) - What B2Connect is
- [DESIGN_DECISIONS.md](./docs/architecture/DESIGN_DECISIONS.md) - Why architectural choices
- [ESTIMATIONS_AND_CAPACITY.md](./docs/architecture/ESTIMATIONS_AND_CAPACITY.md) - Projections & costs
- [GOVERNANCE_RULES.md](./.github/docs/processes/GOVERNANCE/GOVERNANCE_RULES.md) - Overall governance
- [copilot-instructions.md](./.github/copilot-instructions.md) - Agent instructions

---

**Status**: ✅ Enforcement Active  
**Enforced By**: @process-assistant  
**Authority**: @software-architect  
**Last Updated**: 29. Dezember 2025
