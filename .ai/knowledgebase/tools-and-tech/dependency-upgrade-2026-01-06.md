---
docid: KB-DEP-2026-01-06
title: Dependency Upgrade & Migration Guide (Jan 2026)
owner: @TechLead
status: Draft
---

This KB summarizes the dependency upgrade candidates discovered on 2026-01-06, describes migration how-to steps for the most relevant package families, and lists likely breaking changes and mitigation strategies.

Summary of candidates
- See `.ai/dependency-updates/nuget-and-npm-candidates-2026-01-06.md` for the full scan.
- Notable NuGet candidates: `EFCore.BulkExtensions` (8.x → 9.x), `Microsoft.CodeAnalysis.CSharp` (4.x → 5.x), `Dapper` (patch), `Npgsql` (patch), `Microsoft.Extensions.*` (patches).
- Notable frontend (npm) candidates: `nuxt` (3 → 4), `pinia` (2 → 3), `tailwindcss` (3 → 4), `date-fns` (3 → 4), multiple OpenTelemetry packages.

High-level migration strategy
1. Apply patch and minor updates first across the repo (safe branch). Verify compilation, unit tests, integration tests, and CI.
2. Stage major upgrades on separate branches per ecosystem (NuGet-major, frontend-major). Keep each major upgrade focused to minimize blast radius.
3. For each major upgrade branch: update lockfiles, run full test matrix (unit, integration, e2e, visual regression), fix compile and runtime errors, and iterate until green.
4. Prefer small, reviewable PRs. Include migration notes and a checklist in PR description.

NuGet-specific notes and mitigation
- Patch/minor upgrades (e.g., `Dapper`, `Npgsql` patch releases): usually safe — update and run tests.
- `EFCore.BulkExtensions` 8 → 9 (major):
  - Breaking areas: method signature changes, behavior around transactions and batching. Review bulk insert/update/merge call sites and custom transaction handling.
  - Recommended steps: update in a feature branch, run database integration tests, and validate performance and batching behavior on staging.
- Roslyn / Analyzer upgrades (`Microsoft.CodeAnalysis.*`):
  - Upgrading analyzer/runtime packages can introduce new diagnostics; run build with analyzers enabled and treat new findings as code-review tasks.
  - If analyzer versions require SDK bumps, prefer to update analyzers in a separate PR.
- Testcontainers / coverlet updates: update and re-run tests; container image tags may need adjustment for compatibility.

Frontend (npm) migration notes
- `nuxt` 3 → 4: major framework change. Expected migration tasks:
  - Review Nuxt 4 migration guide (routing, server engine, Nitro changes). Replace or update Nuxt modules that haven't been ported.
  - Update plugins and composables to use new Nitro server runtime conventions and Vite config compat.
  - Run full e2e and visual regression tests; expect iterative fixes to SSR/data-fetching and middleware.
- `tailwindcss` 3 → 4:
  - Check breaking-change notes (config format, JIT defaults, plugin APIs). Run `npx tailwindcss -v` and adjust `tailwind.config.js`.
  - Run visual tests to catch CSS regressions.
- `pinia` 2 → 3:
  - Review store API changes (if any), update types and plugin compatibility.

Checklist for each upgrade PR
- Create dedicated branch: `deps/<scope>/<minor|major>-YYYYMMDD`.
- Update package references and lockfiles (`dotnet restore`, `npm install` / `pnpm install`).
- Run: `dotnet build`, `dotnet test` (all), and frontend `npm run build` + unit + e2e tests.
- Run visual-regression tests where applicable.
- Validate container-based integration tests (Testcontainers). Update container images if necessary.
- Document observed breaking changes and remediation in PR description.

Rollback plan
- If runtime issues appear in staging after upgrade, revert the dependency commit and open follow-up PR with deeper analysis.

References
- `.ai/dependency-updates/nuget-and-npm-candidates-2026-01-06.md` (scan results)
- Official migration guides: Nuxt 4 migration docs, Tailwind 4 release notes, EFCore.BulkExtensions release notes, Microsoft.CodeAnalysis release notes.

If you'd like, I can start by automatically applying patch/minor NuGet updates in a branch `deps/upgrade-minor` and run `dotnet build` + tests. Confirm and I'll proceed.
