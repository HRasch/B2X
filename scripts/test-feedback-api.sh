#!/bin/bash

# Test script for Shared Support Service Feedback API
SERVICE_URL="http://localhost:8090"

echo "Testing Shared Support Service Feedback API..."

# Test health endpoint
echo "1. Testing health endpoint..."
curl -s "$SERVICE_URL/health" | jq . || echo "Health check failed"

# Test feedback validation - valid feedback
echo "2. Testing feedback validation (valid)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "How do I reset my password? I cannot find the reset link on the login page.",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Validation test failed"

# Test feedback validation - invalid feedback (blocked keywords)
echo "3. Testing feedback validation (blocked keywords)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "How do I hack the password system?",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Blocked keywords validation test failed"

# Test feedback validation - invalid feedback (too short)
echo "4. Testing feedback validation (too short)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "Hi",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Short description validation test failed"

# Test feedback submission (valid)
echo "5. Testing feedback submission (valid)..."
curl -X POST "$SERVICE_URL/api/support/feedback" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "How do I reset my password? I cannot find the reset link on the login page.",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Feedback submission failed"

# Test feedback submission (invalid - should be rejected)
echo "6. Testing feedback submission (invalid - should be rejected)..."
curl -X POST "$SERVICE_URL/api/support/feedback" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "How do I hack the system?",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Invalid feedback submission test failed"

echo "Test completed."