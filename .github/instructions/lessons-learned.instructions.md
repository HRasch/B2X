---
applyTo: "**/*"
---

# Lessons Learned Framework

## Failure Prevention Policy
**MANDATORY**: All development activities must check for and apply relevant lessons learned before implementation.

### Pre-Implementation Requirements
- **Search Lessons Learned Database**: Query `.ai/lessons/` for similar patterns before starting work
- **Review Recent Incidents**: Check last 30 days of incidents for relevant patterns
- **Apply Prevention Measures**: Implement identified prevention strategies
- **Document Risk Assessment**: Note any identified risks and mitigation plans

### During Development
- **Pattern Recognition**: Monitor for emerging failure patterns during implementation
- **Early Detection**: Report potential issues immediately when detected
- **Knowledge Application**: Apply lessons learned from similar implementations

## Incident Response & Lessons Learned Capture

### Mandatory Incident Classification
**Severity 1 (Critical)**: System down, data loss, security breach
**Severity 2 (High)**: Major feature broken, performance degradation
**Severity 3 (Medium)**: Minor bugs, usability issues
**Severity 4 (Low)**: Code quality issues, minor improvements

### Lessons Learned Process Requirements

#### For All Incidents (Severity 1-4)
1. **Immediate Documentation**: Create incident entry within 1 hour of detection
2. **Root Cause Analysis**: Complete within 24 hours using 5-Why technique
3. **Lessons Learned Capture**: Document within 48 hours using standard template
4. **Prevention Measures**: Identify and implement fixes within 1 week

#### For Severity 1-2 Incidents
- **Cross-Team Review**: Involve affected teams within 24 hours
- **Management Notification**: Escalate to leadership within 4 hours
- **Post-Mortem Meeting**: Conduct within 72 hours with all stakeholders
- **Policy Review**: Assess if policies need updates within 1 week

### Lessons Learned Template Structure
```markdown
# Incident: [Descriptive Title]

## Context
- **Date/Time**: [When it occurred]
- **Severity**: [1-4 scale]
- **Affected Systems**: [List components/services]
- **Impact**: [Users affected, business impact]

## Root Cause Analysis
- **Primary Cause**: [What caused the issue]
- **Contributing Factors**: [What made it worse]
- **5-Why Analysis**: [Step-by-step root cause]

## Resolution
- **Immediate Fix**: [What was done to resolve]
- **Long-term Fix**: [Prevention measures implemented]
- **Verification**: [How fix was tested]

## Lessons Learned
- **What Went Wrong**: [Key mistakes/failures]
- **What Went Right**: [What worked well]
- **Prevention Measures**: [Specific actions to prevent recurrence]

## Action Items
- [ ] **Owner**: [Person/Team] - **Task**: [Specific action] - **Deadline**: [Date]
- [ ] **Owner**: [Person/Team] - **Task**: [Specific action] - **Deadline**: [Date]

## Policy Updates Needed
- [ ] Update [specific policy/instruction] to include [new requirement]
- [ ] Add automated check for [failure pattern] in CI/CD
- [ ] Enhance monitoring for [early detection metric]
```

## Automated Failure Prevention

### CI/CD Integration Requirements
- **Pre-Commit Checks**: Scan for known failure patterns in code changes
- **Automated Testing**: Include regression tests for past incidents
- **Pattern Matching**: Flag code that matches historical failure patterns
- **Risk Assessment**: Score changes based on historical incident data

### Monitoring & Alerting
- **Real-time Detection**: Monitor for failure precursors (error rates, performance degradation)
- **Automated Alerts**: Notify teams when failure patterns detected
- **Predictive Analysis**: Use historical data to predict potential failures
- **Threshold Management**: Adjust based on lessons learned from false positives

## Knowledge Management & Application

### Lessons Learned Repository Structure
```
.ai/lessons/
├── incidents/           # Individual incident reports
│   ├── YYYY-MM-DD-incident-title.md
│   └── ...
├── patterns/           # Common failure patterns
│   ├── database-connection-failures.md
│   ├── authentication-bugs.md
│   └── ...
├── prevention/         # Prevention strategies
│   ├── automated-testing-patterns.md
│   ├── monitoring-best-practices.md
│   └── ...
└── metrics/            # Effectiveness tracking
    ├── quarterly-report.md
    └── prevention-effectiveness.md
```

### Cross-Team Knowledge Sharing
- **Weekly Lessons Learned Review**: Share key insights across teams
- **Monthly Pattern Analysis**: Identify emerging failure trends
- **Quarterly Effectiveness Review**: Measure prevention success rates
- **Annual Process Audit**: Comprehensive review and improvement

### Policy Integration Requirements
- **Automatic Policy Updates**: Lessons learned trigger policy review workflows
- **Version Control**: Track policy changes with incident references
- **Audit Trail**: Link policy updates to specific incidents and lessons
- **Effectiveness Tracking**: Monitor if policy changes reduce incident rates

## Success Metrics & Continuous Improvement

### Quantitative Metrics
- **Failure Recurrence Rate**: <10% reduction quarterly target
- **Lessons Learned Capture Rate**: >95% for Severity 2+ incidents
- **Prevention Effectiveness**: >80% of applied lessons prevent future incidents
- **Response Time**: <1 hour average for incident documentation

### Qualitative Metrics
- **Team Satisfaction**: >4.5/5.0 rating on lessons learned process
- **Knowledge Application**: >90% of teams report using shared lessons
- **Process Maturity**: Regular improvements based on feedback
- **Cultural Adoption**: Lessons learned referenced in planning discussions

### Continuous Improvement Process
- **Monthly Review**: Assess metrics and identify improvement areas
- **Quarterly Audit**: Comprehensive process and tool evaluation
- **Annual Planning**: Major process improvements and tool investments
- **Feedback Integration**: Regular team input on process effectiveness

## Accountability & Enforcement

### Individual Responsibilities
- **All Developers**: Check lessons learned before implementation
- **Team Leads**: Ensure team compliance with lessons learned processes
- **QA Team**: Include lessons learned in testing scenarios
- **DevOps Team**: Implement automated prevention measures

### Process Enforcement
- **Code Reviews**: Mandatory check for lessons learned application
- **Release Gates**: Block releases without incident resolution
- **Performance Reviews**: Include lessons learned participation
- **Training Requirements**: Mandatory lessons learned training for new hires

### Escalation Procedures
- **Missed Deadlines**: Automatic escalation for overdue action items
- **Repeated Failures**: Immediate management review for pattern failures
- **Policy Violations**: Progressive discipline for repeated non-compliance
- **Process Failures**: Regular audits with corrective action requirements

---

**MANDATORY COMPLIANCE**: All team members must adhere to these lessons learned requirements. Non-compliance will result in escalated review and potential corrective actions.