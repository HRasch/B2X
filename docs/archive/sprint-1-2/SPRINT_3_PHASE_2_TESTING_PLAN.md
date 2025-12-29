# Sprint 3 Phase 2: Testing & Validation

**Status**: 🔄 READY TO BEGIN  
**Date**: 29. Dezember 2025  
**Duration**: 5 hours allocated  
**Tests to Create**: Unit + E2E + Accessibility Audit  

---

## 📋 Testing Plan

### Unit Tests (2 hours) - Using Vitest + @vue/test-utils

#### Test Suite: Checkout.vue

**Form Validation Tests** (4 tests):
- [ ] Test validateForm() with valid address data
- [ ] Test validateForm() with missing firstName
- [ ] Test validateForm() with invalid zipCode (not 5 digits)
- [ ] Test validateForm() with missing country

**Step Navigation Tests** (3 tests):
- [ ] Test nextStep() advances from step 1 to 2
- [ ] Test nextStep() fails if step 1 validation fails
- [ ] Test prevStep() goes back to previous step

**Shipping Selection Tests** (4 tests):
- [ ] Test selectShippingMethod() updates selected method
- [ ] Test selectShippingMethod() updates shippingCost
- [ ] Test selectShippingMethod() recalculates total
- [ ] Test all 3 shipping methods have correct prices

**Payment Selection Tests** (2 tests):
- [ ] Test selectPaymentMethod() updates selected method
- [ ] Test all 3 payment methods are selectable

**Price Calculation Tests** (5 tests):
- [ ] Test subtotal calculation with multiple items
- [ ] Test VAT calculation (19% of subtotal)
- [ ] Test total = subtotal + VAT + shipping
- [ ] Test shipping cost updates total when changed
- [ ] Test total formatting with German locale

**Computed Properties Tests** (3 tests):
- [ ] Test currentStepIndex returns correct index (0-2)
- [ ] Test isFormValid works per step
- [ ] Test stepCompletion array tracks completion

**Complete Order Tests** (2 tests):
- [ ] Test completeOrder() with valid form and payment
- [ ] Test completeOrder() fails if terms not agreed

#### Target
- **15+ test cases total**
- **≥80% code coverage**
- **All tests passing**

---

### E2E Tests (1.5 hours) - Using Playwright

#### Test Suite: Checkout Flow

**Happy Path Scenario** (1 test):
1. Navigate to /checkout
2. Fill in shipping address (all fields)
3. Click "Next to Shipping"
4. Select "Express" shipping (€12.99)
5. Click "Next to Review"
6. Verify order review shows address and shipping
7. Select "PayPal" payment
8. Check terms checkbox
9. Click "Complete Order"
10. Verify redirect to /order-confirmation
11. Assert order total is correct

**Validation Error Scenario** (1 test):
1. Navigate to /checkout
2. Leave firstName empty
3. Try to click "Next to Shipping"
4. Verify error message appears
5. Fill in firstName
6. Verify error disappears
7. Complete checkout successfully

**Edit Flow Scenario** (1 test):
1. Navigate to /checkout
2. Fill in address
3. Select shipping
4. Go to review step
5. Click "Edit" button on address
6. Go back to step 1
7. Change street address
8. Go forward to review again
9. Verify new address is displayed

**Mobile Responsiveness** (1 test):
1. Set viewport to 320px width
2. Navigate to /checkout
3. Verify progress indicator is visible
4. Verify form inputs are readable (16px+ font)
5. Verify sticky sidebar becomes static
6. Complete checkout on mobile view

#### Target
- **4 complete test scenarios**
- **All scenarios passing**
- **Mobile viewport verified**

---

### Accessibility Audit (1.5 hours) - Lighthouse + axe + Manual Testing

#### Automated Checks

**Lighthouse Accessibility Audit**:
- [ ] Run Lighthouse audit on /checkout
- [ ] Target score: ≥90
- [ ] Check all metrics:
  - Color contrast (4.5:1+)
  - ARIA labels present
  - Form labels associated
  - Button names accessible
  - List structure proper
  - Focus visible on all interactive elements

**axe DevTools Scan**:
- [ ] Scan for critical violations
- [ ] Scan for serious violations
- [ ] Check for warnings
- [ ] Target: 0 critical, 0 serious, <5 warnings

#### Manual Accessibility Testing

**Keyboard Navigation** (10 min test):
- [ ] Tab through entire form (no focus trap)
- [ ] Shift+Tab works backward
- [ ] Enter key submits forms
- [ ] Enter key selects radio buttons
- [ ] Escape key closes any modals
- [ ] All interactive elements reachable with keyboard
- [ ] Focus order logical and sequential

**Screen Reader Testing** (10 min test with NVDA/VoiceOver):
- [ ] Page title announced correctly
- [ ] Form labels announced with inputs
- [ ] Error messages announced as alerts
- [ ] Step progress announced ("Step 1 of 3")
- [ ] Button purposes clear when read alone
- [ ] Abbreviations expanded (e.g., "Email", not "eMail")
- [ ] Currency symbols announced ("19 Euros", not "19€")

**Color Contrast Verification**:
- [ ] All text ≥4.5:1 contrast ratio
- [ ] Links distinguishable from text
- [ ] Error messages (red) ≥4.5:1 on white
- [ ] Green VAT highlight ≥4.5:1 on white
- [ ] Orange shipping ≥4.5:1 on white

**Mobile Accessibility** (320px viewport):
- [ ] Touch targets ≥44px × 44px
- [ ] Font size ≥16px (no zoom needed)
- [ ] Form inputs auto-suggest turned on
- [ ] Sticky elements don't hide content
- [ ] Buttons large enough for thumb navigation

#### Target
- **Lighthouse score ≥90**
- **Zero critical/serious axe violations**
- **Keyboard navigation 100% functional**
- **Screen reader compatible (10+ elements verified)**
- **Mobile touch-friendly (44px+ targets)**

---

## 📊 Testing Checklist

### Before Testing Begins
- [ ] Build completed successfully (`dotnet build` or npm build)
- [ ] All dependencies installed
- [ ] Test database seeded with sample data
- [ ] Browser dev tools open
- [ ] NVDA/VoiceOver screen reader available
- [ ] Playwright browsers installed (`npx playwright install`)

### Unit Testing Execution
- [ ] Create `src/components/__tests__/Checkout.spec.ts`
- [ ] Write all 15+ test cases
- [ ] Run `npm run test` (Vitest)
- [ ] Verify ≥80% coverage
- [ ] All tests passing (green checkmarks)
- [ ] Generate coverage report

### E2E Testing Execution
- [ ] Create `e2e/checkout.spec.ts`
- [ ] Write all 4 test scenarios
- [ ] Start frontend dev server (`npm run dev`)
- [ ] Run `npx playwright test e2e/checkout.spec.ts`
- [ ] All scenarios passing
- [ ] Generate test report with screenshots

### Accessibility Testing Execution
- [ ] Run Lighthouse: `npx lighthouse http://localhost:5173/checkout --only-categories=accessibility`
- [ ] Record score (target ≥90)
- [ ] Run axe scan in browser dev tools
- [ ] Record results (target: 0 critical, 0 serious)
- [ ] Manual keyboard test (10 min)
- [ ] Manual screen reader test (10 min)
- [ ] Document any issues found
- [ ] Create accessibility report

### Results Documentation
- [ ] Unit test report (pass/fail summary)
- [ ] E2E test report (scenario results + screenshots)
- [ ] Coverage report (line/branch/function coverage %)
- [ ] Accessibility audit report (Lighthouse score, axe results)
- [ ] Any bugs found (create GitHub issues)
- [ ] Any improvements needed (add to backlog)

---

## 🎯 Success Criteria

✅ All unit tests passing (15+ tests)  
✅ Code coverage ≥80% for Checkout.vue  
✅ All E2E scenarios passing (4 scenarios)  
✅ Lighthouse accessibility ≥90  
✅ Zero axe critical violations  
✅ Zero axe serious violations  
✅ Keyboard navigation 100% functional  
✅ Screen reader compatible  
✅ Mobile touch-friendly (44px+ targets)  
✅ All bugs documented and filed  

---

## 📝 Test Execution Order

1. **Start**: Unit tests (framework warmup, quick feedback)
2. **Build confidence**: E2E tests (full workflow validation)
3. **Verify accessibility**: Lighthouse + axe (automated)
4. **Manual verification**: Keyboard + screen reader (comprehensive)
5. **Document**: Create reports and file any issues
6. **Review**: Verify all success criteria met

---

## 🔧 Tools & Commands

```bash
# Unit testing
npm run test                           # Run all unit tests (Vitest)
npm run test -- --coverage             # With coverage report
npm run test -- --watch                # Watch mode for development

# E2E testing
npx playwright test                    # Run all E2E tests
npx playwright test e2e/checkout.spec  # Run specific test file
npx playwright test --debug            # Debug mode

# Accessibility testing
npx lighthouse http://localhost:5173/checkout --only-categories=accessibility

# Browser DevTools
# 1. Open /checkout in Chrome
# 2. Open DevTools (F12)
# 3. Click "Lighthouse" tab
# 4. Select "Accessibility"
# 5. Click "Generate report"
```

---

## 💡 Testing Tips

- **Focus first**: Test happy path, then error cases
- **Use AAA pattern**: Arrange, Act, Assert
- **Mock external services**: useCartStore, useRouter
- **Test one thing per test**: Single responsibility
- **Meaningful assertions**: Test behavior, not implementation
- **Clear test names**: Describe what is being tested
- **Manual testing last**: Automated tests catch most issues

---

**Status**: Testing plan complete, ready to execute  
**Time Allocated**: 5 hours  
**Target Completion**: By 2 January 2025  
**Success Criteria**: All tests passing, Lighthouse ≥90, accessibility verified
