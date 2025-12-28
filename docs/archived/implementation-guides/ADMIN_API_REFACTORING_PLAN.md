# Admin API Refactoring Plan

**Status**: Disabled - Requires systematic refactoring  
**Scope**: 266+ compilation errors across all Admin handlers  
**Timeline**: ~4-6 hours for complete systematic fix

---

## Root Cause Analysis

The Admin API has a fundamental mismatch between **entity domain model** and **result DTOs**:

### Problem 1: LocalizedContent vs String Conversion
- **Entity Layer**: `Product.Name` is `LocalizedContent` (stored as JSON with translations)
- **DTO Layer**: `ProductResult.Name` is `string` (single value)
- **Handler Mismatch**: All handlers try to assign `LocalizedContent` directly to `string` properties

```csharp
// WRONG - Cannot convert LocalizedContent to string
var result = new ProductResult(
    product.Id,
    product.Name,  // ❌ LocalizedContent, not string
    product.Price
);

// CORRECT - Extract English translation
var result = new ProductResult(
    product.Id,
    product.Name?.Get("en") ?? string.Empty,  // ✅ Get English value
    product.Price
);
```

### Problem 2: Query/Command Parameter Mismatch
- **Queries**: Now accept `TenantId` as first parameter
- **Controllers**: Still calling with old parameter order
- **Handlers**: Expecting different signatures

Example from ProductQuery:
```csharp
// Interface expects:
public record GetProductQuery(Guid TenantId, Guid ProductId);

// But controller might still call:
new GetProductQuery(productId)  // ❌ Missing TenantId
```

---

## Affected Files & Fix Count

### 1. Product Handlers (`ProductHandlers.cs`) - ~35 fixes needed
**File**: `backend/BoundedContexts/Admin/API/src/Application/Handlers/Products/ProductHandlers.cs`

**Errors to fix**:
- CreateProductHandler: Extract LocalizedContent values in ProductResult creation
- UpdateProductHandler: Same as Create
- All Query Handlers: GetProductHandler, GetProductBySkuHandler, GetAllProductsHandler, GetProductsPagedHandler, GetProductBySlugHandler, GetProductsByCategoryHandler, GetProductsByBrandHandler, GetFeaturedProductsHandler, GetNewProductsHandler, SearchProductsHandler

**Pattern**:
```csharp
// Replace this:
return new ProductResult(
    product.Id,
    product.TenantId,
    product.Name,  // ❌
    product.Sku,
    product.Price,
    product.Description,  // ❌
    ...
);

// With this:
return new ProductResult(
    product.Id,
    product.TenantId ?? Guid.Empty,
    product.Name?.Get("en") ?? string.Empty,  // ✅
    product.Sku,
    product.Price,
    product.Description?.Get("en"),  // ✅
    product.CategoryId,
    product.BrandId,
    product.CreatedAt
);
```

### 2. Category Handlers (`CategoryHandlers.cs`) - ~25 fixes needed
**File**: `backend/BoundedContexts/Admin/API/src/Application/Handlers/Categories/CategoryHandlers.cs`

**Similar fixes required**:
- All handlers returning `CategoryResult`
- Extract from `category.Name` (LocalizedContent) → `category.Name?.Get("en")`
- Extract from `category.Description` → `category.Description?.Get("en")`

### 3. Brand Handlers (`BrandHandlers.cs`) - ~20 fixes needed
**File**: `backend/BoundedContexts/Admin/API/src/Application/Handlers/Brands/BrandHandlers.cs`

**Additional issue**: Brand entity missing `Logo` property
- Check [Brand.cs](backend/BoundedContexts/Admin/API/src/Core/Entities/Brand.cs)
- Add `public string? Logo { get; set; }` if missing

### 4. Controllers - ~30 fixes needed
**Files**:
- `ProductsController.cs`
- `CategoriesController.cs`
- `BrandsController.cs`

**Pattern**: Pass TenantId to all query/command creations
```csharp
// WRONG
var query = new GetProductQuery(productId);

// CORRECT
var tenantId = GetTenantId();
var query = new GetProductQuery(tenantId, productId);
```

### 5. DTOs (`RequestDtos.cs`) - ~5-10 fixes needed
**File**: `backend/BoundedContexts/Admin/API/src/Application/DTOs/RequestDtos.cs`

- Ensure CreateProductRequest, UpdateProductRequest have correct properties
- Add missing properties like `CategoryId`, `BrandId`
- Verify all DTO properties align with entity properties

### 6. Result Records - ~5 fixes
**File**: `backend/BoundedContexts/Admin/API/src/Application/Commands/Products/ProductCommands.cs`

- Verify `ProductResult`, `CategoryResult`, `BrandResult` have correct string field types
- Ensure all properties are present and nullable where appropriate

---

## Step-by-Step Fix Procedure

### Phase 1: Verify Entity Models (30 min)
1. Check `backend/BoundedContexts/Admin/API/src/Core/Entities/Product.cs`
   - Confirm Name, Description are `LocalizedContent`
   - Confirm CategoryId, BrandId properties exist
2. Check `Brand.cs` - add `Logo` if missing
3. Check `Category.cs` - verify all properties

### Phase 2: Fix Handlers (2-3 hours)
1. **ProductHandlers.cs**:
   - Open file
   - Find all lines creating `ProductResult(...)`
   - Replace Name, Description with `.Get("en")` calls
   - Handle nullable TenantId with `?? Guid.Empty`

2. **CategoryHandlers.cs**: Same pattern as Product

3. **BrandHandlers.cs**: Same pattern + add Logo property reference

### Phase 3: Fix Controllers (1 hour)
1. **ProductsController.cs**:
   - Add `var tenantId = GetTenantId();` at top of each handler
   - Pass tenantId as first parameter to all query/command creations
   
2. **CategoriesController.cs**: Same pattern

3. **BrandsController.cs**: Same pattern

### Phase 4: Verify DTOs (30 min)
1. Check `RequestDtos.cs` properties match entity properties
2. Ensure CategoryId, BrandId are present in request DTOs
3. Verify all nullable properties are marked correctly

### Phase 5: Build & Test (1 hour)
1. `dotnet build backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj`
2. Fix any remaining errors
3. Re-enable in Orchestration
4. Test endpoints with curl

---

## Utility Extension Method (Optional)

To reduce code duplication, add this extension to make conversion cleaner:

```csharp
// File: backend/BoundedContexts/Admin/API/src/Application/Extensions/LocalizationExtensions.cs
using B2Connect.Types.Localization;

namespace B2Connect.Admin.Application.Extensions;

public static class LocalizationExtensions
{
    public static string GetEnglish(this LocalizedContent? content)
        => content?.Get("en") ?? string.Empty;

    public static string? GetEnglishOrNull(this LocalizedContent? content)
        => content?.Get("en");
}
```

Then use in handlers:
```csharp
return new ProductResult(
    product.Id,
    product.TenantId ?? Guid.Empty,
    product.Name.GetEnglish(),  // ✅ Cleaner
    ...
);
```

---

## Known Issues to Address

1. **Nullable TenantId**: Some handlers might have `TenantId = Guid.Empty` - verify if OK or use actual tenant from context
2. **ProductCategories Collection**: Entity has many-to-many, but CategoryId is singular - confirm design
3. **DisplayOrder Property**: Not on all entities but referenced in repository queries - verify existence
4. **IsActive Property**: Referenced extensively - confirm on all entities

---

## Next Steps After Fix

1. Enable Admin API in `backend/Orchestration/B2Connect.Orchestration.csproj`
2. Enable frontend-admin in Orchestration Program.cs
3. Run `dotnet run --project backend/Orchestration`
4. Verify Aspire dashboard shows all services
5. Test API endpoints with proper X-Tenant-ID headers

---

## Estimated Effort

| Phase | Time | Status |
|-------|------|--------|
| Entity verification | 30 min | Not started |
| ProductHandlers | 45 min | Not started |
| CategoryHandlers | 30 min | Not started |
| BrandHandlers | 30 min | Not started |
| Controllers | 60 min | Not started |
| DTO verification | 30 min | Not started |
| Build & debug | 60 min | Not started |
| **Total** | **4-5 hours** | **Not started** |

---

## References

- [LocalizedContent Class](backend/shared/kernel/LocalizedContent.cs)
- [Product Entity](backend/BoundedContexts/Admin/API/src/Core/Entities/Product.cs)
- [Repository Interfaces](backend/BoundedContexts/Admin/API/src/Core/Interfaces/)
- [CQRS Pattern Documentation](docs/features/CQRS_WOLVERINE_PATTERN.md)

