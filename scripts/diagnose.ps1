# B2Connect Aspire Diagnostics Script (Windows PowerShell)
# Helps identify why services don't show in the Aspire Dashboard

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?        B2Connect Aspire Diagnostics                         ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Check if running from correct directory
Write-Host "?? Checking current directory..." -ForegroundColor Yellow
if (Test-Path "AppHost/Program.cs") {
    Write-Host "? Found AppHost/Program.cs" -ForegroundColor Green
} else {
    Write-Host "? Not in B2Connect root directory! Run from: C:\Users\Holge\repos\B2Connect" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "?? Checking required files..." -ForegroundColor Yellow

# Check if all .csproj files exist
$services = @(
    "backend\Domain\Identity\B2Connect.Identity.API.csproj",
    "backend\Domain\Tenancy\B2Connect.Tenancy.API.csproj",
    "backend\Domain\Localization\B2Connect.Localization.API.csproj",
    "backend\Domain\Catalog\B2Connect.Catalog.API.csproj",
    "backend\Domain\Theming\B2Connect.Theming.API.csproj",
    "backend\Gateway\Store\API\B2Connect.Store.csproj",
    "backend\Gateway\Admin\B2Connect.Admin.csproj"
)

foreach ($service in $services) {
    if (Test-Path $service) {
        Write-Host "? $service" -ForegroundColor Green
    } else {
        Write-Host "? MISSING: $service" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Checking IsAspireProjectResource in .csproj files..." -ForegroundColor Yellow

foreach ($service in $services) {
    if (Test-Path $service) {
        $content = Get-Content $service
        if ($content -match "IsAspireProjectResource") {
            Write-Host "? $service has IsAspireProjectResource" -ForegroundColor Green
        } else {
            Write-Host "??  $service MISSING IsAspireProjectResource" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "?? Checking Docker containers..." -ForegroundColor Yellow
$containers = docker ps --format "{{.Names}}" 2>/dev/null | Select-String "b2connect|postgres|redis|rabbitmq|elasticsearch"
if ($containers) {
    Write-Host "? Docker containers running:" -ForegroundColor Green
    docker ps --format "table {{.Names}}`t{{.Status}}" | Select-String "b2connect|postgres|redis|rabbitmq|elasticsearch"
} else {
    Write-Host "??  No B2Connect containers running" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "?? Testing connectivity..." -ForegroundColor Yellow

$ports = @(7002, 7003, 7004, 7005, 7008, 8000, 8080, 5432, 6379, 5672, 9200, 15500)
foreach ($port in $ports) {
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $async = $tcp.BeginConnect("localhost", $port, $null, $null)
        $wait = $async.AsyncWaitHandle.WaitOne(1000, $false)
        if ($wait -and $tcp.Connected) {
            Write-Host "? Port $port is open" -ForegroundColor Green
        } else {
            Write-Host "? Port $port is closed" -ForegroundColor Red
        }
        $tcp.Close()
    } catch {
        Write-Host "? Port $port is closed" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Checking AppHost compilation..." -ForegroundColor Yellow

$buildOutput = dotnet build AppHost -q 2>&1
if ($buildOutput | Select-String "error") {
    Write-Host "? AppHost has compilation errors:" -ForegroundColor Red
    Write-Host $buildOutput
} else {
    Write-Host "? AppHost compiles successfully" -ForegroundColor Green
}

Write-Host ""
Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Troubleshooting tips:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. If services don't appear in dashboard:" -ForegroundColor White
Write-Host "   - Make sure ALL .csproj files have <IsAspireProjectResource>true</IsAspireProjectResource>"
Write-Host "   - Check AppHost console for startup errors"
Write-Host ""
Write-Host "2. If ports are closed:" -ForegroundColor White
Write-Host "   - Services may not have started. Check logs:"
Write-Host "     docker-compose logs [service-name]"
Write-Host ""
Write-Host "3. Run AppHost with verbose output:" -ForegroundColor White
Write-Host "   cd AppHost; dotnet run --verbosity detailed"
Write-Host ""
Write-Host "4. Check Aspire Dashboard logs:" -ForegroundColor White
Write-Host "   Browser Console (F12) -> Network tab"
Write-Host ""
Write-Host "????????????????????????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""
Write-Host "5. Common issue: Service startup order" -ForegroundColor Cyan
Write-Host "   Infrastructure services (postgres, redis) must be healthy before services start."
Write-Host "   Check: docker-compose logs postgres" -ForegroundColor White
Write-Host ""
