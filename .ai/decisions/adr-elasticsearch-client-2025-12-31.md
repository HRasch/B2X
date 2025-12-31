# ADR: Elasticsearch Client Selection - Elastic.Clients.Elasticsearch over Nest

## Status
Accepted

## Context
The B2Connect project requires Elasticsearch integration for search functionality. Two primary .NET clients are available:
- **Nest**: The legacy, community-maintained Elasticsearch client
- **Elastic.Clients.Elasticsearch**: The official, modern client maintained by Elastic

## Decision
We will use **Elastic.Clients.Elasticsearch** as the exclusive Elasticsearch client for all new development and migrate existing code from Nest.

## Rationale
1. **Official Support**: Elastic.Clients.Elasticsearch is the official client maintained by Elastic, ensuring better compatibility and future-proofing
2. **Modern Architecture**: Built on .NET Standard 2.0+, supports async/await patterns natively, and follows modern .NET conventions
3. **Performance**: Generally better performance due to optimized serialization and connection handling
4. **Active Development**: Regular updates and bug fixes from Elastic's team
5. **Type Safety**: Strong typing with generated models for Elasticsearch APIs
6. **Community**: Growing adoption and community support

## Consequences
- **Positive**:
  - Better long-term maintainability
  - Access to latest Elasticsearch features
  - Improved performance and reliability
  - Consistent API patterns with Elastic's ecosystem

- **Negative**:
  - Migration effort required for existing Nest usage
  - Learning curve for team members familiar with Nest
  - Potential breaking changes during migration

## Implementation Plan
1. Update Program.cs to use ElasticsearchClient instead of ElasticClient
2. Remove NEST package reference from project files
3. Ensure Elastic.Clients.Elasticsearch is properly configured in Directory.Packages.props
4. Update any service registrations and dependency injection
5. Test all search functionality after migration
6. Update documentation and code comments

## Related ADRs
- None currently, this establishes the baseline for Elasticsearch client usage

## Date
2025-12-31

## Author
@Architect (via @SARAH delegation)</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/decisions/adr-elasticsearch-client-2025-12-31.md