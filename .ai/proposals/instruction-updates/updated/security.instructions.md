---
docid: PROP-011
title: Security.Instructions
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
applyTo: "**/*"
--

# Security Instructions (Proposed Update)

Suggested additions:

1) Automated SCA & triage
   - Require Dependabot + scheduled CodeQL and Snyk scans (weekly) with SLAs for triage and remediation.
   - Critical/high findings must be converted into blocking issues and assigned.

2) Secret scanning
   - Run pre-commit secret checks locally and CI secret scans; block merges on detected secrets.

3) Control loop
   - CI should publish SCA results to PR checks and to the weekly audit job; unresolved findings older than X days should auto-create escalation issues.

4) Documentation & runbooks
   - Add runbooks for common security remediations and a contact list for incident escalation.

Note for CopilotExpert: These changes should be merged into `.github/instructions/security.instructions.md` and aligned with existing security guidance.
