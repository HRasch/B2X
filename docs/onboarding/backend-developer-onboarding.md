# Developer Onboarding Guide: Backend Development

**Role:** Backend Developer (.NET/Wolverine)  
**Duration:** 2-3 weeks  
**Prerequisites:** C#, .NET experience, basic microservices knowledge

## Week 1: Foundation & Setup

### Day 1-2: Environment Setup
**Goals:** Local development environment running

#### Tasks:
1. **Install Prerequisites**
   ```bash
   # .NET 8 SDK
   dotnet --version  # Should show 8.x.x

   # PostgreSQL
   brew install postgresql
   brew services start postgresql

   # Redis (optional for full stack)
   brew install redis
   brew services start redis
   ```

2. **Clone & Setup Repository**
   ```bash
   git clone [repo-url]
   cd B2Connect
   dotnet restore
   ```

3. **Run Aspire Dashboard**
   ```bash
   cd AppHost
   dotnet run
   # Visit http://localhost:15500
   ```

4. **Database Setup**
   ```bash
   # Create databases (if not using Aspire)
   createdb b2connect_identity
   createdb b2connect_catalog
   # etc.
   ```

#### Verification:
- ✅ Aspire dashboard shows all services green
- ✅ Can access Swagger UI at service endpoints
- ✅ Database connections working

### Day 3-5: Core Concepts
**Goals:** Understand architecture and patterns

#### Learn:
1. **Wolverine Framework**
   - HTTP handlers vs message handlers
   - Event-driven architecture
   - Dependency injection patterns

2. **Domain-Driven Design**
   - Bounded contexts (Identity, Catalog, CMS, etc.)
   - Aggregates and entities
   - Repository patterns

3. **Database Layer**
   - Entity Framework Core setup
   - Migrations and seeding
   - Multi-tenant data isolation

#### Resources:
- [Wolverine Quick Reference](docs/architecture/WOLVERINE_QUICK_REFERENCE.md)
- [DDD Bounded Contexts](docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- [API Documentation](api/)

## Week 2: Development Workflow

### Day 6-8: First Feature Development
**Goals:** Complete end-to-end feature implementation

#### Tasks:
1. **Choose a Simple Feature**
   - Add a new product field
   - Create a basic API endpoint
   - Implement data validation

2. **Follow Development Process**
   ```bash
   # 1. Create feature branch
   git checkout -b feature/add-product-field

   # 2. Implement changes
   # - Update entity/model
   # - Add repository method
   # - Create Wolverine handler
   # - Add API endpoint

   # 3. Write tests
   dotnet test

   # 4. Update documentation
   # Add XML comments to new code
   ```

3. **Testing & Validation**
   - Unit tests for business logic
   - Integration tests for API
   - Manual testing via Swagger

#### Code Review Checklist:
- [ ] XML documentation added
- [ ] Unit tests written
- [ ] Integration tests pass
- [ ] No breaking changes
- [ ] Follows DDD patterns

### Day 9-10: Advanced Topics
**Goals:** Understand complex scenarios

#### Learn:
1. **Event-Driven Architecture**
   - Publishing domain events
   - Event handlers and subscribers
   - Cross-service communication

2. **Multi-Tenancy**
   - Tenant isolation
   - Data partitioning
   - Tenant-specific logic

3. **Performance & Monitoring**
   - OpenTelemetry integration
   - Health checks
   - Logging patterns

## Week 3: Production Readiness

### Day 11-12: Deployment & Operations
**Goals:** Understand production deployment

#### Learn:
1. **CI/CD Pipeline**
   - GitHub Actions workflows
   - Automated testing
   - Deployment to staging/production

2. **Monitoring & Alerting**
   - Application Insights setup
   - Error tracking
   - Performance monitoring

3. **Security Best Practices**
   - Authentication/authorization
   - Input validation
   - Secure coding guidelines

### Day 13-15: Contributing & Best Practices
**Goals:** Become a contributing team member

#### Tasks:
1. **Code Review Participation**
   - Review 2-3 PRs from other developers
   - Provide constructive feedback
   - Learn from senior reviews

2. **Documentation Contribution**
   - Update API docs for your changes
   - Contribute to troubleshooting guides
   - Document any gotchas discovered

3. **Team Processes**
   - Sprint planning participation
   - Daily standup contributions
   - Retrospective input

## Success Metrics

**Technical Skills:**
- ✅ Can implement CRUD operations
- ✅ Understands event-driven patterns
- ✅ Writes comprehensive tests
- ✅ Follows DDD principles

**Process Adherence:**
- ✅ Creates proper PRs with descriptions
- ✅ Participates in code reviews
- ✅ Documents API changes
- ✅ Follows conventional commits

**Team Integration:**
- ✅ Contributes to team discussions
- ✅ Helps other developers
- ✅ Understands business domain

## Resources & Support

**Documentation:**
- [Backend Development Instructions](.github/instructions/backend.instructions.md)
- [Troubleshooting Guide](docs/troubleshooting/developer-troubleshooting.md)
- [Architecture Overview](docs/architecture/)

**Team Support:**
- **Mentor:** [Assign senior developer]
- **Slack:** #backend-dev, #dev-support
- **Office Hours:** [Schedule regular check-ins]

**Key Contacts:**
- **Tech Lead:** [Name] - Architecture questions
- **DevOps:** [Name] - Infrastructure/deployment
- **QA:** [Name] - Testing strategies

## Next Steps

After completing this guide:
1. **Shadow senior developers** on complex features
2. **Take ownership** of a small service or feature area
3. **Participate in on-call rotation** (after additional training)
4. **Mentor new hires** using this guide

Remember: Learning is continuous. Focus on understanding why decisions are made, not just how to implement them.