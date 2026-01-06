---
applyTo: ".github/**,Dockerfile,docker-compose*,*.yml,*.yaml,**/infra/**,**/terraform/**"
--

# DevOps Instructions (Proposed Update)

Additions:

1) CI Quality Gates
   - Define a canonical gate sequence for PRs: build → analyzers/format verification → unit tests → smoke e2e → security scans.
   - Fail fast: fail PR early on build/analyzer failures to reduce wasted cycles.

2) Nightly audits & KB sync
   - Schedule nightly/weekly jobs that aggregate CI failures, SCA alerts, and flaky-test metrics and create remediation issues.
   - Include the weekly KB sync job that produces artifacts (see `.github/workflows/kb-sync.yml`).

3) Dashboards & retention
   - Publish gate pass/fail rates to a lightweight dashboard (GitHub Checks + README summary or SonarCloud).
   - Retain CI artifacts for a minimum timeframe to support Control evidence (e.g., 30 days).

4) Secrets & policies
   - Enforce secret scanning in CI and block merges on detected secrets; use a central secret store for runtime values.

Note for CopilotExpert: Merge these changes into `.github/instructions/devops.instructions.md` ensuring CI job names and schedules match existing pipelines.
