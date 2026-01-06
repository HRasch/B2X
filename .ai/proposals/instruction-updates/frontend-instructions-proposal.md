Proposed additions to `.github/instructions/frontend.instructions.md` (for CopilotExpert review):

1) TypeScript & linters:
   - Require `compilerOptions.strict = true` in frontend TS configs.
   - Enforce ESLint with project/TypeScript rules; run `eslint --max-warnings=0` in CI.

2) Formatting & hooks:
   - Add Prettier configuration and Husky pre-commit hooks to run `npm run lint --fix` and `npm test`.

3) Visual regression:
   - Define policy for Playwright visual tests: baseline update process, tolerances, and when PRs must run smoke visual tests.

4) Control evidence:
   - Publish lint and visual test results to PR checks and include guidance for local quick-checks.
