Title: Agent knowledge request — additional required knowledge
Status: open

Purpose
-------
Requesting each agent to list the specific topics, domain knowledge, or external references they need added to the `.ai/knowledgebase/` to unblock work on upcoming tasks.

Requested from each agent
-------------------------
- @Backend: API contracts, Wolverine endpoints, event/message schemas, and DB migration notes you need.
- @Frontend: critical UI flows, component-level docs (Vue + Pinia), E2E test scenarios, and bundles you want summaries for.
- @Security: threat models, required OWASP mappings, SAST/SCA tools to integrate, and any compliance references.
- @DevOps: infra patterns, deployment playbooks (K8s manifests, docker-compose), and CI steps to document.
- @QA: test strategy, required test data, and critical acceptance criteria to document.
- @Architect / @TechLead: design patterns, ADRs, performance targets, and benchmarks to record.

Reply format
------------
- Provide a short list of topics (1–6) and 1–2 high-value links per topic.
- Optional: a short priority (P0/P1/P2) and suggested filename under `.ai/knowledgebase/`.

Next steps (for SARAH / Copilot)
-------------------------------
1. Collect agent replies into `.ai/issues/agent-knowledge-responses.md`.
2. Research and draft summaries for high-priority topics and add them to `.ai/knowledgebase/`.
3. Notify agents when drafts are available for review.

Please reply in this file (append your sections) or create a PR referencing this file.

--
Notifications:

- Sent: 2025-12-30 — Notification appended here and `agent-knowledge-responses.md` created to collect replies.
- Message sent to agents (copy):

	Please provide a short list of topics (1–6) and 1–2 high-value links per topic that you need added to the `.ai/knowledgebase/` to unblock work. Optionally add a priority (P0/P1/P2) and a suggested filename. Reply directly in `/.ai/issues/agent-knowledge-responses.md`.
