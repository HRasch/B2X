---
agent: Security
description: Security review and compliance checklist
---

# Security Audit

Perform a security audit on the provided code/feature using automated MCP tools and manual review.

## Phase 1: Automated MCP Security Scan (MANDATORY)

**Run all Security MCP tools before manual review:**

### 1. Dependency Vulnerability Scan
```bash
security-mcp/scan_vulnerabilities workspacePath="."
```

**Evaluate Results**:
- ❌ **CRITICAL**: Block deployment, requires immediate fix
- ⚠️ **HIGH**: Requires @Security approval with mitigation plan
- ✅ **MEDIUM/LOW**: Document in `.ai/compliance/security-exceptions.md`

**Action**: If vulnerabilities found, check for available updates or mitigation strategies

---

### 2. SQL Injection Detection (Backend)
```bash
security-mcp/check_sql_injection workspacePath="backend"
```

**Pass Criteria**:
- ✅ All database queries use parameterized statements
- ✅ No string concatenation in SQL queries
- ✅ Entity Framework LINQ or `FromSqlInterpolated` only

**Fail Patterns**:
- ❌ String concatenation: `$"SELECT * FROM Users WHERE Id = {userId}"`
- ❌ Dynamic SQL: `"SELECT * FROM " + tableName`
- ❌ Raw queries without parameters

---

### 3. XSS Vulnerability Scan (Frontend)
```bash
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"
```

**Pass Criteria**:
- ✅ Vue interpolation used (auto-escaped): `{{ userInput }}`
- ✅ CSP headers configured
- ✅ No `innerHTML` with user data
- ✅ No `eval()` or `Function()` usage

**Fail Patterns**:
- ❌ `element.innerHTML = userInput`
- ❌ `eval(userCode)`
- ❌ `v-html` with unsanitized user input

---

### 4. Input Validation Check
```bash
security-mcp/validate_input_sanitization workspacePath="."
```

**Pass Criteria**:
- ✅ Allowlist validation patterns
- ✅ Input length restrictions
- ✅ Type-safe validation (DataAnnotations)
- ✅ Server-side validation on all endpoints

**Fail Patterns**:
- ❌ Denylist-only validation
- ❌ Missing validation on API endpoints
- ❌ Client-side only validation

---

### 5. Authentication/Authorization Review
```bash
security-mcp/check_authentication workspacePath="backend"
```

**Pass Criteria**:
- ✅ Using ASP.NET Core Identity or established library
- ✅ Proper session management
- ✅ Authorization checks on all protected resources
- ✅ Principle of least privilege

**Fail Patterns**:
- ❌ Custom crypto implementations
- ❌ Missing `[Authorize]` attributes
- ❌ Overly permissive roles

---

## MCP Results Summary Template

```markdown
## Automated Security Scan Results

### Vulnerability Scan
- **Status**: [✅ PASS | ⚠️ WARNINGS | ❌ FAIL]
- **CRITICAL**: [count]
- **HIGH**: [count]
- **MEDIUM**: [count]
- **LOW**: [count]

### SQL Injection Scan
- **Status**: [✅ PASS | ❌ FAIL]
- **Issues Found**: [count]
- **Blocking Issues**: [list]

### XSS Scan
- **Status**: [✅ PASS | ❌ FAIL]
- **Issues Found**: [count]
- **Blocking Issues**: [list]

### Input Validation
- **Status**: [✅ PASS | ⚠️ WARNINGS | ❌ FAIL]
- **Coverage**: [percentage]%
- **Missing Validation**: [list]

### Authentication/Authorization
- **Status**: [✅ PASS | ⚠️ WARNINGS | ❌ FAIL]
- **Issues Found**: [count]
- **Recommendations**: [list]

### Overall MCP Assessment
[✅ All automated checks passed | ⚠️ Warnings present | ❌ Critical issues detected]
```

---

## Phase 2: Manual Security Review

**Only proceed if MCP scans show ✅ PASS or ⚠️ WARNINGS**

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
[What was reviewed - components, files, features]

### Phase 1: Automated MCP Scan Results

#### Dependency Vulnerabilities
- CRITICAL: [count] - [Details or "None"]
- HIGH: [count] - [Details or "None"]
- MEDIUM: [count] - [Details or "None"]
- LOW: [count] - [Details or "None"]

#### SQL Injection Scan
- Status: [✅ PASS | ❌ FAIL]
- Issues: [Details or "None found"]

#### XSS Vulnerability Scan
- Status: [✅ PASS | ❌ FAIL]
- Issues: [Details or "None found"]

#### Input Validation
- Status: [✅ PASS | ⚠️ WARNINGS]
- Coverage: [percentage]%
- Missing: [Details or "Complete"]

#### Authentication/Authorization
- Status: [✅ PASS | ⚠️ WARNINGS]
- Issues: [Details or "None found"]

---

### Phase 2: Manual Review Findings

### Risk Level: [Critical/High/Medium/Low/None]

### Critical Findings (MCP + Manual)
| # | Finding | Source | Risk | Recommendation |
|---|---------|--------|------|----------------|
| 1 | ...     | MCP/Manual | ... | ...         |

### Warnings
- [MCP-flagged warnings]
- [Manual review warnings]

### Recommendations
- [MCP-suggested improvements]
- [Manual review recommendations]

### Security Exceptions
[List any documented exceptions in .ai/compliance/security-exceptions.md]

---

### Compliance Status

#### OWASP Top Ten Coverage
- [ ] A01: Broken Access Control - [Status]
- [ ] A02: Cryptographic Failures - [Status]
- [ ] A03: Injection - [Status]
- [ ] A04: Insecure Design - [Status]
- [ ] A05: Security Misconfiguration - [Status]
- [ ] A06: Vulnerable Components - [Status]
- [ ] A07: Auth Failures - [Status]
- [ ] A08: Data Integrity Failures - [Status]
- [ ] A09: Logging Failures - [Status]
- [ ] A10: SSRF - [Status]

---

### Sign-off
[ ] ✅ Approved for Production (All MCP checks passed, no critical findings)
[ ] ⚠️ Approved with Conditions (Documented exceptions, mitigation in place)
[ ] ❌ Not Approved - Remediation Required (Critical MCP or manual findings)

**Approval Conditions** (if applicable):
- [List specific conditions for approval]
- [Required follow-up actions]
- [Timeline for remediation]

**Next Security Review**: [Date]
```

---

## Post-Audit Actions

### If Critical Issues Found:
1. **Block deployment immediately**
2. **Document in `.ai/compliance/security-exceptions.md`** if cannot be fixed immediately
3. **Create GitHub issue** with label `security-critical`
4. **Notify @Security team**
5. **Re-run MCP tools** after fixes to verify resolution

### If Warnings Present:
1. **Document mitigation** in `.ai/compliance/`
2. **Create improvement tasks** for next sprint
3. **Schedule follow-up review**

### If Approved:
1. **Save audit report** to `.ai/compliance/security-audit-[date].md`
2. **Update security tracking** spreadsheet/dashboard
3. **Schedule next audit** (quarterly or before major release)

---

## References

- [KB-055] Security MCP Best Practices
- [KB-010] OWASP Top Ten
- [mcp-operations.instructions.md] MCP Operations Guide
- [ADR-020] PR Quality Gate
- [GL-008] Governance Policies
