# ADR: Domain Services as Microservices in Aspire Orchestration

## Status
Accepted

## Context
The B2Connect platform consists of multiple domain services that handle specific business capabilities. To ensure scalability, maintainability, and proper separation of concerns, all domain services should be deployed as independent microservices within the Aspire orchestration framework.

Current domain services identified:
- Identity (Auth) - Already included
- Tenancy - Already included  
- Localization - Already included
- Catalog - Already included
- Theming - Already included
- CMS - Missing API project
- Customer - Has API project, missing from orchestration
- Email - Missing API project
- Returns - Missing API project and structure
- Search - Missing API project

## Decision
All domain services will be implemented as microservices in the Aspire orchestration, with each service having its own API project following the pattern `B2Connect.{Domain}.API.csproj`.

Services will be added to `AppHost/Program.cs` with appropriate infrastructure dependencies (PostgreSQL, Redis, RabbitMQ, Elasticsearch where needed).

Service discovery will use Aspire's built-in service discovery mechanism.

## Implementation Plan
1. Create API projects for missing services:
   - CMS: Create `B2Connect.CMS.API.csproj`
   - Email: Create `B2Connect.Email.API.csproj` 
   - Search: Create `B2Connect.Search.API.csproj`
   - Returns: Create `B2Connect.Returns.API.csproj` and proper structure
   - Customer: Already has API project, add to orchestration

2. Update AppHost/Program.cs to include all services with proper configurations

3. Update gateway references to include new services where needed

4. Ensure proper database assignments for each service

## Consequences
- Improved scalability and fault isolation
- Clear service boundaries
- Increased complexity in orchestration
- Need for proper service communication patterns

## Date
2025-12-31

## Author
@Architect (via @SARAH delegation)</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/adr-domain-services-aspire-2025-12-31.md