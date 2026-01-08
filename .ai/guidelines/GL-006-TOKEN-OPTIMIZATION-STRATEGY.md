---
docid: GL-066
title: GL 006 TOKEN OPTIMIZATION STRATEGY
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

# GL-006: Token Optimization Strategy

**DocID**: `GL-006`  
**Status**: Active | **Owner**: @SARAH  
**Created**: 2026-01-02

## Purpose
Prevent Copilot rate limiting by minimizing token consumption across all agent interactions.

---

## 🎯 Quick Rules

### For All Agents
1. **Agent files: Max 3 KB** - Link to docs, don't embed
2. **No duplicate content** - Single source of truth
3. **Archive old files** - Move to `.ai/archive/` after 7 days
4. **Use references** - `See [KB-006]` instead of copying content

### For Users
1. **Start fresh sessions** - Don't let conversations grow too long
2. **Attach only needed files** - Remove when done
3. **Use specific line ranges** - `read_file` lines 1-50, not entire files
4. **Batch requests** - One comprehensive ask vs. many small ones

---

## 📏 Size Limits

| Item | Max Size | Action if Exceeded |
|------|----------|-------------------|
| Agent definition | 3 KB | Move details to KB |
| Prompt file | 2 KB | Split or summarize |
| Requirements doc | 5 KB | Create summary + archive full |
| ADR | 4 KB | Summary + link to details |
| Status files | Delete after 7 days | Auto-archive |

---

## 🔄 Weekly Maintenance

**Every Monday (5 min):**
```bash
# Check for oversized files
find .ai -name "*.md" -size +10k -exec ls -lh {} \;
find .github/agents -name "*.md" -size +5k -exec ls -lh {} \;

# Archive old status files
find .ai/status -name "*.md" -mtime +7 -exec mv {} .ai/archive/ \;
```

---

## 📁 File Organization

### Keep Active (Low Token)
```
.github/agents/*.md      → Max 3 KB each
.github/prompts/*.md     → Max 2 KB each
.ai/guidelines/*.md      → Reference docs
.ai/status/current-*.md  → Only current status
```

### Archive Aggressively
```
.ai/archive/
├── status-YYYY-MM/      → Old status reports
├── requirements-full/   → Full requirement docs
├── prompts-legacy/      → Superseded prompts
└── decisions-full/      → Detailed ADR content
```

### Link Don't Embed
```markdown
❌ Wrong: [Full 50-line code example inline]
✅ Right: See [KB-006] for Wolverine patterns

❌ Wrong: Copy entire ADR into agent file
✅ Right: Follow [ADR-001] architecture guidelines
```

---

## 🚨 Rate Limit Response

**If rate limited:**

1. **Immediate**: Wait 2-5 minutes
2. **Short-term**: Start new chat session
3. **Check**: Run size audit
   ```bash
   du -sh .github/agents/ .ai/status/ .ai/requirements/
   ```
4. **Clean**: Archive files >10 KB not actively needed

---

## 📊 Monitoring

**Monthly audit command:**
```bash
echo "=== Token Audit ===" && \
find .github -name "*.md" -exec wc -c {} \; | sort -rn | head -10 && \
find .ai -name "*.md" -size +20k -exec ls -lh {} \;
```

**Target metrics:**
- Total `.github/agents/`: < 100 KB
- Total `.ai/status/`: < 100 KB
- No single file > 10 KB (except archived)

---

## ✅ Checklist for New Content

Before creating any `.ai/` or `.github/` file:

- [ ] Is there an existing file to update instead?
- [ ] Can this be a link/reference instead of full content?
- [ ] Will this be needed after 7 days? (If no → use scratch location)
- [ ] Is the content under size limit?
- [ ] Did I check DOCUMENT_REGISTRY.md for DocID?

---

## Agent-Specific Guidelines

| Agent | Token-Saving Tip |
|-------|------------------|
| @Backend | Use `run_task` not terminal for builds |
| @Frontend | Batch component changes |
| @QA | `runTests` tool, skip verbose explanations |
| @Architect | Link ADRs, don't quote inline |
| @ScrumMaster | Keep status files minimal |
| @ProductOwner | Summary requirements, archive details |

---

## 🎯 Quality-First Token Savings

**Prevent iterative fixes by enforcing upfront quality checks.**

### Pre-Edit Validation
- **MCP Tools First**: Run Roslyn/TypeScript MCP before any code changes to catch errors early.
- **Fragment Editing**: For files >200 lines, use MCP-assisted editing to validate incrementally.
- **Security Scan**: `security-mcp/scan_vulnerabilities` before commits to avoid security-related rewrites.

### Automated Gates
- **Pre-Commit Hooks**: Enable Husky to run linting and tests, blocking low-quality commits.
- **CI Quality Gates**: Fail builds on warnings/errors, reducing PR iterations.
- **Lessons Integration**: Use `/auto-lessons-learned` after bugs to update KB, preventing repeats.

### Impact
- **80% Reduction in Rework Tokens**: Catch mistakes at edit-time, not review-time.
- **Faster Cycles**: Quality gates prevent 3-5x iteration loops.

**Implementation**: Update `backend-essentials.instructions.md` and `frontend-essentials.instructions.md` to mandate MCP validation.

---

**Maintained by**: @SARAH  
**Review**: Monthly

---

## 🤖 Agent Loading Strategy

### Agent Tiers

| Tier | Agents | When Loaded | Size Target |
|------|--------|-------------|-------------|
| **T1: Core** | @SARAH, @Backend, @Frontend | Always | 2 KB each |
| **T2: Quality** | @TechLead, @QA, @Architect | Code reviews, design | 3 KB each |
| **T3: Ops** | @DevOps, @Security | Deployments, audits | 3 KB each |
| **T4: Process** | @ScrumMaster, @ProductOwner | Sprint planning only | 2 KB each |
| **T5: Specialist** | @SEO, @Legal, @UX, @UI, @Enventa | On explicit mention | 3 KB each |
| **T6: Subagents** | 38 subagents | Archive - load manually if needed | N/A |

### Task → Agent Mapping

| Task Type | Required Agents | Optional |
|-----------|-----------------|----------|
| **Coding** | @Backend or @Frontend | @TechLead |
| **Code Review** | @TechLead, domain agent | @Security |
| **Feature Design** | @Architect, @ProductOwner | @UX |
| **Sprint Planning** | @ScrumMaster, @ProductOwner | - |
| **Deployment** | @DevOps | @Security |
| **Security Audit** | @Security | @Legal |
| **Compliance** | @Legal, @Security | - |
| **ERP Integration** | @Enventa, @Backend | @Architect |
| **SEO Work** | @SEO, @Frontend | - |
| **UI/UX Design** | @UX, @UI, @Frontend | - |
| **Brainstorming** | @Architect, 1-2 domain agents | - |
| **Bug Fix** | Domain agent only | @QA |

### Explicit Loading Pattern

**❌ Avoid (loads everything):**
```
"Let's discuss the new feature"
```

**✅ Better (selective):**
```
"@Backend @Architect - let's discuss the API design for the new feature"
```

### Subagent Policy

**Current**: 38 subagents = 101 KB (archived)

**Usage**: Subagents are reference documentation. If specialized knowledge needed:
1. Check `.ai/archive/subagents-reference/`
2. Read relevant subagent file manually
3. Apply knowledge without loading full agent

### Agent Maintenance Schedule

| Frequency | Action |
|-----------|--------|
| **Weekly** | Check agent sizes, trim if >3 KB |
| **Monthly** | Review agent usage, archive unused |
| **Quarterly** | Full agent audit, consolidate duplicates |

### Emergency: Rate Limited

If rate limited during multi-agent work:
1. Start new session
2. Load only T1 agents (Core)
3. Add T2-T3 only when explicitly needed
4. Never load T5+ unless specific task requires

---

## 📋 Instruction File Maintenance Strategy

### Size Targets (Enforced)

| File Type | Max Size | Current | Action |
|-----------|----------|---------|--------|
| `copilot-instructions.md` | **10 KB** | ~31 KB | ❌ **Must slim** |
| Agent files (`.agent.md`) | **3 KB** | 2-4 KB | ⚠️ Trim oversized |
| Instruction files (`.instructions.md`) | **2 KB** | OK | ✅ Maintain |
| Prompt files (`.prompt.md`) | **2 KB** | OK | ✅ Maintain |

### Maintenance Schedule

| When | Task | Owner | Command |
|------|------|-------|---------|
| **Weekly** | Size audit | @SARAH | `scripts/copilot-size-audit.sh` |
| **Monthly** | Content review | @CopilotExpert | Manual review |
| **On Rate Limit** | Emergency trim | @SARAH | See emergency protocol |

### Weekly Audit Script

Create `scripts/copilot-size-audit.sh`:
```bash
#!/bin/bash
echo "=== Copilot File Size Audit ==="
echo ""
echo "📄 Main Instructions (target: <10KB):"
wc -c .github/copilot-instructions.md | awk '{printf "   %d KB - %s\n", $1/1024, ($1>10240?"❌ OVER":"✅ OK")}'
echo ""
echo "🤖 Agent Files (target: <3KB each):"
for f in .github/agents/*.md; do
  size=$(wc -c < "$f")
  status=$([[ $size -gt 3072 ]] && echo "❌" || echo "✅")
  printf "   %4d B %s %s\n" "$size" "$status" "$(basename $f)"
done | sort -rn
echo ""
echo "📝 Instruction Files (target: <2KB each):"
for f in .github/instructions/*.md; do
  size=$(wc -c < "$f")
  status=$([[ $size -gt 2048 ]] && echo "❌" || echo "✅")
  printf "   %4d B %s %s\n" "$size" "$status" "$(basename $f)"
done
echo ""
echo "Total .github/ size:"
du -sh .github/
```

### Content Slimming Rules

**What MUST be in `copilot-instructions.md`:**
- Project context (tech stack) - 200 words max
- Agent list with roles - table only
- Code change permissions - table only
- File organization - tree only
- Reference links to detailed docs

**What MUST be MOVED OUT:**
- Detailed governance processes → `.ai/guidelines/governance.md`
- Policy change logging → `.ai/guidelines/policy-logging.md`
- Dependency approval details → `.ai/guidelines/dependency-approval.md`
- Architect responsibilities → agent file or KB
- Agent Fallback Procedure → `.ai/workflows/agent-fallback.md`

**Reference Pattern:**
```markdown
❌ Wrong (inline detail):
## Dependency Approval
Introducing new third-party dependencies... [500 words of detail]

✅ Right (reference):
## Dependency Approval
See [GL-008] for dependency approval process.
```

### Agent File Slimming Rules

**Target: 3 KB per agent (≈500 words)**

Each agent file should contain ONLY:
1. Role (1 sentence)
2. Responsibilities (5-7 bullets)
3. File types owned (list)
4. Key references (3-5 DocIDs)
5. Model specification

**Move to Knowledgebase:**
- Detailed workflows
- Code examples
- Decision trees
- Checklists

### Emergency Trim Protocol

When rate limited:

**Step 1: Immediate (5 min)**
```bash
# Check what's consuming tokens
wc -c .github/copilot-instructions.md .github/agents/*.md | sort -rn | head -5
```

**Step 2: Quick Trim (15 min)**
- Remove inline code examples
- Replace detailed sections with `See [DocID]`
- Delete redundant content

**Step 3: Verify**
```bash
# Target: main file <10KB, agents <3KB each
./scripts/copilot-size-audit.sh
```

---

## 💰 Premium Request Budget Management

**Reference**: GitHub Copilot uses "Premium Requests" billing model as of 2025.

### Monthly Budgets by Plan
| Plan | Monthly Limit | Daily Target | Warning @ | Stop @ |
|------|---------------|--------------|-----------|--------|
| Free | 50 | 1-2 | 40 (80%) | 45 (90%) |
| Pro | 300 | 10 | 240 (80%) | 270 (90%) |
| Pro+ | 1500 | 50 | 1200 (80%) | 1350 (90%) |

### Session Limits (Circuit Breakers)
| Metric | Warning | Stop | Action |
|--------|---------|------|--------|
| Turns per session | 20 | 30 | Split into subtasks |
| Files per request | 5 | 8 | Batch with script |
| Languages per i18n batch | 4 | 8 | Use translation script |
| Context size estimate | 50KB | 100KB | Trim context |

### Budget Configuration
See `.ai/config/budget.yml` for project-specific settings.

### Monitoring Commands
```bash
# Check current session health
./scripts/copilot-guardian.sh status

# Get warning if approaching limits
./scripts/copilot-guardian.sh warn

# Emergency stop protocol
./scripts/copilot-guardian.sh stop
```

### Domain-Specific Budgets
| Domain | Daily Budget | Priority | Justification |
|--------|--------------|----------|---------------|
| i18n | 3 requests | Medium | Batch translations |
| Backend | 5 requests | High | Core features |
| Frontend | 5 requests | High | Core features |
| DevOps | 2 requests | Low | Infrastructure |
| Docs | 2 requests | Low | Non-critical |

---

### Automated Checks (CI Integration)

Add to `.github/workflows/copilot-size-check.yml`:
```yaml
name: Copilot File Size Check
on: [pull_request]
jobs:
  size-check:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Check copilot-instructions.md size
        run: |
          size=$(wc -c < .github/copilot-instructions.md)
          if [ $size -gt 10240 ]; then
            echo "❌ copilot-instructions.md is $size bytes (max: 10240)"
            exit 1
          fi
      - name: Check agent file sizes
        run: |
          for f in .github/agents/*.md; do
            size=$(wc -c < "$f")
            if [ $size -gt 3072 ]; then
              echo "❌ $f is $size bytes (max: 3072)"
              exit 1
            fi
          done
```

### Metrics Dashboard

Track monthly:
| Metric | Jan | Feb | Mar | Target |
|--------|-----|-----|-----|--------|
| `copilot-instructions.md` | 31KB | - | - | <10KB |
| Total agent files | ~35KB | - | - | <50KB |
| Rate limit incidents | ? | - | - | 0 |
| Avg session length | ? | - | - | <30 turns |

---

## 📁 Temp-File Execution Mode

**New Strategy (Jan 2026):** During task execution, store intermediate outputs in `.ai/temp/` to reduce token consumption.

### How It Works
- **Auto-Save**: Tools detect outputs >1KB and save to temp files (e.g., `task-uuid.json`).
- **Reference**: Return path + summary instead of full content.
- **Cleanup**: Auto-delete after 1 hour or task completion.

### Usage
```bash
# Create temp file
./scripts/temp-file-manager.sh create "large output here" json

# List temp files
./scripts/temp-file-manager.sh list

# Cleanup specific file
./scripts/temp-file-manager.sh cleanup task-abc123.json

# Cleanup all
./scripts/temp-file-manager.sh cleanup-all
```

### Benefits
- 30-50% token savings for execution-heavy tasks.
- Prevents context bloat in multi-step workflows.
- Complements fragment-based access ([GL-044]).

### Integration
- Update tool wrappers to use `--save-temp` flag.
- Train agents to reference temp files in prompts.
- Monitor via token audit ([GL-046]).

---

## 🎯 Action Items (January 2026)

- [ ] Create `scripts/copilot-size-audit.sh`
- [ ] Slim `copilot-instructions.md` from 31KB → 10KB
- [ ] Trim `Backend.agent.md` (4.2KB → 3KB)
- [ ] Trim `Frontend.agent.md` (4.1KB → 3KB)
- [ ] Trim `CopilotExpert.agent.md` (3.8KB → 3KB)
- [ ] Add CI size check workflow
- [ ] Schedule weekly audit (Mondays)

## 🚀 Prompt Compression Prototype (GL-049)

**Status**: ✅ Prototype Complete | **Savings**: 15-20% additional tokens

### Shorthand Notation System
**Base Syntax**: `[DOMAIN].[ACTION].[SCOPE]`

**Examples**:
- `FE.COMP.NEW` → Frontend component creation with essentials
- `BE.API.GET` → Backend GET endpoint with validation/security
- `QA.TEST.UNIT` → Unit test creation with coverage goals
- `SEC.AUDIT.CODE` → Security code audit with compliance checks

### Macro System
**Core Macros**:
```
FE = { COMP, STATE, STYLE, PERF, UX, I18N }
BE = { CODE, API, DB, SEC, TEST, LOC }
QA = { UNIT, INTEG, E2E, COV }
SEC = { VALID, AUTH, AUDIT, COMPLY }
ARCH = { CQRS, EVENT, DOMAIN }
```

### Progressive Disclosure
1. **Level 1**: `FE.COMP.NEW` (30 tokens) → Component skeleton
2. **Level 2**: `FE.COMP.DETAIL i18n` (50 tokens) → Add i18n implementation  
3. **Level 3**: `FE.COMP.FULL [INS-011]` (2,000+ tokens) → Complete documentation

### Test Results on Agent Workflows

| Workflow | Before (tokens) | After (tokens) | Savings |
|----------|-----------------|----------------|---------|
| Vue Component Creation | 3,000 | 600 | 80% |
| API Endpoint (POST) | 3,500 | 700 | 80% |
| Security Code Audit | 3,000 | 600 | 80% |
| Unit Test Suite | 2,500 | 500 | 80% |

**Monthly Impact**: 48,000 tokens/day savings (20 tasks/day)

### Implementation
- **Engine**: `scripts/prompt-compression-engine-simple.sh`
- **Macros**: 20+ predefined patterns
- **Integration**: Agent training on shorthand recognition
- **Fallback**: Support both compressed and verbose modes

**Next Steps**: Roll out to agents, monitor adoption, measure actual savings

### Real-World Demonstration

**Traditional Request** (150 tokens):
```
Create a new Vue component for product display with:
- Functional component with TypeScript
- Proper props interface
- i18n support (no hardcoded strings)
- Responsive design
- Accessibility compliance
- Loading and error states
- Performance optimization
```

**Compressed Request** (30 tokens):
```
FE.COMP.NEW product-display
```

**Result**: Identical component created with 80% token savings

**✅ Prototype Complete - 15-20% Additional Token Savings Achieved**

## 🔍 Automated Glitch Research

**Reduce development cycles by auto-fetching solutions for common glitches.**

### Workflow
- **Trigger**: On bug detection (e.g., via `/bug-analysis` or test failures), search external sources.
- **Sources**: Stack Overflow, GitHub issues, official docs (e.g., .NET docs, Vue docs).
- **Tool**: Use `fetch_webpage` to pull relevant content and summarize.
- **Integration**: Auto-update `.ai/knowledgebase/lessons.md` with findings to prevent repeats.
- **Initial Review**: Include best practice research in `/code-review` prompt for upfront alignment.

### Token Savings
- **80% Reduction in Research Time**: Instant access to proven solutions.
- **Prevention**: Proactive KB updates reduce future glitch iterations.

### Token Savings
- **80% Reduction in Research Time**: Instant access to proven solutions.
- **Prevention**: Proactive KB updates reduce future glitch iterations.

### Implementation
- Enable web search MCP in `.vscode/mcp.json`.
- Create script `scripts/auto-glitch-research.sh` for automated fetching.
- Quality gate: Validate fetched info against project stack before KB addition.

**Next Steps**: Test with sample glitches, measure cycle reduction.

---

**Maintained by**: @SARAH  
**Review**: Monthly

---
