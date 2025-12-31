# Dependency Upgrade Research Workflow

**Version:** 1.0  
**Created:** 30.12.2025  
**Maintained by:** @SARAH, @Architect

## Overview

Structured process to research, document, and distribute information about software dependency upgrades to the entire team.

## When to Use

- New major/minor version of critical dependency available
- Security update needs documentation
- Breaking changes need to be communicated to team
- Need centralized reference for version comparison

## Workflow Steps

### Step 1: Initiate Research (User)
```
Command: @Architect "Recherchiere {Software} von {old-version} zu {new-version}"

Example:
@Architect "Recherchiere React von v17 zu v18"
@Architect "Recherchiere PostgreSQL 15 → 16"
@Architect "Recherchiere Node.js v18 → v20"
```

**User provides:**
- Software name
- Current version (known to LLM)
- Target version
- Any specific areas of concern

### Step 2: Internet Research (Agent)
```
Tools: fetch_webpage, semantic_search

Research targets:
  ✓ Official Changelog/Release Notes
  ✓ Migration Guide
  ✓ Breaking Changes Documentation
  ✓ New Features & Capabilities
  ✓ Security Announcements
  ✓ Performance Improvements
  ✓ Deprecation Notices
  ✓ Known Issues/Bugs
```

**Output:** Raw research data (not yet optimized)

### Step 3: Structured Analysis (Agent)

**Categorize findings:**
```
1. Breaking Changes (Critical)
   - What changed in API/architecture
   - Why it changed
   - How to migrate
   
2. New Features (Important)
   - New capabilities
   - When to use them
   - Related breaking changes
   
3. Security Updates (Critical)
   - CVEs fixed
   - Security improvements
   - Compliance impact
   
4. Performance Improvements (Medium)
   - Speed gains
   - Memory improvements
   - Scalability changes
   
5. Deprecations & Removals (Medium)
   - What's deprecated
   - Replacement patterns
   - Timeline

6. Migration Path (Critical)
   - Step-by-step guide
   - Testing strategy
   - Rollback plan
```

### Step 4: Token Optimization

**Apply condensing rules:**
```
❌ DON'T:
- Copy entire official docs
- Write prose when bullets work
- Include outdated information
- Duplicate content across sections

✓ DO:
- Use bullet lists
- Create comparison tables
- Link to official docs
- Keep descriptions <50 words
- Use clear headings
- Show code only for critical changes
```

**Target file size:** <2 KB per version summary (can be 5-10 KB with full details)

### Step 5: Create Summary Document

**Location:** `.ai/knowledgebase/software/{software-name}/{old-version}--to--{new-version}.md`

**Template:**
```markdown
# {Software} {old-version} → {new-version}

## Quick Summary
[1-2 sentence overview of critical changes]

## Breaking Changes
| Change | Old | New | Migration |
|--------|-----|-----|-----------|
| ... | ... | ... | ... |

## New Features
- Feature 1: [Description]
- Feature 2: [Description]

## Security Fixes
- CVE-XXXX: [Description]
- Update recommendation: [Priority]

## Performance Changes
| Metric | Improvement | Details |
|--------|-------------|---------|
| ... | ... | ... |

## Deprecations & Removals
- Item 1: Deprecated in vX, removed in vY
- Item 2: Migration path: [Link]

## Migration Checklist
- [ ] Review breaking changes
- [ ] Update code references
- [ ] Run test suite
- [ ] Update dependencies
- [ ] Test in staging
- [ ] Deploy to production

## Agent-Specific Notes

### @Architect
System design impact, integration patterns, scalability considerations.

### @Backend
API changes, data model changes, database compatibility.

### @Frontend
Component library updates, breaking UI changes, state management.

### @Security
CVE fixes, authentication changes, compliance implications.

### @DevOps
Deployment considerations, dependency changes, infrastructure.

### @TechLead
Code quality impacts, best practices, technical debt.

## Related Documentation
- [Official Changelog](link)
- [Migration Guide](link)
- [Issue for implementation](link to .ai/issues/)

## Verification
- Document created: [Date]
- Reviewed by: @Architect
- Validated by: @SARAH
- Last updated: [Date]
```

### Step 6: Update Knowledgebase Index

**File:** `.ai/knowledgebase/INDEX.md`

**Update:**
1. Add software to "Software Versions Inventory" table
2. Add row with current/latest versions
3. Assign tags (breaking-changes, security, performance, etc.)
4. Add link to appropriate tag section
5. Update timestamp

**Example row:**
```markdown
| React | 17.x | 18.x | 30.12.2025 | breaking-changes,new-features | Hooks API improved |
```

### Step 7: Distribute to Agents

**SARAH's task:**
1. Tag all relevant agents with notification
2. Create summary for each agent's perspective
3. Update related issues/features in `.ai/issues/`
4. Link from other documents

**Notification template:**
```
@Architect: Check `.ai/knowledgebase/software/{software}/`
@Backend: Review API changes section
@Frontend: Review component changes section
@Security: Review security fixes section
@DevOps: Review deployment changes section
```

### Step 8: Integration with Issues

**Link in existing issues:**
- If `.ai/issues/*/context.md` references this dependency → Link to knowledgebase entry
- Create blocking issue if upgrade required
- Update `.ai/issues/*/progress.md` with version info

## Success Criteria

✅ **Complete when:**
- [ ] Research document created and in `.ai/knowledgebase/software/`
- [ ] All agent perspectives documented
- [ ] INDEX.md updated with tags and links
- [ ] File size optimized (<2 KB for summary, <5 KB with details)
- [ ] All agents notified
- [ ] Related issues linked
- [ ] No duplicate or redundant information

## Common Patterns

### Pattern 1: Breaking Change with Migration Path
```markdown
## Breaking Changes

| Change | Impact | Migration |
|--------|--------|-----------|
| Old API removed | High | Use new API pattern |
| Data format changed | Medium | Use migration script |

See Migration Checklist below for step-by-step guide.
```

### Pattern 2: Security Update Priority
```markdown
## Security Fixes

| CVE | Severity | Fix | Action |
|-----|----------|-----|--------|
| CVE-2024-XXXX | Critical | Upgrade required | Update immediately |
| CVE-2024-YYYY | Medium | Optional | Plan for next sprint |
```

### Pattern 3: Feature with Code Example
```markdown
## New Features

### Improved Hooks API
**What:** New useId() hook for stable identity across renders
**When:** Use for form fields, aria-labels, association
**Example:**
\`\`\`javascript
const id = useId();
\`\`\`
```

## Tools & Resources

### Fetch Tool
```
Use fetch_webpage to get:
- Release notes from official site
- Migration guides
- API documentation
```

### File Operations
```
Create: .ai/knowledgebase/software/{name}/{version}.md
Update: .ai/knowledgebase/INDEX.md
Link: .ai/issues/*/context.md (if related)
```

## Timing & Frequency

- **Security Updates:** Within 24 hours
- **Breaking Changes:** Within 1 week of release
- **Minor/Patch Updates:** As needed for team context
- **Review Cadence:** Monthly for major dependencies

## Escalation

**When to escalate:**
- Breaking changes affect core architecture → @Architect review
- Security issue has compliance impact → @Security approval
- API changes affect multiple services → @SARAH coordinates

---

**Next Step:** Use `.github/prompts/dependency-upgrade-research.prompt.md` to get detailed research instructions.
