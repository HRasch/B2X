# Project Structure Migration Scripts

# This script contains PowerShell functions to migrate the B2Connect project structure

# Function to create target directory structure
function New-TargetDirectories {
    $targetDirs = @(
        "src/backend/Admin/API",
        "src/backend/Admin/Domain",
        "src/backend/Admin/Infrastructure",
        "src/backend/Admin/Tests",
        "src/backend/Store/API",
        "src/backend/Store/Domain",
        "src/backend/Store/Infrastructure",
        "src/backend/Store/Tests",
        "src/backend/Management/API",
        "src/backend/Management/Domain",
        "src/backend/Management/Infrastructure",
        "src/backend/Management/Tests",
        "src/backend/Infrastructure/Hosting",
        "src/backend/Infrastructure/Monitoring",
        "src/backend/Infrastructure/Messaging",
        "src/backend/Infrastructure/Search",
        "src/backend/Infrastructure/Connectors",
        "src/backend/Infrastructure/ERP",
        "src/backend/Services/PunchoutAdapters",
        "src/backend/Services/Search",
        "src/backend/Shared/Domain",
        "src/backend/Shared/Infrastructure",
        "src/backend/Shared/CLI",
        "src/frontend",
        "src/tools/AI",
        "src/tools/MCP",
        "src/tools/seeders",
        "src/tests/Admin",
        "src/tests/Store",
        "src/tests/Management",
        "src/tests/Infrastructure",
        "src/tests/Services",
        "src/tests/Shared"
    )

    foreach ($dir in $targetDirs) {
        if (-not (Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force
            Write-Host "Created directory: $dir"
        }
    }
}

# Function to move items safely
function Move-ProjectItems {
    param(
        [string]$sourcePath,
        [string]$destinationPath,
        [switch]$Recurse
    )

    if (Test-Path $sourcePath) {
        if ($Recurse) {
            Copy-Item $sourcePath $destinationPath -Recurse -Force
        } else {
            Copy-Item $sourcePath $destinationPath -Force
        }
        Write-Host "Copied: $sourcePath -> $destinationPath"
    } else {
        Write-Warning "Source path not found: $sourcePath"
    }
}

# Phase 1: Infrastructure Migration
function Move-Infrastructure {
    Write-Host "Phase 1: Moving infrastructure components..."

    # Move AppHost and ServiceDefaults
    Move-ProjectItems "AppHost" "src/backend/Infrastructure/Hosting/AppHost" -Recurse
    Move-ProjectItems "ServiceDefaults" "src/backend/Infrastructure/Hosting/ServiceDefaults" -Recurse

    # Move tools
    Move-ProjectItems "tools" "src/tools" -Recurse

    Write-Host "Infrastructure migration completed."
}

# Phase 2: Shared Components Consolidation
function Consolidate-SharedComponents {
    Write-Host "Phase 2: Consolidating shared components..."

    # From backend/shared/
    Move-ProjectItems "backend/shared/*" "src/backend/Shared/Infrastructure/" -Recurse

    # From backend/CLI/
    Move-ProjectItems "backend/CLI/*" "src/backend/Shared/CLI/" -Recurse

    # From src/shared/
    Move-ProjectItems "src/shared/Domain/*" "src/backend/Shared/Domain/" -Recurse
    Move-ProjectItems "src/shared/B2X.Shared.*" "src/backend/Shared/Infrastructure/" -Recurse

    Write-Host "Shared components consolidation completed."
}

# Phase 3: Bounded Context Migration
function Move-BoundedContexts {
    Write-Host "Phase 3: Migrating bounded contexts..."

    # Admin bounded context
    Move-ProjectItems "src/Admin/*" "src/backend/Admin/" -Recurse
    Move-ProjectItems "backend/BoundedContexts/Admin/*" "src/backend/Admin/Domain/" -Recurse

    # Store bounded context
    Move-ProjectItems "src/Store/*" "src/backend/Store/" -Recurse
    Move-ProjectItems "backend/Domain/Catalog" "src/backend/Store/Domain/Catalog" -Recurse
    Move-ProjectItems "backend/Domain/Customer" "src/backend/Store/Domain/Customer" -Recurse
    Move-ProjectItems "backend/Domain/Orders" "src/backend/Store/Domain/Orders" -Recurse
    Move-ProjectItems "backend/Domain/Search" "src/backend/Store/Domain/Search" -Recurse

    # Management bounded context
    Move-ProjectItems "src/Management/*" "src/backend/Management/" -Recurse
    Move-ProjectItems "backend/Domain/CMS" "src/backend/Management/Domain/CMS" -Recurse
    Move-ProjectItems "backend/Domain/Email" "src/backend/Management/Domain/Email" -Recurse

    Write-Host "Bounded contexts migration completed."
}

# Phase 4: Services and Infrastructure
function Move-ServicesAndInfrastructure {
    Write-Host "Phase 4: Moving services and infrastructure..."

    # Services
    Move-ProjectItems "backend/services/*" "src/backend/Services/" -Recurse
    Move-ProjectItems "src/services/*" "src/backend/Services/" -Recurse

    # Infrastructure components
    Move-ProjectItems "backend/Connectors/*" "src/backend/Infrastructure/Connectors/" -Recurse
    Move-ProjectItems "src/Connectors/*" "src/backend/Infrastructure/Connectors/" -Recurse
    Move-ProjectItems "src/erp-connector/*" "src/backend/Infrastructure/ERP/" -Recurse
    Move-ProjectItems "src/IdsConnectAdapter/*" "src/backend/Services/PunchoutAdapters/" -Recurse

    Write-Host "Services and infrastructure migration completed."
}

# Phase 5: Frontend Migration
function Move-Frontend {
    Write-Host "Phase 5: Moving frontend applications..."

    Move-ProjectItems "Frontend/*" "src/frontend/" -Recurse

    Write-Host "Frontend migration completed."
}

# Phase 6: Test Reorganization
function Reorganize-Tests {
    Write-Host "Phase 6: Reorganizing tests..."

    Move-ProjectItems "tests/Admin/*" "src/tests/Admin/" -Recurse
    Move-ProjectItems "tests/Store/*" "src/tests/Store/" -Recurse
    Move-ProjectItems "tests/Management/*" "src/tests/Management/" -Recurse
    Move-ProjectItems "backend/Tests/*" "src/tests/" -Recurse
    Move-ProjectItems "tests/shared/*" "src/tests/Shared/" -Recurse
    Move-ProjectItems "tests/services/*" "src/tests/Services/" -Recurse

    Write-Host "Test reorganization completed."
}

# Function to update project references
function Update-ProjectReferences {
    param([string]$projectFile)

    $content = Get-Content $projectFile -Raw

    # Update common path patterns
    $content = $content -replace "../../../shared/", "../../Shared/"
    $content = $content -replace "../../../../src/shared/", "../../Shared/"
    $content = $content -replace "../../../Hosting/", "../Infrastructure/Hosting/"
    $content = $content -replace "../../../src/Hosting/", "../Infrastructure/Hosting/"
    $content = $content -replace "../../../src/shared/", "../../Shared/"
    $content = $content -replace "../../../Backend/", "../"
    $content = $content -replace "../../../Domain/", "../Domain/"

    Set-Content $projectFile $content
    Write-Host "Updated references in: $projectFile"
}

# Function to update all project references
function Update-AllProjectReferences {
    Write-Host "Updating project references..."

    Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | ForEach-Object {
        Update-ProjectReferences $_.FullName
    }

    Write-Host "Project references update completed."
}

# Function to validate build
function Test-Build {
    Write-Host "Running build validation..."

    $buildResult = dotnet build B2X.slnx 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        Write-Host $buildResult
        return $false
    } else {
        Write-Host "Build successful!"
        return $true
    }
}

# Function to validate references
function Test-References {
    Write-Host "Validating project references..."

    $brokenRefs = @()

    Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | ForEach-Object {
        $projectFile = $_.FullName
        $content = Get-Content $projectFile -Raw

        # Find all ProjectReference elements
        $references = [regex]::Matches($content, '<ProjectReference Include="([^"]*)"')

        foreach ($ref in $references) {
            $refPath = $ref.Groups[1].Value
            if ($refPath -notmatch "^\$\(MSBuildThisFileDirectory\)") {
                # Convert relative path to absolute
                $absolutePath = [System.IO.Path]::GetFullPath(
                    [System.IO.Path]::Combine(
                        [System.IO.Path]::GetDirectoryName($projectFile),
                        $refPath
                    )
                )

                if (-not (Test-Path $absolutePath)) {
                    $brokenRefs += "Broken reference in $($_.Name): $refPath"
                }
            }
        }
    }

    if ($brokenRefs.Count -gt 0) {
        Write-Warning "Found $($brokenRefs.Count) broken references:"
        $brokenRefs | ForEach-Object { Write-Warning $_ }
        return $false
    } else {
        Write-Host "All references are valid!"
        return $true
    }
}

# Main migration function
function Start-ProjectMigration {
    Write-Host "Starting B2Connect project structure migration..."
    Write-Host "============================================"

    # Phase 1: Preparation
    Write-Host "Phase 1: Creating target directories..."
    New-TargetDirectories

    # Phase 2: Infrastructure
    Write-Host "Phase 2: Moving infrastructure..."
    Move-Infrastructure

    if (-not (Test-Build)) { return }

    # Phase 3: Shared Components
    Write-Host "Phase 3: Consolidating shared components..."
    Consolidate-SharedComponents

    if (-not (Test-Build)) { return }

    # Phase 4: Bounded Contexts
    Write-Host "Phase 4: Migrating bounded contexts..."
    Move-BoundedContexts

    if (-not (Test-Build)) { return }

    # Phase 5: Services and Infrastructure
    Write-Host "Phase 5: Moving services and infrastructure..."
    Move-ServicesAndInfrastructure

    if (-not (Test-Build)) { return }

    # Phase 6: Frontend
    Write-Host "Phase 6: Moving frontend..."
    Move-Frontend

    # Phase 7: Tests
    Write-Host "Phase 7: Reorganizing tests..."
    Reorganize-Tests

    # Phase 8: Update References
    Write-Host "Phase 8: Updating project references..."
    Update-AllProjectReferences

    # Final Validation
    Write-Host "Final validation..."
    if ((Test-Build) -and (Test-References)) {
        Write-Host "Migration completed successfully!"
    } else {
        Write-Error "Migration completed with issues. Please review errors above."
    }
}

# Export functions for use
Export-ModuleMember -Function * -Alias *</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\ProjectMigration.psm1