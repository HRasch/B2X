---
docid: UNKNOWN-142
title: Agent Review Markdown And Rewrite Vision Issues
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

```markdown
Title: Agent task — Review `markdown-supported-on-github.md` and rewrite issues labeled "vision"
Status: open

Purpose
-------
Requesting every agent to review the new GFM guidance document and then find and rewrite repository issues labeled `vision` so they follow the project's Markdown and clarity standards.

Scope
-----
- File to review: `.ai/knowledgebase/markdown-supported-on-github.md`
- Target issues: all open issues in the repository that have the label `vision`.

Requested from each agent
-------------------------
- Read the doc and flag any syntax/formatting inaccuracies, missing examples, or places that would benefit from clearer wording.
- Propose (or apply) a small PR or direct edit to `.ai/knowledgebase/markdown-supported-on-github.md` with suggested fixes.
- Find all open issues labeled `vision` and either:
  - Rewrite the issue body to use correct GFM (headings, lists, code fences) and clearer language, preserving intent; or
  - If you do not have permission, create a suggested rewrite as a comment on the issue with a copy of the proposed body and a brief rationale.

Guidelines for issue rewrites
----------------------------
- Keep intent and acceptance criteria intact.
- Use short headings, bullet lists for steps/requirements, and code fences for examples.
- Avoid agent role mentions that could create accidental GitHub @-mentions; render role handles as inline code (e.g., `@TechLead`).
- If the issue contains long narrative, move details into an attached file (e.g., `.ai/issues/{issue-number}-details.md`) and keep the issue body concise with a short summary and link.

Reply format (append to this file)
---------------------------------
- Agent: `@AgentName`
- Action taken: `Reviewed / Suggested PR / Edited issue X / Commented on issue Y`
- Notes: short bullets of changes or rationale
- Links: PR or issue links (if applicable)

Priority & Deadline
-------------------
- Priority: P1 — useful but non-blocking
- Please respond within 3 working days. Mark this file with your entry when done.

Next steps (for SARAH / GitHub Copilot)
--------------------------------------
1. Collect agent replies here and in `.ai/issues/agent-knowledge-responses.md`.
2. If multiple conflicting rewrite proposals appear, create a short poll or reviewer assignment to pick the final wording.
3. After edits are merged or comments applied, add a short status note here with links to updated issues.

Notes
-----
- Agents should avoid making substantive scope changes to `vision` issues — preserve original intent unless the reporter is reachable and agrees. If a scope change is proposed, include a clear reasoning note and request reporter confirmation.
- If you lack permission to edit an issue, post the suggested rewrite as a comment and tag the original author (using inline code for agent handles to avoid accidental mentions).

--
Issued by: GitHub Copilot (on request)
Date: 2025-12-31

```
