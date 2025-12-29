# Sprint 3 Phase 2.3 - E2E Testing Setup Guide

**Status**: ✅ READY FOR EXECUTION  
**Date**: 29. Dezember 2025, 14:45 UTC  

---

## 🎯 E2E Testing Status

### Current State
- ✅ Playwright configured (`playwright.config.ts` exists)
- ✅ E2E test file created (`e2e/checkout.spec.ts` - 211 lines)
- ✅ 4 test scenarios written and ready to execute
- ✅ Vite dev server configured on port 5173
- ⏳ Waiting for dev server startup

### Test Scenarios (4 Total)

1. **Happy Path** (Lines 17-74) ✅ READY
   - Navigate to checkout
   - Fill shipping address (John Doe, Hauptstraße 123, 10115 Berlin)
   - Select Express shipping (€12.99)
   - Select PayPal payment
   - Accept terms
   - Complete order
   - Verify navigation to `/order-confirmation`
   - Capture screenshot: `checkout-success.png`
   - Expected: ✅ PASS

2. **Validation Error Flow** (Lines 76-121) ✅ READY
   - Leave firstName empty
   - Try to advance (should show error or block)
   - Add firstName
   - Verify error clears
   - Proceed successfully
   - Expected: ✅ PASS

3. **Edit Flow** (Lines 123-170) ✅ READY
   - Complete to review step
   - Click "Edit" on shipping section
   - Return to step 1
   - Modify address
   - Navigate back to review
   - Verify changes persisted
   - Expected: ✅ PASS

4. **Mobile Responsive** (Lines 172-211) ✅ READY
   - Set viewport to 320px (mobile)
   - Navigate checkout
   - Fill address
   - Verify form layout adapts
   - Verify buttons are clickable (44px+ touch targets)
   - Expected: ✅ PASS

---

## 📋 Prerequisites for E2E Execution

### 1. Dev Server Running (Port 5173)
```bash
cd Frontend/Store
npm run dev
# Expected output:
# ➜ Local: http://localhost:5173/
# ➜ Press q to quit
```

### 2. Playwright Installed
```bash
cd Frontend/Store
npm list @playwright/test
# Expected: @playwright/test@1.40.0+ (or latest)
```

### 3. Browser Available
Playwright uses system Chromium by default:
```bash
npx playwright install
# Downloads: chromium, firefox, webkit (if needed)
```

---

## 🚀 Execution Steps (Phase 2.3)

### Step 1: Start Dev Server (Terminal 1)
```bash
cd /Users/holger/Documents/Projekte/B2Connect/Frontend/Store
npm run dev
```
**Expected**: 
```
VITE v5.x.x ready in 245 ms

➜  Local:   http://localhost:5173/
➜  press q to quit
```

### Step 2: Run E2E Tests (Terminal 2)
```bash
cd /Users/holger/Documents/Projekte/B2Connect/Frontend/Store
npx playwright test e2e/checkout.spec.ts --headed
```

**Flags Explained**:
- `--headed`: Show browser window during test (useful for debugging)
- `--reporter=html`: Generate HTML report (optional)
- `--reporter=list`: Show test list in terminal (default)

**Expected Duration**: ~30-45 seconds

**Expected Output**:
```
Running 4 tests using 1 worker

  ✓  Checkout E2E Tests › should complete full checkout flow successfully (8.2s)
  ✓  Checkout E2E Tests › should show validation errors and allow correction (6.5s)
  ✓  Checkout E2E Tests › should handle edit flow and verify persistence (7.1s)
  ✓  Checkout E2E Tests › should handle mobile responsive layout (5.3s)

  4 tests passed (27.1s)
```

### Step 3: View Test Report (Optional)
```bash
npx playwright show-report
```
**Opens**: HTML report with screenshots and details

### Step 4: Generate Coverage Report
```bash
npx playwright test e2e/checkout.spec.ts --reporter=html
```

---

## 🔧 Troubleshooting E2E Tests

### Issue 1: "connect ECONNREFUSED localhost:5173"
**Cause**: Dev server not running  
**Fix**: 
```bash
# Terminal 1:
cd Frontend/Store && npm run dev
# Wait for "ready in XXX ms"

# Terminal 2:
npx playwright test e2e/checkout.spec.ts
```

### Issue 2: "Timeout waiting for selector"
**Cause**: Element not found or takes too long to load  
**Fix**: Increase timeout in test or add longer wait
```typescript
await page.waitForSelector('.checkout-form', { timeout: 10000 })  // 10 seconds
```

### Issue 3: "Navigation to /order-confirmation failed"
**Cause**: Order submission endpoint not implemented  
**Fix**: Mock the submission in test:
```typescript
await page.route('**/api/orders', route => {
  route.abort('serviceunavailable')  // Or respond with mock data
})
```

### Issue 4: "Input not visible"
**Cause**: Element off-screen or hidden  
**Fix**: Scroll into view first
```typescript
await page.locator('input[id="firstName"]').scrollIntoViewIfNeeded()
```

---

## 📊 Expected Results Summary

### Test Execution
| Test | Scenario | Status | Duration | Pass |
|------|----------|--------|----------|------|
| 1 | Happy Path | ⏳ Ready | ~8s | ✅ Expected |
| 2 | Validation | ⏳ Ready | ~6.5s | ✅ Expected |
| 3 | Edit Flow | ⏳ Ready | ~7s | ✅ Expected |
| 4 | Mobile | ⏳ Ready | ~5s | ✅ Expected |
| **TOTAL** | **4 Scenarios** | **⏳ Ready** | **~27s** | **✅ 4/4** |

### Success Criteria
- [ ] All 4 scenarios passing (0 failures)
- [ ] Total execution < 45 seconds
- [ ] Screenshots captured (checkout-success.png, etc.)
- [ ] No timeout errors
- [ ] No navigation errors

---

## 📸 Screenshots Generated

When tests pass, these images will be created:
1. `checkout-success.png` - Happy path completion
2. Mobile responsive layouts (if configured)
3. Validation error states (if configured)

**Location**: `Frontend/Store/e2e/` or configured output directory

---

## 🎓 What E2E Tests Validate

✅ **User Workflows**
- Complete checkout journey from start to finish
- Step navigation and validation
- Form field interactions
- Payment method selection
- Terms acceptance

✅ **Integration Points**
- Form submission
- API calls (if mocked)
- Navigation routing
- URL transitions

✅ **UI/UX Quality**
- Button visibility and clickability
- Form field responsiveness
- Mobile layout adaptation
- Screenshot captures for visual regression

✅ **Accessibility** (Partial)
- Keyboard navigation
- Button interaction
- Form field focus
- Error message visibility

---

## 🚀 Full Execution Command (One-Liner)

When dev server is running:
```bash
cd /Users/holger/Documents/Projekte/B2Connect/Frontend/Store && \
npx playwright test e2e/checkout.spec.ts --reporter=list && \
npx playwright show-report
```

This:
1. Runs all 4 E2E tests
2. Shows list reporter output
3. Opens HTML report automatically

---

## ⏱️ Timeline for Phase 2.3

| Task | Duration | Status |
|------|----------|--------|
| Start dev server | 2 min | ⏳ Ready |
| Run E2E tests | 1 min (headless) | ⏳ Ready |
| Review results | 2 min | ⏳ Ready |
| Generate report | 1 min | ⏳ Ready |
| Troubleshoot (if needed) | 10-15 min | ⏳ Ready |
| **TOTAL** | **~15-20 min** | **⏳ Ready** |

**Remaining Phase Time**: 90 minutes total  
**Allocated**: 60 minutes (1 hour)  
**Buffer**: 30 minutes for issues

---

## 🎯 Next Actions (in order)

1. ✅ **Review this guide** (Current)
2. ⏳ **Start dev server** (5 min)
3. ⏳ **Run E2E tests** (1 min to execute)
4. ⏳ **Review results** (5 min)
5. ⏳ **Fix any failures** (as needed)
6. ⏳ **Generate coverage report** (2 min)
7. ⏳ **Document Phase 2.3 results** (10 min)
8. ⏳ **Proceed to Phase 2.4 (Accessibility)** (2 Jan)

---

## ✨ Why Phase 2.3 is Critical

**E2E tests validate**:
- ✅ User can complete real checkout workflow
- ✅ Form validation prevents invalid submissions
- ✅ Navigation flows correctly between steps
- ✅ Mobile users can interact with checkout
- ✅ Order confirmation displays after submission
- ✅ All fields and buttons are accessible

**Without E2E tests**, we would have:
- ❌ No proof users can complete checkout
- ❌ Unknown form interaction issues
- ❌ Missing mobile usability validation
- ❌ Undetected navigation problems

---

## 📝 Phase 2.3 Completion Checklist

- [ ] Dev server running on localhost:5173
- [ ] All 4 E2E scenarios pass (0 failures)
- [ ] Test execution < 45 seconds
- [ ] Screenshots captured successfully
- [ ] HTML report generated
- [ ] Results documented in progress file
- [ ] No blocking issues for production

---

**Phase 2.3 Estimate**: 90 min budgeted, ~20 min required (⏳ High confidence)  
**Next Phase**: 2.4 Accessibility Audit (Jan 2)  
**Final Milestone**: Phase 3 Documentation (Jan 3)  
**Launch Target**: 4 Januar 2026 ✅

