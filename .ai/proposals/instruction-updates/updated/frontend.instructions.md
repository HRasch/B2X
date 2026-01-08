---
docid: PROP-010
title: Frontend.Instructions
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
applyTo: "src/components/**,src/pages/**,src/hooks/**,src/ui/**,**/frontend/**"
--

# Frontend Instructions (Proposed Update)

Key additions:

1) TypeScript & Linting
   - Enforce `compilerOptions.strict = true` across frontend projects; document migration steps.
   - Use ESLint with TypeScript rules; run `eslint --max-warnings=0` in CI.
   - Maintain a shared ESLint config in `frontend/.eslintrc.js`.

2) Formatting & Hooks
   - Add Prettier config and Husky pre-commit hooks to run `npm run lint --fix` and `npm test`.
   - Provide `npm run format` commands and require `pre-commit` to run formatters locally.

3) Visual regression policy
   - Define when Playwright visual tests run: quick smoke in PRs (targeted), full visual suite nightly.
   - Document baseline update procedure and acceptable thresholds; require an explanation in PRs when baselines change.

4) Control evidence
   - Publish lint and visual test outputs to PR checks; include instructions for local quick-checks (`scripts/run-local-checks.sh`).

5) Accessibility & Performance
   - Include quick accessibility checks in CI for critical pages; document Lighthouse/perf thresholds for release.

Note for CopilotExpert: Merge these additions into canonical `.github/instructions/frontend.instructions.md` and keep existing i18n guidance intact.
