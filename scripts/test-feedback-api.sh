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

# Test feedback validation - malicious content (SQL injection)
echo "7. Testing feedback validation (malicious - SQL injection)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "How can I SELECT * FROM users table?",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "SQL injection validation test failed"

# Test feedback validation - malicious content (XSS)
echo "8. Testing feedback validation (malicious - XSS)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "Check this <script>alert(\"test\")</script> code",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "XSS validation test failed"

# Test feedback validation - abusive content
echo "9. Testing feedback validation (malicious - abusive)..."
curl -X POST "$SERVICE_URL/api/support/feedback/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "category": "Question",
    "description": "This fucking website is shit and you are all idiots!",
    "context": {
      "url": "https://store.b2connect.com/login",
      "userAgent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36",
      "timestamp": "2024-01-15T10:30:00Z",
      "sessionId": "session-12345"
    },
    "attachments": []
  }' | jq . || echo "Abusive content validation test failed"