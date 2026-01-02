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

For **B2Connect (.NET 10)**, use **1.2.0-beta.556** for full modern C# support:

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
      "companyName": "B2Connect GmbH",
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
stylecop.documentationRules.companyName = B2Connect GmbH
stylecop.documentationRules.documentInternalElements = false

# File header
file_header_template = Copyright (c) B2Connect GmbH. All rights reserved.
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

## Related Topics

- [.NET Code Analysis](./dotnet-code-analysis.md)
- [EditorConfig](./editorconfig.md)
- [Code Quality Guidelines](../../guidelines/)

---

## Version History

| Date | Version | Changes |
|------|---------|---------|
| 2026-01-02 | 1.0 | Initial documentation |

---

**Next Review**: April 2026
