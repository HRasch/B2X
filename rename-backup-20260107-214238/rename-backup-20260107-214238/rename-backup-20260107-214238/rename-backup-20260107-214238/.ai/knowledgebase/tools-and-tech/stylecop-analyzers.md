# StyleCop Analyzers Documentation

**DocID**: `KB-019`  
**Last Updated**: 2. Januar 2026  
**Maintained By**: GitHub Copilot  
**Status**: ✅ Current

---

## Overview

StyleCop Analyzers is an implementation of StyleCop rules using the .NET Compiler Platform (Roslyn). It provides code analysis and automatic code fixes for C# style and consistency rules.

## Official Resources

- **GitHub Repository**: https://github.com/DotNetAnalyzers/StyleCopAnalyzers
- **NuGet (Stable)**: https://www.nuget.org/packages/StyleCop.Analyzers/1.1.118
- **NuGet (Beta)**: https://www.nuget.org/packages/StyleCop.Analyzers/1.2.0-beta.556
- **Documentation**: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md
- **Configuration Guide**: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md
- **Status Page**: https://dotnetanalyzers.github.io/StyleCopAnalyzers/
- **JSON Schema**: https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json

## Current Versions

| Version | Status | Release Date | C# Support |
|---------|--------|--------------|------------|
| **1.1.118** | ✅ Stable | Apr 29, 2019 | C# 1.0 - 7.3 |
| **1.2.0-beta.556** | 🧪 Latest Pre-release | Dec 20, 2023 | C# 8 - 12 |

### Version Recommendation

For **B2X (.NET 10)**, use **1.2.0-beta.556** for full modern C# support:

```xml
<PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

> ⚠️ The stable 1.1.118 only supports C# 7.3 and will produce false positives with modern syntax.

---

## C# Language Version Support

| C# Version | Minimum StyleCop Version | Visual Studio |
|------------|-------------------------|---------------|
| 1.0 - 6.0 | v1.0.2+ | VS2015+ |
| 7.0 - 7.3 | v1.1.0-beta+ | VS2017+ |
| 8.0 | v1.2.0-beta+ | VS2019+ |
| 9.0 - 12.0 | v1.2.0-beta.556 | VS2022+ |

---

## Rule Categories

### SA0xxx - Special Rules
Workarounds, configuration errors, internal diagnostics.

### SA1xxx - Spacing Rules (1000-1027)
Enforce spacing around keywords and symbols.

### SA11xx - Readability Rules (1100-1141)
Ensure code is well-formatted and readable.

### SA12xx - Ordering Rules (1200-1217)
Enforce standard ordering scheme for code contents.

### SA13xx - Naming Rules (1300-1316)
Enforce naming requirements for members, types, and variables.

### SA14xx - Maintainability Rules (1400-1414)
Improve code maintainability.

### SA15xx - Layout Rules (1500-1519)
Enforce code layout and line spacing.

### SA16xx - Documentation Rules (1600-1651)
Verify content and formatting of code documentation.

### SXxxxx - Alternative Rules
Non-standard extensions to default StyleCop behavior.

---

## Configuration

StyleCop Analyzers uses two configuration mechanisms:

1. **Rule Set Files (.ruleset)** - Enable/disable rules, configure severity
2. **stylecop.json** - Project-specific settings and behavior customization

### stylecop.json Structure

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "indentation": { },
    "spacingRules": { },
    "readabilityRules": { },
    "orderingRules": { },
    "namingRules": { },
    "maintainabilityRules": { },
    "layoutRules": { },
    "documentationRules": { }
  }
}
```

### Key Configuration Options

#### Indentation Settings
```json
{
  "settings": {
    "indentation": {
      "indentationSize": 4,
      "tabSize": 4,
      "useTabs": false
    }
  }
}
```

#### Using Directives (SA1200)
```json
{
  "settings": {
    "orderingRules": {
      "systemUsingDirectivesFirst": true,
      "usingDirectivesPlacement": "outsideNamespace",
      "blankLinesBetweenUsingGroups": "allow"
    }
  }
}
```

**Placement Options:**
- `"insideNamespace"` - Default, inside namespace declarations
- `"outsideNamespace"` - Outside namespace declarations
- `"preserve"` - Do not enforce placement

#### Element Ordering (SA1201-SA1215)
```json
{
  "settings": {
    "orderingRules": {
      "elementOrder": [
        "kind",
        "accessibility",
        "constant",
        "static",
        "readonly"
      ]
    }
  }
}
```

#### Naming Rules
```json
{
  "settings": {
    "namingRules": {
      "allowCommonHungarianPrefixes": true,
      "allowedHungarianPrefixes": ["cd", "md"],
      "allowedNamespaceComponents": ["eBay", "iPod"],
      "tupleElementNameCasing": "PascalCase",
      "includeInferredTupleElementNames": false
    }
  }
}
```

#### Documentation Rules
```json
{
  "settings": {
    "documentationRules": {
      "companyName": "B2X GmbH",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "xmlHeader": true,
      "documentInterfaces": true,
      "documentExposedElements": true,
      "documentInternalElements": false,
      "documentPrivateElements": false,
      "documentPrivateFields": false,
      "documentationCulture": "en-US",
      "fileNamingConvention": "stylecop"
    }
  }
}
```

**Supported Documentation Cultures:**
`de-DE`, `en-GB`, `en-US`, `es-MX`, `fr-FR`, `nl-NL`, `pl-PL`, `pt-BR`, `ru-RU`

#### Layout Rules
```json
{
  "settings": {
    "layoutRules": {
      "newlineAtEndOfFile": "require",
      "allowConsecutiveUsings": true,
      "allowDoWhileOnClosingBrace": false
    }
  }
}
```

#### Maintainability Rules
```json
{
  "settings": {
    "maintainabilityRules": {
      "topLevelTypes": ["class", "interface", "struct", "enum", "delegate"]
    }
  }
}
```

---

## Changes in 1.2.0-beta.556

### New C# Feature Support

| Feature | C# Version | Rules Updated |
|---------|------------|---------------|
| Records | C# 9 | SA1202, SA1402, SA1600, SA1642 |
| Record structs | C# 10 | SA1202, SA1402, SA1642 |
| File-scoped namespaces | C# 10 | SA1200, SA1516, SA1649 |
| `init` accessors | C# 9 | SA1137, SA1212, SA1500, SA1513 |
| `required` modifier | C# 11 | SA1206 |
| `file` modifier | C# 11 | SA1400, SA1206 |
| Lambda natural types | C# 10 | SA1015, Updated Dec 2025 |
| Generic attributes | C# 11 | SA1015 |
| List patterns | C# 11 | SA1010, SA1012 |
| Collection expressions | C# 12 | SA1010, SA1118, SA1513 |
| Extended property patterns | C# 10 | SA1101 |
| Static abstract interface members | C# 11 | SA1648 |

### Notable Rule Changes

- **SA1010**: Accept whitespace before collection initializers; don't trigger on list patterns
- **SA1011**: Forbid trailing space before end of switch case; don't require space before range operator
- **SA1118**: Allow multi-line collection expressions
- **SA1119**: Support ranges, stackalloc, `with` expressions, switch expressions
- **SA1131**: Treat methods as constants in comparisons
- **SA1513**: No blank line required at end of collection expressions
- **SA1516**: Support file-scoped namespaces; fix between global statements
- **SA1648**: Accept inheritdoc on static abstract/virtual interface members

### New Configuration Options (1.2.0)

| Option | Default | Description |
|--------|---------|-------------|
| `allowedNamespaceComponents` | `[]` | Allow custom namespace components |
| `tupleElementNameCasing` | `"PascalCase"` | Tuple element name casing |
| `includeInferredTupleElementNames` | `false` | Analyze inferred tuple names |
| `allowDoWhileOnClosingBrace` | `false` | Allow while on closing brace line |

---

## Common Rule Customizations

### Disable SA1101 (Prefix local calls with `this.`)
Many teams disable this rule as modern IDEs make member access clear:

```xml
<!-- In .editorconfig -->
dotnet_diagnostic.SA1101.severity = none
```

### Disable SA1309 (Field names must not begin with underscore)
To allow `_fieldName` convention:

```xml
dotnet_diagnostic.SA1309.severity = none
```

### File Header Configuration
```json
{
  "settings": {
    "documentationRules": {
      "companyName": "Your Company",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license.",
      "variables": {
        "licenseName": "MIT"
      },
      "headerDecoration": "-----------------------------------------------------------------------"
    }
  }
}
```

---

## EditorConfig Integration (1.2.0+)

StyleCop 1.2.0+ supports `.editorconfig` for many settings:

```ini
# StyleCop settings in .editorconfig
[*.cs]

# SA1200 - Using directives placement
stylecop.orderingRules.usingDirectivesPlacement = outsideNamespace

# Documentation
stylecop.documentationRules.companyName = B2X GmbH
stylecop.documentationRules.documentInternalElements = false

# File header
file_header_template = Copyright (c) B2X GmbH. All rights reserved.
```

---

## Installation

### Package Reference (Recommended)
```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>

<ItemGroup>
  <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

### CLI Installation
```bash
dotnet add package StyleCop.Analyzers --version 1.2.0-beta.556
```

### Central Package Management (Directory.Packages.props)
```xml
<PackageVersion Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />
```

---

## Sharing Configuration

Create a NuGet package with shared configuration:

**acme.stylecop.nuspec:**
```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <id>acme.stylecop</id>
    <version>1.0.0</version>
    <dependencies>
      <dependency id="StyleCop.Analyzers" version="1.2.0-beta.556" />
    </dependencies>
  </metadata>
  <files>
    <file src="stylecop.json" target="" />
    <file src="acme.stylecop.ruleset" target="" />
    <file src="acme.stylecop.props" target="build" />
  </files>
</package>
```

---

## Troubleshooting

### Common Issues

1. **False positives with modern C# syntax**
   - Solution: Upgrade to 1.2.0-beta.556

2. **stylecop.json not recognized**
   - Ensure it's added as `<AdditionalFiles>` in project
   - Check JSON schema reference is correct

3. **Rules not applying**
   - Verify package is installed correctly
   - Check .editorconfig isn't overriding settings

4. **Performance issues**
   - Disable rules you don't need
   - Use `<PrivateAssets>all</PrivateAssets>`

---

## B2X Style Rules & Conventions

### Project-Specific Style Guidelines

#### Naming Conventions
- **Interfaces**: Always prefixed with `I` (e.g., `IProductService`)
- **Async Methods**: Always suffixed with `Async` (e.g., `GetProductsAsync()`)
- **Private Fields**: Always prefixed with underscore (e.g., `_logger`)
- **Constants**: PascalCase (e.g., `MaxRetries`)
- **Local Variables**: camelCase, prefer explicit types over `var`

#### Code Organization
- **Using Directives**: System directives first, then third-party, then project-specific
- **Namespace Imports**: Outside namespace declarations
- **File Structure**: One class per file, matching filename
- **Method Order**: Public methods first, then protected, then private

#### Documentation Standards
- **Public APIs**: Full XML documentation required
- **Internal Classes**: Optional documentation
- **Private Members**: No documentation required
- **Company Name**: "B2X GmbH"
- **Copyright**: "Copyright (c) {companyName}. All rights reserved."

#### Spacing & Formatting
- **Indentation**: 4 spaces, no tabs
- **Braces**: Always on new line for methods/classes
- **Blank Lines**: One blank line between methods, no consecutive blank lines
- **Line Length**: No hard limit, but prefer readable line breaks

---

## Detailed Rule Explanations

### Spacing Rules (SA1000-SA1028)

#### SA1000: Keywords must be spaced correctly
**Description**: Keywords like `if`, `for`, `while` must be followed by a space.

**❌ Violation:**
```csharp
if(condition) { }
for(int i=0;i<10;i++) { }
```

**✅ Correct:**
```csharp
if (condition) { }
for (int i = 0; i < 10; i++) { }
```

#### SA1001: Commas must be spaced correctly
**Description**: Comma-separated items must have space after comma, no space before.

**❌ Violation:**
```csharp
Method(param1,param2 , param3);
```

**✅ Correct:**
```csharp
Method(param1, param2, param3);
```

#### SA1005: Single line comments must begin with single space
**Description**: Comments must start with `// ` (double slash + space).

**❌ Violation:**
```csharp
//This is a comment
```

**✅ Correct:**
```csharp
// This is a comment
```

#### SA1009: Closing parenthesis must be spaced correctly
**Description**: No space before closing parenthesis in method calls.

**❌ Violation:**
```csharp
Method( param );
```

**✅ Correct:**
```csharp
Method(param);
```

#### SA1010: Opening square brackets must not be preceded by a space
**Description**: Array access and attributes must not have space before `[` or `[[`.

**❌ Violation:**
```csharp
array [0] = value;
[ Serializable ]
```

**✅ Correct:**
```csharp
array[0] = value;
[Serializable]
```

#### SA1011: Closing square brackets must not be preceded by a space
**Description**: No space before closing square bracket.

**❌ Violation:**
```csharp
array[0 ];
```

**✅ Correct:**
```csharp
array[0];
```

#### SA1012: Opening braces must be spaced correctly
**Description**: Opening braces should be on new line for methods/classes, inline for simple statements.

**❌ Violation:**
```csharp
public void Method() {
    if (condition) {
        // code
    }
}
```

**✅ Correct:**
```csharp
public void Method()
{
    if (condition)
    {
        // code
    }
}
```

#### SA1013: Closing braces must be spaced correctly
**Description**: Closing braces should be on new line.

**❌ Violation:**
```csharp
if (condition) { /* code */ } }
```

**✅ Correct:**
```csharp
if (condition)
{
    /* code */
}
```

#### SA1024: Colons must be spaced correctly
**Description**: Space before colon in base class declarations, no space in case statements.

**❌ Violation:**
```csharp
class MyClass: BaseClass
switch (value)
{
    case 1 : break;
}
```

**✅ Correct:**
```csharp
class MyClass : BaseClass
switch (value)
{
    case 1: break;
}
```

#### SA1028: Code must not contain trailing whitespace
**Description**: No spaces or tabs at end of lines.

**❌ Violation:**
```csharp
public void Method() { }   
//                        ^^ trailing spaces
```

**✅ Correct:**
```csharp
public void Method() { }
// No trailing spaces
```

### Readability Rules (SA1100-SA1199)

#### SA1101: Prefix local calls with this
**Description**: Local method calls should be prefixed with `this.` for clarity.

**❌ Violation:**
```csharp
public class MyClass
{
    private void Helper() { }

    public void Method()
    {
        Helper(); // unclear if local or external
    }
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    private void Helper() { }

    public void Method()
    {
        this.Helper(); // clearly local call
    }
}
```

**Note**: B2X disables this rule as modern IDEs make member access clear.

#### SA1106: Code must not contain empty statements
**Description**: No standalone semicolons or empty blocks.

**❌ Violation:**
```csharp
if (condition); // empty statement
{
} // empty block
```

**✅ Correct:**
```csharp
if (condition)
{
    DoSomething();
}
```

#### SA1111: Closing parenthesis must be on line of last parameter
**Description**: Method parameters should be properly aligned.

**❌ Violation:**
```csharp
Method(param1,
       param2
); // closing paren on new line
```

**✅ Correct:**
```csharp
Method(param1,
       param2); // closing paren on last param line
```

**Note**: B2X disables this rule for better readability with long parameter lists.

#### SA1116: Split parameters must start on line after declaration
**Description**: Multi-line parameters should be properly formatted.

**❌ Violation:**
```csharp
public void Method(int param1, int param2,
                   int param3)
```

**✅ Correct:**
```csharp
public void Method(int param1,
                   int param2,
                   int param3)
```

#### SA1118: Parameter must not span multiple lines
**Description**: Parameter declarations should not span multiple lines.

**❌ Violation:**
```csharp
public void Method(int veryLongParameterNameThatSpansMultipleLines)
```

**✅ Correct:**
```csharp
public void Method(
    int veryLongParameterNameThatSpansMultipleLines)
```

#### SA1122: Use string.Empty for empty strings
**Description**: Use `string.Empty` instead of `""` for empty strings.

**❌ Violation:**
```csharp
string empty = "";
```

**✅ Correct:**
```csharp
string empty = string.Empty;
```

#### SA1124: Do not use regions
**Description**: Avoid using `#region` directives.

**❌ Violation:**
```csharp
#region Private Methods
private void Method() { }
#endregion
```

**✅ Correct:**
```csharp
// Group related methods with comments
// Private Methods
private void Method() { }
```

#### SA1127: Generic type constraints must be on same line
**Description**: Generic constraints should be on same line as type parameter.

**❌ Violation:**
```csharp
public class MyClass<T>
    where T : class
```

**✅ Correct:**
```csharp
public class MyClass<T> where T : class
```

#### SA1131: Use readable conditions
**Description**: Avoid confusing conditional expressions.

**❌ Violation:**
```csharp
if (x = 5) // assignment instead of comparison
```

**✅ Correct:**
```csharp
if (x == 5)
```

#### SA1134: Each attribute must be on separate line
**Description**: Multiple attributes should be on separate lines.

**❌ Violation:**
```csharp
[Serializable, Obsolete]
public class MyClass
```

**✅ Correct:**
```csharp
[Serializable]
[Obsolete]
public class MyClass
```

### Ordering Rules (SA1200-SA1299)

#### SA1200: Using directives must be placed correctly
**Description**: Using directives must be before namespace declaration.

**❌ Violation:**
```csharp
namespace MyNamespace
{
    using System;
}
```

**✅ Correct:**
```csharp
using System;

namespace MyNamespace
{
}
```

#### SA1201: Elements must appear in correct order
**Description**: Class elements must follow specific order: constants, fields, constructors, methods, properties, events.

**❌ Violation:**
```csharp
public class MyClass
{
    public void Method() { }
    private int _field;
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    private int _field;
    public void Method() { }
}
```

#### SA1202: Elements must be ordered by access
**Description**: Elements should be ordered by accessibility: public, internal, protected internal, protected, private.

**❌ Violation:**
```csharp
public class MyClass
{
    private void PrivateMethod() { }
    public void PublicMethod() { }
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    public void PublicMethod() { }
    private void PrivateMethod() { }
}
```

#### SA1204: Static elements must appear before instance elements
**Description**: Static members should come before instance members.

**❌ Violation:**
```csharp
public class MyClass
{
    public void InstanceMethod() { }
    public static void StaticMethod() { }
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    public static void StaticMethod() { }
    public void InstanceMethod() { }
}
```

#### SA1210: Using directives must be ordered alphabetically
**Description**: Using directives should be sorted alphabetically.

**❌ Violation:**
```csharp
using System.Linq;
using System;
using System.Collections.Generic;
```

**✅ Correct:**
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
```

#### SA1214: Readonly fields must appear before non-readonly fields
**Description**: Readonly fields should come before regular fields.

**❌ Violation:**
```csharp
public class MyClass
{
    private int _field;
    private readonly int _readonlyField;
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    private readonly int _readonlyField;
    private int _field;
}
```

### Naming Rules (SA1300-SA1399)

#### SA1300: Element must begin with upper case letter
**Description**: Type names must start with uppercase letter.

**❌ Violation:**
```csharp
public class myClass
```

**✅ Correct:**
```csharp
public class MyClass
```

#### SA1303: Const field names must begin with upper case letter
**Description**: Constant field names must be PascalCase.

**❌ Violation:**
```csharp
public const int maxRetries = 3;
```

**✅ Correct:**
```csharp
public const int MaxRetries = 3;
```

#### SA1306: Field names must begin with lower case letter
**Description**: Field names must start with lowercase letter.

**❌ Violation:**
```csharp
private int FieldName;
```

**✅ Correct:**
```csharp
private int fieldName;
```

#### SA1309: Field names must not begin with underscore
**Description**: Field names should not start with underscore.

**❌ Violation:**
```csharp
private int _fieldName;
```

**✅ Correct:**
```csharp
private int fieldName;
```

**Note**: B2X allows underscore prefix for private fields.

#### SA1310: Field names must not contain underscore
**Description**: Field names should not contain underscores.

**❌ Violation:**
```csharp
private int field_Name;
```

**✅ Correct:**
```csharp
private int fieldName;
```

#### SA1311: Static readonly fields must begin with upper case letter
**Description**: Static readonly field names must be PascalCase.

**❌ Violation:**
```csharp
public static readonly int defaultValue = 42;
```

**✅ Correct:**
```csharp
public static readonly int DefaultValue = 42;
```

### Documentation Rules (SA1600-SA1699)

#### SA1600: Elements must be documented
**Description**: Public elements must have XML documentation.

**❌ Violation:**
```csharp
public class MyClass
{
    public void Method() { }
}
```

**✅ Correct:**
```csharp
/// <summary>
/// Represents a sample class.
/// </summary>
public class MyClass
{
    /// <summary>
    /// Performs an operation.
    /// </summary>
    public void Method() { }
}
```

#### SA1602: Enumeration items must be documented
**Description**: Enum values must be documented.

**❌ Violation:**
```csharp
public enum Status
{
    Active,
    Inactive
}
```

**✅ Correct:**
```csharp
/// <summary>
/// Represents the status of an item.
/// </summary>
public enum Status
{
    /// <summary>
    /// The item is active.
    /// </summary>
    Active,

    /// <summary>
    /// The item is inactive.
    /// </summary>
    Inactive
}
```

#### SA1611: Element parameter documentation must match element parameters
**Description**: XML documentation parameters must match method parameters.

**❌ Violation:**
```csharp
/// <summary>Does something.</summary>
/// <param name="value">The value.</param>
public void Method(int input) { }
```

**✅ Correct:**
```csharp
/// <summary>Does something.</summary>
/// <param name="input">The value.</param>
public void Method(int input) { }
```

#### SA1615: Element return value must be documented
**Description**: Non-void methods must document return value.

**❌ Violation:**
```csharp
/// <summary>Gets a value.</summary>
public int GetValue() { return 42; }
```

**✅ Correct:**
```csharp
/// <summary>Gets a value.</summary>
/// <returns>The value.</returns>
public int GetValue() { return 42; }
```

#### SA1623: Property summary documentation must match accessors
**Description**: Property documentation should describe the property, not getter/setter.

**❌ Violation:**
```csharp
/// <summary>Gets the value.</summary>
public int Value { get; set; }
```

**✅ Correct:**
```csharp
/// <summary>Gets or sets the value.</summary>
public int Value { get; set; }
```

#### SA1629: Documentation text must end with a period
**Description**: Documentation comments must end with period.

**❌ Violation:**
```csharp
/// <summary>This is a method</summary>
```

**✅ Correct:**
```csharp
/// <summary>This is a method.</summary>
```

#### SA1633: File must have header
**Description**: Files must have copyright header.

**❌ Violation:**
```csharp
// File without header
public class MyClass { }
```

**✅ Correct:**
```csharp
// Copyright (c) B2X GmbH. All rights reserved.
public class MyClass { }
```

### Layout Rules (SA1500-SA1599)

#### SA1501: Statement must not be on a single line
**Description**: Control statements should not be on single line.

**❌ Violation:**
```csharp
if (condition) DoSomething();
```

**✅ Correct:**
```csharp
if (condition)
{
    DoSomething();
}
```

#### SA1502: Element must not be on a single line
**Description**: Class/struct elements should not be on single line.

**❌ Violation:**
```csharp
public class MyClass { public void Method() { } }
```

**✅ Correct:**
```csharp
public class MyClass
{
    public void Method()
    {
    }
}
```

#### SA1503: Braces must not be omitted
**Description**: Curly braces must be used even for single statements.

**❌ Violation:**
```csharp
if (condition)
    DoSomething();
```

**✅ Correct:**
```csharp
if (condition)
{
    DoSomething();
}
```

#### SA1505: Opening braces must not be followed by blank line
**Description**: No blank line after opening brace.

**❌ Violation:**
```csharp
public void Method()
{

    DoSomething();
}
```

**✅ Correct:**
```csharp
public void Method()
{
    DoSomething();
}
```

#### SA1507: Code must not contain multiple blank lines in a row
**Description**: No consecutive blank lines.

**❌ Violation:**
```csharp
public void Method()
{
    DoSomething();


    DoMore();
}
```

**✅ Correct:**
```csharp
public void Method()
{
    DoSomething();

    DoMore();
}
```

#### SA1508: Closing braces must not be preceded by blank line
**Description**: No blank line before closing brace.

**❌ Violation:**
```csharp
public void Method()
{
    DoSomething();

}
```

**✅ Correct:**
```csharp
public void Method()
{
    DoSomething();
}
```

#### SA1512: Single-line comments must not be followed by blank line
**Description**: No blank line after single-line comment.

**❌ Violation:**
```csharp
// This is a comment

DoSomething();
```

**✅ Correct:**
```csharp
// This is a comment
DoSomething();
```

#### SA1513: Closing brace must be followed by blank line
**Description**: Blank line after closing brace (except in expression bodies).

**❌ Violation:**
```csharp
public void Method() { }
public void AnotherMethod() { }
```

**✅ Correct:**
```csharp
public void Method()
{
}

public void AnotherMethod()
{
}
```

#### SA1515: Single-line comment must be preceded by blank line
**Description**: Single-line comments should be preceded by blank line if they start a new logical section.

**❌ Violation:**
```csharp
DoSomething();
// New section comment
DoMore();
```

**✅ Correct:**
```csharp
DoSomething();

// New section comment
DoMore();
```

#### SA1516: Elements must be separated by blank line
**Description**: Related elements should be grouped with blank lines between groups.

**❌ Violation:**
```csharp
public class MyClass
{
    private int _field;
    public void Method() { }
    public int Property { get; set; }
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    private int _field;

    public void Method()
    {
    }

    public int Property { get; set; }
}
```

#### SA1518: Use line endings correctly at end of file
**Description**: File must end with single newline.

**❌ Violation:**
```csharp
public class MyClass { }
// (no newline at end)
```

**✅ Correct:**
```csharp
public class MyClass { }
// (single newline at end)
```

### Maintainability Rules (SA1400-SA1499)

#### SA1400: Access modifier must be declared
**Description**: All elements must have explicit access modifier.

**❌ Violation:**
```csharp
class MyClass // implicit internal
{
    void Method() // implicit private
    {
    }
}
```

**✅ Correct:**
```csharp
internal class MyClass
{
    private void Method()
    {
    }
}
```

#### SA1401: Fields must be private
**Description**: Fields should be private (use properties for public access).

**❌ Violation:**
```csharp
public class MyClass
{
    public int field;
}
```

**✅ Correct:**
```csharp
public class MyClass
{
    private int _field;

    public int Field
    {
        get { return _field; }
        set { _field = value; }
    }
}
```

#### SA1402: File may only contain a single type
**Description**: One type per file.

**❌ Violation:**
```csharp
// MyClass.cs
public class MyClass { }
public class AnotherClass { }
```

**✅ Correct:**
```csharp
// MyClass.cs
public class MyClass { }

// AnotherClass.cs
public class AnotherClass { }
```

#### SA1407: Arithmetic expressions must declare precedence
**Description**: Use parentheses to clarify operator precedence.

**❌ Violation:**
```csharp
int result = a + b * c;
```

**✅ Correct:**
```csharp
int result = a + (b * c);
```

#### SA1413: Use trailing comma in multi-line initializers
**Description**: Multi-line collections should have trailing comma.

**❌ Violation:**
```csharp
var list = new List<int>
{
    1,
    2,
    3
};
```

**✅ Correct:**
```csharp
var list = new List<int>
{
    1,
    2,
    3,
};
```

---

## Common Violations & Quick Fixes

### Most Common Issues in B2X

1. **SA1200**: Using directives inside namespace
   - **Fix**: Move all `using` statements before namespace declaration

2. **SA1600**: Missing XML documentation
   - **Fix**: Add `/// <summary>` comments to public members

3. **SA1401**: Public fields
   - **Fix**: Make fields private and expose via properties

4. **SA1310**: Underscore in field names
   - **Fix**: Remove underscores (unless SA1309 is disabled)

5. **SA1503**: Missing braces
   - **Fix**: Always use curly braces for control statements

6. **SA1513**: Missing blank lines after braces
   - **Fix**: Add blank line after closing brace

### IDE Integration Tips

- **Visual Studio**: Use "Format Document" (Ctrl+K, Ctrl+D) for auto-fixing
- **VS Code**: Install "C# FixFormat" extension for similar functionality
- **Rider**: Built-in StyleCop support with quick fixes

### Suppressed Rules in B2X

Based on `stylecop.json`, the following rules are disabled:

- **SA1111**: Closing parenthesis placement (allows more readable formatting)
- **SA1309**: Field names with underscore (allows `_fieldName` convention)

---

## Best Practices for B2X Development

### Code Style Consistency
- Always run StyleCop analysis before committing
- Use consistent formatting across team members
- Prefer explicit over implicit code style

### Documentation Standards
- Document all public APIs with full XML comments
- Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags appropriately
- Keep documentation up-to-date with code changes

### Naming Conventions
- Follow .NET naming guidelines
- Use descriptive names that clearly indicate purpose
- Avoid abbreviations unless they are well-known (e.g., `Id`, `Url`)

### Code Organization
- Group related functionality together
- Use partial classes only when necessary
- Keep methods focused on single responsibility

### Performance Considerations
- Avoid unnecessary object allocations
- Use efficient collection types
- Consider async/await patterns for I/O operations

---

## Troubleshooting StyleCop Issues

### Rule Conflicts
When StyleCop rules conflict with each other or project requirements:
1. Check if the rule can be configured in `stylecop.json`
2. Consider suppressing the rule with severity "none"
3. Document the decision in project guidelines

### Generated Code
For auto-generated code that triggers StyleCop warnings:
1. Use `<auto-generated>` header to exclude from analysis
2. Suppress specific rules for generated files
3. Consider regenerating code with better formatting

### Legacy Code
For existing code that doesn't meet StyleCop standards:
1. Fix violations incrementally
2. Use bulk fixes where possible
3. Consider code cleanup sprints

### Build Integration
Ensure StyleCop runs in CI/CD pipeline:
- Include in build configuration
- Set appropriate warning levels
- Fail builds on critical violations

---

## Advanced Configuration

### Custom Rule Sets
Create project-specific rule sets for different components:

```xml
<!-- B2X.ruleset -->
<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="B2X Rules" ToolsVersion="15.0">
  <Include Path="minimumrecommendedrules.ruleset" Action="Default" />
  <Rules AnalyzerId="StyleCop.Analyzers" RuleNamespace="StyleCop.Analyzers">
    <Rule Id="SA1101" Action="None" /> <!-- Disable this. prefix requirement -->
    <Rule Id="SA1309" Action="None" /> <!-- Allow underscore prefix -->
  </Rules>
</RuleSet>
```

### Global Suppression Files
Use `GlobalSuppressions.cs` for project-wide suppressions:

```csharp
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Internal implementation")]
```

---

## Integration with Other Tools

### Roslyn Analyzers
StyleCop works alongside other Roslyn analyzers:
- **SonarQube**: Code quality metrics
- **FxCop**: Additional .NET rules
- **Custom Analyzers**: Project-specific rules

### IDE Extensions
Recommended extensions for better StyleCop experience:
- **StyleCop.Analyzers** (NuGet package)
- **Roslynator** (additional code fixes)
- **CodeMaid** (code organization)

### CI/CD Integration
Include StyleCop in automated builds:

```yaml
# .github/workflows/build.yml
- name: Run StyleCop
  run: dotnet build --no-restore /p:TreatWarningsAsErrors=true
```

---

## Performance Optimization

### Analysis Performance
- Use incremental analysis in IDE
- Exclude generated code from analysis
- Configure rules appropriately for large codebases

### Build Performance
- Run StyleCop only in Release builds if needed
- Use parallel analysis when possible
- Cache analysis results

---

## Migration Guide

### Upgrading from StyleCop Classic
1. Remove old StyleCop NuGet packages
2. Install StyleCop.Analyzers
3. Convert `.ruleset` files to `.editorconfig` or `stylecop.json`
4. Update suppression files
5. Test build and fix new violations

### From No StyleCop to StyleCop
1. Install StyleCop.Analyzers package
2. Create `stylecop.json` configuration
3. Run analysis and fix violations incrementally
4. Integrate into CI/CD pipeline
5. Train team on new standards

---

## StyleCop Rules Reference

This section provides a comprehensive reference of all StyleCop rules, organized by category. Each rule includes:
- **Rule ID**: The unique identifier
- **Description**: What the rule checks
- **Example**: Code examples showing violations and fixes
- **Configuration**: How to customize the rule
- **Severity**: Default severity level

### SA0xxx - Special Rules

#### SA0001 - XML Comments Must Be Valid
**Description**: XML documentation comments must be syntactically correct.

**Example**:
```csharp
// ❌ Violation
/// <summary>
/// This method does <something
/// </summary>
public void Method() { }

// ✅ Correct
/// <summary>
/// This method does something
/// </summary>
public void Method() { }
```

#### SA0002 - Invalid XML Documentation
**Description**: XML documentation must be well-formed and valid.

### SA1xxx - Spacing Rules

#### SA1000 - Keywords Must Be Spaced Correctly
**Description**: Keywords must be surrounded by appropriate whitespace.

**Example**:
```csharp
// ❌ Violation
if(true) { }

// ✅ Correct
if (true) { }
```

#### SA1001 - Commas Must Be Spaced Correctly
**Description**: Commas must be followed by a space but not preceded by one.

**Example**:
```csharp
// ❌ Violation
Method(a,b,c);

// ✅ Correct
Method(a, b, c);
```

#### SA1002 - Semicolons Must Be Spaced Correctly
**Description**: Semicolons must not be preceded by whitespace.

#### SA1003 - Symbols Must Be Spaced Correctly
**Description**: Operators and symbols must be surrounded by appropriate whitespace.

#### SA1004 - Documentation Lines Must Start With Single Space
**Description**: Documentation lines must start with a single space after `///`.

#### SA1005 - Single Line Comments Must Begin With Single Space
**Description**: Single line comments must begin with a single space.

#### SA1006 - Preprocessor Keywords Must Not Be Preceded By Space
**Description**: Preprocessor keywords must not be preceded by whitespace.

#### SA1007 - Operator Keyword Must Be Followed By Space
**Description**: The `operator` keyword must be followed by a space.

#### SA1008 - Opening Parenthesis Must Be Spaced Correctly
**Description**: Opening parentheses must not be preceded by whitespace.

#### SA1009 - Closing Parenthesis Must Be Spaced Correctly
**Description**: Closing parentheses must not be followed by whitespace in certain contexts.

**Example**:
```csharp
// ❌ Violation
Method( );

// ✅ Correct
Method();
```

#### SA1010 - Opening Square Brackets Must Be Spaced Correctly
**Description**: Opening square brackets must not be preceded by whitespace.

#### SA1011 - Closing Square Brackets Must Be Spaced Correctly
**Description**: Closing square brackets must not be followed by whitespace in certain contexts.

#### SA1012 - Opening Braces Must Be Spaced Correctly
**Description**: Opening braces must be preceded by appropriate whitespace.

#### SA1013 - Closing Braces Must Be Spaced Correctly
**Description**: Closing braces must be followed by appropriate whitespace.

#### SA1014 - Opening Generic Brackets Must Be Spaced Correctly
**Description**: Opening generic brackets must not be preceded by whitespace.

#### SA1015 - Closing Generic Brackets Must Be Spaced Correctly
**Description**: Closing generic brackets must not be followed by whitespace in certain contexts.

#### SA1016 - Opening Attribute Brackets Must Be Spaced Correctly
**Description**: Opening attribute brackets must not be preceded by whitespace.

#### SA1017 - Closing Attribute Brackets Must Be Spaced Correctly
**Description**: Closing attribute brackets must not be followed by whitespace.

#### SA1018 - Nullable Type Symbols Must Be Spaced Correctly
**Description**: Nullable type symbols (?) must not be preceded by whitespace.

#### SA1019 - Member Access Symbols Must Be Spaced Correctly
**Description**: Member access symbols (.) must not be preceded or followed by whitespace.

#### SA1020 - Increment Decrement Symbols Must Be Spaced Correctly
**Description**: Increment/decrement operators must not be preceded by whitespace.

#### SA1021 - Negative And Positive Signs Must Be Spaced Correctly
**Description**: Unary operators must not be followed by whitespace.

#### SA1022 - Positive Signs Must Be Spaced Correctly
**Description**: Positive signs must not be followed by whitespace.

#### SA1023 - Dereference And Access Of Symbols Must Be Spaced Correctly
**Description**: Dereference and access operators must not be preceded or followed by whitespace.

#### SA1024 - Colons Must Be Spaced Correctly
**Description**: Colons must be followed by a space in certain contexts.

#### SA1025 - Code Must Not Contain Multiple Whitespace In A Row
**Description**: Multiple consecutive whitespace characters are not allowed.

#### SA1026 - Code Must Not Contain Space After New Keyword In Implicitly Typed Array Allocations
**Description**: The `new` keyword in implicitly typed arrays must not be followed by whitespace.

#### SA1027 - Use Tabs Correctly
**Description**: Code must use consistent tab usage.

#### SA1028 - Code Must Not Contain Trailing Whitespace
**Description**: Lines must not end with whitespace characters.

### SA11xx - Readability Rules

#### SA1101 - Prefix Local Calls With This
**Description**: Local method calls should be prefixed with `this.` for clarity.

**Configuration**: Often disabled in modern C# projects.

#### SA1102 - Query Clause Must Follow Previous Clause
**Description**: LINQ query clauses must be properly ordered.

#### SA1103 - Query Clauses Must Be On Separate Lines Or All On One Line
**Description**: LINQ query clauses must be consistently formatted.

#### SA1104 - Query Clause Must Begin On New Line When Previous Clause Spans Multiple Lines
**Description**: Multi-line query clauses must be properly formatted.

#### SA1105 - Query Clauses Spanning Multiple Lines Must Begin On Own Line
**Description**: Query clauses spanning multiple lines must start on new lines.

#### SA1106 - Code Must Not Contain Empty Statements
**Description**: Empty statements are not allowed.

#### SA1107 - Code Must Not Contain Multiple Statements On One Line
**Description**: Multiple statements on one line are not allowed.

#### SA1108 - Block Statements Must Not Contain Embedded Comments
**Description**: Comments should not be embedded within statement blocks.

#### SA1109 - Block Statements Must Not Contain Embedded Regions
**Description**: Regions should not be embedded within statement blocks.

#### SA1110 - Opening Parenthesis Must Be On Declaration Line
**Description**: Opening parentheses must be on the same line as the declaration.

#### SA1111 - Closing Parenthesis Must Be On Line Of Last Parameter
**Description**: Closing parentheses must be on the same line as the last parameter.

**Configuration**: Often disabled for long parameter lists.

#### SA1112 - Closing Parenthesis Must Be On Line Of Opening Parenthesis
**Description**: Closing parentheses should align with opening parentheses.

#### SA1113 - Comma Must Be On Same Line As Previous Parameter
**Description**: Commas in parameter lists must be on the same line as the previous parameter.

#### SA1114 - Parameter List Must Follow Declaration
**Description**: Parameter lists must follow the method declaration properly.

#### SA1115 - Parameter Must Follow Comma
**Description**: Parameters must follow commas in parameter lists.

#### SA1116 - Split Parameters Must Start On Line After Declaration
**Description**: Split parameters must start on the line after the declaration.

#### SA1117 - Parameters Must Be On Same Line Or Separate Lines
**Description**: Parameters must be consistently formatted.

#### SA1118 - Parameter Must Not Span Multiple Lines
**Description**: Parameters should not span multiple lines.

#### SA1119 - Statement Must Not Use Unnecessary Parentheses
**Description**: Unnecessary parentheses should be removed.

#### SA1120 - Comments Must Contain Text
**Description**: Comments must contain meaningful text.

#### SA1121 - Use Built-in Type Alias
**Description**: Use built-in type aliases instead of CLR type names.

#### SA1122 - Use String.Empty For Empty Strings
**Description**: Use `string.Empty` instead of `""` for empty strings.

#### SA1123 - Do Not Place Regions Within Elements
**Description**: Regions should not be placed within elements.

#### SA1124 - Do Not Use Regions
**Description**: Regions are discouraged.

#### SA1125 - Use Shorthand For Nullable Types
**Description**: Use `T?` instead of `Nullable<T>`.

#### SA1126 - Prefix Calls Correctly
**Description**: Calls to static members should be prefixed appropriately.

#### SA1127 - Generic Type Constraints Must Be On Own Line
**Description**: Generic type constraints must be on their own line.

#### SA1128 - Constructor Should Not Call Base Class Virtual Methods
**Description**: Constructors should not call virtual methods on the base class.

#### SA1129 - Do Not Use Default Value Type Constructor
**Description**: Do not use `new T()` for value types.

**Example**:
```csharp
// ❌ Violation
int value = new int();

// ✅ Correct
int value = 0;
```

#### SA1130 - Use Lambda Syntax
**Description**: Use lambda syntax instead of anonymous methods.

#### SA1131 - Use Readable Conditions
**Description**: Use readable conditions in comparisons.

#### SA1132 - Do Not Combine Fields
**Description**: Fields should not be combined on the same line.

#### SA1133 - Do Not Combine Attributes
**Description**: Attributes should not be combined inappropriately.

#### SA1134 - Attributes Must Not Share Line
**Description**: Attributes must not share lines with other elements.

#### SA1135 - Using Directives Must Be Placed Correctly
**Description**: Using directives must be placed correctly.

#### SA1136 - Enum Values Should Be On Separate Lines
**Description**: Enum values should be on separate lines.

#### SA1137 - Elements Should Have The Same Indentation
**Description**: Elements should have consistent indentation.

#### SA1138 - Block Should Not Be Empty
**Description**: Code blocks should not be empty.

#### SA1139 - Use Literal Suffix Notation Instead Of Casting
**Description**: Use literal suffixes instead of casting.

#### SA1140 - Use Tuple Syntax
**Description**: Use tuple syntax when appropriate.

#### SA1141 - Use Dynamic
**Description**: Use `dynamic` when appropriate.

### SA12xx - Ordering Rules

#### SA1200 - Using Directives Must Be Placed Correctly
**Description**: Using directives must be placed correctly relative to namespaces.

#### SA1201 - Elements Must Appear In The Correct Order
**Description**: Elements must appear in the correct order within types.

#### SA1202 - Elements Must Be Ordered By Access
**Description**: Elements must be ordered by accessibility.

#### SA1203 - Constants Must Appear Before Fields
**Description**: Constants must appear before fields.

#### SA1204 - Static Elements Must Appear Before Instance Elements
**Description**: Static elements must appear before instance elements.

#### SA1205 - Partial Elements Must Declare Access
**Description**: Partial elements must declare access modifiers.

#### SA1206 - Declaration Keywords Must Follow Order
**Description**: Declaration keywords must follow the correct order.

#### SA1207 - Protected Must Come Before Internal
**Description**: `protected` must come before `internal`.

#### SA1208 - System Using Directives Must Be Placed Before Other Using Directives
**Description**: System using directives must be placed first.

#### SA1209 - Using Alias Directives Must Be Placed After Other Using Directives
**Description**: Using alias directives must be placed after other using directives.

#### SA1210 - Using Directives Must Be Ordered Alphabetically By Namespace
**Description**: Using directives must be ordered alphabetically.

**Example**:
```csharp
// ❌ Violation
using System.Linq;
using System.Collections.Generic;
using System;

// ✅ Correct
using System;
using System.Collections.Generic;
using System.Linq;
```

#### SA1211 - Using Alias Directives Must Be Ordered Alphabetically By Alias Name
**Description**: Using alias directives must be ordered alphabetically by alias name.

#### SA1212 - Property Accessors Must Follow Order
**Description**: Property accessors must follow the correct order (get then set).

#### SA1213 - Event Accessors Must Follow Order
**Description**: Event accessors must follow the correct order.

#### SA1214 - Readonly Fields Must Appear Before Non-Readonly Fields
**Description**: Readonly fields must appear before non-readonly fields.

#### SA1215 - Instance Fields Must Appear Before Properties
**Description**: Instance fields must appear before properties.

#### SA1216 - Using Static Directives Must Be Placed Correctly
**Description**: Using static directives must be placed correctly.

#### SA1217 - Using Static Directives Must Be Ordered Alphabetically
**Description**: Using static directives must be ordered alphabetically.

### SA13xx - Naming Rules

#### SA1300 - Element Must Begin With Upper Case Letter
**Description**: Element names must begin with an upper case letter.

#### SA1301 - Element Must Begin With Lower Case Letter
**Description**: Element names must begin with a lower case letter.

#### SA1302 - Interface Names Must Begin With I
**Description**: Interface names must begin with 'I'.

#### SA1303 - Const Field Names Must Begin With Upper Case Letter
**Description**: Const field names must begin with upper case letters.

#### SA1304 - Non Private Readonly Fields Must Begin With Upper Case Letter
**Description**: Non-private readonly fields must begin with upper case letters.

#### SA1305 - Field Names Must Not Use Hungarian Notation
**Description**: Field names must not use Hungarian notation.

#### SA1306 - Field Names Must Begin With Lower Case Letter
**Description**: Field names must begin with lower case letters.

#### SA1307 - Accessible Fields Must Begin With Upper Case Letter
**Description**: Accessible fields must begin with upper case letters.

#### SA1308 - Variable Names Must Not Be Prefixed
**Description**: Variable names must not be prefixed inappropriately.

#### SA1309 - Field Names Must Not Begin With Underscore
**Description**: Field names must not begin with underscore.

**Configuration**: Often disabled to allow `_fieldName` convention.

#### SA1310 - Field Names Must Not Contain Underscore
**Description**: Field names must not contain underscores.

#### SA1311 - Static Readonly Fields Must Begin With Upper Case Letter
**Description**: Static readonly fields must begin with upper case letters.

#### SA1312 - Variable Names Must Begin With Lower Case Letter
**Description**: Variable names must begin with lower case letters.

#### SA1313 - Parameter Names Must Begin With Lower Case Letter
**Description**: Parameter names must begin with lower case letters.

#### SA1314 - Type Parameter Names Must Begin With T
**Description**: Type parameter names must begin with 'T'.

#### SA1315 - Type Parameter Names Must Be Unique
**Description**: Type parameter names must be unique.

#### SA1316 - Tuple Element Names Must Use Correct Casing
**Description**: Tuple element names must use correct casing.

### SA14xx - Maintainability Rules

#### SA1400 - Access Modifier Must Be Declared
**Description**: Access modifiers must be explicitly declared.

#### SA1401 - Fields Must Be Private
**Description**: Fields should be private.

#### SA1402 - File May Only Contain A Single Type
**Description**: Files should contain only a single type.

#### SA1403 - File May Only Contain A Single Namespace
**Description**: Files should contain only a single namespace.

#### SA1404 - Code Analysis Suppression Must Have Justification
**Description**: Code analysis suppressions must have justification.

#### SA1405 - Debug.Assert Must Provide Message
**Description**: Debug.Assert must provide a message.

#### SA1406 - Debug.Fail Must Provide Message
**Description**: Debug.Fail must provide a message.

#### SA1407 - Arithmetic Expressions Must Declare Precedence
**Description**: Arithmetic expressions must declare precedence with parentheses.

#### SA1408 - Conditional Expressions Must Declare Precedence
**Description**: Conditional expressions must declare precedence.

#### SA1409 - Remove Unnecessary Code
**Description**: Unnecessary code should be removed.

#### SA1410 - Remove Delegate Parenthesis When Possible
**Description**: Delegate parentheses should be removed when possible.

#### SA1411 - Attribute Constructor Should Not Use Unnecessary Parenthesis
**Description**: Attribute constructors should not use unnecessary parentheses.

#### SA1412 - Store Files As UTF-8 With Byte Order Mark
**Description**: Files should be stored as UTF-8 with byte order mark.

#### SA1413 - Use Trailing Comma In Multi-Line Initializers
**Description**: Use trailing commas in multi-line initializers.

#### SA1414 - Tuple Types In Signatures Should Have Element Names
**Description**: Tuple types in signatures should have element names.

### SA15xx - Layout Rules

#### SA1500 - Braces For Multi-Line Statements Must Not Share Line
**Description**: Braces for multi-line statements must not share lines.

#### SA1501 - Statement Must Not Be On Single Line
**Description**: Statements must not be on a single line.

#### SA1502 - Element Must Not Be On Single Line
**Description**: Elements must not be on a single line.

#### SA1503 - Braces Must Not Be Omitted
**Description**: Braces must not be omitted.

#### SA1504 - All Accessors Must Be Single-Line Or Multi-Line
**Description**: All accessors must be consistently formatted.

#### SA1505 - Opening Braces Must Not Be Followed By Blank Line
**Description**: Opening braces must not be followed by blank lines.

#### SA1506 - Element Documentation Headers Must Not Be Followed By Blank Line
**Description**: Element documentation headers must not be followed by blank lines.

#### SA1507 - Code Must Not Contain Multiple Blank Lines In A Row
**Description**: Code must not contain multiple blank lines in a row.

#### SA1508 - Closing Braces Must Not Be Preceded By Blank Line
**Description**: Closing braces must not be preceded by blank lines.

#### SA1509 - Opening Braces Should Not Be Preceded By Blank Line
**Description**: Opening braces should not be preceded by blank lines.

#### SA1510 - Chained Statement Blocks Must Not Be Preceded By Blank Line
**Description**: Chained statement blocks must not be preceded by blank lines.

#### SA1511 - While-Do Footer Must Not Be Preceded By Blank Line
**Description**: While-do footers must not be preceded by blank lines.

#### SA1512 - Single-Line Comments Must Not Be Followed By Blank Line
**Description**: Single-line comments must not be followed by blank lines.

#### SA1513 - Closing Brace Must Be Followed By Blank Line
**Description**: Closing braces must be followed by blank lines.

**Configuration**: Often disabled for compact code.

#### SA1514 - Element Documentation Header Must Be Preceded By Blank Line
**Description**: Element documentation headers must be preceded by blank lines.

#### SA1515 - Single-Line Comment Must Be Preceded By Blank Line
**Description**: Single-line comments must be preceded by blank lines.

#### SA1516 - Elements Must Be Separated By Blank Line
**Description**: Elements must be separated by blank lines.

#### SA1517 - Code Must Not Contain Blank Lines At Start Of File
**Description**: Files must not contain blank lines at the start.

#### SA1518 - Use Line Endings Correctly At End Of File
**Description**: Files must end with the correct line endings.

#### SA1519 - Braces Must Not Be Empty
**Description**: Braces must not be empty.

### SA16xx - Documentation Rules

#### SA1600 - Elements Must Be Documented
**Description**: Elements must be documented.

#### SA1601 - Partial Elements Must Be Documented
**Description**: Partial elements must be documented.

#### SA1602 - Enumeration Items Must Be Documented
**Description**: Enumeration items must be documented.

#### SA1603 - Documentation Must Contain Valid XML
**Description**: Documentation must contain valid XML.

#### SA1604 - Element Documentation Must Have Summary
**Description**: Element documentation must have a summary.

#### SA1605 - Partial Element Documentation Must Have Summary
**Description**: Partial element documentation must have a summary.

#### SA1606 - Element Documentation Must Have Summary Text
**Description**: Element documentation must have summary text.

#### SA1607 - Code Element Does Not Contain Summary Documentation
**Description**: Code elements must contain summary documentation.

#### SA1608 - Element Documentation Must Not Have Default Summary
**Description**: Element documentation must not have default summary.

#### SA1609 - Property Documentation Must Have Value
**Description**: Property documentation must have value.

#### SA1610 - Property Documentation Must Have Value Text
**Description**: Property documentation must have value text.

#### SA1611 - Element Parameters Must Be Documented
**Description**: Element parameters must be documented.

#### SA1612 - Element Parameter Documentation Must Match Element Parameters
**Description**: Element parameter documentation must match element parameters.

#### SA1613 - Element Parameter Documentation Must Declare Parameter Name
**Description**: Element parameter documentation must declare parameter name.

#### SA1614 - Element Parameter Documentation Must Have Text
**Description**: Element parameter documentation must have text.

#### SA1615 - Element Return Value Must Be Documented
**Description**: Element return values must be documented.

#### SA1616 - Element Return Value Documentation Must Have Text
**Description**: Element return value documentation must have text.

#### SA1617 - Void Return Value Must Not Be Documented
**Description**: Void return values must not be documented.

#### SA1618 - Generic Type Parameters Must Be Documented
**Description**: Generic type parameters must be documented.

#### SA1619 - Generic Type Parameter Documentation Must Have Text
**Description**: Generic type parameter documentation must have text.

#### SA1620 - Generic Type Parameter Documentation Must Match Type Parameters
**Description**: Generic type parameter documentation must match type parameters.

#### SA1621 - Generic Type Parameter Documentation Must Declare Parameter Name
**Description**: Generic type parameter documentation must declare parameter name.

#### SA1622 - Generic Type Parameters Must Be Documented Partial Class
**Description**: Generic type parameters must be documented in partial classes.

#### SA1623 - Property Summary Documentation Must Match Accessors
**Description**: Property summary documentation must match accessors.

#### SA1624 - Property Summary Documentation Must Omit Set Accessor With Restricted Access
**Description**: Property summary documentation must omit set accessor with restricted access.

#### SA1625 - Element Documentation Must Not Be Copied And Pasted
**Description**: Element documentation must not be copied and pasted.

#### SA1626 - Single Line Comments Must Not Use Documentation Style Slashes
**Description**: Single line comments must not use documentation style slashes.

#### SA1627 - Documentation Text Must Not Be Empty
**Description**: Documentation text must not be empty.

#### SA1628 - Documentation Text Must Begin With A Capital Letter
**Description**: Documentation text must begin with a capital letter.

#### SA1629 - Documentation Text Must End With A Period
**Description**: Documentation text must end with a period.

#### SA1630 - Documentation Text Must Contain Whitespace
**Description**: Documentation text must contain whitespace.

#### SA1631 - Documentation Must Meet Character Percentage
**Description**: Documentation must meet character percentage.

#### SA1632 - Documentation Text Must Meet Minimum Character Length
**Description**: Documentation text must meet minimum character length.

#### SA1633 - File Must Have Header
**Description**: Files must have header.

#### SA1634 - File Header Must Show Copyright
**Description**: File header must show copyright.

#### SA1635 - File Header Must Have Copyright Text
**Description**: File header must have copyright text.

#### SA1636 - File Header Copyright Text Must Match
**Description**: File header copyright text must match.

#### SA1637 - File Header Must Contain File Name
**Description**: File header must contain file name.

#### SA1638 - File Header File Name Documentation Must Match File Name
**Description**: File header file name documentation must match file name.

#### SA1639 - File Header Must Have Summary
**Description**: File header must have summary.

#### SA1640 - File Header Must Have Valid Company Text
**Description**: File header must have valid company text.

#### SA1641 - File Header Company Name Text Must Match
**Description**: File header company name text must match.

#### SA1642 - Constructor Summary Documentation Must Begin With Standard Text
**Description**: Constructor summary documentation must begin with standard text.

#### SA1643 - Destructor Summary Documentation Must Begin With Standard Text
**Description**: Destructor summary documentation must begin with standard text.

#### SA1644 - Documentation Headers Must Not Contain Blank Lines
**Description**: Documentation headers must not contain blank lines.

#### SA1645 - Included Documentation File Does Not Exist
**Description**: Included documentation file must exist.

#### SA1646 - Included Documentation XPath Does Not Exist
**Description**: Included documentation XPath must exist.

#### SA1647 - Include Node Does Not Contain Valid File And Path
**Description**: Include node must contain valid file and path.

#### SA1648 - Inheritdoc Must Be Used With Inheriting Class
**Description**: Inheritdoc must be used with inheriting class.

#### SA1649 - File Name Must Match Type Name
**Description**: File name must match type name.

**Configuration**: Often disabled for flexibility.

#### SA1650 - Element Documentation Must Be Spelled Correctly
**Description**: Element documentation must be spelled correctly.

#### SA1651 - Do Not Use Placeholder Elements
**Description**: Do not use placeholder elements.

### SXxxxx - Alternative Rules

#### SX1101 - Do Not Prefix Local Members With This
**Description**: Local members should not be prefixed with `this.`.

#### SX1309S - Static Field Names Must Begin With Underscore
**Description**: Static field names should begin with underscore.

#### SX1309T - Field Names Must Begin With Underscore
**Description**: Field names should begin with underscore.

---

## Resources

### Official Documentation
- [StyleCop Analyzers GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Configuration Guide](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md)
- [.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-csharp/coding-conventions)

### Community Resources
- [StyleCop Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/)
- [C# Coding Standards](https://github.com/ktaranov/naming-convention/blob/master/C%23%20Coding%20Standards.md)
- [Roslyn Analyzers](https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview)

---

**B2X StyleCop Configuration**: Updated for project conventions  
**Last Reviewed**: January 2026  
**Next Review**: July 2026
