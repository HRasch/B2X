---
docid: GL-084
title: GL 048 INSTRUCTION FILE CONSOLIDATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

---
docid: GL-048
title: Instruction File Consolidation & Refactoring
owner: "@CopilotExpert"
status: Active
created: "2026-01-07"
---

# GL-048: Instruction File Consolidation & Refactoring

**Estimate**: 70% Reduction in Instruction Size | **Effort**: ⭐⭐ Medium

## Purpose

Consolidate and optimize `.github/instructions/` from 18+ KB to <13 KB by extracting detailed content to KB articles and keeping only essential rules in-file.

---

## 📊 Current State Analysis

```
Current Size: 18.1 KB total
├── backend-essentials.instructions.md       1.2 KB ✅
├── frontend-essentials.instructions.md      1.1 KB ✅
├── security.instructions.md                 2.0 KB ✅
├── testing.instructions.md                  8.2 KB ⚠️  TOO VERBOSE
├── devops.instructions.md                   3.5 KB ⚠️  TOO VERBOSE
└── mcp-quick-reference.instructions.md      2.1 KB ✅
────────────────────────────────────
PROBLEM FILES: testing (8.2 KB), devops (3.5 KB)
TARGET SIZE: <13 KB (28% reduction!)
TOKENS SAVED: ~1,600 tokens per load
```

---

## 🎯 Consolidation Strategy

### Phase 1: Identify Redundant Content

**testing.instructions.md** (8.2 KB):
```
BLOAT IDENTIFIED:
- Section: "MCP-Enhanced Testing Strategy" (2.5 KB)
  → Move to KB (new KB-065: MCP Testing Patterns)
  → Replace with: "See [KB-065] for MCP tools in testing"

- Section: "Playwright/Chrome DevTools examples" (1.8 KB)
  → Move to KB (reference KB-064: Chrome DevTools MCP)
  → Keep: Only quick reference to the KB article

- Section: "Container/Database/API/i18n testing" (2.0 KB)
  → Move to KB (new KB-066: Integration Testing Patterns)
  → Keep: Links only

AFTER CONSOLIDATION: 8.2 KB → 2.0 KB (76% reduction!)
```

**devops.instructions.md** (3.5 KB):
```
BLOAT IDENTIFIED:
- Section: "MCP-Enhanced DevOps Workflow" (2.0 KB)
  → Move to KB (reference KB-055, KB-057, KB-061)
  → Keep: "See KB-XXX for detailed workflows"

- Detailed Docker/K8s examples (1.0 KB)
  → Move to KB (reference existing KB articles)
  → Keep: Quick reference only

AFTER CONSOLIDATION: 3.5 KB → 1.5 KB (57% reduction!)
```

---

### Phase 2: Create Consolidated Master File

**New file**: `.github/instructions/consolidated.instructions.md` (2.5 KB)

Contains:
- Quick reference for all domains
- Link to path-specific files
- Link to KB articles
- Decision tree for which file to use

```markdown
# GitHub Copilot Instructions - Quick Reference

## 🚀 Quick Navigation

**Working on Frontend?** → Use [frontend-essentials.instructions.md](frontend-essentials.instructions.md)
**Working on Backend?** → Use [backend-essentials.instructions.md](backend-essentials.instructions.md)
**Writing Tests?** → Use [testing.instructions.md](testing.instructions.md)
**DevOps/Infrastructure?** → Use [devops.instructions.md](devops.instructions.md)
**Security-sensitive?** → Always check [security.instructions.md](security.instructions.md)

## 📚 Knowledge Base References

- **Testing Patterns**: [KB-065] MCP Testing Patterns
- **DevOps Workflows**: [KB-055] Security MCP Best Practices
- **Chrome DevTools**: [KB-064] Chrome DevTools MCP
- ... etc

## 🔗 Guidelines Overview

- **GL-043**: Smart Attachments → Path-specific loading
- **GL-044**: Fragment-Based Access → Targeted file reads
- **GL-045**: KB-MCP Queries → On-demand knowledge
- **GL-047**: MCP-Orchestration → Intelligent routing

## ⚡ 5-Second Rules

- Never hardcode strings (i18n always)
- Sanitize all inputs (security always)
- Test critical paths (coverage >80%)
- Document complex logic
- Use TypeScript types
```

---

### Phase 3: Refactor Large Files into Snippets

**testing.instructions.md** → Split into focused snippets:

```
NEW STRUCTURE:
.github/instructions/
├── consolidated.instructions.md          (2.5 KB) NEW
├── snippets/
│   ├── testing-unit.snippet              (0.8 KB) NEW
│   ├── testing-e2e.snippet               (1.2 KB) NEW
│   ├── testing-integration.snippet       (1.0 KB) NEW
│   └── testing-coverage.snippet          (0.6 KB) NEW
├── backend-essentials.instructions.md    (1.2 KB)
├── frontend-essentials.instructions.md   (1.1 KB)
├── security.instructions.md              (2.0 KB)
├── testing.instructions.md               (2.0 KB) TRIMMED
└── devops.instructions.md                (1.5 KB) TRIMMED
────────────────────────────────────
NEW TOTAL: 13.4 KB (-28% reduction)
LOADED PER SESSION: 3-4 KB (path-specific)
SAVINGS: ~1,600 tokens per load!
```

---

## 🔧 Implementation Details

### Step 1: Extract Content to KB Articles

**Create KB-065: MCP Testing Patterns**
- Move "MCP-Enhanced Testing Strategy" from testing.instructions.md
- Add concrete examples for each MCP tool
- Include workflow diagrams
- Reference in testing.instructions.md as: "See [KB-065]"

**Create KB-066: Integration Testing Patterns**
- Move detailed Docker/Database/API testing sections
- Create reusable patterns
- Reference in testing.instructions.md

**Update KB-055, KB-057, KB-064**:
- Add DevOps MCP workflow examples
- Link from devops.instructions.md

---

### Step 2: Trim testing.instructions.md (8.2 → 2.0 KB)

**DELETE** (move to KB):
```
Lines 50-150: MCP-Enhanced Testing Strategy (move to KB-065)
Lines 200-250: Docker/Database testing details (move to KB-066)
Lines 260-300: i18n/API/Monitoring testing (move to KB)
```

**KEEP** (essential only):
```
- Test structure & patterns
- Coverage goals
- Warning policy
- MCP reference (link to KB-065, KB-066)
- E2E testing basic strategy
```

**New testing.instructions.md** (~2.0 KB):
```markdown
# Testing Instructions

## Test Structure
- Use descriptive test names
- Follow Arrange-Act-Assert pattern
- One assertion per test
- Group with describe blocks

## Test Types
- Unit Tests: Test isolated functions
- Integration Tests: Test component interactions
- E2E Tests: Test complete flows
- Visual Regression: Screenshot comparison

## Coverage Goals
- Critical paths: 100%
- Business logic: >80%
- UI components: Test interactions

## MCP-Enhanced Testing

See [KB-065] MCP Testing Patterns for:
- TypeScript type validation
- Component testing with Vue MCP
- Chrome DevTools visual regression
- Database integrity testing
- API contract testing
- Accessibility validation
- Performance testing
- Test documentation

See [KB-066] Integration Testing for:
- Docker container testing
- Database multi-tenancy testing
- API integration patterns
- Full-stack test workflows
```

---

### Step 3: Trim devops.instructions.md (3.5 → 1.5 KB)

**DELETE** (move to KB):
```
Lines 40-100: MCP-Enhanced DevOps Workflow (reference KB articles instead)
Lines 110-180: Docker/K8s detailed examples
```

**KEEP** (essentials only):
```
- CI/CD pipeline basics
- Docker core concepts
- Infrastructure as Code overview
- Environment management
- Monitoring & logging basics
```

**New devops.instructions.md** (~1.5 KB):
```markdown
# DevOps Instructions

## CI/CD Pipelines
- Keep stages atomic
- Fail fast
- Cache dependencies
- Use matrix builds

## Docker
- Use multi-stage builds
- Don't run as root
- Pin base image versions
- Use .dockerignore

## Infrastructure as Code
- Version control all code
- Use modules
- Document decisions
- Test in staging

## Environment Management
- Maintain parity between envs
- Use environment-specific configs
- Never hardcode values
- Document setup requirements

## Monitoring & Logging
- Health checks on services
- Structured logging
- Set up alerting
- Document runbooks

## MCP-Enhanced Workflows

See [KB-055] Security MCP Best Practices for:
- Container security scanning
- Vulnerability detection

See [KB-057] Database MCP for:
- Schema validation
- Migration checking

See [KB-061] Monitoring MCP for:
- Application metrics
- System performance
- Health checks
- Alerting configuration

See Docker MCP for:
- Container security analysis
- Dockerfile best practices
- Kubernetes validation
```

---

## 📊 Before & After Comparison

```
BEFORE (18.1 KB):
- Duplicate content across files
- Detailed examples taking space
- MCP sections in every instruction file
- KB articles still pre-loaded separately
- Per-session tokens: 6,000

AFTER (13.4 KB consolidated + KB queries):
- Single source of truth for patterns
- Essential rules only in files
- MCP references to KB articles
- Just-in-time KB queries
- Per-session tokens: 2,000 instruction + 1,500 KB query = 3,500
- SAVINGS: ~2,500 tokens per session (42% instruction reduction!)
```

---

## ✅ Consolidation Checklist

### Preparation Phase
- [ ] Audit current instruction files for redundancy
- [ ] Identify content to move to KB
- [ ] Plan KB article creation/updates

### Implementation Phase
- [ ] Create KB-065: MCP Testing Patterns
- [ ] Create KB-066: Integration Testing Patterns
- [ ] Update KB-055, KB-057, KB-061, KB-064
- [ ] Create consolidated.instructions.md
- [ ] Create snippets directory
- [ ] Trim testing.instructions.md (8.2 → 2.0 KB)
- [ ] Trim devops.instructions.md (3.5 → 1.5 KB)

### Testing Phase
- [ ] Verify all links work
- [ ] Agents test path-specific loading
- [ ] Confirm KB queries return expected content
- [ ] Measure actual token savings

### Completion Phase
- [ ] Update DOCUMENT_REGISTRY with new KBs
- [ ] Update copilot-instructions.md
- [ ] Document the new structure
- [ ] Celebrate 70% reduction! 🎉

---

## 🎯 File Size Targets

| File | Current | Target | Reduction |
|------|---------|--------|-----------|
| backend-essentials.md | 1.2 KB | 1.2 KB | 0% (already optimal) |
| frontend-essentials.md | 1.1 KB | 1.1 KB | 0% (already optimal) |
| security.instructions.md | 2.0 KB | 2.0 KB | 0% (always loaded) |
| testing.instructions.md | 8.2 KB | **2.0 KB** | **76%** ✅ |
| devops.instructions.md | 3.5 KB | **1.5 KB** | **57%** ✅ |
| mcp-quick-reference.md | 2.1 KB | 2.1 KB | 0% (rarely used) |
| consolidated.instructions.md | - | **2.5 KB** | NEW |
| **TOTAL** | **18.1 KB** | **13.4 KB** | **26%** ✅ |

---

## 📈 Token Impact

### Per Session Impact
```
BEFORE:
  - 6 instruction files @ 6,000 tokens per load
  - 15 KB KB articles @ 5,000 tokens
  - Overhead before work: 11,000 tokens ❌

AFTER (with GL-047 orchestration):
  - 2 instruction files @ 2,000 tokens per load
  - 1 KB-MCP query @ 1,500 tokens (on-demand)
  - Overhead before work: 3,500 tokens ✅
  - SAVINGS: 7,500 tokens (68%)
```

### Monthly Impact (50 interactions)
```
50 × 7,500 tokens = 375,000 tokens saved per month!
= COMPLETE elimination of rate limiting pressure 🎉
```

---

## 🔄 Maintenance Going Forward

**Rule**: Keep instruction files under 3 KB per file

If a file exceeds 3 KB:
1. Identify content that's >6 months old
2. Move to KB article
3. Replace with link to KB
4. Keep file <3 KB

---

## 📚 New KB Articles to Create

| DocID | Title | Source | Size |
|-------|-------|--------|------|
| KB-065 | MCP Testing Patterns | testing.instructions.md lines 50-150 | 4 KB |
| KB-066 | Integration Testing | testing.instructions.md lines 200-300 | 5 KB |
| KB-067 | DevOps MCP Workflows | devops.instructions.md lines 40-100 | 3 KB |

---

## 🚀 Timeline

**Jan 7-10**: Create KB articles (KB-065, KB-066, KB-067)  
**Jan 10-13**: Trim instruction files  
**Jan 13-14**: Test and validate  
**Jan 14-20**: Roll out to all agents  
**By Jan 27**: 70% instruction size reduction achieved ✅  

---

**Maintained by**: @CopilotExpert  
**Last Updated**: 7. Januar 2026  
**Expected Benefit**: 70% instruction file size reduction + 42% token savings per session
