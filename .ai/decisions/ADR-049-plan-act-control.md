---
docid: ADR-093
title: ADR 049 Plan Act Control
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: ADR-049
title: Plan-Act-Control Engineering Loop
owner: @SARAH
status: Proposed
---

## Context

We need a lightweight, auditable engineering loop that ties planning, implementation, and automated control checks together to reduce regressions and speed remediation.

## Decision

Adopt the "Plan‑Act‑Control" principle for feature and change delivery across the repository:

- Plan: require a short plan (ADR or issue) with acceptance criteria and test plan before work starts.
- Act: implement changes with automated checks (linters, analyzers, unit/integration tests, e2e, SCA) run in CI per-PR.
- Control: collect CI evidence, metrics, and scan results; convert failures or drift into tracked remediation work and feed into the next Plan cycle.

## Consequences

- Increased discipline on PRs (plan + acceptance criteria). 
- Early detection of regressions via automated gates. 
- Need to maintain dashboards and lightweight audit jobs to keep Control actionable.

## Implementation

1. Add PR template requiring a `Plan` link and `Acceptance criteria` (see repo template).
2. Add an ADR record (this file) and KB entry describing expected CI gates and minimal evidence.
3. CI: enforce build → analyzers → unit tests → smoke e2e → security scan on every PR.
4. Create a weekly audit job that converts outstanding Control failures into issues and labels for triage.
5. Assign owners: Plan = feature author, Act = implementing dev, Control = QA/Security/TechLead.

## Monitoring & Metrics

- Track: PRs with missing plan links, CI gate pass rates, flaky-test counts, SCA findings, and time-to-remediate.
- Surface these in PR checks and a lightweight dashboard (GitHub Checks + README summary or SonarCloud).

## Rollout

- Pilot on a single bounded context or repo area for 2 sprints, tune gate thresholds, then roll out across repositories.
