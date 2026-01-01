# ADR: Removal of MediatR Dependency

## Status
Accepted

## Date
2025-12-31

## Context
The B2Connect project initially used MediatR for implementing the CQRS (Command Query Responsibility Segregation) pattern. However, with the adoption of Wolverine as the messaging framework, MediatR is no longer needed as Wolverine provides superior capabilities for handling commands, queries, and events in a distributed system.

Wolverine offers:
- Built-in support for CQRS patterns
- Message routing and handling
- Better integration with ASP.NET Core and distributed systems
- Improved performance and scalability

## Decision
We will remove the MediatR dependency from all projects in the B2Connect solution. This includes:
- Removing MediatR package references from all .csproj files
- Removing any commented-out MediatR configuration code
- Ensuring no active usage of MediatR in the codebase

## Consequences
### Positive
- Reduced dependency footprint
- Simplified architecture with Wolverine as the single messaging framework
- Better alignment with modern .NET messaging patterns

### Negative
- Potential need for minor refactoring if any hidden MediatR usage exists (though analysis shows none)

### Risks
- Low risk: Analysis shows MediatR is only referenced in package manifests and one commented code block

## Implementation Plan
1. Remove MediatR PackageReference from the following projects:
   - B2Connect.Localization.API.csproj
   - B2Connect.Identity.API.csproj
   - B2Connect.CMS.csproj
   - B2Connect.Theming.API.csproj
   - B2Connect.Tenancy.API.csproj
   - B2Connect.Catalog.API.csproj

2. Remove commented MediatR configuration in backend/Domain/Identity/Program.cs (lines 185-186)

3. Run full build and tests to ensure no breaking changes

4. Update package lock files and commit changes

## Alternatives Considered
- Keep MediatR alongside Wolverine: Rejected due to unnecessary complexity
- Gradual migration: Not needed as Wolverine fully replaces MediatR functionality

## References
- Wolverine Implementation: backend/WOLVERINE_IMPLEMENTATION.md
- CQRS Pattern: https://martinfowler.com/bliki/CQRS.html
- Wolverine Documentation: https://wolverinefx.net/