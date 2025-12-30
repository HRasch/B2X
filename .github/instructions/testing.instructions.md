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
