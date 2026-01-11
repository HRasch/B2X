---
docid: BS-PROBLEM-SOLVING-001
title: Problem-Solving and Refactoring Patterns for KB
owner: "@Architect, @TechLead"
status: Brainstorm
created: "2026-01-11"
---

# 🧩 Problem-Solving & Refactoring Patterns for Knowledge Base

**Goal**: Develop reusable, documented patterns that guide AI agents and developers through common problem-solving scenarios and refactoring tasks.

---

## 📊 Current Gaps Analysis

### What We Have (Existing KB)
| Category | DocID | Coverage |
|----------|-------|----------|
| Patterns/Antipatterns | KB-011 | General design patterns |
| Lessons Learned | KB-LESSONS | Session-specific learnings |
| Feature Implementation | KB-146 | End-to-end feature workflow |
| Refactoring Strategy | BS-REFACTOR-001 | Large refactoring methodology |
| Vue3 Composition | KB-PAT | Component patterns |

### What's Missing
1. **Diagnostic Decision Trees** - Structured problem diagnosis workflows
2. **Error Pattern Library** - Common error categories with solutions
3. **Refactoring Recipes** - Step-by-step transformation templates
4. **Recovery Patterns** - How to recover from failed changes
5. **Context-Switching Patterns** - Efficient handoff between agents
6. **Debugging Workflows** - Systematic debugging approaches

---

## 🎯 PROPOSED KB ADDITIONS

### 1. Problem Diagnosis Patterns (KB-DIAG-*)

#### KB-DIAG-001: Build Failure Diagnosis
```
Build Failed?
├─ Compilation Error
│   ├─ Type Error → Check recent interface/model changes
│   ├─ Missing Reference → Verify package restore
│   └─ Syntax Error → Check for incomplete edits
├─ NuGet Restore Failed
│   ├─ Version Conflict → Check Directory.Packages.props
│   ├─ Feed Unreachable → Check nuget.config
│   └─ Framework Mismatch → Verify TFM compatibility
├─ Project Resolution Order
│   ├─ Circular Reference → Break dependency cycle
│   ├─ Missing ProjectReference → Add to .csproj
│   └─ Build Order Wrong → Check transitive dependencies
└─ MSBuild Error
    ├─ SDK Missing → Verify global.json
    └─ Target Error → Check custom targets
```

#### KB-DIAG-004: Project Resolution Order Diagnosis
```
Project Order Issues?
├─ Symptoms
│   ├─ "Type not found" but code exists → Build order wrong
│   ├─ Intermittent build failures → Race condition in parallel build
│   └─ Works locally, fails in CI → Different restore/build flags
│
├─ Diagnostic Commands
│   ├─ dotnet build --verbosity detailed 2>&1 | Select-String "Project"
│   ├─ dotnet msbuild -preprocess:full.xml (see full import chain)
│   └─ dotnet build -graph (visualize dependency graph)
│
├─ Common Causes
│   ├─ Missing ProjectReference
│   │   └─ Fix: Add <ProjectReference Include="..\..\Path\Project.csproj"/>
│   ├─ Circular Dependencies
│   │   ├─ A → B → C → A (direct cycle)
│   │   └─ Fix: Extract shared interface project, invert dependency
│   ├─ Implicit Dependencies (not in .csproj)
│   │   ├─ Code uses type from Project X but no reference
│   │   └─ Fix: Add explicit ProjectReference
│   ├─ Transitive Reference Mismatch
│   │   ├─ A refs B (v1), C refs B (v2)
│   │   └─ Fix: Align versions in Directory.Packages.props
│   └─ Parallel Build Race Conditions
│       ├─ -m:N flag causes non-deterministic order
│       └─ Fix: Ensure all dependencies explicit, or reduce parallelism
│
├─ Resolution Steps
│   1. Map current dependency graph
│   2. Identify missing/incorrect edges
│   3. Check for cycles (topological sort fails)
│   4. Add missing ProjectReferences
│   5. Verify with clean build: dotnet clean && dotnet build
│
└─ Prevention
    ├─ Use solution filters (.slnf) for focused builds
    ├─ Run `dotnet build -graph` in CI to detect issues early
    └─ Document expected build order in README
```

#### KB-DIAG-005: Solution Structure & Build Order Analysis
```
Analyze Solution Build Order:

1. EXTRACT PROJECT GRAPH
   dotnet msbuild B2X.slnx -t:GenerateRestoreGraphFile -p:RestoreGraphOutputPath=graph.json
   
2. VISUALIZE DEPENDENCIES
   # PowerShell: List all project references
   Get-ChildItem -Recurse *.csproj | ForEach-Object {
     $proj = $_.Name
     Select-Xml -Path $_ -XPath "//ProjectReference" | ForEach-Object {
       "$proj -> $($_.Node.Include | Split-Path -Leaf)"
     }
   }

3. DETECT CYCLES
   # If build hangs or fails with circular reference:
   dotnet build --verbosity diagnostic 2>&1 | Select-String "circular"

4. OPTIMAL BUILD ORDER (Topological Sort)
   # Projects with no dependencies build first
   # Projects depending only on "leaf" projects build second
   # Continue until all projects ordered
   
   Typical B2X Order:
   ┌─ Level 0: Shared.Contracts, Shared.Abstractions
   ├─ Level 1: Shared.Infrastructure, Domain.Core
   ├─ Level 2: Domain.Catalog, Domain.Identity, Domain.Orders
   ├─ Level 3: Store.API, Admin.API, Management.API
   └─ Level 4: AppHost (orchestration)

5. PARALLEL BUILD OPTIMIZATION
   # Max parallelism = number of independent branches
   dotnet build -m:4  # Adjust based on dependency depth
```

#### KB-DIAG-002: Runtime Failure Diagnosis
```
Runtime Exception?
├─ NullReferenceException
│   ├─ Async/Await Issue → Check task completion
│   ├─ DI Not Registered → Verify service registration
│   └─ Data Not Loaded → Check initialization order
├─ FileNotFoundException
│   ├─ Assembly → Check deps.json + output directory
│   ├─ Resource → Verify Build Action = EmbeddedResource
│   └─ Config → Check appsettings.json paths
└─ InvalidOperationException
    ├─ Thread Safety → Check concurrent access
    └─ State Machine → Check object lifecycle
```

#### KB-DIAG-003: Frontend Error Diagnosis
```
Frontend Error?
├─ Build Error
│   ├─ TypeScript → Run typescript-mcp/analyze_types
│   ├─ ESLint → Check rule conflicts
│   └─ Vite/Nuxt → Check plugin compatibility
├─ Runtime Error
│   ├─ Vue Hydration → Check SSR/CSR mismatches
│   ├─ Pinia State → Verify store initialization
│   └─ API Error → Check CORS and endpoints
└─ Visual Error
    ├─ CSS Conflict → Check specificity
    ├─ Layout Shift → Use stable dimensions
    └─ i18n Missing → Run vue-mcp/validate_i18n_keys
```

#### KB-DIAG-006: Outdated Information Detection & Research Triggers
```
Information Might Be Outdated?
├─ DETECTION SIGNALS
│   ├─ Version Mismatch
│   │   ├─ KB says "v1.x" but package.json/csproj shows "v2.x"
│   │   ├─ API signatures don't match documentation
│   │   └─ Deprecated warnings in build output
│   ├─ Date Indicators
│   │   ├─ KB article >6 months old without update
│   │   ├─ Referenced GitHub issues are closed/resolved
│   │   └─ "Coming soon" features that should exist now
│   ├─ Behavioral Mismatch
│   │   ├─ Code pattern from KB doesn't compile
│   │   ├─ Expected behavior differs from actual
│   │   └─ Error messages don't match documentation
│   └─ Ecosystem Signals
│       ├─ Major version released (breaking changes likely)
│       ├─ Package marked deprecated on npm/NuGet
│       └─ Framework announced EOL or migration path
│
├─ RESEARCH TRIGGERS (When to fetch_webpage)
│   ├─ ALWAYS Research:
│   │   ├─ Package version upgrades (major.x)
│   │   ├─ Security vulnerability fixes
│   │   ├─ Breaking changes mentioned in errors
│   │   └─ New framework features (preview → stable)
│   ├─ CONSIDER Research:
│   │   ├─ Build errors with unfamiliar messages
│   │   ├─ Deprecated API usage warnings
│   │   └─ Performance issues with known libs
│   └─ SKIP Research (use cached KB):
│       ├─ Stable patterns (SOLID, DDD, etc.)
│       ├─ Internal project conventions
│       └─ Recently validated information (<1 month)
│
├─ RESEARCH WORKFLOW
│   1. Identify knowledge gap/staleness
│   2. Determine authoritative sources:
│   │   ├─ Official docs: docs.microsoft.com, vuejs.org
│   │   ├─ Release notes: GitHub releases, changelogs
│   │   ├─ Package registries: nuget.org, npmjs.com
│   │   └─ Issue trackers: GitHub issues (for known bugs)
│   3. Use fetch_webpage with specific queries
│   4. Cross-validate with multiple sources
│   5. Update KB article with findings + date
│
└─ POST-RESEARCH ACTIONS
    ├─ Update KB article with new information
    ├─ Add "Last Verified" date to article
    ├─ Create lessons.md entry if significant
    └─ Flag related code for potential updates
```

#### KB-DIAG-007: Knowledge Freshness Validation
```
Validate Knowledge Before Using:

1. CHECK PACKAGE VERSIONS
   # NuGet - compare KB version vs actual
   dotnet list package --outdated
   
   # npm - check for updates  
   npm outdated
   
   # If major version differs → RESEARCH REQUIRED

2. VERIFY API SIGNATURES
   # Does the documented API still exist?
   # Check for [Obsolete] attributes
   # Look for breaking change annotations

3. FRESHNESS INDICATORS IN KB
   ✅ Fresh (use directly):
      - "Last Updated: [within 30 days]"
      - "Verified with v[current version]"
      - Links to current documentation
   
   ⚠️ Stale (verify before using):
      - No update date or >3 months old
      - References old version numbers
      - Contains "TODO: verify" markers
   
   ❌ Outdated (research required):
      - >6 months without update
      - References deprecated packages
      - Code examples don't compile

4. AUTHORITATIVE SOURCES BY TECHNOLOGY
   | Technology | Primary Source | Changelog |
   |------------|----------------|-----------|
   | .NET/C# | docs.microsoft.com | GitHub releases |
   | ASP.NET Core | learn.microsoft.com | Release notes |
   | Wolverine | wolverine.net | GitHub releases |
   | Vue.js | vuejs.org | GitHub changelog |
   | Nuxt | nuxt.com | GitHub releases |
   | Pinia | pinia.vuejs.org | GitHub releases |
   | TypeScript | typescriptlang.org | GitHub releases |
   | Tailwind | tailwindcss.com | GitHub releases |
   | PostgreSQL | postgresql.org | Release notes |
   | Elasticsearch | elastic.co/docs | Release notes |

5. RESEARCH QUERY TEMPLATES
   # For version migration:
   fetch_webpage("[package] v[old] to v[new] migration guide")
   
   # For breaking changes:
   fetch_webpage("[package] [version] breaking changes")
   
   # For new features:
   fetch_webpage("[package] [version] new features changelog")
   
   # For error resolution:
   fetch_webpage("[exact error message] [package] [version]")
```

#### KB-DIAG-008: Dependency Update Research Pattern
```
When Package Update Needed:

1. PRE-UPDATE RESEARCH
   ├─ Check current version in project
   ├─ Identify target version (latest stable)
   ├─ Research breaking changes between versions
   └─ Review GitHub issues for known problems

2. RESEARCH SOURCES (Priority Order)
   a) Official Migration Guide
      fetch_webpage("[package] migration guide v[X] to v[Y]")
   
   b) Release Notes / Changelog
      fetch_webpage("[package] [version] release notes")
      
   c) GitHub Issues (known problems)
      fetch_webpage("site:github.com [package] [version] issue")
   
   d) Community Resources (Stack Overflow, blogs)
      fetch_webpage("[package] [version] [specific issue]")

3. VALIDATION CHECKLIST
   □ Breaking changes documented?
   □ Migration steps identified?
   □ Dependencies compatible?
   □ Tests updated for new behavior?
   □ KB article updated with findings?

4. POST-RESEARCH ACTIONS
   ├─ Update .ai/knowledgebase/dependency-updates/[package].md
   ├─ Add entry to lessons.md if significant learnings
   ├─ Update Directory.Packages.props with notes
   └─ Create ADR if architectural impact
```

#### KB-DIAG-009: Breaking Changes Detection & Documentation
```
Breaking Changes Workflow:

1. DETECTION SOURCES
   ├─ Build/Compile Errors After Update
   │   ├─ CS0619: 'X' is obsolete (with error)
   │   ├─ CS0117: 'X' does not contain definition for 'Y'
   │   ├─ CS1061: 'X' does not contain method 'Y'
   │   └─ TS2339: Property 'X' does not exist on type 'Y'
   │
   ├─ Runtime Exceptions After Update
   │   ├─ MissingMethodException
   │   ├─ TypeLoadException
   │   └─ NotSupportedException (behavior changed)
   │
   ├─ Package Metadata
   │   ├─ NuGet: Check <PackageReleaseNotes> in .nuspec
   │   ├─ npm: Check CHANGELOG.md, BREAKING_CHANGES.md
   │   └─ GitHub: Check releases with "breaking" label
   │
   └─ Documentation Markers
       ├─ "⚠️ Breaking Change" in release notes
       ├─ "Migration Required" sections
       └─ Major version bump (semver: X.0.0)

2. BREAKING CHANGE CATEGORIES
   ├─ API Surface Changes
   │   ├─ Removed: Method/class/property deleted
   │   ├─ Renamed: Identifier changed
   │   ├─ Signature: Parameters added/removed/reordered
   │   └─ Return Type: Changed return type
   │
   ├─ Behavioral Changes
   │   ├─ Default values changed
   │   ├─ Exception types changed
   │   ├─ Null handling changed
   │   └─ Threading model changed
   │
   ├─ Configuration Changes
   │   ├─ Config keys renamed/removed
   │   ├─ Environment variable changes
   │   └─ DI registration changes
   │
   └─ Dependency Changes
       ├─ Transitive dependency version bumped
       ├─ New required dependency
       └─ Framework requirement changed (TFM)

3. DOCUMENTATION TEMPLATE
   Create/Update: .ai/knowledgebase/dependency-updates/[Package].md
   
   ```markdown
   ## [Package] v[OLD] → v[NEW] Breaking Changes
   
   **Updated**: [DATE]
   **Severity**: Critical | High | Medium | Low
   **Migration Effort**: Hours | Days | Weeks
   
   ### Breaking Changes
   
   #### 1. [Change Name]
   - **Type**: API Removal | Behavioral | Configuration
   - **Old Behavior**: [description]
   - **New Behavior**: [description]
   - **Migration**:
     ```csharp
     // Before
     oldMethod();
     
     // After
     newMethod(requiredParam);
     ```
   - **Affected Files**: [list or pattern]
   
   ### Deprecation Warnings (Future Breaking)
   - `OldMethod()` → Use `NewMethod()` (removal in v[X])
   
   ### New Features (Optional)
   - [Feature] - [brief description]
   ```

4. PROACTIVE DETECTION COMMANDS
   # NuGet - Check for deprecation warnings
   dotnet build -warnaserror:CS0618,CS0612
   
   # List packages with newer versions
   dotnet list package --outdated --include-prerelease
   
   # npm - Check for deprecation
   npm outdated
   npm audit
   
   # Check release notes programmatically
   gh release view [tag] --repo [owner/repo]

5. KB INTEGRATION
   ├─ .ai/knowledgebase/dependency-updates/
   │   ├─ INDEX.md (all tracked packages)
   │   ├─ Microsoft.Extensions.AI.md
   │   ├─ Wolverine.md
   │   ├─ Vue.md
   │   ├─ Nuxt.md
   │   └─ [other packages]
   │
   └─ Cross-reference in:
       ├─ lessons.md (if caused issues)
       ├─ patterns-antipatterns.md (if pattern change)
       └─ ADRs (if architectural decision)
```

#### KB-DIAG-010: Semver & Breaking Change Prediction
```
Semantic Versioning Signals:

MAJOR (X.0.0) - Breaking Changes Expected
├─ Always research before updating
├─ Expect API surface changes
├─ Review full migration guide
└─ Plan dedicated migration sprint

MINOR (x.Y.0) - New Features, No Breaking
├─ Generally safe to update
├─ Review new features for adoption
├─ Watch for deprecation warnings
└─ May have behavioral edge cases

PATCH (x.y.Z) - Bug Fixes Only
├─ Usually safe to update immediately
├─ Review for security fixes
├─ Rare: May have "fix" that breaks workarounds
└─ Check if you depend on "buggy" behavior

PRE-RELEASE (-alpha, -beta, -rc, -preview)
├─ Expect breaking changes between previews
├─ Not recommended for production
├─ API may change without notice
└─ Document workarounds needed

Version Comparison Commands:
# NuGet - Compare versions
dotnet package search [name] --take 5

# npm - View all versions
npm view [package] versions

# Check if update is major/minor/patch
# Current: 2.3.4, Available: 3.0.0 → MAJOR (breaking likely)
# Current: 2.3.4, Available: 2.4.0 → MINOR (features)
# Current: 2.3.4, Available: 2.3.5 → PATCH (fixes)
```

#### KB-DIAG-011: Breaking Change Impact Analysis
```
Impact Analysis Workflow:

1. IDENTIFY AFFECTED CODE
   # Find all usages of changed API
   grep -r "OldMethodName" --include="*.cs" src/
   grep -r "deprecatedProperty" --include="*.ts" src/
   
   # Use code search tools
   list_code_usages("OldClassName")
   semantic_search("usage of [deprecated feature]")

2. CATEGORIZE IMPACT
   ├─ Direct Usage (must change)
   │   └─ Code directly calls removed/changed API
   ├─ Indirect Usage (may need change)
   │   └─ Code depends on type that changed
   ├─ Test Impact (update tests)
   │   └─ Tests mock/verify changed behavior
   └─ Config Impact (update settings)
       └─ Configuration keys renamed/removed

3. ESTIMATE EFFORT
   | Scope | Files | Effort |
   |-------|-------|--------|
   | <5 files | Simple | 1-2 hours |
   | 5-20 files | Medium | 1-2 days |
   | 20-50 files | Large | 3-5 days |
   | >50 files | Major | 1-2 weeks |

4. MIGRATION STRATEGY
   ├─ Simple (< 5 files)
   │   └─ Direct replacement in single PR
   ├─ Medium (5-20 files)
   │   └─ Grouped PRs by domain/layer
   ├─ Large (20-50 files)
   │   ├─ Adapter/shim pattern
   │   ├─ Gradual migration
   │   └─ Feature flag rollout
   └─ Major (> 50 files)
       ├─ Dedicated migration branch
       ├─ Strangler fig pattern
       └─ Parallel implementations

5. ROLLBACK PLAN
   ├─ Pin to old version in Directory.Packages.props
   ├─ Document exact rollback commit
   └─ Test rollback procedure before migration
```

---

### 2. Error Pattern Library (KB-ERR-*)

#### Template Structure
```markdown
## Error: [ERROR_NAME]

**Pattern ID**: KB-ERR-XXX
**Severity**: Critical | High | Medium | Low
**Frequency**: Daily | Weekly | Monthly | Rare

### Recognition
- **Error Message**: `exact error text or regex pattern`
- **Context**: When this typically occurs
- **Symptoms**: Observable behaviors

### Root Causes
1. [Most common cause] - 70%
2. [Second cause] - 20%
3. [Edge case] - 10%

### Diagnostic Steps
1. Check [specific thing]
2. Verify [specific condition]
3. Run [diagnostic command]

### Solutions
**For Cause 1**:
```code
// Solution code
```

**For Cause 2**:
```code
// Alternative solution
```

### Prevention
- [ ] Add to CI/CD checks
- [ ] Add to code review checklist
- [ ] Update related tests
```

#### Proposed Error Patterns

| ID | Error Pattern | Priority |
|----|---------------|----------|
| KB-ERR-001 | Assembly Loading Failures (.NET 10 + Preview Packages) | Critical |
| KB-ERR-002 | Circular Dependency Detection | Critical |
| KB-ERR-003 | Project Build Order / Resolution Failures | Critical |
| KB-ERR-004 | Outdated KB Information / Stale Knowledge | Critical |
| KB-ERR-005 | Breaking Changes in Dependencies (Undocumented/Missed) | Critical |
| KB-ERR-006 | Async Deadlock Patterns | High |
| KB-ERR-007 | Vue Hydration Mismatches | High |
| KB-ERR-008 | TypeScript Strict Mode Violations | Medium |
| KB-ERR-009 | i18n Key Missing Cascade | Medium |
| KB-ERR-010 | Database Migration Conflicts | Medium |
| KB-ERR-011 | CQRS Handler Registration Missing | Medium |
| KB-ERR-012 | Docker Build Cache Invalidation | Low |
| KB-ERR-013 | Git Merge Conflict in Generated Files | Low |

---

### 3. Refactoring Recipes (KB-REFACTOR-*)

#### Template Structure
```markdown
## Refactoring: [TRANSFORMATION_NAME]

**Recipe ID**: KB-REFACTOR-XXX
**Complexity**: Simple | Medium | Complex
**Risk Level**: Low | Medium | High
**Estimated Time**: X hours/days

### When to Apply
- [Trigger condition 1]
- [Trigger condition 2]

### Prerequisites
- [ ] All tests passing
- [ ] No uncommitted changes
- [ ] Dependencies analyzed

### Step-by-Step Recipe

#### Step 1: Analysis
```bash
# MCP commands for analysis
```

#### Step 2: Preparation
```code
// Preparation code
```

#### Step 3: Transformation
```code
// Before → After examples
```

#### Step 4: Validation
```bash
# Validation commands
```

### Rollback Plan
1. [How to revert]
2. [What to check after revert]

### Success Criteria
- [ ] Tests still passing
- [ ] No breaking changes
- [ ] Performance not degraded
```

#### Proposed Refactoring Recipes

| ID | Recipe | Complexity | Priority |
|----|--------|------------|----------|
| KB-REFACTOR-001 | Extract Service from Controller | Medium | High |
| KB-REFACTOR-002 | Convert Sync to Async | Medium | High |
| KB-REFACTOR-003 | Replace Inheritance with Composition | Complex | High |
| KB-REFACTOR-004 | Introduce Repository Pattern | Medium | Medium |
| KB-REFACTOR-005 | Extract Vue Composable | Simple | High |
| KB-REFACTOR-006 | Convert Options API to Composition API | Medium | Medium |
| KB-REFACTOR-007 | Normalize Database Schema | Complex | Medium |
| KB-REFACTOR-008 | Split Monolithic Component (<500 LOC rule) | Medium | High |
| KB-REFACTOR-009 | Introduce CQRS to Existing Endpoint | Complex | Medium |
| KB-REFACTOR-010 | Migrate Any Types to Strict TypeScript | Simple | High |

---

### Detailed Refactoring Recipes

#### KB-REFACTOR-001: Extract Service from Controller
```
Complexity: Medium | Risk: Low | Time: 2-4 hours

WHEN TO APPLY:
- Controller has >100 lines of business logic
- Same logic duplicated across controllers
- Unit testing controller is difficult

PREREQUISITES:
□ All tests passing
□ Identify all methods to extract
□ Define service interface

STEP-BY-STEP:

1. CREATE INTERFACE
   ```csharp
   // Domain/Interfaces/IProductService.cs
   public interface IProductService
   {
       Task<Product> GetByIdAsync(Guid id);
       Task<IEnumerable<Product>> SearchAsync(SearchCriteria criteria);
   }
   ```

2. CREATE SERVICE IMPLEMENTATION
   ```csharp
   // Domain/Services/ProductService.cs
   public class ProductService : IProductService
   {
       private readonly IProductRepository _repository;
       
       public ProductService(IProductRepository repository)
       {
           _repository = repository;
       }
       
       // Move business logic here
   }
   ```

3. REGISTER IN DI
   ```csharp
   // Program.cs or ServiceRegistration.cs
   services.AddScoped<IProductService, ProductService>();
   ```

4. UPDATE CONTROLLER
   ```csharp
   // Before
   public class ProductController
   {
       private readonly IProductRepository _repository;
       
       public async Task<IActionResult> Get(Guid id)
       {
           // 50 lines of business logic
       }
   }
   
   // After
   public class ProductController
   {
       private readonly IProductService _productService;
       
       public async Task<IActionResult> Get(Guid id)
       {
           var product = await _productService.GetByIdAsync(id);
           return Ok(product);
       }
   }
   ```

5. UPDATE TESTS
   - Create unit tests for service
   - Update controller tests to mock service

VALIDATION:
□ All existing tests pass
□ New service tests added
□ Controller tests simplified
□ No duplicate business logic
```

#### KB-REFACTOR-005: Extract Vue Composable
```
Complexity: Simple | Risk: Low | Time: 1-2 hours

WHEN TO APPLY:
- Same reactive logic in multiple components
- Component setup() >50 lines
- Stateful logic that's reusable

PREREQUISITES:
□ Identify shared logic pattern
□ Determine composable inputs/outputs
□ Check if existing composable fits

STEP-BY-STEP:

1. IDENTIFY EXTRACTABLE LOGIC
   ```vue
   <!-- Before: ProductList.vue -->
   <script setup lang="ts">
   const products = ref<Product[]>([])
   const loading = ref(false)
   const error = ref<string | null>(null)
   
   async function fetchProducts() {
     loading.value = true
     try {
       products.value = await api.getProducts()
     } catch (e) {
       error.value = e.message
     } finally {
       loading.value = false
     }
   }
   
   onMounted(fetchProducts)
   </script>
   ```

2. CREATE COMPOSABLE
   ```typescript
   // composables/useAsyncData.ts
   export function useAsyncData<T>(
     fetcher: () => Promise<T>,
     options?: { immediate?: boolean }
   ) {
     const data = ref<T | null>(null)
     const loading = ref(false)
     const error = ref<string | null>(null)
     
     async function execute() {
       loading.value = true
       error.value = null
       try {
         data.value = await fetcher()
       } catch (e) {
         error.value = e instanceof Error ? e.message : 'Unknown error'
       } finally {
         loading.value = false
       }
     }
     
     if (options?.immediate !== false) {
       onMounted(execute)
     }
     
     return { data, loading, error, execute }
   }
   ```

3. REFACTOR COMPONENT
   ```vue
   <!-- After: ProductList.vue -->
   <script setup lang="ts">
   const { data: products, loading, error } = useAsyncData(
     () => api.getProducts()
   )
   </script>
   ```

4. ADD TYPES
   ```typescript
   // types/composables.ts
   export interface UseAsyncDataReturn<T> {
     data: Ref<T | null>
     loading: Ref<boolean>
     error: Ref<string | null>
     execute: () => Promise<void>
   }
   ```

VALIDATION:
□ Composable has TypeScript types
□ Original component still works
□ Composable tested independently
□ No hardcoded values in composable
```

#### KB-REFACTOR-008: Split Monolithic Component (<500 LOC rule)
```
Complexity: Medium | Risk: Medium | Time: 2-4 hours

WHEN TO APPLY:
- Component >500 lines of code
- Component has multiple distinct sections
- Difficult to understand or maintain
- Multiple developers working on same file

PREREQUISITES:
□ Identify logical boundaries
□ Map data flow between sections
□ Plan component hierarchy

STEP-BY-STEP:

1. ANALYZE COMPONENT STRUCTURE
   ```
   ProductPage.vue (800 lines)
   ├─ Header section (lines 1-100)
   ├─ Product details (lines 101-300)
   ├─ Reviews section (lines 301-500)
   ├─ Related products (lines 501-650)
   └─ Footer/actions (lines 651-800)
   ```

2. IDENTIFY EXTRACTION CANDIDATES
   - Self-contained UI sections
   - Sections with own state
   - Reusable across pages
   - Sections by different owners

3. EXTRACT CHILD COMPONENTS
   ```vue
   <!-- components/product/ProductHeader.vue -->
   <script setup lang="ts">
   defineProps<{
     product: Product
   }>()
   
   const emit = defineEmits<{
     (e: 'share'): void
     (e: 'favorite'): void
   }>()
   </script>
   ```

4. REFACTOR PARENT
   ```vue
   <!-- pages/ProductPage.vue (now ~150 lines) -->
   <template>
     <div class="product-page">
       <ProductHeader 
         :product="product" 
         @share="handleShare"
         @favorite="handleFavorite"
       />
       <ProductDetails :product="product" />
       <ProductReviews :product-id="product.id" />
       <RelatedProducts :category="product.category" />
       <ProductActions :product="product" @add-to-cart="addToCart" />
     </div>
   </template>
   ```

5. HANDLE STATE MANAGEMENT
   - Props down, events up (simple)
   - Provide/inject (medium complexity)
   - Pinia store (shared state)

VALIDATION:
□ Parent component <300 lines
□ Each child <200 lines
□ Clear prop/event interfaces
□ No prop drilling >2 levels
□ All tests updated
```

#### KB-REFACTOR-010: Migrate Any Types to Strict TypeScript
```
Complexity: Simple | Risk: Low | Time: 1-3 hours per file

WHEN TO APPLY:
- `any` types in TypeScript code
- `eslint-disable` for type errors
- Props without type definitions
- API responses untyped

PREREQUISITES:
□ Understand actual data shape
□ Have sample data for reference
□ Check API documentation

STEP-BY-STEP:

1. FIND ALL `any` USAGES
   ```bash
   # Find explicit any
   grep -r ": any" --include="*.ts" --include="*.vue" src/
   
   # Find implicit any (eslint warnings)
   npm run lint 2>&1 | grep "@typescript-eslint/no-explicit-any"
   ```

2. CATEGORIZE BY TYPE
   ├─ API Responses → Create response interfaces
   ├─ Props → Use defineProps<T>()
   ├─ Event handlers → Type event parameter
   ├─ Utility functions → Add generics
   └─ Third-party → Use existing types or create declarations

3. CREATE INTERFACES
   ```typescript
   // types/api/products.ts
   export interface ProductResponse {
     id: string
     name: string
     price: number
     attributes: ProductAttribute[]
   }
   
   export interface ProductAttribute {
     key: string
     value: string
     unit?: string
   }
   ```

4. REPLACE ANY WITH TYPES
   ```typescript
   // Before
   async function fetchProduct(id: string): Promise<any> {
     const response = await api.get(`/products/${id}`)
     return response.data
   }
   
   // After
   async function fetchProduct(id: string): Promise<ProductResponse> {
     const response = await api.get<ProductResponse>(`/products/${id}`)
     return response.data
   }
   ```

5. HANDLE EDGE CASES
   ```typescript
   // Unknown external data → use unknown + type guard
   function processData(data: unknown): Product {
     if (!isProduct(data)) {
       throw new Error('Invalid product data')
     }
     return data
   }
   
   function isProduct(data: unknown): data is Product {
     return (
       typeof data === 'object' &&
       data !== null &&
       'id' in data &&
       'name' in data
     )
   }
   ```

VALIDATION:
□ No `any` in changed files
□ No new eslint-disable comments
□ Types exported for reuse
□ Tests pass with strict types
```

---

### 4. Recovery Patterns (KB-RECOVER-*)

#### KB-RECOVER-001: Failed Migration Recovery
```
Migration Failed?
├─ Database State
│   ├─ Check __EFMigrationsHistory table
│   ├─ Identify partial migration state
│   └─ Run compensating migration or rollback
├─ Code State
│   ├─ Git stash current changes
│   ├─ Return to known good commit
│   └─ Re-apply changes incrementally
└─ Test State
    ├─ Reset test database
    ├─ Clear caches
    └─ Re-run from clean state
```

#### KB-RECOVER-002: Broken Build Recovery
```
Build Unrecoverable?
├─ Quick Fixes (< 5 min)
│   ├─ dotnet clean + rebuild
│   ├─ Delete bin/obj folders
│   └─ Restore packages
├─ Medium Fixes (5-30 min)
│   ├─ Check recent commits for breaking changes
│   ├─ Bisect to find breaking commit
│   └─ Review merge conflicts
└─ Deep Recovery (> 30 min)
    ├─ Create fresh clone
    ├─ Compare with known good state
    └─ Rebuild incrementally
```

#### KB-RECOVER-003: Corrupted Document Recovery
```
Document Structure Corrupted?
├─ Detection
│   ├─ Read full file (not chunks)
│   ├─ Check section numbering
│   └─ Validate markdown structure
├─ Recovery
│   ├─ Git show previous version
│   ├─ Diff to identify corruption
│   └─ Reconstruct from clean sections
└─ Prevention
    ├─ Limit sequential replace_string_in_file operations
    ├─ Periodic full-file structure reviews
    └─ Use atomic file writes for large changes
```

---

### 5. Debugging Workflow Patterns (KB-DEBUG-*)

#### KB-DEBUG-001: Systematic Backend Debugging
```
┌─────────────────────────────────────────┐
│ 1. REPRODUCE                            │
│    - Get exact steps to reproduce       │
│    - Identify minimal reproduction      │
│    - Document environment details       │
├─────────────────────────────────────────┤
│ 2. ISOLATE                              │
│    - Unit test the component            │
│    - Mock dependencies                  │
│    - Binary search through code paths   │
├─────────────────────────────────────────┤
│ 3. DIAGNOSE                             │
│    - Use logging/tracing                │
│    - Check Aspire dashboard             │
│    - Review recent changes (git blame)  │
├─────────────────────────────────────────┤
│ 4. FIX                                  │
│    - Write failing test first           │
│    - Implement minimal fix              │
│    - Verify test passes                 │
├─────────────────────────────────────────┤
│ 5. PREVENT                              │
│    - Add regression test                │
│    - Update lessons.md                  │
│    - Consider architectural fix         │
└─────────────────────────────────────────┘
```

#### KB-DEBUG-002: Systematic Frontend Debugging
```
┌─────────────────────────────────────────┐
│ 1. BROWSER DEVTOOLS                     │
│    - Console for JS errors              │
│    - Network for API issues             │
│    - Vue Devtools for state             │
├─────────────────────────────────────────┤
│ 2. COMPONENT ISOLATION                  │
│    - Storybook or test harness          │
│    - Minimal props reproduction         │
│    - Check reactivity chains            │
├─────────────────────────────────────────┤
│ 3. STATE DEBUGGING                      │
│    - Pinia devtools inspection          │
│    - Action/mutation logging            │
│    - State snapshot comparison          │
├─────────────────────────────────────────┤
│ 4. TYPE SAFETY CHECK                    │
│    - Run typescript-mcp/analyze_types   │
│    - Check prop/emit type mismatches    │
│    - Verify API response types          │
└─────────────────────────────────────────┘
```

#### KB-DEBUG-003: Test Failure Analysis
```
Test Failed?
├─ CATEGORIZE FAILURE TYPE
│   ├─ Compilation Error
│   │   ├─ Missing type/reference → Check recent changes
│   │   └─ Syntax error → Review test code
│   ├─ Assertion Failure
│   │   ├─ Expected vs Actual mismatch → Check test data or code
│   │   ├─ Null/undefined received → Check async/await, mock setup
│   │   └─ Wrong exception type → Verify exception handling
│   ├─ Timeout
│   │   ├─ Async operation hung → Check await, deadlock
│   │   ├─ External dependency slow → Mock or increase timeout
│   │   └─ Infinite loop → Check recursion, while conditions
│   └─ Flaky (Intermittent)
│       ├─ Race condition → Add proper synchronization
│       ├─ Test order dependency → Make tests isolated
│       └─ Shared state mutation → Reset state between tests
│
├─ DIAGNOSTIC COMMANDS
│   # Run single test with verbose output
│   dotnet test --filter "FullyQualifiedName=Namespace.Class.Method" -v detailed
│   
│   # Run with logging
│   dotnet test --logger "console;verbosity=detailed"
│   
│   # npm/vitest single test
│   npm test -- --run -t "test name"
│   
│   # Check test in isolation
│   dotnet test --filter "Category=Unit" -- RunConfiguration.DisableParallelization=true
│
├─ ROOT CAUSE ANALYSIS
│   1. Is this a test bug or code bug?
│   2. Did it pass before? (git bisect)
│   3. Does it fail consistently or flaky?
│   4. Does it fail in CI but pass locally?
│   5. What changed recently? (git log --oneline -10)
│
├─ COMMON FIXES BY CATEGORY
│   ├─ Mock Not Working
│   │   └─ Verify mock setup, check DI registration
│   ├─ Async Test Failing
│   │   └─ Add await, use async test pattern, check cancellation
│   ├─ Database Test Failing
│   │   └─ Reset database, check transaction rollback
│   ├─ Time-Dependent Test
│   │   └─ Mock DateTime/clock, use deterministic time
│   └─ Environment-Dependent
│       └─ Check appsettings.Test.json, environment variables
│
└─ POST-FIX ACTIONS
    ├─ Verify test actually tests what it should
    ├─ Add to lessons.md if new pattern
    └─ Consider adding similar test coverage
```

#### KB-DEBUG-004: CI/CD Pipeline Failure Analysis
```
Pipeline Failed?
├─ BUILD STAGE FAILURE
│   ├─ Compilation Error
│   │   ├─ Works locally? → Check SDK/framework version in CI
│   │   ├─ Missing dependency? → Check package restore
│   │   └─ Recent merge? → Check for conflicts
│   ├─ Restore Failed
│   │   ├─ NuGet feed down → Check feed status, use cache
│   │   ├─ npm registry error → Check registry, try mirror
│   │   └─ Auth error → Check CI secrets, feed permissions
│   └─ Docker Build Failed
│       ├─ Base image unavailable → Pin image tags
│       ├─ COPY failed → Check .dockerignore, paths
│       └─ Build arg missing → Check CI variables
│
├─ TEST STAGE FAILURE
│   ├─ Unit Tests
│   │   ├─ All fail → Environment issue (SDK, config)
│   │   ├─ Some fail → Recent code change broke them
│   │   └─ Flaky → Race condition, shared state
│   ├─ Integration Tests
│   │   ├─ Service unavailable → Check test containers
│   │   ├─ Database error → Check migrations, seed data
│   │   └─ Timeout → Increase timeout, check test DB
│   └─ E2E Tests
│       ├─ Browser launch failed → Check Playwright install
│       ├─ Element not found → UI changed, update selectors
│       └─ Network error → Check test server running
│
├─ DEPLOY STAGE FAILURE
│   ├─ Auth/Permission
│   │   └─ Check service principal, secrets expiry
│   ├─ Resource Unavailable
│   │   └─ Check target environment health
│   └─ Configuration Missing
│       └─ Check environment variables, Key Vault
│
├─ DIAGNOSTIC STEPS
│   1. Read full error message (not just summary)
│   2. Check which step failed in pipeline
│   3. Compare with last successful run
│   4. Check recent commits since last success
│   5. Try to reproduce locally
│   
│   # GitHub Actions - get logs
│   gh run view [run-id] --log
│   
│   # Azure DevOps - get logs
│   az pipelines runs show --id [run-id]
│
└─ COMMON CI-SPECIFIC ISSUES
    ├─ Works locally, fails in CI
    │   ├─ Different SDK version → Pin in global.json
    │   ├─ Missing env vars → Add to CI secrets
    │   ├─ File path case → Linux is case-sensitive
    │   └─ Timezone/locale → Use UTC, invariant culture
    ├─ Flaky in CI only
    │   ├─ Resource contention → Reduce parallelism
    │   ├─ Network instability → Add retries
    │   └─ Time-based tests → Mock time providers
    └─ Cache Issues
        ├─ Stale cache → Clear/invalidate cache
        └─ Cache miss → Check cache key generation
```

#### KB-DEBUG-005: Performance Issue Diagnosis
```
Performance Problem?
├─ IDENTIFY SYMPTOMS
│   ├─ Slow Response Time (API)
│   │   ├─ Check database queries (N+1, missing index)
│   │   ├─ Check external service calls
│   │   └─ Check serialization overhead
│   ├─ Slow Page Load (Frontend)
│   │   ├─ Bundle size too large → Code splitting
│   │   ├─ Too many requests → Combine, cache
│   │   └─ Render blocking → Lazy load, defer
│   ├─ Memory Growth
│   │   ├─ Memory leak → Check event handlers, closures
│   │   ├─ Large objects → Stream instead of buffer
│   │   └─ Cache unbounded → Add eviction policy
│   └─ High CPU
│       ├─ Expensive computation → Cache results
│       ├─ Busy loop → Add delays, use events
│       └─ Regex catastrophe → Simplify patterns
│
├─ DIAGNOSTIC TOOLS
│   # .NET - Profile with dotnet-trace
│   dotnet trace collect -p [pid] --duration 00:00:30
│   
│   # .NET - Memory dump
│   dotnet dump collect -p [pid]
│   
│   # Database - Query analysis
│   EXPLAIN ANALYZE SELECT ...
│   
│   # Frontend - Lighthouse
│   npx lighthouse http://localhost:3000 --view
│   
│   # Frontend - Bundle analysis
│   npm run build -- --analyze
│
├─ COMMON FIXES
│   ├─ Database
│   │   ├─ Add missing indexes
│   │   ├─ Fix N+1 with Include/eager loading
│   │   ├─ Add pagination
│   │   └─ Use read replicas
│   ├─ API
│   │   ├─ Add response caching
│   │   ├─ Use async I/O
│   │   ├─ Implement pagination
│   │   └─ Use DTOs (don't return full entities)
│   └─ Frontend
│       ├─ Lazy load routes/components
│       ├─ Virtual scrolling for lists
│       ├─ Debounce user input
│       └─ Optimize images (WebP, lazy load)
│
└─ PREVENTION
    ├─ Add performance tests to CI
    ├─ Set response time budgets
    ├─ Monitor with APM tools
    └─ Regular performance audits
```

#### KB-DEBUG-006: Security Vulnerability Diagnosis
```
Security Issue Detected?
├─ VULNERABILITY SOURCES
│   ├─ Dependency Scanner (Dependabot, Snyk)
│   │   ├─ CVE reported → Check severity, exploitability
│   │   ├─ Outdated package → Research update path
│   │   └─ Transitive dependency → Find root package
│   ├─ Code Scanner (CodeQL, SonarQube)
│   │   ├─ SQL Injection → Parameterize queries
│   │   ├─ XSS → Encode output, CSP headers
│   │   ├─ Hardcoded secrets → Move to config/vault
│   │   └─ Insecure deserialization → Use safe deserializers
│   └─ Penetration Test / Bug Bounty
│       ├─ Auth bypass → Review auth middleware
│       ├─ IDOR → Add authorization checks
│       └─ Data exposure → Review API responses
│
├─ SEVERITY ASSESSMENT
│   ├─ CRITICAL (Fix immediately)
│   │   ├─ RCE (Remote Code Execution)
│   │   ├─ Auth bypass
│   │   └─ Data breach potential
│   ├─ HIGH (Fix within 24-48h)
│   │   ├─ SQL Injection
│   │   ├─ XSS (stored)
│   │   └─ Privilege escalation
│   ├─ MEDIUM (Fix within 1 week)
│   │   ├─ XSS (reflected)
│   │   ├─ CSRF
│   │   └─ Information disclosure
│   └─ LOW (Fix in next sprint)
│       ├─ Missing headers
│       ├─ Verbose errors
│       └─ Outdated non-vulnerable deps
│
├─ DIAGNOSTIC COMMANDS
│   # Check for known vulnerabilities
│   dotnet list package --vulnerable
│   npm audit
│   
│   # Scan for secrets
│   git secrets --scan
│   gitleaks detect
│   
│   # Security headers check
│   curl -I https://yoursite.com | grep -i "security\|content-security"
│
├─ FIX PATTERNS
│   ├─ SQL Injection
│   │   ```csharp
│   │   // BAD
│   │   $"SELECT * FROM Users WHERE Id = {userId}"
│   │   // GOOD
│   │   "SELECT * FROM Users WHERE Id = @Id", new { Id = userId }
│   │   ```
│   ├─ XSS Prevention
│   │   ```typescript
│   │   // BAD
│   │   element.innerHTML = userInput
│   │   // GOOD
│   │   element.textContent = userInput
│   │   // Or use Vue's {{ }} which auto-escapes
│   │   ```
│   ├─ Secret Management
│   │   ```csharp
│   │   // BAD
│   │   var apiKey = "sk-12345..."
│   │   // GOOD
│   │   var apiKey = configuration["ApiKey"]
│   │   // Or use Azure Key Vault, AWS Secrets Manager
│   │   ```
│   └─ Auth Check
│       ```csharp
│       // Ensure every endpoint has authorization
│       [Authorize(Policy = "RequireAdmin")]
│       public async Task<IActionResult> DeleteUser(Guid id)
│       ```
│
└─ POST-FIX ACTIONS
    ├─ Verify fix with security scan
    ├─ Add regression test
    ├─ Update security documentation
    ├─ Consider similar patterns elsewhere
    └─ Add to lessons.md
```

#### KB-DEBUG-007: Migration Failure Diagnosis
```
Migration Failed?
├─ DATABASE MIGRATION
│   ├─ Schema Conflict
│   │   ├─ Column already exists → Check migration history
│   │   ├─ Foreign key violation → Order migrations correctly
│   │   └─ Data loss warning → Review migration, add data preservation
│   ├─ Timeout
│   │   ├─ Large table alteration → Use batched approach
│   │   ├─ Index creation on large table → Create concurrently
│   │   └─ Lock contention → Run during low traffic
│   ├─ Rollback Needed
│   │   ├─ EF Core: dotnet ef migrations remove
│   │   ├─ Manual: Apply down migration script
│   │   └─ Data restore: Restore from backup
│   │
│   └─ DIAGNOSTIC COMMANDS
│       # Check pending migrations
│       dotnet ef migrations list
│       
│       # Generate SQL without applying
│       dotnet ef migrations script --idempotent
│       
│       # Check current DB state
│       SELECT * FROM "__EFMigrationsHistory"
│
├─ CODE MIGRATION (Breaking Changes)
│   ├─ API Signature Changed
│   │   ├─ Compile errors → Follow migration guide
│   │   ├─ Runtime errors → Check behavioral changes
│   │   └─ Test failures → Update test expectations
│   ├─ Configuration Changed
│   │   ├─ Keys renamed → Update appsettings.json
│   │   ├─ Format changed → Transform config
│   │   └─ New required keys → Add with defaults
│   ├─ Dependency Conflict
│   │   ├─ Version mismatch → Align in Directory.Packages.props
│   │   ├─ Removed dependency → Find replacement
│   │   └─ Transitive conflict → Use explicit version
│   │
│   └─ ROLLBACK STRATEGY
│       1. Git revert migration commit
│       2. Restore previous package versions
│       3. Restore database from backup (if needed)
│       4. Verify rollback with tests
│
├─ FRAMEWORK MIGRATION (e.g., .NET 8 → .NET 10)
│   ├─ TFM Update
│   │   ├─ Update global.json
│   │   ├─ Update TargetFramework in .csproj
│   │   └─ Update Docker base images
│   ├─ API Deprecations
│   │   ├─ Use [Obsolete] warnings as guide
│   │   ├─ Check breaking changes doc
│   │   └─ Update to recommended alternatives
│   ├─ Package Compatibility
│   │   ├─ Not all packages support new TFM
│   │   ├─ Check package release notes
│   │   └─ May need AssetTargetFallback
│   │
│   └─ VALIDATION
│       □ All projects build
│       □ All tests pass
│       □ App starts and runs
│       □ Key user journeys work
│       □ Performance not degraded
│
└─ FRONTEND MIGRATION (e.g., Vue 2 → Vue 3, Nuxt 2 → Nuxt 3)
    ├─ Breaking API Changes
    │   ├─ Options API → Composition API (optional)
    │   ├─ Filters removed → Use computed/methods
    │   ├─ Event bus removed → Use mitt or Pinia
    │   └─ Vuex → Pinia
    ├─ Build Tool Changes
    │   ├─ Webpack → Vite
    │   ├─ Config format changed
    │   └─ Plugin ecosystem different
    │
    └─ MIGRATION APPROACH
        1. Run official migration tool/codemod
        2. Fix compilation errors
        3. Fix runtime errors
        4. Update tests
        5. Visual regression testing
```

---

### 7. Code Smell Detection Patterns (KB-SMELL-*)

#### KB-SMELL-001: Code Smell Identification
```
Common Code Smells & Remedies:

├─ BLOATERS (Too Big)
│   ├─ Long Method (>30 lines)
│   │   └─ Extract Method, Compose Method
│   ├─ Large Class (>500 lines)
│   │   └─ Extract Class, Extract Interface
│   ├─ Long Parameter List (>4 params)
│   │   └─ Introduce Parameter Object, Builder Pattern
│   ├─ Data Clumps (same params everywhere)
│   │   └─ Extract Class, Introduce Parameter Object
│   └─ Primitive Obsession
│       └─ Replace Primitive with Object (Value Objects)
│
├─ OBJECT-ORIENTATION ABUSERS
│   ├─ Switch Statements (type checking)
│   │   └─ Replace with Polymorphism, Strategy Pattern
│   ├─ Parallel Inheritance
│   │   └─ Collapse Hierarchy, Use Composition
│   ├─ Refused Bequest (unused inherited methods)
│   │   └─ Replace Inheritance with Delegation
│   └─ Alternative Classes with Different Interfaces
│       └─ Rename Methods, Extract Superclass
│
├─ CHANGE PREVENTERS
│   ├─ Divergent Change (one class, many reasons to change)
│   │   └─ Extract Class (Single Responsibility)
│   ├─ Shotgun Surgery (one change, many classes)
│   │   └─ Move Method, Inline Class
│   └─ Feature Envy (method uses other class more)
│       └─ Move Method to appropriate class
│
├─ DISPENSABLES
│   ├─ Comments (explaining bad code)
│   │   └─ Refactor code to be self-documenting
│   ├─ Duplicate Code
│   │   └─ Extract Method, Pull Up Method
│   ├─ Dead Code
│   │   └─ Delete it (version control has history)
│   ├─ Lazy Class (does too little)
│   │   └─ Inline Class, Collapse Hierarchy
│   └─ Speculative Generality (unused abstractions)
│       └─ Remove unused, YAGNI principle
│
└─ COUPLERS
    ├─ Inappropriate Intimacy (classes too coupled)
    │   └─ Move Method, Extract Class, Hide Delegate
    ├─ Message Chains (a.b().c().d())
    │   └─ Hide Delegate, Extract Method
    ├─ Middle Man (class only delegates)
    │   └─ Remove Middle Man, Inline Method
    └─ Incomplete Library Class
        └─ Introduce Foreign Method, Extension Methods
```

#### KB-SMELL-002: Detection Commands & Tools
```
Automated Smell Detection:

# .NET Code Analysis
dotnet build /p:TreatWarningsAsErrors=true
dotnet format --verify-no-changes

# Roslyn Analyzers (in .csproj)
<PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="9.0.0" />
<PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />

# Cyclomatic Complexity
# Install dotnet-counters or use SonarQube

# TypeScript/JavaScript
npm run lint -- --max-warnings 0
npx eslint --rule 'complexity: ["error", 10]'

# Duplication Detection
# SonarQube, jscpd, Simian

# Architecture Tests (ArchUnitNET)
# Enforce layer dependencies, naming conventions
```

#### KB-SMELL-003: Refactoring Prioritization
```
Prioritize Refactoring By:

1. PAIN FREQUENCY
   How often does this code cause problems?
   ├─ Daily → High priority
   ├─ Weekly → Medium priority
   └─ Monthly → Low priority

2. CHANGE FREQUENCY
   How often is this code modified?
   ├─ Every sprint → High priority
   ├─ Occasionally → Medium priority
   └─ Rarely → Low priority (leave it)

3. BUG DENSITY
   How many bugs originated here?
   ├─ Multiple bugs → High priority
   ├─ One bug → Medium priority
   └─ No bugs → Low priority

4. BUSINESS CRITICALITY
   How important is this feature?
   ├─ Core business logic → High priority
   ├─ Supporting feature → Medium priority
   └─ Legacy/unused → Consider removal

REFACTORING DECISION MATRIX:
┌─────────────────┬────────────┬─────────────┬─────────────┐
│                 │ Low Change │ Med Change  │ High Change │
├─────────────────┼────────────┼─────────────┼─────────────┤
│ Low Pain        │ Ignore     │ Opportunist │ Plan        │
│ Medium Pain     │ Opportunist│ Plan        │ Prioritize  │
│ High Pain       │ Plan       │ Prioritize  │ URGENT      │
└─────────────────┴────────────┴─────────────┴─────────────┘

Opportunistic = Refactor when touching for other reasons
Plan = Add to backlog, schedule in future sprint
Prioritize = Schedule in next sprint
URGENT = Stop and fix now
```

---

### 8. Multi-Agent Coordination Patterns (KB-COORD-*)

#### KB-COORD-001: Parallel Work Coordination
```
Multiple Agents Working Simultaneously:

├─ BEFORE STARTING
│   ├─ Define clear boundaries
│   │   ├─ @Backend: src/backend/**
│   │   ├─ @Frontend: src/frontend/**
│   │   └─ Shared: Coordinate via @SARAH
│   ├─ Identify shared dependencies
│   │   ├─ API contracts (OpenAPI spec)
│   │   ├─ Shared types/DTOs
│   │   └─ Database schema
│   └─ Establish communication protocol
│       ├─ Breaking changes → Announce immediately
│       ├─ Interface changes → Create issue first
│       └─ Merge conflicts → Coordinate timing
│
├─ DURING WORK
│   ├─ Avoid editing same files
│   ├─ Use feature branches
│   ├─ Commit frequently, push often
│   ├─ Run tests before push
│   └─ Update shared contracts first
│
├─ CONFLICT RESOLUTION
│   ├─ Git merge conflict
│   │   ├─ Both made same change → Keep one, delete other
│   │   ├─ Different changes → Integrate both
│   │   └─ Incompatible changes → Discuss, decide
│   ├─ API contract conflict
│   │   └─ Backend wins for API shape (owns contract)
│   └─ Logical conflict
│       └─ Escalate to @Architect or @TechLead
│
└─ SYNCHRONIZATION POINTS
    ├─ Start of day: Align on priorities
    ├─ Before merge: Cross-check integration
    └─ End of work: Update progress, document blockers
```

#### KB-COORD-002: Sequential Handoff Pattern
```
Agent A → Agent B Handoff:

1. COMPLETION CHECKLIST (Agent A)
   □ Code compiles without errors
   □ Tests pass
   □ Changes committed with clear message
   □ No uncommitted files
   □ Dependencies documented

2. HANDOFF DOCUMENT
   ```markdown
   ## Handoff: @Backend → @Frontend
   
   ### Completed
   - API endpoint: POST /api/v1/orders
   - Request/Response types in B2X.Contracts
   - Unit tests: 95% coverage
   
   ### Ready for Frontend
   - OpenAPI spec updated: /docs/api/orders.yaml
   - Example request in /test-data/orders.json
   
   ### Known Issues
   - Validation error messages not localized yet
   - Rate limiting not implemented
   
   ### Next Steps for @Frontend
   1. Create order form component
   2. Implement API client using generated types
   3. Add error handling for validation errors
   ```

3. VERIFICATION (Agent B)
   □ Can access/build completed work
   □ Dependencies available
   □ Documentation clear
   □ Questions answered before starting
```

#### KB-COORD-003: Blocking Issue Escalation
```
When Progress is Blocked:

1. IDENTIFY BLOCKER TYPE
   ├─ Technical (code doesn't work)
   │   └─ Try self-resolution first (30 min max)
   ├─ Dependency (waiting for other work)
   │   └─ Escalate to dependent agent
   ├─ Decision (need architectural guidance)
   │   └─ Escalate to @Architect or @TechLead
   ├─ Permission (need access/approval)
   │   └─ Escalate to @SARAH
   └─ External (third-party service, etc.)
       └─ Document, find workaround

2. ESCALATION FORMAT
   ```markdown
   ## Blocker: [Brief Description]
   
   **Blocked Agent**: @Backend
   **Blocking On**: @Frontend / @Architect / External
   **Since**: [Date/Time]
   **Impact**: Cannot complete [Task/Feature]
   
   **Details**:
   [Specific issue description]
   
   **Attempted Solutions**:
   1. [What was tried]
   2. [What was tried]
   
   **Requested Action**:
   [Specific ask]
   
   **Workaround Available**: Yes/No
   [If yes, describe temporary solution]
   ```

3. ESCALATION PATH
   Technical → @TechLead → @Architect
   Process → @ScrumMaster → @SARAH
   Security → @Security
   Legal/Compliance → @Legal
```

---

### 6. Agent Context-Switching Patterns (KB-AGENT-*)

#### KB-AGENT-001: Efficient Handoff Template
```markdown
## Handoff: [FromAgent] → [ToAgent]

### Context Summary (< 500 tokens)
- **Task**: [Brief description]
- **Status**: [Current state]
- **Blockers**: [What's preventing progress]

### Files Involved
- `path/to/file1.cs` - [Purpose]
- `path/to/file2.vue` - [Purpose]

### What Was Done
1. [Completed action 1]
2. [Completed action 2]

### What Needs To Be Done
1. [Pending action 1] - @TargetAgent
2. [Pending action 2] - @TargetAgent

### Critical Context
- [Important constraint]
- [Known issue to avoid]

### MCP Tools Used/Needed
- `roslyn-mcp/analyze_types` ✅ Completed
- `vue-mcp/validate_i18n_keys` ⏳ Pending
```

#### KB-AGENT-002: runSubagent Delegation Patterns
```
Task Analysis
├─ Single Domain?
│   └─ Execute directly (no subagent)
├─ Multi-Domain?
│   ├─ Independent tasks → Parallel subagents
│   └─ Dependent tasks → Sequential subagents
└─ Complex Analysis?
    └─ Isolated subagent (prevent context pollution)

Subagent Selection
├─ Code Analysis → "Analyze [component] for [criteria]"
├─ Validation → "Validate [files] against [standard]"
├─ Research → "Find all usages of [pattern]"
└─ Transformation → "Convert [source] to [target]"
```

---

## 📁 PROPOSED KB STRUCTURE

```
.ai/knowledgebase/
├─ patterns/
│   ├─ FEATURE_IMPLEMENTATION_PATTERNS.md (existing)
│   ├─ VUE3_COMPOSITION_PATTERNS.md (existing)
│   ├─ PROBLEM_DIAGNOSIS_PATTERNS.md (new - KB-DIAG-*)
│   ├─ DEBUGGING_WORKFLOW_PATTERNS.md (new - KB-DEBUG-*)
│   ├─ AGENT_HANDOFF_PATTERNS.md (new - KB-AGENT-*)
│   ├─ CODE_SMELL_PATTERNS.md (new - KB-SMELL-*)
│   └─ COORDINATION_PATTERNS.md (new - KB-COORD-*)
├─ errors/
│   ├─ INDEX.md
│   ├─ dotnet-errors.md (KB-ERR-001 to KB-ERR-005)
│   ├─ frontend-errors.md (KB-ERR-006 to KB-ERR-009)
│   └─ infrastructure-errors.md (KB-ERR-010 to KB-ERR-013)
├─ refactoring/
│   ├─ INDEX.md
│   ├─ backend-recipes.md (KB-REFACTOR-001 to KB-REFACTOR-004)
│   ├─ frontend-recipes.md (KB-REFACTOR-005 to KB-REFACTOR-008)
│   ├─ typescript-recipes.md (KB-REFACTOR-010)
│   └─ database-recipes.md
└─ recovery/
    ├─ INDEX.md
    ├─ build-recovery.md (KB-RECOVER-001 to KB-RECOVER-003)
    ├─ migration-recovery.md
    └─ data-recovery.md
```

---

## 📊 PATTERN SUMMARY

### Diagnosis Patterns (KB-DIAG-*)
| ID | Pattern | Category |
|----|---------|----------|
| KB-DIAG-001 | Build Failure Diagnosis | Build |
| KB-DIAG-002 | Runtime Failure Diagnosis | Runtime |
| KB-DIAG-003 | Frontend Error Diagnosis | Frontend |
| KB-DIAG-004 | Project Resolution Order | Build |
| KB-DIAG-005 | Solution Build Order Analysis | Build |
| KB-DIAG-006 | Outdated Information Detection | Knowledge |
| KB-DIAG-007 | Knowledge Freshness Validation | Knowledge |
| KB-DIAG-008 | Dependency Update Research | Dependencies |
| KB-DIAG-009 | Breaking Changes Detection | Dependencies |
| KB-DIAG-010 | Semver & Breaking Change Prediction | Dependencies |
| KB-DIAG-011 | Breaking Change Impact Analysis | Dependencies |

### Debugging Patterns (KB-DEBUG-*)
| ID | Pattern | Category |
|----|---------|----------|
| KB-DEBUG-001 | Systematic Backend Debugging | Backend |
| KB-DEBUG-002 | Systematic Frontend Debugging | Frontend |
| KB-DEBUG-003 | Test Failure Analysis | Testing |
| KB-DEBUG-004 | CI/CD Pipeline Failure Analysis | DevOps |
| KB-DEBUG-005 | Performance Issue Diagnosis | Performance |
| KB-DEBUG-006 | Security Vulnerability Diagnosis | Security |
| KB-DEBUG-007 | Migration Failure Diagnosis | Migration |

### Code Smell Patterns (KB-SMELL-*)
| ID | Pattern | Category |
|----|---------|----------|
| KB-SMELL-001 | Code Smell Identification | Quality |
| KB-SMELL-002 | Detection Commands & Tools | Automation |
| KB-SMELL-003 | Refactoring Prioritization | Planning |

### Coordination Patterns (KB-COORD-*)
| ID | Pattern | Category |
|----|---------|----------|
| KB-COORD-001 | Parallel Work Coordination | Multi-Agent |
| KB-COORD-002 | Sequential Handoff Pattern | Multi-Agent |
| KB-COORD-003 | Blocking Issue Escalation | Process |

---

## 🔄 INTEGRATION WITH EXISTING SYSTEMS

### Link to Lessons Learned
Each pattern should reference relevant lessons:
```markdown
**Related Lessons**:
- [KB-LESSONS-BACKEND-RED-MONOLITHIC] - Why this pattern exists
- Session 2026-01-10: Aspire Assembly Loading - Original discovery
```

### Link to Agent Instructions
Patterns should be referenced in agent instructions:
```markdown
# backend-essentials.instructions.md
## Error Handling
When encountering build failures, follow [KB-DIAG-001] Build Failure Diagnosis
```

### Link to Quality Gates
Patterns inform PR review:
```markdown
# code-review.prompt.md
## Checklist
- [ ] Refactoring follows [KB-REFACTOR-*] recipes
- [ ] Known error patterns from [KB-ERR-*] avoided
```

---

## 📊 PRIORITIZED IMPLEMENTATION PLAN

### Phase 1: Foundation (Week 1)
- [ ] Create `patterns/PROBLEM_DIAGNOSIS_PATTERNS.md` with KB-DIAG-001 to KB-DIAG-003
- [ ] Create `errors/INDEX.md` with error pattern template
- [ ] Migrate existing lessons to error patterns (KB-ERR-001)

### Phase 2: Core Recipes (Week 2)
- [ ] Create `refactoring/backend-recipes.md` with top 4 recipes
- [ ] Create `refactoring/frontend-recipes.md` with top 4 recipes
- [ ] Create `recovery/build-recovery.md`

### Phase 3: Integration (Week 3)
- [ ] Create `patterns/DEBUGGING_WORKFLOW_PATTERNS.md`
- [ ] Create `patterns/AGENT_HANDOFF_PATTERNS.md`
- [ ] Update agent instructions with pattern references

### Phase 4: Automation (Week 4)
- [ ] Add pattern validation to CI/CD
- [ ] Create pattern search MCP tool
- [ ] Integrate with runSubagent for pattern-based suggestions

---

## 🎯 SUCCESS METRICS

| Metric | Current | Target | How to Measure |
|--------|---------|--------|----------------|
| Problem Resolution Time | ~45 min avg | < 20 min | Track from error to fix |
| Pattern Reuse Rate | ~10% | > 60% | Count pattern references in PRs |
| Agent Handoff Efficiency | ~3 exchanges | < 2 exchanges | Measure context switches |
| Error Recurrence | ~25% | < 10% | Track same-type errors |

---

## ✅ NEXT STEPS

1. **Review**: @Architect, @TechLead review this brainstorm
2. **Prioritize**: Select Phase 1 patterns for immediate implementation
3. **Template**: Finalize pattern template structures
4. **Assign**: Determine ownership for each pattern category
5. **Implement**: Begin Phase 1 documentation

---

**Status**: 📋 Brainstorm Complete - Ready for Review
**Next Action**: @SARAH to coordinate review with @Architect and @TechLead
