# Agent Optimization & Best Practices

**Version**: 1.0  
**Last Updated**: 28. Dezember 2025  
**Purpose**: Guidelines for creating and maintaining agents

---

## ✅ Agent Template Structure

Every agent should follow this structure for consistency:

```markdown
---
description: 'Brief description of agent role and responsibilities'
tools: ['workspace', 'fileSearch', 'documentation']
trigger: 'How/when this agent is activated'
---

You are a [Role] with expertise in:
- **Area 1**: Description
- **Area 2**: Description
- **Area 3**: Description

Your responsibilities:
1. Primary responsibility
2. Secondary responsibility
3. Tertiary responsibility

---

## 📋 [Section Title]

[Content]

---

## 🎯 [Section Title]

[Content]

---

## ✅ Definition of Done

Before marking work complete:
- [ ] Checklist item 1
- [ ] Checklist item 2

---

**Last Updated**: [Date]
**Author**: [Your name]
**Version**: 1.0
```

---

## 🎯 Agent Naming Convention

| Category | Prefix | Examples |
|----------|--------|----------|
| Core Development | (none) | `backend-developer.agent.md`, `frontend-developer.agent.md` |
| QA/Testing | `qa-` | `qa-engineer.agent.md`, `qa-frontend.agent.md`, `qa-pentesting.agent.md` |
| Specialist Tech | (specific) | `security-engineer.agent.md`, `devops-engineer.agent.md` |
| Experts | `[skill]-expert` | `ui-expert.agent.md`, `ux-expert.agent.md` |
| Specialists | `[area]-[role]` | `backend-store.agent.md`, `frontend-admin.agent.md` |
| Stakeholders | `stakeholder-` | `stakeholder-erp.agent.md`, `stakeholder-crm.agent.md` |
| Support | `support-` | `support-triage.agent.md` |

---

## 📚 Agent Content Guidelines

### What to Include

✅ **Expertise Areas**: 3-5 key areas of responsibility  
✅ **Key Technologies**: List tech stack relevant to role  
✅ **Responsibilities**: 3-7 clear actionable responsibilities  
✅ **Decision Trees**: Visual flowcharts for complex decisions  
✅ **Code Examples**: Working code patterns with ✅ and ❌  
✅ **Templates**: Response templates, checklists, scripts  
✅ **Quick Reference**: Lookup tables for common scenarios  
✅ **Definition of Done**: Checkpoints before marking complete  
✅ **Escalation Paths**: Who to contact for different issues  

### What to Avoid

❌ **Duplication**: Don't repeat copilot-instructions.md rules  
❌ **Off-Topic**: Stay focused on agent's specific role  
❌ **Outdated**: Update references regularly  
❌ **Too Long**: Keep focused; link to detailed docs elsewhere  
❌ **Vague Instructions**: Be specific with examples  

### Optimal Length

- **Small Agents** (specialists): 30-50 lines
- **Medium Agents** (developer roles): 100-150 lines
- **Large Agents** (complex roles): 200-400 lines

---

## 🔄 Agent Categories & Hierarchy

### Core Development (Must-Have)
- `backend-developer.agent.md` - Services, APIs, databases
- `frontend-developer.agent.md` - Vue.js components, UX
- `qa-engineer.agent.md` - Testing, quality verification
- `tech-lead.agent.md` - Architecture, code review, decisions

### Specialization (Role-Specific)
- `backend-store.agent.md` - Store API specifics
- `backend-admin.agent.md` - Admin API specifics
- `frontend-store.agent.md` - Store UI specifics
- `frontend-admin.agent.md` - Admin UI specifics

### Engineering Expertise
- `devops-engineer.agent.md` - Infrastructure, deployment
- `security-engineer.agent.md` - Security, encryption, compliance
- `ai-specialist.agent.md` - AI/ML patterns
- `ui-expert.agent.md` - Design systems, UI components
- `ux-expert.agent.md` - User experience, accessibility

### Quality Assurance (Specialized)
- `qa-frontend.agent.md` - Frontend testing specifics
- `qa-performance.agent.md` - Performance, load testing
- `qa-pentesting.agent.md` - Security testing, penetration

### Stakeholders (External Partners)
- `stakeholder-erp.agent.md` - ERP integration context
- `stakeholder-pim.agent.md` - PIM integration context
- `stakeholder-crm.agent.md` - CRM integration context
- `stakeholder-bi.agent.md` - BI/Analytics context
- `stakeholder-reseller.agent.md` - Reseller partner context

### Leadership & Management
- `product-owner.agent.md` - Prioritization, roadmap
- `legal-compliance.agent.md` - Regulatory, legal context

### Support
- `support-triage.agent.md` - GitHub issue triage, classification

---

## 🎯 Best Practices for Each Agent Type

### Developer Agents (backend-developer, frontend-developer)

**Must Include**:
- ✅ Technology stack details
- ✅ Project structure overview
- ✅ Code patterns with examples
- ✅ Testing requirements (80%+ coverage)
- ✅ Common pitfalls (❌ examples)
- ✅ Quick command reference
- ✅ Troubleshooting guide

**Should NOT Include**:
- ❌ Compliance rules (reference copilot-instructions.md)
- ❌ Role definitions (use docs/)
- ❌ Team structure (reference .github/TEAM_MEMBERS.md)

---

### QA Agents (qa-engineer, qa-frontend, qa-performance, qa-pentesting)

**Must Include**:
- ✅ Test types to focus on
- ✅ Tools and frameworks
- ✅ Test scenarios (happy path, edge cases)
- ✅ Acceptance criteria
- ✅ Test templates/examples
- ✅ Automation patterns
- ✅ Defect reporting template

**Should NOT Include**:
- ❌ Implementation details (link to backend agent)
- ❌ Architecture (link to tech-lead agent)

---

### Specialist Agents (security-engineer, devops-engineer)

**Must Include**:
- ✅ Key responsibilities
- ✅ Tools/technologies
- ✅ Checklists for common tasks
- ✅ Escalation procedures
- ✅ Emergency procedures (if applicable)
- ✅ Documentation links
- ✅ Configuration examples

**Should NOT Include**:
- ❌ Implementation code (link to developers)
- ❌ Broad architecture (already in copilot-instructions.md)

---

### Expert/Specialist Agents (ui-expert, ux-expert, ai-specialist)

**Must Include**:
- ✅ Expertise areas (3-5)
- ✅ Best practices/guidelines
- ✅ Anti-patterns (❌ examples)
- ✅ Tool recommendations
- ✅ Common questions/answers
- ✅ Reference to industry standards

---

### Stakeholder Agents (erp, pim, crm, bi, reseller)

**Must Include**:
- ✅ Integration points
- ✅ Data formats/schemas
- ✅ API endpoints
- ✅ Common integration challenges
- ✅ Success criteria
- ✅ Contact/escalation path

---

## 🔗 Cross-Agent References

When one agent needs to reference another:

```markdown
**For security questions**: See [Security Engineer agent](./security-engineer.agent.md)
**For testing**: See [QA Engineer agent](./qa-engineer.agent.md)
**For architecture**: See [Tech Lead agent](./tech-lead.agent.md)
**For compliance**: See [Legal/Compliance agent](./legal-compliance.agent.md)
```

**Rule**: Link, don't duplicate. If content exists elsewhere, reference it.

---

## 📊 Agent Maintenance

### Update Frequency

| Type | Frequency | Trigger |
|------|-----------|---------|
| Core Agents | Weekly | Architecture changes, new patterns |
| Specialist Agents | Monthly | Technology updates, process changes |
| Stakeholder Agents | Quarterly | Integration changes, requirements |
| Support Agents | As-needed | Policy changes, new procedures |

### Version Management

```markdown
---
version: '1.0'
lastUpdated: '28. Dezember 2025'
author: 'Team'
---
```

**When to bump version**:
- `1.0 → 1.1`: Minor updates (typos, clarifications)
- `1.0 → 2.0`: Major changes (new patterns, responsibilities)

---

## ✅ Agent Quality Checklist

Before considering an agent complete:

**Content Quality**:
- [ ] Clear, concise language
- [ ] No grammatical errors
- [ ] Consistent formatting
- [ ] Working code examples (tested)
- [ ] Links to relevant documentation

**Completeness**:
- [ ] All major responsibilities covered
- [ ] Decision trees for complex scenarios
- [ ] Templates/checklists provided
- [ ] Quick reference table included
- [ ] Definition of done specified

**Navigation**:
- [ ] Clear section headers
- [ ] Table of contents (for long agents)
- [ ] Links to other agents when relevant
- [ ] References to external documentation
- [ ] Author and last update date

**Usability**:
- [ ] Quick start section (first 10 lines)
- [ ] Code examples with ✅ and ❌
- [ ] Common pitfalls identified
- [ ] Troubleshooting guide
- [ ] Escalation procedures

---

## 🚀 Creating a New Agent

1. **Choose Name** (follow naming convention)
   ```
   [category]-[role].agent.md
   Example: qa-accessibility.agent.md
   ```

2. **Use Template**
   ```markdown
   ---
   description: 'Clear description'
   tools: ['relevant', 'tools']
   trigger: 'When/how activated'
   ---
   
   You are a [Role]...
   ```

3. **Write Content** (follow guidelines above)

4. **Add to Registry** (AGENTS_REGISTRY.md)
   ```markdown
   ### [Your Agent Name]
   **File**: `agents/[your-agent].agent.md`
   **Focus**: [What it does]
   **Key Tech**: [Relevant tech stack]
   ```

5. **Link from Index** (copilot-instructions-refactored.md)
   ```markdown
   | [Your Role] | `[your-agent].agent.md` | [→ Go](./agents/[your-agent].agent.md) |
   ```

6. **Test & Validate**
   - [ ] All links work
   - [ ] Code examples are correct
   - [ ] No broken references
   - [ ] Consistent with other agents

---

## 📋 Agent Registry Location

**File**: `.github/AGENTS_REGISTRY.md`

This is the single source of truth for all agents. Update it whenever creating, updating, or deprecating agents.

---

## 🔍 Finding the Right Agent

**Flow**:
1. Start: `copilot-instructions-refactored.md` (Quick reference table)
2. Find your role → Click agent link
3. If specialized: Check "Specialist Roles" section
4. If problem-specific: Check "Support/Triage" agent
5. Still stuck? Reference `AGENTS_REGISTRY.md`

---

**Version**: 1.0  
**Last Updated**: 28. Dezember 2025  
**Maintainers**: Architecture team
