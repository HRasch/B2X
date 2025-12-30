# Agent & Guideline Management Permissions

## Exclusive Authority: SARAH

Only **SARAH** has permission to:

### Agent Management
- Create new agents
- Modify existing agent definitions
- Delete agents
- Rename agents
- Update agent responsibilities and expertise areas

### Guideline Management
- Create new guidelines
- Modify existing guidelines in `.ai/guidelines/`
- Delete guidelines
- Update guidelines across categories (coding-standards, process, security, quality, architecture, team)

### Permission Management
- Grant specific permissions to other agents for defined tasks
- Revoke permissions when no longer needed
- Maintain permission audit trail
- Document permission decisions and rationale

### Approval Process
- All requests for new agents or guideline changes must go through SARAH
- SARAH evaluates requests based on project needs and consistency
- SARAH documents decisions for audit and future reference

## For Other Agents & Team Members

If you need:
- A new agent created → Request through SARAH
- A guideline changed → Request through SARAH  
- An existing agent's instructions updated → Request through SARAH

SARAH will evaluate your request and implement changes if approved.

## Files Under SARAH's Authority
- `.github/agents/*.agent.md` - All agent files
- `.ai/guidelines/**/*.md` - All guidelines
- `.ai/guidelines/**/*.yml` - All guideline configurations
- `.ai/guidelines/**/*.json` - All guideline data

## Rationale
This centralized authority ensures:
- Consistency across all agents and guidelines
- No conflicting instructions or policies
- Single source of truth for project standards
- Controlled evolution of the agent system
