# Role-Specific Instructions Update Complete

**Date**: 29. Dezember 2025  
**Focus**: Embedding Software Architect and CLI Developer agents into all role-specific guidance

---

## 📋 Summary

All 5 role-specific copilot instruction files have been updated to reference the new agent hierarchy and escalation paths. Every developer consulting their role-specific instructions now understands:

1. **Which agent to consult** for their role (@backend-developer, @frontend-developer, etc.)
2. **Clear escalation paths** (Tech Lead for complex problems, Software Architect for architecture)
3. **When to involve specialists** (CLI Developer for automation, Security Engineer for compliance)

---

## ✅ Completed Updates

### 1. **copilot-instructions-backend.md**
- **Header Updated**: Added `**Agent**: @backend-developer` and escalation path
- **Escalation Path Added**: 🚀 Escalation Path section with clear guidance
  - Complex implementation → @tech-lead
  - Service design questions → @tech-lead (may escalate to @software-architect)
  - Architectural decisions → @software-architect
  - DevOps/Operations → @cli-developer

**Before**:
```
**Focus**: Wolverine services, onion architecture, database design  
**For full reference**: [copilot-instructions.md](./copilot-instructions.md)
```

**After**:
```
**Focus**: Wolverine services, onion architecture, database design  
**Agent**: @backend-developer (or specialized: @backend-admin, @backend-store)  
**Escalation**: Complex problems → @tech-lead | System structure → @software-architect  
**For full reference**: [copilot-instructions.md](./copilot-instructions.md)

---

## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Complex implementation**: Ask @tech-lead for code patterns
- **Service design question**: Ask @tech-lead, they may escalate to @software-architect
- **Architectural decision**: Ask @software-architect directly (e.g., should search be separate service?)
- **DevOps/Operations**: Ask @cli-developer if it should be a CLI command
- **Performance issue**: Ask @tech-lead, may escalate to @software-architect
```

---

### 2. **copilot-instructions-frontend.md**
- **Header Updated**: Added `**Agent**: @frontend-developer` and escalation path  
- **Escalation Path Added**: Clear guidance on escalating complex problems

**New Escalation Paths**:
```
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Complex component architecture**: Ask @tech-lead for design patterns
- **State management challenges**: Ask @tech-lead or @software-architect
- **API contract design**: Ask @tech-lead, may escalate to @software-architect
- **Multi-service integration**: Ask @software-architect directly
- **Bulk operations**: Coordinate with @cli-developer for CLI alternative
- **Accessibility concerns**: Ask @tech-lead, verify with @software-architect if architectural
```

---

### 3. **copilot-instructions-devops.md**
- **Header Updated**: Added `**Agent**: @devops-engineer` and escalation path
- **Escalation Path Added**: Clear guidance on infrastructure and deployment questions

**New Escalation Paths**:
```
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Infrastructure architecture**: Ask @software-architect for infrastructure design
- **CI/CD pipeline changes**: Ask @software-architect for deployment patterns
- **Service deployment patterns**: Ask @software-architect for orchestration strategy
- **CLI automation**: Work with @cli-developer for DevOps automation scripts
```

---

### 4. **copilot-instructions-qa.md**
- **Header Updated**: Added `**Agent**: @qa-engineer` and escalation path
- **Escalation Path Added**: Clear guidance on test architecture and compliance

**New Escalation Paths**:
```
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Complex test architecture**: Ask @software-architect for multi-service test strategies
- **CLI testing**: Work with @cli-developer to ensure CLI commands are properly tested
- **Performance testing needs**: Ask @software-architect for system bottleneck analysis
- **Security test strategy**: Coordinate with @security-engineer for compliance testing
```

---

### 5. **copilot-instructions-security.md**
- **Header Updated**: Added `**Agent**: @security-engineer` and escalation path
- **Escalation Path Added**: Clear guidance on architecture and compliance coordination

**New Escalation Paths**:
```
## 🚀 Escalation Path

**Problem?** → Ask your agent
- **Architectural security decisions**: Ask @software-architect for system-wide security design
- **Compliance strategy changes**: Ask @tech-lead for P0 requirements validation
- **Key management infrastructure**: Ask @devops-engineer for vault setup and rotation
- **Security test coverage**: Work with @qa-engineer for penetration test planning
```

---

## 🎯 Impact

Every developer in B2Connect now has clear guidance on:

1. **Which agent to consult** for their role
2. **When escalation is needed** (complexity, architecture, compliance)
3. **Who to escalate to** (Tech Lead, Software Architect, CLI Developer, Security Engineer)
4. **What types of questions** warrant escalation

### Agent Hierarchy Embedded in Instructions

```
Developer (Role-Specific) 
    ↓ (for complex problems)
Tech Lead (@tech-lead - Claude Sonnet 4.5)
    ↓ (for architecture/system-wide decisions)
Software Architect (@software-architect - Claude Sonnet 4.5)
    ↓ (for infrastructure decisions)
DevOps Engineer (@devops-engineer)
    ↓ (for CLI/automation)
CLI Developer (@cli-developer)
```

---

## 📊 Files Modified

**Role-Specific Instructions** (5 files):
- ✅ `.github/copilot-instructions-backend.md` - Updated header + escalation path
- ✅ `.github/copilot-instructions-frontend.md` - Updated header + escalation path
- ✅ `.github/copilot-instructions-devops.md` - Updated header + escalation path
- ✅ `.github/copilot-instructions-qa.md` - Updated header + escalation path
- ✅ `.github/copilot-instructions-security.md` - Updated header + escalation path

**No Changes Required**:
- `.github/copilot-instructions.md` - Main reference (already comprehensive)
- `.github/copilot-instructions-quickstart.md` - Already references agent hierarchy

---

## 🔄 Integration with Previous Work

This update **completes** the agent ecosystem integration started in previous sessions:

1. ✅ Created @software-architect agent (source of truth on system architecture)
2. ✅ Created @cli-developer agent (DevOps automation tool development)
3. ✅ Updated 15+ agent definitions to reference hierarchy
4. ✅ Created 4 comprehensive integration documentation files
5. ✅ **NOW**: Updated 5 role-specific instruction files to embed agents throughout

---

## ✨ Next Steps (Optional)

If you want to take further action:

1. **Review the changes**: `git diff .github/copilot-instructions-*.md`
2. **Commit these updates**: Group with agent integration work
3. **Communicate to team**: Mention new escalation paths in standup
4. **Test with developers**: Ask team members to verify their role instructions are clear

---

## 📚 Reference

For full context on the agent system:
- [AGENT_QUICK_START.md](./.github/AGENT_QUICK_START.md) - Quick overview
- [AGENT_WORKFLOW_INTEGRATION.md](./.github/AGENT_WORKFLOW_INTEGRATION.md) - Detailed integration
- [AGENT_INTEGRATION_SUMMARY.md](./.github/AGENT_INTEGRATION_SUMMARY.md) - What was created
- [AGENT_DOCUMENTATION_INDEX.md](./.github/AGENT_DOCUMENTATION_INDEX.md) - Master index

---

**Status**: ✅ Complete | All 5 role-specific instructions updated with agent references and escalation paths
