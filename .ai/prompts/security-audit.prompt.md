---
agent: Security
description: Security review and compliance checklist
---

# Security Audit

Perform a security audit on the provided code/feature.

## Audit Checklist

### Authentication & Authorization
- [ ] Authentication mechanism is secure
- [ ] Session management is properly implemented
- [ ] Authorization checks on all protected resources
- [ ] Token/session expiration handled correctly

### Data Protection
- [ ] Sensitive data encrypted at rest
- [ ] HTTPS/TLS for data in transit
- [ ] No sensitive data in logs
- [ ] Proper data sanitization

### Input Validation
- [ ] All user inputs validated
- [ ] Proper encoding for outputs (XSS prevention)
- [ ] File upload restrictions in place
- [ ] Path traversal prevention

### Injection Prevention
- [ ] Parameterized queries (SQL injection)
- [ ] Command injection prevention
- [ ] LDAP injection prevention (if applicable)

### Configuration Security
- [ ] No hardcoded credentials
- [ ] Secure default configurations
- [ ] Error messages don't expose internals
- [ ] Debug mode disabled in production

### Dependencies
- [ ] No known vulnerable dependencies
- [ ] Dependencies from trusted sources
- [ ] Regular dependency updates

## Output Format

```
## Security Audit Report

### Audit Scope
[What was reviewed]

### Risk Level: [Critical/High/Medium/Low/None]

### Critical Findings
| # | Finding | Risk | Recommendation |
|---|---------|------|----------------|
| 1 | ...     | ...  | ...            |

### Warnings
- [Warning 1]
- [Warning 2]

### Recommendations
- [Recommendation 1]
- [Recommendation 2]

### Sign-off
[ ] Approved for Production
[ ] Approved with Conditions
[ ] Not Approved - Remediation Required
```
