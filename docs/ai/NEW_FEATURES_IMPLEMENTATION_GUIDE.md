# 🚀 New Features & Implementation Guide

**Generated**: 30. Dezember 2025  
**Purpose**: Detailed guide on new features from updated dependencies and how to use them

---

## Table of Contents

1. [Vite 7.3.0 Features](#vite-73-features)
2. [Vue 3.5.26 Features](#vue-356-features)
3. [Tailwind CSS 4.1.18 Features](#tailwind-css-418-features)
4. [Playwright 1.57.0 Features](#playwright-157-features)
5. [EF Core 10.0 Features](#ef-core-100-features)
6. [TypeScript 5.9 Features](#typescript-59-features)
7. [Implementation Examples](#implementation-examples)

---

## Vite 7.3.0 Features

### Feature 1: Vite Inspect - Performance Profiling

**What**: Built-in performance profiling tool for bundle analysis

**How to Use**:
```typescript
// In vite.config.ts
import inspect from 'vite-plugin-inspect'

export default {
  plugins: [
    inspect(),
    vue()
  ]
}

// Access at: http://localhost:5173/__inspect/
```

**Benefits for B2Connect**:
- Identify slow components in Store and Admin frontends
- Optimize bundle size
- Track dependency bloat

---

### Feature 2: Improved HMR (Hot Module Replacement)

**What**: 50% faster hot module replacement for development

**Performance Gain**:
- Before: 2-3 seconds for component update
- After: 500-1000ms for component update

**Impact on Development**:
```typescript
// Vue component changes are now reflected instantly
// Great for rapid iteration on:
// - ProductPrice component (Issue #30)
// - B2BVatIdInput component (Issue #31)
// - InvoiceDisplay component (Issue #32)
```

---

### Feature 3: Native Web Worker Support

**What**: Built-in support for Web Workers without additional configuration

**Use Case Example**:
```typescript
// backend/Frontend/Store/src/workers/priceCalculation.ts
self.onmessage = (event) => {
  const { price, vatRate } = event.data;
  const vatAmount = price * (vatRate / 100);
  self.postMessage({
    vatAmount,
    total: price + vatAmount
  });
};

// In component: src/components/ProductPrice.vue
import PriceWorker from '../workers/priceCalculation?worker';

const worker = new PriceWorker();
worker.postMessage({
  price: 99.99,
  vatRate: 19
});

worker.onmessage = (event) => {
  console.log('VAT calculation:', event.data.vatAmount);
};
```

**Benefits**:
- Off-thread price calculations (VAT, shipping)
- Better responsive UI
- Parallelized computation

---

### Feature 4: Vite Inspector for Debugging

**What**: Interactive dependency graph and module inspection

```bash
# Run dev server
npm run dev

# Click the Inspector badge in bottom right
# Or navigate to: http://localhost:5173/__inspect/

# See:
# - Module dependency graph
# - Module size breakdown
# - Import/export relationships
# - Dead code analysis
```

---

## Vue 3.5.26 Features

### Feature 1: Improved TypeScript Type Inference

**What**: Better automatic type detection in templates

**Before (3.5.13)**:
```vue
<script setup lang="ts">
interface Props {
  items: Array<{ id: number; name: string }>;
}

const props = defineProps<Props>();
// Type inference sometimes struggled with complex nested types
</script>
```

**After (3.5.26)**:
```vue
<script setup lang="ts">
interface Props {
  items: Array<{ id: number; name: string }>;
}

const props = defineProps<Props>();
// Perfect type inference, even for deeply nested objects
// Better IDE autocomplete
</script>

<template>
  <!-- Full TypeScript support in template -->
  <div v-for="item in props.items" :key="item.id">
    {{ item.name }}
  </div>
</template>
```

**Application to B2Connect**:
- Better type safety in ProductPrice, B2BVatIdInput, InvoiceDisplay components
- Improved developer experience with autocomplete
- Fewer runtime type errors

---

### Feature 2: Enhanced Template Performance

**What**: Optimized reactivity tracking and rendering

**Impact**:
- 5-10% faster re-renders
- Better memory usage
- Reduced garbage collection pauses

**Great For**:
- Product listing pages with 100+ items
- Real-time price calculations
- Invoice data tables

---

## Tailwind CSS 4.1.18 Features

### Feature 1: Container Queries

**What**: Responsive design based on container size, not viewport

**Example - Admin Dashboard**:
```vue
<script setup lang="ts">
// Admin/src/components/StatCard.vue
</script>

<template>
  <div class="@container">
    <!-- Responsive to container width, not viewport -->
    <div class="
      grid grid-cols-1
      @md:grid-cols-2
      @lg:grid-cols-4
    ">
      <div class="@container">
        <div class="@sm:block hidden">
          <!-- Show only in containers wider than 400px -->
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@container (min-width: 640px) {
  .card {
    @apply grid-cols-2;
  }
}
</style>
```

**Benefits**:
- Responsive cards in sidebar without media queries
- Better component reusability
- Cleaner CSS

---

### Feature 2: Dynamic Color Functions

**What**: CSS-native color manipulation

**Example - Theme Switcher**:
```css
/* tailwind.config.js */
@import "tailwindcss";

@layer components {
  .btn-primary {
    @apply px-4 py-2 rounded-lg;
    background-color: var(--color-primary);
    
    &:hover {
      /* Darken on hover - CSS native */
      background-color: color-mix(
        in srgb,
        var(--color-primary) 80%,
        black
      );
    }
  }
  
  .btn-primary:disabled {
    /* Semi-transparent disabled state */
    opacity: 0.5;
  }
}

@layer utilities {
  .text-emphasis {
    @apply font-bold text-lg;
  }
}
```

**Implementation for B2Connect**:
```vue
<!-- Store/src/components/ProductCard.vue -->
<template>
  <div :class="[
    'card',
    priceHighlight && 'ring-2 ring-orange-500'
  ]">
    <!-- Price with dynamic coloring -->
    <div :class="[
      'text-2xl font-bold',
      isDiscounted ? 'text-green-600' : 'text-gray-900'
    ]">
      {{ formatPrice(product.price) }}
    </div>
  </div>
</template>
```

---

### Feature 3: CSS-First Configuration

**What**: No JavaScript config file required (optional)

**Before (3.x)**:
```javascript
// tailwind.config.js (required)
module.exports = {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: '#3498db',
      }
    },
  },
  plugins: [],
}
```

**After (4.1.18)**:
```css
/* Optional: tailwind.config.css */
@import "tailwindcss";

@layer components {
  .btn {
    @apply px-4 py-2 rounded-lg;
  }
}

:root {
  --color-primary: #3498db;
}
```

**Or skip config entirely** if using defaults

---

## Playwright 1.57.0 Features

### Feature 1: WebSocket Testing

**What**: Native WebSocket testing support

**Use Case for B2Connect**:
```typescript
// Frontend/Admin/tests/e2e/websocket.spec.ts
import { test, expect } from '@playwright/test';

test('Real-time price updates via WebSocket', async ({ page }) => {
  let messageReceived = false;
  
  // Listen for WebSocket messages
  page.on('websocket', ws => {
    console.log('WebSocket opened:', ws.url());
    
    ws.on('framereceived', event => {
      const data = JSON.parse(event.payload);
      if (data.type === 'price_update') {
        messageReceived = true;
      }
    });
  });
  
  await page.goto('http://localhost:5173/products');
  
  // Price should update via WebSocket
  await expect(page.locator('[data-testid="price"]'))
    .toContainText('99.99');
  
  expect(messageReceived).toBe(true);
});
```

---

### Feature 2: Network HAR Recording & Replay

**What**: Record HTTP traffic and replay for testing

**Use Case - Offline Testing**:
```typescript
// tests/e2e/checkout.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Checkout Flow with HAR Recording', () => {
  test.beforeAll(async () => {
    // Record API responses
    // context.routeFromHAR('requests.har', { recordMode: 'all' })
  });
  
  test('Complete checkout without network', async ({ page }) => {
    // Create a new context and route from HAR
    const context = await browser.newContext({
      recordVideo: { dir: 'test-results' }
    });
    
    const newPage = await context.newPage();
    
    // Route from recorded HAR file
    await newPage.routeFromHAR('tests/fixtures/checkout.har');
    
    // These requests will be served from HAR file
    await newPage.goto('http://localhost:5173/checkout');
    
    // Test VAT calculation
    await newPage.locator('[data-testid="vat-id"]').fill('DE123456789');
    await newPage.locator('[data-testid="validate-vat"]').click();
    
    // Should work without network (from HAR)
    await expect(newPage.locator('[data-testid="reverse-charge"]'))
      .toBeVisible();
  });
});
```

**Benefits**:
- Test without network dependency
- Consistent test results
- Faster test execution

---

### Feature 3: Accessibility Testing with Axe

**What**: Integrated accessibility testing

**Implementation**:
```typescript
// Frontend/Admin/tests/e2e/accessibility.spec.ts
import { test, expect } from '@playwright/test';

test('Admin Dashboard meets WCAG 2.1 AA standards', async ({ page }) => {
  await page.goto('http://localhost:5174/dashboard');
  
  // Inject axe-core
  const accessibilityScanResults = await page.evaluate(() =>
    new Promise((resolve) => {
      const script = document.createElement('script');
      script.src = 'https://cdnjs.cloudflare.com/ajax/libs/axe-core/4.7.0/axe.min.js';
      script.onload = () => {
        // @ts-ignore
        axe.run((error, results) => {
          if (error) throw error;
          resolve(results);
        });
      };
      document.head.appendChild(script);
    })
  );
  
  // Verify no critical violations
  const violations = accessibilityScanResults.violations || [];
  const criticalViolations = violations.filter(
    v => v.impact === 'critical'
  );
  
  expect(criticalViolations).toHaveLength(0);
  console.log('✅ Accessibility test passed');
});
```

---

## EF Core 10.0 Features

### Feature 1: Temporal Tables (Audit Trail)

**What**: Native database support for time-travel queries

**Implementation for Invoice Audit Trail**:
```csharp
// backend/Domain/Catalog/src/Core/Invoice.cs
namespace B2Connect.Catalog.Core;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public decimal VatAmount { get; set; }
    public bool ReverseChargeApplied { get; set; }
    
    // Temporal columns (auto-managed by EF Core 10)
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

// backend/Domain/Catalog/src/Infrastructure/CatalogDbContext.cs
public class CatalogDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enable temporal table
        modelBuilder.Entity<Invoice>()
            .ToTable(tb => tb.IsTemporal());
        
        base.OnModelCreating(modelBuilder);
    }
}

// Migration
public class AddInvoiceTemporalTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Invoices",
            columns: table => new
            {
                Id = table.Column<Guid>(),
                OrderId = table.Column<Guid>(),
                Amount = table.Column<decimal>(),
                VatAmount = table.Column<decimal>(),
                ReverseChargeApplied = table.Column<bool>(),
                // Temporal columns
                ValidFrom = table.Column<DateTime>(),
                ValidTo = table.Column<DateTime>()
            });
        
        // Enable temporal table in PostgreSQL
        migrationBuilder.Sql(@"
            ALTER TABLE Invoices
            ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            
            ALTER TABLE Invoices
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = InvoicesHistory));
        ");
    }
}

// Query historical data
public class InvoiceService
{
    public async Task<Invoice> GetInvoiceAsOfDateAsync(
        Guid invoiceId, 
        DateTime asOfDate)
    {
        return await _context.Invoices
            .TemporalAsOf(asOfDate)  // <-- Time-travel query!
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }
    
    // Get all changes to invoice
    public async Task<List<Invoice>> GetInvoiceHistoryAsync(Guid invoiceId)
    {
        return await _context.Invoices
            .TemporalAll()  // <-- All versions
            .Where(i => i.Id == invoiceId)
            .OrderBy(i => i.ValidFrom)
            .ToListAsync();
    }
}
```

**Benefits for B2Connect**:
- ✅ Complete audit trail (Issue #32 Invoice Modification)
- ✅ See who changed what and when
- ✅ Revert to previous invoice state
- ✅ Compliance documentation
- ✅ No manual audit logging needed

---

### Feature 2: Complex Properties

**What**: Strongly-typed nested objects without separate tables

**Example - Address Management**:
```csharp
public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    
    // Complex property (not a separate table)
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
}

[ComplexType]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

// Configuration
modelBuilder.Entity<Order>()
    .ComplexProperty(o => o.ShippingAddress)
    .IsRequired();

// Maps to single table with columns:
// ShippingAddress_Street, ShippingAddress_City, etc.
```

---

## TypeScript 5.9 Features

### Feature 1: Satisfies Operator

**What**: Validate type without changing inferred type

**Use Case in B2Connect**:
```typescript
// Define VAT rates config with type checking
const vatRates = {
  DE: 19.0,
  AT: 20.0,
  FR: 20.0,
  IT: 22.0,
  BE: 21.0
} satisfies Record<string, number>;

// Type is inferred as { DE: 19.0, AT: 20.0, ... }
// Not just Record<string, number>
// So autocomplete works: vatRates.DE ✅

// Without satisfies:
const vatRates2: Record<string, number> = { /* ... */ };
// Type is Record<string, number>
// Autocomplete doesn't know about .DE ❌
```

---

### Feature 2: Const Type Parameters

**What**: Generic functions with literal type inference

```typescript
// Group products by category (exact type preserved)
function groupBy<T, const K extends PropertyKey>(
  items: T[],
  keySelector: (item: T) => K
): Record<K, T[]> {
  const result = {} as Record<K, T[]>;
  for (const item of items) {
    const key = keySelector(item);
    (result[key] ??= []).push(item);
  }
  return result;
}

// Usage
const products = [
  { id: 1, category: 'electronics', name: 'Laptop' },
  { id: 2, category: 'clothing', name: 'Shirt' },
  { id: 3, category: 'electronics', name: 'Phone' }
];

const grouped = groupBy(products, p => p.category);

// Type is inferred as:
// {
//   electronics: Product[];
//   clothing: Product[];
// }

// Perfect autocomplete:
grouped.electronics  // ✅ autocomplete knows this key
grouped.xyz          // ❌ TypeScript error (key doesn't exist)
```

---

## Implementation Examples

### Example 1: Product Price Component with Vite 7 Worker

**File**: `Frontend/Store/src/components/ProductPrice.vue`

```vue
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { Product } from '@/types'
import PriceWorkerClass from '@/workers/priceCalculator?worker'

interface Props {
  product: Product
  vatRate?: number
}

withDefaults(defineProps<Props>(), {
  vatRate: 19.0
})

const priceWorker = ref<Worker | null>(null)
const calculatedPrice = ref<number>(0)
const vatAmount = ref<number>(0)
const totalPrice = computed(() => 
  calculatedPrice.value + vatAmount.value
)

onMounted(() => {
  // Vite 7.3 native worker support
  priceWorker.value = new PriceWorkerClass()
  
  priceWorker.value.onmessage = (event) => {
    const { price, vat } = event.data
    calculatedPrice.value = price
    vatAmount.value = vat
  }
})

const updatePrice = () => {
  if (priceWorker.value) {
    priceWorker.value.postMessage({
      basePrice: props.product.basePrice,
      vatRate: props.vatRate,
      currency: props.product.currency
    })
  }
}
</script>

<template>
  <div class="@container">
    <div class="grid @md:grid-cols-2">
      <div class="price-display">
        <span class="@sm:text-lg text-xl font-bold">
          {{ formatPrice(totalPrice) }}
        </span>
        <span class="text-sm text-gray-600">
          incl. VAT {{ vatAmount.toFixed(2) }} EUR
        </span>
      </div>
      <button 
        @click="updatePrice"
        class="btn btn-primary"
      >
        Recalculate
      </button>
    </div>
  </div>
</template>

<style scoped>
.btn-primary {
  @apply px-4 py-2 rounded-lg bg-blue-500 hover:bg-blue-600;
}
</style>
```

**File**: `Frontend/Store/src/workers/priceCalculator.ts`

```typescript
interface PriceMessage {
  basePrice: number
  vatRate: number
  currency: string
}

self.onmessage = (event: MessageEvent<PriceMessage>) => {
  const { basePrice, vatRate, currency } = event.data
  
  // Complex calculation off-thread
  const vatAmount = basePrice * (vatRate / 100)
  const totalPrice = basePrice + vatAmount
  
  self.postMessage({
    price: basePrice,
    vat: vatAmount,
    total: totalPrice,
    currency
  })
}
```

---

### Example 2: Invoice with Temporal Auditing

**File**: `backend/Domain/Catalog/src/Core/InvoiceAggregate.cs`

```csharp
namespace B2Connect.Catalog.Core;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    
    public string InvoiceNumber { get; set; }
    public DateTime IssuedDate { get; set; }
    
    // Amount tracking
    public decimal BaseAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingCost { get; set; }
    
    // Reverse charge
    public bool ReverseChargeApplied { get; set; }
    public string? BuyerVatId { get; set; }
    
    // Status
    public InvoiceStatus Status { get; set; }
    
    // Temporal columns (EF Core 10)
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateTime ValidFrom { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DateTime ValidTo { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    Paid = 3,
    Refunded = 4,
    Cancelled = 5
}
```

**File**: `backend/Domain/Catalog/src/Infrastructure/CatalogDbContext.cs`

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Enable temporal table for audit trail
    modelBuilder.Entity<Invoice>()
        .ToTable(tb => tb.IsTemporal());
    
    // Index for queries
    modelBuilder.Entity<Invoice>()
        .HasIndex(i => new { i.TenantId, i.InvoiceNumber });
}
```

**File**: `backend/Domain/Catalog/src/Application/Services/InvoiceService.cs`

```csharp
public class InvoiceService
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<InvoiceService> _logger;
    
    public async Task<Invoice> GetInvoiceAsync(
        Guid tenantId,
        Guid invoiceId)
    {
        return await _context.Invoices
            .Where(i => i.TenantId == tenantId && i.Id == invoiceId)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Invoice not found");
    }
    
    // EF Core 10: Time-travel query!
    public async Task<Invoice?> GetInvoiceAsOfAsync(
        Guid tenantId,
        Guid invoiceId,
        DateTime asOfDate)
    {
        return await _context.Invoices
            .TemporalAsOf(asOfDate)
            .Where(i => i.TenantId == tenantId && i.Id == invoiceId)
            .FirstOrDefaultAsync();
    }
    
    // Get complete audit trail
    public async Task<List<InvoiceHistory>> GetInvoiceHistoryAsync(
        Guid tenantId,
        Guid invoiceId)
    {
        var history = await _context.Invoices
            .TemporalAll()
            .Where(i => i.TenantId == tenantId && i.Id == invoiceId)
            .Select(i => new InvoiceHistory
            {
                InvoiceNumber = i.InvoiceNumber,
                TotalAmount = i.TotalAmount,
                VatAmount = i.VatAmount,
                ReverseChargeApplied = i.ReverseChargeApplied,
                Status = i.Status,
                ChangedAt = i.ValidFrom,
                ChangedBy = i.ModifiedBy ?? "system"
            })
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();
        
        return history;
    }
    
    public async Task UpdateInvoiceAsync(
        Guid tenantId,
        Guid invoiceId,
        UpdateInvoiceCommand command,
        string userId)
    {
        var invoice = await GetInvoiceAsync(tenantId, invoiceId);
        
        // Apply changes
        invoice.VatAmount = command.VatAmount;
        invoice.TotalAmount = command.TotalAmount;
        invoice.ModifiedAt = DateTime.UtcNow;
        invoice.ModifiedBy = userId;
        
        // EF Core 10 temporal table automatically tracks:
        // - Old record moved to history with ValidTo = now
        // - New record inserted with ValidFrom = now
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Invoice {InvoiceId} updated by {UserId}",
            invoiceId, userId);
    }
}

public class InvoiceHistory
{
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal VatAmount { get; set; }
    public bool ReverseChargeApplied { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; }
}
```

---

## Summary Table: Features to Adopt

| Feature | Package | Benefit | Priority |
|---------|---------|---------|----------|
| Vite Inspect | Vite 7.3 | Performance profiling | Medium |
| HMR 50% faster | Vite 7.3 | Developer experience | High |
| Web Workers | Vite 7.3 | Off-thread calculations | Medium |
| Better types | Vue 3.5.26 | Type safety | Medium |
| Container queries | Tailwind 4.1.18 | Component responsiveness | Medium |
| Temporal tables | EF Core 10 | Complete audit trail | High |
| Complex properties | EF Core 10 | Cleaner data models | Medium |
| WebSocket testing | Playwright 1.57 | Real-time feature testing | Medium |
| Network HAR | Playwright 1.57 | Offline test support | Low |
| Accessibility testing | Playwright 1.57 | WCAG compliance | High |

---

**Document Created**: 30. Dezember 2025  
**Purpose**: Guide developers on new features available in updated dependencies  
**Maintenance**: Update quarterly as new versions release

