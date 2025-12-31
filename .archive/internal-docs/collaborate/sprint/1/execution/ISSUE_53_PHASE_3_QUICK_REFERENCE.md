# 🚀 QUICK REFERENCE: Issue #53 Phase 3 Execution

**Print This Or Keep It Open While Working**

---

## 📋 Phase 3 At a Glance

**Task 1**: Update ReturnManagementService (45 min)  
**Task 2**: Extract validation logic (30 min)  
**Task 3**: Review remaining services (30 min)  
**Total**: 90 minutes

---

## 🎯 Task 1: ReturnManagementService (45 min)

### File Location
```
backend/Domain/Customer/src/Services/ReturnManagementService.cs
```

### Find & Replace 1: Status Constants
```
"Pending"      → ReturnConstants.Status.Pending
"Approved"     → ReturnConstants.Status.Approved
"Rejected"     → ReturnConstants.Status.Rejected
"Processing"   → ReturnConstants.Status.Processing
"Completed"    → ReturnConstants.Status.Completed
"Failed"       → ReturnConstants.Status.Failed
```

### Find & Replace 2: Reason Constants
```
"NoLongerNeeded"  → ReturnConstants.Reason.NoLongerNeeded
"DamageOrDefect"  → ReturnConstants.Reason.DamageOrDefect
"NotAsDescribed"  → ReturnConstants.Reason.NotAsDescribed
"FoundBetter"     → ReturnConstants.Reason.FoundBetter
"ChangedMind"     → ReturnConstants.Reason.ChangedMind
```

### Find & Replace 3: Magic Numbers
```
daysElapsed > 30     → daysElapsed > ReturnConfig.MaxReturnDays
maxAttempts = 3      → maxAttempts = ReturnConfig.MaxLogisticsRetries
```

### Find & Replace 4: Null Checks
```
== null  → is null
!= null  → is not null
```

### Verify Import
```csharp
using B2Connect.Customer.Models;  // For ReturnConstants
```

---

## 🎯 Task 2: Extract Validation (30 min)

### Create New File
**Location**: `backend/Domain/Customer/src/Utilities/ValidationHelper.cs`

**Content**:
```csharp
using System;
using B2Connect.Customer.Models;

namespace B2Connect.Customer.Utilities;

/// <summary>
/// Common validation logic for Customer domain services.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates that a deadline hasn't passed.
    /// </summary>
    public static void ValidateDeadlineNotPassed(DateTime deadline, string fieldName = "Deadline")
    {
        if (DateTime.Now > deadline)
            throw new InvalidOperationException($"{fieldName} has passed.");
    }

    /// <summary>
    /// Validates that an amount is positive.
    /// </summary>
    public static void ValidatePositiveAmount(decimal amount, string fieldName = "Amount")
    {
        if (amount <= 0)
            throw new ArgumentException($"{fieldName} must be positive (actual: {amount}).");
    }

    /// <summary>
    /// Validates enum value is defined.
    /// </summary>
    public static void ValidateEnumDefined<T>(T value, string fieldName = "Value") where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            throw new ArgumentException($"Invalid {fieldName}: {value}");
    }
}
```

### Update Services to Use It
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
ValidationHelper.ValidateDeadlineNotPassed(request.Deadline);
ValidationHelper.ValidatePositiveAmount(request.Amount);
```

---

## 🎯 Task 3: Review Services (30 min)

**Services to scan**:
1. `PaymentService.cs` - Check for magic strings/numbers
2. `RefundService.cs` - Check for duplicate logic
3. `ShippingService.cs` - Check for hardcoded values

**What to look for**:
- [ ] Magic strings (hardcoded text)
- [ ] Magic numbers (hardcoded values)
- [ ] Null check patterns (`== null` vs `is null`)
- [ ] Duplicate validation logic

**If found**: Apply same fixes as Task 1-2

---

## ✅ Verification Steps

```bash
# After each change:
dotnet build B2Connect.slnx
# Should show: 0 errors

dotnet test B2Connect.slnx -v minimal
# Should show: All tests passing (156/156)

# Check for remaining magic strings in ReturnManagementService
grep -n "\"Pending\"\|\"Approved\"\|\"Rejected\"" \
  backend/Domain/Customer/src/Services/ReturnManagementService.cs
# Should show: nothing (empty)

# Check for old null check patterns
grep -n " == null\| != null" \
  backend/Domain/Customer/src/Services/ReturnManagementService.cs
# Should show: nothing (empty)
```

---

## 🔗 Key Files

| File | Purpose | Location |
|------|---------|----------|
| ReturnConstants | Constants to use | `backend/Domain/Customer/src/Models/ReturnConstants.cs` |
| InvoiceConstants | Reference example | `backend/Domain/Customer/src/Models/InvoiceConstants.cs` |
| Phase 3 Guide | Detailed instructions | `ISSUE_53_PHASE_3_EXECUTION_GUIDE.md` |

---

## 🎯 Success Checklist

- [ ] ReturnManagementService updated with ReturnConstants
- [ ] All magic strings replaced (0 remaining in function)
- [ ] All magic numbers replaced
- [ ] Null check patterns modernized (`is null`)
- [ ] ValidationHelper created
- [ ] Services updated to use ValidationHelper
- [ ] Build successful: `0 errors`
- [ ] Tests passing: `156/156`
- [ ] Committed to git with Issue #53 reference

---

## 📝 Commit Message

```
feat(customer): apply ReturnConstants and consolidate validation logic (#53)

- Apply ReturnConstants to ReturnManagementService
- Eliminate 12+ magic strings and magic numbers
- Create ValidationHelper for duplicate logic consolidation
- Modernize null check patterns (is null)
- All tests passing, zero build errors
```

---

## ⚡ Tips

✅ Use multi-line find & replace to apply all changes at once  
✅ Test after each major change (ReturnManagementService, ValidationHelper)  
✅ Keep Phase 3 guide open for reference  
✅ If something breaks, check the git diff to see what changed  

---

## 🆘 Troubleshooting

| Problem | Solution |
|---------|----------|
| Build fails after changes | Check that ReturnConstants import exists |
| Tests fail | Verify all changes preserved logic (use git diff) |
| Can't find magic strings | Try case-insensitive search or check different files |
| Grep command not working | Try IDE's find-in-file instead |

---

## ⏱️ Time Breakdown

| Task | Time | Status |
|------|------|--------|
| Task 1 (ReturnManagement) | 45 min | ← Start here |
| Task 2 (ValidationHelper) | 30 min | After Task 1 |
| Task 3 (Review Services) | 30 min | Optional, can defer |
| **Total** | **90 min** | |

---

## 🚀 Next Phase

After Phase 3 is complete:
1. ✅ Commit your changes
2. ⏳ Move to Phase 4 (Frontend Refactoring - ESLint fixes)
3. 📅 Then Phase 5 (Testing & Verification)

---

**Let's Execute Phase 3! 💪**

**You've got 90 minutes. You can do this!**

---

**Quick Links**:
- Full Guide: [ISSUE_53_PHASE_3_EXECUTION_GUIDE.md](./ISSUE_53_PHASE_3_EXECUTION_GUIDE.md)
- Status: [ISSUE_53_SESSION_COMPLETE_SUMMARY.md](./ISSUE_53_SESSION_COMPLETE_SUMMARY.md)
- Index: [ISSUE_53_DOCUMENTATION_INDEX.md](./ISSUE_53_DOCUMENTATION_INDEX.md)

