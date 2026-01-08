---
docid: PROP-002
title: Devops Instructions Proposal
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

Proposed additions to `.github/instructions/devops.instructions.md` (for CopilotExpert review):

1) CI gates and ordering:
   - Define mandatory gates: build, analyzers, unit tests, smoke-e2e, SCA.
   - Fail fast ordering: build -> analyzers -> unit tests -> smoke-e2e -> SCA.

2) Nightly audits:
   - Add a scheduled job that aggregates CI failures, SCA alerts, and flaky-test metrics and opens remediation issues.

3) Dashboards & retention:
   - Publish gate pass/fail rates to a simple dashboard and retain artifact logs for X days.
