---
applyTo: "src/api/**,src/services/**,src/models/**,src/repositories/**,**/backend/**"
--

# Backend Instructions (Proposed Update)

Summary of additions to align with Plan‑Act‑Control and best practices:

1) Plan‑Act‑Control mapping
   - Require link to an ADR or issue with acceptance criteria and test plan in every PR that changes behavior; PR template to be updated accordingly.
   - Owners: Plan = feature author, Act = implementer, Control = QA/Security/TechLead.

2) Static analysis & formatting
   - Enable and configure StyleCop Analyzers and relevant Roslyn analyzers.
   - Treat new analyzer warnings as errors in CI; legacy warnings can be triaged via TODO tickets.
   - Add `dotnet format --verify-no-changes` as a CI step and document local fix commands.

3) Testing expectations
   - Unit tests for business logic; aim >80% coverage for core domain code.
   - Integration tests for database/repository interactions; use in-memory providers only for unit tests.
   - Add consumer-driven contract tests for gateway APIs where applicable.

4) Localization
   - Return translation keys in public-facing responses; use `IStringLocalizer<T>`.
   - Add a CI validation step that checks for missing keys across supported locales.

5) CI Evidence & Control
   - CI must publish analyzer reports, coverage, and test results as artifacts.
   - Fail PRs on regressions in gates (build, analyzers, unit tests). Non-blocking gates (visuals) can be warnings depending on pilot settings.

6) Developer guidance
   - Document `run-local-checks.sh` usage and common troubleshooting steps in `CONTRIBUTING.md`.

Note for CopilotExpert: Apply these additions carefully to maintain file style and header front-matter.
