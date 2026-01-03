# Pilot Migration Candidates

**Generated:** 2026-01-03T06:59:14.266Z
**Total Files Analyzed:** 55
**Top 10 Selected:** Based on criticality and business impact

## Top 10 Critical Files


### 1. frontend/Store/src/views/PrivateCustomerRegistration.vue

**Project:** Store Frontend
**Errors:** 4 | **Warnings:** 0
**Critical Score:** 40 | **Priority:** 6
**Final Score:** 240

**Reasons:** 🔄 Frequently Modified

**Top Issues:**
- error: Unnecessary escape character: \[  no-useless-escape
- error: Unnecessary escape character: \/  no-useless-escape
- error: Unnecessary escape character: \[  no-useless-escape
- error: Unnecessary escape character: \/  no-useless-escape

**Migration Priority:** 🟡 MEDIUM


### 2. frontend/Store/src/composables/useLocale.ts

**Project:** Store Frontend
**Errors:** 2 | **Warnings:** 1
**Critical Score:** 21 | **Priority:** 9
**Final Score:** 189

**Reasons:** 🔄 Frequently Modified, 📜 Legacy TypeScript Patterns

**Top Issues:**
- error: Do not use "@ts-nocheck" because it alters compilation errors                                                        @typescript-eslint/ban-ts-comment
- error: Use "@ts-expect-error" instead of "@ts-ignore", as "@ts-ignore" will do nothing if the following line is error-free  @typescript-eslint/ban-ts-comment
- warning: Unexpected any. Specify a different type                                                                             @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


### 3. frontend/Store/src/components/ERP/CustomerLookup.vue

**Project:** Store Frontend
**Errors:** 2 | **Warnings:** 0
**Critical Score:** 20 | **Priority:** 6
**Final Score:** 120

**Reasons:** 🔄 Frequently Modified

**Top Issues:**
- error: `defineEmits` has been called multiple times  vue/valid-define-emits
- error: `defineEmits` has been called multiple times  vue/valid-define-emits

**Migration Priority:** 🟡 MEDIUM


### 4. frontend/Store/tests/e2e/cms/cms-api.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 8
**Critical Score:** 8 | **Priority:** 14
**Final Score:** 112

**Reasons:** 🔗 API/Service Layer, 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


### 5. frontend/Store/tests/e2e/api.health.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 5
**Critical Score:** 5 | **Priority:** 14
**Final Score:** 70

**Reasons:** 🔗 API/Service Layer, 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'API_BASE' is assigned a value but never used       @typescript-eslint/no-unused-vars
- warning: 'validStatuses' is assigned a value but never used  @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type            @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type            @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type            @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


### 6. frontend/Store/src/views/__tests__/CustomerTypeSelection.test.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 11
**Critical Score:** 11 | **Priority:** 6
**Final Score:** 66

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'vi' is defined but never used              @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type    @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type    @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type    @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type    @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 7. frontend/Store/tests/views/CustomerTypeSelection.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 9
**Critical Score:** 9 | **Priority:** 6
**Final Score:** 54

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 8. frontend/Store/tests/components/Checkout.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 4
**Critical Score:** 4 | **Priority:** 13
**Final Score:** 52

**Reasons:** 🛒 Core Business Logic, 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'vi' is defined but never used                    @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type          @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type          @typescript-eslint/no-explicit-any
- warning: 'initialText' is assigned a value but never used  @typescript-eslint/no-unused-vars

**Migration Priority:** 🔴 HIGH


### 9. frontend/Store/src/composables/__tests__/useErpIntegration.spec.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 8
**Critical Score:** 8 | **Priority:** 6
**Final Score:** 48

**Reasons:** 🔄 Frequently Modified, 🧪 Test File (Lower Priority), 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: 'ref' is defined but never used           @typescript-eslint/no-unused-vars
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🟡 MEDIUM


### 10. frontend/Store/src/types/cms.ts

**Project:** Store Frontend
**Errors:** 0 | **Warnings:** 5
**Critical Score:** 5 | **Priority:** 9
**Final Score:** 45

**Reasons:** 🔄 Frequently Modified, 📜 Legacy TypeScript Patterns

**Top Issues:**
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any
- warning: Unexpected any. Specify a different type  @typescript-eslint/no-explicit-any

**Migration Priority:** 🔴 HIGH


## Migration Strategy

1. **Phase 1 (Week 1):** Files 1-3 (High Priority - Auth/Security)
2. **Phase 2 (Week 1):** Files 4-6 (Medium Priority - Core Business)
3. **Phase 3 (Week 2):** Files 7-10 (Remaining High Impact)

## Success Criteria
- ✅ Zero ESLint errors in migrated files
- ✅ TypeScript strict mode compliance
- ✅ No breaking changes to functionality
- ✅ Tests still pass
