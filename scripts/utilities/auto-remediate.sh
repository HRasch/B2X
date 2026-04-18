#!/bin/bash
# B2X Auto-Remediation Script
# Automatically resolves common issues based on alerts

set -e

# Configuration
DOCKER_COMPOSE_FILE="/Users/holger/Documents/Projekte/B2X/docker-compose.yml"
PROJECT_DIR="/Users/holger/Documents/Projekte/B2X"

# Function to restart unhealthy services
restart_service() {
    local service_name=$1
    echo "Restarting service: $service_name"

    docker-compose -f "$DOCKER_COMPOSE_FILE" restart "$service_name"

    # Wait for service to be healthy
    local max_attempts=30
    local attempt=1

    while [ $attempt -le $max_attempts ]; do
        if docker-compose -f "$DOCKER_COMPOSE_FILE" ps "$service_name" | grep -q "healthy\|running"; then
            echo "Service $service_name is now healthy"
            return 0
        fi

        echo "Waiting for $service_name to become healthy... (attempt $attempt/$max_attempts)"
        sleep 10
        ((attempt++))
    done

    echo "Failed to restart service $service_name"
    return 1
}

# Function to clear Redis cache if memory is high
clear_redis_cache() {
    echo "Clearing Redis cache due to high memory usage"

    docker-compose -f "$DOCKER_COMPOSE_FILE" exec redis redis-cli FLUSHALL

    echo "Redis cache cleared"
}

# Function to restart database connections
restart_database_connections() {
    echo "Restarting services with database connection issues"

    # Restart all services that depend on postgres
    docker-compose -f "$DOCKER_COMPOSE_FILE" restart identity-service catalog-service cms-service localization-service search-service

    echo "Database-dependent services restarted"
}

# Function to check and restart Elasticsearch if unhealthy
restart_elasticsearch() {
    echo "Checking Elasticsearch health..."

    local health_status
    health_status=$(curl -s http://localhost:9200/_cluster/health | jq -r '.status')

    if [ "$health_status" != "green" ] && [ "$health_status" != "yellow" ]; then
        echo "Elasticsearch is unhealthy (status: $health_status), restarting..."
        restart_service elasticsearch
    else
        echo "Elasticsearch is healthy (status: $health_status)"
    fi
}

# Main remediation logic based on alert type
case "$1" in
    "service-down")
        restart_service "$2"
        ;;
    "redis-memory-high")
        clear_redis_cache
        ;;
    "database-connection-issues")
        restart_database_connections
        ;;
    "elasticsearch-unhealthy")
        restart_elasticsearch
        ;;
    "all")
        echo "Running all remediation checks..."
        restart_elasticsearch
        clear_redis_cache
        restart_database_connections
        ;;
    *)
        echo "Usage: $0 {service-down <service>|redis-memory-high|database-connection-issues|elasticsearch-unhealthy|all}"
        exit 1
        ;;
esac

echo "Auto-remediation completed"