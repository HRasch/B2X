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

Tailwind CSS v4 findings (official upgrade guide summary)
- Tailwind provides an official upgrade guide and an automated upgrade tool `npx @tailwindcss/upgrade` which handles most of the migration from v3 to v4 (requires Node.js 20+).
- Major breaking topics to review:
  - `@tailwind` directives removed; import Tailwind via `@import "tailwindcss"`.
  - Many utilities renamed (e.g., `shadow`→`shadow-sm`→`shadow-xs`), default scales changed (shadow, radius, blur).
  - Default `ring` width changed (3px → 1px) and default color changed to `currentColor`.
  - Removed deprecated utilities and changed selector behaviour for `space-*` and `divide-*` (performance-driven changes).
  - `@apply` and custom utilities behavior changed; new `@utility` API introduced.
  - Tailwind v4 targets modern browsers (Safari 16.4+, Chrome 111+, Firefox 128+) — if you must support older browsers, stay on v3.
  - The upgrade guide documents many renames, preflight changes, and configuration differences — use the upgrade tool and run visual tests.

Reference: Tailwind upgrade guide — https://tailwindcss.com/docs/upgrade-guide

EFCore.BulkExtensions releases
- The project's releases are on GitHub; recent published artifacts show v8.x releases (example: v8.1.2 published Dec 2024). When upgrading EFCore.BulkExtensions, review release notes and changes to bulk APIs and transaction behavior.

Reference: EFCore.BulkExtensions releases — https://github.com/borisdj/EFCore.BulkExtensions/releases

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

Observed breakages (frontend `Store` after `npm update`)

- Symptoms seen while building `/frontend/Store` after minor/patch updates:
  - TypeScript errors reporting values of type `unknown` (e.g. `error.message` access) and `TS18046` / `TS18047` style diagnostics.
  - Object-literal assignment errors where calls to `cartStore.addItem({...})` are missing newly-required fields (e.g. `category`, `rating`, `b2bPrice`, `description`, `inStock`).
  - Module/type export mismatches (e.g. "declares 'CartItem' locally, but it is not exported").
  - `i18n` plugin symbols reported as `unknown` in client plugin code.

- Root-cause analysis (internet sources)
  - TypeScript stricter defaults and new compiler behaviors: TypeScript has progressively tightened defaults and added options that change behavior (notably `useUnknownInCatchVariables` which makes `catch` clause variables `unknown` by default under `strict`), stricter excess-property checks, and new module/emit options (`--verbatimModuleSyntax`, `--moduleResolution bundler`) that affect resolution and type/value distinctions. See TypeScript docs: https://www.typescriptlang.org/tsconfig/#useUnknownInCatchVariables and release notes: https://www.typescriptlang.org/docs/handbook/release-notes/typescript-5-0.html
  - Library/type upgrades: when dependency packages (Nuxt, Pinia, vue-i18n, or shared `types` packages) change exported type shapes (add fields to `CartItem`, switch to `export type` vs value exports, or change plugin signatures) existing call sites will fail type-checking until code is adjusted to new shapes.
  - Framework typing changes: Nuxt/Turbo/Vite/Pinia upgrades commonly bump TypeScript expectations (TS config, lib targets) and change runtime plugin shapes; e.g., `vue-i18n` v11 introduces new APIs and typing surfaces — plugin usage must match the installed `vue-i18n` version.

- Direct links for verification and migration references
  - TypeScript: `useUnknownInCatchVariables` / TS 5.x release notes — https://www.typescriptlang.org/tsconfig/#useUnknownInCatchVariables and https://www.typescriptlang.org/docs/handbook/release-notes/typescript-5-0.html
  - Pinia docs (typing, API): https://pinia.vuejs.org/
  - Vite guide: https://vite.dev/guide/
  - Nuxt upgrade information: https://nuxt.com/docs/getting-started/upgrade-guide
  - vue-i18n migration guide: https://vue-i18n.intlify.dev/guide/migration/migration-v11.html

- Recommended mitigation steps (practical quick fixes)
  1. Narrow `catch` variables or set `useUnknownInCatchVariables` / update `tsconfig.json` to match project policy:
     - Preferred: tighten handler code: `catch (err: unknown) { if (err instanceof Error) console.log(err.message) }`.
     - Short term: toggle `useUnknownInCatchVariables` to `false` if unavoidable (document the exception).
  2. Fix object-literal calls by either: supply missing fields, make those fields optional in `CartItem` (if semantically optional), or update the shared `types` package to preserve backwards-compatible optional fields.
  3. For `CartItem` export/type errors: ensure `stores/cart.ts` and `~/types` export the symbol consistently (use `export type CartItem = ...` and import with `import type { CartItem } from '...'` where appropriate), or export a runtime value when needed.
  4. For `i18n`/plugin `unknown` symbols: update `vue-i18n` usage to match installed version (or pin `vue-i18n` to the previous working version until migration PR completes). Use the official migration guide for breaking API changes.
  5. If multiple front-end libs upgraded together (nuxt/pinia/tailwind), consider rolling back to previous lockfile and apply upgrades one-by-one (upgrade order: build tooling (vite) → framework (nuxt) → state (pinia) → UI (tailwind)).

- KB Action Items
  - Add small code examples to repo showing safe `catch (err: unknown)` handling and `CartItem` compatibility wrappers.
  - Create a dedicated `frontend/deps/upgrade-minor` branch that pins currently-working versions in `package.json`/lockfile, then apply upgrades one-by-one with quick fixes and visual testing.


If you'd like, I can start by automatically applying patch/minor NuGet updates in a branch `deps/upgrade-minor` and run `dotnet build` + tests. Confirm and I'll proceed.
