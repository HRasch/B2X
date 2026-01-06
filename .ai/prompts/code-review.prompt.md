---
agent: TechLead
description: Standardized code review process
---

# Code Review Request

Review the provided code changes with focus on:

## Review Checklist

### 0. MCP Automated Quality Gates (Run First)

**Before manual review, verify automated MCP validation has passed:**

#### Frontend Changes
```bash
# 1. Type safety
typescript-mcp/analyze_types workspacePath="frontend/Store"
# ✅ Required: Zero type errors

# 2. i18n compliance (CRITICAL)
vue-mcp/validate_i18n_keys workspacePath="frontend/Store"
# ✅ Required: Zero hardcoded strings

# 3. Responsive design
vue-mcp/check_responsive_design filePath="[changed-component].vue"
# ✅ Required: Mobile-first patterns validated

# 4. Accessibility
htmlcss-mcp/check_html_accessibility workspacePath="frontend/Store" filePath="[changed-file].html"
# ✅ Required: Zero CRITICAL violations

# 5. XSS security
security-mcp/scan_xss_vulnerabilities workspacePath="frontend/Store"
# ✅ Required: Zero XSS vulnerabilities
```

#### Backend Changes
```bash
# 1. SQL injection detection
security-mcp/check_sql_injection workspacePath="backend"
# ✅ Required: Zero SQL injection vulnerabilities

# 2. Input validation
security-mcp/validate_input_sanitization workspacePath="backend"
# ✅ Required: All inputs validated

# 3. Authentication patterns
security-mcp/check_authentication workspacePath="backend"
# ✅ Required: Proper auth implementation

# Optional: Type analysis (enable Roslyn MCP if needed)
# roslyn-mcp/analyze_types workspacePath="backend"
```

#### All Changes
```bash
# Dependency security scan
security-mcp/scan_vulnerabilities workspacePath="."
# ✅ Required: No CRITICAL vulnerabilities
# ⚠️ Review: HIGH vulnerabilities require @Security approval
```

**MCP Validation Policy**:
- ❌ **BLOCK PR** if any CRITICAL MCP issues detected
- ⚠️ **REQUIRE JUSTIFICATION** for SERIOUS/HIGH issues
- ✅ **DOCUMENT** plan for MODERATE issues
- ℹ️ **TRACK** MINOR issues for future cleanup

**MCP Results Template** (add to PR description):
```markdown
## MCP Analysis Results

### ✅ Passed
- TypeScript: 0 type errors
- Vue i18n: 0 hardcoded strings
- Accessibility: WCAG AAA compliant
- Security: 0 vulnerabilities

### ⚠️ Warnings
- [List any non-blocking warnings]

### 📋 Actions
- [Any follow-up tasks for future PRs]
```

---

### 1. Code Quality
- [ ] MCP validations passed (see above)
- [ ] Code is readable and well-structured
- [ ] Naming is clear and consistent
- [ ] No unnecessary complexity
- [ ] DRY principle followed

### 2. Logic & Correctness
- [ ] Logic is correct and handles edge cases
- [ ] Error handling is appropriate
- [ ] No potential bugs or issues

### 3. Security
- [ ] MCP security scans passed (SQL injection, XSS, input validation)
- [ ] No exposed secrets or credentials
- [ ] Input validation present where needed
- [ ] No SQL injection or XSS vulnerabilities (verified by Security MCP)
- [ ] Authorization checks in place
- [ ] Dependency vulnerabilities addressed (verified by Security MCP)

### 4. Performance
- [ ] No obvious performance issues
- [ ] Efficient algorithms used
- [ ] Database queries optimized

### 5. Testing
- [ ] Tests cover critical paths
- [ ] Tests are meaningful and maintainable

## Output Format

Provide review as:

```
## Summary
[One sentence overall assessment]

## MCP Validation Status
✅ All automated MCP checks passed
OR
❌ MCP validation failures detected (see details below)

[Include MCP results from PR description or re-run tools]

## Approval Status
[✅ Approved | ⚠️ Approved with Comments | ❌ Changes Requested]

**Note**: PRs with failed MCP validations cannot be approved until issues are resolved.

## Critical Issues (Blocking)
- [MCP-flagged CRITICAL issues]
- [Manual review blocking issues]

## Suggestions (Non-blocking)
- [Suggestion 1]
- [Suggestion 2]

## Positive Notes
- [What was done well]

## Next Steps
- [Actions required before merge]
- [Follow-up tasks for future PRs]
```

---

## References

- [KB-053] TypeScript MCP Integration Guide
- [KB-054] Vue MCP Integration Guide
- [KB-055] Security MCP Best Practices
- [KB-056] HTML/CSS MCP Usage Guide
- [mcp-operations.instructions.md] MCP Operations Guide
