# ASPIRE FRONTENDS QUICK START (Windows PowerShell)
# Builds Docker images and starts AppHost with Frontends

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?        B2Connect Aspire Frontend Setup                      ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build Docker Images
Write-Host "?? Step 1: Building Docker Images..." -ForegroundColor Yellow
Write-Host "   Building: b2connect-frontend-store:latest" -ForegroundColor White
docker build -t b2connect-frontend-store:latest ./Frontend/Store
Write-Host "   ? Done" -ForegroundColor Green
Write-Host ""

Write-Host "   Building: b2connect-frontend-admin:latest" -ForegroundColor White
docker build -t b2connect-frontend-admin:latest ./Frontend/Admin
Write-Host "   ? Done" -ForegroundColor Green
Write-Host ""

# Step 2: Build AppHost
Write-Host "?? Step 2: Building AppHost..." -ForegroundColor Yellow
dotnet build AppHost -q
Write-Host "   ? AppHost compiled successfully" -ForegroundColor Green
Write-Host ""

# Step 3: Start AppHost
Write-Host "?? Step 3: Starting AppHost with Frontends..." -ForegroundColor Yellow
Write-Host ""
Write-Host "   ? Waiting for services to start..." -ForegroundColor Cyan
Write-Host "   ?? Dashboard will be available at: http://localhost:15500" -ForegroundColor White
Write-Host "   ?? Store Frontend: http://localhost:5173" -ForegroundColor White
Write-Host "   ????? Admin Frontend: http://localhost:5174" -ForegroundColor White
Write-Host ""
Write-Host "   Press Ctrl+C to stop all services" -ForegroundColor Magenta
Write-Host ""

Push-Location AppHost
dotnet run
Pop-Location
