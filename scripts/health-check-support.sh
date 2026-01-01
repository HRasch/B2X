#!/bin/bash

# Health check for Shared Support Service
SERVICE_URL="http://localhost:8090/health"

echo "Checking Shared Support Service health..."

if curl -f -s "$SERVICE_URL" > /dev/null; then
    echo "✅ Shared Support Service is healthy"
    exit 0
else
    echo "❌ Shared Support Service is not responding"
    exit 1
fi