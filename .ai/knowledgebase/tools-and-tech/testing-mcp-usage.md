# Testing MCP Usage Guide

**DocID**: `KB-058`
**Title**: Testing MCP Usage Guide
**Owner**: @QA
**Status**: Active
**Last Updated**: 6. Januar 2026

---

## Overview

The Testing MCP provides comprehensive test execution, analysis, and quality assurance tools for the B2Connect project. It ensures code quality through automated testing, coverage analysis, and performance validation across unit, integration, and end-to-end test suites.

---

## Supported Test Frameworks

- **xUnit**: Primary unit testing framework (.NET)
- **Jest**: JavaScript/TypeScript testing
- **Playwright**: End-to-end testing
- **Shouldly**: Assertion library
- **Moq**: Mocking framework

---

## Core Commands

### Test Execution

```bash
# Run all tests
testing-mcp/run_tests workspacePath="."

# Run specific test types
testing-mcp/run_tests workspacePath="." testType="unit"
testing-mcp/run_tests workspacePath="." testType="integration"
testing-mcp/run_tests workspacePath="." testType="e2e"

# Run tests for specific component
testing-mcp/run_tests workspacePath="backend/Domain/Catalog" testType="unit"
```

**Execution Options**:
- Parallel execution across test projects
- Selective test running by category/tags
- Environment-specific test execution
- Test result aggregation and reporting

### Coverage Analysis

```bash
# Analyze test coverage
testing-mcp/analyze_coverage workspacePath="." minCoverage="80"

# Generate coverage reports
testing-mcp/analyze_coverage workspacePath="." format="html,json,cobertura"

# Check specific file coverage
testing-mcp/analyze_coverage filePath="backend/Domain/Catalog/ProductService.cs"
```

**Coverage Metrics**:
- Line coverage percentage
- Branch coverage analysis
- Method coverage validation
- Uncovered code identification
- Coverage trend analysis

### Test Quality Validation

```bash
# Validate test quality
testing-mcp/validate_tests workspacePath="."

# Check specific test patterns
testing-mcp/validate_tests workspacePath="backend/Domain/Catalog/tests"
```

**Quality Checks**:
- Test naming conventions
- Test structure validation (Arrange-Act-Assert)
- Mock usage patterns
- Test isolation verification
- Flaky test detection

### Performance Testing

```bash
# Run performance tests
testing-mcp/run_performance_tests workspacePath="." baseline="previous"

# Performance regression analysis
testing-mcp/run_performance_tests workspacePath="." compare="main"

# Load testing
testing-mcp/run_performance_tests workspacePath="." loadTest="true" users="100"
```

**Performance Metrics**:
- Response time analysis
- Throughput measurement
- Memory usage tracking
- CPU utilization monitoring
- Regression detection

### Integration Testing

```bash
# Run integration tests
testing-mcp/run_integration_tests workspacePath="." environment="test"

# Test specific integrations
testing-mcp/run_integration_tests workspacePath="." service="database"
testing-mcp/run_integration_tests workspacePath="." service="elasticsearch"

# End-to-end testing
testing-mcp/run_integration_tests workspacePath="." e2e="true"
```

**Integration Validation**:
- Service communication testing
- Database integration verification
- External API testing
- Message queue validation
- Cross-service workflows

### Test Data Generation

```bash
# Generate test data (optional feature)
testing-mcp/generate_test_data workspacePath="." scenario="catalog-import"

# Generate mock data for specific tests
testing-mcp/generate_test_data workspacePath="backend/Domain/Catalog/tests" type="products"
```

**Data Generation**:
- Realistic test data creation
- Edge case scenario generation
- Multi-tenant test data
- Localized content generation
- Performance test data sets

### Test Reporting

```bash
# Generate comprehensive test reports
testing-mcp/generate_reports workspacePath="." format="html,json,junit"

# Generate coverage reports
testing-mcp/generate_reports workspacePath="." type="coverage" format="html"

# Generate performance reports
testing-mcp/generate_reports workspacePath="." type="performance" format="json"
```

**Report Formats**:
- HTML dashboards
- JSON data exports
- JUnit XML for CI/CD
- Coverage reports
- Performance trend charts

---

## Integration with Development Workflow

### Pre-Commit Testing

```bash
# Run tests on changed files
git diff --name-only | while read file; do
  case "$file" in
    *.cs) testing-mcp/run_tests workspacePath="backend" testType="unit" ;;
    *.vue) testing-mcp/run_tests workspacePath="frontend" testType="unit" ;;
    *.test.*) testing-mcp/validate_tests filePath="$file" ;;
  esac
done
```

### CI/CD Integration

```yaml
# .github/workflows/testing.yml
name: Testing Pipeline
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run Testing MCP
        run: |
          testing-mcp/validate_tests workspacePath="."
          testing-mcp/analyze_coverage workspacePath="." minCoverage="80"
          testing-mcp/run_tests workspacePath="." testType="unit"
          testing-mcp/run_integration_tests workspacePath="." environment="test"
          testing-mcp/generate_reports workspacePath="." format="html,json"
```

### Quality Gates

**Mandatory Checks**:
- Test quality validation (blocks PR merge)
- Minimum coverage requirements (80%)
- All unit tests passing
- Integration tests successful
- No performance regressions

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "testing-mcp": {
      "disabled": false,
      "config": {
        "testFrameworks": ["xunit", "jest", "playwright"],
        "coverage": {
          "minCoverage": 80,
          "excludePatterns": ["**/Migrations/**", "**/TestResults/**"]
        },
        "performance": {
          "baselineBranch": "main",
          "thresholds": {
            "responseTime": 1000,
            "memoryUsage": 100
          }
        },
        "reporting": {
          "formats": ["html", "json", "junit"],
          "outputDir": "./test-results"
        }
      }
    }
  }
}
```

### Environment Variables

```bash
# Test database connection
TEST_DATABASE_CONNECTION="Host=localhost;Database=b2connect_test"

# Test environment settings
ASPNETCORE_ENVIRONMENT="Testing"
TEST_ELASTICSEARCH_HOST="http://localhost:9200"

# Performance test settings
PERFORMANCE_TEST_USERS="50"
PERFORMANCE_TEST_DURATION="300"
```

---

## Best Practices

### Test Organization

1. **Test Structure**: Follow project conventions for test organization
2. **Naming Conventions**: Use descriptive test names (not just method names)
3. **Test Isolation**: Ensure tests don't depend on each other
4. **Mock Usage**: Use mocks for external dependencies
5. **Data Management**: Clean up test data between runs

### Coverage Goals

1. **Critical Paths**: 100% coverage for business logic
2. **New Code**: 90% coverage for new features
3. **Integration Points**: Full coverage for API endpoints
4. **Error Handling**: Test all error scenarios
5. **Edge Cases**: Cover boundary conditions

### Performance Testing

1. **Baseline Establishment**: Set performance baselines regularly
2. **Regression Detection**: Monitor for performance degradation
3. **Load Testing**: Test under realistic load conditions
4. **Resource Monitoring**: Track memory and CPU usage
5. **Scalability Testing**: Verify horizontal scaling capabilities

### Integration Testing

1. **Service Contracts**: Test API contracts between services
2. **Data Consistency**: Verify data integrity across services
3. **Error Scenarios**: Test failure modes and recovery
4. **Message Flows**: Validate asynchronous message processing
5. **External Dependencies**: Mock or stub external services

---

## Test Categories

### Unit Tests

```csharp
// Example: ProductService unit test
[Fact]
public async Task CreateProduct_ValidInput_ReturnsProduct()
{
    // Arrange
    var mockRepository = new Mock<IProductRepository>();
    var service = new ProductService(mockRepository.Object);
    var request = new CreateProductRequest { Name = "Test Product" };

    // Act
    var result = await service.CreateProductAsync(request);

    // Assert
    result.ShouldNotBeNull();
    result.Name.ShouldBe("Test Product");
}
```

**Unit Test Focus**:
- Individual method testing
- Logic validation
- Error condition handling
- Mock external dependencies

### Integration Tests

```csharp
// Example: Product API integration test
[Fact]
public async Task CreateProduct_ValidRequest_ReturnsCreatedProduct()
{
    // Arrange
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();

    var request = new CreateProductRequest
    {
        Name = "Integration Test Product",
        Price = 29.99m
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/products", request);
    var product = await response.Content.ReadFromJsonAsync<Product>();

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.Created);
    product.Name.ShouldBe("Integration Test Product");
}
```

**Integration Test Focus**:
- API endpoint testing
- Database operations
- Service interactions
- Authentication/authorization

### End-to-End Tests

```typescript
// Example: Product creation E2E test
test('user can create product', async ({ page }) => {
  // Arrange
  await page.goto('/admin/products');

  // Act
  await page.fill('[data-testid="product-name"]', 'E2E Test Product');
  await page.fill('[data-testid="product-price"]', '49.99');
  await page.click('[data-testid="create-product"]');

  // Assert
  await expect(page.locator('[data-testid="product-list"]')).toContainText('E2E Test Product');
});
```

**E2E Test Focus**:
- Complete user workflows
- UI interaction validation
- Cross-browser compatibility
- Performance under load

---

## Troubleshooting

### Common Test Issues

**Test Failures**:
```bash
# Debug failing tests
testing-mcp/run_tests workspacePath="." testName="CreateProduct_ValidInput_ReturnsProduct" --debug

# Check test dependencies
testing-mcp/validate_tests workspacePath="." --dependencies
```

**Coverage Issues**:
```bash
# Identify uncovered code
testing-mcp/analyze_coverage workspacePath="." --uncovered

# Exclude generated code
testing-mcp/analyze_coverage workspacePath="." excludePatterns="**/Generated/**"
```

**Performance Regressions**:
```bash
# Compare performance baselines
testing-mcp/run_performance_tests workspacePath="." compare="main" --details

# Profile slow tests
testing-mcp/run_performance_tests workspacePath="." --profile
```

**Integration Problems**:
```bash
# Test service connectivity
testing-mcp/run_integration_tests workspacePath="." --connectivity

# Debug message flows
testing-mcp/run_integration_tests workspacePath="." --trace
```

---

## Performance Monitoring

### Key Metrics

- **Test Execution Time**: Target <10 minutes for full suite
- **Coverage Percentage**: Maintain >80% overall coverage
- **Flaky Test Rate**: Target <1% flaky tests
- **Performance Regression**: <5% degradation allowed
- **Test Reliability**: >99% success rate

### Monitoring Commands

```bash
# Test suite health check
testing-mcp/run_tests workspacePath="." --health

# Coverage trends
testing-mcp/analyze_coverage workspacePath="." --trend

# Performance monitoring
testing-mcp/run_performance_tests workspacePath="." --monitor
```

---

## Advanced Features

### Test Data Management

```bash
# Generate comprehensive test datasets
testing-mcp/generate_test_data workspacePath="." scenario="full-catalog" count="1000"

# Create specific test scenarios
testing-mcp/generate_test_data workspacePath="." type="edge-cases" domain="pricing"
```

### AI-Assisted Testing

```bash
# Generate test cases for new features
testing-mcp/generate_test_data workspacePath="." feature="product-search" --ai

# Analyze test gaps
testing-mcp/validate_tests workspacePath="." --ai-suggestions
```

### Continuous Testing

```bash
# Run tests on file changes
testing-mcp/run_tests workspacePath="." --watch

# Incremental testing
testing-mcp/run_tests workspacePath="." --incremental
```

---

## Integration Examples

### Backend Testing Integration

```csharp
// Test collection definition
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}

[Collection("Database")]
public class ProductRepositoryTests
{
    private readonly DatabaseFixture _fixture;

    public ProductRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetById_ExistingProduct_ReturnsProduct()
    {
        // Test implementation
    }
}
```

### Frontend Testing Integration

```typescript
// Jest configuration
export default {
  testEnvironment: 'jsdom',
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts'],
  moduleNameMapping: {
    '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
  },
  collectCoverageFrom: [
    'src/**/*.{ts,tsx}',
    '!src/**/*.d.ts',
  ],
};
```

### CI/CD Pipeline Integration

```yaml
# Complete testing pipeline
name: Quality Assurance
on: [push, pull_request]

jobs:
  quality:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Test Quality Validation
        run: testing-mcp/validate_tests workspacePath="."

      - name: Coverage Analysis
        run: testing-mcp/analyze_coverage workspacePath="." minCoverage="80"

      - name: Unit Tests
        run: testing-mcp/run_tests workspacePath="." testType="unit"

      - name: Integration Tests
        run: testing-mcp/run_integration_tests workspacePath="." environment="test"

      - name: Performance Tests
        run: testing-mcp/run_performance_tests workspacePath="." baseline="main"

      - name: Generate Reports
        run: testing-mcp/generate_reports workspacePath="." format="html,json"

      - name: Upload Results
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: ./test-results/
```

---

## Related Documentation

- [ADR-020] PR Quality Gate
- [GL-005] SARAH Quality Gate Criteria
- [INS-003] Testing Instructions
- [KB-055] Security MCP Best Practices

---

**Maintained by**: @QA  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026