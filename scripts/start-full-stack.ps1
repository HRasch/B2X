# B2Connect Full Stack Startup Script (Windows)
# Starts all services (Backend + Frontend) using Docker Compose + Aspire

Write-Host "?? B2Connect Full Stack Startup" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

# Check if Docker is running
try {
    docker info > $null 2>&1
    Write-Host "? Docker is running" -ForegroundColor Green
} catch {
    Write-Host "? Docker is not running. Please start Docker and try again." -ForegroundColor Red
    exit 1
}

# Start Docker Compose
Write-Host ""
Write-Host "Starting all services with Docker Compose..." -ForegroundColor Cyan
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Docker Compose services started" -ForegroundColor Green
} else {
    Write-Host "? Failed to start Docker Compose services" -ForegroundColor Red
    exit 1
}

# Wait for infrastructure
Write-Host ""
Write-Host "Waiting for infrastructure services to be ready..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "? All services are starting!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Available Endpoints:" -ForegroundColor Yellow
Write-Host "   - Store Frontend:     http://localhost:5173"
Write-Host "   - Admin Frontend:     http://localhost:5174"
Write-Host "   - Store Gateway:      http://localhost:8000"
Write-Host "   - Admin Gateway:      http://localhost:8080"
Write-Host ""
Write-Host "   - Auth Service:       http://localhost:7002"
Write-Host "   - Tenant Service:     http://localhost:7003"
Write-Host "   - Localization:       http://localhost:7004"
Write-Host "   - Catalog Service:    http://localhost:7005"
Write-Host "   - Theming Service:    http://localhost:7008"
Write-Host ""
Write-Host "   - PostgreSQL:         localhost:5432 (postgres/postgres)"
Write-Host "   - Redis:              localhost:6379"
Write-Host "   - RabbitMQ Admin:     http://localhost:15672 (guest/guest)"
Write-Host "   - Elasticsearch:      http://localhost:9200 (elastic/elastic)"
Write-Host ""
Write-Host "??  To stop all services:" -ForegroundColor Cyan
Write-Host "   docker-compose down"
Write-Host ""
Write-Host "??  To view logs:" -ForegroundColor Cyan
Write-Host "   docker-compose logs -f [service-name]"
Write-Host ""
Write-Host "??  For Aspire Dashboard:" -ForegroundColor Cyan
Write-Host "   cd AppHost && dotnet run"
Write-Host "   Then open http://localhost:15500"
