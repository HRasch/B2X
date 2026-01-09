# B2Connect Project Structure Migration Script
# Run this script to migrate the project structure to src/backend/ organization

param(
    [switch]$DryRun,
    [switch]$Phase1,
    [switch]$Phase2,
    [switch]$Phase3,
    [switch]$Phase4,
    [switch]$Phase5,
    [switch]$Phase6,
    [switch]$All
)

# Import migration module
Import-Module "$PSScriptRoot\ProjectMigration.psm1"

if ($DryRun) {
    Write-Host "DRY RUN MODE - No changes will be made"
    Write-Host "========================================="
}

if ($All) {
    Write-Host "Running complete migration..."
    if (-not $DryRun) {
        Start-ProjectMigration
    } else {
        Write-Host "Would run: Start-ProjectMigration"
    }
} elseif ($Phase1) {
    Write-Host "Running Phase 1: Infrastructure Migration..."
    if (-not $DryRun) {
        Move-Infrastructure
    } else {
        Write-Host "Would run: Move-Infrastructure"
    }
} elseif ($Phase2) {
    Write-Host "Running Phase 2: Shared Components Consolidation..."
    if (-not $DryRun) {
        Consolidate-SharedComponents
    } else {
        Write-Host "Would run: Consolidate-SharedComponents"
    }
} elseif ($Phase3) {
    Write-Host "Running Phase 3: Bounded Context Migration..."
    if (-not $DryRun) {
        Move-BoundedContexts
    } else {
        Write-Host "Would run: Move-BoundedContexts"
    }
} elseif ($Phase4) {
    Write-Host "Running Phase 4: Services and Infrastructure..."
    if (-not $DryRun) {
        Move-ServicesAndInfrastructure
    } else {
        Write-Host "Would run: Move-ServicesAndInfrastructure"
    }
} elseif ($Phase5) {
    Write-Host "Running Phase 5: Frontend Migration..."
    if (-not $DryRun) {
        Move-Frontend
    } else {
        Write-Host "Would run: Move-Frontend"
    }
} elseif ($Phase6) {
    Write-Host "Running Phase 6: Test Reorganization..."
    if (-not $DryRun) {
        Reorganize-Tests
    } else {
        Write-Host "Would run: Reorganize-Tests"
    }
} else {
    Write-Host "B2Connect Project Migration Script"
    Write-Host "==================================="
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\Migrate-Project.ps1 -All              # Run complete migration"
    Write-Host "  .\Migrate-Project.ps1 -Phase1           # Phase 1: Infrastructure"
    Write-Host "  .\Migrate-Project.ps1 -Phase2           # Phase 2: Shared Components"
    Write-Host "  .\Migrate-Project.ps1 -Phase3           # Phase 3: Bounded Contexts"
    Write-Host "  .\Migrate-Project.ps1 -Phase4           # Phase 4: Services & Infra"
    Write-Host "  .\Migrate-Project.ps1 -Phase5           # Phase 5: Frontend"
    Write-Host "  .\Migrate-Project.ps1 -Phase6           # Phase 6: Tests"
    Write-Host "  .\Migrate-Project.ps1 -DryRun -All      # Dry run complete migration"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\Migrate-Project.ps1 -DryRun -Phase1   # Test Phase 1 without changes"
    Write-Host "  .\Migrate-Project.ps1 -Phase1           # Run Phase 1 migration"
    Write-Host ""
}</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\Migrate-Project.ps1