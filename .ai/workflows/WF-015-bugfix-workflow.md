---
docid: WF-015
title: WF 015 Bugfix Workflow
owner: @DocMaintainer
status: Active
created: 2026-01-09
---

# WF-015: Bugfix Workflow with runSubagent

**Version:** 1.0  
**Created:** 9. Januar 2026  
**Used by:** All Agents (coordinated by @SARAH)

## Overview

This workflow provides a structured approach to bug fixing using runSubagent strategies for efficient error diagnosis, isolation, and resolution. It leverages parallel processing and specialized subagents to reduce debugging time and improve fix quality.

## When to Use

- **Build failures:** Compilation errors, dependency issues
- **Runtime errors:** Exceptions, crashes, unexpected behavior
- **Test failures:** Unit test failures, integration test issues
- **Performance issues:** Memory leaks, slow operations, bottlenecks
- **Security vulnerabilities:** Identified through audits or scans
- **Compliance issues:** Regulatory or standards violations

## Core Strategies

### Strategy 1: Diagnostic Isolation
**Description:** Use dedicated runSubagent for error analysis and reproduction.

**Advantages:**
- Separates diagnosis from implementation
- Enables specialized debugging tools per error type
- Reduces context pollution in main development session
- Provides structured bug reports for tracking

**When to Use:**
- Complex error stacks requiring deep analysis
- Intermittent or environment-specific issues
- When error reproduction needs isolation

### Strategy 2: Parallel Fix Development
**Description:** Deploy multiple runSubagents for different fix approaches.

**Advantages:**
- Explores multiple solution paths simultaneously
- Enables A/B testing of fix strategies
- Reduces time to first working solution
- Provides fallback options if primary fix fails

**When to Use:**
- Multiple potential root causes identified
- High-impact bugs requiring rapid resolution
- When fix complexity suggests multiple approaches

### Strategy 3: Validation Automation
**Description:** Automated testing and validation through background runSubagents.

**Advantages:**
- Continuous validation during fix development
- Early detection of regression issues
- Ensures fix completeness across all test scenarios
- Reduces manual testing overhead

**When to Use:**
- Fixes affecting multiple components
- Critical path bugs requiring thorough validation
- When automated test coverage is available

## Step-by-Step Process

### Phase 1: Error Capture (5-10 min)
```
1. Document error details:
   - Error message and stack trace
   - Reproduction steps
   - Environment details (OS, versions, config)
   - Impact assessment (severity, affected users)

2. Create bug tracking entry:
   - .ai/issues/bug-{id}.md
   - Include all capture details
   - Assign priority and timeline
```

### Phase 2: Diagnostic Analysis (15-30 min)
```
Deploy diagnostic runSubagent:
- Reproduce error in isolated environment
- Analyze error patterns and dependencies
- Identify potential root causes
- Generate diagnostic report

Output: .ai/logs/diagnostic-{bug-id}-{timestamp}.md
```

### Phase 3: Fix Development (30-60 min)
```
Parallel runSubagent execution:
- Subagent A: Implement primary fix approach
- Subagent B: Develop alternative solutions
- Subagent C: Create comprehensive test cases
- Subagent D: Review security/compliance implications

Coordinate through @SARAH for handovers
```

### Phase 4: Validation & Testing (15-30 min)
```
Automated validation:
- Run full test suite
- Performance regression testing
- Security scanning
- Integration testing

Manual validation:
- Reproduction scenario testing
- Edge case verification
- User acceptance testing
```

### Phase 5: Deployment & Monitoring (10-15 min)
```
1. Create fix documentation
2. Update knowledgebase with lessons learned
3. Deploy fix to staging/production
4. Monitor for regression issues
5. Close bug tracking entry
```

## runSubagent Roles in Bugfix

### Diagnostic Subagent
**Responsibilities:**
- Error reproduction and isolation
- Root cause analysis
- Impact assessment
- Recommendation of fix strategies

**Tools Used:**
- Logging analysis
- Stack trace parsing
- Dependency checking
- Environment comparison

### Fix Implementation Subagent
**Responsibilities:**
- Code changes for bug resolution
- Minimal invasive fixes preferred
- Comprehensive testing integration
- Documentation updates

**Tools Used:**
- Code editors and IDEs
- Version control systems
- Automated testing frameworks
- Code review tools

### Validation Subagent
**Responsibilities:**
- Test case development
- Regression testing
- Performance validation
- Security verification

**Tools Used:**
- Test automation frameworks
- Performance monitoring tools
- Security scanners
- Integration test suites

## Success Metrics

### Time Reduction
- **Target:** 50% faster resolution vs. traditional debugging
- **Measurement:** Time from error report to fix deployment
- **Tracking:** .ai/metrics/bugfix-times-{month}.json

### Quality Improvement
- **Target:** <5% regression rate
- **Measurement:** Post-fix issues within 30 days
- **Tracking:** .ai/logs/regression-tracking.md

### Token Efficiency
- **Target:** 40-60% token savings per bugfix
- **Measurement:** Context size and API calls per session
- **Tracking:** .ai/logs/token-usage-bugfix.md

## Common Patterns

### Pattern 1: Build Failures
```
Diagnostic: Check dependency versions, compiler flags
Fix: Update packages, adjust build configuration
Validation: Clean build, multi-environment testing
```

### Pattern 2: Runtime Exceptions
```
Diagnostic: Stack trace analysis, input validation
Fix: Error handling, null checks, boundary validation
Validation: Exception scenarios, edge cases
```

### Pattern 3: Performance Issues
```
Diagnostic: Profiling, bottleneck identification
Fix: Algorithm optimization, caching, async processing
Validation: Load testing, performance benchmarks
```

### Pattern 4: Security Vulnerabilities
```
Diagnostic: Vulnerability scanning, code review
Fix: Input sanitization, secure coding practices
Validation: Security testing, compliance checks
```

## Integration with Other Workflows

### WF-001 Context Optimization
- Use during long debugging sessions
- Optimize context between diagnostic phases
- Archive completed diagnostic logs

### WF-002 Subagent Delegation
- Coordinate multiple runSubagents for complex bugs
- Hand over specialized tasks (security, performance)
- Maintain communication channels

### GL-009 AI Behavior
- Follow token optimization during debugging
- Use structured commit messages for fixes
- Document lessons in knowledgebase

## Tools & Automation

### MCP Integration
```
Background processing:
- Continuous error monitoring
- Automated test execution
- Performance regression detection
- Security vulnerability scanning
```

### Scripts & Templates
```
Available tools:
- scripts/bug-diagnostic.sh
- templates/bug-report.md
- scripts/regression-test.sh
- templates/fix-validation.md
```

## Risk Mitigation

### Escalation Triggers
- **High Priority:** Security or data loss risks
- **Stuck Issues:** No progress after 2 hours
- **Scope Creep:** Fix requires architecture changes

### Fallback Procedures
- **Manual Override:** If runSubagent fails, switch to direct implementation
- **Expert Consultation:** Involve domain specialists for complex issues
- **Pair Debugging:** Two agents collaborate on critical fixes

## Documentation Outputs

### Bug Report Template
```markdown
# Bug Report: {BUG-ID}

**Status:** Open | **Priority:** High  
**Reported:** {DATE} | **Assignee:** @{AGENT}

## Error Details
- **Message:** {ERROR_MESSAGE}
- **Stack Trace:** {STACK_TRACE}
- **Environment:** {ENV_DETAILS}

## Reproduction Steps
1. {STEP_1}
2. {STEP_2}
3. {STEP_3}

## Root Cause
{Analysis from diagnostic subagent}

## Fix Applied
{Description of changes}

## Validation Results
{Test outcomes and coverage}

## Lessons Learned
{What was learned, knowledgebase updates}
```

### Fix Documentation
- Code comments explaining fix rationale
- Test cases added for regression prevention
- Knowledgebase entry for similar issues
- Changelog entry for release notes

---

**Next Review:** 15. Januar 2026  
**Maintained by:** @SARAH