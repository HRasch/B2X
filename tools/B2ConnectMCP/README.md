# B2Connect MCP Server

Custom Model Context Protocol server for B2Connect domain-specific validations and AI assistance.

## Overview

The B2Connect MCP server provides specialized tools for validating and analyzing B2Connect-specific configurations, domain models, and integration setups. It extends the development workflow with domain-aware intelligence.

## Tools

### validate_tenant_config
Validates tenant configuration files for required fields, domain format, and feature configurations.

```bash
b2connect-mcp/validate_tenant_config configPath="config/tenant.json"
```

### validate_catalog_structure
Validates product catalog structure and metadata for consistency and completeness.

```bash
b2connect-mcp/validate_catalog_structure catalogPath="backend/Domain/Catalog"
```

### check_erp_integration
Validates ERP integration configurations for supported systems and proper mappings.

```bash
b2connect-mcp/check_erp_integration erpConfigPath="connectors/enventa/config.json"
```

### analyze_domain_models
Analyzes domain models for consistency, patterns, and best practices.

```bash
b2connect-mcp/analyze_domain_models domainPath="backend/Domain"
```

### validate_lifecycle_stages
Validates customer integration lifecycle stages and progress tracking.

```bash
b2connect-mcp/validate_lifecycle_stages tenantId="tenant-123"
```

## Installation

```bash
cd tools/B2ConnectMCP
npm install
npm run build
```

## Usage

The server runs on stdio and integrates with VS Code MCP configurations.

## Development

```bash
npm run dev    # Development mode with ts-node
npm run build  # Build to dist/
npm run start  # Run built server
npm test       # Run tests
```

## Configuration

Add to `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "b2connect-mcp": {
      "command": "node",
      "args": ["tools/B2ConnectMCP/dist/index.js"],
      "disabled": false
    }
  }
}
```

## Integration with Development Workflow

### Pre-Commit Validation
```bash
# Validate tenant configs before commit
b2connect-mcp/validate_tenant_config configPath="config/tenants/"

# Check domain model consistency
b2connect-mcp/analyze_domain_models domainPath="backend/Domain"
```

### CI/CD Integration
```bash
# Validate catalog structure in CI
b2connect-mcp/validate_catalog_structure catalogPath="backend/Domain/Catalog"

# Check ERP integrations
b2connect-mcp/check_erp_integration erpConfigPath="connectors/"
```

### Development Assistance
```bash
# Get lifecycle stage guidance
b2connect-mcp/validate_lifecycle_stages tenantId="current-tenant"
```

## Dependencies

- `@modelcontextprotocol/sdk`: MCP protocol implementation
- `fast-glob`: File pattern matching
- `js-yaml`: YAML parsing
- `zod`: Schema validation

## Contributing

Follow the established patterns in other MCP servers in the `tools/` directory.

## License

MIT