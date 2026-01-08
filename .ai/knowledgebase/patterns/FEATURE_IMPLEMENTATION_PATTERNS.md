---
docid: KB-146
title: FEATURE_IMPLEMENTATION_PATTERNS
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# 🚀 Feature Implementation Patterns

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
namespace B2X.Store.Catalog.Core.Entities;

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
namespace B2X.Store.Catalog.Application.DTOs;

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
namespace B2X.Store.Catalog.Core.Events;

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
namespace B2X.Store.Catalog.Application.Services;

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
namespace B2X.Store.API.Endpoints;

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

## References

- [Wolverine Pattern Reference](../architecture/WOLVERINE_PATTERN_REFERENCE.md)
- [DDD Bounded Contexts](../architecture/DDD_BOUNDED_CONTEXTS_REFERENCE.md)
- [ERROR_HANDLING_PATTERNS](../INDEX.md)
- [VUE3_COMPOSITION_PATTERNS](VUE3_COMPOSITION_PATTERNS.md)
- [ASPIRE_ORCHESTRATION_REFERENCE](../../../docs/architecture/INDEX.md)

---

*Updated: 30. Dezember 2025*  
*End-to-End feature implementation guide*
