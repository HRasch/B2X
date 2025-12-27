---
name: P0 - Critical Security Issue
about: Critical security issues that must be fixed before production
title: "[P0] "
labels: "severity: critical, type: security"
assignees: ''

---

## 🚨 Critical Security Issue

**Priority:** P0 - MUST FIX  
**Category:** [Select one: P0.1-Secrets, P0.2-CORS, P0.3-Encryption, P0.4-Audit]  
**Timeline:** Must be fixed by Friday EOD  

---

## Problem Description

### What is the issue?
[Describe the security vulnerability clearly]

### Why is this critical?
- [ ] Data exposure risk
- [ ] Unauthorized access risk
- [ ] Compliance violation (GDPR, SOC2, etc.)
- [ ] Authentication bypass
- [ ] Injection attack vulnerability

### Affected Areas
- **Services:** [e.g., Admin API, Store API, Identity Service]
- **Files:** [e.g., Program.cs, appsettings.json]
- **Scope:** [Production, Development, All]

---

## Solution Overview

### Proposed Fix
[Link to CRITICAL_ISSUES_ROADMAP.md section or describe fix approach]

### Implementation Approach
1. Step 1: [specific action]
2. Step 2: [specific action]
3. Step 3: [specific action]

### Files to Modify
- [ ] File 1 - Change description
- [ ] File 2 - Change description
- [ ] File 3 - Change description

---

## Testing Requirements

### Unit Tests
- [ ] Test case 1: [describe what to test]
- [ ] Test case 2: [describe what to test]

### Integration Tests
- [ ] Service starts correctly with fix
- [ ] Configuration loads properly
- [ ] No regression in other services

### Manual Tests
```bash
# Test command 1
dotnet test backend/...

# Test command 2
curl http://localhost:8080/health
```

---

## Success Criteria

- [ ] No hardcoded secrets/credentials in code
- [ ] All tests passing (unit + integration)
- [ ] Code review approved by 2 developers
- [ ] Build successful
- [ ] SECURITY_HARDENING_GUIDE.md checklist complete
- [ ] PR merged to main

---

## Related Documentation

- [CRITICAL_ISSUES_ROADMAP.md](../../CRITICAL_ISSUES_ROADMAP.md)
- [SECURITY_HARDENING_GUIDE.md](../../SECURITY_HARDENING_GUIDE.md)
- [COMPREHENSIVE_REVIEW.md](../../COMPREHENSIVE_REVIEW.md)

---

## Checklist

- [ ] Problem identified and understood
- [ ] Solution approach validated
- [ ] Code changes implemented
- [ ] Tests written and passing
- [ ] Documentation updated
- [ ] Code reviewed
- [ ] Ready to merge
