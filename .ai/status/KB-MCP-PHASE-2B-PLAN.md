---
docid: STATUS-019
title: KB MCP PHASE 2B PLAN
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: KB-MCP-PHASE-2B-PLAN
title: Phase 2b - Attachment Removal Plan
owner: @SARAH
status: Planned
---

# Phase 2b - Attachment Removal Plan

**Status**: Planned for execution after 1-week validation period  
**Date**: 7. Januar 2026  
**Owner**: @SARAH

## Overview

Phase 2b removes large instruction file attachments and switches to compressed essentials versions, realizing the final ~28 KB of token savings for a total 87.6% reduction.

## Files to Remove

| File | Current Size | Replacement | Savings |
|------|-------------|-------------|---------|
| `.github/instructions/mcp-operations.instructions.md` | 16.5 KB | `mcp-quick-reference.instructions.md` (2.0 KB) | 14.5 KB |
| `.github/instructions/backend.instructions.md` | 9.6 KB | `backend-essentials.instructions.md` (1.2 KB) | 8.4 KB |
| `.github/instructions/frontend.instructions.md` | 10.7 KB | `frontend-essentials.instructions.md` (1.1 KB) | 9.6 KB |
| **TOTAL** | **36.8 KB** | **4.3 KB** | **32.5 KB** |

## Execution Steps

### Pre-Execution Checklist
- [ ] Phase 2a validation reports reviewed by team
- [ ] 1-week monitoring period completed
- [ ] No critical issues reported
- [ ] Backup of original files created

### Removal Process
1. **Create backups** of original files in `.ai/backups/phase-2b/`
2. **Remove large files** from `.github/instructions/`
3. **Update references** in copilot-instructions.md to point to essentials
4. **Run validation suite** to ensure no broken references
5. **Test KB-MCP functionality** with new configuration

### Post-Execution Validation
- [ ] All MCP tools still functional
- [ ] No broken references
- [ ] Context size reduced as expected
- [ ] Copilot responses working correctly

## Risk Mitigation

### Rollback Plan
If issues arise, rollback by:
1. Restore files from `.ai/backups/phase-2b/`
2. Revert copilot-instructions.md changes
3. Run validation suite

### Fallback Mechanisms
- KB-MCP has built-in fallback to direct file reading
- Compressed instructions contain essential information
- Full documentation available via KB-MCP tools

## Expected Impact

### Token Savings
- Additional: 32.5 KB per KB query
- Combined: 87.6% total reduction (74.8 KB → 9.3 KB)

### Performance Impact
- Faster context loading
- Reduced memory usage
- Better Copilot response times

## Timeline

- **Start**: After 1-week validation period (14. Januar 2026)
- **Duration**: 2-4 hours
- **Rollback Window**: 24 hours post-execution

## Success Criteria

- [ ] All large instruction files removed
- [ ] Compressed versions active
- [ ] No broken references
- [ ] Context size reduced by ~32 KB
- [ ] KB-MCP tools functional
- [ ] Team feedback positive

## Communication Plan

- **Pre-execution**: Team notification 24 hours before
- **During execution**: Status updates
- **Post-execution**: Results summary and impact analysis

---

**Next**: Execute after team validation period completes.