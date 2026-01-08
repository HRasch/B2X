---
docid: LOG-005
title: AGENT_MODEL_AUDIT_2025_12_31
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# AGENT MODEL AUDIT
Date: 2025-12-31

Default model policy: `gpt-5-mini` (see .github/copilot-instructions.md)

Summary:
- /Users/holger/Documents/Projekte/B2X/.github/agents/Architect.agent.md: **model not set**
- /Users/holger/Documents/Projekte/B2X/.github/agents/Backend.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/DevOps.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/DevelopmentTeam.agent.md: **model not set**
- /Users/holger/Documents/Projekte/B2X/.github/agents/DocMaintainer.agent.md: **model not set**
- /Users/holger/Documents/Projekte/B2X/.github/agents/Frontend.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/Legal.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/ProductOwner.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/QA.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/SARAH.agent.md: **model not set**
- /Users/holger/Documents/Projekte/B2X/.github/agents/SEO.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/ScrumMaster.agent.md: **model not set**
- /Users/holger/Documents/Projekte/B2X/.github/agents/Security.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/TechLead.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/UI.agent.md: gpt-5-mini
- /Users/holger/Documents/Projekte/B2X/.github/agents/UX.agent.md: gpt-5-mini

Non-default models found: 0

Recommended actions:
1. For each non-default model, either:
   - Keep it as an approved override: add `justification:` frontmatter and open a PR referencing this audit, then request `@SARAH` approval.  Or
   - Revert to `gpt-5-mini` by editing the agent file and documenting the change in the audit log (requires PR).
2. `DocMaintainer` to validate model identifiers and flag unsupported values.

Automated listing produced by DocMaintainer.