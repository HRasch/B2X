---
docid: ISSUE-001
title: Implement Cleanup Skill for @DocMaintainer
owner: @SARAH
status: Active
created: 2026-01-10
---

# Issue: Implement Cleanup Skill for @DocMaintainer

**DocID**: `ISSUE-001`  
**Owner**: @SARAH  
**Status**: Active  
**Created**: January 10, 2026  

## Summary
Implement a new cleanup skill within @DocMaintainer to address ongoing workspace pollution with documentation, reports, and temporary files. This is an event-driven, on-demand solution per user preferences (no scheduled tasks).

## Background
- Project suffers from root directory pollution (e.g., audit_results.txt, compliance_documentation.md).
- Causes: Lack of enforcement, agent autonomy, ad-hoc processes.
- Previous solutions rejected scheduled automation; need manual/event-driven approach.

## Requirements
- **Skill**: Add `/cleanup-workspace` command to @DocMaintainer.
- **Scope**: Scan root, relocate files to designated folders (e.g., docs/, .ai/archive/), archive old files.
- **Safety**: Include confirmation prompts to prevent accidental deletions.
- **Integration**: Use MCP tools (e.g., GitKraken) for file operations.
- **Token Optimization**: Minimize iterations per [GL-006].

## Implementation Steps
1. ✅ Create prompt file: `.github/prompts/cleanup-workspace.prompt.md` (PRM-023)
2. ✅ Update registries: DOCUMENT_REGISTRY.md and PROMPTS_INDEX.md
3. [ ] Update @DocMaintainer agent instructions to include the skill
4. [ ] Test the skill with a sample cleanup
5. [ ] Document usage in [GL-010]

## Acceptance Criteria
- ✅ @DocMaintainer can invoke `/cleanup-workspace` successfully.
- ✅ Files are relocated/archived without errors.
- ✅ No scheduled automation; manual only.
- ✅ Token usage optimized.

## Progress
- **2026-01-10**: Prompt created, registries updated. @DocMaintainer agent instructions updated with cleanup skill. Testing completed: relocated 3 files (audit_results.txt → docs/archive/, compliance_documentation.md → docs/, deployment_plan.md → docs/).

## Related Docs
- [GL-010] Agent & Artifact Organization
- [GL-006] Token Optimization Strategy
- [PRM-023] Cleanup Workspace Prompt