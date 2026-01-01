#!/bin/bash

# Test script for Shared Support Service Feedback API
SERVICE_URL="http://localhost:8090"

echo "Testing Shared Support Service Feedback API..."

# Test health endpoint
echo "1. Testing health endpoint..."
curl -s "$SERVICE_URL/health" | jq . || echo "Health check failed"

# Test feedback submission (mock data)
echo "2. Testing feedback submission..."
curl -X POST "$SERVICE_URL/api/feedback" \
  -H "Content-Type: application/json" \
  -d '{
    "question": "How do I reset my password?",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "metadata": {
      "app": "Store",
      "version": "1.0.0"
    }
  }' | jq . || echo "Feedback submission failed"

echo "Test completed."