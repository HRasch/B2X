Title: Apply proposed instruction updates

Summary:
- This PR-ready draft packages the proposal updates in `.ai/proposals/instruction-updates/updated/` and requests `@CopilotExpert` to apply them into the canonical `.github/instructions/` files.

Files to apply (source -> target):
- `.ai/proposals/instruction-updates/updated/backend.instructions.md` -> `.github/instructions/backend-essentials.instructions.md`
- `.ai/proposals/instruction-updates/updated/frontend.instructions.md` -> `.github/instructions/frontend-essentials.instructions.md`
- `.ai/proposals/instruction-updates/updated/testing.instructions.md` -> `.github/instructions/testing.instructions.md`
- `.ai/proposals/instruction-updates/updated/devops.instructions.md` -> `.github/instructions/devops.instructions.md`
- `.ai/proposals/instruction-updates/updated/security.instructions.md` -> `.github/instructions/security.instructions.md`

Suggested branch name: `chore/instructions-update-2026-01-07`

Suggested commit message:
"chore(instructions): apply proposed instruction updates (backend, frontend, testing, devops, security)"

PR description template (copy into PR body):
```
This PR applies the instruction updates proposed in `.ai/proposals/instruction-updates/`.

Key changes:
- Backend: Plan‑Act‑Control PR requirements, StyleCop/Roslyn, `dotnet format` CI step, i18n validation, CI artifacts
- Frontend: TypeScript `strict`, ESLint/Prettier, Husky hooks, visual-regression policy, accessibility quick-checks
- Testing: Contract testing guidance, visual baseline policy, flaky-test triage, metrics publishing
- DevOps: Canonical CI gate ordering, nightly audits, artifact retention, secret policies
- Security: Dependabot + scheduled Snyk/CodeQL scans, secret scanning enforcement, SLAs for triage

Clarifications requested before merge (please resolve in PR comments):
- Exact CI job names/paths for analyzer/format/contract test jobs
- SLA targets for security triage (time-to-triage / time-to-remediate)
- Rollout plan for enabling TypeScript `strict` (opt-out or migration steps for legacy modules)
- Owners for flaky-test metrics and dashboard location

Approvers: @CopilotExpert, @SARAH, @Security, @TechLead
```

Checklist for `@CopilotExpert` before merging:
- [ ] Verify updated files match company style and front-matter
- [ ] Update PR template if adding mandatory ADR link requirement
- [ ] Add CI job bindings or reference existing job names
- [ ] Add migration/rollout notes where requested above

If you want, I can also create a local branch and commit these changes into the repository (requires permission). Otherwise please apply via your normal PR process.

Status: Waiting on @CopilotExpert to open PR and resolve clarifications.
