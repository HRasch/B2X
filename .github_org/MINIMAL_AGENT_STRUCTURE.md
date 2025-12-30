# Minimal Agent Structure - Refactored

**Date**: 30. Dezember 2025  
**Objective**: Reduce documentation bloat from 23,432 lines → ~3,000 lines (87% reduction)  
**Principle**: DRY - Don't Repeat Yourself. One source of truth per concept.

---

## 📏 New Structure

### Layer 1: Core Agent Definition (1 file per agent)

**File**: `.github/agents/{agent-name}.agent.md`

**Content** (max 300-500 lines each):
- Header (model, description, tools)
- Mission (what this agent does - 1 paragraph)
- Authority/Responsibility (3-5 bullet points)
- Key Patterns (3-5 code examples max)
- Escalation Path (who to ask when)

**What's REMOVED**:
- ❌ Role-specific detailed guides (move to role-specific quick-start if needed)
- ❌ Process walkthroughs (each agent documents their own simple workflow)
- ❌ Repeated patterns across agents
- ❌ Historical context/retrospectives (in git, not in instructions)

### Layer 2: Quick Reference Per Role (optional, 1-2 pages)

**File**: `.github/roles/{role-name}.md`

**Content** (max 200 lines):
- Role summary (what does this role do?)
- Quick command reference (3-5 most common commands)
- Key files this role works with
- One escalation path

**When created**: Only if role has >5 lines of "how to" content

### Layer 3: Minimal Processes

**File**: `.github/docs/processes/{process-name}.md`

**Content** (max 150 lines):
- Title + scope
- Participants (who's involved)
- Steps (numbered, concise)
- Success criteria (3-5 bullet points)
- Links to related docs

**What's REMOVED**:
- ❌ Multiple versions of same workflow
- ❌ Decision trees (use clear if/then in steps)
- ❌ Detailed examples (one example maximum)
- ❌ Best practices (in agent instructions, not process docs)

### Layer 4: Collaboration System (minimal)

**File**: `/collaborate/README.md` (main entry point only)

**Content** (max 100 lines):
- What is collaboration system?
- When to use it (agent-to-agent coordination only)
- File structure (`/collaborate/issue/{N}/@{agent}/`)
- Link to PLAIN_COMMUNICATION_RULE.md

**What's REMOVED**:
- ❌ 20+ support files (archived or deleted)
- ❌ Duplicate explanation documents
- ❌ Architecture narrative (one sentence: "Agents use GitHub for team communication, mailbox for private coordination, monitor for auto events")

---

## 🎯 Cleanup Action Plan

### Phase 1: Agent Files (.github/agents/) - 37 agents, 10,357 lines

**Target**: 1,500-2,000 lines (300-500 per agent avg)

**Action**:
1. Keep only essential agents (15-20 core + 5 role-specialists)
2. Remove duplicate agents (@backend-admin, @backend-store, @frontend-admin, @frontend-store) - these are just implementations of @backend-developer and @frontend-developer
3. Archive stakeholder agents (@stakeholder-*) - reference in main README, not agent files
4. Compress each remaining agent to essentials (mission + authority + 3-5 patterns + escalation)

**Deletions** (consolidate into core agents):
- backend-admin.agent.md → @backend-developer pattern
- backend-store.agent.md → @backend-developer pattern
- frontend-admin.agent.md → @frontend-developer pattern
- frontend-store.agent.md → @frontend-developer pattern
- qa-frontend.agent.md → @qa-engineer responsibility
- qa-pentesting.agent.md → @qa-engineer responsibility
- qa-performance.agent.md → @qa-engineer responsibility
- stakeholder-*.agent.md (5 files) → archive in /collaborate/stakeholders/

**Result**: ~15 core agents × 300 lines = 4,500 lines

### Phase 2: Copilot Instructions (.github/copilot-instructions*.md) - 6,178 lines

**Target**: 1,000-1,500 lines

**Actions**:
1. Keep **copilot-instructions.md** (main reference) - trim to essentials only
2. Keep **copilot-instructions-backend.md**, **frontend.md**, **devops.md**, **security.md** (core roles)
3. DELETE: all role-specific variants (qa.md, scrum-master.md, product-owner.md, etc.) - move critical content to agent files
4. DELETE: -quickstart, -index, -refactored, -ai-specialist, -ui-expert, -ux-expert, -qa-reviewer, -software-architect - consolidate into agent files

**Keep Only** (500-800 lines each):
- copilot-instructions.md (main overview)
- copilot-instructions-backend.md
- copilot-instructions-frontend.md
- copilot-instructions-devops.md
- copilot-instructions-security.md

**Result**: 5 files × 300 lines = 1,500 lines

### Phase 3: Processes (.github/docs/processes/) - 4,925 lines

**Target**: 500-800 lines

**Actions**:
1. Keep only CRITICAL processes:
   - GOVERNANCE_RULES.md (130 lines) - who can do what
   - SPRINT_EXECUTION.md (100 lines) - how sprints work
   - CODE_REVIEW.md (80 lines) - code review process
   - ESCALATION_PATH.md (80 lines) - who to ask when

2. DELETE:
   - INDEX_COMPLETE_PACKAGE.md (index files are bloat)
   - CREATION_COMPLETE.md (status files, not processes)
   - PROCESS_ASSISTANT_IMPLEMENTATION_SUMMARY.md (retrospective, not process)
   - QUICK_REFERENCE.md (move to README)
   - All AGENT_COORDINATION/ files (consolidate to ESCALATION_PATH.md)
   - All CORE_WORKFLOWS/ files except sprint/code review (consolidate)
   - All GOVERNANCE/ files except GOVERNANCE_RULES.md (consolidate)
   - All TEMPLATES/ files (not needed)

**Result**: 4 core processes × 100 lines = 400 lines

### Phase 4: Collaboration System (/collaborate/) - 1,972 lines

**Target**: 200-300 lines

**Actions**:
1. Keep ONLY:
   - README.md (main index, 100 lines)
   - PLAIN_COMMUNICATION_RULE.md (bind rule for @team-assistant, 150 lines)
   
2. DELETE (20 files):
   - AGENT_MAILBOX_DOCUMENTATION_INDEX.md
   - AGENT_MAILBOX_QUICK_REFERENCE.md
   - COLLABORATION_MAILBOX_SYSTEM.md
   - COMMUNICATION_SYSTEMS_ARCHITECTURE.md
   - IMPLEMENTATION_VERIFICATION_CHECKLIST.md
   - MAILBOX_CLARITY_REVIEW.md
   - MAILBOX_SYSTEM_REVIEW_COMPLETE.md
   - MONITOR_MANAGEMENT_GUIDE.md
   - MONITOR_TEST_RESULTS.md
   - NO_EXTRA_DOCUMENTATION_RULE.md
   - PLAIN_COMMUNICATION_RULE_DOCUMENTATION_INDEX.md
   - PLAIN_COMMUNICATION_RULE_IMPLEMENTED.md
   - TEAM_ASSISTANT_COMMUNICATION_RULE.md
   - TEAM_ASSISTANT_CONTEXT_SWITCHING.md
   - TEAM_ASSISTANT_QUICK_START.md
   - REVIEW_SUMMARY_FINAL.md
   - SYSTEM_INDEX.md
   - SYSTEM_OPERATIONAL.md
   - SYSTEM_READY_FOR_PRODUCTION.md
   - All archive/*.md files

3. KEEP structure:
   - `/collaborate/issue/` (for agent coordination)
   - `/collaborate/sprint/` (for sprint docs)
   - `/collaborate/lessons-learned/` (for retros)

**Result**: 250 lines

---

## 💡 Anti-Bloat Rules (Future Prevention)

### Rule 1: One Document Per Concept
- ❌ Don't create: "MAILBOX_SYSTEM.md", "MAILBOX_DOCUMENTATION_INDEX.md", "MAILBOX_CLARIFICATION.md"
- ✅ Do: Single "MAILBOX_SYSTEM.md" (150 lines max)

### Rule 2: Index Files Are Bloat
- ❌ Don't create: "-INDEX", "-DOCUMENTATION_INDEX", "-PACKAGE"
- ✅ Do: Sections in README.md with table of contents

### Rule 3: Status Documents Don't Stay
- ❌ Don't commit: "-COMPLETE.md", "-READY.md", "-OPERATIONAL.md", "-SUMMARY.md"
- ✅ Do: Mark progress in GitHub issues, not new files

### Rule 4: No Duplicate Explanations
- ❌ Don't: Explain process in 5 different files
- ✅ Do: Single explanation in one file, link from others

### Rule 5: Process Docs < 150 Lines
- ❌ If process doc > 200 lines, it's bloated
- ✅ If > 150 lines: break into sub-processes (separate files, each <150 lines)

### Rule 6: Agent Instructions < 500 Lines
- ❌ If agent instructions > 600 lines, trim it
- ✅ If > 500 lines: move implementation patterns to role-specific quick-start

### Rule 7: No Retrospective Clutter
- ❌ Don't create new docs for "what we learned"
- ✅ Do: 1 file per sprint in `/collaborate/sprint/{N}/retrospective/RETRO_{DATE}.md`

### Rule 8: Quick References Stay Short
- ❌ Don't: 50-line quick reference (it's not quick)
- ✅ Do: 10-20 lines max, or it becomes its own doc with longer name

---

## 📋 Implementation Steps

### Step 1: Delete Bloat (30 min)
```bash
# Delete redundant agent files
rm .github/agents/backend-admin.agent.md
rm .github/agents/backend-store.agent.md
rm .github/agents/frontend-admin.agent.md
rm .github/agents/frontend-store.agent.md
rm .github/agents/qa-frontend.agent.md
rm .github/agents/qa-pentesting.agent.md
rm .github/agents/qa-performance.agent.md

# Delete redundant copilot instructions
rm .github/copilot-instructions-quickstart.md
rm .github/copilot-instructions-index.md
rm .github/copilot-instructions-refactored.md
rm .github/copilot-instructions-qa.md
rm .github/copilot-instructions-scrum-master.md
rm .github/copilot-instructions-product-owner.md
rm .github/copilot-instructions-ai-specialist.md
rm .github/copilot-instructions-ui-expert.md
rm .github/copilot-instructions-ux-expert.md
rm .github/copilot-instructions-qa-reviewer.md
rm .github/copilot-instructions-software-architect.md

# Move stakeholder agents to archive
mkdir -p .github/agents/archived
mv .github/agents/stakeholder-*.agent.md .github/agents/archived/

# Delete process bloat
rm .github/docs/processes/INDEX_COMPLETE_PACKAGE.md
rm .github/docs/processes/CREATION_COMPLETE.md
rm .github/docs/processes/QUICK_REFERENCE.md
rm .github/docs/processes/PROCESS_ASSISTANT_IMPLEMENTATION_SUMMARY.md

# Delete collaboration bloat (keep issue/, sprint/, lessons-learned/)
rm collaborate/AGENT_MAILBOX_DOCUMENTATION_INDEX.md
rm collaborate/AGENT_MAILBOX_QUICK_REFERENCE.md
rm collaborate/COLLABORATION_MAILBOX_SYSTEM.md
rm collaborate/COMMUNICATION_SYSTEMS_ARCHITECTURE.md
rm collaborate/IMPLEMENTATION_VERIFICATION_CHECKLIST.md
rm collaborate/MAILBOX_CLARITY_REVIEW.md
rm collaborate/MAILBOX_SYSTEM_REVIEW_COMPLETE.md
rm collaborate/MONITOR_MANAGEMENT_GUIDE.md
rm collaborate/MONITOR_TEST_RESULTS.md
rm collaborate/NO_EXTRA_DOCUMENTATION_RULE.md
rm collaborate/PLAIN_COMMUNICATION_RULE_DOCUMENTATION_INDEX.md
rm collaborate/PLAIN_COMMUNICATION_RULE_IMPLEMENTED.md
rm collaborate/TEAM_ASSISTANT_COMMUNICATION_RULE.md
rm collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md
rm collaborate/TEAM_ASSISTANT_QUICK_START.md
rm collaborate/REVIEW_SUMMARY_FINAL.md
rm collaborate/SYSTEM_INDEX.md
rm collaborate/SYSTEM_OPERATIONAL.md
rm collaborate/SYSTEM_READY_FOR_PRODUCTION.md

# Clear archive folder
rm -rf collaborate/archive/
rm -rf .monitor-logs/
```

### Step 2: Trim Core Agent Files (1 hour)
- Each agent: remove >500 lines of narrative, keep essentials
- Each agent: max 3-5 code examples
- Each agent: 1 escalation path only

### Step 3: Trim Core Instructions (30 min)
- Keep only backend, frontend, devops, security
- Consolidate role-specific content into agent files
- Remove role-specific instruction files

### Step 4: Consolidate Processes (30 min)
- Merge AGENT_COORDINATION into ESCALATION_PATH.md
- Merge CORE_WORKFLOWS into SPRINT_EXECUTION.md
- Delete all TEMPLATES/ and GOVERNANCE/ redundancy

### Step 5: Update Collaboration README (15 min)
- Explain system in 100 lines
- Link to PLAIN_COMMUNICATION_RULE.md
- Reference issue/, sprint/, lessons-learned/ folders

### Step 6: Add Anti-Bloat Rules (10 min)
- Create `.github/DOCUMENTATION_STANDARDS.md`
- Document the 8 anti-bloat rules
- Make it a quick-reference (max 80 lines)

---

## 📊 Expected Results

**Before Cleanup**:
- 37 agent files: 10,357 lines
- 16 copilot instruction files: 6,178 lines
- 20+ process files: 4,925 lines
- 25+ collaborate files: 1,972 lines
- **Total: 23,432 lines**

**After Cleanup**:
- 20 core agent files: 4,500 lines
- 5 copilot instruction files: 1,500 lines
- 4 core process files: 400 lines
- 2 core collaborate files: 250 lines
- **Total: 6,650 lines (71% reduction)**

**Quality Improvements**:
- ✅ Easier to find information (less noise)
- ✅ Easier to keep consistent (single source per concept)
- ✅ Easier to maintain (fewer files to update)
- ✅ Clear rules prevent future bloat

---

## 🔐 Authority & Enforcement

**@process-assistant** enforces anti-bloat rules:
1. No new documentation files without @process-assistant approval
2. All process files subject to 150-line limit
3. All agent files subject to 500-line limit
4. Quarterly cleanup of any documentation drift

---

**Status**: Ready to execute cleanup  
**Estimated Time**: 2-3 hours  
**Impact**: 71% reduction in configuration bloat, improved maintainability
