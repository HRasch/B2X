---
docid: KB-121
title: KB 051 External Knowledge Sync
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: KB-051
title: External Knowledge Sync Procedure
owner: @DocMaintainer
status: Active
---

# External Knowledge Sync Procedure

Purpose: Ensure our KB and tooling recommendations stay up-to-date with authoritative internet sources (framework releases, SCA advisories, Playwright/.NET changes, major library deprecations).

Summary
- Cadence: Weekly automated checks + monthly manual review + ad-hoc security alerts.
- Owners: `@DocMaintainer` (run and approve KB updates), `@Security` (triage SCA alerts), `@TechLead` (approve toolchain changes), `@SARAH` (final coordination).

Automated checks (weekly)
- Dependabot / Renovate: ensure configured and PRs are created for minor/patch bumps.
- Scripted checks (see `scripts/update-kb-sources.sh`) that gather:
  - `dotnet list package --outdated` (backend)
  - `npm outdated` (frontend folders)
  - optionally fetch latest releases for key tools (Playwright, .NET, Node) via simple `curl` calls (if credentials/policies allow).
- Aggregate output into `artifacts/kb-sync-report-YYYYMMDD.txt` and attach to the weekly KB update issue.

Manual review (monthly)
- Review aggregated report, triage changes that require policy decisions (major semver bumps, breaking changes).
- Create ADRs or KB updates where policy/implementation changes are required.

Emergency / Security
- For critical SCA alerts (high/critical), `@Security` opens a priority issue and notifies owners; run fast-track updates and patch PRs.

KB update process
1. `@DocMaintainer` runs the weekly script and files a weekly KB-sync issue with the report attached.
2. Team triages items; smaller updates create PRs to update KB entries or instruction proposals.
3. Approved changes are merged; the DOC registry is updated accordingly.

Visibility & audit
- Keep reports under `artifacts/` and a short changelog entry in `.ai/logs/` for each sync.

Tools & automation notes
- Use Dependabot/ Renovate for automated dependency PRs.
- Where possible, prefer verified sources (official release notes, GitHub releases, vendor advisories) over third-party summaries.

Security & privacy
- Do not embed API keys or secrets in scripts.
- When querying third-party services, follow rate limits and caching best practices.
