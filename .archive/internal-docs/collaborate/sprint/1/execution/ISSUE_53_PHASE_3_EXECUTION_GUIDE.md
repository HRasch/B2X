# Issue #53 Phase 3 Execution Guide - Backend Service Updates

**Date**: 30. Dezember 2025 (Evening) → 31. Dezember 2025 (Day)  
**Status**: 🚀 READY TO EXECUTE  
**Estimated Time**: 1.5-2 hours  
**Assigned**: @backend-developer (@HRasch)

---

## 🎯 Phase 3 Objective

Apply `ReturnConstants` to remaining services and consolidate duplicate validation logic across the Customer domain.

**Expected Outcome**: 
- ✅ ReturnManagementService updated to use ReturnConstants
- ✅ Duplicate validation logic extracted
- ✅ Modern C# patterns applied throughout
- ✅ Build: 0 compiler errors
- ✅ Tests: 100% pass rate

---

## 📋 Phase 3 Detailed Checklist

### Task 1: Update ReturnManagementService (45 minutes)

**File**: `backend/Domain/Customer/src/Services/ReturnManagementService.cs`

#### Subtask 1.1: Replace Magic Strings with ReturnConstants

Find and replace these strings:

```csharp
// BEFORE
requestStatus = "Pending";
requestStatus = "Approved";
requestStatus = "Rejected";
requestStatus = "Processing";
requestStatus = "Completed";
requestStatus = "Failed";

// AFTER
requestStatus = ReturnConstants.Status.Pending;
requestStatus = ReturnConstants.Status.Approved;
requestStatus = ReturnConstants.Status.Rejected;
requestStatus = ReturnConstants.Status.Processing;
requestStatus = ReturnConstants.Status.Completed;
requestStatus = ReturnConstants.Status.Failed;
```

#### Subtask 1.2: Replace Reason Constants

```csharp
// BEFORE
reason = "NoLongerNeeded";
reason = "DamageOrDefect";
reason = "NotAsDescribed";
reason = "FoundBetter";
reason = "ChangedMind";

// AFTER
reason = ReturnConstants.Reason.NoLongerNeeded;
reason = ReturnConstants.Reason.DamageOrDefect;
reason = ReturnConstants.Reason.NotAsDescribed;
reason = ReturnConstants.Reason.FoundBetter;
reason = ReturnConstants.Reason.ChangedMind;
```

#### Subtask 1.3: Replace Magic Numbers with Config Constants

```csharp
// BEFORE
if (daysElapsed > 30)  // VVVG §357 14-day right period + buffer
    throw new InvalidOperationException("Return period expired");

var maxAttempts = 3;  // Max retry attempts for logistics

// AFTER
if (daysElapsed > ReturnConfig.MaxReturnDays)
    throw new InvalidOperationException("Return period expired");

var maxAttempts = ReturnConfig.MaxLogisticsRetries;
```

#### Subtask 1.4: Apply Modern C# Patterns

```csharp
// BEFORE
if (request == null)
if (request != null)

// AFTER
if (request is null)
if (request is not null)
```

**Estimated Time**: 45 minutes  
**Priority**: 🔴 HIGH - Unblocks Phase 4

---

### Task 2: Extract Duplicate Validation Logic (30 minutes)

**Files Involved**:
- `backend/Domain/Customer/src/Services/ReturnManagementService.cs`
- `backend/Domain/Customer/src/Services/InvoiceService.cs`
- (Identify other services with duplicate validation)

#### Subtask 2.1: Identify Duplicates

Search for these common patterns:

```csharp
// Pattern 1: Date validation (appears in multiple services)
if (DateTime.Now > validUntil)
    throw new InvalidOperationException("Deadline passed");

// Pattern 2: Enum validation
if (!Enum.IsDefined(typeof(ReturnStatus), status))
    throw new ArgumentException("Invalid status");

// Pattern 3: Amount validation
if (amount <= 0)
    throw new ArgumentException("Amount must be positive");
```

#### Subtask 2.2: Create ValidationHelper (if duplicates found)

**File**: `backend/Domain/Customer/src/Utilities/ValidationHelper.cs` (NEW)

```csharp
using System;
using B2Connect.Customer.Models;

namespace B2Connect.Customer.Utilities;

/// <summary>
/// Common validation logic shared across Customer domain services.
/// Consolidates duplicate validation to reduce code duplication and improve maintainability.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates that a deadline hasn't passed.
    /// </summary>
    /// <param name="deadline">The deadline to validate</param>
    /// <param name="fieldName">Name of the field being validated (for error message)</param>
    /// <exception cref="InvalidOperationException">Thrown when deadline has passed</exception>
    public static void ValidateDeadlineNotPassed(DateTime deadline, string fieldName = "Deadline")
    {
        if (DateTime.Now > deadline)
            throw new InvalidOperationException($"{fieldName} has passed and this action is no longer valid.");
    }

    /// <summary>
    /// Validates that an amount is positive.
    /// </summary>
    /// <param name="amount">The amount to validate</param>
    /// <param name="fieldName">Name of the field being validated (for error message)</param>
    /// <exception cref="ArgumentException">Thrown when amount is not positive</exception>
    public static void ValidatePositiveAmount(decimal amount, string fieldName = "Amount")
    {
        if (amount <= 0)
            throw new ArgumentException($"{fieldName} must be positive (actual: {amount}).");
    }

    /// <summary>
    /// Validates enum value is defined.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="value">The value to validate</param>
    /// <param name="fieldName">Name of the field being validated (for error message)</param>
    /// <exception cref="ArgumentException">Thrown when enum value is invalid</exception>
    public static void ValidateEnumDefined<T>(T value, string fieldName = "Value") where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            throw new ArgumentException($"Invalid {fieldName}: {value}");
    }
}
```

#### Subtask 2.3: Update Services to Use ValidationHelper

In `ReturnManagementService.cs` and `InvoiceService.cs`:

```csharp
// BEFORE
if (request == null)
    throw new ArgumentNullException(nameof(request));

if (DateTime.Now > request.Deadline)
    throw new InvalidOperationException("Deadline passed");

if (request.Amount <= 0)
    throw new ArgumentException("Amount must be positive");

// AFTER
ArgumentNullException.ThrowIfNull(request);
ValidationHelper.ValidateDeadlineNotPassed(request.Deadline, nameof(request.Deadline));
ValidationHelper.ValidatePositiveAmount(request.Amount, nameof(request.Amount));
```

**Estimated Time**: 30 minutes  
**Priority**: 🟡 MEDIUM - Improves code quality

---

### Task 3: Review and Update Remaining Services (30 minutes)

Scan these services for additional optimization opportunities:

**Files to Review**:
1. `backend/Domain/Customer/src/Services/PaymentService.cs`
   - [ ] Check for magic strings or numbers
   - [ ] Apply ReturnConstants where applicable
   - [ ] Update null checks to `is null` pattern

2. `backend/Domain/Customer/src/Services/RefundService.cs`
   - [ ] Check for duplicate logic
   - [ ] Apply constants
   - [ ] Modernize patterns

3. `backend/Domain/Customer/src/Services/ShippingService.cs`
   - [ ] Check for hardcoded values
   - [ ] Apply configuration pattern
   - [ ] Update null checks

**Estimated Time**: 30 minutes  
**Priority**: 🟢 LOW - Can defer if time limited

---

## 📋 Implementation Steps

### Step 1: Locate ReturnManagementService

```bash
cd /Users/holger/Documents/Projekte/B2Connect
find . -name "ReturnManagementService.cs" -type f
```

**Expected**: `backend/Domain/Customer/src/Services/ReturnManagementService.cs`

### Step 2: Read Current File

Before making changes, understand the current implementation:

```bash
# Count lines
wc -l backend/Domain/Customer/src/Services/ReturnManagementService.cs

# Search for magic strings/numbers
grep -n "\"Pending\"\|\"Approved\"\|\"Rejected\"" \
  backend/Domain/Customer/src/Services/ReturnManagementService.cs

grep -n " == null\| != null" \
  backend/Domain/Customer/src/Services/ReturnManagementService.cs
```

### Step 3: Apply Changes (Multi-Replace Workflow)

Use the `multi_replace_string_in_file` tool to apply all ReturnConstants replacements in one operation.

**Example**:
```
File: backend/Domain/Customer/src/Services/ReturnManagementService.cs

Replace 1: "Pending" → ReturnConstants.Status.Pending
Replace 2: "Approved" → ReturnConstants.Status.Approved
Replace 3: == null → is null
... (additional replacements)
```

### Step 4: Verify ReturnConstants Import

Ensure the file has this import:

```csharp
using B2Connect.Customer.Models;  // For ReturnConstants
```

### Step 5: Build and Test

```bash
# Build
dotnet build B2Connect.slnx

# Test Customer service specifically
dotnet test backend/Domain/Customer/tests -v minimal

# Test all
dotnet test B2Connect.slnx -v minimal
```

---

## 🎯 Quality Criteria for Phase 3

### Code Quality Metrics

| Metric | Target | Verification |
|--------|--------|--------------|
| **Magic Strings in ReturnManagementService** | 0 | `grep "Pending\|Approved\|Rejected" ReturnManagementService.cs` should be empty |
| **Magic Strings in InvoiceService** | 0 | `grep "Draft\|Issued" InvoiceService.cs` should return only constants |
| **Null Check Pattern** | 100% `is null` | `grep -c " == null\| != null" *.cs` should be 0 |
| **Build Warnings** | 0 | `dotnet build` should show "0 warnings" |
| **Test Pass Rate** | 100% | `dotnet test` should show all tests passing |
| **Code Duplication** | Minimal | ValidationHelper extraction successful |
| **XML Documentation** | 100% public | All new/updated code documented |

### Acceptance Criteria

- [x] ReturnConstants applied to ReturnManagementService
- [ ] Duplicate validation logic extracted (if found)
- [ ] All services reviewed for optimization
- [ ] Null check patterns modernized
- [ ] Build: 0 errors, minimal warnings
- [ ] Tests: 100% pass rate
- [ ] Code ready for Phase 4 review

---

## 🚀 Execution Timeline

| Step | Time | Owner |
|------|------|-------|
| **1. Understand Current Code** | 15 min | @backend-developer |
| **2. Apply ReturnConstants** | 30 min | @backend-developer |
| **3. Extract Validation Logic** | 20 min | @backend-developer |
| **4. Build & Test** | 10 min | @backend-developer |
| **5. Code Review** | 15 min | @tech-lead |
| **Total Phase 3** | **90 minutes** | |

---

## 📞 Support & Questions

| Question | Answer | Reference |
|----------|--------|-----------|
| Where are ReturnConstants? | `backend/Domain/Customer/src/Models/ReturnConstants.cs` | ISSUE_53_REFACTORING_LOG.md |
| What about InvoiceConstants? | Already done in Phase 2 | ISSUE_53_PHASE_1_2_COMPLETION.md |
| Should I create new tests? | No - existing tests cover refactored code | ISSUE_53_DEVELOPMENT_PLAN.md |
| Build still has warnings? | Address in Phase 5 (not Phase 3 focus) | ISSUE_53_EXECUTIVE_SUMMARY.md |

---

## ✅ Phase 3 Sign-Off Checklist

**For @backend-developer** (Execution):
- [ ] ReturnManagementService updated with ReturnConstants
- [ ] Magic strings eliminated (0 remaining in updated service)
- [ ] Modern null check patterns applied
- [ ] ValidationHelper created (if duplicates found)
- [ ] Build passes: `dotnet build B2Connect.slnx`
- [ ] Tests pass: `dotnet test B2Connect.slnx -v minimal`
- [ ] Code follows SOLID principles
- [ ] XML documentation complete on new code

**For @tech-lead** (Review):
- [ ] Code quality standards met
- [ ] No breaking changes introduced
- [ ] Performance not degraded
- [ ] Architecture decisions justified
- [ ] Ready for Phase 4 frontend refactoring
- [ ] Approve for merge

---

## 🔄 What Happens After Phase 3?

Once Phase 3 is complete:

1. **Phase 4** (Frontend Refactoring) - 2-3 hours
   - ESLint fixes in Store, Admin, Management frontends
   - Vue 3 pattern modernization
   - TypeScript strict mode compliance

2. **Phase 5** (Testing & Verification) - 1-2 hours
   - Build with 0 warnings target
   - Full test suite 100% pass rate
   - Code review and merge approval

3. **GitHub Issue #53 Creation**
   - Create official issue once PR is ready
   - Link all phases together
   - Track as technical debt completion

---

## 📚 Reference Documentation

- **Development Plan**: [ISSUE_53_DEVELOPMENT_PLAN.md](./ISSUE_53_DEVELOPMENT_PLAN.md)
- **Refactoring Log**: [ISSUE_53_REFACTORING_LOG.md](./ISSUE_53_REFACTORING_LOG.md)
- **Phase 1-2 Summary**: [ISSUE_53_PHASE_1_2_COMPLETION.md](./ISSUE_53_PHASE_1_2_COMPLETION.md)
- **Executive Summary**: [ISSUE_53_EXECUTIVE_SUMMARY.md](./ISSUE_53_EXECUTIVE_SUMMARY.md)
- **Architecture Guide**: [docs/architecture/ONION_ARCHITECTURE.md](./docs/ONION_ARCHITECTURE.md)
- **Code Standards**: [.github/copilot-instructions-backend.md](./.github/copilot-instructions-backend.md)

---

## 🎯 Success Metric

**Phase 3 is complete when**:
- ✅ ReturnManagementService uses ReturnConstants
- ✅ 0 magic strings in updated code
- ✅ Duplicate validation logic extracted
- ✅ `dotnet build` shows 0 errors
- ✅ `dotnet test` shows 100% pass rate
- ✅ Code review approved

**Estimated Time to Complete**: **90 minutes** (9:00 AM - 10:30 AM on 31 Dec)

---

**Ready to begin Phase 3? Let's execute! 🚀**

**Next**: Read through this guide, then execute the refactoring in order. When Phase 3 is complete, phase 4 (Frontend Refactoring) will be triggered automatically.

