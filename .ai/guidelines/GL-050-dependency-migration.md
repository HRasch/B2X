---
docid: GL-050
title: Dependency Migration Guideline
owner: @TechLead
status: Active
---

**Purpose**
- **Goal:** Provide a safe, repeatable process for upgrading dependencies across backend and frontend projects while minimising regressions and operational risk.

**Scope**
- Applies to npm, NuGet, and build/test tooling upgrades across this repository. Excludes runtime configuration changes unrelated to package upgrades.

**Principles**
- **Safety-first:** Prefer CI automation for minor/patch upgrades; require human review for majors.
- **Small steps:** Upgrade one dependency or a small group per canary PR.
- **Observability:** Each upgrade must include automated tests and monitoring checks.
- **Documentation:** Record observed breakages and fixes in the KB for future runs.

**Process (high-level)**
1. **Inventory & Baseline:** Commit current lockfiles; run full test suite and record results.
2. **Categorize:** Mark packages as `runtime-critical`, `ui`, `dev/test`, or `optional`.
3. **Automate minor/patch upgrades:** Use Dependabot or nightly `ncu` to open canary PRs into `deps/upgrade-minor/*`.
4. **Canary checks:** For each PR run:
   - Frontend: `npm ci && npm run build && npm test && npx playwright test --reporter=list` and visual-regression snapshots.
   - Backend: `dotnet restore && dotnet build && dotnet test` and analyzers.
   - Linters and vulnerability scans.
5. **Staging promotion:** If checks pass, merge to `staging` and deploy a canary instance; monitor 24–72h.
6. **Major upgrades:** Create a design ADR, include migration notes, and require TechLead + Architect approval; run extended integration tests and staged rollout.
7. **Emergency security patches:** Auto-apply, run full CI; auto-merge on green with post-merge monitoring.
8. **Rollback:** Maintain automated revert scripts; preserve previous lockfiles and deploy previous build on failure.

**Frontend-specific mitigations**
- Externalize large translation objects to `locales/*.json` to avoid TypeScript inference blowups. Keep `i18n` config minimal and typed.
- Use `@tailwind` directives in CSS and configure `postcss.config.js` with `tailwindcss` + `autoprefixer`.
- Consolidate shared types (e.g., `CartItem`) under `frontend/Store/types` and export a single canonical type.
- Add visual-regression tests (Playwright) for UI-critical components.

**Backend-specific mitigations**
- Use Central Package Management (`Directory.Packages.props`) — do not set per-project `Version` attributes (avoids NU1008).
- Run analyzers pre- and post-upgrade; fix warnings before promoting major runtime upgrades.
- Use Testcontainers or integration test DBs when upgrading DB drivers or ORMs.

**CI & Automation (examples)**
- Canary job (frontend):

```bash
npm ci
npm run build
npm test
npx playwright test --project=chromium --reporter=html
# run visual-regression comparator
```

- Canary job (backend):

```bash
dotnet restore
dotnet build --no-incremental
dotnet test --logger:trx
# run analyzers and smoke tests
```

**Quality Gate checklist (PR required checks)**
- Unit tests: pass
- Integration/e2e tests: pass
- Visual regression: no unexpected diffs
- Vulnerability scan: no new critical/high issues
- Performance smoke tests: no regressions
- Migration notes / KB entry included for majors
- Approvals: required reviewers signed off

**Roles & Ownership**
- **Frontend owner:** Responsible for npm upgrades, visual-tests, and UI mitigations.
- **Backend owner:** Responsible for NuGet upgrades, analyzers, and integration tests.
- **Security:** Triage vulnerabilities and approve emergency patches.
- **QA:** Maintain and run visual/e2e test suites.
- **Coordinator (@SARAH):** Approve cross-team rollouts and schedule maintenance windows.

**KB & Changelog**
- For each upgrade run, add one-line summary to `CHANGELOG.md` and a short KB note under `.ai/knowledgebase/tools-and-tech/` describing observed breakages and fixes.

**Rollback & Incident Playbook**
- If canary shows failures: revert the specific canary PR, redeploy prior build, open an incident issue with logs and revert PR link.
- For production issues: run the emergency revert script and page the on-call owners.

**References**
- See `.github/instructions/*` and `.ai/guidelines/` for related policies.

**Status:** Draft — request TechLead review and publish after approval.
