---
docid: BS-REFACTOR-PATH-MAPPING
title: Refactoring Path Mapping
owner: @SARAH
status: Active
created: 2026-01-09
---

# B2X Project Structure: Current vs Planned
# Path Adjustment Reference Guide

## 📁 Current Structure (Flat Layout)

```
B2X/
├── AppHost/                          # Application host
│   ├── B2X.AppHost.csproj
│   ├── Program.cs
│   ├── Controllers/
│   ├── Services/
│   ├── Configuration/
│   ├── Extensions/
│   ├── Validation/
│   ├── Views/
│   ├── appsettings*.json
│   └── Properties/
├── AppHost.Tests/                    # AppHost unit tests
│   ├── B2X.AppHost.Tests.csproj
│   └── SeedingInfrastructureTests.cs
├── Backend/                          # Backend services & domain
│   ├── Api/
│   ├── BoundedContexts/
│   ├── CLI/
│   ├── Connectors/
│   ├── Domain/
│   ├── Gateway/
│   ├── Tests/
│   ├── services/
│   ├── shared/
│   ├── docs/
│   ├── kubernetes/
│   ├── CodeStyle*.cs
│   ├── Directory.Build.props
│   ├── stylecop.json
│   └── README.md
├── Frontend/                         # Frontend applications
│   ├── Admin/
│   ├── Management/
│   ├── Store/
│   └── package.json
├── docs/                             # Documentation
│   ├── api/
│   ├── architecture/
│   ├── backend/
│   ├── developer/
│   ├── features/
│   ├── frontend/
│   ├── guides/
│   ├── reports/
│   ├── user/
│   └── user-guides/
├── ServiceDefaults/                  # Service defaults
├── IdsConnectAdapter/                # Identity adapter
├── erp-connector/                    # ERP connector
├── B2X.Seeding.API/                  # Seeding API
├── test-data/                        # Test data files
├── mock-db*.json                     # Mock database files
├── scripts/                          # Build scripts
├── monitoring/                       # Monitoring config
├── tools/                            # Development tools
├── artifacts/                        # Build artifacts
├── benchmark-results/                # Performance benchmarks
├── coverage.json                     # Coverage reports
├── identity-coverage.json            # Identity coverage
├── *.md                              # Root documentation files
├── package.json                      # Root package config
├── B2X.slnx                          # Solution file
├── Directory.Packages.props          # Package management
└── [config files]                    # .gitignore, .editorconfig, etc.
```

## 🎯 Planned New Structure (src/docs/tests Layout)

```
B2X/
├── src/                              # Source code
│   ├── AppHost/                      # Application host (moved)
│   │   ├── B2X.AppHost.csproj
│   │   ├── Program.cs
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Configuration/
│   │   ├── Extensions/
│   │   ├── Validation/
│   │   ├── Views/
│   │   ├── appsettings*.json
│   │   └── Properties/
│   ├── Backend/                      # Backend services (moved)
│   │   ├── Api/
│   │   ├── BoundedContexts/
│   │   ├── CLI/
│   │   ├── Connectors/
│   │   ├── Domain/
│   │   ├── Gateway/
│   │   ├── Tests/
│   │   ├── services/
│   │   ├── shared/
│   │   ├── docs/
│   │   ├── kubernetes/
│   │   ├── CodeStyle*.cs
│   │   ├── Directory.Build.props
│   │   ├── stylecop.json
│   │   └── README.md
│   ├── Frontend/                     # Frontend apps (moved)
│   │   ├── Admin/
│   │   ├── Management/
│   │   ├── Store/
│   │   └── package.json
│   ├── ServiceDefaults/              # Service defaults (moved)
│   ├── IdsConnectAdapter/            # Identity adapter (moved)
│   └── erp-connector/                # ERP connector (moved)
├── docs/                             # Documentation
│   ├── project/                      # Root docs moved here
│   │   ├── README.md
│   │   ├── QUICK_START_GUIDE.md
│   │   ├── CONTRIBUTING.md
│   │   ├── GOVERNANCE.md
│   │   ├── SECURITY.md
│   │   └── [other root .md files]
│   └── developer/                    # Existing docs (preserved)
│       ├── api/
│       ├── architecture/
│       ├── backend/
│       ├── developer/
│       ├── features/
│       ├── frontend/
│       ├── guides/
│       ├── reports/
│       ├── user/
│       └── user-guides/
├── tests/                            # Test projects
│   ├── AppHost.Tests/                # AppHost tests (moved)
│   │   ├── B2X.AppHost.Tests.csproj
│   │   └── SeedingInfrastructureTests.cs
│   └── integration/                  # Integration tests (future)
├── data/                             # Data files
│   ├── mock-db.json
│   ├── mock-db 2.json
│   └── test-data/
├── config/                           # Configuration files
│   ├── appsettings.schema.json       # Schema files
│   └── [other config files]
├── build/                            # Build artifacts
├── archive/                          # Legacy files
├── scripts/                          # Build scripts (preserved)
├── monitoring/                       # Monitoring (preserved)
├── tools/                            # Tools (preserved)
├── artifacts/                        # Artifacts (preserved)
├── benchmark-results/                # Benchmarks (preserved)
├── .ai/                              # AI tooling (preserved)
├── .aspire/                          # Aspire config (preserved)
├── .husky/                           # Git hooks (preserved)
├── .github/                          # GitHub config (preserved)
├── .vscode/                          # VS Code config (preserved)
├── package.json                      # Root package (preserved)
├── B2X.slnx                          # Solution file (preserved)
├── Directory.Packages.props          # Package mgmt (preserved)
└── [other root files]                # Config files (preserved)
```

## 🔄 Path Mapping Reference

### For C# Project References (.csproj files)

| Current Path | New Path | Pattern Replacement |
|--------------|----------|-------------------|
| `../AppHost/B2X.AppHost.csproj` | `../src/AppHost/B2X.AppHost.csproj` | `../AppHost/` → `../src/AppHost/` |
| `../Backend/Api/B2X.Api.csproj` | `../src/Backend/Api/B2X.Api.csproj` | `../Backend/` → `../src/Backend/` |
| `../Frontend/Store/package.json` | `../src/Frontend/Store/package.json` | `../Frontend/` → `../src/Frontend/` |

### For C# Code References

| Current Reference | New Reference | File Types |
|------------------|---------------|------------|
| `using B2X.Backend.Domain;` | `using B2X.src.Backend.Domain;` | .cs files |
| `namespace B2X.Backend.Api` | `namespace B2X.src.Backend.Api` | .cs files |
| `Backend/Domain/Entities/` | `src/Backend/Domain/Entities/` | All references |

### For TypeScript/JavaScript Imports

| Current Import | New Import | File Types |
|----------------|------------|------------|
| `import { Api } from '../../Backend/Api'` | `import { Api } from '../../src/Backend/Api'` | .ts, .js, .vue |
| `import config from '../../../Frontend/Store/config'` | `import config from '../../../src/Frontend/Store/config'` | .ts, .js, .vue |

### For Documentation Links

| Current Link | New Link | File Types |
|--------------|----------|------------|
| `[Backend API](../Backend/Api/README.md)` | `[Backend API](../src/Backend/Api/README.md)` | .md files |
| `[Frontend Guide](../Frontend/Store/docs/guide.md)` | `[Frontend Guide](../src/Frontend/Store/docs/guide.md)` | .md files |
| `[AppHost Config](../AppHost/appsettings.json)` | `[AppHost Config](../src/AppHost/appsettings.json)` | .md files |

### For Configuration Files

| Current Path | New Path | File Types |
|--------------|----------|------------|
| `../Backend/appsettings.Development.json` | `../src/Backend/appsettings.Development.json` | .json, .yml |
| `../Frontend/Store/package.json` | `../src/Frontend/Store/package.json` | .json, .yml |

### For Build Scripts

| Current Path | New Path | File Types |
|--------------|----------|------------|
| `./Backend/run-tests.sh` | `./src/Backend/run-tests.sh` | .sh, .ps1 |
| `./Frontend/Store/build.sh` | `./src/Frontend/Store/build.sh` | .sh, .ps1 |

## 📋 Quick Reference: Files to Update

### High Priority (Break Builds)
- **77 .csproj files**: Project references
- **843 .cs files**: Namespace declarations, using statements
- **36 .ts/.js/.vue files**: Import statements

### Medium Priority (Break Runtime)
- **84 .json files**: Configuration paths
- **14 .yml/.yaml files**: Docker, CI/CD paths
- **50 .sh files**: Build script paths

### Low Priority (Break Documentation)
- **493 .md files**: Documentation links
- **5 .txt files**: Path references
- **5 .html files**: Link references

## 🔧 Path Adjustment Patterns

### Regex Patterns for Bulk Replacement

#### C# Files (.cs)
```regex
# Namespace updates
Find: namespace B2X\.Backend\.
Replace: namespace B2X.src.Backend.

# Using statements
Find: using B2X\.Backend\.
Replace: using B2X.src.Backend.

# File path references
Find: "Backend/
Replace: "src/Backend/
```

#### TypeScript/JavaScript (.ts, .js, .vue)
```regex
# Import statements
Find: from ['"]\.\./\.\./Backend/
Replace: from '../../src/Backend/

Find: from ['"]\.\./\.\./Frontend/
Replace: from '../../src/Frontend/
```

#### Markdown Files (.md)
```regex
# Relative links
Find: \(\.\./Backend/
Replace: (../src/Backend/

Find: \(\.\./Frontend/
Replace: (../src/Frontend/

Find: \(\.\./AppHost/
Replace: (../src/AppHost/
```

#### Project Files (.csproj)
```regex
# Project references
Find: <ProjectReference Include="\.\./Backend/
Replace: <ProjectReference Include="../src/Backend/

Find: <ProjectReference Include="\.\./Frontend/
Replace: <ProjectReference Include="../src/Frontend/
```

## ✅ Validation Checklist

After path adjustments, verify:

- [ ] .NET projects build successfully
- [ ] TypeScript compilation passes
- [ ] All relative imports resolve
- [ ] Documentation links work
- [ ] Build scripts execute
- [ ] Configuration files load
- [ ] Tests run successfully

## 🚨 Common Path Issues

### Issue: Circular References
**Symptom**: Build fails with circular dependency errors
**Cause**: Incorrect relative paths after move
**Fix**: Verify all `<ProjectReference>` paths are correct

### Issue: Import Resolution
**Symptom**: TypeScript/ESLint errors for missing modules
**Cause**: Import paths not updated
**Fix**: Update relative import paths in .ts/.vue files

### Issue: Documentation Links
**Symptom**: Broken links in README files
**Cause**: Relative paths not updated
**Fix**: Update markdown link references

### Issue: Configuration Loading
**Symptom**: Apps can't find config files
**Cause**: appsettings.json paths incorrect
**Fix**: Update configuration file references

## 📊 Impact Summary

- **Directories Moved**: 7 (AppHost, Backend, Frontend, ServiceDefaults, IdsConnectAdapter, erp-connector, AppHost.Tests)
- **Files Moved**: ~92 total
- **References to Update**: ~1,680 files
- **High-Risk Updates**: 843 C# files, 77 project files
- **Build Impact**: HIGH (requires all references updated)
- **Test Impact**: MEDIUM (test paths may need updates)

---

**Use this guide to systematically update all path references during the refactoring phases.**