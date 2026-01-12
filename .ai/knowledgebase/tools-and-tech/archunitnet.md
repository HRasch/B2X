---
docid: KB-020
title: ArchUnitNET - Architecture Testing Framework
owner: @TechLead
status: Active
created: 2026-01-08
updated: 2026-01-11
---

# ArchUnitNET - Architecture Testing Framework

**Last Updated**: 11. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current  
**DocID**: `KB-020`

---

## Overview

ArchUnitNET is a free, simple library for checking the architecture of C# code. It is the C# fork of [ArchUnit Java](https://www.archunit.org/) that enables automated architecture testing by analyzing C# bytecode and importing all classes into a code structure. The main focus is to automatically test architecture and coding rules.

---

## Official Resources

- **GitHub Repository**: [TNG/ArchUnitNET](https://github.com/TNG/ArchUnitNET) (1.2k+ ⭐, 34 contributors)
- **Official Documentation**: [archunitnet.readthedocs.io](https://archunitnet.readthedocs.io/)
  - **Latest Version**: https://archunitnet.readthedocs.io/en/latest/
  - **Stable Version**: https://archunitnet.readthedocs.io/en/stable/
- **NuGet Packages**:
  - Core: [TngTech.ArchUnitNET](https://www.nuget.org/packages/TngTech.ArchUnitNET/)
  - xUnit: [TngTech.ArchUnitNET.xUnit](https://www.nuget.org/packages/TngTech.ArchUnitNET.xUnit/)
  - xUnit V3: [TngTech.ArchUnitNET.xUnitV3](https://www.nuget.org/packages/TngTech.ArchUnitNET.xUnitV3/)
  - NUnit: [TngTech.ArchUnitNET.NUnit](https://www.nuget.org/packages/TngTech.ArchUnitNET.NUnit/)
  - MSTest V2: [TngTech.ArchUnitNET.MSTestV2](https://www.nuget.org/packages/TngTech.ArchUnitNET.MSTestV2/)
  - TUnit: [TngTech.ArchUnitNET.TUnit](https://www.nuget.org/packages/TngTech.ArchUnitNET.TUnit/) (new)
- **License**: Apache 2.0 (✅ commercial use allowed)
- **Maintainer**: [TNG Technology Consulting GmbH](https://www.tngtech.com/)

---

## Quick Reference

| Aspect | Details |
|--------|---------|
| **Purpose** | Automated architecture testing for .NET |
| **Current Version** | 0.13.1 (latest stable) |
| **B2X Version** | 0.13.1 |
| **Test Frameworks** | xUnit, xUnit V3, NUnit, MSTest V2, TUnit |
| **.NET Support** | .NET Framework 4.6.2+, .NET Core 6, 7, 8, 9, 10 |
| **Pattern** | Fluent API for readable rules |
| **Performance** | < 30 seconds for full architecture scan |
| **C# Version** | 99.7% of codebase |
| **Last Release** | 0.13.1 (2 days ago) |
| **Total Releases** | 55+ versions

---

## Installation

### Test Framework Support

ArchUnitNET offers framework-specific extensions for seamless integration:

| Framework | Package | Status | Purpose |
|-----------|---------|--------|---------|
| **xUnit** | `TngTech.ArchUnitNET.xUnit` | Stable | ✅ B2X Standard |
| **xUnit V3** | `TngTech.ArchUnitNET.xUnitV3` | Stable | **B2X Standard** (.NET 10) |
| **NUnit** | `TngTech.ArchUnitNET.NUnit` | Stable | NUnit integration |
| **MSTest V2** | `TngTech.ArchUnitNET.MSTestV2` | Stable | Visual Studio test explorer |
| **TUnit** | `TngTech.ArchUnitNET.TUnit` | New | Modern .NET testing |

### NuGet Package (Central Package Management)

**For B2X (xUnit)**:

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="TngTech.ArchUnitNET.xUnit" Version="0.13.1" />
```

**Alternative frameworks**:

```xml
<!-- For other test frameworks -->
<PackageVersion Include="TngTech.ArchUnitNET.NUnit" Version="0.13.1" />
<PackageVersion Include="TngTech.ArchUnitNET.MSTestV2" Version="0.13.1" />
<PackageVersion Include="TngTech.ArchUnitNET.TUnit" Version="0.13.1" />
<PackageVersion Include="TngTech.ArchUnitNET" Version="0.13.1" />
```

### Project Reference

```xml
<!-- .csproj -->
<PackageReference Include="TngTech.ArchUnitNET.xUnit" />
```

### Command Line Installation

```bash
# B2X standard (xUnit)
dotnet add package TngTech.ArchUnitNET.xUnit

# Other frameworks
dotnet add package TngTech.ArchUnitNET.NUnit
dotnet add package TngTech.ArchUnitNET.MSTestV2
dotnet add package TngTech.ArchUnitNET.TUnit
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
        .That().ResideInNamespace("B2X.Catalog.*", useRegularExpressions: true)
        .Should().NotDependOnAny(
            Types().That().ResideInNamespace("B2X.CMS.*", useRegularExpressions: true))
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

## B2X Implementation

### Project Structure

```
backend/Tests/B2X.Architecture.Tests/
├── B2X.Architecture.Tests.csproj
├── ArchitectureTestBase.cs      ← Shared architecture loading
├── LayerDependencyTests.cs      ← Clean architecture rules
├── BoundedContextTests.cs       ← BC isolation rules
├── NamingConventionTests.cs     ← Handler/Event/Command naming
└── WolverinePatternTests.cs     ← CQRS pattern rules
```

### Architecture Base Class

> ⚠️ **xUnit V3 Note**: For xUnit V3, use `using ArchUnitNET.xUnitV3;` (NOT `xUnit`). Also add `using Xunit;` for `[Fact]` attributes.

```csharp
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;  // For xUnit V3 - provides Check() extension
using Xunit;                 // Required for [Fact] attribute
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace B2X.ArchitectureTests;  // Avoid 'Architecture' in namespace name

public abstract class ArchitectureTestBase
{
    // Use unique field name to avoid conflict with ArchUnitNET.Domain.Architecture type
    protected static readonly Architecture B2XArchitecture = new ArchLoader()
        .LoadAssemblies(
            typeof(B2X.Catalog.Core.Entities.TaxRate).Assembly,
            typeof(B2X.CMS.Core.Domain.Pages.PageDefinition).Assembly,
            typeof(B2X.Identity.Data.AppUser).Assembly,
            typeof(B2X.LocalizationService.Models.LocalizedString).Assembly,
            typeof(B2X.Search.SearchResult<>).Assembly,
            typeof(B2X.Types.Domain.Entity).Assembly)
        .Build();

    protected static class BoundedContexts
    {
        public const string Catalog = "B2X.Catalog";
        public const string CMS = "B2X.CMS";
        public const string Identity = "B2X.Identity";           // NOT AuthService
        public const string Localization = "B2X.LocalizationService";
        public const string Search = "B2X.Search";               // NOT Domain.Search
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

## Troubleshooting & Debug Artifacts

### Why Tests Fail in Release Mode

ArchUnitNET analyzes .NET bytecode to build a code structure representing your architecture. In Release mode, the compiler performs aggressive optimizations that can remove or obfuscate information needed for accurate analysis:

- **Inlining**: Methods are merged into callers, losing dependency information
- **Dead code elimination**: Unused code is removed
- **Optimizations**: References may be altered or removed

**Solution**: Always run ArchUnitNET tests in `Debug` configuration:

```bash
dotnet test *.csproj -c Debug
```

### Common Issues & Solutions

#### Issue: Slow Test Execution

**Cause**: Loading architecture in each test  
**Solution**: Use static architecture field in base class (loaded once at class initialization)

```csharp
// ✅ Correct: Static field, loaded once
public abstract class ArchitectureTestBase
{
    protected static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(...).Build();
}

// ❌ Wrong: Loading in each test method
[Fact]
public void Test()
{
    var arch = new ArchLoader().LoadAssemblies(...).Build(); // Slow!
    arch.GetArchRules(...).Check();
}
```

#### Issue: False Positives (Rules Pass When They Shouldn't)

**Cause**: Running in Release mode (debug artifacts missing)  
**Solution**: Use `-c Debug` flag

```bash
# ❌ Wrong
dotnet test *.csproj

# ✅ Correct
dotnet test *.csproj -c Debug
```

#### Issue: Regex Not Matching

**Cause**: Missing `useRegularExpressions: true` parameter  
**Solution**: Always specify when using patterns with `.*`

```csharp
// ✅ Correct
.ResideInNamespace(".*\\.Core\\.", useRegularExpressions: true)

// ❌ Wrong: Doesn't interpret as regex
.ResideInNamespace(".*\\.Core\\.") // Treated as literal namespace
```

#### Issue: Missing Types or Assembly Not Found

**Cause**: Assembly not loaded in `LoadAssemblies()`  
**Solution**: Add marker type from assembly

```csharp
// ✅ Correct: Use typeof() to ensure assembly is included
.LoadAssemblies(
    typeof(Catalog.Core.Entities.TaxRate).Assembly,
    typeof(CMS.Core.Domain.Pages.PageDefinition).Assembly)

// ❌ Wrong: String-based assembly names may not resolve
.LoadAssemblies(Assembly.Load("B2X.Catalog.Core"))
```

#### Issue: Test Namespace Pollution

**Cause**: Test types being analyzed by architecture rules  
**Solution**: Exclude test namespaces

```csharp
// ✅ Correct
.And().DoNotResideInNamespace(".*Test.*", useRegularExpressions: true)
.And().DoNotResideInNamespace(".*Mock.*", useRegularExpressions: true)
```

#### Issue: CS0234 - 'xUnit' namespace not found (xUnit V3)

**Cause**: Using wrong namespace for xUnit V3
**Solution**: Use `ArchUnitNET.xUnitV3` instead of `ArchUnitNET.xUnit`

```csharp
// ❌ Wrong: Old xUnit namespace
using ArchUnitNET.xUnit;

// ✅ Correct: xUnit V3 namespace
using ArchUnitNET.xUnitV3;
```

#### Issue: CS0118 - 'Architecture' is namespace, used as type

**Cause**: Field name conflicts with `ArchUnitNET.Domain.Architecture` type
**Solution**: Rename field to avoid collision

```csharp
// ❌ Wrong: Name conflicts with type
protected static readonly Architecture Architecture = ...;

// ✅ Correct: Unique field name
protected static readonly Architecture B2XArchitecture = ...;
```

#### Issue: Missing [Fact] attribute after xUnit V3 migration

**Cause**: `ArchUnitNET.xUnitV3` provides `Check()` extension but NOT `[Fact]` attribute
**Solution**: Add explicit `using Xunit;`

```csharp
using ArchUnitNET.xUnitV3;  // Provides Check() extension
using Xunit;                 // Required for [Fact] attribute
```

#### Issue: Types() or Classes() unresolved when you have B2X.Types namespace

**Cause**: `B2X.Types` namespace shadows ArchRuleDefinition static members
**Solution**: Use fully qualified name

```csharp
// ❌ Wrong: Conflicts with B2X.Types namespace
using static ArchUnitNET.Fluent.ArchRuleDefinition;
Types().That()...  // Ambiguous with B2X.Types

// ✅ Correct: Fully qualified
ArchRuleDefinition.Types().That()...
```

#### Issue: "The rule requires positive evaluation" error

**Cause**: Rule predicate matches no types (empty result set)
**Solution**: Add `WithoutRequiringPositiveResults()`

```csharp
// ❌ Wrong: Fails if no types match
Types().That().ResideInNamespaceMatching("B2X.Search\\..*")
    .Should().NotDependOnAny(...)
    .Check(Architecture);  // Fails if no B2X.Search.* types exist

// ✅ Correct: Allow empty result sets
Types().That().ResideInNamespaceMatching("B2X.Search\\..*")
    .Should().NotDependOnAny(...)
    .WithoutRequiringPositiveResults()
    .Check(Architecture);
```

#### Issue: Package version mismatch errors

**Cause**: Core package and test-framework package at different versions
**Solution**: Use same version for all ArchUnitNET packages

```xml
<!-- Directory.Packages.props - All must match -->
<PackageVersion Include="TngTech.ArchUnitNET" Version="0.13.1" />
<PackageVersion Include="TngTech.ArchUnitNET.xUnitV3" Version="0.13.1" />
```

---

## Integration with CI/CD

### Run Architecture Tests

```bash
# Run only architecture tests (B2X standard)
dotnet test backend/Tests/B2X.Architecture.Tests -c Debug -v minimal

# Run as part of full test suite
dotnet test B2X.slnx --filter "Category=Architecture"

# Run with verbose output for debugging
dotnet test backend/Tests/B2X.Architecture.Tests -c Debug -v detailed
```

**⚠️ Important**: Always run ArchUnitNET tests in **Debug configuration**. ArchUnitNET relies on debug artifacts to accurately analyze the architecture. Running in Release mode may cause false positives or false negatives.

Reference: [ArchUnitNET Debug Artifacts Documentation](https://archunitnet.readthedocs.io/en/stable/limitations/debug_artifacts/)

### VS Code Task

```json
{
  "label": "test-architecture",
  "command": "dotnet",
  "type": "shell",
  "args": [
    "test",
    "${workspaceFolder}/backend/Tests/B2X.Architecture.Tests/B2X.Architecture.Tests.csproj",
    "-c", "Debug",
    "-v", "minimal"
  ],
  "group": "test"
}
```

---

## Getting Started - Complete Example (xUnit V3)

### Step 1: Create Test Project

```bash
dotnet new xunit -n B2X.Architecture.Tests
cd B2X.Architecture.Tests
dotnet add package TngTech.ArchUnitNET.xUnitV3  # Use xUnitV3 for .NET 10+
```

### Step 2: Create Base Class

```csharp
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

namespace B2X.ArchitectureTests;  // Avoid 'Architecture' in namespace

public abstract class ArchitectureTestBase
{
    // Load architecture once at class initialization (expensive operation)
    // Use unique field name to avoid conflict with ArchUnitNET.Domain.Architecture
    protected static readonly Architecture B2XArchitecture = new ArchLoader()
        .LoadAssemblies(
            typeof(B2X.Catalog.Core.Entities.TaxRate).Assembly,
            typeof(B2X.CMS.Core.Domain.Pages.PageDefinition).Assembly,
            typeof(B2X.Identity.Data.AppUser).Assembly)
        .Build();
}
```

### Step 3: Write Your First Rule

```csharp
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;  // xUnit V3 - provides Check() extension
using Xunit;                 // Required for [Fact] attribute
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace B2X.ArchitectureTests;

[Collection("Architecture")]
public class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        // Use ArchRuleDefinition.Types() if you have a B2X.Types namespace conflict
        ArchRuleDefinition.Types()
            .That().ResideInNamespaceMatching(@"B2X\.Catalog\.Core\..*")
            .Should().NotDependOnAny(
                ArchRuleDefinition.Types().That().ResideInNamespaceMatching(@"B2X\.Catalog\.Infrastructure\..*"))
            .Because("Domain must be independent of infrastructure concerns (Clean Architecture)")
            .Check(B2XArchitecture);  // Use renamed field
    }
}
```

### Step 4: Run Tests

```bash
# Run in Debug configuration (required!)
dotnet test -c Debug

# Output:
# ✓ B2X.ArchitectureTests.LayerDependencyTests.Domain_Should_Not_Depend_On_Infrastructure
```

### Example GitHub Repository

The official repository includes complete examples:
- **Location**: [TNG/ArchUnitNET/ExampleTest](https://github.com/TNG/ArchUnitNET/tree/master/ExampleTest)
- **Content**: Full working examples with xUnit, NUnit, MSTest

---

## Community & Contribution

- **Contributors**: 34+ active contributors
- **Stars**: 1.2k+ on GitHub
- **License**: Apache 2.0 (open source)
- **Support**: [TNG Technology Consulting GmbH](https://www.tngtech.com/)

### Contributing

The project welcomes contributions:
- Fork the repository on GitHub
- Follow the [Contributing Guidelines](https://github.com/TNG/ArchUnitNET/blob/master/CONTRIBUTING.md)
- Test with multiple frameworks (xUnit, NUnit, MSTest, TUnit)
- Ensure Debug configuration compatibility

---

## Related Documentation

- **ADR-021**: [ArchUnitNET for Automated Architecture Testing](../../decisions/ADR-021-archunitnet-architecture-testing.md)
- **ADR-002**: Onion Architecture Pattern
- **ADR-001**: Event-Driven Architecture
- **KB-006**: [Wolverine CQRS Patterns](../wolverine.md)

---

## Alternatives Considered

| Tool | Comparison | Why ArchUnitNET for B2X |
|------|-----------|------------------------|
| **NetArchTest** | Older, fewer features | ArchUnitNET is more actively maintained |
| **NDepend** | Commercial, powerful | ArchUnitNET is free and sufficient |
| **Roslyn Analyzers** | Complex syntax checking | ArchUnitNET is simpler for architecture rules |
| **Manual Code Reviews** | Error-prone, doesn't scale | ArchUnitNET enables automated testing |

---

## Version History & Release Timeline

| Version | Release Date | .NET Support | Key Changes | Status |
|---------|--------------|--------------|-------------|--------|
| **0.13.1** | Jan 9, 2026 (2 days ago) | .NET 6-10 | Latest - Documentation fix | ✅ Current B2X |
| 0.13.0 | Dec 2025 | .NET 6-10 | Performance improvements, .NET 10 support | Stable |
| 0.12.x | 2024 | .NET 6-9 | Initial stable release | Legacy |
| 0.11.x | 2023 | .NET 5-8 | Early versions | Deprecated |

### Release Cadence

- Active maintenance with regular updates (55+ releases total)
- Recent commit: Merge PR for documentation improvements
- Dependencies automatically updated to latest .NET versions
- TUnit framework support recently added

### Upgrade Path

```bash
# Update to latest stable
dotnet package update TngTech.ArchUnitNET.xUnit --version 0.13.1

# Or via Central Package Management
# Update Directory.Packages.props:
# <PackageVersion Include="TngTech.ArchUnitNET.xUnit" Version="0.13.1" />
```

---

**Next Review**: Monthly (check for new releases)
