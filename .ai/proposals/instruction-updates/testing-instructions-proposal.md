---
docid: PROP-007
title: Testing Instructions Proposal
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

Proposed additions to `.github/instructions/testing.instructions.md` (for CopilotExpert review):

1) Contract testing:
   - Add guidance to create consumer-driven contract tests for gateway APIs; run these in CI on integration lanes.

2) Visual regression policy:
   - Define baseline management, thresholds, and when visual tests run (PR vs nightly).

3) Flaky tests:
   - Add a documented process for flaky-test triage: mark, retest, quarantine, and fix.

4) Control metrics:
   - Track flaky-test counts, test pass rates, and publish weekly test-health report.
