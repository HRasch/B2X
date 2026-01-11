---
docid: PRM-023
title: Cleanup Workspace
owner: @DocMaintainer
status: Active
created: 2026-01-10
---

# Cleanup Workspace Prompt

**DocID**: `PRM-023`  
**Owner**: @DocMaintainer  
**Status**: Active  
**Created**: January 10, 2026  

## Purpose
On-demand cleanup of workspace pollution, including documentation, reports, and temporary files. Relocates files to designated folders and archives old artifacts per [GL-010] Agent & Artifact Organization.

## Trigger
- Manual invocation: `/cleanup-workspace`
- Event-driven: During quality gates or after agent handoffs

## Scope
- Scan root directory for non-essential files (e.g., *.txt, *.md not in docs/, reports)
- Relocate to appropriate folders (e.g., reports → docs/, benchmarks → benchmark-results/)
- Archive files >7 days old to .ai/archive/
- Validate against project structure guidelines
- Include safety checks (confirmation prompts)

## Workflow
1. Scan workspace root
2. Identify files to move/archive
3. Prompt for confirmation
4. Execute moves/archives
5. Update .ai/status/ with summary

## Token Optimization
- Use MCP tools (e.g., GitKraken) for file operations
- Aggregate actions to minimize iterations
- Reference [GL-006] for efficiency

## Output
- Summary of actions taken
- Updated file structure
- Any blocked actions (e.g., protected files)

## Dependencies
- [GL-010] Agent & Artifact Organization
- [GL-006] Token Optimization Strategy
- MCP: GitKraken for file moves