---
title: Documentation Validation Report
date: 2025-12-31
author: DocMaintainer
---

# Validation Report — 2025-12-31

Summary:
- Performed automated checks for deprecated prompt frontmatter (`mode:`) and agent `model:` values.

Findings:
- No `mode: agent` occurrences were found in `.github/prompts/`.
- A small number of example occurrences of `mode: agent` remain in `.ai/collaboration/AGENT_COORDINATION.md` (these are documentation examples and not runtime prompts).
- All `model:` entries in `.github/agents/` and subagents were standardized to `'gpt-5-mini'`.

Recommendations:
- Update example usage in `.ai/collaboration/AGENT_COORDINATION.md` to use `agent:` instead of `mode:` to avoid confusion.
- Run a full broken-link scan across `.ai/` and `.github/prompts/` (external link checks are recommended in CI) and fix reported paths.
- Commit changes and include this validation report in the commit message.

Actions taken:
- Created this validation report and added it to `.ai/logs/documentation/`.
