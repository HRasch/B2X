---
docid: PROP-006
title: Security Instructions Proposal
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

Proposed additions to `.github/instructions/security.instructions.md` (for CopilotExpert review):

1) Automated SCA & alerting:
   - Require Dependabot + periodic Snyk/CodeQL scans and define SLAs for triage.

2) Secret scanning enforcement:
   - Run pre-commit secret checks and CI secret scans. Block merges on detected secrets.

3) Control loop:
   - Convert critical SCA/scan failures into tracked issues and include remediation deadlines.
