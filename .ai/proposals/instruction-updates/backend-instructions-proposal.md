Proposed additions to `.github/instructions/backend.instructions.md` (for CopilotExpert review):

1) Add Plan-Act-Control section mapping:
   - Require ADR/Plan link in PRs for substantive changes.
   - Define owners: Plan = feature author, Act = implementer, Control = QA/Security/TechLead.

2) Static analysis & format:
   - Enable StyleCop and Roslyn analyzers as warnings-as-errors in CI for new code.
   - Add `dotnet format --verify-no-changes` step to CI.

3) Testing expectations:
   - Unit tests for business logic with target >80% coverage.
   - Integration tests for repository/db interactions.

4) Localization & messages:
   - Return translation keys, and include a CI validation step that checks for missing i18n keys.

5) Control outputs:
   - CI must publish analyzer and coverage reports and fail PRs on regressions.
