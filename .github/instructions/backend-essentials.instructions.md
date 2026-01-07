---
applyTo: "src/api/**,src/services/**,src/models/**,src/repositories/**,**/backend/**"
---

# Backend Development Instructions (Essentials)

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

## Localization
- All backend messages must be translated
- Return translation keys (not hardcoded strings)
- Use `IStringLocalizer<T>`
- Support: en, de, fr, es, it, pt, nl, pl

## MCP Tools

For detailed backend-specific MCP tools documentation:

```
kb-mcp/search_knowledge_base
  query: "Roslyn MCP" OR "Backend security" OR "Database MCP"
```

See [KB-052] Roslyn MCP, [KB-055] Security MCP, [KB-057] Database MCP

---

**Full documentation**: Use `kb-mcp/get_article` or search Knowledge Base  
**Size**: 1.2 KB (optimized from 5+ KB)
