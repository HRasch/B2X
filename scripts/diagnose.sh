#!/bin/bash

# B2Connect Aspire Diagnostics Script
# Helps identify why services don't show in the Aspire Dashboard

echo "??????????????????????????????????????????????????????????????"
echo "?        B2Connect Aspire Diagnostics                         ?"
echo "??????????????????????????????????????????????????????????????"
echo ""

# Check if running from correct directory
echo "?? Checking current directory..."
if [ -f "AppHost/Program.cs" ]; then
    echo "? Found AppHost/Program.cs"
else
    echo "? Not in B2Connect root directory! Run from: C:\Users\Holge\repos\B2Connect"
    exit 1
fi

echo ""
echo "?? Checking required files..."

# Check if all .csproj files exist
services=(
    "backend/Domain/Identity/B2Connect.Identity.API.csproj"
    "backend/Domain/Tenancy/B2Connect.Tenancy.API.csproj"
    "backend/Domain/Localization/B2Connect.Localization.API.csproj"
    "backend/Domain/Catalog/B2Connect.Catalog.API.csproj"
    "backend/Domain/Theming/B2Connect.Theming.API.csproj"
    "backend/Gateway/Store/API/B2Connect.Store.csproj"
    "backend/Gateway/Admin/B2Connect.Admin.csproj"
)

for service in "${services[@]}"; do
    if [ -f "$service" ]; then
        echo "? $service"
    else
        echo "? MISSING: $service"
    fi
done

echo ""
echo "?? Checking IsAspireProjectResource in .csproj files..."

for service in "${services[@]}"; do
    if [ -f "$service" ]; then
        if grep -q "IsAspireProjectResource" "$service"; then
            echo "? $service has IsAspireProjectResource"
        else
            echo "??  $service MISSING IsAspireProjectResource"
        fi
    fi
done

echo ""
echo "?? Checking Docker containers..."
docker ps --format "table {{.Names}}\t{{.Status}}" | grep -E "b2connect|postgres|redis|rabbitmq|elasticsearch" || echo "No B2Connect containers running"

echo ""
echo "?? Testing connectivity..."

# Test if ports are open
for port in 7002 7003 7004 7005 7008 8000 8080 5432 6379 5672 9200 15500; do
    timeout 1 bash -c "echo >/dev/tcp/localhost/$port" 2>/dev/null && \
        echo "? Port $port is open" || \
        echo "? Port $port is closed"
done

echo ""
echo "?? Checking AppHost compilation..."

if dotnet build AppHost -q 2>&1 | grep -q "error"; then
    echo "? AppHost has compilation errors:"
    dotnet build AppHost
else
    echo "? AppHost compiles successfully"
fi

echo ""
echo "????????????????????????????????????????????????????????????"
echo ""
echo "?? Troubleshooting tips:"
echo ""
echo "1. If services don't appear in dashboard:"
echo "   - Make sure ALL .csproj files have <IsAspireProjectResource>true</IsAspireProjectResource>"
echo "   - Check AppHost console for startup errors"
echo ""
echo "2. If ports are closed:"
echo "   - Services may not have started. Check logs:"
echo "     docker-compose logs [service-name]"
echo ""
echo "3. Run AppHost with verbose output:"
echo "   cd AppHost && dotnet run --verbosity detailed"
echo ""
echo "4. Check Aspire Dashboard logs:"
echo "   Browser Console (F12) -> Network tab"
echo ""
echo "????????????????????????????????????????????????????????????"
echo ""
