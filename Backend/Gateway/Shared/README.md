# B2Connect Shared Support Service

This service provides the backend for the "Questions & Hints" feedback system, enabling users to submit questions and automatically create anonymized GitHub issues.

## Features

- **Feedback Collection**: Collects user questions and context data
- **Data Anonymization**: Automatically removes PII and sensitive information
- **Pre-Validation**: Validates feedback content before processing
- **Content Filtering**: Blocks spam, inappropriate content, and security issues
- **GitHub Integration**: Creates structured GitHub issues from validated feedback
- **GDPR Compliance**: Ensures data privacy and user consent
- **Rate Limiting**: Prevents abuse and spam
- **Audit Logging**: Tracks all operations for compliance

## API Endpoints

### POST /api/support/feedback/validate
Validates feedback content before submission to check if it meets acceptance criteria.

**Request Body:**
```json
{
  "category": "Question",
  "description": "How do I reset my password?",
  "context": {
    "url": "https://store.b2connect.com/login",
    "userAgent": "Mozilla/5.0...",
    "timestamp": "2024-01-15T10:30:00Z",
    "sessionId": "session-12345"
  },
  "attachments": []
}
```

**Response (Valid):**
```json
{
  "isValid": true,
  "status": "Valid",
  "message": "Feedback is valid and can be processed.",
  "reasons": [],
  "severity": "Low"
}
```

**Response (Rejected):**
```json
{
  "isValid": false,
  "status": "Rejected",
  "message": "Feedback cannot be processed due to validation errors.",
  "reasons": [
    "Content contains blocked keywords: hack",
    "Description must be at least 3 words long"
  ],
  "severity": "Critical"
}
```

### POST /api/support/feedback
Submits user feedback and creates a GitHub issue (only if validation passes).

**Request Body:** Same as validation endpoint

**Response:**
```json
{
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "issueUrl": "https://github.com/org/repo/issues/456",
  "status": "submitted",
  "message": "Vielen Dank für Ihr Feedback!"
}
```

**Error Response (Validation Failed):**
```json
{
  "error": "Feedback validation failed",
  "message": "Feedback cannot be processed due to validation errors.",
  "reasons": ["Content contains blocked keywords: hack"]
}
```

### GET /health
Health check endpoint.

## Validation Rules

The service includes comprehensive validation to ensure only appropriate feedback is processed:

### Content Validation
- **Blocked Keywords**: Rejects content containing security-related, offensive, or inappropriate terms
- **Minimum Length**: Requires at least 3 words in the description
- **Spam Detection**: Identifies excessive capitalization, repeated characters, and spam patterns
- **URL Limits**: Allows maximum 2 URLs per feedback

### Attachment Validation
- **File Types**: Only allows image (JPEG, PNG, GIF), text, and JSON files
- **Size Limits**: Maximum 5MB per attachment, up to 3 attachments total
- **Content Type Verification**: Validates file content matches declared type

### Context Validation
- **Required Data**: Ensures context information is provided
- **Data Integrity**: Validates context data structure and completeness

### Rejection Reasons
Common reasons for feedback rejection:
- Content contains blocked keywords (hack, password, offensive terms)
- Description too short or contains spam patterns
- Invalid or oversized attachments
- Missing required context data
- Rate limiting violations

## Configuration

### Environment Variables
- `GitHub__Token`: GitHub Personal Access Token with repo permissions
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)

### Appsettings
See `appsettings.json` for configuration options including:
- GitHub repository settings
- Validation rules and blocked keywords
- Feedback limits and rate limiting
- Logging levels

## Development

### Prerequisites
- .NET 8.0 SDK
- GitHub Personal Access Token

### Running Locally
```bash
# Build and run
dotnet run --project src/B2Connect.Shared.csproj

# Or use the AppHost
dotnet run --project ../../../../../AppHost/B2Connect.AppHost.csproj
```

### Testing
```bash
# Run unit tests
dotnet test

# Test API endpoints
./scripts/test-feedback-api.sh

# Health check
./scripts/health-check-support.sh
```

## Docker

```bash
# Build image
docker build -t b2connect/shared-support .

# Run container
docker run -p 8090:80 -e GitHub__Token=your-token b2connect/shared-support
```

## Architecture

The service follows Clean Architecture principles with:
- **Domain Layer**: Core business logic and entities
- **Application Layer**: Use cases and commands/queries
- **Infrastructure Layer**: External dependencies (GitHub API, repositories)
- **API Layer**: REST endpoints and DTOs

## Security

- JWT authentication for admin endpoints
- CORS configuration for frontend access
- Input validation and sanitization
- Rate limiting to prevent abuse
- Comprehensive audit logging

## Monitoring

- Health checks at `/health`
- Structured logging with Serilog
- OpenTelemetry integration
- Metrics collection for performance monitoring