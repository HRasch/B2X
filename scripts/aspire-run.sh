#!/bin/bash
# B2Connect Aspire Application Host Startup Script
# Starts the Aspire orchestrator with all registered services

set -e

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$PROJECT_DIR/backend/services/AppHost"

echo "🚀 Starting B2Connect Aspire Application Host..."
echo ""
echo "Services will be available at:"
echo "  - Auth Service: http://localhost:9002"
echo "  - Tenant Service: http://localhost:9003"
echo "  - Localization Service: http://localhost:9004"
echo ""
echo "Frontend services (port 5173, 5174) run via VS Code Tasks"
echo ""

dotnet run --project B2Connect.AppHost.csproj
