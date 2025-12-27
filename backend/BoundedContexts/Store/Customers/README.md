# B2Connect.Store.Customers - Store-spezifischer Customer Context

## 📋 Übersicht

Der **Customers Bounded Context** verwaltet **Store-spezifische** Kundeninteraktionen:

- **ShoppingCart**: Warenkörbe mit Aggregat-Root Verhalten
- **ShoppingCartItem**: Positionen im Warenkorb
- **Bookmark**: Favorisierte/Lesezeichen-Produkte

## 🏗️ Architektur

### Bounded Context Separation
```
Shared/User/         ← User, Profile, Address (zentral)
    ↓ (via IUserRepository)
Store/Customers/     ← ShoppingCart, Bookmark (Store-spezifisch)
```

### Kernprinzipien
- **No Direct Navigation** - ShoppingCart referenziert User nur via `UserId`
- **Injected Dependencies** - Nutzt `IUserRepository` aus Shared/User
- **Clean Separation** - Keine Duplikation, aber auch keine enge Koppelung

### Layers
```
Models/              → ShoppingCart, ShoppingCartItem, Bookmark
Handlers/            → CQRS Commands/Queries (planned)
Infrastructure/      → DbContext, Repositories (planned)
Application/         → Services, DTOs, Validators (planned)
API/                 → REST Endpoints (planned)
```

## 🔄 Kommunikation

### ShoppingCart ↔ User
```csharp
// In Store/Customers
public class ShoppingCart
{
    public Guid UserId { get; set; } // No navigation property!
    // Zugriff zu User via IUserRepository Injection
}
```

### ShoppingCartItem ↔ Product (Catalog)
```csharp
// In Store/Customers
public class ShoppingCartItem
{
    public Guid ProductId { get; set; } // No navigation property!
    // Zugriff zu Product via HTTP-Call zum Catalog-Service
}
```

### Bookmark ↔ User & Product
```csharp
// In Store/Customers
public class Bookmark
{
    public Guid UserId { get; set; }    // Reference nur
    public Guid ProductId { get; set; } // zu IDs
    // Keine direkten Navigations-Properties
}
```

## 🎯 Aggregate Roots
- **ShoppingCart**: Aggregate Root mit Items-Collection
  - Status-based State Machine (Active → Abandoned → Completed)
  - Auto-calculation of totals
  - Guest & registered user support

## 💾 Datenbank

**Separate Database** für Store-spezifische Daten:
```sql
shopping_carts
├── id (UUID)
├── user_id (FK to Shared.User) [kann NULL sein für Guests]
├── tenant_id (UUID)
├── status (Active|Abandoned|CheckoutStarted|Completed|Cancelled)
├── sub_total, tax_amount, shipping_cost, discount_amount
└── timestamps

shopping_cart_items
├── id (UUID)
├── shopping_cart_id (FK)
├── product_id (UUID) [keine FK - Catalog ist separater Service]
├── quantity, unit_price, discount_price
└── timestamps

bookmarks
├── id (UUID)
├── user_id (FK to Shared.User)
├── product_id (UUID) [keine FK - Catalog ist separater Service]
├── status (Active|Removed|Archived)
└── timestamps
```

## 📐 Design Patterns

### Shopping Cart Aggregate Pattern
```csharp
// Command: Add product to cart
var cart = await _cartRepository.GetByUserAsync(userId);
cart.AddItem(productId, quantity, price);  // Domain logic
await _cartRepository.UpdateAsync(cart);
```

### Bookmark Repository Pattern
```csharp
// Query: Get user's bookmarks
var bookmarks = await _bookmarkRepository.GetByUserAsync(userId);
// Load product details from Catalog Service via HTTP
foreach (var bookmark in bookmarks)
{
    bookmark.Product = await _catalogService.GetProductAsync(bookmark.ProductId);
}
```

## ✅ Nächste Schritte

- [ ] Infrastructure/Data/CustomersDbContext.cs
- [ ] Repository Implementations
- [ ] EF Core Entity Type Configurations
- [ ] Database Migrations
- [ ] CQRS Handlers (CreateCart, AddItem, RemoveItem, etc.)
- [ ] Validators (FluentValidation)
- [ ] API Controllers
- [ ] Integration Tests

## 📚 Referenzen

- [Bounded Context Pattern](../../../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- [Shopping Cart Aggregate](https://github.com/microsoft/eShopOnContainers)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)

## 💾 Datenbank

Jeder Aggregate Root hat:
- `TenantId` für Multi-Tenancy
- `CreatedAt`, `UpdatedAt` für Audit Trail
- Status-Enums für State Management

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="EFCore.NamingConventions" />
```

## ✅ Nächste Schritte

- [ ] Infrastructure Layer mit DbContext
- [ ] Repository Interfaces & Implementations
- [ ] CQRS Handlers (CreateCart, AddItem, etc.)
- [ ] Validators (FluentValidation)
- [ ] API Controllers
- [ ] Integration Tests

