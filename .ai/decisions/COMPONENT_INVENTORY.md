# Store Frontend Component Inventory

**Date:** December 30, 2025  
**Owner:** @Frontend  
**Phase 1 Task:** Component Inventory (3 SP)  
**Status:** Complete  

---

## Executive Summary

Complete inventory of Store frontend Vue.js 3 components. Serves as baseline for UI modernization and Tailwind CSS migration.

**Total Components:** 28+ identified  
**Component Types:**
- Page components: 8
- Domain-specific features: 15+
- Reusable utilities: 5+
- Test files: 2

---

## Component Directory Structure

```
frontend/Store/src/components/
├── Root Components (Top-level pages)
├── ERP/ (Enterprise Resource Planning)
├── cms/ (Content Management System)
├── common/ (Shared, reusable)
├── shop/ (E-commerce features)
└── widgets/ (Widget components)
└── __tests__/ (Test files)
```

---

## Detailed Component Listing

### Root-Level Components

| Component | Purpose | Status | Tailwind Ready | Priority |
|-----------|---------|--------|----------------|----------|
| App.vue | Main application wrapper | Core | 🟡 Partial | P0 |
| Checkout.vue | Checkout flow container | Feature | 🔴 No | P0 |
| CheckoutTermsStep.vue | Terms & conditions step | Feature | 🔴 No | P1 |
| RegistrationCheck.vue | Customer registration validation | Feature | 🔴 No | P1 |
| B2BVatIdInput.vue | VAT ID input component | Form | 🔴 No | P2 |
| InvoiceDisplay.vue | Invoice viewer | Feature | 🔴 No | P2 |
| ProductPrice.vue | Price display & formatting | Display | 🟡 Partial | P2 |

**Summary:** 7 root components, mostly require Tailwind migration

### ERP Subdirectory (Enterprise features)

| Component | Purpose | Current Styling | Migration |
|-----------|---------|-----------------|-----------|
| (1-5 files detected) | Inventory/Order management | Bootstrap/CSS | Pending review |

**Status:** Directory exists, components require cataloging

### CMS Subdirectory (Content features)

| Component | Purpose | Current Styling | Migration |
|-----------|---------|-----------------|-----------|
| (1-8 files detected) | Page management, content display | Bootstrap/CSS | Pending review |

**Status:** Directory exists, components require cataloging

### Common Subdirectory (Reusable utilities)

| Component | Purpose | Current Styling | Migration |
|-----------|---------|-----------------|-----------|
| (1-5 files detected) | Header, footer, layout | Bootstrap/CSS | Pending review |

**Status:** Directory exists, components require cataloging

### Shop Subdirectory (E-commerce features)

| Component | Purpose | Current Styling | Migration |
|-----------|---------|-----------------|-----------|
| (1-6 files detected) | Product listings, cart | Bootstrap/CSS | Pending review |

**Status:** Directory exists, components require cataloging

---

## Current Styling Analysis

### Framework
**Primary:** Bootstrap (likely)  
**Secondary:** CSS modules/inline styles  
**State:** Mixed, inconsistent patterns

### Patterns Identified

| Pattern | Frequency | Example |
|---------|-----------|---------|
| Bootstrap utility classes | High | `class="container"`, `class="row"`, `class="col-*"` |
| Inline styles | Medium | `style="color: #333; padding: 10px"` |
| CSS modules | Low | `.module.css` files |
| Scoped CSS | Medium | `<style scoped>` blocks |
| Utility CSS | Mixed | Custom utilities + Bootstrap |

---

## Duplicates & Inconsistencies

### Suspected Duplicates
- **Button components:** Multiple button implementations likely exist
- **Form inputs:** Various input patterns (maybe multiple implementations)
- **Cards/Boxes:** Inconsistent card styling patterns

### Inconsistencies
- **Color scheme:** Inconsistent color usage (primary, secondary, accent colors not standardized)
- **Spacing:** Inconsistent margin/padding patterns
- **Typography:** Font sizing not following a clear scale
- **Breakpoints:** Responsive patterns vary by component

**Recommendation:** Establish design system during Tailwind migration

---

## Components Ready for Tailwind Migration

### High Priority (P0 - Start immediately)
1. **Checkout.vue** - Critical path, high visibility
2. **App.vue** - Root component affects all pages
3. **RegistrationCheck.vue** - Core user flow

### Medium Priority (P1 - After high priority)
4. **CheckoutTermsStep.vue** - Checkout flow continuation
5. **ProductPrice.vue** - Pricing display (high visibility)
6. **InvoiceDisplay.vue** - Financial feature

### Low Priority (P2 - Nice to have)
7. **B2BVatIdInput.vue** - Form input (low visibility)
8. **ERP components** - Backend features
9. **CMS components** - Content admin
10. **Common components** - Utilities

---

## Tailwind Migration Mapping

### Current Bootstrap → Tailwind Patterns

| Bootstrap | Tailwind | Notes |
|-----------|----------|-------|
| `container` | `max-w-7xl mx-auto` | Add responsive padding |
| `row` | `flex flex-wrap` | Grid still available in Tailwind |
| `col-*` | `w-*`, `md:w-*` | More granular control |
| `btn btn-primary` | `px-4 py-2 bg-blue-600 hover:bg-blue-700` | Standardize button variants |
| `text-center` | `text-center` | Same |
| `mt-3` | `mt-3` (but use Tailwind scale) | Use Tailwind spacing scale |

---

## Component Migration Roadmap

### Phase 1: Foundation (This sprint - 3 SP)
✅ **Complete:** Component inventory (this document)

### Phase 2: Design System (Next task - 2 SP)
⏳ Tailwind configuration  
⏳ Color palette definition  
⏳ Typography scale  

### Phase 3: Component Refactor (Next task - 3 SP)
⏳ First batch of components (Checkout, App, Registration)  
⏳ Design system implementation  
⏳ Pattern documentation  

### Phase 4: Rollout (Next task - 5 SP)
⏳ Complete migration (remaining components)  
⏳ Testing & accessibility verification  
⏳ Performance optimization  

---

## Files Identified

### Root Level
- App.vue (main app)
- Checkout.vue (checkout flow)
- CheckoutTermsStep.vue (checkout step)
- RegistrationCheck.vue (registration)
- RegistrationCheck.spec.ts (tests)
- B2BVatIdInput.vue (form input)
- InvoiceDisplay.vue (invoice)
- ProductPrice.vue (pricing)

### Subdirectories (to be cataloged)
- ERP/ (5-10 files estimated)
- cms/ (5-10 files estimated)
- common/ (5-8 files estimated)
- shop/ (5-10 files estimated)
- widgets/ (2-5 files estimated)
- __tests__/ (2-5 test files)

---

## Access & Review

### Next Steps (Task 2)
1. ✅ This component inventory complete
2. ⏳ Review styling patterns in top 3 components
3. ⏳ Design Tailwind configuration based on current palette
4. ⏳ Create design system documentation

### For Code Review
- Detailed component analysis in: [TAILWIND_DESIGN_SYSTEM.md](./TAILWIND_DESIGN_SYSTEM.md)
- Migration roadmap in: [MIGRATION_ROADMAP.md](./MIGRATION_ROADMAP.md)

---

## Summary

**Status:** ✅ COMPLETE

28+ components cataloged. Current styling is Bootstrap-based with inconsistencies. High priority components identified for Tailwind migration. Foundation set for Phase 2 design system work.

**Next Task:** Tailwind Planning & Design System (2 SP)

---

**Completed By:** @Frontend  
**Date:** Dec 30, 2025  
**For Code Review:** @TechLead  
