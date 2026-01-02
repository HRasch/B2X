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
