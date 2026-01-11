# Pilot Migration Candidates

**Generated:** 2026-01-03T07:07:22.475Z
**Total Files Analyzed:** 46
**Top 10 Selected:** Based on criticality and business impact

## Top 10 Critical Files


### 1. frontend/Store/src/components/RegistrationCheck.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 7
**Critical Score:** 7 | **Priority:** 6
**Final Score:** 42

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 2. frontend/Store/tests/components/cms/WidgetRenderer.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 5
**Critical Score:** 5 | **Priority:** 6
**Final Score:** 30

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'createRouter' is defined but never used         @typescript-eslint/no-unused-vars
- warning: 'createMemoryHistory' is defined but never used  @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type         @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type         @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type         @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 3. frontend/Store/src/composables/useLocale.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 3
**Critical Score:** 3 | **Priority:** 9
**Final Score:** 27

**Reasons:** 🔄 Frequently Modified, 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


### 4. frontend/Store/src/components/__tests__/Checkout.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 2
**Critical Score:** 2 | **Priority:** 13
**Final Score:** 26

**Reasons:** 🛒 Core Business Logic, 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type          @typescript-eslint/no-explicit-any
- warning: 'initialText' is assigned a value but never used  @typescript-eslint/no-unused-vars

**Migration Priority:** 🔴 HIGH


### 5. frontend/Store/src/views/ProductDetail.vue

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 4
**Critical Score:** 4 | **Priority:** 6
**Final Score:** 24

**Reasons:** 🔄 Frequently Modified

**Top Issues:**
- warning: 'router' is assigned a value but never used           @typescript-eslint/no-unused-vars
- warning: 'relatedProducts' is assigned a value but never used  @typescript-eslint/no-unused-vars
- warning: 'priceBreakdown' is assigned a value but never used   @typescript-eslint/no-unused-vars
- warning: 'averageRating' is assigned a value but never used    @typescript-eslint/no-unused-vars

**Migration Priority:** 🟡 MEDIUM


### 6. frontend/Store/src/components/ERP/__tests__/CustomerLookup.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 3
**Critical Score:** 3 | **Priority:** 6
**Final Score:** 18

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 7. frontend/Store/src/components/widgets/FeatureGrid.vue

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 2
**Critical Score:** 2 | **Priority:** 9
**Final Score:** 18

**Reasons:** 🔄 Frequently Modified, 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'Feature' is defined but never used       @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


### 8. frontend/Store/src/components/widgets/NewsletterSignup.vue

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 2
**Critical Score:** 2 | **Priority:** 9
**Final Score:** 18

**Reasons:** 🔄 Frequently Modified, 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: 'error' is defined but never used         @typescript-eslint/no-unused-vars

**Migration Priority:** 🔴 HIGH


### 9. frontend/Store/src/views/ProductListing.vue

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 3
**Critical Score:** 3 | **Priority:** 6
**Final Score:** 18

**Reasons:** 🔄 Frequently Modified

**Top Issues:**
- warning: 'ProductPrice' is defined but never used         @typescript-eslint/no-unused-vars
- warning: 'router' is assigned a value but never used      @typescript-eslint/no-unused-vars
- warning: 'categories' is assigned a value but never used  @typescript-eslint/no-unused-vars

**Migration Priority:** 🟡 MEDIUM


### 10. frontend/Store/tests/views/PrivateCustomerRegistration.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 3
**Critical Score:** 3 | **Priority:** 6
**Final Score:** 18

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'beforeEach' is defined but never used    @typescript-eslint/no-unused-vars
- warning: 'VueWrapper' is defined but never used    @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


## Migration Strategy

1. **Phase 1 (Week 1):** Files 1-3 (High Priority - Auth/Security)
2. **Phase 2 (Week 1):** Files 4-6 (Medium Priority - Core Business)
3. **Phase 3 (Week 2):** Files 7-10 (Remaining High Impact)

## Success Criteria
- ✅ Zero ESLint errors in migrated files
- ✅ TypeScript strict mode compliance
- ✅ No breaking changes to functionality
- ✅ Tests still pass
