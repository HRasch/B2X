---
docid: KB-050
title: Plan-Act-Control Practical Guide
owner: @SARAH
status: Active
---

# Plan-Act-Control Practical Guide

This KB article explains how to apply the Plan‑Act‑Control engineering loop in practice for the B2Connect codebase.

Summary:
- Plan: Create a short plan (ADR or issue) with acceptance criteria and a minimal test plan. Link it from PRs.
- Act: Implement changes with automated checks: analyzers, formatters, unit/integration tests and smoke E2E tests.
- Control: Collect CI evidence and metrics, convert failures into tracked remediation work, and feed learnings back into planning.

Quick checklist (for authors):
- Create ADR or issue that outlines acceptance criteria and test plan.
- Run analyzers and linters locally; fix violations.
- Add unit/integration tests and a smoke E2E test when relevant.
- Link the Plan in the PR and include Control validation steps in the PR template.

Examples & recommended CI gates:
- Build: `dotnet build` across solution
- Analyzers: `dotnet format --verify-no-changes`, StyleCop/Roslyn warnings as errors
- Unit tests: `dotnet test` with coverage for business logic
- Smoke E2E: Playwright visual-sanity test for critical UI paths
- SCA / Security: CodeQL or Snyk run on PR

Metrics to track:
- PRs missing Plan links
- CI gate pass rate
- Flaky-test rate
- Time-to-remediate security findings

Rollout suggestion:
- Pilot in `backend/Domain/Catalog` for two sprints, iterate on thresholds, then extend to full repo.
