---
applyTo: "**/*.test.*,**/*.spec.*,**/tests/**,**/__tests__/**"
---

# Testing Instructions

## Test Structure
- Use descriptive test names (describe what, not how)
- Follow Arrange-Act-Assert pattern
- One assertion per test (when practical)
- Group related tests with describe blocks

## Test Types
- **Unit Tests**: Test isolated functions/methods
- **Integration Tests**: Test component interactions
- **E2E Tests**: Test complete user flows

## Best Practices
- Test behavior, not implementation
- Use meaningful test data
- Avoid testing framework internals
- Mock external dependencies consistently

## Coverage Goals
- Critical paths: 100% coverage
- Business logic: >80% coverage
- UI components: Test user interactions

## Testing Policies
- Mandate test-driven development (TDD) policies for critical features, requiring tests before code implementation
- Implement automated test coverage policies with minimum thresholds and regular reporting
- Create policies for test environment standardization and data seeding to ensure reliable testing
- Establish QA sign-off requirements for all releases with clear acceptance criteria

## Test Naming Convention
```
describe('[Component/Function Name]', () => {
  it('should [expected behavior] when [condition]', () => {
    // Arrange
    // Act
    // Assert
  });
});
```

## Mocking
- Mock at boundaries (APIs, databases)
- Reset mocks between tests
- Use factories for test data
- Prefer dependency injection over global mocks
