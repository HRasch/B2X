# Security MCP Server

A Model Context Protocol (MCP) server for security analysis and OWASP compliance in the B2X project. This server provides AI-powered security scanning and vulnerability detection.

## Features

### 🔍 Vulnerability Scanning
- Scan dependencies for known vulnerabilities
- Check for outdated packages with security issues
- Monitor CVEs and security advisories

### 🗄️ SQL Injection Detection
- Analyze database queries for injection vulnerabilities
- Check parameterized query usage
- Validate input sanitization in data access layers

### 🛡️ Input Validation
- Check input sanitization patterns
- Validate allowlists vs denylists
- Ensure proper encoding of user inputs

### 🔐 Authentication & Authorization
- Check authentication implementations
- Validate authorization patterns
- Ensure principle of least privilege

### 🌐 XSS Vulnerability Scanning
- Scan frontend code for XSS vulnerabilities
- Check output encoding and sanitization
- Validate CSP (Content Security Policy) usage

## Installation

```bash
cd tools/SecurityMCP
npm install
npm run build
```

## Usage

### Development
```bash
npm run dev
```

### Production
```bash
npm run build
npm start
```

## MCP Tools

### `scan_vulnerabilities`
Scans for known vulnerabilities in project dependencies.

**Parameters:**
- `workspacePath`: Workspace root directory

### `check_sql_injection`
Checks for potential SQL injection vulnerabilities.

**Parameters:**
- `workspacePath`: Workspace root directory

### `validate_input_sanitization`
Validates input sanitization and validation patterns.

**Parameters:**
- `workspacePath`: Workspace root directory

### `check_authentication`
Checks authentication and authorization implementations.

**Parameters:**
- `workspacePath`: Workspace root directory

### `scan_xss_vulnerabilities`
Scans for potential XSS vulnerabilities in frontend code.

**Parameters:**
- `workspacePath`: Workspace root directory

## Integration with B2X

This MCP server is designed to work with the B2X security requirements:

- **OWASP Top Ten** compliance ([KB-010])
- **Input validation** following security instructions
- **Dependency management** per governance policies
- **Authentication** using established libraries
- **Data protection** with encryption and HTTPS

## Development Status

This is a **basic skeleton implementation**. Full security scanning features need to be implemented in future iterations.

## Next Steps

1. **Vulnerability Database Integration**: Connect to NVD, GitHub Security Advisories
2. **Code Analysis Engine**: Implement AST-based security scanning
3. **OWASP Rule Engine**: Add comprehensive OWASP Top Ten checks
4. **Reporting Integration**: Generate security reports and alerts
5. **CI/CD Integration**: Automate security scanning in pipelines
6. **False Positive Management**: Implement security scan result filtering

## Contributing

Follow B2X security guidelines and coordinate with @Security agent for feature additions.