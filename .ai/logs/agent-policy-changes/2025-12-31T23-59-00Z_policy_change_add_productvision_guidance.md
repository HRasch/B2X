---
docid: LOG-002
title: 2025 12 31T23 59 00Z_policy_change_add_productvision_guidance
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
timestamp: 2025-12-31T23:59:00Z
issuer: GitHub Copilot
approver: PENDING (@SARAH)
targeted_agents:
  - @Architect
  - @TechLead
  - @DocMaintainer
summary: >-
  Added Product Vision alignment guidance requiring `@Architect` and `@TechLead` to
  keep the ProductVision central, verify assumptions against authoritative
  sources, and present proposals as numbered multiple-choice options. Also
  clarified that agent policy changes require explicit `@SARAH` approval and
  must be logged under `.ai/logs/agent-policy-changes/` with required metadata.
pr: TODO (create PR linking these edits and request @SARAH review)
files_changed:
  - .github/copilot-instructions.md
  - .github/agents/Architect.agent.md
  - .github/agents/TechLead.agent.md

notes: |
  - Change applied as documentation edits by GitHub Copilot prior to SARAH
    approval. This entry records the change and requests SARAH review/approval.
  - Once `@SARAH` reviews and approves, update `approver` with `@SARAH` and
    fill `pr` with the PR URL or merge reference.
---

Status: pending-approval

Next steps:
- Create a PR including the three modified files and request `@SARAH` approval.
- After `@SARAH` approval, update this log entry with `approver` and `pr`.
