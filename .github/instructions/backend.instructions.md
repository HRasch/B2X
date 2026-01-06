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

## Localization Support

**Reference**: See [GL-042] for token-optimized i18n patterns.

- **All backend messages must be translated** - error messages, validation messages, notifications
- Return translation keys (not hardcoded strings) for user-facing messages
- Use `IStringLocalizer<T>` for server-side localization
- Maintain localization API endpoints for frontend i18n support
- Implement caching for localization data to improve performance
- Support languages: en, de, fr, es, it, pt, nl, pl
- English (`en`) is source of truth for all translations
- Validate localization keys exist before deployment

