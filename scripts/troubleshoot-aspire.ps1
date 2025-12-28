# Interactive Aspire Dashboard Troubleshooting (Windows PowerShell)

param(
    [switch]$QuickFix,
    [switch]$FullDiagnosis,
    [switch]$StartAppHost
)

# Colors
$success = "Green"
$error = "Red"
$warning = "Yellow"
$info = "Cyan"

function Write-Title {
    param([string]$Title)
    Write-Host "`n??????????????????????????????????????????????" -ForegroundColor $info
    Write-Host "? $Title" -ForegroundColor $info
    Write-Host "??????????????????????????????????????????????" -ForegroundColor $info
}

function Write-Step {
    param([string]$Step, [int]$Number)
    Write-Host "`n[$Number] $Step" -ForegroundColor $info
}

function Test-FileExists {
    param([string]$Path)
    if (Test-Path $Path) {
        Write-Host "  ? $Path" -ForegroundColor $success
        return $true
    } else {
        Write-Host "  ? $Path" -ForegroundColor $error
        return $false
    }
}

Write-Title "B2Connect Aspire Dashboard Troubleshooting"

# Interactive Menu
if (-not $QuickFix -and -not $FullDiagnosis -and -not $StartAppHost) {
    Write-Host "`nWhat do you want to do?`n"
    Write-Host "1 - Quick Fix (3 steps)" -ForegroundColor $info
    Write-Host "2 - Full Diagnosis" -ForegroundColor $info
    Write-Host "3 - Start AppHost directly" -ForegroundColor $info
    Write-Host "4 - Exit" -ForegroundColor $info
    
    $choice = Read-Host "`nSelect (1-4)"
    
    switch ($choice) {
        "1" { $QuickFix = $true }
        "2" { $FullDiagnosis = $true }
        "3" { $StartAppHost = $true }
        "4" { exit }
        default { Write-Host "Invalid choice"; exit }
    }
}

# QUICK FIX
if ($QuickFix) {
    Write-Title "QUICK FIX - 3 STEPS"
    
    Write-Step "Kill all existing processes" 1
    Write-Host "Killing dotnet processes..." -ForegroundColor $warning
    Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
    Write-Host "  ? Done" -ForegroundColor $success
    
    Write-Step "Stop Docker containers" 2
    Write-Host "Stopping docker-compose..." -ForegroundColor $warning
    docker-compose down --quiet 2>$null
    Write-Host "  ? Done" -ForegroundColor $success
    
    Write-Step "Clean rebuild AppHost" 3
    Write-Host "Building AppHost..." -ForegroundColor $warning
    Push-Location AppHost
    $buildResult = dotnet build -q
    Pop-Location
    
    if ($buildResult) {
        Write-Host "  ? Build successful" -ForegroundColor $success
        Write-Host "`nNow run:" -ForegroundColor $info
        Write-Host "  cd AppHost" -ForegroundColor $warning
        Write-Host "  dotnet run" -ForegroundColor $warning
        Write-Host "`nThen open: http://localhost:15500" -ForegroundColor $warning
    } else {
        Write-Host "  ? Build failed" -ForegroundColor $error
    }
}

# FULL DIAGNOSIS
if ($FullDiagnosis) {
    Write-Title "FULL DIAGNOSIS"
    
    $allOk = $true
    
    Write-Step "Checking project structure" 1
    $projects = @(
        "backend\Domain\Identity\B2Connect.Identity.API.csproj",
        "backend\Domain\Tenancy\B2Connect.Tenancy.API.csproj",
        "backend\Domain\Localization\B2Connect.Localization.API.csproj",
        "backend\Domain\Catalog\B2Connect.Catalog.API.csproj",
        "backend\Domain\Theming\B2Connect.Theming.API.csproj",
        "backend\Gateway\Store\API\B2Connect.Store.csproj",
        "backend\Gateway\Admin\B2Connect.Admin.csproj"
    )
    
    foreach ($proj in $projects) {
        if (-not (Test-FileExists $proj)) {
            $allOk = $false
        }
    }
    
    Write-Step "Checking IsAspireProjectResource" 2
    foreach ($proj in $projects) {
        if (Test-Path $proj) {
            $content = Get-Content $proj
            if ($content -match "IsAspireProjectResource") {
                Write-Host "  ? $($proj.Split('\')[-1])" -ForegroundColor $success
            } else {
                Write-Host "  ? $($proj.Split('\')[-1]) - MISSING IsAspireProjectResource" -ForegroundColor $error
                $allOk = $false
            }
        }
    }
    
    Write-Step "Checking AppHost build" 3
    $buildOutput = dotnet build AppHost -q 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? AppHost builds successfully" -ForegroundColor $success
    } else {
        Write-Host "  ? AppHost has build errors:" -ForegroundColor $error
        Write-Host $buildOutput
        $allOk = $false
    }
    
    Write-Step "Checking Docker containers" 4
    $containers = docker-compose ps --quiet 2>$null
    if ($containers) {
        Write-Host "  ? Docker containers exist" -ForegroundColor $success
        docker-compose ps
    } else {
        Write-Host "  ??  No containers running. Start with: docker-compose up -d" -ForegroundColor $warning
    }
    
    Write-Step "Checking open ports" 5
    $ports = @(15500, 7002, 7003, 7004, 7005, 8000, 8080, 5432, 6379, 5672)
    foreach ($port in $ports) {
        try {
            $tcp = New-Object System.Net.Sockets.TcpClient
            $async = $tcp.BeginConnect("localhost", $port, $null, $null)
            $wait = $async.AsyncWaitHandle.WaitOne(500, $false)
            if ($wait -and $tcp.Connected) {
                Write-Host "  ? Port $port open" -ForegroundColor $success
            } else {
                Write-Host "  ??  Port $port closed" -ForegroundColor $warning
            }
            $tcp.Close()
        } catch {
            Write-Host "  ??  Port $port closed" -ForegroundColor $warning
        }
    }
    
    Write-Host ""
    if ($allOk) {
        Write-Host "? All checks passed! Start AppHost with: cd AppHost && dotnet run" -ForegroundColor $success
    } else {
        Write-Host "??  Some checks failed. Review above and see: docs/ASPIRE_DASHBOARD_TROUBLESHOOTING.md" -ForegroundColor $warning
    }
}

# START APPHOST
if ($StartAppHost) {
    Write-Title "STARTING APPHOST"
    
    Write-Host "Opening AppHost directory..." -ForegroundColor $info
    Push-Location AppHost
    
    Write-Host "`nStarting AppHost with diagnostic output..." -ForegroundColor $warning
    Write-Host "Waiting for message: 'Listening on http://localhost:15500'" -ForegroundColor $warning
    Write-Host "Then open browser to: http://localhost:15500`n" -ForegroundColor $warning
    
    dotnet run --verbosity normal
    
    Pop-Location
}

Write-Host "`n" -ForegroundColor $info
