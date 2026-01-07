---
docid: BS-REFACTOR-001
title: Strategie für effiziente große Refactorings
owner: "@Architect, @TechLead"
status: Brainstorm (In Evaluation)
created: "2026-01-07"
---

# 🏗️ Strategie: Große Refactorings effizienter machen

**Ziel**: Reduzierung der Dauer, des Komplexitäts-Overhead und des Risk großer Refactorings um 50-70%

---

## 📊 AKTUELLE HERAUSFORDERUNGEN

### Identifizierte Probleme
1. **Monolithische Refactorings** - Zu große Schritte, schwer zu reviewen
2. **Fehlende Automation** - Manuelle Codeänderungen in vielen Dateien
3. **Unzureichende Analyse** - Keine gründliche Impact-Analyse vor Start
4. **Technische Schulden** - Zu viele Abhängigkeiten entdeckt während Refactoring
5. **Token-Ineffizienz** - Große Dateien und Kontexte überlasten Copilot
6. **Testing-Bottleneck** - Unzureichende Tests verzögern Validierung
7. **Kommunikations-Overhead** - Viele kleine Updates statt klarer Milestones

---

## 🎯 KERNSTRATEGIE: "Divide & Conquer + Automation First"

### 3 Säulen

```
┌─────────────────────────────────────────────┐
│  1. PRE-ANALYSIS PHASE (Planung)            │
│     ↓ Dependency Mapping, Impact Analysis   │
├─────────────────────────────────────────────┤
│  2. INCREMENTAL EXECUTION (Mikrorefactorings) │
│     ↓ MCP-gestützte Automatisierung         │
├─────────────────────────────────────────────┤
│  3. CONTINUOUS VALIDATION (Parallel Testing)│
│     ↓ Automated Quality Gates               │
└─────────────────────────────────────────────┘
```

---

## 🔍 PHASE 1: PRE-ANALYSIS (1-2 Tage)

### 1.1 Dependency Graph erstellen
**Tool**: Roslyn MCP (Backend) / TypeScript MCP (Frontend)

```bash
# Backend: Alle Abhängigkeiten analysieren
roslyn-mcp/analyze_dependencies \
  workspacePath="backend" \
  targetComponent="ComponentToRefactor"

# Frontend: TypeScript Dependencies
typescript-mcp/analyze_types \
  workspacePath="frontend/Store" \
  filePath="src/components/TargetComponent.vue"
```

**Output**: `refactoring-dependency-graph.json`
- Alle Dateien identifizieren, die sich ändern müssen
- Circular Dependencies erkennen
- Impact-Radius bestimmen

### 1.2 Impact Radius kategorisieren
**3 Ebenen**:
- **Level 0** (Isoliert): <5 Dateien → Kann einzeln refaktoriert werden
- **Level 1** (Gekoppelt): 5-20 Dateien → Mehrere PRs nötig
- **Level 2** (Kritisch): >20 Dateien → Breaking Changes? → Neue Strategie

### 1.3 Breaking Changes identifizieren
**Tool**: API-Dokumentation & Git-History

```bash
# Altgre APIs finden
git log --oneline --all | grep -i "deprecated\|breaking"

# Contract-Breaking Changes erkennen
api-mcp/check_breaking_changes \
  oldSpec="current-api.yaml" \
  newSpec="refactored-api.yaml"
```

**Frage**: Brauchen wir Compatibility Layer (Adapter Pattern)?

---

## 📋 PHASE 2: INCREMENTAL EXECUTION

### 2.1 Refactoring in Mikroschritte aufteilen
**Regel**: Jeder PR sollte reviewbar sein (<400 Zeilen Code)

**Strategie A: "Extract First"**
```
1. PR #1: Extract neue Service/Component (mit alt-Logik, parallel)
2. PR #2: Migriere Consumer #1-3 zu neu
3. PR #3: Migriere Consumer #4-6 zu neu
4. PR #4: Entferne alte Implementation
```

**Strategie B: "Adapt Then Migrate"**
```
1. PR #1: Füge Adapter-Layer ein (alt bleibt funktional)
2. PR #2: Schreibe Consumer neu (mit Adapter)
3. PR #3: Adapter entfernen (alte API verbleibt, nutzt Adapter)
4. PR #4: Alte API deprecate + dokumentieren
```

### 2.2 MCP-Automatisierung nutzen

**Roslyn MCP (C# Refactoring)**
```bash
# Große Umbenennungen automatisieren
roslyn-mcp/refactor_code \
  filePath="backend/Domain/Catalog/Services/OldService.cs" \
  refactoringType="rename-public-member" \
  oldName="GetProducts" \
  newName="FetchAvailableProducts"

# Schnelle Codeanalyse für Pattern-Matching
roslyn-mcp/find_pattern \
  workspacePath="backend" \
  pattern="*Service.cs" \
  matchCriteria="HasPublicMethod_GetProducts"
```

**Vue/TypeScript MCP (Frontend Refactoring)**
```bash
# Component Extraction automatisieren
vue-mcp/extract_component \
  filePath="src/pages/ProductDetail.vue" \
  sectionName="SpecificationsPanel"

# i18n-Key Migration
vue-mcp/validate_i18n_keys \
  workspacePath="frontend/Store"
```

### 2.3 Git-Strategie: Single-Topic Branches

```bash
# Branch Naming nach GL-004
refactor/catalog/120-extract-product-service
refactor/catalog/121-migrate-consumer-store
refactor/catalog/122-migrate-consumer-admin
refactor/catalog/123-deprecate-old-service
refactor/catalog/124-remove-old-service
```

**Commits** (Conventional Commits):
```
refactor(catalog): extract ProductFetcher service from ProductService

- Create new ProductFetcher interface
- Extract fetch logic with zero behavior change
- Tests unchanged
```

---

## ✅ PHASE 3: CONTINUOUS VALIDATION (Parallel)

### 3.1 Automated Quality Gates (PRE-MERGE)

**Jeder PR automatisch prüfen**:
```bash
# 1. Type Safety
roslyn-mcp/analyze_types workspacePath="backend" || exit 1
typescript-mcp/analyze_types workspacePath="frontend/Store" || exit 1

# 2. Breaking Changes
api-mcp/detect_breaking_changes || exit 1

# 3. Test Coverage
test-mcp/calculate_coverage || exit 1
# Fail wenn coverage sinkt >2%

# 4. Security
security-mcp/scan_xss_vulnerabilities workspacePath="frontend" || exit 1
security-mcp/check_sql_injection workspacePath="backend" || exit 1

# 5. Architecture
archunit-mcp/validate_layering workspacePath="backend" || exit 1
```

### 3.2 Parallel Testing Strategie

**Level 1: Unit Tests** (schnell, parallel)
```bash
# Tests NUR für geänderte Services/Components
dotnet test backend/Domain/Catalog/tests/
npm run test:unit frontend/Store/
```

**Level 2: Integration Tests** (nach PR-Merge)
```bash
# E2E für kritische Flows
npm run test:e2e -- --tags=@critical
```

**Level 3: Canary Deployment** (zu Staging)
```bash
# Smoke Tests in Staging
- Login flows ✓
- Search funktioniert ✓
- Checkout abgeschlossen ✓
```

### 3.3 Kontinuierliches Monitoring

**MCP Monitoring Integration**:
```bash
# Nach jedem Merge zu main
monitoring-mcp/collect_metrics serviceName="refactored-service"
# Alerts wenn:
# - Response Time +20%
# - Error Rate >0.1%
# - Memory Leak Indizien
```

---

## 🛠️ TOOLS & AUTOMATION MATRIX

### Verfügbare MCPs für Refactoring

| Tool | Backend | Frontend | Automation |
|------|---------|----------|-----------|
| **Roslyn MCP** | ✅ Extract/Rename | — | High |
| **TypeScript MCP** | — | ✅ Type Checking | Medium |
| **Vue MCP** | — | ✅ Component Extract | High |
| **ArchUnitNET** | ✅ Architecture Validation | — | High |
| **Git MCP** | ✅ Commit Analysis | ✅ Branch Strategy | Medium |
| **Database MCP** | ✅ Schema Migration | — | High |
| **Docker MCP** | ✅ Container Updates | — | High |

---

## 📅 TIMING: SCHNELLE REFACTORINGS

### Kleine Refactorings (<5 Dateien): 2-3 Tage
```
Day 1: Analysis + PR #1 (Extract)
Day 2: PR #2 (Migrate) + Review
Day 3: Merge + Validation
```

### Mittlere Refactorings (5-20 Dateien): 1-2 Wochen
```
Week 1:
  Day 1: Complete analysis
  Day 2-3: PR #1-2 (Extract + First migrations)
  Day 4: PR #3 (More migrations)
  Day 5: Code review + final PR

Week 2:
  Day 1: Merge window, monitoring
  Day 2-3: E2E Testing, fix issues
  Day 4-5: Buffer for regressions
```

### Große Refactorings (>20 Dateien): Neu bewerten!
```
❌ Nicht: "Großes Refactoring in einem gehen"
✅ Ja: "Mehrere Medium-Refactorings in Sequence"

Beispiel: Service Rewrite
├── Phase 1: New Service parallel bauen (1 Woche)
├── Phase 2: Consumer #1-5 migrieren (1 Woche)
├── Phase 3: Consumer #6-10 + alte entfernen (1 Woche)
```

---

## 🎓 PRAKTISCHE CHECKLISTE: REFACTORING VORBEREITUNG

### Pre-Start (1 Tag)
- [ ] Dependency Graph erstellt (Roslyn/TypeScript MCP)
- [ ] Breaking Changes identifiziert (API MCP)
- [ ] Impact Radius kategorisiert (Level 0/1/2?)
- [ ] Alte Tests dokumentiert (welche passen noch?)
- [ ] Kommunikation: Team informiert + Review-Partner identifiziert

### Während Refactoring
- [ ] Jeder Commit ist reviewbar (<400 Linien)
- [ ] Tests bleiben grün (no broken builds!)
- [ ] MCP-Automation maximal nutzen (nicht manuell tippen)
- [ ] Daily standup: Blockers früh identifizieren

### Post-Merge
- [ ] Monitoring: Metrics normal? (performance, errors)
- [ ] Smoke Tests bestanden?
- [ ] Alte Code-Pfade deprecate + dokumentieren
- [ ] Lessons learned dokumentieren (`.ai/knowledgebase/lessons.md`)

---

## 🏛️ DOMAIN-SPEZIFISCHE REFACTORING PATTERNS

### A) Backend: Wolverine CQRS + Multi-Tenant Architecture

#### Pattern: Handler Extraction
**Szenario**: Großer Command Handler (500+ Zeilen) → Mehrere Handler

```csharp
// PR #1: Extract CommandValidator (new handler)
public class ValidateProductDetailsCommand : ICommand
{
    public ProductId Id { get; set; }
    public string Name { get; set; }
    // nur Validierung
}

// PR #2: Extract CommandParser (separate handler)
public class ParseProductDetailsCommand : ICommand
{
    // Parsing logic
}

// PR #3: Update original CreateProduct handler
public class CreateProductHandler
{
    public void Handle(CreateProductCommand cmd)
    {
        _mediator.Send(new ValidateProductDetailsCommand(...));
        _mediator.Send(new ParseProductDetailsCommand(...));
        // Jetzt nur Orchestration
    }
}

// PR #4: Remove old logic
```

**MCP Automation**:
```bash
# Dependency Analysis
roslyn-mcp/analyze_dependencies \
  workspacePath="backend/Domain/Catalog" \
  targetComponent="CreateProductHandler"

# Auto-extract interface
roslyn-mcp/extract_interface \
  filePath="CreateProductHandler.cs" \
  className="CreateProductHandler"
```

#### Pattern: Domain Entity Refactoring
**Szenario**: Entity hat zu viele Responsibilities

```csharp
// PR #1: Extract AggregateSpecification
public class ProductSpecification : ValueObject
{
    public string Description { get; set; }
    public List<Attribute> Attributes { get; set; }
}

// PR #2: Update Entity (composition)
public class Product : AggregateRoot
{
    public ProductSpecification Specification { get; set; }
    // moved from Product
}

// PR #3: Migrate consumers (CatalogService, etc.)
public class CatalogService
{
    public void UpdateSpecification(ProductId id, string desc)
    {
        var product = _repo.GetById(id);
        product.Specification = new ProductSpecification { Description = desc };
    }
}
```

**Multi-Tenant Consideration**:
- Ensure TenantId propagation in all handlers
- Test mit mindestens 2 verschiedenen Tenants
- Keine Daten-Leaks zwischen Tenants!

#### Pattern: Service Layer Consolidation
**Szenario**: Zu viele Services mit overlapping Concerns

```bash
# Step 1: Dependency Analysis
roslyn-mcp/find_all_calls \
  workspacePath="backend/Domain" \
  methodName="GetProductByCategory"
# Output: 23 call sites in ProductService, CatalogService, SearchService

# Step 2: Extract interface (PR #1)
# Create IProductRetriever interface

# Step 3: Move logic (PR #2-4, migrate consumers)

# Step 4: Consolidate (PR #5, remove duplicates)
```

---

### B) Frontend: Vue 3 + Pinia Refactoring

#### Pattern: Monolithic Component → Composables
**Szenario**: ProductDetail.vue (800 Zeilen) → Composables + Smaller Components

```bash
# Step 1: Analyze component complexity
typescript-mcp/analyze_complexity \
  filePath="src/pages/ProductDetail.vue"
# Output: 12 computed properties, 8 methods, 4 watchers

# Step 2: Extract useProductDetails (PR #1)
# New file: src/composables/useProductDetails.ts

# Step 3: Extract useProductReviews (PR #1)
# New file: src/composables/useProductReviews.ts

# Step 4: Extract subcomponent (PR #2)
# New file: src/components/ProductSpecifications.vue

# Step 5: Update main component (PR #3)
<script setup>
const { product, loading } = useProductDetails()
const { reviews } = useProductReviews()
</script>
```

**i18n-Sicherheit**:
```bash
# Step 0: Validate before extraction
vue-mcp/validate_i18n_keys \
  filePath="src/pages/ProductDetail.vue"
# Fail wenn hardcoded strings vorhanden

# During extraction: ensure alle Strings via $t()
```

#### Pattern: Pinia Store Refactoring
**Szenario**: CartStore hat zu viel Logic

```bash
# Step 1: Extract CartValidation Store (PR #1)
// stores/useCartValidationStore.ts
export const useCartValidationStore = defineStore(...)

# Step 2: Extract CartPersistence (PR #2)
// stores/useCartPersistenceStore.ts
export const useCartPersistenceStore = defineStore(...)

# Step 3: Keep CartStore lean (PR #3)
// Nur orchestration
export const useCartStore = defineStore('cart', {
  dependencies: [validation, persistence]
})
```

#### Pattern: Component Library Migration
**Szenario**: Alte UI Library → Neue Component Library

```bash
# Analysis Phase
typescript-mcp/find_pattern \
  workspacePath="frontend" \
  pattern="import.*OldButton" \
  # Output: 34 files, 67 usages

# Step 1: Create Adapter Component (PR #1)
// src/components/Button.vue (wraps new library)
<OldButton v-bind="$attrs" />

# Step 2-N: Migrate consumers (34 files = 5-6 PRs à 6-7 files each)
# Each PR: batch update 5-7 files (same pattern)

# Final: Remove adapter (PR #N+1)
```

---

### C) Multi-Tenant Architecture Refactoring

#### Pattern: Tenant-Aware Query Extraction
**Szenario**: TenantId wird überall hardcoded/manuell weitergegeben

```bash
# Step 1: Analyze TenantId propagation
roslyn-mcp/find_all_calls \
  workspacePath="backend" \
  pattern="GetByTenantId"

# Step 2: Extract TenantContext (PR #1)
public class TenantContext : ValueObject
{
    public TenantId Current { get; }
}

# Step 3: Update handlers to use context (PR #2-5)
public class UpdateProductHandler
{
    private readonly TenantContext _tenantContext;
    
    public void Handle(UpdateProductCommand cmd)
    {
        var product = _repo.GetByIdAndTenant(cmd.Id, _tenantContext.Current);
    }
}

# Step 4: Update repository layer (PR #6)
```

**CRITICAL CHECK**:
```bash
# Vor jedem Merge: Tenant Isolation validieren
security-mcp/validate_multitenancy \
  workspacePath="backend" \
  testTenant1="tenant-123" \
  testTenant2="tenant-456"
# Fail wenn Tenant1-Daten in Tenant2-Results!
```

---

### D) Database Schema Refactoring

#### Pattern: Backwards-Compatible Schema Migration
**Szenario**: Spalten-Umbennung oder Entity-Split

```bash
# Step 1: Plan migration
# Old: Products.Description
# New: Products.LongDescription, Products.ShortDescription

# Step 2: Add new columns (PR #1)
database-mcp/add_migration \
  migration="AddLongAndShortDescription" \
  upScript="ALTER TABLE Products ADD LongDescription, ShortDescription"
  downScript="ALTER TABLE Products DROP LongDescription, ShortDescription"

# Step 3: Data migration (PR #2)
database-mcp/data_migration \
  migration="MigrateDescriptionData"
  script="UPDATE Products SET LongDescription = Description"

# Step 4: Code migration (PR #3-5)
# Update all queries to use new columns

# Step 5: Drop old column (PR #6, final)
database-mcp/add_migration \
  migration="RemoveOldDescription"
  upScript="ALTER TABLE Products DROP Description"
```

**Timeline**:
- PR #1-5: Deployed zu Production (2 Wochen)
- PR #6: Frühestens 2 Wochen nach PR #5 merge
- → Fallback möglich, wenn Issues auftauchen

---

### E) API Contract Refactoring

#### Pattern: Contract Evolution mit Versionierung
**Szenario**: API v1 → v2 (Breaking Changes)

```bash
# Step 1: Create v2 endpoints parallel (PR #1)
[HttpPost("/api/v2/products")]
public async Task<CreateProductResponse> CreateProductV2(CreateProductRequestV2 req)
{
    // new implementation
}

# Step 2: Map v1 to v2 (Adapter, PR #2)
[HttpPost("/api/v1/products")]
public async Task<CreateProductResponse> CreateProduct(CreateProductRequestV1 req)
{
    return await CreateProductV2(MapV1ToV2(req));
}

# Step 3: Migrate consumers (Frontend, PR #3-4)
// Gradually: v1 endpoints → v2 endpoints

# Step 4: Deprecation notice (PR #5)
[Obsolete("Use /api/v2/products instead")]
public async Task<CreateProductResponse> CreateProduct(...)

# Step 5: Remove v1 (PR #6, nach 2 Releases)
```

**Contract Validation**:
```bash
api-mcp/validate_openapi \
  oldSpec="openapi-v1.yaml" \
  newSpec="openapi-v2.yaml" \
  checkBreakingChanges=true
```

---

## 💡 ANTI-PATTERNS (Was vermeiden)

| ❌ Anti-Pattern | ✅ Richtig | Domain |
|---|---|---|
| 1 PR mit 5000 Zeilen | Multiple PRs à <400 Zeilen | All |
| Refactor + neue Features together | Refactor first (PR #1), Feature second (PR #2) | All |
| Keine Tests während Refactor | Tests parallel (grün bleiben!) | All |
| Breaking Changes unangekündigt | 1-2 Release Cycles mit Deprecation Warning | API/Contract |
| Manual Copy-Paste Code-Änderungen | MCP Automation (Rename, Extract, etc.) | All |
| Refactor ohne Dependency Analysis | PRE-ANALYSIS Phase (1-2 Tage invest!) | All |
| Silence: Kein Team-Update | Daily standup + clear blockers | All |
| Refactor Handler ohne TenantId Check | Always validate tenant isolation | Multi-Tenant |
| Schema Migration ohne Fallback | Backwards-compatible, versioned migrations | Database |
| v1 API immediate removal | 2+ release cycles deprecation warning | API |
| i18n hardcoded during component extract | Validate before+after extraction | Frontend |

---

## 🚀 QUICK START: NÄCHSTES REFACTORING

### Template für Refactoring Issue:

```markdown
## Refactoring: [Component/Service Name]

### Phase 1: Analysis (Done)
- [ ] Dependency Graph erstellt
- [ ] Breaking Changes: [list]
- [ ] Impact Radius: [Level 0/1/2]
- [ ] PRs geplant: [#X-Y]

### Phase 2: Execution
- PR #120: Extract new service
- PR #121: Migrate consumers 1-3
- PR #122: Migrate consumers 4-6
- PR #123: Remove old implementation

### Phase 3: Validation
- [ ] All unit tests grün
- [ ] Performance baseline met
- [ ] E2E smoke tests passed
- [ ] Monitoring clean

### Estimated Duration: [X days]
### Risk Level: [Low/Medium/High]
```

---

## 📈 SUCCESS METRICS

**Für jedes Refactoring messen**:

| Metrik | Ziel | Tracking |
|--------|------|----------|
| **Dauer** | Kleine: <3 Tage, Medium: <2 Wochen | GitHub Issue timeline |
| **PR-Größe** | <400 Zeilen pro PR | GitHub Stats |
| **Test-Coverage** | Nicht sinken | CI reports |
| **Review-Zyklen** | <2 Reviews pro PR | PR comments count |
| **Merge-Zeit** | <1 Woche nach PR-Start | GitHub timestamps |
| **Post-Merge Issues** | Zero regressions | Monitoring + Error tracking |

---

## 🎯 KONKRETE REFACTORING CHECKLISTEN

### BACKEND: Wolverine Handler Extraction Checklist

```markdown
## Refactoring: [Handler Name] Extraction

### Phase 1: Analysis (1 Tag)
- [ ] Dependency Analysis: `roslyn-mcp/analyze_dependencies`
- [ ] Current Handler Size: [X lines]
- [ ] Identify extraction candidates: [Method #1, #2, #3]
- [ ] Breaking Changes: None expected? ✓
- [ ] Affected Tests: [X test files]

### Phase 2a: PR #1 - Extract New Handler
**Scope**: Create new handler with extracted logic
- [ ] New handler file created
- [ ] Implements ICommandHandler / IQueryHandler
- [ ] Unit tests written (mirror original tests)
- [ ] No breaking changes to public interface
- [ ] CI passes ✓
- [ ] Code review approval ✓

### Phase 2b: PR #2 - Update Original Handler
**Scope**: Refactor original to delegate to new handler
- [ ] Original handler calls new handler
- [ ] Behavior identical to PR #1 state
- [ ] Tests still pass
- [ ] No changes to external contracts
- [ ] CI passes ✓

### Phase 2c: PR #3 - Migrate Consumers (if needed)
**Scope**: Update handlers that use refactored logic
- [ ] [ConsumerA] updated
- [ ] [ConsumerB] updated
- [ ] Integration tests pass
- [ ] TenantId propagation ✓

### Phase 3: Validation (1 Tag)
- [ ] All tests pass (unit + integration)
- [ ] Performance: No regression
- [ ] Monitoring: Metrics baseline
- [ ] Tenant isolation verified ✓
- [ ] Lessons learned documented

### Timeline: 4-5 Days
### Risk: Low (breaking changes: none)
```

---

### FRONTEND: Component Extraction Checklist

```markdown
## Refactoring: [Component Name] to Composables

### Phase 1: Analysis (1 Tag)
- [ ] Component size: [X lines]
- [ ] i18n validation: `vue-mcp/validate_i18n_keys`
- [ ] No hardcoded strings? ✓
- [ ] Identify composables: [useProductLogic, useProductUI]
- [ ] Child component candidates: [SpecPanel, ReviewPanel]
- [ ] Breaking changes: None (internal refactor)

### Phase 2a: PR #1 - Extract Composables
**Scope**: Create composables with business logic
- [ ] New files created:
  - [ ] src/composables/useProductLogic.ts
  - [ ] src/composables/useProductUI.ts
- [ ] Types exported correctly
- [ ] Unit tests for composables
- [ ] i18n validated ✓
- [ ] CI passes ✓

### Phase 2b: PR #2 - Extract Child Components
**Scope**: Break large component into smaller ones
- [ ] New components created:
  - [ ] ProductSpecifications.vue
  - [ ] ProductReviews.vue
- [ ] Props typed with TypeScript
- [ ] Emits documented
- [ ] i18n in each component ✓
- [ ] CI passes ✓

### Phase 2c: PR #3 - Update Main Component
**Scope**: Refactor main component to use composables + children
- [ ] Imports from new composables/components
- [ ] <script setup> language="ts"
- [ ] Behavior identical to before
- [ ] Tests updated
- [ ] CI passes ✓

### Phase 3: Validation (1 Tag)
- [ ] Unit tests: 100% still passing
- [ ] E2E tests: Critical flows working
- [ ] i18n: All keys present ✓
- [ ] Performance: No regression
- [ ] Bundle size: Check change

### Timeline: 3-4 Days
### Risk: Low (internal refactor)
```

---

### DATABASE: Schema Migration Checklist

```markdown
## Refactoring: [Schema Change Name]

### Phase 1: Analysis (1 Tag)
- [ ] Current schema analyzed
- [ ] New schema designed
- [ ] Data migration strategy: [copy, transform, delete, etc.]
- [ ] Rollback plan documented
- [ ] Affected entities: [Product, Catalog]
- [ ] Affected queries: [X queries]

### Phase 2a: PR #1 - Add New Columns
**Scope**: Create new schema elements (backwards compatible)
```

bash
database-mcp/add_migration \
  name="AddNewColumns" \
  upScript="ALTER TABLE Products ADD NewColumn1, NewColumn2"
  downScript="ALTER TABLE Products DROP NewColumn1, NewColumn2"
```

- [ ] Migration created
- [ ] Tests: Can create/update with new columns
- [ ] Old queries still work
- [ ] CI passes ✓

### Phase 2b: PR #2 - Migrate Data
**Scope**: Transform existing data to new schema
```

bash
database-mcp/data_migration \
  name="TransformProductData" \
  script="UPDATE Products SET NewColumn1 = OldColumn1"
```

- [ ] Migration script tested locally
- [ ] Data validation: All rows migrated correctly
- [ ] Verify no data loss
- [ ] CI passes ✓

### Phase 2c: PR #3-5 - Migrate Code (multiple PRs)
**Scope**: Update code to use new columns
- [ ] [Service #1] updated
- [ ] [Service #2] updated
- [ ] [Service #3] updated
- [ ] Tests pass
- [ ] CI passes ✓

### Phase 2d: PR #6 - Cleanup (only after 2 weeks in production)
**Scope**: Remove old columns
```

bash
database-mcp/add_migration \
  name="RemoveOldColumns" \
  upScript="ALTER TABLE Products DROP OldColumn1"
```

- [ ] All code updated to new schema
- [ ] No fallback references remaining
- [ ] Production monitoring: Clean (2+ weeks)
- [ ] CI passes ✓

### Phase 3: Validation
- [ ] All tests pass
- [ ] Performance: No regression
- [ ] Monitoring: Metrics baseline
- [ ] Rollback tested (in staging)

### Timeline: 3-4 Weeks (incl. production soak time)
### Risk: Medium (data transformation risk)
### Rollback: Available for 2+ weeks
```

---

### API: Contract Evolution Checklist

```markdown
## Refactoring: [API Endpoint] Contract Update

### Phase 1: Analysis (1 Tag)
- [ ] Current endpoint documented: /api/v1/[resource]
- [ ] Breaking changes identified: [list]
- [ ] Consumer count: [X internal, Y external]
- [ ] Deprecation timeline: [2-3 releases]
- [ ] Migration difficulty: [Easy/Medium/Hard]

### Phase 2a: PR #1 - Implement v2 Endpoint
**Scope**: New endpoint with new contract
- [ ] /api/v2/[resource] created
- [ ] New request/response DTOs
- [ ] Validation rules
- [ ] Tests: Unit + integration
- [ ] OpenAPI spec updated
- [ ] CI passes ✓

### Phase 2b: PR #2 - Add v1→v2 Adapter
**Scope**: Map old to new (backwards compatibility)
- [ ] Adapter logic: MapV1ToV2()
- [ ] Tests for adapter
- [ ] v1 endpoint delegates to v2
- [ ] Behavior identical
- [ ] CI passes ✓

### Phase 2c: PR #3-4 - Migrate Consumers
**Scope**: Update internal consumers to v2
- [ ] [Frontend] updated to use v2
- [ ] [External service] contacted (if applicable)
- [ ] Tests updated
- [ ] CI passes ✓

### Phase 2d: PR #5 - Add Deprecation Warning
**Scope**: Mark v1 as deprecated
- [ ] [Obsolete] attribute added
- [ ] Response header: X-API-Deprecated: true
- [ ] Documentation: Migration guide
- [ ] Logging: Track v1 usage
- [ ] CI passes ✓

### Phase 2e: PR #6 - Remove v1 (only after 2+ releases)
**Scope**: Delete old endpoint
- [ ] All consumers migrated ✓
- [ ] Production monitoring: Zero v1 calls
- [ ] OpenAPI spec cleaned
- [ ] CI passes ✓

### Phase 3: Validation
- [ ] Contract tests: All passing
- [ ] Backwards compatibility: 2 releases maintained
- [ ] Monitoring: Migration progress tracked
- [ ] External partners notified

### Timeline: 6-8 Weeks (incl. multi-release deprecation)
### Risk: Medium (external consumers if applicable)
### Communication: External partners notified at PR #5
```

---

## 🌳 QUICK DECISION TREE: Welche Refactoring-Strategie?

```
START: Wir möchten etwas refaktorieren
  │
  ├─→ Ist es nur EINE Datei? (<100 Zeilen ändern?)
  │   └─→ JA: "Quick Fix" - 1 PR, same day
  │   └─→ NEIN: Weitermachen ↓
  │
  ├─→ Sind es <5 Dateien betroffen?
  │   └─→ JA: "Small Refactor" - 2-3 PRs, <3 Tage
  │       ├─→ PR #1: Extract/Create neu
  │       ├─→ PR #2: Migrate consumers
  │       └─→ Validieren + Done
  │   └─→ NEIN: Weitermachen ↓
  │
  ├─→ Sind es 5-20 Dateien betroffen?
  │   └─→ JA: "Medium Refactor" - 4-6 PRs, 1-2 Wochen
  │       ├─→ Phase 1: Analysis + Planning (1 Tag)
  │       ├─→ Phase 2: Extract + Migrate in Batches
  │       └─→ Phase 3: Validate + Monitor (1 Tag)
  │   └─→ NEIN: Weitermachen ↓
  │
  ├─→ Sind es >20 Dateien betroffen?
  │   └─→ JA: STOP! ❌ Zu groß für ein Refactoring
  │       └─→ LÖSUNG: "Split into Multiple Refactorings"
  │           ├─→ Refactoring A: Phase 1 (files 1-7)
  │           ├─→ Refactoring B: Phase 2 (files 8-14)
  │           └─→ Refactoring C: Phase 3 (files 15-X)
  │           └─→ Jedes einzeln durchführen (1-2 Wochen apart)
  │
  └─→ ALLE Cases: BEGINNE MIT ANALYSIS PHASE!
       ├─→ Dependency Graph erstellen (Roslyn/TypeScript MCP)
       ├─→ Breaking Changes identifizieren
       └─→ Impact Radius bestimmen
```

---

## 📈 REFACTORING REIFE-MODELL

**Wie reif ist euer Refactoring-Prozess aktuell?**

### Level 1: Chaotisch ❌
```
Merkmale:
- "Ad hoc" refactoring ohne Plan
- Große PRs (1000+ Zeilen)
- Tests brechen regelmäßig
- Unerwartete Abhängigkeiten entdeckt mid-refactor
- Keine Rollback-Strategie

Action: Implementiert "PRE-ANALYSIS Phase"!
Timeline: 1-2 Wochen Transition
```

### Level 2: Strukturiert ⚠️
```
Merkmale:
- Analysis + Planning (1-2 Tage)
- PRs mittlerer Größe (400-800 Zeilen)
- Meiste Tests grün
- Einige Überraschungen, aber manageable
- Rollback vorhanden, aber selten getestet

Action: Implementiert "MCP-Automation"!
Timeline: 2-3 Wochen Adoption
Tools:
- Roslyn MCP für C#
- TypeScript MCP für Frontend
- Database MCP für Migrations
```

### Level 3: Optimiert ✅ (ZIEL)
```
Merkmale:
- Vollständige Dependency Analysis vorher
- Micro-PRs (<400 Zeilen, 1-2 Stunden review)
- Tests grün, Monitoring baseline
- MCP-Automation für >80% der Änderungen
- Rollback getestet, bei Bedarf <30 min deployment
- Monitoring alerts für Post-Merge anomalies

Timeline: 4-6 Wochen bis Level 3 erreichbar
Requirements:
- MCP Tools konfiguriert ✓ (wir haben sie!)
- Team trained (1 Woche)
- Workflows dokumentiert (dieses Dokument!)
- Success Metrics tracking (Copilot dashboard)
```

### Level 4: Automatisiert 🚀 (Zukunft)
```
Merkmale:
- Fully automated refactoring recommendations (AI)
- Self-healing tests (generative tests)
- Zero-downtime refactorings (blue-green deployments)
- Predictive impact analysis
- Automatic rollback on anomalies

Timeline: 6-12 Months (nach Level 3 stabilisiert)
Requirements:
- Advanced MCP orchestration
- ML-basierte Code-Analyse
- Canary deployments infrastructure
```

---

## 📊 SUCCESS DASHBOARD: Was messen?

**Nach jedem Refactoring diese Metriken tracken**:

```
┌─────────────────────────────────────────────┐
│ REFACTORING SUCCESS METRICS TEMPLATE        │
├─────────────────────────────────────────────┤
│                                             │
│ Refactoring: [Name]                        │
│ Status: [In Progress / Complete]           │
│ Date Range: [Start] - [End]                │
│                                             │
├─ EXECUTION ─────────────────────────────────┤
│ Duration:          [Planned: 5d, Actual: 4d] ✓
│ PRs created:       [5/5 planned] ✓
│ PR avg size:       [350 lines] ✓
│ PR review cycles:  [avg 1.2] ✓
│ Merge time:        [3 days] (target: <5)
│                                             │
├─ QUALITY ───────────────────────────────────┤
│ Test coverage:     [87% → 89%] ✓
│ Tests failing:     [0] ✓
│ Breaking changes:  [0 unexpected] ✓
│ Security issues:   [0] ✓
│ Code review:       [+5 feedback items]
│                                             │
├─ PERFORMANCE ───────────────────────────────┤
│ Response time:     [15ms → 14ms] ✓
│ Error rate:        [0.05% → 0.03%] ✓
│ Memory usage:      [150mb → 148mb] ✓
│ DB queries:        [42 → 38] ✓ (optimized!)
│                                             │
├─ TEAM FEEDBACK ──────────────────────────────┤
│ Difficulty:        [Medium] (analysis helped)
│ Automation value:  [High] (MCP saved 10h)
│ Process rating:    [4/5] (clear PRs)
│ Improvements:      [Add security checks]
│                                             │
├─ LESSONS ───────────────────────────────────┤
│ What went well:    [Micro-PRs strategy]
│ What was hard:     [TenantId propagation]
│ Next time:         [Better unit tests first]
│                                             │
└─────────────────────────────────────────────┘
```

**→ Dieses Template in `.ai/templates/` speichern für Wiederverwendung**

---

## 🎓 TRAINING PLAN: Team vorbereiten

**Wenn ihr Level 3 erreichen wollt, braucht ihr:**

### Week 1: Fundamentals (4 Stunden)
- [ ] Diese Strategie durchlesen (1h)
- [ ] Dependency Analysis Demo mit Roslyn MCP (1h)
- [ ] Decision Tree durchspielen (team exercise, 1h)
- [ ] Q&A + Diskussion (1h)

### Week 2: Hands-On (6 Stunden)
- [ ] Pilot Refactoring starten (mittel, 5-7 Dateien)
- [ ] Täglich Standup (blockers, learnings)
- [ ] Code Review Training (bei jedem PR)

### Week 3: Optimization (3 Stunden)
- [ ] Pilot Retrospektive (lessons learned)
- [ ] MCP automation improvements (basiert auf Feedback)
- [ ] Dokumentation updaten

### Ongoing: Monthly Refresher (1 Stunde)
- [ ] Lessons learned review
- [ ] Metrics dashboard update
- [ ] Process improvements

---

## 🚀 NEXT STEPS: Implementierungs-Plan

### Immediate (This Week)
- [ ] Diese Strategie mit @TechLead + @Architect reviewen
- [ ] Bestätigung: Wollt ihr Level 3 erreichen?
- [ ] Pilot Refactoring identifizieren (mittel, nicht groß!)

### Short-term (Next 2 Weeks)
- [ ] Team Training durchführen (Weeks 1-2 oben)
- [ ] Pilot Refactoring starten
- [ ] MCP Tools konfigurieren (sollte schon done sein)

### Medium-term (Weeks 3-4)
- [ ] Pilot durchführen + Lessons learned
- [ ] Prozess optimieren basiert auf Feedback
- [ ] 2. Refactoring mit optimiertem Prozess

### Long-term (Months 2-3)
- [ ] Level 3 Reife erreichen + stabilisieren
- [ ] Alle großen Refactorings mit dieser Strategie durchführen
- [ ] Metrics zeigen: 50-70% Effizienzgewinn

---

## 🔗 VERWANDTE DOKUMENTE

- [GL-004] Branch Naming Strategy
- [GL-006] Token Optimization
- [KB-052] Roslyn MCP
- [KB-053] TypeScript MCP
- [KB-054] Vue MCP
- [ADR-021] ArchUnitNET Architecture Testing

---

## 🎯 Nächste Schritte

1. **Diese Strategie validieren**: Review mit @Architect + @TechLead
2. **Template erstellen**: Issue Template für Refactoring-Issues
3. **MCP-Scripts schreiben**: Automation für Analyze-Phase
4. **Piloten**: Nächstes Refactoring mit dieser Strategie durchführen
5. **Lessons Learned**: Nach Pilot, dieses Dokument updaten

---

**Status**: 🟡 Brainstorm (Ready for Review)  
**Nächste Review**: Nach erstem Pilot-Refactoring
