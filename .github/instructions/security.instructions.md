---
docid: INS-013
title: Security.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

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

## Automated SCA & Triage (Proposed)
- Require Dependabot + scheduled CodeQL and Snyk scans (weekly)
- Define SLAs for triage and remediation (e.g., CRITICAL: 24h, HIGH: 7d)
- Critical/high findings must be converted into blocking issues and assigned

## Secret Scanning Enforcement (Proposed)
- Run pre-commit secret checks locally (e.g., git-secrets, detect-secrets)
- CI secret scans block merges on detected secrets

## Control Loop (Proposed)
- CI publishes SCA results to PR checks and weekly audit job
- Unresolved findings older than X days auto-create escalation issues

## Documentation & Runbooks (Proposed)
- Add runbooks for common security remediations
- Maintain contact list for incident escalation

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

# 6. Container security (infrastructure)
docker-mcp/check_container_security imageName="nginx:latest"
# Check: Official images, minimal attack surface, no vulnerabilities
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

#### Infrastructure/Container Security
```typescript
// Container Image Security
docker-mcp/check_container_security imageName="nginx:latest"
// ✓ Official images with specific tags
// ✓ Minimal base images (Alpine, Distroless)
// ✗ Images with known vulnerabilities
// ✗ Latest tag usage

// Dockerfile Security Analysis
docker-mcp/analyze_dockerfile dockerfilePath="Dockerfile"
// ✓ Non-root user execution
// ✓ Minimal attack surface
// ✗ Secrets in environment variables
// ✗ Privilege escalation risks

// Kubernetes Manifest Security
docker-mcp/validate_kubernetes_manifests manifestPath="k8s/"
// ✓ Security contexts configured
// ✓ Resource limits set
// ✗ Privilege escalation allowed
// ✗ Insecure pod configurations
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
  - `security-mcp/scan_vulnerabilities workspacePath="."`
  - `security-mcp/check_sql_injection workspacePath="backend"`
  - `security-mcp/scan_xss_vulnerabilities workspacePath="frontend"`
  - `docker-mcp/check_container_security` (for all container images)
  - `docker-mcp/validate_kubernetes_manifests` (for K8s deployments)
- **Scheduled scans**: Weekly automated security MCP runs
- **Incident response**: Run security MCP to validate fixes

