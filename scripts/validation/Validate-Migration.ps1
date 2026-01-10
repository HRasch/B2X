# Migration Validation Script

param(
    [switch]$Quick,
    [switch]$Full
)

# Import validation module
Import-Module "$PSScriptRoot\Validate-Migration.psm1"

if ($Quick) {
    Write-Host "Running quick validation..."
    $dirOk = Test-DirectoryStructure
    Get-ProjectCounts
    $refsOk = Test-ProjectReferences

    if ($dirOk -and $refsOk) {
        Write-Host "✓ Quick validation passed"
    } else {
        Write-Host "❌ Quick validation failed"
    }
} elseif ($Full) {
    Write-Host "Running full validation..."
    Start-MigrationValidation
} else {
    Write-Host "Migration Validation Script"
    Write-Host "==========================="
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\Validate-Migration.ps1 -Quick    # Quick validation (structure + references)"
    Write-Host "  .\Validate-Migration.ps1 -Full     # Full validation (includes builds)"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\Validate-Migration.ps1 -Quick    # Fast validation after migration phase"
    Write-Host "  .\Validate-Migration.ps1 -Full     # Complete validation before commit"
    Write-Host ""
}</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\Validate-Migration.ps1