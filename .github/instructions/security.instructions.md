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

