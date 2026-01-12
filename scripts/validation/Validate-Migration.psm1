# Project Migration Validation Script

# Function to validate directory structure
function Test-DirectoryStructure {
    Write-Host "Validating directory structure..."

    $requiredDirs = @(
        "src/backend/Admin",
        "src/backend/Store",
        "src/backend/Management",
        "src/backend/Infrastructure",
        "src/backend/Services",
        "src/backend/Shared",
        "src/frontend",
        "src/tools",
        "src/tests"
    )

    $missingDirs = @()
    foreach ($dir in $requiredDirs) {
        if (-not (Test-Path $dir)) {
            $missingDirs += $dir
        }
    }

    if ($missingDirs.Count -gt 0) {
        Write-Warning "Missing directories:"
        $missingDirs | ForEach-Object { Write-Warning "  $_" }
        return $false
    } else {
        Write-Host "✓ All required directories exist"
        return $true
    }
}

# Function to count projects in each area
function Get-ProjectCounts {
    Write-Host "Project counts by area:"

    $areas = @(
        @{Name="Admin"; Path="src/backend/Admin"},
        @{Name="Store"; Path="src/backend/Store"},
        @{Name="Management"; Path="src/backend/Management"},
        @{Name="Infrastructure"; Path="src/backend/Infrastructure"},
        @{Name="Services"; Path="src/backend/Services"},
        @{Name="Shared"; Path="src/backend/Shared"},
        @{Name="Frontend"; Path="src/frontend"},
        @{Name="Tools"; Path="src/tools"},
        @{Name="Tests"; Path="src/tests"}
    )

    foreach ($area in $areas) {
        $count = (Get-ChildItem -Path $area.Path -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue | Measure-Object).Count
        Write-Host "  $($area.Name): $count projects"
    }
}

# Function to validate build
function Test-ProjectBuilds {
    Write-Host "Testing project builds..."

    $failedBuilds = @()

    # Test individual bounded contexts
    $boundedContexts = @("Admin", "Store", "Management")
    foreach ($bc in $boundedContexts) {
        Write-Host "Testing $bc bounded context..."
        $projects = Get-ChildItem -Path "src/backend/$bc" -Filter "*.csproj" -Recurse
        foreach ($project in $projects) {
            $buildResult = dotnet build $project.FullName 2>&1
            if ($LASTEXITCODE -ne 0) {
                $failedBuilds += "$bc/$($project.Name)"
                Write-Warning "Failed to build: $bc/$($project.Name)"
            }
        }
    }

    # Test shared components
    Write-Host "Testing shared components..."
    $sharedProjects = Get-ChildItem -Path "src/backend/Shared" -Filter "*.csproj" -Recurse
    foreach ($project in $sharedProjects) {
        $buildResult = dotnet build $project.FullName 2>&1
        if ($LASTEXITCODE -ne 0) {
            $failedBuilds += "Shared/$($project.Name)"
            Write-Warning "Failed to build: Shared/$($project.Name)"
        }
    }

    if ($failedBuilds.Count -gt 0) {
        Write-Error "Build failures detected:"
        $failedBuilds | ForEach-Object { Write-Error "  $_" }
        return $false
    } else {
        Write-Host "✓ All projects build successfully"
        return $true
    }
}

# Function to validate solution build
function Test-SolutionBuild {
    Write-Host "Testing solution build..."

    $buildResult = dotnet build B2X.slnx 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Solution build failed!"
        Write-Host $buildResult
        return $false
    } else {
        Write-Host "✓ Solution builds successfully"
        return $true
    }
}

# Function to check for remaining old directories
function Test-OldDirectories {
    Write-Host "Checking for remaining old directories..."

    $oldDirs = @(
        "backend",
        "Frontend",
        "AppHost",
        "ServiceDefaults"
    )

    $remainingDirs = @()
    foreach ($dir in $oldDirs) {
        if (Test-Path $dir) {
            $remainingDirs += $dir
        }
    }

    if ($remainingDirs.Count -gt 0) {
        Write-Warning "Old directories still exist (can be removed after validation):"
        $remainingDirs | ForEach-Object { Write-Warning "  $_" }
        return $false
    } else {
        Write-Host "✓ No old directories remain"
        return $true
    }
}

# Function to validate project references
function Test-ProjectReferences {
    Write-Host "Validating project references..."

    $brokenRefs = @()

    Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
        $projectFile = $_.FullName
        $projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
        $content = Get-Content $projectFile -Raw

        # Find all ProjectReference elements
        $references = [regex]::Matches($content, '<ProjectReference Include="([^"]*)"')

        foreach ($ref in $references) {
            $refPath = $ref.Groups[1].Value
            if ($refPath -notmatch "^\$\(MSBuildThisFileDirectory\)") {
                # Convert relative path to absolute
                $absolutePath = [System.IO.Path]::GetFullPath(
                    [System.IO.Path]::Combine($projectDir, $refPath)
                )

                if (-not (Test-Path $absolutePath)) {
                    $brokenRefs += "$($_.Name): $refPath"
                }
            }
        }
    }

    if ($brokenRefs.Count -gt 0) {
        Write-Error "Broken project references found:"
        $brokenRefs | ForEach-Object { Write-Error "  $_" }
        return $false
    } else {
        Write-Host "✓ All project references are valid"
        return $true
    }
}

# Main validation function
function Start-MigrationValidation {
    Write-Host "B2Connect Project Migration Validation"
    Write-Host "====================================="
    Write-Host ""

    $results = @()

    # Test directory structure
    $results += Test-DirectoryStructure

    # Get project counts
    Get-ProjectCounts

    # Test builds
    $results += Test-ProjectBuilds
    $results += Test-SolutionBuild

    # Test references
    $results += Test-ProjectReferences

    # Check old directories
    $results += Test-OldDirectories

    Write-Host ""
    Write-Host "Validation Summary:"
    Write-Host "==================="

    $passed = ($results | Where-Object { $_ -eq $true }).Count
    $total = $results.Count

    Write-Host "Passed: $passed/$total"

    if ($passed -eq $total) {
        Write-Host "🎉 Migration validation successful!"
        return $true
    } else {
        Write-Host "❌ Migration validation failed. Please review errors above."
        return $false
    }
}

# Export functions
Export-ModuleMember -Function *</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\Validate-Migration.psm1