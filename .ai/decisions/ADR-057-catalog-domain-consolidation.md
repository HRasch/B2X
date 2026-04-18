---
docid: ADR-057
title: Catalog Domain Consolidation - Unify Categories, Products, and Variants
status: Proposed
owner: @Backend, @Architect
approved-by: @SARAH
created: 2026-01-11
---

# ADR-057: Catalog Domain Consolidation

## Context

The current B2X Store bounded context violates Domain-Driven Design principles by distributing tightly coupled catalog entities across separate bounded contexts:

- **Catalog Domain**: Manages Product entities with weak string-based category references
- **Categories Domain**: Manages Category hierarchies independently  
- **Variants Domain**: Manages ProductVariant entities separately

This creates several architectural problems:

1. **Inconsistent Domain Boundaries**: Catalog-related entities are split across microservices despite strong coupling
2. **Weak Entity Relationships**: Products reference categories by name (strings) instead of proper entity relationships
3. **Distributed Data Consistency**: No transactional consistency across catalog operations
4. **CQRS Handler Fragmentation**: Business logic scattered across separate services
5. **Service Coupling**: Store Gateway must orchestrate multiple service calls for catalog operations

## Problem Statement

The Catalog domain should be responsible for all catalog-dependent data (Categories, Products, Variants, Attributes, RelatedProducts, ProductUnits, ...) as stated in the implementation requirements. The current separation violates this principle and creates unnecessary complexity.

## Decision

**Consolidate Categories and Variants into the unified Catalog bounded context** with proper DDD aggregate design.

### New Domain Structure

```
B2X.Store.Domain.Catalog/
├── Core/
│   ├── Entities/
│   │   ├── Product.cs (Aggregate Root)
│   │   ├── Category.cs (Aggregate Root)
│   │   ├── Variant.cs (Entity)
│   │   └── Catalog.cs (Aggregate Root)
│   ├── ValueObjects/
│   │   ├── Sku.cs, Price.cs, CategoryPath.cs
│   ├── DomainEvents/
│   │   ├── ProductCategorized.cs
│   │   ├── CategoryHierarchyChanged.cs
│   └── Interfaces/ICatalogRepository.cs
├── Application/
│   ├── Commands/ (unified command handlers)
│   ├── Queries/ (unified query handlers)
│   └── DTOs/ (unified data transfer objects)
├── Infrastructure/
│   ├── Repositories/CatalogRepository.cs
│   └── Data/CatalogDbContext.cs
└── API/Controllers/ (unified endpoints)
```

### Key Changes

1. **Proper Aggregate Relationships**:
   - Product references CategoryIds (not strings)
   - Variants belong to Products
   - Categories maintain hierarchical relationships

2. **Unified CQRS Handlers**:
   - Single command handler for all catalog operations
   - Unified query handler for catalog browsing

3. **Single Service Boundary**:
   - One catalog service instead of three separate services
   - Consolidated API endpoints

4. **Domain Events for Consistency**:
   - ProductCategorized, CategoryHierarchyChanged, VariantStockChanged

## Consequences

### Positive

- ✅ **DDD Compliance**: Proper bounded context with strong consistency
- ✅ **Simplified Architecture**: Single service for catalog operations
- ✅ **Better Performance**: No cross-service calls for related data
- ✅ **Maintainability**: Unified business logic and data models
- ✅ **Data Consistency**: Transactional integrity across catalog entities

### Negative

- ❌ **Breaking Changes**: API contracts and data models change
- ❌ **Migration Complexity**: Database schema and service consolidation
- ❌ **Deployment Risk**: Large refactoring requires careful rollout

### Risks

- **Data Migration**: Converting string categories to ID relationships
- **Service Downtime**: During consolidation and testing
- **Frontend Updates**: API contract changes require frontend updates

## Implementation Plan

### Phase 1: Domain Model Consolidation
1. Create unified Catalog domain with proper aggregates
2. Implement domain events and business rules
3. Add comprehensive unit tests

### Phase 2: Database Migration
1. Add foreign key relationships to existing schema
2. Migrate string categories to CategoryId arrays
3. Create migration scripts with rollback capability

### Phase 3: Service Consolidation
1. Update Admin Gateway to use unified domain
2. Consolidate Store Gateway service clients
3. Update AppHost configuration

### Phase 4: API Migration
1. Implement unified catalog API endpoints
2. Maintain backward compatibility during transition
3. Update frontend integrations

### Phase 5: Testing and Deployment
1. Comprehensive integration testing
2. Gradual rollout with feature flags
3. Monitoring and rollback procedures

## Alternatives Considered

### Alternative 1: Keep Separate Services
- **Pros**: Incremental changes, service isolation
- **Cons**: Violates DDD principles, ongoing complexity

### Alternative 2: Event-Driven Architecture
- **Pros**: Loose coupling, scalability
- **Cons**: Eventual consistency complexity, higher latency

### Alternative 3: Database Views (Rejected)
- **Pros**: Minimal code changes
- **Cons**: Poor performance, no domain integrity

## Decision Timeline

- **Proposed**: January 11, 2026
- **Review**: January 12-13, 2026 (@Architect, @TechLead)
- **Implementation Start**: January 15, 2026
- **Completion Target**: February 15, 2026

## References

- [ADR-046: Unified Category Navigation Architecture](ADR-046-unified-category-navigation.md)
- [ARCH-001: Project Structure - Verified](docs/architecture/components/PROJECT_STRUCTURE.md)
- DDD Bounded Contexts: [DDD_BOUNDED_CONTEXTS.md](docs/architecture/DDD_BOUNDED_CONTEXTS.md)</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\decisions\ADR-057-catalog-domain-consolidation.md