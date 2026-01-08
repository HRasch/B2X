#!/bin/bash

# B2X Full Stack Startup Script
# Starts all services (Backend + Frontend) using Docker Compose + Aspire

echo "?? B2X Full Stack Startup"
echo "================================"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "? Docker is not running. Please start Docker and try again."
    exit 1
fi

echo "? Docker is running"

# Option 1: Start everything with Docker Compose
echo ""
echo "Starting all services with Docker Compose..."
docker-compose up -d

if [ $? -eq 0 ]; then
    echo "? Docker Compose services started"
else
    echo "? Failed to start Docker Compose services"
    exit 1
fi

# Wait for infrastructure to be healthy
echo ""
echo "Waiting for infrastructure services to be ready..."
sleep 10

# Option 2: Start Aspire AppHost in a separate terminal (Windows)
# For Windows, uncomment the following:
# start cmd /k "cd AppHost && dotnet run"

# For Mac/Linux, use:
# (cd AppHost && dotnet run) &

echo ""
echo "? All services are starting!"
echo ""
echo "?? Available Endpoints:"
echo "   - Store Frontend:     http://localhost:5173"
echo "   - Admin Frontend:     http://localhost:5174"
echo "   - Store Gateway:      http://localhost:8000"
echo "   - Admin Gateway:      http://localhost:8080"
echo ""
echo "   - Auth Service:       http://localhost:7002"
echo "   - Tenant Service:     http://localhost:7003"
echo "   - Localization:       http://localhost:7004"
echo "   - Catalog Service:    http://localhost:7005"
echo "   - Theming Service:    http://localhost:7008"
echo ""
echo "   - PostgreSQL:         localhost:5432 (postgres/postgres)"
echo "   - Redis:              localhost:6379"
echo "   - RabbitMQ Admin:     http://localhost:15672 (guest/guest)"
echo "   - Elasticsearch:      http://localhost:9200 (elastic/elastic)"
echo ""
echo "??  To stop all services:"
echo "   docker-compose down"
echo ""
echo "??  To view logs:"
echo "   docker-compose logs -f [service-name]"
echo ""
echo "??  For Aspire Dashboard:"
echo "   cd AppHost && dotnet run"
echo "   Then open http://localhost:15500"
