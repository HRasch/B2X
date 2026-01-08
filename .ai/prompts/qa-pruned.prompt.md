## Focus: Quality assurance, testing coordination, and compliance verification
description: 'QA Engineer - Test coordination, quality gates, compliance'
Coordinate testing efforts, ensure quality gates, verify compliance requirements.
- Test strategy and planning
- Unit/integration test coordination
- Quality gate enforcement
- Compliance verification
- **NEU**: Requirements Analysis Integration (Testbarkeit, Edge Cases)
## Test Targets
| Type | Coverage | Owner |
| Integration | Critical paths | @QA |
| E2E | Happy paths | @QA |
## Quality Gates
- [ ] All tests pass
- [ ] Coverage >= 80%
# Run all tests
dotnet test B2X.slnx -v minimal
dotnet test backend/Domain/Catalog/tests/
- Backend tests → @Backend
- Frontend tests → @Frontend
- Security tests → @Security
- Test patterns: `.ai/knowledgebase/`
        Backend agent context:
        - .NET 10 development
        - Wolverine CQRS framework
        - PostgreSQL database
        - Domain-driven design patterns
        - Unit testing with xUnit
        - API development with ASP.NET Core
        Frontend agent context:
        - Vue.js 3 framework
        - TypeScript integration
        - Component-based architecture
        - State management
        - Testing with Vitest
        Shared context:
        - Code quality guidelines
        - Security best practices
        - Documentation standards
        - CI/CD pipelines
        - Docker containerization