---
applyTo: "**/*.test.*,**/*.spec.*,**/tests/**,**/__tests__/**"
--

# Testing Instructions (Proposed Update)

Enhancements:

1) Contract testing
   - Require consumer-driven contract tests for gateway APIs; run in CI on integration lanes.
   - Store contract artifacts/versioning in `.ai/contracts/`.

2) Visual regression policy
   - Define baseline management: PRs should run a focused visual smoke test; full visual suite runs nightly.
   - Baseline updates must reference a review comment and acceptance criteria.

3) Flaky tests
   - Add a triage process: mark flaky tests, quarantine them, and create remediation tasks with owners.
   - Track flaky-test counts and surface in weekly test-health reports.

4) Metrics & Control
   - Track test pass rates, coverage, and time-to-fix; publish weekly to a testing dashboard or PR checks summary.

5) Local tooling
   - Document `scripts/run-local-checks.sh` usage and add troubleshooting tips for common test failures.

Note for CopilotExpert: Apply these as additions to `.github/instructions/testing.instructions.md` keeping existing E2E and visual guidance.
