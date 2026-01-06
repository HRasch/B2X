---
docid: KB-055
title: Security MCP Best Practices
owner: @Security
status: Active
---

# Security MCP Best Practices

**DocID**: `KB-055`  
**Last Updated**: 6. Januar 2026  
**Owner**: @Security

## Overview

The Security MCP server provides automated security analysis tools for vulnerability scanning, SQL injection detection, XSS prevention, input validation, and authentication/authorization checks. This guide covers best practices for integrating security MCP into development workflows.

## MCP Server Configuration

**Location**: `tools/SecurityMCP/`  
**MCP Config**: `.vscode/mcp.json` (enabled by default)

```json
{
  "mcpServers": {
    "security-mcp": {
      "command": "node",
      "args": ["tools/SecurityMCP/dist/index.js"],
      "env": { "NODE_ENV": "production" },
      "disabled": false
    }
  }
}
```

## Available Tools

### 1. scan_vulnerabilities
**Purpose**: Scan dependencies for known CVEs and security advisories  
**Use Cases**:
- Daily morning scan for new vulnerabilities
- After dependency updates
- Before production deployment
- Weekly security audit

**Example**:
```typescript
{
  workspacePath: "."
}
```

**Returns**:
- CVE numbers and descriptions
- Severity levels (LOW, MEDIUM, HIGH, CRITICAL)
- Affected packages and versions
- Available fixes or workarounds
- CVSS scores

**Policy**:
- ❌ **BLOCK PR** on CRITICAL vulnerabilities
- ⚠️ **REQUIRE @Security review** on HIGH vulnerabilities
- ✅ **DOCUMENT** MEDIUM vulnerabilities with mitigation plan
- ℹ️ **TRACK** LOW vulnerabilities for future updates

---

### 2. check_sql_injection
**Purpose**: Detect potential SQL injection vulnerabilities  
**Use Cases**:
- Validate database queries in backend code
- Check Entity Framework queries
- Ensure parameterized queries
- Pre-commit validation on data access code

**Example**:
```typescript
{
  workspacePath: "backend/Domain"
}
```

**Returns**:
- Potential SQL injection points
- String concatenation in queries
- Dynamic SQL without validation
- Recommendations for parameterized queries

**Pattern Detection**:
```csharp
// ❌ VULNERABLE - String concatenation
var query = $"SELECT * FROM Users WHERE Id = {userId}";

// ❌ VULNERABLE - Dynamic SQL
var sql = "SELECT * FROM " + tableName + " WHERE Id = " + id;

// ✅ SAFE - Parameterized query
var query = context.Users.Where(u => u.Id == userId);

// ✅ SAFE - EF Core parameter
var result = await context.Users
    .FromSqlInterpolated($"SELECT * FROM Users WHERE Id = {userId}")
    .ToListAsync();
```

---

### 3. scan_xss_vulnerabilities
**Purpose**: Scan frontend code for XSS vulnerabilities  
**Use Cases**:
- Validate Vue component templates
- Check output encoding
- Ensure CSP headers
- Detect unsafe DOM manipulation

**Example**:
```typescript
{
  workspacePath: "frontend/Store"
}
```

**Returns**:
- Potential XSS points
- Missing output encoding
- Unsafe innerHTML usage
- eval() or Function() calls
- CSP header validation
- Recommendations

**Pattern Detection**:
```typescript
// ❌ VULNERABLE - innerHTML with user data
element.innerHTML = userInput;

// ❌ VULNERABLE - eval() usage
eval(userCode);

// ❌ VULNERABLE - Function constructor
new Function(userInput)();

// ✅ SAFE - textContent
element.textContent = userInput;

// ✅ SAFE - Vue interpolation (auto-escaped)
<template>
  <div>{{ userInput }}</div>
</template>

// ✅ SAFE - Explicit sanitization
import DOMPurify from 'dompurify';
element.innerHTML = DOMPurify.sanitize(userInput);
```

---

### 4. validate_input_sanitization
**Purpose**: Validate input sanitization and validation patterns  
**Use Cases**:
- Check form validation
- Ensure allowlist validation
- Validate input encoding
- API endpoint security

**Example**:
```typescript
{
  workspacePath: "backend/Gateway"
}
```

**Returns**:
- Input validation coverage
- Missing sanitization
- Denylist-only patterns (weak)
- Allowlist patterns (strong)
- Input length validation
- Recommendations

**Best Practices**:
```csharp
// ❌ WEAK - Denylist approach
if (input.Contains("<script>") || input.Contains("javascript:"))
    throw new ValidationException();

// ✅ STRONG - Allowlist approach
if (!Regex.IsMatch(input, @"^[a-zA-Z0-9\s]+$"))
    throw new ValidationException("Only alphanumeric characters allowed");

// ✅ STRONG - Bounded input
[StringLength(100, MinimumLength = 3)]
public string Username { get; set; }

// ✅ STRONG - Type-safe validation
public record CreateUserCommand(
    [EmailAddress] string Email,
    [StringLength(100)] string Name,
    [Range(18, 120)] int Age
);
```

---

### 5. check_authentication
**Purpose**: Validate authentication and authorization patterns  
**Use Cases**:
- Check auth library usage
- Validate session management
- Ensure authorization checks
- Principle of least privilege

**Example**:
```typescript
{
  workspacePath: "backend"
}
```

**Returns**:
- Authentication implementation analysis
- Custom crypto detection (❌ forbidden)
- Session management patterns
- Authorization check coverage
- Principle of least privilege validation
- Recommendations

**Best Practices**:
```csharp
// ❌ FORBIDDEN - Custom crypto
public class CustomPasswordHasher 
{
    public string Hash(string password) 
    {
        // Never implement custom crypto!
    }
}

// ✅ CORRECT - Use ASP.NET Core Identity
services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ✅ CORRECT - Authorization on every endpoint
[Authorize(Roles = "Admin")]
[HttpDelete("users/{id}")]
public async Task<IActionResult> DeleteUser(string id)

// ✅ CORRECT - Resource-based authorization
var authResult = await _authorizationService
    .AuthorizeAsync(User, resource, "ResourcePolicy");
if (!authResult.Succeeded)
    return Forbid();
```

---

## Development Workflows

### Daily Security Workflow

```bash
# Morning routine (5 minutes)
security-mcp/scan_vulnerabilities workspacePath="."
# Check for new CVEs overnight

# Review results → Update dependencies if CRITICAL found
```

---

### Pre-Commit Security Checklist

**Backend changes**:
```bash
# 1. SQL injection check
security-mcp/check_sql_injection workspacePath="backend"

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="backend"

# 3. Authentication check (if auth code changed)
security-mcp/check_authentication workspacePath="backend"
```

**Frontend changes**:
```bash
# 1. XSS vulnerability scan
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="frontend/Store"
```

**Dependency changes**:
```bash
# After adding/updating packages
security-mcp/scan_vulnerabilities workspacePath="."
```

---

### Pre-PR Security Workflow

```bash
# Full security scan before requesting review
echo "Running comprehensive security analysis..."

# 1. Vulnerability scan
security-mcp/scan_vulnerabilities workspacePath="."

# 2. SQL injection check
security-mcp/check_sql_injection workspacePath="backend"

# 3. XSS scan
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"

# 4. Input validation
security-mcp/validate_input_sanitization workspacePath="."

# 5. Authentication check
security-mcp/check_authentication workspacePath="backend"

echo "✅ All security MCP tools passed"
```

**Policy**:
- All tools must return GREEN before PR can be approved
- CRITICAL issues → PR blocked automatically
- HIGH issues → Requires @Security approval
- Document exceptions in `.ai/compliance/security-exceptions.md`

---

### Weekly Security Audit

```bash
# Full codebase security audit (run Friday)
echo "Weekly security audit - $(date)"

# 1. Full vulnerability scan
security-mcp/scan_vulnerabilities workspacePath="."

# 2. Backend security
security-mcp/check_sql_injection workspacePath="backend"
security-mcp/validate_input_sanitization workspacePath="backend"
security-mcp/check_authentication workspacePath="backend"

# 3. Frontend security
security-mcp/scan_xss_vulnerabilities workspacePath="frontend"
security-mcp/validate_input_sanitization workspacePath="frontend"

# 4. Generate report
echo "Saving results to .ai/compliance/security-audit-$(date +%Y%m%d).md"
```

---

## Integration with Quality Gates (ADR-020)

### Automated PR Gates

Security MCP results determine PR merge eligibility:

```yaml
# .github/workflows/security-gate.yml
name: Security Gate

on: [pull_request]

jobs:
  security-scan:
    runs-on: ubuntu-latest
    steps:
      - name: Vulnerability Scan
        run: npm run security:vulnerabilities
        
      - name: SQL Injection Check
        run: npm run security:sql-injection
        
      - name: XSS Scan
        run: npm run security:xss
        
      - name: Evaluate Results
        run: |
          if grep -q "CRITICAL" security-results.json; then
            echo "❌ CRITICAL vulnerabilities found - blocking PR"
            exit 1
          fi
```

**Result Interpretation**:
- ✅ **GREEN** (no issues) → PR approved for merge
- ⚠️ **YELLOW** (warnings) → Requires @Security review and approval
- ❌ **RED** (critical) → PR blocked until issues resolved

---

## OWASP Top Ten Compliance

Security MCP tools map to OWASP Top Ten vulnerabilities:

| OWASP Category | MCP Tool | Coverage |
|---|---|---|
| A01: Broken Access Control | `check_authentication` | Authorization patterns |
| A02: Cryptographic Failures | `check_authentication` | Custom crypto detection |
| A03: Injection | `check_sql_injection` | SQL injection |
| A03: Injection | `scan_xss_vulnerabilities` | XSS prevention |
| A04: Insecure Design | `validate_input_sanitization` | Input validation |
| A05: Security Misconfiguration | `scan_vulnerabilities` | Dependency issues |
| A06: Vulnerable Components | `scan_vulnerabilities` | Known CVEs |
| A07: Auth Failures | `check_authentication` | Session management |

**Reference**: See [KB-010] OWASP Top Ten for detailed requirements

---

## Security Exception Handling

### When MCP Reports Cannot Be Fixed Immediately

**Example**: Dependency vulnerability with no available fix

1. **Document in `.ai/compliance/security-exceptions.md`**:
```markdown
## Exception: CVE-2025-12345 (lodash@4.17.20)

- **Component**: lodash@4.17.20
- **Severity**: HIGH
- **CVE**: CVE-2025-12345
- **Issue**: Prototype pollution vulnerability
- **Reason**: No fix available, awaiting upstream release
- **Mitigation**: 
  - Not using affected functions (_.merge, _.mergeWith)
  - Input validation prevents exploitability
  - Firewall rules block exploit vectors
- **Review Date**: 2026-02-01
- **Approved By**: @Security
- **GitHub Issue**: #1234
```

2. **Create GitHub Issue**:
```bash
gh issue create \
  --title "Security Exception: CVE-2025-12345 (lodash)" \
  --label "security-exception" \
  --assignee @Security \
  --body "See .ai/compliance/security-exceptions.md"
```

3. **Weekly Re-check**:
```bash
# Check if fix is now available
security-mcp/scan_vulnerabilities workspacePath="."
```

---

## Common Security Patterns

### SQL Injection Prevention

```csharp
// Entity Framework Core (recommended)
public class UserRepository
{
    private readonly AppDbContext _context;
    
    // ✅ SAFE - LINQ with parameters
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }
    
    // ✅ SAFE - FromSqlInterpolated
    public async Task<List<User>> GetUsersByStatusAsync(string status)
    {
        return await _context.Users
            .FromSqlInterpolated($"SELECT * FROM Users WHERE Status = {status}")
            .ToListAsync();
    }
    
    // ❌ VULNERABLE - FromSqlRaw with concatenation
    public async Task<List<User>> GetUsersUnsafe(string status)
    {
        return await _context.Users
            .FromSqlRaw($"SELECT * FROM Users WHERE Status = '{status}'")
            .ToListAsync();
    }
}
```

---

### XSS Prevention

```vue
<template>
  <!-- ✅ SAFE - Vue auto-escapes interpolation -->
  <div>{{ userInput }}</div>
  
  <!-- ✅ SAFE - Attribute binding auto-escaped -->
  <a :href="userProvidedUrl">Link</a>
  
  <!-- ❌ VULNERABLE - v-html with user input -->
  <div v-html="userInput"></div>
  
  <!-- ✅ SAFE - v-html with sanitization -->
  <div v-html="sanitize(userInput)"></div>
</template>

<script setup lang="ts">
import DOMPurify from 'dompurify';

const sanitize = (html: string) => DOMPurify.sanitize(html);

// ✅ SAFE - Content Security Policy
// Add to index.html:
// <meta http-equiv="Content-Security-Policy" 
//       content="default-src 'self'; script-src 'self' 'unsafe-inline'">
</script>
```

---

### Input Validation

```csharp
// ✅ STRONG - Allowlist with regex
public class CreateProductCommand
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9\s-]{3,100}$", 
        ErrorMessage = "Product name must be 3-100 alphanumeric characters")]
    public string Name { get; set; }
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [EmailAddress]
    public string? ContactEmail { get; set; }
    
    [Url]
    public string? ProductUrl { get; set; }
}

// Handler validates automatically
public class CreateProductHandler : ICommandHandler<CreateProductCommand>
{
    public async Task<Result> HandleAsync(CreateProductCommand command)
    {
        // DataAnnotations validated by Wolverine pipeline
        // Additional business validation
        if (await _repo.ExistsAsync(command.Name))
            return Result.Fail("Product already exists");
            
        // Safe to proceed
        var product = new Product(command.Name, command.Price);
        await _repo.AddAsync(product);
        return Result.Success();
    }
}
```

---

## Continuous Monitoring

### Scheduled Security Scans

```bash
# .github/workflows/scheduled-security.yml
name: Scheduled Security Scan

on:
  schedule:
    - cron: '0 9 * * 1' # Every Monday at 9am
    
jobs:
  security-audit:
    runs-on: ubuntu-latest
    steps:
      - name: Full Security MCP Audit
        run: npm run security:full-audit
        
      - name: Generate Report
        run: npm run security:report
        
      - name: Create Issue if Critical
        if: failure()
        run: |
          gh issue create \
            --title "🚨 Critical Security Issues Detected" \
            --label "security,critical" \
            --assignee @Security
```

---

### Post-Deploy Security Validation

```bash
# After production deployment
echo "Running post-deploy security validation..."

# 1. Verify no new vulnerabilities introduced
security-mcp/scan_vulnerabilities workspacePath="."

# 2. Validate authentication in production
# (manual check - ensure MCP patterns deployed correctly)

# 3. Check CSP headers
curl -I https://production-url.com | grep -i "content-security-policy"

# 4. Verify HTTPS enforcement
curl -I http://production-url.com | grep -i "location: https"
```

---

## Performance Considerations

### Targeted Scans

```typescript
// ✅ EFFICIENT - Scan specific directory
security-mcp/check_sql_injection workspacePath="backend/Domain/Catalog"

// ❌ SLOW - Full workspace scan
security-mcp/check_sql_injection workspacePath="."
```

### Incremental Analysis

```bash
# During development - check changed files only
git diff --name-only | grep '.cs$' | xargs -I {} \
  security-mcp/check_sql_injection workspacePath="backend"
  
# Pre-PR - full analysis
security-mcp/check_sql_injection workspacePath="backend"
```

---

## Related Documentation

- [KB-010] - OWASP Top Ten
- [KB-053] - TypeScript MCP Integration
- [KB-054] - Vue MCP Integration
- [ADR-020] - PR Quality Gate
- [GL-008] - Governance Policies
- [INS-005] - Security Instructions

---

## Troubleshooting

### False Positives

```typescript
// If MCP flags safe code as vulnerable:
// 1. Document in code with comment
// 2. Add to security exceptions file
// 3. Create GitHub issue for MCP improvement

// Example:
// MCP-SECURITY-EXCEPTION: This dynamic SQL is safe because tableName
// is validated against whitelist of allowed tables in DatabaseSchema class
var sql = $"SELECT * FROM {validatedTableName} WHERE Id = @id";
```

### MCP Server Performance

```bash
# If scans are slow:
# 1. Check MCP server logs
cd tools/SecurityMCP
npm run dev # Run in dev mode to see detailed logs

# 2. Clear caches
rm -rf node_modules/.cache

# 3. Restart MCP server
# VS Code: Reload window
```

---

**Maintained by**: @Security  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026
