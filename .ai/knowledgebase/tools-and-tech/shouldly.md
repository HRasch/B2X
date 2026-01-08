---
docid: KB-183
title: Shouldly
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Shouldly - Assertion Framework for .NET

**Last Updated**: 5. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current  
**DocID**: `KB-031`

---

## Official Resources

- **GitHub Repository**: [shouldly/shouldly](https://github.com/shouldly/shouldly) (3.2k+ ⭐)
- **Official Documentation**: [docs.shouldly.org](https://docs.shouldly.org/)
- **NuGet Package**: [Shouldly](https://www.nuget.org/packages/Shouldly/)
- **License**: BSD 3-Clause (✅ commercial use allowed)

---

## Quick Reference

| Aspect | Details |
|--------|---------|
| **Purpose** | Fluent assertion library for .NET testing |
| **Version** | 4.2.0 (current in B2X) |
| **Test Frameworks** | xUnit, NUnit, MSTest, custom |
| **.NET Support** | .NET Standard 2.0+, .NET 5+ |
| **Syntax** | `actual.ShouldBe(expected)` |
| **Performance** | Minimal overhead, fast execution |
| **IDE Integration** | Excellent IntelliSense support |

---

## Why Shouldly?

Shouldly provides more readable and meaningful assertion failures compared to traditional assertion libraries. Instead of generic messages like "Expected: True, Actual: False", Shouldly gives context-specific messages like "expected `result` to be `true` but was `false`".

### Key Benefits

1. **Better Error Messages**: Context-aware failure messages that tell you exactly what went wrong
2. **Fluent API**: Readable assertion syntax that reads like natural language
3. **Type Safety**: Strongly typed assertions prevent runtime errors
4. **Extensible**: Easy to add custom assertions for domain-specific needs
5. **Lightweight**: No external dependencies, fast compilation

---

## Basic Usage

### Equality Assertions

```csharp
// Instead of: Assert.Equal(expected, actual);
actual.ShouldBe(expected);

// Collections
list.ShouldContain(item);
list.ShouldNotBeEmpty();
list.Count.ShouldBe(expectedCount);
```

### Boolean Assertions

```csharp
result.ShouldBeTrue();
result.ShouldBeFalse();
```

### Null Assertions

```csharp
result.ShouldBeNull();
result.ShouldNotBeNull();
```

### String Assertions

```csharp
text.ShouldContain("substring");
text.ShouldStartWith("prefix");
text.ShouldEndWith("suffix");
text.ShouldBeNullOrEmpty();
```

### Numeric Assertions

```csharp
value.ShouldBeGreaterThan(0);
value.ShouldBeLessThan(100);
value.ShouldBeInRange(1, 10);
```

### Exception Assertions

```csharp
Should.Throw<ArgumentException>(() => method());
Should.NotThrow(() => method());
```

---

## Advanced Usage

### Custom Error Messages

```csharp
result.ShouldBe(expected, "Custom error message with context");
```

### Collection Assertions

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

numbers.ShouldBe(new[] { 1, 2, 3, 4, 5 }); // Exact match
numbers.ShouldContain(3);                    // Contains element
numbers.ShouldNotContain(6);                // Does not contain
numbers.Length.ShouldBe(5);                 // Property assertion
```

### Dictionary Assertions

```csharp
var dict = new Dictionary<string, int> { ["key"] = 42 };

dict.ShouldContainKey("key");
dict["key"].ShouldBe(42);
dict.ShouldNotContainKey("missing");
```

### DateTime Assertions

```csharp
var date = new DateTime(2024, 1, 1);

date.ShouldBeGreaterThan(new DateTime(2023, 12, 31));
date.Year.ShouldBe(2024);
```

---

## Integration with xUnit

Shouldly works seamlessly with xUnit and other test frameworks:

```csharp
[Fact]
public void Calculator_Add_ShouldReturnCorrectSum()
{
    // Arrange
    var calculator = new Calculator();

    // Act
    var result = calculator.Add(2, 3);

    // Assert
    result.ShouldBe(5);
}
```

---

## Best Practices

### 1. Use Descriptive Variable Names

```csharp
// Good
var actualResult = calculator.Add(2, 3);
actualResult.ShouldBe(5);

// Avoid
calculator.Add(2, 3).ShouldBe(5); // Less readable
```

### 2. Chain Related Assertions

```csharp
// Good - related assertions together
user.ShouldNotBeNull();
user.Name.ShouldBe("John Doe");
user.Age.ShouldBe(30);

// Avoid - mixing unrelated assertions
user.ShouldNotBeNull();
order.Total.ShouldBe(100.00m); // Unrelated
```

### 3. Use Appropriate Assertion Methods

```csharp
// Prefer specific assertions
list.ShouldNotBeEmpty();      // Clear intent
list.Count.ShouldBeGreaterThan(0); // Less clear

// Use exact matches when possible
result.ShouldBe(expected);    // Exact equality
result.ShouldBeOneOf(expected1, expected2); // Multiple valid values
```

### 4. Custom Assertions for Domain Logic

```csharp
public static class UserAssertions
{
    public static void ShouldBeActive(this User user)
    {
        user.ShouldNotBeNull();
        user.IsActive.ShouldBeTrue($"User {user.Name} should be active");
        user.LastLogin.ShouldBeGreaterThan(DateTime.Now.AddDays(-30));
    }
}

// Usage
user.ShouldBeActive();
```

---

## Common Patterns in B2X

Based on current usage in the codebase:

### Repository Testing

```csharp
[Fact]
public async Task GetById_ShouldReturnEntity_WhenExists()
{
    // Arrange
    var entity = new TestEntity { Id = 1, Name = "Test" };

    // Act
    var result = await repository.GetByIdAsync(1);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(1);
    result.Name.ShouldBe("Test");
}
```

### Service Testing

```csharp
[Fact]
public async Task ProcessOrder_ShouldSucceed_WhenValid()
{
    // Arrange
    var order = CreateValidOrder();

    // Act
    var result = await service.ProcessOrderAsync(order);

    // Assert
    result.IsSuccess.ShouldBeTrue();
    result.Value.Status.ShouldBe(OrderStatus.Processing);
}
```

### Validation Testing

```csharp
[Fact]
public void ValidateEmail_ShouldPass_WhenValid()
{
    // Arrange
    var validator = new EmailValidator();

    // Act
    var result = validator.Validate("user@example.com");

    // Assert
    result.IsValid.ShouldBeTrue();
    result.Errors.ShouldBeEmpty();
}
```

---

## Migration from Other Assertion Libraries

### From FluentAssertions

```csharp
// FluentAssertions
result.Should().Be(expected);
result.Should().NotBeNull();
result.Should().HaveCount(5);

// Shouldly
result.ShouldBe(expected);
result.ShouldNotBeNull();
result.Count.ShouldBe(5);
```

### From xUnit Assert

```csharp
// xUnit Assert
Assert.Equal(expected, result);
Assert.NotNull(result);
Assert.True(condition);

// Shouldly
result.ShouldBe(expected);
result.ShouldNotBeNull();
condition.ShouldBeTrue();
```

---

## Performance Considerations

- **Compilation**: Minimal impact on build times
- **Runtime**: Negligible performance overhead
- **Memory**: Low memory footprint
- **IDE**: Excellent IntelliSense and refactoring support

---

## Troubleshooting

### Common Issues

1. **"Should" extension method not found**
   - Ensure `using Shouldly;` is added
   - Check that Shouldly NuGet package is installed

2. **Ambiguous reference errors**
   - May conflict with other assertion libraries
   - Remove conflicting packages (e.g., FluentAssertions)

3. **Custom assertion compilation errors**
   - Ensure extension methods are `public static`
   - Return type should be `void` for assertions

### Debugging Assertion Failures

Shouldly provides detailed error messages. For complex objects, consider:

```csharp
// For complex object comparison
actual.ShouldBe(expected, "Detailed context about the failure");

// For partial object validation
actual.Id.ShouldBe(expected.Id);
actual.Name.ShouldBe(expected.Name, "Name should match the expected value");
```

---

## Related Documentation

- [xUnit Documentation](https://xunit.net/) - Test framework used with Shouldly
- [Testing Best Practices](../best-practices/testing.md) - General testing guidelines
- [FluentAssertions Migration](fluent-assertions-migration.md) - Migration guide from FluentAssertions

---

**Integration Status**: ✅ Fully integrated in B2X test suite  
**Maintenance**: Low - stable, well-maintained library  
**Recommendation**: Preferred assertion library for new tests</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2X/.ai/knowledgebase/tools-and-tech/shouldly.md