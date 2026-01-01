---
applyTo: "src/api/**,src/services/**,src/models/**,src/repositories/**,**/backend/**"
---

# Backend Development Instructions

## Code Standards
- Use async/await for all asynchronous operations
- Implement proper error handling with typed exceptions
- Apply input validation on all public endpoints
- Use dependency injection for testability

## API Design
- Follow RESTful conventions
- Use proper HTTP status codes
- Return consistent error response format
- Document endpoints with OpenAPI/Swagger

## Database
- Use parameterized queries (prevent SQL injection)
- Implement proper connection pooling
- Add database migrations for schema changes
- Index frequently queried columns

## Security
- Never expose internal errors to clients
- Sanitize all user inputs
- Implement rate limiting on public endpoints
- Use environment variables for secrets

## Testing
- Write unit tests for business logic
- Write integration tests for API endpoints
- Mock external dependencies
- Aim for >80% code coverage

## Code Review & Quality
- Require mandatory code reviews for all backend changes with at least two approvals from senior developers
- **MANDATORY**: For database-related changes (Data/, repositories/, models/), require review from @DatabaseSpecialist for query optimization, indexing, and EF Core SQL generation
- Implement automated dependency update policies with quarterly reviews and security vulnerability scans
- Establish API versioning and deprecation notice policies for smooth transitions and backward compatibility
- Mandate comprehensive unit and integration testing for all new features with 90%+ coverage targets

## Lessons Learned Integration
- **MANDATORY**: Query `.ai/lessons/` for backend-related incidents before implementing new features
- **MANDATORY**: Include regression tests for past backend failures in all PRs
- **MANDATORY**: Document lessons learned for all backend incidents (Severity 1-3)
- **MANDATORY**: Apply prevention patterns from historical backend issues
- **MANDATORY**: Review API design against past API failure patterns
