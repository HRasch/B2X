# Subagents Resource Library

This folder contains archived SubAgent definition files kept as a resource library. These files are not active agents and should not be treated as first-class agent personas in agent-selection tooling.

Purpose:
- Preserve historical SubAgent definitions for reference and reuse.
- Provide on-demand templates for creating new agent definitions or restoring specialized behaviours.

Usage:
- Agents and tools should ignore this folder when enumerating active agents.
- To search the resource library programmatically, read files under `.github/agents/subagents/`.
- To re-instate a SubAgent as active, open an issue or PR and assign `@SARAH` / `@GitManager` for governance.

Maintenance:
- Keep files read-only and small; prefer linking to canonical family agent files for current workflows.
- Update this README if policies for re-activation or usage change.
