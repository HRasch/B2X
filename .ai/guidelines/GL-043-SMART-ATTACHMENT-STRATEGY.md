---
docid: GL-043
title: Smart Attachment Strategy - Path-Specific Loading
owner: "@SARAH"
status: Active
created: "2026-01-07"
---

# GL-043: Smart Attachment Strategy

**Estimate**: 50-70% Token Savings | **Effort**: ⭐ Small

## Purpose

Minimize token consumption by loading **only path-specific instruction files** instead of all 6 instruction files for every interaction.

---

## 🎯 Quick Rule

**BEFORE (Wasteful)**:
```yaml
attachments:
  - backend-essentials.instructions.md          # 1.2 KB
  - frontend-essentials.instructions.md         # 1.1 KB
  - testing.instructions.md                     # 8+ KB
  - devops.instructions.md                      # 3+ KB
  - security.instructions.md                    # 2+ KB
  - mcp-quick-reference.instructions.md         # 2+ KB
# Total: ~18+ KB loaded for EVERY interaction
```

**AFTER (Optimized)**:
```yaml
attachments:
  # For frontend files (src/components/**, src/pages/**, etc.)
  - frontend-essentials.instructions.md         # 1.1 KB ONLY
  
  # For backend files (src/api/**, backend/**, etc.)
  - backend-essentials.instructions.md          # 1.2 KB ONLY
  
  # For test files (**/*.test.*, **/*.spec.*, tests/**)
  - testing.instructions.md                     # 8+ KB (when needed)
  
  # For DevOps (.github/**, Dockerfile, *.yml)
  - devops.instructions.md                      # 3+ KB (when needed)
  
  # ALWAYS: Global security rules (applies to all files)
  - security.instructions.md                    # 2 KB (UNIVERSAL)
```

---

## 📋 Path-to-Instruction Mapping

| File Pattern | Primary Instruction | Secondary | Notes |
|---|---|---|---|
| `src/api/**` | backend-essentials | security | Backend-specific |
| `src/services/**` | backend-essentials | security | Backend-specific |
| `src/models/**` | backend-essentials | security | Backend-specific |
| `backend/**` | backend-essentials | security | Backend-specific |
| `src/components/**` | frontend-essentials | security | Frontend-specific |
| `src/pages/**` | frontend-essentials | security | Frontend-specific |
| `src/hooks/**` | frontend-essentials | security | Frontend-specific |
| `src/ui/**` | frontend-essentials | security | Frontend-specific |
| `frontend/**` | frontend-essentials | security | Frontend-specific |
| `**/*.test.*` | testing | security | Test-specific |
| `**/*.spec.*` | testing | security | Test-specific |
| `tests/**` | testing | security | Test-specific |
| `**/__tests__/**` | testing | security | Test-specific |
| `.github/**` | devops | security | DevOps-specific |
| `Dockerfile` | devops | security | DevOps-specific |
| `*.yml` | devops | security | DevOps-specific |
| `*.yaml` | devops | security | DevOps-specific |
| `infra/**` | devops | security | DevOps-specific |
| `terraform/**` | devops | security | DevOps-specific |

---

## 🔧 Implementation Rules

### Rule 1: Always Attach Security
```yaml
# ALWAYS include security.instructions.md
# It applies to **/* (all files)
attachments:
  - security.instructions.md          # ALWAYS
```

### Rule 2: Single Primary Instruction
```yaml
# Load ONLY the primary instruction for the file being edited
# If editing: src/components/Header.vue
# Then attach: frontend-essentials.instructions.md ONLY
#
# DON'T attach: backend, devops, testing
```

### Rule 3: MCP Quick Reference on Demand
```yaml
# Load mcp-quick-reference.instructions.md ONLY when:
# - Explicitly discussing MCP tools
# - Using kb-mcp/* commands
# - Troubleshooting MCP integration
#
# OTHERWISE: Skip it (reference via [MCP-QUICK-REF] instead)
```

### Rule 4: Testing Instructions Only for Test Files
```yaml
# Load testing.instructions.md ONLY when:
# - Creating/modifying test files
# - Working in tests/** or **/*.test.* or **/*.spec.*
#
# DON'T load for regular backend/frontend work
```

---

## 📊 Token Savings Example

### Scenario 1: Frontend Component Development
**Old approach** (18+ KB):
```
- backend-essentials.instructions.md
- frontend-essentials.instructions.md
- testing.instructions.md
- devops.instructions.md
- security.instructions.md
- mcp-quick-reference.instructions.md
```

**New approach** (3.1 KB):
```
- frontend-essentials.instructions.md      (1.1 KB)
- security.instructions.md                 (2 KB)
```

**Savings**: ~15 KB = ~5,000 tokens = 83% reduction ✅

### Scenario 2: Backend API Development
**Old approach** (18+ KB):
```
- All 6 files
```

**New approach** (3.2 KB):
```
- backend-essentials.instructions.md       (1.2 KB)
- security.instructions.md                 (2 KB)
```

**Savings**: ~15 KB = ~5,000 tokens = 83% reduction ✅

### Scenario 3: Test Writing
**Old approach** (18+ KB):
```
- All 6 files
```

**New approach** (10 KB):
```
- testing.instructions.md                  (8 KB)
- security.instructions.md                 (2 KB)
```

**Savings**: ~8 KB = ~2,600 tokens = 44% reduction ✅

---

## ✅ Checklist for Agents

When opening a file to edit, check:

- [ ] Is this a **frontend** file? Attach `frontend-essentials.instructions.md`
- [ ] Is this a **backend** file? Attach `backend-essentials.instructions.md`
- [ ] Is this a **test** file? Attach `testing.instructions.md`
- [ ] Is this a **DevOps** file? Attach `devops.instructions.md`
- [ ] **Always** attach `security.instructions.md`
- [ ] Only attach `mcp-quick-reference.instructions.md` if discussing MCP tools

---

## 🚀 Quick Reference

**Copy this for your Copilot context:**

```yaml
# Smart Attachment Strategy (GL-043)
# Load ONLY path-specific instruction, plus security

# For Frontend: frontend-essentials.instructions.md + security.instructions.md
# For Backend: backend-essentials.instructions.md + security.instructions.md
# For Testing: testing.instructions.md + security.instructions.md
# For DevOps: devops.instructions.md + security.instructions.md
# For Discussing MCPs: Add mcp-quick-reference.instructions.md

# Default: Load only what's relevant to the current file
```

---

## 📝 Migration Timeline

**Week 1 (Jan 7-13)**:
- [ ] All agents adopt GL-043 for new interactions
- [ ] No changes needed to existing files

**Week 2 (Jan 14-20)**:
- [ ] Update `.github/copilot-instructions.md` with GL-043 reference
- [ ] All agents follow path-specific loading

**By Jan 27**:
- [ ] 50-70% average token reduction across all interactions
- [ ] Rate limiting pressure significantly reduced

---

## 📊 Expected Impact

| Metric | Before | After | Savings |
|--------|--------|-------|---------|
| Avg Attachments/Interaction | 6 files | 2 files | 67% |
| Avg Attachment Size | 18+ KB | 3-10 KB | 50-85% |
| Tokens per Interaction | Baseline | -5,000 avg | **42%** |
| Monthly Rate Limit Headroom | Limited | +40% | Significant |

---

## 🔄 Maintenance

- **Review monthly**: Check attachment patterns in conversations
- **Update mapping**: If new file patterns emerge, add to table above
- **Refine sizes**: Keep `.instructions.md` files under stated limits

---

**Maintained by**: @SARAH  
**Last Updated**: 7. Januar 2026
