#!/bin/bash

# ML Model Training Script for Malicious Request Analyzer
SERVICE_URL="http://localhost:8090"

echo "Training ML model for malicious request detection..."

# Training data for the ML model
TRAINING_DATA='[
  {
    "content": "How do I reset my password? I cannot find the reset link on the login page.",
    "isMalicious": false,
    "categories": ["legitimate", "question"]
  },
  {
    "content": "Please help me with the registration process",
    "isMalicious": false,
    "categories": ["legitimate", "question"]
  },
  {
    "content": "The website is not loading properly on my browser",
    "isMalicious": false,
    "categories": ["legitimate", "bug"]
  },
  {
    "content": "BUY CHEAP VIAGRA NOW!!! SPECIAL OFFER ONLY TODAY!!!",
    "isMalicious": true,
    "categories": ["spam", "advertisement"]
  },
  {
    "content": "How to hack this website and get admin access?",
    "isMalicious": true,
    "categories": ["malicious", "security"]
  },
  {
    "content": "You idiots made this terrible. Fix it or I will report you!",
    "isMalicious": true,
    "categories": ["abusive", "threat"]
  },
  {
    "content": "SELECT * FROM users WHERE id = 1; DROP TABLE users;",
    "isMalicious": true,
    "categories": ["malicious", "sql_injection"]
  },
  {
    "content": "<script>alert(\"XSS Attack\")</script>",
    "isMalicious": true,
    "categories": ["malicious", "xss"]
  },
  {
    "content": "rm -rf / && shutdown now",
    "isMalicious": true,
    "categories": ["malicious", "command_injection"]
  },
  {
    "content": "This is a legitimate question about the product features",
    "isMalicious": false,
    "categories": ["legitimate", "question"]
  }
]'

# Note: In a real implementation, you would call a training endpoint
# For now, this script just validates that the service is running
echo "Checking service health..."
curl -s "$SERVICE_URL/health" | jq . || echo "Service not running"

echo "Training data prepared. In production, this would train the ML model."
echo "Training data sample:"
echo "$TRAINING_DATA" | jq '.[0:3]'

echo "ML model training simulation completed."