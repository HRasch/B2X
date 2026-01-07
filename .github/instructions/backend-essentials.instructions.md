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

## Governance & Process (Plan-Act-Control)
- Require link to an ADR or issue with acceptance criteria and test plan in every PR that changes behavior
- Owners: Plan = feature author, Act = implementer, Control = QA/Security/TechLead

## Static Analysis & Quality Gate
- Enable StyleCop Analyzers and relevant Roslyn analyzers
- Treat new analyzer warnings as errors in CI; legacy warnings triaged via TODO tickets
- Add `dotnet format --verify-no-changes` as a CI step
- CI must publish analyzer reports, coverage, and test results as artifacts
- Fail PRs on regressions in gates (build, analyzers, unit tests)

## Developer Guidance
- Document `scripts/run-local-checks.sh` usage in `CONTRIBUTING.md`
- Integration tests for database/repository interactions; use in-memory providers only for unit tests
- Add consumer-driven contract tests for gateway APIs where applicable
- Add a CI validation step that checks for missing i18n keys across supported locales

## MCP Tools

For detailed backend-specific MCP tools documentation:

```
kb-mcp/search_knowledge_base
  query: "Roslyn MCP" OR "Backend security" OR "Database MCP"
```

See [KB-052] Roslyn MCP, [KB-055] Security MCP, [KB-057] Database MCP

---

**Full documentation**: Use `kb-mcp/get_article` or search Knowledge Base  
**Size**: 2.0 KB (with governance additions)
