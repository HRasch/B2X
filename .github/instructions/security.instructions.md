---
applyTo: "**/*"
---

# Security Instructions

## Secrets & Credentials
- NEVER commit secrets, API keys, or passwords
- Use environment variables for all sensitive data
- Check for exposed secrets before committing
- Rotate compromised credentials immediately

## Input Validation
- Validate ALL user inputs (client and server side)
- Use allowlists over denylists
- Sanitize inputs before database operations
- Limit input lengths appropriately

## Authentication & Authorization
- Use established auth libraries (no custom crypto)
- Implement proper session management
- Check authorization on every protected resource
- Use principle of least privilege

## Data Protection
- Encrypt sensitive data at rest and in transit
- Use HTTPS for all communications
- Mask sensitive data in logs
- Implement proper data retention policies

## Common Vulnerabilities
- SQL Injection: Use parameterized queries
- XSS: Encode output, use CSP headers
- CSRF: Implement anti-CSRF tokens
- Path Traversal: Validate file paths

## Security Reviews
- Flag security-sensitive changes for review
- Document security decisions
- Report vulnerabilities through proper channels

## MCP-Powered Security Workflow

**Reference**: See [KB-055] Security MCP Best Practices for detailed workflows.

### Mandatory Pre-Commit Security Checks

Run these MCP tools before every commit:

```bash
# 1. Dependency vulnerability scan
security-mcp/scan_vulnerabilities workspacePath="."
# Action: FAIL on HIGH or CRITICAL vulnerabilities

# 2. SQL injection detection (backend)
security-mcp/check_sql_injection workspacePath="backend"
# Check: All database queries use parameterized statements

# 3. XSS vulnerability scan (frontend)
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"
# Check: Output encoding, CSP headers, DOM sanitization

# 4. Input validation patterns
security-mcp/validate_input_sanitization workspacePath="."
# Check: All user inputs validated and sanitized

# 5. Authentication patterns
security-mcp/check_authentication workspacePath="backend"
# Check: Proper auth implementation, session management
```

### Security MCP Integration with Development Workflow

#### Daily Security Checks
- **Morning**: Run `security-mcp/scan_vulnerabilities` for new CVEs
- **Pre-commit**: Run security MCP on changed files only
- **Pre-PR**: Full security MCP suite on modified areas

#### Code Review Requirements (PRM-002)
- **Automated gates**: All security MCP tools must pass GREEN
- **Auto-fail**: CRITICAL vulnerabilities block PR merge
- **Documentation**: Exceptions documented in `.ai/compliance/`

#### Weekly Security Audits
- **Full scan**: Run all security MCP tools on entire codebase
- **Dependency review**: Check for outdated packages with security issues
- **Compliance check**: Verify OWASP Top Ten coverage
- **Report**: Generate security audit report in `.ai/compliance/`

### Security MCP Tool Reference

#### Backend Security
```typescript
// SQL Injection Detection
security-mcp/check_sql_injection workspacePath="backend/Domain"
// ✓ Parameterized queries only
// ✗ String concatenation in queries
// ✗ Dynamic SQL without validation

// Authentication Validation  
security-mcp/check_authentication workspacePath="backend/Gateway"
// ✓ Established auth libraries
// ✓ Proper session management
// ✗ Custom crypto implementations
```

#### Frontend Security
```typescript
// XSS Vulnerability Scanning
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"
// ✓ Output encoding in templates
// ✓ CSP headers configured
// ✗ innerHTML with user data
// ✗ eval() or Function() usage

// Input Sanitization
security-mcp/validate_input_sanitization workspacePath="frontend/Store"
// ✓ Form validation on all inputs
// ✓ Allowlist validation patterns
// ✗ Denylist-only validation
```

#### Dependency Security
```typescript
// Vulnerability Scanning
security-mcp/scan_vulnerabilities workspacePath="."
// Checks: npm audit, NuGet packages
// Reports: CVE numbers, severity levels
// Actions: Update or document exceptions
```

### Security Exception Handling

When MCP tools report issues that cannot be immediately fixed:

1. **Document in `.ai/compliance/security-exceptions.md`**:
   ```markdown
   ## Exception: [CVE-XXXX-YYYY]
   - **Component**: package-name@version
   - **Severity**: HIGH
   - **Reason**: No fix available, awaiting upstream patch
   - **Mitigation**: [Describe compensating controls]
   - **Review Date**: [Next review date]
   - **Approved By**: @Security
   ```

2. **Track in GitHub Issues**: Create security issue with label `security-exception`

3. **Regular Review**: Re-run MCP tools weekly to check if fix is available

### Integration with Quality Gates (ADR-020)

Security MCP results are **mandatory quality gates**:

- ✅ **GREEN**: All checks passed → PR can proceed
- ⚠️ **YELLOW**: Warnings present → Requires @Security review
- ❌ **RED**: Critical issues → PR blocked until resolved

### Continuous Security Monitoring

- **After dependency updates**: Re-run all security MCP tools
- **Before production deploy**: Full security MCP audit mandatory
- **Scheduled scans**: Weekly automated security MCP runs
- **Incident response**: Run security MCP to validate fixes

