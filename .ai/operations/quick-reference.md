# Lessons Learned Quick Reference

## Process Overview
1. **Detect** incident within 1 hour
2. **Document** using incident template within 24 hours
3. **Analyze** root cause using 5-Why method
4. **Implement** fixes and prevention measures
5. **Review** quarterly for effectiveness

## Severity Classification
- **1 (Critical)**: System down, data loss, security breach → Immediate escalation, post-mortem required
- **2 (High)**: Major feature broken, performance issues → Cross-team review within 24 hours
- **3 (Medium)**: Minor bugs, usability issues → Team-level resolution
- **4 (Low)**: Code quality, minor improvements → Individual/team discretion

## Response Time Requirements
- **Severity 1-2**: Document within 1 hour, resolve within 24 hours
- **Severity 3**: Document within 24 hours, resolve within 1 week
- **Severity 4**: Document within 1 week, resolve as needed

## File Naming Convention
`YYYY-MM-DD-incident-title.md`

## Key Directories
- `incidents/` - Individual incident reports
- `patterns/` - Common failure patterns and trends
- `prevention/` - Prevention strategies and best practices
- `metrics/` - Effectiveness tracking and reports

## Search Commands
```bash
# Find incidents by component
grep -r "PostgreSQL" .ai/lessons/incidents/

# Find prevention measures for specific issue
grep -r "authentication" .ai/lessons/prevention/

# Find recent incidents
find .ai/lessons/incidents/ -name "2025-*.md" | head -10
```

## Before Starting Work
1. Search `.ai/lessons/` for similar patterns
2. Check recent incidents (last 30 days)
3. Apply identified prevention measures
4. Document any risks identified

## During Development
- Monitor for emerging failure patterns
- Report potential issues immediately
- Apply lessons from similar implementations

## Code Review Checklist
- [ ] Lessons learned reviewed for similar patterns
- [ ] Prevention measures from past incidents applied
- [ ] Regression tests added for historical failures
- [ ] Risk assessment documented

## Common Failure Patterns to Watch For
- Database connection timeouts
- Authentication token expiration
- API rate limiting issues
- Memory leaks in long-running processes
- Race conditions in concurrent operations
- Input validation bypasses
- CORS configuration errors
- Bundle size performance issues

## Prevention Best Practices
- Always check lessons learned before implementation
- Include regression tests for past failures
- Use automated tools for pattern detection
- Document assumptions and edge cases
- Implement proper error handling and logging
- Test in production-like environments
- Monitor key metrics proactively

## Metrics to Track
- Failure recurrence rate (<10% quarterly reduction target)
- Lessons learned capture rate (>95% for Severity 2+)
- Prevention effectiveness (>80% success rate)
- Response time (<1 hour average documentation time)

## Getting Help
- **Template Issues**: Use `incident-report-template.md`
- **Process Questions**: Check this quick reference
- **Pattern Research**: Search existing incidents first
- **Tool Support**: Contact DevOps for automated checks

## Emergency Contacts
- **Security Incidents**: @Security immediately
- **System Down**: @DevOps immediately
- **Data Loss**: @Security and management immediately
- **Process Questions**: @SARAH for coordination

---

**Last Updated:** January 2026
**Version:** 1.0