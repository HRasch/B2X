---
agent: ProductOwner
description: Single-Agent Requirements Analysis - Rate limit optimized
---

# Single-Agent Requirements Analysis

**Rate Limit Optimized**: Sequential execution instead of parallel multi-agent analysis.

## Process Overview

Instead of triggering 4+ agents simultaneously, use **sequential workflow**:

```
@ProductOwner → Creates comprehensive spec
↓ (10 min cooldown)
Other agents → Review via documentation asynchronously
↓ (SARAH consolidation)
Final spec → Ready for implementation
```

## Analysis Framework

### 1. Business Requirements
- **User Stories**: As a [user], I want [feature] so that [benefit]
- **Acceptance Criteria**: Clear, testable conditions
- **Business Value**: Priority and impact assessment
- **Stakeholder Impact**: Who benefits, who is affected

### 2. Technical Requirements
- **Functional Requirements**: What the system must do
- **Non-Functional Requirements**: Performance, security, scalability
- **Integration Points**: External systems, APIs, data sources
- **Constraints**: Technical limitations, compliance requirements

### 3. Implementation Considerations
- **Architecture Impact**: New services, data models, APIs
- **UI/UX Requirements**: User interface, accessibility, responsive design
- **Security Requirements**: Authentication, authorization, data protection
- **Testing Requirements**: Unit tests, integration tests, E2E coverage

### 4. Risk Assessment
- **Technical Risks**: Complexity, dependencies, unknowns
- **Business Risks**: Timeline, budget, stakeholder impact
- **Operational Risks**: Deployment, monitoring, maintenance

## ⚡ Rate Limit Optimization

### Session Structure:
1. **45 minutes**: Analysis and documentation
2. **10 minutes**: Cooldown (no Copilot usage)
3. **Repeat**: Maximum 3 sessions per requirement

### Optimization Rules:
- **Single agent focus**: Complete analysis before involving others
- **Batch documentation**: Create all required files in one session
- **Text-based reviews**: Other agents review via `.ai/` files
- **Archive promptly**: Move completed analysis to archive after 7 days

## Output Files

Create these files in `.ai/requirements/`:

```
REQ-XXX-specification.md     # Complete requirements spec
REQ-XXX-technical-analysis.md # Technical implementation details
REQ-XXX-risk-assessment.md   # Risk and mitigation analysis
REQ-XXX-acceptance-criteria.md # Detailed acceptance criteria
```

## Template

```markdown
# REQ-XXX: [Feature Title]

## Business Requirements
### User Stories
- As a [user type], I want [functionality] so that [benefit]

### Acceptance Criteria
- [ ] Criterion 1: [Measurable condition]
- [ ] Criterion 2: [Measurable condition]

## Technical Requirements
### Functional
- [ ] Feature 1: [Description]
- [ ] Feature 2: [Description]

### Non-Functional
- Performance: [Requirements]
- Security: [Requirements]
- Scalability: [Requirements]

## Implementation Plan
### Phase 1: [Timeline]
- [Task 1]
- [Task 2]

### Phase 2: [Timeline]
- [Task 3]
- [Task 4]

## Risk Assessment
| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| [Risk 1] | High | Medium | [Mitigation strategy] |

## Success Metrics
- [ ] [Measurable outcome 1]
- [ ] [Measurable outcome 2]

---
Created: [Date] | Agent: @ProductOwner
Status: Ready for technical review
```</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/prompts/requirements-analysis-single.prompt.md