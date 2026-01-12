---
docid: INS-008
title: Backend Essentials.Instructions
owner: @CopilotExpert
status: Active
created: 2026-01-08
---

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
- **MANDATORY**: Run Roslyn MCP validation (`roslyn-mcp/analyze_types`) before editing any .NET files to catch errors early and save tokens on iterations

## runSubagent for Backend Analysis (Token-Optimized)

For comprehensive backend validation, use `#runSubagent` to execute all analyses in isolation:

```text
Validate backend code with #runSubagent:
- Run Roslyn type analysis for {workspacePath}
- Check for breaking changes in public APIs
- Validate Wolverine CQRS patterns (handlers, sagas, events)
- Analyze message handler dependencies
- Validate DI container configuration

Return ONLY: type_errors + breaking_changes + pattern_violations + fix_recommendations
```

**Benefits**:
- ~3500 Token savings per backend analysis
- All 5 checks run in isolated context
- Only actionable issues returned to main context
- Prevents regressions in CQRS patterns and public APIs

**When to use**: Before committing .NET changes, PR reviews, refactoring

---

## MCP Tools

For detailed backend-specific MCP tools documentation:

```
kb-mcp/search_knowledge_base
  query: "Roslyn MCP" OR "Backend security" OR "Database MCP"
```

See [KB-052] Roslyn MCP, [KB-055] Security MCP, [KB-057] Database MCP

## Large File Editing Strategy ([GL-043])

When editing large .NET files (>200 lines), use the Multi-Language Fragment Editing approach with Roslyn MCP and Wolverine MCP integration:

### Pre-Edit Analysis
```bash
# Semantic analysis of types and dependencies
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

# Find usage patterns and breaking changes
list_code_usages("ClassName", filePaths: ["backend/**"])
roslyn-mcp/check_breaking_changes workspacePath="backend" filePath="Service.cs"
```

### Fragment-Based Editing Patterns
```csharp
// Fragment: Business logic method (80% token savings)
public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
{
    // Edit only this method body, reference external dependencies
    var validation = await _validator.ValidateAsync(request);
    if (!validation.IsValid) return Result.Failure<Order>(validation.Errors);
    
    // ... rest of method implementation
}
```

**Roslyn MCP Workflows**:
```bash
# 1. Type safety validation before edits
roslyn-mcp/analyze_types workspacePath="backend/Domain/Catalog"

# 2. Fragment extraction and refactoring
read_file("Service.cs", startLine: 45, endLine: 85)
roslyn-mcp/invoke_refactoring fileUri="Service.cs" name="source.unusedImports"
roslyn-mcp/invoke_refactoring fileUri="Service.cs" name="source.addTypeAnnotation"

# 3. Dependency injection validation
roslyn-mcp/validate_di_container workspacePath="backend" filePath="Program.cs"

# 4. Breaking change detection
roslyn-mcp/check_breaking_changes workspacePath="backend" filePath="ApiController.cs"
```

**Wolverine MCP Integration**:
```bash
# CQRS pattern validation
wolverine-mcp/validate_cqrs_patterns workspacePath="backend" filePath="Commands/OrderCommand.cs"

# Message handler analysis
wolverine-mcp/analyze_message_handlers workspacePath="backend/Domain"

# Saga orchestration validation
wolverine-mcp/validate_sagas workspacePath="backend" filePath="OrderSaga.cs"

# Event sourcing patterns
wolverine-mcp/check_event_sourcing workspacePath="backend/Domain/Events"
```

### Quality Gates
- Always run `get_errors()` after edits
- Execute related unit tests with `runTests()`
- Use `roslyn-mcp/analyze_types` for semantic validation
- Validate Wolverine patterns with dedicated MCP tools

**Token Savings**: 80% vs. reading entire files | **Quality**: Compiler-level validation with CQRS pattern enforcement

## Temp-File Usage for Token Optimization

For large outputs during task execution (e.g., test results, logs >1KB), save to temp files to reduce token consumption:

```bash
# Auto-save large test output
OUTPUT=$(dotnet test --verbosity minimal)
if [ $(echo "$OUTPUT" | wc -c) -gt 1024 ]; then
  bash scripts/temp-file-manager.sh create "$OUTPUT" txt
else
  echo "$OUTPUT"
fi

# Reference in prompts/responses
"See temp file: .ai/temp/task-uuid.json (5KB saved)"
```

- Auto-cleanup after 1 hour or task completion.
- Complements [GL-006] token optimization strategy.

---

**Full documentation**: Use `kb-mcp/get_article` or search Knowledge Base  
**Size**: 2.1 KB (with temp-file additions)
