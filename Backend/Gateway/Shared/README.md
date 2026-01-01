# B2Connect Shared Support Service

This service provides the backend for the "Questions & Hints" feedback system, enabling users to submit questions and automatically create anonymized GitHub issues.

## Features

- **Feedback Collection**: Collects user questions and context data
- **Data Anonymization**: Automatically removes PII and sensitive information
- **GitHub Integration**: Creates structured GitHub issues from feedback
- **GDPR Compliance**: Ensures data privacy and user consent
- **Rate Limiting**: Prevents abuse and spam
- **Audit Logging**: Tracks all operations for compliance

## API Endpoints

### POST /api/feedback
Submits user feedback and creates a GitHub issue.

**Request Body:**
```json
{
  "question": "How do I reset my password?",
  "context": {
    "url": "https://store.b2connect.com/login",
    "userAgent": "Mozilla/5.0...",
    "timestamp": "2024-01-15T10:30:00Z",
    "sessionId": "session-12345"
  },
  "metadata": {
    "app": "Store",
    "version": "1.0.0"
  }
}
```

**Response:**
```json
{
  "id": "feedback-123",
  "issueUrl": "https://github.com/org/repo/issues/456",
  "status": "created"
}
```

### GET /health
Health check endpoint.

## Configuration

### Environment Variables
- `GitHub__Token`: GitHub Personal Access Token with repo permissions
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)

### Appsettings
See `appsettings.json` for configuration options including:
- GitHub repository settings
- Validation rules
- Rate limiting configuration
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