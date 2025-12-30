# 📋 Scrum Master - Sprint Documentation Update

**Date**: 30. Dezember 2025  
**Agent Updated**: @scrum-master  
**Section Added**: 3. Sprint Documentation Management  
**Authority**: @process-assistant (instruction update)

---

## What Was Added

New comprehensive section on **Sprint Documentation Management** to the scrum-master agent instructions.

### Location in File
- **File**: `.github/agents/scrum-master.agent.md`
- **Section Number**: 3 (after Team Coordination & Scheduling)
- **Lines Added**: ~260 lines
- **Total New Instructions**: Detailed guidance on managing sprint documentation

---

## Key Instructions Added

### �� Documentation Structure

Sprint documentation is organized at `B2Connect/collaborate/` with these folders:

```
collaborate/
├── sprint/{sprint-number}/          # Sprint-specific planning, execution, retro
├── pr/{pr-number}/                  # PR-specific design, implementation, feedback
├── lessons-learned/                 # Consolidated learnings from sprints
├── request-to/{agent}/              # Requests logged by agent name
└── agreements/                       # Team norms and process agreements
```

### 👥 Role Assignments

**@scrum-master**: 
- Coordinates documentation in `collaborate/sprint/{sprint-number}/`
- Consolidates outputs after PR merge
- Archives and cleans up folders

**@product-owner** → advises **@team-assistant**:
- Updates GitHub issues based on sprint progress
- Links issues to consolidated documentation
- Closes resolved items and notes blockers

**@tech-lead**:
- Documents architecture decisions in `pr/{pr-number}/design-decisions/`

**All agents**:
- Contribute to `lessons-learned/` throughout sprint

### 🔄 Sprint Workflow

```
Sprint Execution → PR Created → PR Merged → Consolidation → Next Sprint
     ↓              ↓             ↓              ↓
 Document       Design &      Archive &      Clean &
 planning,      implement     consolidate    organize
 execution,     document
 lessons
```

### 📋 Consolidation Process (After Each PR)

**@scrum-master** does:
- [ ] Move sprint artifacts to `lessons-learned/{sprint-number}/`
- [ ] Create consolidated summary: `consolidated-{sprint-number}.md`
- [ ] Archive PR folder: `pr/{pr-number}/` → mark as archived
- [ ] Remove duplicate entries from `request-to/`
- [ ] Update `collaborate/README.md`
- [ ] Verify agreements reflect current standards

**@product-owner** advises **@team-assistant** to:
- [ ] Update all GitHub issues with consolidation status
- [ ] Link issues to documentation
- [ ] Close resolved issues
- [ ] Create new issues for identified blockers
- [ ] Note lessons-learned in issue comments

### 📂 Documentation Types

| Type | Location | Created | Purpose |
|------|----------|---------|---------|
| **Lessons Learned** | `lessons-learned/{date}-{topic}.md` | Throughout sprint | What we learned, validated, metrics |
| **Requests to Agents** | `request-to/{agent}/{date}-request-{id}.md` | During sprint | Requests logged with context/deadline |
| **Agreements** | `agreements/{standard}.md` | After retrospectives | Team norms, coding standards |
| **Sprint Docs** | `sprint/{sprint-number}/{folder}/` | Start-end of sprint | Planning, execution, retrospective notes |
| **PR Docs** | `pr/{pr-number}/{folder}/` | During development | Design decisions, implementation, feedback |

### 🎯 Key Principles

1. **Centralization**: All sprint coordination in single `collaborate/` folder
2. **Parallelization**: Multiple folders enable agents to work simultaneously
3. **Consolidation**: Regular cleanup keeps structure clean and manageable
4. **Traceability**: GitHub issues linked to documentation
5. **Accessibility**: Clear folder structure enables discovery
6. **Automation**: Product Owner coordinates GitHub updates

---

## How This Improves Processes

### Before
- Sprint documentation scattered across multiple locations
- No clear consolidation process
- Orphaned documents after sprints
- Difficulty finding historical references
- Manual GitHub issue updates

### After
- ✅ Centralized documentation at `collaborate/`
- ✅ Clear consolidation workflow after each PR
- ✅ Automatic archival and cleanup
- ✅ Structured lessons-learned repository
- ✅ Product owner advises team-assistant for GitHub updates
- ✅ Each agent knows their documentation responsibilities
- ✅ Documentation remains discoverable and organized

---

## Implementation Timeline

**Immediate**:
1. Create `B2Connect/collaborate/` folder structure
2. Create first sprint folder: `collaborate/sprint/1/`
3. Create first PR documentation: `collaborate/pr/1/`

**Per Sprint**:
1. At sprint start: Create `sprint/{sprint-number}/` folders
2. During development: Document in respective folders
3. After PR merge: Run consolidation checklist
4. Archive and clean up for next sprint

**Ongoing**:
- @scrum-master coordinates documentation
- Consolidation after each PR (not accumulated)
- Agreements updated as team standards evolve

---

## Example Consolidation Message

After PR merge, @scrum-master posts to GitHub:

```markdown
## Sprint X Consolidation Complete ✅

Consolidated documentation for Sprint X is now available in `collaborate/lessons-learned/consolidated-{sprint-number}.md`

**Archive**:
- Sprint planning: `collaborate/sprint/{sprint-number}/` → archived
- PR documentation: `pr/{pr-number}/` → archived
- Lessons learned: Consolidated into single index

**Updated Agreements**:
- [List any new standards established]

**Next Steps**:
- Use consolidated docs as reference for Sprint X+1
- @product-owner will update all related GitHub issues
- Link to: `collaborate/lessons-learned/consolidated-{sprint-number}.md`

See `collaborate/README.md` for full index.
```

---

## Impact on Other Agents

**No changes required for**:
- @backend-developer, @frontend-developer, @qa-engineer, etc.
- They document their work; @scrum-master coordinates

**Coordination needed with**:
- **@product-owner**: Update GitHub issues (advises @team-assistant)
- **@tech-lead**: Document architecture decisions
- **@team-assistant**: Execute GitHub issue updates

---

## Related Sections in Scrum Master

- **Section 1**: Retrospectives & Continuous Improvement
- **Section 2**: Team Coordination & Scheduling
- **Section 3**: **Sprint Documentation Management** ← NEW
- **Section 4**: Efficient Processes & Continuous Progress
- **Section 5**: Disagreement Resolution & Consensus Building
- **Section 6**: Process Improvement Requests

---

## File Reference

**Modified File**: `.github/agents/scrum-master.agent.md`  
**Section**: 3. Sprint Documentation Management  
**Lines**: Added ~260 lines with comprehensive guidance  
**Subsections**:
- Documentation Structure
- Responsibilities (During Sprint, During PR Review, Consolidation)
- Documentation Types
- Workflow: From Sprint to Consolidation
- Consolidation Checklist
- Key Principles

---

**Authority**: @process-assistant (exclusive over agent instructions)  
**Effective**: 30. Dezember 2025  
**Version**: Updated with comprehensive sprint documentation workflow

