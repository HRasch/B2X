# B2Connect Project Structure Migration

This directory contains scripts to migrate the B2Connect project from its current dual-backend structure to a unified `src/backend/` organization with clear bounded contexts.

## Migration Overview

The migration reorganizes the project structure as follows:

```
src/
├── backend/
│   ├── Admin/              # Admin bounded context
│   ├── Store/              # Store bounded context
│   ├── Management/         # Management bounded context
│   ├── Infrastructure/     # Cross-cutting infrastructure
│   ├── Services/           # Background services
│   └── Shared/             # Shared kernel
├── frontend/               # All frontend applications
├── tools/                  # Development tools
└── tests/                  # Tests organized by bounded context
```

## Scripts Overview

### Migration Scripts

- **`Migrate-Project.ps1`** - Main migration script with phase-by-phase execution
- **`ProjectMigration.psm1`** - PowerShell module containing migration functions

### Validation Scripts

- **`Validate-Migration.ps1`** - Validation script to check migration results
- **`Validate-Migration.psm1`** - PowerShell module containing validation functions

## Usage

### Step 1: Preparation

1. Create a backup branch:
   ```bash
   git checkout -b refactor-project-structure-backup
   git push origin refactor-project-structure-backup
   ```

2. Switch to migration branch:
   ```bash
   git checkout refactor-project-structure
   ```

### Step 2: Dry Run

Test the migration without making changes:

```powershell
.\Migrate-Project.ps1 -DryRun -All
```

### Step 3: Phase-by-Phase Migration

Run migration phases individually:

```powershell
# Phase 1: Infrastructure
.\Migrate-Project.ps1 -Phase1

# Validate after each phase
.\Validate-Migration.ps1 -Quick

# Phase 2: Shared Components
.\Migrate-Project.ps1 -Phase2
.\Validate-Migration.ps1 -Quick

# Phase 3: Bounded Contexts
.\Migrate-Project.ps1 -Phase3
.\Validate-Migration.ps1 -Quick

# Phase 4: Services & Infrastructure
.\Migrate-Project.ps1 -Phase4
.\Validate-Migration.ps1 -Quick

# Phase 5: Frontend
.\Migrate-Project.ps1 -Phase5

# Phase 6: Tests
.\Migrate-Project.ps1 -Phase6
```

### Step 4: Final Validation

Run full validation:

```powershell
.\Validate-Migration.ps1 -Full
```

### Step 5: Update Solution File

After migration, update the solution file paths manually or use the reference update functions.

### Step 6: Commit and Test

```bash
# Commit migration results
git add .
git commit -m "refactor: migrate to src/backend/ bounded context structure

- Consolidate backend projects under src/backend/
- Organize by bounded contexts: Admin, Store, Management
- Move shared components to src/backend/Shared/
- Reorganize tests by bounded context
- Update project references"

# Run full test suite
dotnet test B2X.slnx
```

## Troubleshooting

### Build Failures

If builds fail after migration:

1. Check for broken project references:
   ```powershell
   .\Validate-Migration.ps1 -Quick
   ```

2. Manually update any remaining incorrect paths in .csproj files

3. Check the solution file (B2X.slnx) for incorrect paths

### Missing Files

If files are missing:

1. Check if they were moved to the correct location
2. Verify the migration phase completed successfully
3. Restore from backup if necessary

### Reference Issues

Common reference patterns that need updating:

- `../../../shared/` → `../../Shared/`
- `../../../Hosting/` → `../Infrastructure/Hosting/`
- `../../../src/shared/` → `../../Shared/`

## Rollback Plan

If migration fails:

1. Switch to backup branch:
   ```bash
   git checkout refactor-project-structure-backup
   ```

2. Reset migration branch:
   ```bash
   git checkout refactor-project-structure
   git reset --hard origin/refactor-project-structure
   ```

3. Identify and fix issues in the migration scripts

## Files Created During Migration

- `PROJECT_RESTRUCTURE_ANALYSIS.md` - Detailed analysis of current structure
- `PROJECT_RESTRUCTURE_MIGRATION_PLAN.md` - Step-by-step migration plan
- `scripts/ProjectMigration.psm1` - Migration functions
- `scripts/Migrate-Project.ps1` - Main migration script
- `scripts/Validate-Migration.psm1` - Validation functions
- `scripts/Validate-Migration.ps1` - Validation script

## Post-Migration Tasks

1. Update documentation (README, architecture docs)
2. Update CI/CD pipelines
3. Update Docker configurations
4. Update any scripts that reference old paths
5. Train team on new structure

## Support

If you encounter issues:

1. Check the validation output for specific errors
2. Review the migration logs for failed operations
3. Check the analysis documents for expected structure
4. Create an issue with the specific error details</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\README-Migration.md