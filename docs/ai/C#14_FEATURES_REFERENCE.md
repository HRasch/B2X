# C# 14 Features Reference for B2Connect Agents

**Last Updated**: 30. Dezember 2025  
**Source**: Official Microsoft Documentation  
**Purpose**: AI agent code generation support and pattern reference  
**Target Audience**: All backend, frontend, and AI specialists

---

## 🎯 Quick Navigation

| Feature | Performance | Readability | Use Case | B2Connect Priority |
|---------|-------------|-------------|----------|-------------------|
| [Extension Members](#1-extension-members) | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Service extensions, LINQ enhancements | 🔴 HIGH |
| [field Keyword](#2-field-keyword-backed-properties) | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Property validation, encapsulation | 🔴 HIGH |
| [Implicit Span Conversions](#3-implicit-span-conversions) | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | Performance-critical code, memory ops | 🟡 MEDIUM |
| [Null-Conditional Assignment](#4-null-conditional-assignment) | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Safe property updates, guard clauses | 🔴 HIGH |
| [Lambda Parameter Modifiers](#5-simple-lambda-parameters-with-modifiers) | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | Higher-order functions, callbacks | 🟡 MEDIUM |
| [Partial Members](#6-more-partial-members) | ⭐⭐⭐ | ⭐⭐⭐ | Code generation, partial implementations | 🟡 MEDIUM |
| [Compound Assignment](#7-user-defined-compound-assignment) | ⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | Operator overloading | 🟢 LOW |
| [Unbound Generics nameof](#8-unbound-generic-types-and-nameof) | ⭐⭐⭐ | ⭐⭐⭐⭐ | Reflection, logging, type names | 🟢 LOW |

---

## 1. Extension Members

**Release**: C# 14 (November 2024)  
**Use Case**: Add methods and properties to existing types without inheritance  
**Performance Impact**: Zero overhead (compile-time)  
**Readability**: ⭐⭐⭐⭐⭐ Excellent

### What's New

Extension members allow defining instance **and static** extension members with cleaner syntax than traditional extension methods.

### Instance Extension Members

```csharp
// Define extensions for IEnumerable<TSource>
public static class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        // Extension property:
        public bool IsEmpty => !source.Any();

        // Extension method:
        public IEnumerable<TSource> WhereNotNull() 
            => source.Where(x => x != null);

        // Return count as extension:
        public int CountItems => source.Count();
    }
}

// Usage (called as instance members):
var items = new[] { 1, 2, 3 };
if (!items.IsEmpty)
{
    var filtered = items.WhereNotNull();
    Console.WriteLine($"Count: {items.CountItems}");
}
```

### Static Extension Members

```csharp
public static class EnumerableExtensions
{
    // Static extension members for IEnumerable<TSource>
    extension<TSource>(IEnumerable<TSource>)
    {
        // Static extension method:
        public static IEnumerable<TSource> Combine(
            IEnumerable<TSource> first, 
            IEnumerable<TSource> second) 
            => first.Concat(second);

        // Static extension property:
        public static IEnumerable<TSource> Empty 
            => Enumerable.Empty<TSource>();

        // Static user-defined operator:
        public static IEnumerable<TSource> operator +(
            IEnumerable<TSource> left, 
            IEnumerable<TSource> right) 
            => left.Concat(right);
    }
}

// Usage (called as static members):
var combined = IEnumerable<int>.Combine(list1, list2);
var empty = IEnumerable<int>.Empty;
var result = list1 + list2; // operator
```

### B2Connect Applications

**Catalog Service - Product Extensions**:
```csharp
public static class ProductExtensions
{
    extension<Product>(IEnumerable<Product> products)
    {
        // Check if any products match tenant criteria
        public bool HasActiveTenantProducts 
            => products.Any(p => !p.IsDeleted && p.IsActive);

        // Get high-value products
        public IEnumerable<Product> GetPremium(decimal minPrice)
            => products.Where(p => p.Price >= minPrice);
    }
}

// Usage:
if (allProducts.HasActiveTenantProducts)
{
    var premium = allProducts.GetPremium(1000m);
}
```

### When to Use
✅ Adding domain-specific methods to LINQ results  
✅ Creating fluent interfaces  
✅ Extending third-party types  
❌ Don't use for core business logic (use entities instead)  

---

## 2. field Keyword (Backed Properties)

**Release**: C# 14 (November 2024)  
**Use Case**: Simple property backing without explicit fields  
**Performance Impact**: Zero overhead  
**Readability**: ⭐⭐⭐⭐⭐ Excellent

### What's New

The `field` keyword enables property accessors to reference a compiler-synthesized backing field without declaring it explicitly.

### Before C# 14

```csharp
private string _message;
public string Message
{
    get => _message;
    set => _message = value ?? throw new ArgumentNullException(nameof(value));
}
```

### After C# 14

```csharp
public string Message
{
    get;
    set => field = value ?? throw new ArgumentNullException(nameof(value));
}
```

### Advanced Examples

```csharp
// Full property with validation
public string Email
{
    get;
    set => field = value?.ToLower() ?? throw new ArgumentNullException();
}

// Lazy initialization with field keyword
public List<Order> Orders
{
    get => field ??= [];
    set => field = value;
}

// Computed property with side effect tracking
private int _accessCount;
public string Name
{
    get => field;
    set
    {
        field = value;
        _accessCount++;
    }
}
```

### B2Connect Applications

**Identity Service - Email Validation**:
```csharp
public class User
{
    // Auto-validates email on set
    public string Email
    {
        get;
        set => field = value?.ToLower().Trim() 
            ?? throw new ArgumentNullException(nameof(Email));
    }

    // Encodes password automatically
    public string Password
    {
        get;
        set => field = BCrypt.Net.BCrypt.HashPassword(
            value ?? throw new ArgumentNullException()
        );
    }

    // Timestamps
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
```

**Catalog Service - Price with Tenant Currency**:
```csharp
public class ProductPrice
{
    private Currency _currency;

    public decimal Amount
    {
        get;
        set => field = value >= 0 
            ? value 
            : throw new InvalidOperationException("Price cannot be negative");
    }

    public Currency Currency
    {
        get => _currency;
        set => field = value ?? throw new ArgumentNullException(nameof(Currency));
    }
}
```

### When to Use
✅ Properties with validation  
✅ Lazy initialization (`??=`)  
✅ Auto-transformation (lowercase, trim, encode)  
✅ Access control (read-only, write-only)  
❌ Don't use for complex logic (use methods instead)

---

## 3. Implicit Span Conversions

**Release**: C# 14 (November 2024)  
**Use Case**: High-performance memory operations, zero-copy scenarios  
**Performance Impact**: ⭐⭐⭐⭐⭐ Zero overhead  
**Readability**: ⭐⭐⭐⭐ Good

### What's New

C# 14 provides first-class support for `Span<T>` and `ReadOnlySpan<T>` with implicit conversions and extension method support.

### Key Conversions

```csharp
// Array to Span
int[] array = [1, 2, 3];
Span<int> span = array;  // Implicit

// Span to ReadOnlySpan
Span<int> mutable = [1, 2, 3];
ReadOnlySpan<int> readOnly = mutable;  // Implicit

// String to ReadOnlySpan<char>
string text = "Hello";
ReadOnlySpan<char> chars = text;  // Implicit

// Stackalloc conversion
Span<byte> buffer = stackalloc byte[256];
ProcessBuffer(buffer);  // Works as ReadOnlySpan<byte>
```

### Span as Extension Method Receiver

```csharp
public static class SpanExtensions
{
    // Span can now receive extensions
    extension<T>(Span<T> items)
    {
        public bool Contains(T value) 
            => items.IndexOf(value) >= 0;

        public void Fill(T value)
        {
            for (int i = 0; i < items.Length; i++)
                items[i] = value;
        }
    }
}

// Usage:
Span<int> numbers = [1, 2, 3, 4, 5];
if (numbers.Contains(3))
{
    numbers.Fill(0);  // Clear all
}
```

### B2Connect Applications

**Search Service - Buffer Processing** (Elasticsearch integration):
```csharp
public class SearchBufferProcessor
{
    // Process large result sets with zero-copy
    public async Task<List<Product>> ProcessResults(
        ReadOnlySpan<byte> jsonBuffer)
    {
        // No allocation for intermediate Span
        return ParseProductsFromSpan(jsonBuffer);
    }

    private List<Product> ParseProductsFromSpan(
        ReadOnlySpan<byte> data)
    {
        // Direct memory access without copying
        var products = new List<Product>();
        // ... parsing logic ...
        return products;
    }
}
```

**Catalog Service - Batch Processing**:
```csharp
public class BatchProcessor
{
    public async Task UpdatePrices(ReadOnlySpan<PriceUpdate> updates)
    {
        // No allocation even for large batches
        foreach (var update in updates)
        {
            await _priceService.UpdateAsync(update);
        }
    }

    // Stack-based buffer for small sets (zero heap allocation)
    public void ProcessSmallBatch(Span<Product> items)
    {
        Span<byte> buffer = stackalloc byte[1024];
        // Process with pre-allocated stack buffer
    }
}
```

### When to Use
✅ Processing large datasets (JSON, binary data)  
✅ Performance-critical code (search, indexing)  
✅ Stream processing  
✅ Buffer management  
❌ Don't use for simple property access (use arrays)  
❌ Don't use for long-lived state (Span is stack-only or ref)

---

## 4. Null-Conditional Assignment

**Release**: C# 14 (November 2024)  
**Use Case**: Safe property updates without explicit null checks  
**Performance Impact**: Minimal  
**Readability**: ⭐⭐⭐⭐⭐ Excellent

### What's New

The `?.` and `?[]` operators can now be used on the **left side** of assignments.

### Before C# 14

```csharp
if (customer is not null)
{
    customer.Order = GetCurrentOrder();
}
```

### After C# 14

```csharp
customer?.Order = GetCurrentOrder();

// Compound assignment also works:
customer?.Balance += 50m;
customer?.Items?.Add(newItem);
```

### Key Behavior

```csharp
// Right side evaluated ONLY if left side is not null
customer?.Order = GetExpensiveOrder();  // GetExpensiveOrder() called only if customer != null

// Works with nested nulls
company?.Address?.City = "Berlin";  // Safe through the chain

// Works with indexers
list?[0] = newValue;

// Works with compound assignment
stats?.Count += 1;
stats?.Total *= multiplier;

// ⚠️ Does NOT work with ++ and --
// order?.Quantity++;  // ❌ Compilation error
```

### B2Connect Applications

**Order Management - Safe Property Updates**:
```csharp
public class OrderService
{
    public async Task UpdateOrderAsync(Guid orderId, OrderUpdate update)
    {
        var order = await _repo.GetAsync(orderId);

        // Safe property updates
        order?.Status = update.Status;
        order?.TotalAmount = update.Total;
        order?.LastModified = DateTime.UtcNow;

        // Safe nested updates
        order?.ShippingAddress?.City = update.City;

        // Safe collection operations
        order?.Items?.Clear();
    }
}
```

**Customer Service - Optional Data Updates**:
```csharp
public class CustomerService
{
    public async Task UpdateProfileAsync(Guid customerId, ProfileUpdate dto)
    {
        var customer = await _repo.GetAsync(customerId);

        // All safe - no explicit null checks needed
        customer?.FirstName = dto.FirstName;
        customer?.LastName = dto.LastName;
        customer?.PreferredLanguage = dto.Language;
        customer?.Preferences?.ReceiveNewsletter = dto.Newsletter;

        // Compound assignment
        customer?.LoginCount += 1;
        customer?.TotalPurchaseAmount += dto.Amount;

        await _repo.SaveAsync(customer);
    }
}
```

### When to Use
✅ Optional property updates  
✅ Deep object navigation  
✅ Defensive programming  
✅ Reducing null-check boilerplate  
❌ Don't use if you need to know if assignment happened (use conditional)

---

## 5. Simple Lambda Parameters with Modifiers

**Release**: C# 14 (November 2024)  
**Use Case**: Lambda expressions with ref, out, scoped, in parameters  
**Performance Impact**: Zero overhead  
**Readability**: ⭐⭐⭐⭐ Good

### What's New

Lambda parameters can now have modifiers (`out`, `ref`, `in`, `scoped`, `ref readonly`) without requiring explicit type declarations.

### Before C# 14

```csharp
// Had to specify types with modifiers
TryParse<int> parser = (string text, out int result) 
    => int.TryParse(text, out result);
```

### After C# 14

```csharp
// No type needed for simple parameter modifiers
TryParse<int> parser = (text, out result) 
    => int.TryParse(text, out result);
```

### Real Examples

```csharp
// Using 'out' parameter
delegate bool TryOperation(string input, out string output);
TryOperation op = (input, out output) => 
{
    output = input.ToUpper();
    return true;
};

// Using 'ref' parameter (mutation)
Func<int[], int> sumRef = (ref int[] arr) => arr.Sum();

// Using 'in' parameter (readonly reference)
Func<string, bool> isEmpty = (in ReadOnlySpan<char> text) => text.IsEmpty;

// Using 'scoped' for safety
Action<Span<byte>> process = (scoped Span<byte> buffer) => 
{
    // buffer is guaranteed to be stack-only
};

// Using 'ref readonly'
Func<Vector3, float> magnitude = (ref readonly Vector3 v) => 
    MathF.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
```

### B2Connect Applications

**Validation Handlers - TryParse Pattern**:
```csharp
public class ValidationService
{
    public delegate bool TryValidate<T>(string input, out T result);

    // Simple lambda without type noise
    public TryValidate<Guid> GuidValidator = (input, out result) =>
    {
        return Guid.TryParse(input, out result);
    };

    public TryValidate<decimal> PriceValidator = (input, out result) =>
    {
        var parsed = decimal.TryParse(input, out var price);
        result = parsed ? Math.Round(price, 2) : 0m;
        return parsed;
    };
}
```

**Batch Operations - Ref Parameter Lambda**:
```csharp
public class BatchUpdateService
{
    public async Task ProcessBatch(ref List<Order> orders)
    {
        // Modify collection in-place
        var modifier = (ref List<Order> list) =>
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Status = OrderStatus.Processed;
            }
        };

        modifier(ref orders);
    }
}
```

### When to Use
✅ TryParse delegates  
✅ Mutable callbacks  
✅ Performance-critical ref parameters  
✅ Safe scoped operations  
❌ Don't overuse - clarity matters more than brevity

---

## 6. More Partial Members

**Release**: C# 14 (November 2024)  
**Use Case**: Code generation, separation of generated vs. manual code  
**Performance Impact**: Zero overhead  
**Readability**: ⭐⭐⭐⭐ Good

### What's New

Constructors and events can now be declared as partial members.

### Partial Constructors

```csharp
public partial class Product
{
    // Defining declaration (no implementation)
    public partial Product(string sku, string name);

    // Implementing declaration (has body)
    public partial Product(string sku, string name)
    {
        Sku = sku;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}
```

### Partial Events

```csharp
public partial class OrderNotifier
{
    // Defining declaration
    public partial event EventHandler<OrderEventArgs> OrderCreated;

    // Implementing declaration
    public partial event EventHandler<OrderEventArgs> OrderCreated
    {
        add => _orderCreatedHandlers += value;
        remove => _orderCreatedHandlers -= value;
    }
}
```

### B2Connect Applications

**Auto-Generated Entity Constructors**:
```csharp
// File: Product.generated.cs (auto-generated)
public partial class Product
{
    public partial Product(string sku, string name, decimal price);
}

// File: Product.cs (hand-written)
public partial class Product
{
    public partial Product(string sku, string name, decimal price)
    {
        Sku = sku.ToUpper();
        Name = name.Trim();
        Price = Math.Max(price, 0);
        CreatedAt = DateTime.UtcNow;
        Id = Guid.NewGuid();
    }

    // Domain logic
    public void ApplyDiscount(decimal percentage) { }
}
```

**Partial Events for Domain Events**:
```csharp
// File: Order.generated.cs (auto-generated)
public partial class Order
{
    public partial event EventHandler<OrderEventArgs> OrderConfirmed;
    public partial event EventHandler<OrderEventArgs> OrderShipped;
}

// File: Order.cs (hand-written)
public partial class Order
{
    public partial event EventHandler<OrderEventArgs> OrderConfirmed
    {
        add => _orderConfirmedHandlers += value;
        remove => _orderConfirmedHandlers -= value;
    }

    public partial event EventHandler<OrderEventArgs> OrderShipped
    {
        add => _orderShippedHandlers += value;
        remove => _orderShippedHandlers -= value;
    }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        OrderConfirmed?.Invoke(this, new(Id));
    }
}
```

### When to Use
✅ Code generators (SourceGen)  
✅ Entity scaffolding  
✅ Event declarations  
✅ Separating generated from manual code  
❌ Don't use just to split code (use classes instead)

---

## 7. User-Defined Compound Assignment

**Release**: C# 14 (November 2024)  
**Use Case**: Operator overloading with compound operators  
**Performance Impact**: Zero overhead  
**Readability**: ⭐⭐⭐⭐ Good

### What's New

You can now overload compound assignment operators (`+=`, `-=`, `*=`, `/=`, `%=`, etc.) by implementing `operator checked +`, etc.

### Example: Money Type

```csharp
public struct Money
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }

    // Basic operator
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Currency mismatch");
        return new Money { Amount = left.Amount + right.Amount, Currency = left.Currency };
    }

    // Compound assignment (automatically uses +)
    // money1 += money2;  // Now works!
}
```

### B2Connect Applications

**Price Calculations**:
```csharp
public readonly struct Price
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public static Price operator +(Price left, Price right)
    {
        ValidateCurrency(left, right);
        return new(left.Amount + right.Amount, left.Currency);
    }

    public static Price operator *(Price price, decimal multiplier)
    {
        return new(price.Amount * multiplier, price.Currency);
    }
}

// Usage - now more natural
var basePrice = new Price(100m, Currency.EUR);
var tax = new Price(19m, Currency.EUR);
basePrice += tax;  // Works!

var total = basePrice * 1.1m;  // Also works!
```

### When to Use
✅ Value types with arithmetic operations  
✅ Custom numeric types  
✅ DSL implementations  
❌ Don't overload operators frivolously

---

## 8. Unbound Generic Types and nameof

**Release**: C# 14 (November 2024)  
**Use Case**: Reflection, logging, generic type names  
**Performance Impact**: Minimal  
**Readability**: ⭐⭐⭐⭐ Good

### What's New

The `nameof` operator now accepts unbound generic types like `List<>`.

### Before C# 14

```csharp
// Only closed generics worked
string name1 = nameof(List<int>);  // "List"

// Unbound generics didn't work
// string name2 = nameof(List<>);  // ❌ Compilation error
```

### After C# 14

```csharp
// Unbound generics now work
string name = nameof(List<>);  // "List"
string dictName = nameof(Dictionary<,>);  // "Dictionary"
```

### B2Connect Applications

**Generic Handler Logging**:
```csharp
public class LoggingInterceptor
{
    public void LogHandlerExecution<TCommand, TResult>(
        TCommand cmd, 
        TResult result)
    {
        var handlerType = nameof(CommandHandler<,>);
        var commandName = nameof(TCommand);
        var resultName = nameof(TResult);

        _logger.LogInformation(
            "Handler {Handler} processed {Command} -> {Result}",
            handlerType, commandName, resultName);
    }
}
```

**Dynamic Service Resolution**:
```csharp
public class ServiceRegistry
{
    public void RegisterGeneric<T>() where T : class
    {
        var genericName = nameof(T);
        _registry[genericName] = typeof(T);
    }

    // Register with unbound generic
    var repoType = nameof(IRepository<>);
    var queryType = nameof(IQuery<>);
}
```

### When to Use
✅ Generic logging  
✅ Type name strings for reflection  
✅ Dependency injection registration  
✅ Compile-time safety for type names  
❌ Don't use for runtime type checking (use typeof instead)

---

## 🚀 B2Connect Implementation Priority

### Phase 1 (Immediate - Next Sprint)

1. **field keyword**: Use in all entity properties with validation
   - User entities (email, password validation)
   - Order entities (amount validation)
   - Product entities (price validation)

2. **Null-conditional assignment**: Replace all guard clause patterns
   - Order updates
   - Customer profile updates
   - Configuration setting

3. **Extension members**: Enhance LINQ queries
   - Product filtering
   - Order querying
   - Collection operations

### Phase 2 (Next 2-3 Sprints)

4. **Lambda parameter modifiers**: TryParse and validation delegates
5. **Implicit Span conversions**: Search and batch operations
6. **Partial members**: Code generation infrastructure

### Phase 3 (Future)

7. **Compound assignment**: Numeric/value type enhancements
8. **Unbound generics nameof**: Logging and reflection

---

## 📊 Adoption Checklist

- [ ] Update `.csproj` files to explicitly reference C# 14
  ```xml
  <PropertyGroup>
    <LangVersion>14.0</LangVersion>
  </PropertyGroup>
  ```

- [ ] Add to copilot instructions for backend developers

- [ ] Update code examples in documentation

- [ ] Conduct team training on top 3 features (field, null-conditional assignment, extension members)

- [ ] Update code review checklist for C# 14 patterns

- [ ] Create linting rules for deprecated patterns (explicit fields, null-checked assignments)

---

## 📚 Official References

- **Microsoft Learn**: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14
- **.NET 10 Release Notes**: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview
- **Language Reference**: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/

---

**Last Updated**: 30. Dezember 2025  
**Created by**: AI Agent (Technical Research)  
**For**: All B2Connect Agents  
**Questions?** Reference this guide or ask @tech-lead
