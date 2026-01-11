param(
    [Parameter(Mandatory=$true)]
    [string]$TargetPath
)

Write-Host "Running namespace migration validation..." -ForegroundColor Green

# Check for build errors
Write-Host "`n1. Checking build..." -ForegroundColor Yellow
$buildResult = dotnet build $TargetPath --verbosity quiet 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    Write-Host $buildResult
    exit 1
} else {
    Write-Host "✅ Build successful" -ForegroundColor Green
}

# Check for namespace consistency
Write-Host "`n2. Checking namespace consistency..." -ForegroundColor Yellow
$csFiles = Get-ChildItem -Path $TargetPath -Filter "*.cs" -Recurse
$namespaceIssues = @()

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw

    # Extract namespace from file
    if ($content -match "namespace (B2X\.[^;\s]+)") {
        $actualNamespace = $matches[1]

        # Calculate expected namespace from file path
        $relativePath = $file.FullName.Replace($TargetPath, "").TrimStart("\\").Replace("\", "/")
        $expectedNamespace = "B2X"

        # Split path and build namespace
        $pathParts = $relativePath -split "/"
        $inSrc = $false

        for ($i = 0; $i -lt $pathParts.Length; $i++) {
            $part = $pathParts[$i]

            if ($part -eq "src") {
                $inSrc = $true
                continue
            }

            if (-not $inSrc) { continue }

            # Skip certain directories
            if ($part -in @("backend", "frontend", "tests", "tools", "scripts", "docs")) { continue }

            # Map bounded contexts
            switch ($part) {
                "Admin" { $expectedNamespace += ".Admin" }
                "Store" { $expectedNamespace += ".Store" }
                "Management" { $expectedNamespace += ".Management" }
                "Shared" { $expectedNamespace += ".Shared" }
                "Infrastructure" { $expectedNamespace += ".Infrastructure" }
                "Services" { $expectedNamespace += ".Services" }
                default {
                    # Add other directory names as namespace parts
                    if ($part -notmatch "\.cs$") {
                        $expectedNamespace += "." + $part
                    }
                }
            }
        }

        # Remove .cs extension from expected namespace
        $expectedNamespace = $expectedNamespace -replace "\.cs$", ""

        if ($actualNamespace -ne $expectedNamespace) {
            $namespaceIssues += @{
                File = $file.FullName
                Expected = $expectedNamespace
                Actual = $actualNamespace
            }
        }
    }
}

if ($namespaceIssues.Count -gt 0) {
    Write-Warning "Found $($namespaceIssues.Count) namespace inconsistencies:"
    $namespaceIssues | ForEach-Object {
        Write-Host "  $($_.File)" -ForegroundColor Red
        Write-Host "    Expected: $($_.Expected)" -ForegroundColor Yellow
        Write-Host "    Actual:   $($_.Actual)" -ForegroundColor Red
    }
} else {
    Write-Host "✅ All namespaces are consistent" -ForegroundColor Green
}

# Check for broken project references
Write-Host "`n3. Checking project references..." -ForegroundColor Yellow
$csprojFiles = Get-ChildItem -Path $TargetPath -Filter "*.csproj" -Recurse
$brokenRefs = @()

foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw

    # Find project references
    $projectRefs = [regex]::Matches($content, '<ProjectReference Include="([^"]*)"')

    foreach ($match in $projectRefs) {
        $refPath = $match.Groups[1].Value
        $fullRefPath = Join-Path (Split-Path $file.FullName) $refPath

        if (-not (Test-Path $fullRefPath)) {
            $brokenRefs += @{
                File = $file.FullName
                Reference = $refPath
                FullPath = $fullRefPath
            }
        }
    }
}

if ($brokenRefs.Count -gt 0) {
    Write-Warning "Found $($brokenRefs.Count) broken project references:"
    $brokenRefs | ForEach-Object {
        Write-Host "  $($_.File)" -ForegroundColor Red
        Write-Host "    References: $($_.Reference)" -ForegroundColor Yellow
    }
} else {
    Write-Host "✅ All project references are valid" -ForegroundColor Green
}

# Summary
Write-Host "`nValidation Summary:" -ForegroundColor Cyan
Write-Host "  Build: $(if ($LASTEXITCODE -eq 0) { '✅ PASS' } else { '❌ FAIL' })"
Write-Host "  Namespaces: $(if ($namespaceIssues.Count -eq 0) { '✅ PASS' } else { '❌ FAIL' })"
Write-Host "  References: $(if ($brokenRefs.Count -eq 0) { '✅ PASS' } else { '❌ FAIL' })"

if ($LASTEXITCODE -eq 0 -and $namespaceIssues.Count -eq 0 -and $brokenRefs.Count -eq 0) {
    Write-Host "`n🎉 All validations passed!" -ForegroundColor Green
} else {
    Write-Host "`n⚠️  Some validations failed. Please review the issues above." -ForegroundColor Red
}