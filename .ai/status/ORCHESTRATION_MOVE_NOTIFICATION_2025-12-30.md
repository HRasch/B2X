---
title: Orchestration moved to AppHost
date: 2025-12-30
tags: [orchestration, docs, announcement]
---

Summary
-------

The orchestration component has been moved from `backend/Orchestration` to `AppHost`.

Status
------

- Repository sweep performed on 2025-12-30: remaining developer docs, scripts and DevOps instructions referencing `backend/Orchestration` have been updated to `AppHost` where appropriate.
- Files updated in this pass include:
  - `.github/instructions/copilot-instructions-devops.md`
  - `.github/instructions/copilot-instructions-devops 2.md`
  - `.github/copilot-instructions.md` (where applicable)
  - `.vscode/tasks.json`, `.vscode/launch.json.full`, and `.vscode/launch.json` (validated for AppHost paths)
  - `scripts/run-login-e2e-tests.sh` and `scripts/kill-all-services.sh`
  - `QUICK_START_GUIDE.md`, `README.md`, and several docs under `docs/architecture/`

Completed Actions
-----------------

1. Replaced `cd backend/Orchestration && dotnet run` examples with `cd AppHost && dotnet run` across the repository where appropriate.
2. Updated DevOps instruction files to reference `AppHost` and AppHost build/run commands.
3. Verified `AppHost/Program.cs` exists and used it to run local e2e tests (Playwright auth suite passed locally).

Notes
-----

- This status file intentionally records that a migration occurred; it now documents that the migration and repo sweep were performed on 2025-12-30.
- If you want, I can open a PR containing all edits or generate a short issue comment summarizing these changes for review.

Acknowledgement
---------------

If you want a PR, reply `open PR` and I'll prepare it. If you prefer a direct commit, no further action is needed.
