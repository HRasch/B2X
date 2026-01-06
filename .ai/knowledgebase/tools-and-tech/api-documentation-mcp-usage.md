# API Documentation MCP Usage Guide

**DocID**: `KB-059`
**Title**: API Documentation MCP Usage Guide
**Owner**: @Backend
**Status**: Active
**Last Updated**: 6. Januar 2026

---

## Overview

The API Documentation MCP provides comprehensive OpenAPI specification validation, API contract management, and documentation quality assurance for the B2Connect project. It ensures API consistency, prevents breaking changes, and maintains high-quality documentation across all API endpoints.

---

## Supported Frameworks

- **ASP.NET Core**: Web API controllers and minimal APIs
- **OpenAPI/Swagger**: Specification validation and generation
- **API Versioning**: Semantic versioning support
- **Contract Testing**: Consumer-driven contract validation

---

## Core Commands

### OpenAPI Validation

```bash
# Validate OpenAPI specifications
api-mcp/validate_openapi workspacePath="backend"

# Validate specific API version
api-mcp/validate_openapi workspacePath="backend" version="v1.0"

# Validate with custom rules
api-mcp/validate_openapi workspacePath="backend" ruleset="strict"
```

**Validates**:
- OpenAPI specification syntax and structure
- Schema definitions and data types
- Parameter and response definitions
- Security scheme configurations
- API versioning compliance

### API Contract Validation

```bash
# Validate API contracts
api-mcp/validate_contracts workspacePath="backend"

# Validate specific service contracts
api-mcp/validate_contracts workspacePath="backend/Gateway/Store"

# Validate consumer contracts
api-mcp/validate_contracts workspacePath="." consumer="frontend"
```

**Validates**:
- Request/response contract consistency
- Data transfer object definitions
- API endpoint signatures
- Consumer expectations alignment
- Backward compatibility

### Breaking Change Detection

```bash
# Check for breaking changes
api-mcp/check_breaking_changes workspacePath="backend" baseline="main"

# Analyze specific version changes
api-mcp/check_breaking_changes workspacePath="backend" from="v1.0" to="v2.0"

# Generate breaking change report
api-mcp/check_breaking_changes workspacePath="backend" report="html"
```

**Detects**:
- Removed or renamed endpoints
- Changed parameter types
- Modified response schemas
- Security requirement changes
- Deprecation notices

### Documentation Completeness

```bash
# Check documentation completeness
api-mcp/check_documentation workspacePath="backend"

# Validate specific documentation areas
api-mcp/check_documentation workspacePath="backend" area="endpoints"

# Generate completeness report
api-mcp/check_documentation workspacePath="backend" coverage="true"
```

**Validates**:
- Endpoint descriptions and summaries
- Parameter documentation
- Response examples
- Error response documentation
- Authentication requirements

### Schema Validation

```bash
# Validate API schemas
api-mcp/validate_schemas workspacePath="backend"

# Validate specific schema components
api-mcp/validate_schemas workspacePath="backend" component="Product"

# Cross-reference schema validation
api-mcp/validate_schemas workspacePath="backend" crossRef="true"
```

**Validates**:
- JSON Schema compliance
- Data type consistency
- Required field definitions
- Validation rules
- Schema references

### API Versioning Analysis

```bash
# Analyze API versioning
api-mcp/analyze_versioning workspacePath="backend"

# Check version compatibility
api-mcp/analyze_versioning workspacePath="backend" compatibility="true"

# Generate versioning report
api-mcp/analyze_versioning workspacePath="backend" report="json"
```

**Analyzes**:
- Version numbering consistency
- Deprecation timelines
- Migration paths
- Version-specific features
- Sunset policies

### Client SDK Generation

```bash
# Generate client SDKs
api-mcp/generate_sdk workspacePath="backend" language="typescript"

# Generate multiple language SDKs
api-mcp/generate_sdk workspacePath="backend" languages="typescript,csharp,java"

# Generate SDK with custom configuration
api-mcp/generate_sdk workspacePath="backend" config="sdk-config.json"
```

**Generates**:
- TypeScript client libraries
- C# client SDKs
- Java client libraries
- SDK documentation
- Integration examples

---

## Integration with Development Workflow

### Pre-Commit API Validation

```bash
# Run API validation on changed files
git diff --name-only | while read file; do
  case "$file" in
    *Controller*.cs) api-mcp/validate_openapi workspacePath="backend" ;;
    *Api*.cs) api-mcp/validate_contracts workspacePath="backend" ;;
    *.yaml) api-mcp/validate_schemas workspacePath="backend" ;;
  esac
done
```

### CI/CD Integration

```yaml
# .github/workflows/api-validation.yml
name: API Validation
on: [push, pull_request]

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run API Documentation MCP
        run: |
          api-mcp/validate_openapi workspacePath="backend"
          api-mcp/validate_contracts workspacePath="backend"
          api-mcp/check_breaking_changes workspacePath="backend" baseline="main"
          api-mcp/check_documentation workspacePath="backend"
          api-mcp/validate_schemas workspacePath="backend"
```

### Quality Gates

**Mandatory Checks**:
- OpenAPI specification validation (blocks deployment)
- API contract validation (blocks deployment)
- Breaking change detection (warnings for major versions)
- Documentation completeness (80% minimum)
- Schema validation (blocks deployment)

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "api-mcp": {
      "disabled": false,
      "config": {
        "openapi": {
          "version": "3.0.3",
          "validateExamples": true,
          "checkSecurity": true
        },
        "contracts": {
          "validateConsumers": true,
          "checkCompatibility": true
        },
        "breakingChanges": {
          "baselineBranch": "main",
          "strictMode": false
        },
        "documentation": {
          "minCoverage": 80,
          "requireExamples": true
        },
        "schemas": {
          "validateReferences": true,
          "checkConsistency": true
        },
        "versioning": {
          "semanticVersioning": true,
          "deprecationPeriod": 180
        }
      }
    }
  }
}
```

### Environment Variables

```bash
# API specification settings
OPENAPI_VERSION="3.0.3"
API_BASE_URL="https://api.b2connect.com"

# Documentation settings
DOCS_MIN_COVERAGE="80"
REQUIRE_EXAMPLES="true"

# Versioning settings
SEMANTIC_VERSIONING="true"
DEPRECATION_DAYS="180"

# SDK generation settings
SDK_LANGUAGES="typescript,csharp"
SDK_OUTPUT_DIR="./generated-sdks"
```

---

## Best Practices

### API Design

1. **Consistent Naming**: Use RESTful naming conventions
2. **Versioning Strategy**: Implement semantic versioning
3. **Documentation First**: Design APIs with documentation in mind
4. **Contract Testing**: Validate contracts early and often
5. **Breaking Changes**: Communicate changes clearly

### OpenAPI Specification

1. **Complete Definitions**: Provide comprehensive schema definitions
2. **Security Schemes**: Document authentication requirements
3. **Response Examples**: Include realistic response examples
4. **Error Responses**: Document all possible error conditions
5. **Deprecation Notices**: Mark deprecated endpoints clearly

### Documentation Standards

1. **Endpoint Descriptions**: Provide clear, concise descriptions
2. **Parameter Documentation**: Explain all parameters and their purposes
3. **Response Documentation**: Document all response codes and schemas
4. **Authentication**: Clearly document auth requirements
5. **Examples**: Provide practical usage examples

### Versioning Strategy

1. **Semantic Versioning**: Use MAJOR.MINOR.PATCH format
2. **Backward Compatibility**: Maintain compatibility within major versions
3. **Deprecation Process**: Provide migration timelines
4. **Version Headers**: Support version negotiation
5. **Documentation**: Document version differences

---

## API Contract Examples

### Controller Definition

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Retrieves a product by ID
    /// </summary>
    /// <param name="id">The product identifier</param>
    /// <returns>A product object</returns>
    /// <response code="200">Returns the requested product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // Implementation
    }
}
```

### OpenAPI Specification

```yaml
openapi: 3.0.3
info:
  title: B2Connect API
  version: 1.0.0
  description: Product catalog and ordering API

paths:
  /api/v1/products/{id}:
    get:
      summary: Get product by ID
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      responses:
        '200':
          description: Successful response
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Product'
        '404':
          description: Product not found

components:
  schemas:
    Product:
      type: object
      required:
        - id
        - name
        - price
      properties:
        id:
          type: integer
        name:
          type: string
        price:
          type: number
          format: decimal
```

### Contract Testing

```csharp
// Consumer contract test
[Fact]
public async Task GetProduct_ReturnsExpectedContract()
{
    // Arrange
    var contract = new ProductContract
    {
        Id = 1,
        Name = "Test Product",
        Price = 29.99m
    };

    // Act
    var response = await _client.GetAsync("/api/v1/products/1");
    var product = await response.Content.ReadFromJsonAsync<ProductContract>();

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    product.ShouldNotBeNull();
    product.Id.ShouldBe(contract.Id);
    product.Name.ShouldBe(contract.Name);
    product.Price.ShouldBe(contract.Price);
}
```

---

## Troubleshooting

### Common API Issues

**OpenAPI Validation Failures**:
```bash
# Debug OpenAPI specification
api-mcp/validate_openapi workspacePath="backend" --verbose

# Check specific validation rules
api-mcp/validate_openapi workspacePath="backend" --rules
```

**Contract Validation Errors**:
```bash
# Validate specific contracts
api-mcp/validate_contracts workspacePath="backend" --service="catalog"

# Check consumer compatibility
api-mcp/validate_contracts workspacePath="." --consumers
```

**Breaking Change Detection**:
```bash
# Analyze breaking changes in detail
api-mcp/check_breaking_changes workspacePath="backend" --details

# Generate change report
api-mcp/check_breaking_changes workspacePath="backend" --report="json"
```

**Documentation Gaps**:
```bash
# Identify missing documentation
api-mcp/check_documentation workspacePath="backend" --missing

# Generate documentation coverage report
api-mcp/check_documentation workspacePath="backend" --coverage
```

---

## Advanced Features

### API Versioning Management

```bash
# Manage version lifecycles
api-mcp/analyze_versioning workspacePath="backend" --lifecycle

# Plan version migrations
api-mcp/analyze_versioning workspacePath="backend" --migration-plan
```

### Consumer-Driven Contracts

```bash
# Validate consumer expectations
api-mcp/validate_contracts workspacePath="." --consumer-driven

# Generate consumer contracts
api-mcp/validate_contracts workspacePath="." --generate-contracts
```

### API Governance

```bash
# Enforce API standards
api-mcp/validate_openapi workspacePath="backend" --governance

# Audit API compliance
api-mcp/validate_openapi workspacePath="backend" --audit
```

---

## Integration Examples

### ASP.NET Core Integration

```csharp
// Program.cs configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "B2Connect API",
        Version = "v1",
        Description = "Product catalog and ordering API"
    });

    // API Documentation MCP will validate this configuration
    options.EnableAnnotations();
    options.DescribeAllParametersInCamelCase();
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

### Client SDK Usage

```typescript
// Generated TypeScript SDK
import { ProductsApi, Product } from './generated-api';

const api = new ProductsApi({
  basePath: 'https://api.b2connect.com'
});

async function getProduct(id: number): Promise<Product> {
  try {
    const response = await api.getProduct(id);
    return response.data;
  } catch (error) {
    console.error('API Error:', error);
    throw error;
  }
}
```

### CI/CD Pipeline Integration

```yaml
# Complete API validation pipeline
name: API Quality Assurance
on: [push, pull_request]

jobs:
  api-validation:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: OpenAPI Validation
        run: api-mcp/validate_openapi workspacePath="backend"

      - name: Contract Validation
        run: api-mcp/validate_contracts workspacePath="backend"

      - name: Breaking Change Detection
        run: |
          api-mcp/check_breaking_changes workspacePath="backend" baseline="main"

      - name: Documentation Completeness
        run: api-mcp/check_documentation workspacePath="backend"

      - name: Schema Validation
        run: api-mcp/validate_schemas workspacePath="backend"

      - name: Generate SDKs
        run: api-mcp/generate_sdk workspacePath="backend" language="typescript"

      - name: Upload SDKs
        uses: actions/upload-artifact@v3
        with:
          name: generated-sdks
          path: ./generated-sdks/
```

---

## Related Documentation

- [ADR-025] Gateway-Service Communication Strategy
- [KB-055] Security MCP Best Practices
- [INS-001] Backend Instructions

---

**Maintained by**: @Backend  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026