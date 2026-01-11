---
docid: BS-REFACTOR-GUIDE
title: Refactoring Guide
owner: @SARAH
status: Active
created: 2026-01-09
---
created: 2026-01-09
---

# B2X Project Refactoring Guide

## Overview

This guide documents the refactoring of the B2X project structure to follow modern .NET conventions with `src/`, `docs/`, and `tests/` directories at the root level.

## Goals

- **Better Organization**: Separate source code, documentation, and tests into dedicated directories
- **Standard Structure**: Follow common .NET project layout conventions
- **Maintain Compatibility**: Preserve all existing tooling and build processes
- **Safe Migration**: Zero breaking changes through careful planning and validation

## New Directory Structure

```
B2X/
├── src/                    # Source code (new)
│   ├── AppHost/           # Application host
│   ├── Backend/           # Backend services and domain
│   ├── Frontend/          # Frontend applications
│   ├── ServiceDefaults/   # Service defaults
│   ├── IdsConnectAdapter/ # Identity adapter
│   └── erp-connector/     # ERP connector
├── docs/                  # Documentation (new)
│   ├── project/           # Root-level docs moved here
│   └── developer/         # Developer documentation
├── tests/                 # Tests (new)
│   ├── AppHost.Tests/     # AppHost tests
│   └── integration/       # Integration tests
├── build/                 # Build artifacts (new)
├── config/                # Configuration files (new)
├── data/                  # Data files (new)
├── archive/               # Legacy files (new)
├── scripts/               # Build scripts (keep at root)
├── .ai/                   # AI tooling (keep at root)
├── .aspire/               # Aspire config (keep at root)
├── .husky/                # Git hooks (keep at root)
└── [root files]           # package.json, B2X.slnx, etc.
```

## Preserved Directories

The following directories remain at the root level to respect .NET tooling conventions:

- `.ai/` - AI-driven development tools
- `.aspire/` - .NET Aspire configuration
- `.husky/` - Git hooks
- `.githooks/` - Git hooks configuration
- `scripts/` - Build and utility scripts
- `tools/` - Development tools
- `monitoring/` - Monitoring configuration

## Migration Strategy

### Phase 1: Directory Creation & Low-Risk Moves
1. Create new directory structure
2. Move data files, documentation, configuration
3. Validate moves and commit

### Phase 2: Source Code Migration
1. Move source code directories (Backend/, Frontend/, etc.)
2. Update all file references and imports
3. Update project files and build configurations
4. Validate builds and tests

## Scripts Overview

### Master Script
```bash
./scripts/refactor-project.sh [phase]
```

**Phases:**
- `all` - Complete refactoring
- `structure` - Create directories only
- `phase1` - Move data/docs/config
- `phase2` - Move source code
- `validate` - Validate structure
- `update` - Update references
- `test` - Test builds
- `rollback` - Rollback changes

### Individual Scripts
- `create-refactor-structure.sh` - Creates new directory structure
- `move-files-phase1.sh` - Moves low-risk files
- `move-files-phase2.sh` - Moves source code
- `validate-moves.sh` - Validates all moves
- `update-references.sh` - Updates file references
- `test-builds.sh` - Tests builds and runs
- `rollback-refactor.sh` - Rollback utility

## Safety Features

### Git Branching Strategy
- `main` - Production branch (untouched)
- `refactor/backup` - Backup of current state
- `refactor/working` - Working branch for changes

### Validation Checks
- Git status validation before operations
- File existence checks before moves
- Build validation after each phase
- Reference integrity checks

### Rollback Capability
- Complete rollback script available
- Git commit after each phase
- Backup branch for recovery

## Execution Steps

### Preparation
```bash
# Ensure clean working directory
git status

# Switch to working branch
git checkout refactor/working

# Run complete refactoring
./scripts/refactor-project.sh all
```

### Step-by-Step Approach
```bash
# Phase 1: Structure and low-risk moves
./scripts/refactor-project.sh structure
./scripts/refactor-project.sh phase1

# Validate Phase 1
./scripts/refactor-project.sh validate

# Phase 2: Source code migration
./scripts/refactor-project.sh phase2

# Final validation
./scripts/refactor-project.sh test
```

## File Reference Updates

### C# Project Files
- Update `<ProjectReference>` paths
- Update `<PackageReference>` if needed
- Update output paths

### TypeScript/JavaScript
- Update import paths
- Update file references in config files

### Documentation
- Update links in README files
- Update paths in documentation

### Build Scripts
- Update paths in package.json scripts
- Update paths in shell scripts

## Testing Strategy

### Build Validation
- .NET projects build successfully
- Frontend builds complete
- Docker images build correctly

### Runtime Testing
- Services start without errors
- API endpoints respond
- Frontend loads correctly

### Integration Testing
- End-to-end tests pass
- Database connections work
- External service integrations function

## Risk Mitigation

### Breaking Changes Prevention
- Preserve exact file and directory casing
- Update all references systematically
- Test builds after each change

### Backup Strategy
- Git commits after each phase
- Backup branch with original state
- Rollback script for emergency recovery

### Validation Gates
- Automated validation scripts
- Manual review checkpoints
- Build and test verification

## Post-Refactoring Tasks

### Update Documentation
- Update README.md with new structure
- Update CONTRIBUTING.md
- Update any hardcoded paths in docs

### CI/CD Updates
- Update GitHub Actions workflows
- Update Docker build contexts
- Update deployment scripts

### Team Communication
- Notify team of new structure
- Update development guides
- Provide migration FAQ

## Troubleshooting

### Common Issues
- **Build failures**: Check file paths in project files
- **Import errors**: Verify import paths updated
- **Test failures**: Check test configuration paths

### Recovery Steps
```bash
# If issues occur during refactoring
./scripts/refactor-project.sh rollback

# Or manually reset to backup
git reset --hard refactor/backup
```

### Getting Help
- Check script output for specific errors
- Review git history for what changed
- Consult team for complex issues

## Success Criteria

- [ ] All directories created successfully
- [ ] All files moved to correct locations
- [ ] All references updated
- [ ] Builds pass without errors
- [ ] Tests execute successfully
- [ ] Services start and run
- [ ] No breaking changes to functionality

## Timeline

- **Phase 1**: 30 minutes (directory creation + low-risk moves)
- **Phase 2**: 1-2 hours (source code migration + reference updates)
- **Validation**: 30-60 minutes (builds + tests)
- **Total**: 2-3 hours for complete refactoring

## Contacts

- **Technical Lead**: @TechLead
- **Architecture**: @Architect
- **DevOps**: @DevOps
- **Coordinator**: @SARAH

---

*This refactoring maintains backward compatibility while establishing a cleaner, more maintainable project structure following .NET best practices.*