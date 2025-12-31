Agent Policy Changes Log
=======================

Purpose:
- This folder stores approved agent policy change logs. Each approved policy change must have a dedicated markdown file containing the required fields described in the project's instructions.

File naming:
- Use an ISO-8601 UTC timestamp and short description, e.g.: `2025-12-31T15-30-00Z_policy_change_docmaintainer-model.md`

Required fields (YAML or simple key-value):
- `timestamp`: ISO-8601 UTC timestamp
- `issuer`: GitHub handle or agent name requesting the change
- `approver`: `@SARAH`
- `targeted_agents`: list of agent DocIDs or names
- `summary`: short description of the change
- `pr`: link to the PR or issue that contains the change

Process:
1. Propose the change in a PR. The PR author should include the proposed log entry as a draft in this folder.
2. `@SARAH` reviews and approves the PR. After approval/merge, `DocMaintainer` verifies the log entry exists.
3. The log file remains as a permanent audit trail.

Note: Only `@SARAH` may approve and merge policy changes. `DocMaintainer` may prepare PRs and verify logs, but must not unilaterally change agent policies.
