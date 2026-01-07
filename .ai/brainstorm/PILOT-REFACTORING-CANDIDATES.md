---
docid: PILOT-REFACTOR-001
title: Candidate Refactorings for Pilot Implementation
owner: "@TechLead, @Architect"
status: Planning
created: "2026-01-07"
---

# 🎯 Pilot Refactoring Candidates

**Purpose**: Identify best candidates for first refactoring using BS-REFACTOR-001 strategy  
**Target**: Medium-sized refactoring (5-20 files, 4-7 days)  
**Goal**: Validate strategy and measure efficiency gains

---

## 📊 Candidate Scoring Matrix

| Criterion | Weight | Notes |
|-----------|--------|-------|
| **Scope Fit** | 25% | 5-20 files? (Medium, not too big/small) |
| **Risk Level** | 20% | Low-risk? (No Breaking Changes = better) |
| **Team Impact** | 20% | Affects blocked work? (unblock team?) |
| **Learning Value** | 20% | Pattern variety? (multiple MCPs?) |
| **Timeline** | 15% | Can finish within week? |

---

## 🏆 TOP CANDIDATES

### 1️⃣ BACKEND: ProductService Handler Consolidation

**Scope**: Backend/Domain/Catalog/Services  
**Current State**: `ProductService.cs` (~400 lines, 3 handlers mixed)

```
ProductService
├── GetProductByIdHandler
├── UpdateProductHandler  
└── SearchProductsHandler (redundant with SearchService!)
```

**Refactoring Goal**: Extract handlers into separate files

**Affected Files**: ~8
```
- ProductService.cs (main)
- Tests/ProductServiceTests.cs
- GetProductByIdHandler.cs (new)
- UpdateProductHandler.cs (new)
- SearchProductsHandler.cs (new)
- Consumers: CatalogController, AdminService, SearchService (~5)
```

**Pattern**: Backend → Handler Extraction (Section A in strategy)

**Risk Level**: 🟢 **LOW**
- No Breaking Changes (internal refactoring)
- Clear, single responsibility
- Existing tests
- No database changes
- No tenant-related logic

**MCP Tools**:
- [ ] Roslyn MCP: `analyze_dependencies`, `extract_interface`
- [ ] Git MCP: `validate_commit_messages`
- [ ] TypeScript MCP: Type checking (if consumer checks needed)

**Estimated Duration**: **4-5 days**
- Day 1: Analysis + PR planning
- Day 2: PR #1 (Extract handlers)
- Day 3: PR #2 (Migrate consumers)
- Day 4: Testing + minor fixes
- Day 5: Code review + merge

**Team Impact**: 🔵 **MEDIUM**
- Unblocks: Better testability for [feature X]?
- Enables: Easier handler reuse

**Learning Value**: 🟢 **HIGH**
- Uses Roslyn MCP (backend automation)
- Clear extraction pattern
- Multi-consumer migration

**✅ RECOMMENDATION**: **Top Choice for Pilot**

---

### 2️⃣ FRONTEND: ProductDetail Component to Composables

**Scope**: Frontend/Store/src/pages  
**Current State**: `ProductDetail.vue` (~600 lines)

```
ProductDetail.vue
├── 12 computed properties
├── 8 methods (mixing UI logic + business logic)
├── 4 watchers
├── Multiple responsibilities:
│   ├── Product loading
│   ├── Review management
│   ├── Recommendations display
│   └── Cart interaction
```

**Refactoring Goal**: Split into composables + smaller components

**Affected Files**: ~6
```
- ProductDetail.vue (main, reduced to ~150 lines)
- ProductDetail.spec.ts (tests)
- composables/useProductDetails.ts (new)
- composables/useProductReviews.ts (new)
- components/ProductSpecifications.vue (new)
- components/ProductRecommendations.vue (new)
```

**Pattern**: Frontend → Monolithic to Composables (Section B in strategy)

**Risk Level**: 🟢 **LOW**
- Behavior unchanged (refactoring only)
- No i18n changes needed (check with i18n validation)
- E2E tests can validate
- No API contract changes

**MCP Tools**:
- [ ] Vue MCP: `validate_i18n_keys`, `extract_component`
- [ ] TypeScript MCP: `analyze_types`
- [ ] Git MCP: `validate_commit_messages`

**Estimated Duration**: **4-5 days**
- Day 1: Analysis + i18n validation
- Day 2-3: PR #1 (Extract composables)
- Day 3-4: PR #2 (Extract components + update main)
- Day 5: Testing + E2E validation

**Team Impact**: 🟢 **HIGH**
- Unblocks: Other team members reusing product logic?
- Enables: Better testing of product features

**Learning Value**: 🟢 **HIGH**
- Uses Vue MCP + TypeScript MCP
- i18n validation pattern
- Composables extraction

**✅ RECOMMENDATION**: **Strong Alternative (if Frontend focus preferred)**

---

### 3️⃣ DATABASE: Product Attributes Schema Consolidation

**Scope**: Backend/Domain/Catalog/Migrations  
**Current State**: Product attributes stored in multiple ways

```
Current Schema:
├── Products table
│   ├── Name (nvarchar)
│   ├── Description (nvarchar) ← OLD
│   ├── LongDescription (nvarchar) ← NEW
│   ├── ShortDescription (nvarchar) ← NEW
│   └── [more old columns needing cleanup]
├── ProductAttributes table (JSON)
└── ProductSpecs (separate table) ← REDUNDANT?
```

**Refactoring Goal**: Consolidate to single schema (remove duplication)

**Affected Files**: ~10
```
- Migration files (add new schema)
- ProductEntity.cs
- ProductRepository.cs
- [5+ Services using Product queries]
- Tests
```

**Pattern**: Database → Backwards-Compatible Migration (Section D in strategy)

**Risk Level**: 🟡 **MEDIUM** (data migration involved)
- Multi-step migration required
- 2-week production soak time needed
- Rollback complexity moderate

**MCP Tools**:
- [ ] Database MCP: `analyze_schema`, `validate_migration`
- [ ] Roslyn MCP: `find_pattern` (find all Product queries)
- [ ] Git MCP: `validate_commit_messages`

**Estimated Duration**: **3-4 weeks** (incl. 2+ week production soak)
- Week 1: Analysis + migration planning (3 days)
- Week 1-2: PR #1 (Add new schema), PR #2 (Migrate data)
- Week 2-3: PR #3-5 (Code migration)
- Week 3-4+: Monitor production (2+ weeks)

**Team Impact**: 🟡 **MEDIUM**
- Enables: Future schema improvements
- Not blocking current work

**Learning Value**: 🟡 **MEDIUM**
- Database migration pattern
- Multi-tenant schema considerations
- Longer timeline (not ideal for first pilot)

**⚠️ RECOMMENDATION**: **Good but later** (longer timeline, consider after 1st pilot success)

---

### 4️⃣ API: Store Gateway Endpoint Consolidation

**Scope**: Backend/Gateway/Store/API  
**Current State**: Multiple overlapping endpoints

```
Current API:
├── GET /api/v1/products
├── GET /api/v1/products/{id}
├── GET /api/v2/products (new, slightly different)
├── GET /api/v2/products/{id} (new)
├── POST /api/v1/products/search
└── POST /api/v2/products/search ← BOTH ACTIVE
```

**Refactoring Goal**: Deprecate v1, consolidate to v2

**Affected Files**: ~12
```
- Controllers (v1 + v2)
- DTOs (v1 + v2)
- Services
- Tests
- Frontend consumers
```

**Pattern**: API → Contract Evolution (Section E in strategy)

**Risk Level**: 🟡 **MEDIUM-HIGH**
- External consumers might be affected
- 2+ release deprecation cycle needed
- Breaking changes require communication

**MCP Tools**:
- [ ] API MCP: `check_breaking_changes`, `detect_consumers`
- [ ] Roslyn MCP: `find_all_calls`
- [ ] Git MCP: `validate_commit_messages`

**Estimated Duration**: **6-8 weeks** (multi-release deprecation)
- Week 1: Analysis + consumer identification
- Week 2-3: PR #1-2 (Adapter + migration)
- Week 4-8+: Monitor deprecation + gradual removal

**Team Impact**: 🟡 **MEDIUM**
- Unblocks: Cleaner API surface
- Coordination needed: External partners

**Learning Value**: 🔵 **MEDIUM-HIGH**
- Contract evolution pattern
- Deprecation strategy
- Longer timeline (not ideal for first pilot)

**⚠️ RECOMMENDATION**: **Good but later** (requires external coordination)

---

## 🎯 RECOMMENDATION FOR FIRST PILOT

### **Option A (RECOMMENDED)**: Backend - ProductService Handler Consolidation
- ✅ Perfect scope (8 files)
- ✅ Low risk (no breaking changes)
- ✅ High learning value
- ✅ Realistic timeline (4-5 days)
- ✅ Team can validate strategy
- ✅ MCP tools (Roslyn) utilized

**Suggested**: **Start Monday (Jan 13), Finish Friday (Jan 17)**

### **Option B (ALTERNATIVE)**: Frontend - ProductDetail Composables
- ✅ Good scope (6 files)
- ✅ Low risk (internal refactoring)
- ✅ High learning value
- ✅ Realistic timeline (4-5 days)
- ✅ Valuable for frontend team
- ✅ MCP tools (Vue, TypeScript) utilized

**Suggested**: **Option A first, then Option B (Week 2)**

---

## 📋 NEXT STEPS

### This Week (Jan 7-10)
- [ ] @Architect + @TechLead review candidates
- [ ] Select preferred option
- [ ] Notify team of choice
- [ ] Schedule team training (Week 2)

### Next Week (Jan 13-17)
- [ ] **Team Training** (Fundamentals + Decision Tree)
- [ ] **Pilot Refactoring Begins**
  - Day 1: Complete Phase 1 analysis
  - Day 2-4: Execute PRs
  - Day 5: Validation + merge

### Week After (Jan 20-24)
- [ ] **Retrospective**
  - Measure success metrics
  - Collect team feedback
  - Document lessons learned
- [ ] **Process Optimization** (v2)
- [ ] **2nd Refactoring** (using optimized process)

---

## 🎓 Training Prep (for chosen candidate)

**Pre-Pilot Training (1 day)**:
1. Review BS-REFACTOR-001 (30 min)
2. Decision tree workshop (30 min)
3. MCP tools demo (domain-specific, 30 min)
4. Q&A + blockers (30 min)

**During Pilot (daily)**:
- Morning standup (15 min)
- Blockers identification
- Code review feedback

**Post-Pilot (1 day)**:
- Retrospective meeting (1.5h)
- Success metrics review
- Lessons learned documentation

---

## 📊 Success Definition

**Pilot Success** = Achievement of ≥4 of these:
- ✅ All PRs merged on schedule (within 5 days)
- ✅ All tests passing (zero regressions)
- ✅ Avg PR size <400 lines
- ✅ Avg review cycles ≤1.5 (target: 1)
- ✅ Team feedback ≥3.5/5 on process
- ✅ MCP automation saved 5+ hours
- ✅ Zero unexpected blockers
- ✅ Lessons learned identified & documented

---

**Ready for Review?** → Share with @Architect + @TechLead for decision
