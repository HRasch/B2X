# ArchUnitNET - Architecture Testing Framework

**Last Updated**: 2. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current  
**DocID**: `KB-020`

---

## Official Resources

- **GitHub Repository**: [TNG/ArchUnitNET](https://github.com/TNG/ArchUnitNET) (1.2k+ ⭐)
- **Official Documentation**: [archunitnet.readthedocs.io](https://archunitnet.readthedocs.io/)
- **NuGet Package**: [TngTech.ArchUnitNET.xUnit](https://www.nuget.org/packages/TngTech.ArchUnitNET.xUnit/)
- **License**: Apache 2.0 (✅ commercial use allowed)

---

## Quick Reference

| Aspect | Details |
|--------|---------|
| **Purpose** | Automated architecture testing for .NET |
| **Version** | 0.13.1 (current in B2Connect) |
| **Test Framework** | xUnit integration |
| **.NET Support** | .NET 6, 7, 8, 9, 10 |
| **Pattern** | Fluent API for readable rules |
| **Performance** | < 30 seconds for full architecture scan |

---

## Installation

### NuGet Package (Central Package Management)

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="TngTech.ArchUnitNET.xUnit" Version="0.13.1" />
```

### Project Reference

```xml
<!-- .csproj -->
<PackageReference Include="TngTech.ArchUnitNET.xUnit" />
```

---

## Core Concepts

### Architecture Loading

```csharp
using ArchUnitNET.Loader;
using ArchUnitDomain = ArchUnitNET.Domain;

// Load architecture once (expensive operation)
protected static readonly ArchUnitDomain.Architecture Architecture = new ArchLoader()
    .LoadAssemblies(
        typeof(MyEntity).Assembly,           // Domain
        typeof(MyService).Assembly,          // Application
        typeof(MyRepository).Assembly)       // Infrastructure
    .Build();
```

### Fluent Rule Definition

```csharp
using ArchUnitNET.Fluent;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

// Type-based rules
Types().That().ResideInNamespace("*.Core.*")
    .Should().NotDependOnAny(
        Types().That().ResideInNamespace("*.Infrastructure.*"));

// Class-based rules
Classes().That().HaveNameEndingWith("Handler")
    .Should().ResideInNamespace("*.Handlers.*");
```

---

## Common Rule Patterns

### 1. Layer Dependency (Clean Architecture)

```csharp
[Fact]
public void Domain_Core_Should_Not_Depend_On_Infrastructure()
{
    var rule = Types()
        .That().ResideInNamespace(".*\\.Core\\.", useRegularExpressions: true)
        .Should().NotDependOnAny(
            Types().That().ResideInNamespace(".*\\.Infrastructure\\.", useRegularExpressions: true))
        .Because("Domain Core must be independent of infrastructure concerns");

    rule.Check(Architecture);
}
```

### 2. Bounded Context Isolation

```csharp
[Fact]
public void Catalog_Should_Not_Depend_On_CMS()
{
    var rule = Types()
        .That().ResideInNamespace("B2Connect.Catalog.*", useRegularExpressions: true)
        .Should().NotDependOnAny(
            Types().That().ResideInNamespace("B2Connect.CMS.*", useRegularExpressions: true))
        .Because("Bounded contexts must be isolated");

    rule.Check(Architecture);
}
```

### 3. Naming Conventions

```csharp
[Fact]
public void Handlers_Should_Have_Handler_Suffix()
{
    var rule = Classes()
        .That().ResideInNamespace(".*\\.Handlers\\.", useRegularExpressions: true)
        .And().AreNotAbstract()
        .Should().HaveNameEndingWith("Handler")
        .Because("All message handlers must follow naming convention");

    rule.Check(Architecture);
}
```

### 4. Framework Isolation

```csharp
[Fact]
public void Domain_Should_Not_Depend_On_EntityFramework()
{
    var rule = Types()
        .That().ResideInNamespace(".*\\.Core\\.", useRegularExpressions: true)
        .Should().NotDependOnAny(
            Types().That().ResideInNamespace("Microsoft.EntityFrameworkCore.*", useRegularExpressions: true))
        .Because("Domain must be persistence-ignorant");

    rule.Check(Architecture);
}
```

### 5. Multiple Conditions (Or)

```csharp
[Fact]
public void Controllers_Should_Have_Controller_Or_Endpoint_Suffix()
{
    var rule = Classes()
        .That().ResideInNamespace(".*\\.Controllers\\.", useRegularExpressions: true)
        .Should().HaveNameEndingWith("Controller")
        .OrShould().HaveNameEndingWith("Endpoint")
        .Because("API controllers must follow naming convention");

    rule.Check(Architecture);
}
```

### 6. Exclusions

```csharp
[Fact]
public void Services_Should_Have_Service_Suffix()
{
    var rule = Classes()
        .That().ResideInNamespace(".*\\.Services\\.", useRegularExpressions: true)
        .And().AreNotAbstract()
        .And().DoNotHaveNameContaining("Base")
        .And().DoNotHaveNameContaining("Factory")
        .And().DoNotHaveNameContaining("Options")
        .Should().HaveNameEndingWith("Service");

    rule.Check(Architecture);
}
```

---

## B2Connect Implementation

### Project Structure

```
backend/Tests/B2Connect.Architecture.Tests/
├── B2Connect.Architecture.Tests.csproj
├── ArchitectureTestBase.cs      ← Shared architecture loading
├── LayerDependencyTests.cs      ← Clean architecture rules
├── BoundedContextTests.cs       ← BC isolation rules
├── NamingConventionTests.cs     ← Handler/Event/Command naming
└── WolverinePatternTests.cs     ← CQRS pattern rules
```

### Architecture Base Class

```csharp
public abstract class ArchitectureTestBase
{
    protected static readonly ArchUnitDomain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Catalog.Core.Entities.TaxRate).Assembly,
            typeof(CMS.Core.Domain.Pages.PageDefinition).Assembly,
            typeof(B2Connect.AuthService.Data.AppUser).Assembly,
            typeof(LocalizationService.Models.LocalizedString).Assembly,
            typeof(Domain.Search.Models.ProductDocument).Assembly,
            typeof(B2Connect.Types.Domain.Entity).Assembly)
        .Build();

    protected static class BoundedContexts
    {
        public const string Catalog = "B2Connect.Catalog";
        public const string CMS = "B2Connect.CMS";
        public const string Identity = "B2Connect.AuthService";
        public const string Localization = "B2Connect.LocalizationService";
        public const string Search = "B2Connect.Domain.Search";
    }
}
```

### Test Categories

| Test File | Purpose | Rule Count |
|-----------|---------|------------|
| `LayerDependencyTests` | Clean/Onion architecture enforcement | 5 |
| `BoundedContextTests` | BC isolation (no cross-BC dependencies) | 8 |
| `NamingConventionTests` | Handler, Validator, Controller naming | 5 |
| `WolverinePatternTests` | CQRS handler patterns | 5 |

---

## Fluent API Reference

### Type Selectors

| Method | Description |
|--------|-------------|
| `Types()` | All types (classes, interfaces, structs) |
| `Classes()` | Only classes |
| `Interfaces()` | Only interfaces |
| `Attributes()` | Only attributes |

### Predicates (`.That()`)

| Method | Description |
|--------|-------------|
| `.ResideInNamespace(pattern)` | Match namespace pattern |
| `.HaveNameEndingWith(suffix)` | Name ends with |
| `.HaveNameStartingWith(prefix)` | Name starts with |
| `.HaveNameContaining(text)` | Name contains |
| `.AreAbstract()` | Abstract types only |
| `.AreNotAbstract()` | Non-abstract types |
| `.ImplementInterface(type)` | Implements interface |
| `.HaveAnyAttribute(attribute)` | Has attribute |

### Conditions (`.Should()`)

| Method | Description |
|--------|-------------|
| `.NotDependOnAny(types)` | No dependencies to |
| `.OnlyDependOn(types)` | Only allowed dependencies |
| `.ResideInNamespace(pattern)` | Must be in namespace |
| `.HaveNameEndingWith(suffix)` | Name must end with |
| `.NotHaveAnyDependency()` | No dependencies at all |
| `.BePublic()` | Must be public |
| `.BeInternal()` | Must be internal |

### Modifiers

| Method | Description |
|--------|-------------|
| `.And()` | Additional predicate |
| `.Or()` | Alternative predicate |
| `.OrShould()` | Alternative condition |
| `.AndShould()` | Additional condition |
| `.Because(reason)` | Error message with reason |

---

## Best Practices

### 1. Load Architecture Once

```csharp
// ✅ Good: Static field, loaded once
protected static readonly Architecture Arch = new ArchLoader()
    .LoadAssemblies(...).Build();

// ❌ Bad: Loading in each test
[Fact]
public void Test()
{
    var arch = new ArchLoader().LoadAssemblies(...).Build(); // Slow!
}
```

### 2. Use Regex for Flexible Matching

```csharp
// ✅ Good: Regex for wildcard matching
.ResideInNamespace(".*\\.Core\\.", useRegularExpressions: true)

// ⚠️ Limited: Exact match only
.ResideInNamespace("MyApp.Core")
```

### 3. Use `.Because()` for Clear Errors

```csharp
// ✅ Good: Clear error message
.Because("Domain Core must be persistence-ignorant (ADR-002)")

// ❌ Bad: No explanation
.Check(Architecture); // Unclear why it fails
```

### 4. Test Collection for Parallel Safety

```csharp
// Prevent parallel execution issues with shared Architecture
[Collection("Architecture")]
public class LayerDependencyTests : ArchitectureTestBase
```

### 5. Exclude Test Namespaces

```csharp
.And().DoNotResideInNamespace(".*Test.*", useRegularExpressions: true)
.And().DoNotResideInNamespace(".*Mock.*", useRegularExpressions: true)
```

---

## Common Issues & Solutions

### Issue: Slow Test Execution

**Cause**: Loading architecture in each test  
**Solution**: Use static architecture field in base class

### Issue: False Positives

**Cause**: Test types being analyzed  
**Solution**: Exclude test namespaces from rules

### Issue: Regex Not Matching

**Cause**: Missing `useRegularExpressions: true`  
**Solution**: Always specify when using patterns with `.*`

### Issue: Missing Types

**Cause**: Assembly not loaded  
**Solution**: Add marker type from assembly to `LoadAssemblies()`

---

## Integration with CI/CD

### Run Architecture Tests

```bash
# Run only architecture tests
dotnet test backend/Tests/B2Connect.Architecture.Tests -v minimal

# Run as part of full test suite
dotnet test B2Connect.slnx --filter "Category=Architecture"
```

### VS Code Task

```json
{
  "label": "test-architecture",
  "command": "dotnet",
  "type": "shell",
  "args": [
    "test",
    "${workspaceFolder}/backend/Tests/B2Connect.Architecture.Tests/B2Connect.Architecture.Tests.csproj",
    "-v", "minimal"
  ],
  "group": "test"
}
```

---

## Related Documentation

- **ADR-021**: [ArchUnitNET for Automated Architecture Testing](../../decisions/ADR-021-archunitnet-architecture-testing.md)
- **ADR-002**: Onion Architecture
- **ADR-001**: Event-Driven Architecture
- **KB-006**: [Wolverine Patterns](../wolverine.md)

---

## Alternatives Considered

| Tool | Why Not Chosen |
|------|----------------|
| **NetArchTest** | Less active, fewer features |
| **NDepend** | Commercial license, overkill |
| **Roslyn Analyzers** | Complex to write, better for code style |
| **Manual Reviews** | Doesn't scale, human error prone |

---

## Version History

| Version | .NET Support | Key Changes |
|---------|--------------|-------------|
| 0.13.1 | .NET 6-10 | Current B2Connect version |
| 0.13.0 | .NET 6-10 | Performance improvements |
| 0.12.x | .NET 6-9 | Initial stable release |

---

**Next Review**: Monthly (check for new releases)
