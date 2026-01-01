# 🚀 Feature Implementation Patterns

**Audience**: All developers (backend + frontend)  
**Purpose**: Unified workflow for implementing new features end-to-end  
**Scope**: From requirement to deployment

---

## Feature Workflow Overview

```
1. Requirement Analysis
   ↓
2. Backend Design (DDD + Wolverine)
   ↓
3. Backend Implementation
   ↓
4. Frontend Design (Vue3 + Pinia)
   ↓
5. Frontend Implementation
   ↓
6. Integration Testing
   ↓
7. Deployment
```

---

## 1. Requirement Analysis

### Questions to Ask

**Domain**:
- What bounded context does this belong to?
- Is this read-only (Store context) or write (Admin context)?
- Does this affect multiple contexts?

**Data**:
- What data needs to be stored?
- How long is data valid?
- Does data need to be cached?
- Should this be indexed (Elasticsearch)?

**Users**:
- Who can perform this action?
- What role(s) are required?
- Is tenant isolation needed?

**Integration**:
- Does this call other services?
- What events should be published?
- What events should this listen to?

**Performance**:
- How many requests/sec expected?
- What's the acceptable latency?
- Should results be cached?

---

## 2. Backend Design (DDD + Wolverine)

### Step 1: Identify Bounded Context

**Decision Tree**:
```
Write operation?
├─ Yes → Admin Context
│        └─ Use CRUD endpoints
├─ No → Store Context
        └─ Use read-only endpoints
            
Cross-context?
├─ Yes → Shared Context
│        └─ Identity or Tenancy
└─ No → Specific context
```

### Step 2: Define Domain Model (Aggregate Root)

```csharp
namespace B2Connect.Store.Catalog.Core.Entities;

/// <summary>
/// Product aggregate root (DDD)
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    
    // Value objects
    public SKU Sku { get; set; }
    public Money ListPrice { get; set; }
    
    // Collections
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Optimistic locking
    [Timestamp]
    public byte[] RowVersion { get; set; }
    
    // Factory method for creation
    public static Product Create(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("ProductNameRequired", "Product name is required");
        
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = 0
        };
    }
    
    // Business methods
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainException("InvalidPrice", "Price must be greater than 0");
        
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AdjustStock(int quantity)
    {
        if (StockQuantity + quantity < 0)
            throw new DomainException("InsufficientStock", "Not enough stock");
        
        StockQuantity += quantity;
    }
}
```

### Step 3: Define Commands & Responses

```csharp
namespace B2Connect.Store.Catalog.Application.DTOs;

// Command (input)
public class CreateProductCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int InitialStock { get; set; } = 0;
}

// Response (output)
public class CreateProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Step 4: Define Domain Events

```csharp
namespace B2Connect.Store.Catalog.Core.Events;

// Event (published by service, subscribed by others)
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductPriceChangedEvent
{
    public Guid ProductId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

### Step 5: Implement Service (Wolverine Handler)

```csharp
namespace B2Connect.Store.Catalog.Application.Services;

public class CatalogService
{
    private readonly IProductRepository _repo;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<CatalogService> _logger;
    
    public CatalogService(
        IProductRepository repo,
        IMessageBus messageBus,
        ILogger<CatalogService> logger)
    {
        _repo = repo;
        _messageBus = messageBus;
        _logger = logger;
    }
    
    // ✅ Wolverine handler pattern
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(cmd.Name))
            throw new ValidationException(nameof(cmd.Name), "Name is required");
        
        if (cmd.Price <= 0)
            throw new ValidationException(nameof(cmd.Price), "Price must be > 0");
        
        // Check business rules
        var existing = await _repo.GetByNameAsync(cmd.Name, ct);
        if (existing != null)
            throw new DomainException("DuplicateProduct", $"Product '{cmd.Name}' already exists");
        
        // Create aggregate
        var product = Product.Create(cmd.Name, cmd.Description, cmd.Price);
        product.AdjustStock(cmd.InitialStock);
        
        // Persist
        await _repo.AddAsync(product, ct);
        
        // Publish event (Wolverine subscribes automatically)
        await _messageBus.PublishAsync(new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        }, ct);
        
        _logger.LogInformation("Product created: {ProductId}", product.Id);
        
        return new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            CreatedAt = product.CreatedAt
        };
    }
}

// Register service
builder.Services.AddScoped<CatalogService>();
```

### Step 6: Define API Endpoint

```csharp
namespace B2Connect.Store.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/products")
            .WithName("Products")
            .WithOpenApi();
        
        // POST /api/products
        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        
        // GET /api/products/{id}
        group.MapGet("/{id}", GetProduct)
            .WithName("GetProduct")
            .Produces<ProductDetailResponse>()
            .Produces(StatusCodes.Status404NotFound);
        
        // GET /api/products
        group.MapGet("/", ListProducts)
            .WithName("ListProducts")
            .Produces<List<ProductDto>>();
    }
    
    private static async Task<IResult> CreateProduct(
        CreateProductCommand cmd,
        CatalogService service,
        CancellationToken ct)
    {
        var response = await service.CreateProduct(cmd, ct);
        return Results.Created($"/api/products/{response.Id}", response);
    }
    
    private static async Task<IResult> GetProduct(
        Guid id,
        CatalogService service,
        CancellationToken ct)
    {
        var product = await service.GetProductDetail(id, ct);
        if (product == null)
            return Results.NotFound();
        
        return Results.Ok(product);
    }
    
    private static async Task<IResult> ListProducts(
        CatalogService service,
        CancellationToken ct)
    {
        var products = await service.ListProducts(ct);
        return Results.Ok(products);
    }
}

// Register endpoints in Program.cs
app.MapProductEndpoints();
```

---

## 3. Backend Implementation Checklist

- [ ] Domain model created (entities, value objects)
- [ ] Domain events defined
- [ ] Service class created with Wolverine handlers
- [ ] Repository interface & EF Core implementation
- [ ] API endpoints defined (REST)
- [ ] Validation implemented (FluentValidation or inline)
- [ ] Error handling (custom exceptions)
- [ ] Logging added (correlation IDs)
- [ ] Unit tests written (80%+ coverage)
- [ ] Integration tests written
- [ ] Documentation updated (comments)

---

## 4. Frontend Design (Vue3 + Pinia)

### Step 1: Define Store (Pinia)

```typescript
// stores/productStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Product {
  id: string
  name: string
  description: string
  price: number
}

export const useProductStore = defineStore('products', () => {
  // State
  const products = ref<Product[]>([])
  const selectedProduct = ref<Product | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const productCount = computed(() => products.value.length)
  const total = computed(() =>
    products.value.reduce((sum, p) => sum + p.price, 0)
  )

  // Actions
  const fetchProducts = async () => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/products')
      if (!response.ok) throw new Error('Failed to fetch')
      products.value = await response.json()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
    } finally {
      isLoading.value = false
    }
  }

  const createProduct = async (cmd: CreateProductCommand) => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await fetch('/api/products', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cmd)
      })
      
      if (!response.ok) throw new Error('Failed to create')
      
      const newProduct = await response.json()
      products.value.push(newProduct)
      return newProduct
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Unknown error'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    // State
    products,
    selectedProduct,
    isLoading,
    error,
    // Getters
    productCount,
    total,
    // Actions
    fetchProducts,
    createProduct,
  }
})
```

### Step 2: Create Component (Vue3)

```vue
<template>
  <div class="products-page">
    <header>
      <h1>Products</h1>
      <button @click="showCreateForm = true" class="btn-primary">
        Add Product
      </button>
    </header>

    <!-- Create Form -->
    <div v-if="showCreateForm" class="modal">
      <div class="modal-content">
        <h2>Create Product</h2>
        <form @submit.prevent="handleCreate">
          <input v-model="formData.name" placeholder="Product name" />
          <textarea v-model="formData.description" placeholder="Description"></textarea>
          <input v-model.number="formData.price" type="number" placeholder="Price" />
          <input v-model.number="formData.initialStock" type="number" placeholder="Stock" />
          
          <div class="form-actions">
            <button type="submit" :disabled="store.isLoading">Create</button>
            <button type="button" @click="showCreateForm = false">Cancel</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Loading -->
    <p v-if="store.isLoading && !store.products.length" class="loading">Loading...</p>

    <!-- Error -->
    <div v-if="store.error" class="error-message">
      {{ store.error }}
      <button @click="store.fetchProducts">Retry</button>
    </div>

    <!-- Product List -->
    <div class="product-grid">
      <div v-for="product in store.products" :key="product.id" class="product-card">
        <h3>{{ product.name }}</h3>
        <p>{{ product.description }}</p>
        <p class="price">${{ product.price }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useProductStore } from '@/stores/productStore'

const store = useProductStore()
const showCreateForm = ref(false)
const formData = ref({
  name: '',
  description: '',
  price: 0,
  initialStock: 0,
})

const handleCreate = async () => {
  try {
    await store.createProduct(formData.value)
    showCreateForm.value = false
    formData.value = { name: '', description: '', price: 0, initialStock: 0 }
  } catch (error) {
    console.error('Failed to create product:', error)
  }
}

onMounted(() => {
  store.fetchProducts()
})
</script>

<style scoped>
.products-page {
  padding: 2rem;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-top: 2rem;
}

.product-card {
  border: 1px solid #ddd;
  border-radius: 0.5rem;
  padding: 1.5rem;
  transition: all 0.2s;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.price {
  font-size: 1.5rem;
  font-weight: bold;
  color: #0066cc;
}

.modal {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 0.5rem;
  width: 100%;
  max-width: 500px;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

input, textarea {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 0.25rem;
  font-size: 1rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.btn-primary {
  background: #0066cc;
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.25rem;
  cursor: pointer;
  font-weight: 600;
}

.error-message {
  background: #ffebee;
  border: 1px solid #ef5350;
  padding: 1rem;
  border-radius: 0.25rem;
  margin: 1rem 0;
}
</style>
```

---

## 5. Integration Testing

### Backend: Integration Test

```csharp
[Collection("Integration Tests")]
public class CreateProductTests : IAsyncLifetime
{
    private readonly TestApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public CreateProductTests()
    {
        _factory = new TestApplicationFactory();
        _client = _factory.CreateClient();
    }
    
    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
    
    [Fact]
    public async Task CreateProduct_WithValidData_Returns201()
    {
        // Arrange
        var cmd = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            InitialStock = 10
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/products", cmd);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadAsAsync<CreateProductResponse>();
        result.Name.Should().Be("Test Product");
    }
    
    [Fact]
    public async Task CreateProduct_WithInvalidPrice_Returns400()
    {
        // Arrange
        var cmd = new CreateProductCommand
        {
            Name = "Test",
            Price = -10m  // Invalid
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/products", cmd);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

### Frontend: Unit Test (Vitest)

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { useProductStore } from '@/stores/productStore'
import { setActivePinia, createPinia } from 'pinia'

describe('Product Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('should fetch products', async () => {
    const store = useProductStore()
    
    // Mock API
    global.fetch = vi.fn(() =>
      Promise.resolve({
        ok: true,
        json: () => Promise.resolve([
          { id: '1', name: 'Product 1', price: 99 },
        ])
      })
    )
    
    await store.fetchProducts()
    
    expect(store.products).toHaveLength(1)
    expect(store.products[0].name).toBe('Product 1')
  })
})
```

---

## 6. Deployment Checklist

### Backend
- [ ] All tests passing (80%+ coverage)
- [ ] Code reviewed
- [ ] No secrets in code
- [ ] Error handling complete
- [ ] Logging configured
- [ ] Health checks added
- [ ] API documented (OpenAPI)
- [ ] Database migrations created

### Frontend
- [ ] Components tested
- [ ] No console errors
- [ ] Accessibility checked
- [ ] Performance profiled
- [ ] Dark mode works (if applicable)
- [ ] Mobile responsive
- [ ] i18n translations added
- [ ] Build size optimized

### DevOps
- [ ] Aspire configuration updated
- [ ] Environment variables documented
- [ ] CI/CD pipeline passes
- [ ] Staging environment tested
- [ ] Rollback plan ready
- [ ] Monitoring alerts set
- [ ] Documentation updated

---

## Feature Template Summary

```
Feature: Create Product

BACKEND:
✅ Aggregate: Product with Create() factory
✅ Events: ProductCreatedEvent  
✅ Service: CatalogService.CreateProduct()
✅ Endpoint: POST /api/products
✅ Errors: ValidationException, DomainException
✅ Tests: Integration + Unit

FRONTEND:
✅ Store: useProductStore
✅ Component: ProductCreateForm
✅ API: fetch('/api/products', { method: 'POST' })
✅ State: isLoading, error handling
✅ Tests: Store + Component

INTEGRATION:
✅ API call succeeds
✅ Store updates
✅ UI reflects changes
✅ Error messages shown
```

---

## 🎨 Design Development Patterns

### **Design System Architecture (SoftUI)**

#### **Color Palette & Theming**
```typescript
// ✅ DO: Centralized color system with CSS custom properties
// tailwind.config.ts
export default {
  theme: {
    extend: {
      colors: {
        // Primary brand colors
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          500: '#0ea5e9',
          600: '#0284c7',
          900: '#0c4a6e',
        },
        // Semantic colors
        success: '#10b981',
        warning: '#f59e0b',
        error: '#ef4444',
        // SoftUI specific
        soft: {
          shadow: 'rgba(0, 0, 0, 0.1)',
          border: '#e5e7eb',
          surface: '#ffffff',
        }
      },
      boxShadow: {
        'soft': '0 4px 20px rgba(0, 0, 0, 0.08)',
        'soft-lg': '0 10px 40px rgba(0, 0, 0, 0.12)',
      }
    }
  }
}
```

#### **Typography Scale**
```css
/* ✅ DO: Consistent typography system */
:root {
  --font-family-primary: 'Inter', system-ui, sans-serif;
  --font-size-xs: 0.75rem;    /* 12px */
  --font-size-sm: 0.875rem;   /* 14px */
  --font-size-base: 1rem;     /* 16px */
  --font-size-lg: 1.125rem;   /* 18px */
  --font-size-xl: 1.25rem;    /* 20px */
  --font-size-2xl: 1.5rem;    /* 24px */
  --font-size-3xl: 1.875rem;  /* 30px */
  --font-size-4xl: 2.25rem;   /* 36px */
  
  --font-weight-normal: 400;
  --font-weight-medium: 500;
  --font-weight-semibold: 600;
  --font-weight-bold: 700;
  
  --line-height-tight: 1.25;
  --line-height-normal: 1.5;
  --line-height-relaxed: 1.75;
}
```

#### **Component Tokens**
```typescript
// ✅ DO: Design tokens for consistent component styling
export const designTokens = {
  borderRadius: {
    none: '0',
    sm: '0.25rem',    // 4px
    md: '0.375rem',   // 6px
    lg: '0.5rem',     // 8px
    xl: '0.75rem',    // 12px
    full: '9999px',
  },
  spacing: {
    xs: '0.25rem',    // 4px
    sm: '0.5rem',     // 8px
    md: '1rem',       // 16px
    lg: '1.5rem',     // 24px
    xl: '2rem',       // 32px
    '2xl': '3rem',    // 48px
  },
  shadows: {
    soft: '0 4px 20px rgba(0, 0, 0, 0.08)',
    medium: '0 8px 30px rgba(0, 0, 0, 0.12)',
    large: '0 20px 40px rgba(0, 0, 0, 0.15)',
  }
} as const
```

### **Component Design Patterns**

#### **SoftUI Button Component**
```vue
<!-- ✅ DO: SoftUI button with multiple variants -->
<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="handleClick"
  >
    <Spinner v-if="loading" class="w-4 h-4 mr-2" />
    <Icon v-if="icon && !loading" :name="icon" class="w-4 h-4 mr-2" />
    <slot />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface Props {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost'
  size?: 'sm' | 'md' | 'lg'
  icon?: string
  loading?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  loading: false,
  disabled: false
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const buttonClasses = computed(() => {
  const baseClasses = [
    'inline-flex items-center justify-center',
    'font-medium rounded-lg',
    'transition-all duration-200',
    'focus:outline-none focus:ring-2 focus:ring-offset-2',
    'disabled:opacity-50 disabled:cursor-not-allowed',
    'active:scale-95'
  ]

  // Variant styles
  const variantClasses = {
    primary: [
      'bg-primary-500 text-white',
      'hover:bg-primary-600',
      'focus:ring-primary-500',
      'shadow-soft hover:shadow-soft-lg'
    ],
    secondary: [
      'bg-gray-100 text-gray-900',
      'hover:bg-gray-200',
      'focus:ring-gray-500',
      'shadow-soft'
    ],
    outline: [
      'border-2 border-primary-500 text-primary-600',
      'hover:bg-primary-50',
      'focus:ring-primary-500'
    ],
    ghost: [
      'text-primary-600',
      'hover:bg-primary-50',
      'focus:ring-primary-500'
    ]
  }

  // Size styles
  const sizeClasses = {
    sm: ['px-3 py-1.5 text-sm'],
    md: ['px-4 py-2 text-base'],
    lg: ['px-6 py-3 text-lg']
  }

  return [
    ...baseClasses,
    ...variantClasses[props.variant],
    ...sizeClasses[props.size]
  ]
})

const handleClick = (event: MouseEvent) => {
  if (!props.disabled && !props.loading) {
    emit('click', event)
  }
}
</script>
```

#### **SoftUI Card Component**
```vue
<!-- ✅ DO: SoftUI card with flexible content areas -->
<template>
  <div :class="cardClasses">
    <!-- Header -->
    <div v-if="$slots.header" class="card-header">
      <slot name="header" />
    </div>

    <!-- Body -->
    <div class="card-body">
      <slot />
    </div>

    <!-- Footer -->
    <div v-if="$slots.footer" class="card-footer">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface Props {
  variant?: 'default' | 'elevated' | 'outlined'
  padding?: 'none' | 'sm' | 'md' | 'lg'
  hover?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  padding: 'md',
  hover: false
})

const cardClasses = computed(() => {
  const baseClasses = [
    'bg-white rounded-xl',
    'border border-soft-border',
    'transition-all duration-200'
  ]

  // Variant styles
  const variantClasses = {
    default: ['shadow-soft'],
    elevated: ['shadow-soft-lg'],
    outlined: ['shadow-none border-2']
  }

  // Padding styles
  const paddingClasses = {
    none: [],
    sm: ['p-4'],
    md: ['p-6'],
    lg: ['p-8']
  }

  // Hover effects
  const hoverClasses = props.hover ? [
    'hover:shadow-soft-lg',
    'hover:-translate-y-1'
  ] : []

  return [
    ...baseClasses,
    ...variantClasses[props.variant],
    ...paddingClasses[props.padding],
    ...hoverClasses
  ]
})
</script>

<style scoped>
.card-header {
  @apply border-b border-soft-border pb-4 mb-4;
}

.card-body {
  @apply flex-1;
}

.card-footer {
  @apply border-t border-soft-border pt-4 mt-4;
}
</style>
```

#### **Form Field Component**
```vue
<!-- ✅ DO: Accessible form field with validation -->
<template>
  <div class="form-field">
    <label
      v-if="label"
      :for="inputId"
      class="form-label"
      :class="{ 'text-error-500': hasError }"
    >
      {{ label }}
      <span v-if="required" class="text-error-500 ml-1">*</span>
    </label>

    <div class="relative">
      <slot :input-id="inputId" :has-error="hasError" />
    </div>

    <div
      v-if="hasError"
      class="form-error"
      role="alert"
      aria-live="polite"
    >
      {{ errorMessage }}
    </div>

    <div
      v-if="hint && !hasError"
      class="form-hint"
    >
      {{ hint }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, useId } from 'vue'

export interface Props {
  label?: string
  hint?: string
  errorMessage?: string
  required?: boolean
}

const props = defineProps<Props>()

const inputId = useId()
const hasError = computed(() => Boolean(props.errorMessage))
</script>

<style scoped>
.form-field {
  @apply space-y-2;
}

.form-label {
  @apply block text-sm font-medium text-gray-700;
}

.form-error {
  @apply text-sm text-error-600;
}

.form-hint {
  @apply text-sm text-gray-500;
}
</style>
```

### **User Experience Patterns**

#### **Loading States Pattern**
```vue
<!-- ✅ DO: Comprehensive loading states -->
<template>
  <div class="loading-container">
    <!-- Skeleton Loading -->
    <div v-if="loading === 'skeleton'" class="space-y-4">
      <div class="skeleton h-4 w-3/4 rounded"></div>
      <div class="skeleton h-4 w-1/2 rounded"></div>
      <div class="skeleton h-32 w-full rounded"></div>
    </div>

    <!-- Spinner Loading -->
    <div
      v-else-if="loading === 'spinner'"
      class="flex items-center justify-center p-8"
    >
      <Spinner class="w-8 h-8 text-primary-500" />
      <span class="ml-3 text-gray-600">{{ loadingText }}</span>
    </div>

    <!-- Progress Loading -->
    <div v-else-if="loading === 'progress'" class="space-y-2">
      <div class="flex justify-between text-sm">
        <span>{{ progressText }}</span>
        <span>{{ progress }}%</span>
      </div>
      <div class="progress-bar">
        <div
          class="progress-fill"
          :style="{ width: progress + '%' }"
        ></div>
      </div>
    </div>

    <!-- Content -->
    <slot v-else />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface Props {
  loading?: 'skeleton' | 'spinner' | 'progress' | false
  loadingText?: string
  progress?: number
  progressText?: string
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  loadingText: 'Loading...',
  progress: 0,
  progressText: 'Processing...'
})
</script>

<style scoped>
.skeleton {
  @apply bg-gray-200 animate-pulse;
}

.progress-bar {
  @apply w-full bg-gray-200 rounded-full h-2;
}

.progress-fill {
  @apply bg-primary-500 h-2 rounded-full transition-all duration-300;
}
</style>
```

#### **Empty States Pattern**
```vue
<!-- ✅ DO: Informative empty states -->
<template>
  <div class="empty-state">
    <div class="empty-icon">
      <component :is="icon" class="w-16 h-16 text-gray-400" />
    </div>

    <div class="empty-content">
      <h3 class="empty-title">{{ title }}</h3>
      <p class="empty-description">{{ description }}</p>
    </div>

    <div v-if="$slots.actions" class="empty-actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<script setup lang="ts">
export interface Props {
  icon?: string
  title: string
  description: string
}

defineProps<Props>()
</script>

<style scoped>
.empty-state {
  @apply flex flex-col items-center justify-center p-12 text-center;
}

.empty-icon {
  @apply mb-6;
}

.empty-title {
  @apply text-lg font-semibold text-gray-900 mb-2;
}

.empty-description {
  @apply text-gray-600 mb-6 max-w-md;
}

.empty-actions {
  @apply flex gap-3;
}
</style>
```

#### **Error States Pattern**
```vue
<!-- ✅ DO: User-friendly error states -->
<template>
  <div class="error-state">
    <div class="error-icon">
      <ExclamationTriangleIcon class="w-16 h-16 text-error-500" />
    </div>

    <div class="error-content">
      <h3 class="error-title">{{ title }}</h3>
      <p class="error-description">{{ description }}</p>

      <div v-if="showDetails && technicalDetails" class="error-details">
        <details class="error-details-toggle">
          <summary class="cursor-pointer text-sm text-gray-500 hover:text-gray-700">
            Technical Details
          </summary>
          <pre class="mt-2 p-3 bg-gray-100 rounded text-xs overflow-auto">{{ technicalDetails }}</pre>
        </details>
      </div>
    </div>

    <div class="error-actions">
      <Button
        v-if="retryable"
        variant="primary"
        @click="$emit('retry')"
      >
        Try Again
      </Button>

      <Button
        variant="outline"
        @click="$emit('report')"
      >
        Report Issue
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'

export interface Props {
  title: string
  description: string
  technicalDetails?: string
  showDetails?: boolean
  retryable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showDetails: false,
  retryable: false
})

const emit = defineEmits<{
  retry: []
  report: []
}>()
</script>

<style scoped>
.error-state {
  @apply flex flex-col items-center justify-center p-12 text-center max-w-lg mx-auto;
}

.error-icon {
  @apply mb-6;
}

.error-title {
  @apply text-lg font-semibold text-gray-900 mb-2;
}

.error-description {
  @apply text-gray-600 mb-6;
}

.error-details {
  @apply w-full text-left mb-6;
}

.error-actions {
  @apply flex gap-3;
}
</style>
```

### **Responsive Design Patterns**

#### **Mobile-First Grid System**
```vue
<!-- ✅ DO: Responsive grid with mobile-first approach -->
<template>
  <div class="responsive-grid">
    <div
      v-for="(item, index) in items"
      :key="index"
      class="grid-item"
      :class="getGridClasses(item)"
    >
      <slot :item="item" :index="index" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface GridItem {
  size?: {
    mobile?: 1 | 2 | 3 | 4 | 6 | 12
    tablet?: 1 | 2 | 3 | 4 | 6 | 12
    desktop?: 1 | 2 | 3 | 4 | 6 | 12
  }
}

export interface Props {
  items: GridItem[]
  gap?: 'none' | 'sm' | 'md' | 'lg'
  columns?: {
    mobile?: 1 | 2 | 3 | 4
    tablet?: 1 | 2 | 3 | 4 | 6
    desktop?: 1 | 2 | 3 | 4 | 6 | 12
  }
}

const props = withDefaults(defineProps<Props>(), {
  gap: 'md',
  columns: () => ({
    mobile: 1,
    tablet: 2,
    desktop: 3
  })
})

const getGridClasses = (item: GridItem) => {
  const classes = []

  // Mobile (default: full width)
  const mobileSize = item.size?.mobile || 12
  classes.push(`col-span-${mobileSize}`)

  // Tablet
  if (item.size?.tablet) {
    classes.push(`md:col-span-${item.size.tablet}`)
  }

  // Desktop
  if (item.size?.desktop) {
    classes.push(`lg:col-span-${item.size.desktop}`)
  }

  return classes
}
</script>

<style scoped>
.responsive-grid {
  @apply grid gap-4;
  grid-template-columns: repeat(12, minmax(0, 1fr));
}

@media (min-width: 768px) {
  .responsive-grid {
    @apply gap-6;
  }
}

@media (min-width: 1024px) {
  .responsive-grid {
    @apply gap-8;
  }
}
</style>
```

#### **Adaptive Navigation Pattern**
```vue
<!-- ✅ DO: Adaptive navigation for mobile and desktop -->
<template>
  <nav class="adaptive-nav">
    <!-- Mobile Menu Button -->
    <button
      class="mobile-menu-btn md:hidden"
      @click="toggleMobileMenu"
      :aria-expanded="mobileMenuOpen"
      aria-label="Toggle navigation menu"
    >
      <Bars3Icon v-if="!mobileMenuOpen" class="w-6 h-6" />
      <XMarkIcon v-else class="w-6 h-6" />
    </button>

    <!-- Desktop Navigation -->
    <div class="hidden md:flex desktop-nav">
      <NavItem
        v-for="item in navigationItems"
        :key="item.id"
        :item="item"
        :active="activeItem === item.id"
        @click="handleNavClick"
      />
    </div>

    <!-- Mobile Navigation -->
    <div
      v-if="mobileMenuOpen"
      class="mobile-nav md:hidden"
      v-click-outside="closeMobileMenu"
    >
      <NavItem
        v-for="item in navigationItems"
        :key="item.id"
        :item="item"
        :active="activeItem === item.id"
        :mobile="true"
        @click="handleNavClick"
      />
    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Bars3Icon, XMarkIcon } from '@heroicons/vue/24/outline'

export interface NavigationItem {
  id: string
  label: string
  icon?: string
  href?: string
  children?: NavigationItem[]
}

export interface Props {
  navigationItems: NavigationItem[]
  activeItem?: string
}

const props = defineProps<Props>()

const mobileMenuOpen = ref(false)

const toggleMobileMenu = () => {
  mobileMenuOpen.value = !mobileMenuOpen.value
}

const closeMobileMenu = () => {
  mobileMenuOpen.value = false
}

const handleNavClick = (item: NavigationItem) => {
  // Handle navigation logic
  closeMobileMenu()
}
</script>

<style scoped>
.adaptive-nav {
  @apply relative;
}

.mobile-menu-btn {
  @apply p-2 rounded-lg hover:bg-gray-100 transition-colors;
}

.mobile-nav {
  @apply absolute top-full left-0 right-0 mt-2;
  @apply bg-white border border-gray-200 rounded-lg shadow-lg;
  @apply py-2 z-50;
}

.desktop-nav {
  @apply flex items-center space-x-6;
}
</style>
```

### **Accessibility Patterns**

#### **Focus Management**
```typescript
// ✅ DO: Proper focus management for modals and overlays
import { nextTick, ref } from 'vue'

export function useFocusTrap(containerRef: Ref<HTMLElement | null>) {
  const previouslyFocusedElement = ref<Element | null>(null)

  const trapFocus = () => {
    if (!containerRef.value) return

    // Store currently focused element
    previouslyFocusedElement.value = document.activeElement

    // Find all focusable elements within container
    const focusableElements = containerRef.value.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    )

    const firstElement = focusableElements[0] as HTMLElement
    const lastElement = focusableElements[focusableElements.length - 1] as HTMLElement

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Tab') {
        if (e.shiftKey) {
          // Shift + Tab
          if (document.activeElement === firstElement) {
            e.preventDefault()
            lastElement.focus()
          }
        } else {
          // Tab
          if (document.activeElement === lastElement) {
            e.preventDefault()
            firstElement.focus()
          }
        }
      }

      if (e.key === 'Escape') {
        // Handle escape key
        restoreFocus()
      }
    }

    // Add event listener
    containerRef.value.addEventListener('keydown', handleKeyDown)

    // Focus first element
    nextTick(() => {
      firstElement?.focus()
    })

    // Return cleanup function
    return () => {
      containerRef.value?.removeEventListener('keydown', handleKeyDown)
      restoreFocus()
    }
  }

  const restoreFocus = () => {
    if (previouslyFocusedElement.value instanceof HTMLElement) {
      previouslyFocusedElement.value.focus()
    }
  }

  return {
    trapFocus,
    restoreFocus
  }
}
```

#### **Screen Reader Support**
```vue
<!-- ✅ DO: Screen reader friendly components -->
<template>
  <div
    role="status"
    aria-live="polite"
    aria-atomic="true"
    class="sr-only"
  >
    {{ screenReaderText }}
  </div>

  <button
    :aria-label="buttonLabel"
    :aria-describedby="descriptionId"
    :aria-expanded="isExpanded"
    :aria-pressed="isPressed"
  >
    {{ buttonText }}
  </button>

  <div
    v-if="hasDescription"
    :id="descriptionId"
    class="sr-only"
  >
    {{ description }}
  </div>
</template>

<script setup lang="ts">
import { computed, useId } from 'vue'

export interface Props {
  statusMessage?: string
  buttonText: string
  buttonLabel?: string
  description?: string
  isExpanded?: boolean
  isPressed?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isExpanded: false,
  isPressed: false
})

const descriptionId = useId()

const screenReaderText = computed(() => {
  // Provide context for screen readers
  return props.statusMessage || ''
})

const hasDescription = computed(() => Boolean(props.description))
</script>
```

### **Design-Development Workflow**

#### **Component-Driven Development**
```typescript
// ✅ DO: Component documentation and storybook-style development
import type { Meta, StoryObj } from '@storybook/vue3'

const meta: Meta<typeof SoftButton> = {
  title: 'Components/SoftButton',
  component: SoftButton,
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['primary', 'secondary', 'outline', 'ghost']
    },
    size: {
      control: { type: 'select' },
      options: ['sm', 'md', 'lg']
    },
    loading: { control: 'boolean' },
    disabled: { control: 'boolean' }
  },
  args: {
    variant: 'primary',
    size: 'md'
  }
}

export default meta
type Story = StoryObj<typeof meta>

export const Primary: Story = {
  args: {
    default: 'Click me'
  }
}

export const Loading: Story = {
  args: {
    loading: true,
    default: 'Saving...'
  }
}

export const Disabled: Story = {
  args: {
    disabled: true,
    default: 'Disabled'
  }
}
```

#### **Design Token Management**
```typescript
// ✅ DO: Automated design token generation
import { writeFileSync } from 'fs'
import { designTokens } from './design-tokens'

function generateCSSTokens() {
  const cssVariables = Object.entries(designTokens)
    .map(([category, values]) => {
      if (typeof values === 'object') {
        return Object.entries(values)
          .map(([key, value]) => `  --${category}-${key}: ${value};`)
          .join('\n')
      }
      return `  --${category}: ${values};`
    })
    .join('\n')

  const css = `:root {\n${cssVariables}\n}\n`

  writeFileSync('./src/styles/tokens.css', css)
}

function generateTypeScriptTokens() {
  const ts = `export const designTokens = ${JSON.stringify(designTokens, null, 2)} as const\n`

  writeFileSync('./src/types/design-tokens.ts', ts)
}

// Generate on build
generateCSSTokens()
generateTypeScriptTokens()
```

#### **Design System Checklist**
- [ ] **Color Palette** - Consistent brand colors defined
- [ ] **Typography** - Readable font scale and weights
- [ ] **Spacing** - Consistent spacing tokens
- [ ] **Components** - Reusable component library
- [ ] **Patterns** - Established UX patterns
- [ ] **Accessibility** - WCAG 2.1 AA compliance
- [ ] **Responsive** - Mobile-first responsive design
- [ ] **Dark Mode** - Light/dark theme support
- [ ] **Documentation** - Component documentation
- [ ] **Testing** - Visual regression tests
- [ ] **Maintenance** - Regular design system updates

---

## Coding Guidelines & Best Practices

### 🔧 **Backend (.NET 10 + Wolverine CQRS)**

#### **Domain Layer (Entities & Value Objects)**
```csharp
// ✅ DO: Rich domain model with business logic
public class Product : EntityBase
{
    // Private constructor for EF Core
    private Product() { }
    
    // Public factory method with validation
    public static Product Create(string name, decimal price, string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("ProductNameRequired", "Product name cannot be empty");
        
        if (price <= 0)
            throw new DomainException("InvalidPrice", "Price must be greater than zero");
        
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Price = price,
            Sku = Sku.Create(sku), // Value object
            Status = ProductStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    // Business methods encapsulate domain logic
    public void Publish()
    {
        if (Status != ProductStatus.Draft)
            throw new DomainException("InvalidStatusTransition", "Only draft products can be published");
        
        Status = ProductStatus.Published;
        PublishedAt = DateTime.UtcNow;
        
        // Raise domain event
        AddDomainEvent(new ProductPublishedEvent(Id, Name, Price));
    }
}

// ✅ DO: Value Objects for type safety
public record Sku : ValueObject
{
    public string Value { get; private init; }
    
    private Sku(string value) => Value = value;
    
    public static Sku Create(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SkuRequired", "SKU cannot be empty");
        
        if (!Regex.IsMatch(sku, @"^[A-Z0-9-]{3,20}$"))
            throw new DomainException("InvalidSkuFormat", "SKU must be 3-20 chars, uppercase letters, numbers, and hyphens only");
        
        return new Sku(sku.ToUpperInvariant());
    }
}
```

#### **Application Layer (Commands & Queries)**
```csharp
// ✅ DO: Command with validation attributes
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }
    
    [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
    public decimal Price { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Z0-9-]{3,20}$", ErrorMessage = "SKU must be 3-20 characters, uppercase letters, numbers, and hyphens only")]
    public string Sku { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

// ✅ DO: Wolverine handler with proper error handling
public class CreateProductHandler
{
    public static async Task<Result<ProductDto>> Handle(
        CreateProductCommand command,
        IProductRepository repository,
        IMessageBus bus,
        CancellationToken ct)
    {
        try
        {
            // Check business rules
            var existingProduct = await repository.GetBySkuAsync(command.Sku, ct);
            if (existingProduct != null)
                return Result.Failure<ProductDto>("Product with this SKU already exists");
            
            // Create domain object
            var product = Product.Create(command.Name, command.Price, command.Sku);
            if (!string.IsNullOrEmpty(command.Description))
                product.UpdateDescription(command.Description);
            
            // Persist
            await repository.AddAsync(product, ct);
            await repository.SaveChangesAsync(ct);
            
            // Publish events
            await bus.PublishAsync(new ProductCreatedEvent(product.Id, product.Name, product.Sku), ct);
            
            return Result.Success(product.ToDto());
        }
        catch (DomainException ex)
        {
            return Result.Failure<ProductDto>(ex.Message);
        }
        catch (Exception ex)
        {
            // Log unexpected errors
            return Result.Failure<ProductDto>("An unexpected error occurred while creating the product");
        }
    }
}
```

#### **Infrastructure Layer (EF Core)**
```csharp
// ✅ DO: Repository with specifications
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetPublishedProductsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
}

// ✅ DO: EF Core configuration with indexes
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Sku)
            .HasMaxLength(20)
            .IsRequired();
        
        // Index for SKU lookups
        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasDatabaseName("IX_Products_Sku");
        
        // Index for status filtering
        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Products_Status");
        
        // Optimistic concurrency
        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();
        
        // Value object configuration
        builder.OwnsOne(p => p.Sku, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("Sku")
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}
```

#### **API Layer (Minimal APIs)**
```csharp
// ✅ DO: Minimal API with proper validation and error handling
public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();
        
        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Creates a new product")
            .WithDescription("Creates a new product with the specified details")
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        group.MapGet("/{id}", GetProduct)
            .WithName("GetProduct")
            .WithSummary("Gets a product by ID")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
    
    private static async Task<IResult> CreateProduct(
        CreateProductCommand command,
        IMediator mediator,
        HttpContext context,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        
        if (result.IsFailure)
        {
            return result.Error.Contains("already exists")
                ? Results.Conflict(new ProblemDetails
                {
                    Title = "Product already exists",
                    Detail = result.Error,
                    Status = StatusCodes.Status409Conflict
                })
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Validation failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
        }
        
        var location = $"{context.Request.Path}/{result.Value.Id}";
        return Results.Created(location, result.Value);
    }
}
```

### 🎨 **Frontend (Vue 3 + TypeScript + Pinia)**

#### **Store Pattern (Pinia Composition API)**
```typescript
// ✅ DO: Type-safe Pinia store with proper error handling
import { defineStore } from 'pinia'
import { ref, computed, readonly } from 'vue'
import type { Ref } from 'vue'

export interface Product {
  readonly id: string
  readonly name: string
  readonly sku: string
  readonly price: number
  readonly description?: string
  readonly status: ProductStatus
  readonly createdAt: Date
}

export interface CreateProductCommand {
  name: string
  sku: string
  price: number
  description?: string
}

export const useProductStore = defineStore('products', () => {
  // State with proper typing
  const products = ref<readonly Product[]>([])
  const selectedProduct = ref<Product | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  
  // Computed properties
  const publishedProducts = computed(() =>
    products.value.filter(p => p.status === ProductStatus.Published)
  )
  
  const totalValue = computed(() =>
    products.value.reduce((sum, p) => sum + p.price, 0)
  )
  
  // Actions with proper error handling
  const fetchProducts = async (): Promise<void> => {
    isLoading.value = true
    error.value = null
    
    try {
      const response = await $fetch<readonly Product[]>('/api/products')
      products.value = response
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch products'
      throw err // Re-throw for component handling
    } finally {
      isLoading.value = false
    }
  }
  
  const createProduct = async (command: CreateProductCommand): Promise<Product> => {
    isLoading.value = true
    error.value = null
    
    try {
      const newProduct = await $fetch<Product>('/api/products', {
        method: 'POST',
        body: command
      })
      
      // Optimistically update local state
      products.value = [...products.value, newProduct]
      
      return newProduct
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create product'
      throw err
    } finally {
      isLoading.value = false
    }
  }
  
  const updateProduct = async (id: string, updates: Partial<Product>): Promise<void> => {
    try {
      const updatedProduct = await $fetch<Product>(`/api/products/${id}`, {
        method: 'PUT',
        body: updates
      })
      
      // Update local state
      const index = products.value.findIndex(p => p.id === id)
      if (index !== -1) {
        products.value = [
          ...products.value.slice(0, index),
          updatedProduct,
          ...products.value.slice(index + 1)
        ]
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update product'
      throw err
    }
  }
  
  return {
    // State
    products: readonly(products),
    selectedProduct: readonly(selectedProduct),
    isLoading: readonly(isLoading),
    error: readonly(error),
    
    // Computed
    publishedProducts,
    totalValue,
    
    // Actions
    fetchProducts,
    createProduct,
    updateProduct
  }
})
```

#### **Component Pattern (Composition API)**
```vue
<!-- ✅ DO: Type-safe Vue component with proper separation of concerns -->
<template>
  <div class="product-create-form">
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <FormField v-model="form.name" label="Product Name" required>
          <InputText
            v-model="form.name"
            placeholder="Enter product name"
            :error="errors.name"
            @blur="validateField('name')"
          />
        </FormField>
        
        <FormField v-model="form.sku" label="SKU" required>
          <InputText
            v-model="form.sku"
            placeholder="PRODUCT-001"
            :error="errors.sku"
            @blur="validateField('sku')"
          />
        </FormField>
      </div>
      
      <FormField v-model="form.price" label="Price" required>
        <InputNumber
          v-model="form.price"
          :min="0.01"
          :max="10000"
          :step="0.01"
          prefix="$"
          :error="errors.price"
          @blur="validateField('price')"
        />
      </FormField>
      
      <FormField v-model="form.description" label="Description">
        <Textarea
          v-model="form.description"
          placeholder="Product description (optional)"
          :maxlength="500"
          rows="3"
        />
      </FormField>
      
      <div class="flex justify-end space-x-4">
        <Button
          variant="secondary"
          @click="$emit('cancel')"
          :disabled="isSubmitting"
        >
          Cancel
        </Button>
        
        <Button
          type="submit"
          :loading="isSubmitting"
          :disabled="!isFormValid"
        >
          Create Product
        </Button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useProductStore } from '@/stores/productStore'
import { useFormValidation } from '@/composables/useFormValidation'
import { CreateProductCommand } from '@/types/commands'

// Props & Emits
interface Props {
  initialData?: Partial<CreateProductCommand>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  success: [product: Product]
  cancel: []
}>()

// Store
const productStore = useProductStore()

// Form state
const form = ref<CreateProductCommand>({
  name: props.initialData?.name ?? '',
  sku: props.initialData?.sku ?? '',
  price: props.initialData?.price ?? 0,
  description: props.initialData?.description ?? ''
})

// Validation
const { errors, validateField, validateForm, isFormValid } = useFormValidation({
  name: { required: true, minLength: 2, maxLength: 100 },
  sku: { 
    required: true, 
    pattern: /^[A-Z0-9-]{3,20}$/,
    customMessage: 'SKU must be 3-20 characters, uppercase letters, numbers, and hyphens only'
  },
  price: { required: true, min: 0.01, max: 10000 }
})

// Submission
const isSubmitting = ref(false)

const handleSubmit = async () => {
  if (!validateForm()) return
  
  isSubmitting.value = true
  
  try {
    const product = await productStore.createProduct(form.value)
    emit('success', product)
    
    // Reset form
    form.value = {
      name: '',
      sku: '',
      price: 0,
      description: ''
    }
  } catch (error) {
    // Error is handled by the store
    console.error('Failed to create product:', error)
  } finally {
    isSubmitting.value = false
  }
}

// Watch for initial data changes
watch(() => props.initialData, (newData) => {
  if (newData) {
    form.value = { ...form.value, ...newData }
  }
}, { deep: true })
</script>

<style scoped>
.product-create-form {
  @apply max-w-2xl mx-auto;
}
</style>
```

#### **Composable Pattern**
```typescript
// ✅ DO: Reusable composables for common logic
import { ref, computed, type Ref } from 'vue'

export interface ValidationRule {
  required?: boolean
  minLength?: number
  maxLength?: number
  min?: number
  max?: number
  pattern?: RegExp
  customMessage?: string
}

export interface ValidationRules {
  [key: string]: ValidationRule
}

export function useFormValidation(rules: ValidationRules) {
  const errors = ref<Record<string, string>>({})
  
  const validateField = (field: string): boolean => {
    const value = (form as any)[field]
    const rule = rules[field]
    
    if (!rule) return true
    
    // Required validation
    if (rule.required && (!value || (typeof value === 'string' && !value.trim()))) {
      errors.value[field] = `${field} is required`
      return false
    }
    
    // Skip other validations if field is empty and not required
    if (!value && !rule.required) {
      delete errors.value[field]
      return true
    }
    
    // String validations
    if (typeof value === 'string') {
      if (rule.minLength && value.length < rule.minLength) {
        errors.value[field] = rule.customMessage || `Must be at least ${rule.minLength} characters`
        return false
      }
      
      if (rule.maxLength && value.length > rule.maxLength) {
        errors.value[field] = rule.customMessage || `Must be no more than ${rule.maxLength} characters`
        return false
      }
      
      if (rule.pattern && !rule.pattern.test(value)) {
        errors.value[field] = rule.customMessage || 'Invalid format'
        return false
      }
    }
    
    // Number validations
    if (typeof value === 'number') {
      if (rule.min !== undefined && value < rule.min) {
        errors.value[field] = rule.customMessage || `Must be at least ${rule.min}`
        return false
      }
      
      if (rule.max !== undefined && value > rule.max) {
        errors.value[field] = rule.customMessage || `Must be no more than ${rule.max}`
        return false
      }
    }
    
    delete errors.value[field]
    return true
  }
  
  const validateForm = (): boolean => {
    let isValid = true
    
    Object.keys(rules).forEach(field => {
      if (!validateField(field)) {
        isValid = false
      }
    })
    
    return isValid
  }
  
  const isFormValid = computed(() => {
    return Object.keys(errors.value).length === 0 && validateForm()
  })
  
  return {
    errors: readonly(errors),
    validateField,
    validateForm,
    isFormValid
  }
}
```

### 🧪 **Testing Guidelines**

#### **Backend Testing (xUnit + NSubstitute)**
```csharp
// ✅ DO: Comprehensive unit tests
public class CreateProductHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IMessageBus _messageBus = Substitute.For<IMessageBus>();
    
    [Fact]
    public async Task CreateProduct_WithValidData_ShouldSucceed()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Sku = "TEST-001",
            Price = 29.99m
        };
        
        _repository.GetBySkuAsync(command.Sku, Arg.Any<CancellationToken>())
            .Returns((Product)null);
        
        // Act
        var result = await CreateProductHandler.Handle(
            command, _repository, _messageBus, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Sku.Should().Be(command.Sku);
        
        await _repository.Received(1).AddAsync(
            Arg.Is<Product>(p => p.Name == command.Name), 
            Arg.Any<CancellationToken>());
        
        await _messageBus.Received(1).PublishAsync(
            Arg.Is<ProductCreatedEvent>(e => e.ProductId == result.Value.Id),
            Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand { Sku = "DUPLICATE" };
        var existingProduct = new Product { Sku = Sku.Create("DUPLICATE") };
        
        _repository.GetBySkuAsync(command.Sku, Arg.Any<CancellationToken>())
            .Returns(existingProduct);
        
        // Act
        var result = await CreateProductHandler.Handle(
            command, _repository, _messageBus, CancellationToken.None);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
        
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}

// ✅ DO: Integration tests with TestContainers
[Collection("Database")]
public class ProductRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    private readonly IServiceProvider _services;
    
    public ProductRepositoryIntegrationTests()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:Default"] = _dbContainer.GetConnectionString()
            })
            .Build();
        
        // Setup DI container with real database
        // ... test implementation
    }
    
    [Fact]
    public async Task AddAsync_ShouldPersistProduct()
    {
        // Arrange
        var repository = _services.GetRequiredService<IProductRepository>();
        var product = Product.Create("Integration Test", 19.99m, "INT-001");
        
        // Act
        await repository.AddAsync(product, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);
        
        // Assert
        var retrieved = await repository.GetByIdAsync(product.Id, CancellationToken.None);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be(product.Name);
    }
}
```

#### **Frontend Testing (Vitest + Vue Test Utils)**
```typescript
// ✅ DO: Component tests with proper mocking
import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ProductCreateForm from '@/components/ProductCreateForm.vue'
import { useProductStore } from '@/stores/productStore'

// Mock the store
vi.mock('@/stores/productStore')

describe('ProductCreateForm', () => {
  let mockStore: ReturnType<typeof useProductStore>
  
  beforeEach(() => {
    setActivePinia(createPinia())
    mockStore = useProductStore()
    mockStore.createProduct = vi.fn()
  })
  
  it('should validate required fields', async () => {
    const wrapper = mount(ProductCreateForm)
    
    // Submit empty form
    await wrapper.find('form').trigger('submit.prevent')
    
    // Check for validation errors
    expect(wrapper.text()).toContain('Product name is required')
    expect(wrapper.text()).toContain('SKU is required')
    expect(wrapper.text()).toContain('Price is required')
  })
  
  it('should create product on valid submission', async () => {
    const wrapper = mount(ProductCreateForm)
    
    // Fill form
    await wrapper.find('input[name="name"]').setValue('Test Product')
    await wrapper.find('input[name="sku"]').setValue('TEST-001')
    await wrapper.find('input[name="price"]').setValue('29.99')
    
    // Mock successful creation
    mockStore.createProduct.mockResolvedValue({
      id: '123',
      name: 'Test Product',
      sku: 'TEST-001',
      price: 29.99
    })
    
    // Submit form
    await wrapper.find('form').trigger('submit.prevent')
    
    // Verify store was called
    expect(mockStore.createProduct).toHaveBeenCalledWith({
      name: 'Test Product',
      sku: 'TEST-001',
      price: 29.99
    })
  })
  
  it('should handle creation errors', async () => {
    const wrapper = mount(ProductCreateForm)
    
    // Fill form
    await wrapper.find('input[name="name"]').setValue('Test Product')
    await wrapper.find('input[name="sku"]').setValue('TEST-001')
    await wrapper.find('input[name="price"]').setValue('29.99')
    
    // Mock error
    mockStore.createProduct.mockRejectedValue(new Error('SKU already exists'))
    
    // Submit form
    await wrapper.find('form').trigger('submit.prevent')
    
    // Check for error display
    expect(wrapper.text()).toContain('SKU already exists')
  })
})

// ✅ DO: Store tests
describe('useProductStore', () => {
  it('should fetch products successfully', async () => {
    const mockProducts = [
      { id: '1', name: 'Product 1', sku: 'P1', price: 10 },
      { id: '2', name: 'Product 2', sku: 'P2', price: 20 }
    ]
    
    // Mock $fetch
    global.$fetch = vi.fn().mockResolvedValue(mockProducts)
    
    const store = useProductStore()
    
    await store.fetchProducts()
    
    expect(store.products).toEqual(mockProducts)
    expect(store.isLoading).toBe(false)
    expect(store.error).toBe(null)
  })
})
```

### 📋 **Code Quality Checklist**

#### **Backend Code Quality**
- [ ] **SOLID Principles** befolgt (Single Responsibility, Open/Closed, etc.)
- [ ] **DRY Principle** - Keine Code-Duplikation
- [ ] **Domain Logic** in Domain Layer gekapselt
- [ ] **Validation** sowohl client- als auch server-seitig
- [ ] **Error Handling** mit benutzerdefinierten Exceptions
- [ ] **Logging** mit korrelierenden IDs
- [ ] **Async/Await** durchgängig verwendet
- [ ] **CancellationToken** unterstützt
- [ ] **EF Core** Queries optimiert (N+1 Problem vermieden)
- [ ] **Unit Tests** mit 80%+ Coverage
- [ ] **Integration Tests** für kritische Pfade

#### **Frontend Code Quality**
- [ ] **TypeScript** strikt verwendet (no `any`, proper types)
- [ ] **Composition API** bevorzugt gegenüber Options API
- [ ] **Reaktivität** optimiert (computed, watchers sparsam)
- [ ] **Performance** berücksichtigt (lazy loading, code splitting)
- [ ] **Accessibility** implementiert (ARIA, keyboard navigation)
- [ ] **Responsive Design** mit Tailwind CSS
- [ ] **Error Boundaries** für robuste Fehlerbehandlung
- [ ] **Loading States** und **Error States** behandelt
- [ ] **Form Validation** sowohl client- als auch server-seitig
- [ ] **Component Tests** für alle UI-Komponenten

#### **General Quality**
- [ ] **Security** - Keine Secrets im Code, Input Validation
- [ ] **Performance** - Lazy Loading, Caching, Optimierungen
- [ ] **Documentation** - XML Comments, README Updates
- [ ] **Git History** - Atomic Commits, klare Commit Messages
- [ ] **Code Reviews** - Mindestens 2 Reviewer für komplexe Changes
- [ ] **CI/CD** - Alle Tests passieren, Linting erfolgreich

### 🔄 **Continuous Improvement**

#### **Regular Reviews**
- **Weekly**: Code Quality Metrics überprüfen
- **Monthly**: Architecture Reviews durchführen
- **Quarterly**: Tech Stack Evaluation und Updates planen

#### **Learning & Adaptation**
- **Tech Radar**: Neue Technologien evaluieren
- **Community**: .NET und Vue.js Communities folgen
- **Conferences**: Tech Events besuchen
- **Books**: Domain-Driven Design und Clean Architecture studieren

---

*Coding Guidelines aktualisiert: 1. Januar 2026*  
*@LeadTech - Modern Best Practices für .NET 10 + Vue 3 + TypeScript*
