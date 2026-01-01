# C# 6-14 Best Practices (2026)

## Übersicht
Diese Dokumentation enthält aktualisierte Best Practices für C# Versionen 6 bis 14, basierend auf Microsoft-Richtlinien und modernen Entwicklungsstandards. Die Empfehlungen berücksichtigen die neuesten Sprachfeatures und bewährte Praktiken für .NET-Entwicklung.

## Allgemeine Prinzipien

### Codequalität und Wartbarkeit
- **Verwende moderne Sprachfeatures**: Nutze die neuesten C#-Features für besseren Code
- **Konsistente Namensgebung**: Folge PascalCase für Typen/Properties, camelCase für Parameter/Lokale Variablen
- **Vermeide Legacy-Konstrukte**: Ersetze veraltete Patterns durch moderne Alternativen
- **Null-Sicherheit**: Verwende Nullable Reference Types und Null-Conditional Operatoren

### Performance und Effizienz
- **Span<T> und ReadOnlySpan<T>**: Verwende für Zero-Copy Operationen auf Arrays/Strings
- **Async/Await**: Nutze durchgängig für I/O-gebundene Operationen
- **Structs für kleine Daten**: Verwende für kleine, unveränderliche Datenstrukturen
- **Collection Expressions**: Nutze für typsichere Collection-Erstellung

## C# 6 Best Practices

### String Interpolation
```csharp
// ✅ Empfohlen
string message = $"User {user.Name} logged in at {DateTime.Now:HH:mm}";

// ❌ Vermeide
string message = string.Format("User {0} logged in at {1:HH:mm}", user.Name, DateTime.Now);
```

### Expression-Bodied Members
```csharp
// ✅ Empfohlen
public string FullName => $"{FirstName} {LastName}";
public void Validate() => _validator.Validate(this);

// ❌ Vermeide
public string FullName
{
    get { return $"{FirstName} {LastName}"; }
}
```

### Null-Conditional Operator
```csharp
// ✅ Empfohlen
var length = customer?.Name?.Length ?? 0;

// ❌ Vermeide
var length = customer != null && customer.Name != null ? customer.Name.Length : 0;
```

### Auto-Property Initializers
```csharp
// ✅ Empfohlen
public List<Order> Orders { get; } = new List<Order>();

// ❌ Vermeide
private readonly List<Order> _orders = new List<Order>();
public List<Order> Orders => _orders;
```

## C# 7 Best Practices

### Tuples und Deconstruction
```csharp
// ✅ Empfohlen
public (string Name, int Age) GetPerson() => ("John", 30);
var (name, age) = GetPerson();

// ❌ Vermeide
public PersonDto GetPerson() => new PersonDto { Name = "John", Age = 30 };
```

### Pattern Matching
```csharp
// ✅ Empfohlen
if (obj is string s && s.Length > 0)
{
    Console.WriteLine($"String: {s}");
}

// ❌ Vermeide
if (obj is string)
{
    var s = (string)obj;
    if (s.Length > 0)
    {
        Console.WriteLine($"String: {s}");
    }
}
```

### Local Functions
```csharp
// ✅ Empfohlen
public IEnumerable<int> GetEvenNumbers(int[] numbers)
{
    return numbers.Where(IsEven);

    bool IsEven(int n) => n % 2 == 0;
}
```

### Ref Returns und Locals
```csharp
// ✅ Empfohlen
public ref int Find(int[] array, int value)
{
    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] == value) return ref array[i];
    }
    throw new InvalidOperationException();
}
```

## C# 8 Best Practices

### Nullable Reference Types
```csharp
// ✅ Empfohlen
#nullable enable

public class Customer
{
    public string Name { get; set; } = null!; // Non-nullable mit Initialisierung
    public string? MiddleName { get; set; } // Nullable
}
```

### Async Streams
```csharp
// ✅ Empfohlen
public async IAsyncEnumerable<int> GenerateNumbersAsync()
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(100);
        yield return i;
    }
}

// Verwendung
await foreach (var number in GenerateNumbersAsync())
{
    Console.WriteLine(number);
}
```

### Using Declarations
```csharp
// ✅ Empfohlen
using var file = File.OpenRead("data.txt");
// file wird automatisch disposed am Ende des Scopes
```

### Indices und Ranges
```csharp
// ✅ Empfohlen
var lastThree = array[^3..];     // Letzte 3 Elemente
var middle = array[1..^1];       // Alles außer erstes und letztes
var first = array[0];            // Erstes Element
var last = array[^1];            // Letztes Element
```

### Switch Expressions
```csharp
// ✅ Empfohlen
var result = status switch
{
    Status.Active => "Aktiv",
    Status.Inactive => "Inaktiv",
    Status.Pending => "Ausstehend",
    _ => "Unbekannt"
};
```

## C# 9 Best Practices

### Records
```csharp
// ✅ Empfohlen für immutable Daten
public record Person(string FirstName, string LastName)
{
    public string FullName => $"{FirstName} {LastName}";
}

// Mit Vererbung
public record Employee(string FirstName, string LastName, string Department)
    : Person(FirstName, LastName);
```

### Init-Only Properties
```csharp
// ✅ Empfohlen
public class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

// Verwendung
var person = new Person { FirstName = "John", LastName = "Doe" };
// person.FirstName = "Jane"; // Compiler-Fehler
```

### Top-Level Statements
```csharp
// ✅ Empfohlen für kleine Programme/Prototypen
using System;

Console.WriteLine("Hello, World!");
var name = Console.ReadLine();
Console.WriteLine($"Hello, {name}!");
```

### Pattern Matching Enhancements
```csharp
// ✅ Empfohlen
public static decimal CalculateDiscount(object customer) =>
    customer switch
    {
        VIPCustomer vip => vip.MembershipYears switch
        {
            >= 10 => 0.3m,
            >= 5 => 0.2m,
            _ => 0.1m
        },
        RegularCustomer => 0.05m,
        _ => 0m
    };
```

### Covariant Return Types
```csharp
// ✅ Empfohlen
public abstract class Animal
{
    public abstract Food GetFood();
}

public class Dog : Animal
{
    public override DogFood GetFood() => new DogFood();
}
```

## C# 10 Best Practices

### File-Scoped Namespaces
```csharp
// ✅ Empfohlen
namespace MyCompany.MyProject.Domain;

public class Customer
{
    // ...
}
```

### Global Using Directives
```csharp
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
```

### Record Structs
```csharp
// ✅ Empfohlen für kleine, immutable Daten
public readonly record struct Point(double X, double Y)
{
    public double Distance => Math.Sqrt(X * X + Y * Y);
}
```

### Improved Lambda Expressions
```csharp
// ✅ Empfohlen
var parse = (string s) => int.Parse(s);           // Natürlicher Typ
var asyncParse = async (string s) => int.Parse(s); // Attributes erlaubt
```

### Constant Interpolated Strings
```csharp
// ✅ Empfohlen
const string Version = "1.0";
const string FullVersion = $"{Version}.0"; // Jetzt konstant
```

## C# 11 Best Practices

### Raw String Literals
```csharp
// ✅ Empfohlen für Multi-Line Strings
var json = """
    {
        "name": "John",
        "age": 30,
        "city": "New York"
    }
    """;

// Mit Interpolation
var name = "John";
var message = $"""
    Hello {name}!
    Welcome to our application.
    """;
```

### UTF-8 String Literals
```csharp
// ✅ Empfohlen für UTF-8 Daten
ReadOnlySpan<byte> utf8Data = "Hello 🌍"u8;
```

### List Patterns
```csharp
// ✅ Empfohlen für Collection Pattern Matching
public static bool IsValidSequence(int[] numbers) =>
    numbers is [1, 2, .. var middle, 99] && middle.Length > 0;
```

### Generic Math
```csharp
// ✅ Empfohlen für mathematische Algorithmen
public static T Add<T>(T left, T right) where T : INumber<T>
{
    return left + right;
}
```

### Required Members
```csharp
// ✅ Empfohlen für garantierte Initialisierung
public class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
}

// Verwendung
var person = new Person { FirstName = "John", LastName = "Doe" };
```

## C# 12 Best Practices

### Primary Constructors
```csharp
// ✅ Empfohlen für einfache Klassen
public class Customer(string name, string email)
{
    public string Name { get; } = name;
    public string Email { get; } = email;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Name)) throw new ArgumentException(nameof(Name));
        if (string.IsNullOrEmpty(Email)) throw new ArgumentException(nameof(Email));
    }
}
```

### Collection Expressions
```csharp
// ✅ Empfohlen für alle Collection-Typen
int[] numbers = [1, 2, 3, 4, 5];
List<string> names = ["Alice", "Bob", "Charlie"];
Dictionary<string, int> ages = new() { ["Alice"] = 30, ["Bob"] = 25 };
```

### Alias Any Type
```csharp
// ✅ Empfohlen für semantische Klarheit
using Point = (int X, int Y);
using ErrorHandler = Action<Exception>;

Point p = (10, 20);
ErrorHandler handler = ex => Console.WriteLine(ex.Message);
```

### Default Lambda Parameters
```csharp
// ✅ Empfohlen für flexible APIs
var processor = (IEnumerable<int> numbers, int multiplier = 1) =>
    numbers.Select(n => n * multiplier);
```

### Inline Arrays
```csharp
// ✅ Empfohlen für High-Performance Szenarien
[System.Runtime.CompilerServices.InlineArray(10)]
public struct Buffer
{
    private int _element0;
    // Compiler generiert _element1 bis _element9
}
```

## C# 13 Best Practices

### Params Collections
```csharp
// ✅ Empfohlen für flexible Parameter
public void ProcessItems(params ReadOnlySpan<string> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

// Verwendung
ProcessItems("apple", "banana", "cherry");
ProcessItems(["apple", "banana", "cherry"]);
```

### New Lock Type
```csharp
// ✅ Empfohlen für moderne Thread-Synchronisation
private readonly Lock _lock = new();

public void ThreadSafeOperation()
{
    using (_lock.EnterScope())
    {
        // Thread-sicherer Code
    }
}
```

### Ref Struct Interfaces
```csharp
// ✅ Empfohlen für High-Performance APIs
public interface ISpanParsable<T> where T : ISpanParsable<T>
{
    static abstract T Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null);
}

public readonly ref struct SpanParser
{
    // Implementierung...
}
```

### Partial Properties
```csharp
// ✅ Empfohlen für Code-Generierung
public partial class GeneratedClass
{
    // Deklaration
    public partial string GeneratedProperty { get; set; }
}

public partial class GeneratedClass
{
    // Implementierung
    private string _generatedProperty;
    public partial string GeneratedProperty
    {
        get => _generatedProperty;
        set => _generatedProperty = value?.ToUpper();
    }
}
```

### Field Keyword (Preview)
```csharp
// ✅ Empfohlen für Property-Backed Fields
public class Person
{
    public string Name
    {
        get => field ?? "Unknown";
        set => field = value?.Trim();
    }
}
```

## C# 14 Best Practices (Preview)

### Extension Members
```csharp
// ✅ Empfohlen für nahtlose Erweiterungen
public extension StringExtensions for string
{
    public int WordCount => this.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}

// Verwendung
string text = "Hello world from C#";
int words = text.WordCount; // Wie eine native Eigenschaft
```

### Null-Conditional Assignment
```csharp
// ✅ Empfohlen für sichere Zuweisungen
public class Customer
{
    public Address? Address { get; set; }
}

// Verwendung
customer.Address?.City = "New York"; // Compiler-generierte Null-Prüfung
```

### First-Class Span Types
```csharp
// ✅ Empfohlen für verbesserte Type-Inferenz
Span<int> numbers = [1, 2, 3, 4, 5];
ReadOnlySpan<char> text = "Hello";

// Verbesserte Überladungsauflösung
void Process(ReadOnlySpan<int> data) { /* ... */ }
void Process(ReadOnlySpan<char> text) { /* ... */ }
```

## Performance Best Practices

### Span<T> und Memory<T>
```csharp
// ✅ Empfohlen für Zero-Copy Operationen
public void ProcessData(ReadOnlySpan<byte> data)
{
    // Kein Kopieren notwendig
    var text = Encoding.UTF8.GetString(data);
}
```

### Async/Await Patterns
```csharp
// ✅ Empfohlen für skalierbare I/O
public async Task<Customer> GetCustomerAsync(int id)
{
    var customer = await _repository.GetByIdAsync(id);
    var orders = await _orderService.GetOrdersAsync(customer.Id);

    return customer with { Orders = orders };
}
```

### ValueTask für häufige synchrone Ergebnisse
```csharp
// ✅ Empfohlen für High-Performance APIs
public ValueTask<Customer> GetCustomerAsync(int id)
{
    if (_cache.TryGetValue(id, out var customer))
    {
        return ValueTask.FromResult(customer);
    }

    return GetCustomerFromDatabaseAsync(id);
}
```

## Sicherheit und Robustheit

### Input Validation
```csharp
// ✅ Empfohlen
public void ProcessUserInput(string input)
{
    ArgumentException.ThrowIfNullOrEmpty(input);
    ArgumentException.ThrowIfNullOrWhiteSpace(input);

    // Weitere Validierung...
}
```

### Exception Handling

**Bevorzuge das Result-Pattern für erwartete Fehler**: Verwende Result<T> anstelle von Exceptions für erwartete Fehlerfälle, um typsichere und explizite Fehlerbehandlung zu gewährleisten.

```csharp
// ✅ Empfohlen: Result-Pattern für erwartete Fehler
public async Task<Result<User>> CreateUserAsync(CreateUserRequest request)
{
    var validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return Result.Failure<User>(validationResult.Errors);

    try
    {
        var user = new User(request.Name, request.Email);
        await _repository.SaveAsync(user);
        return Result.Success(user);
    }
    catch (DuplicateKeyException ex)
    {
        _logger.LogWarning(ex, "User with email {Email} already exists", request.Email);
        return Result.Failure<User>("User already exists");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error creating user: {Email}", request.Email);
        return Result.Failure<User>("An unexpected error occurred");
    }
}

// ❌ Vermeide: Exceptions für erwartete Fehler
public async Task<User> CreateUserAsync(CreateUserRequest request)
{
    if (string.IsNullOrEmpty(request.Name))
        throw new ValidationException("Name is required");

    var user = new User(request.Name, request.Email);
    await _repository.SaveAsync(user);
    return user;
}
```

**Verwende Exceptions nur für unerwartete Fehler**: Exceptions sollten nur für echte Ausnahmefälle verwendet werden, nicht für erwartete Geschäftslogik-Fehler.

## Testing Best Practices

### Unit Tests mit modernen Features
```csharp
// ✅ Empfohlen
[Theory]
[InlineData("valid@email.com", true)]
[InlineData("invalid-email", false)]
public void EmailValidation_WorksCorrectly(string email, bool expected)
{
    // Arrange & Act
    var result = EmailValidator.IsValid(email);

    // Assert
    Assert.Equal(expected, result);
}
```

## Migration von älteren Versionen

### C# 6 zu 7
- Ersetze manuelle Tuple-Erstellung durch ValueTuple-Syntax
- Verwende Pattern Matching statt Type-Checking und Casting
- Nutze Local Functions für Helper-Methoden

### C# 7 zu 8
- Aktiviere Nullable Reference Types
- Verwende Async Streams für sequentielle asynchrone Daten
- Ersetze manuelle using-Blöcke durch using-Declarations

### C# 8 zu 9
- Konvertiere geeignete Klassen zu Records
- Verwende Top-Level Statements für Konsolen-Apps
- Nutze enhanced Pattern Matching

### C# 9 zu 10
- Verwende File-Scoped Namespaces
- Implementiere Global Usings
- Nutze Record Structs für kleine Datenstrukturen

### C# 10 zu 11
- Verwende Raw String Literals für komplexe Strings
- Implementiere Generic Math für numerische Algorithmen
- Nutze Required Members für garantierte Initialisierung

### C# 11 zu 12
- Verwende Primary Constructors für einfache Klassen
- Ersetze Collection-Initialisierung durch Collection Expressions
- Nutze Alias Any Type für bessere Semantik

### C# 12 zu 13
- Verwende Params Collections für flexible APIs
- Implementiere neue Lock-Type für Thread-Safety
- Nutze Ref Struct Interfaces für Performance

### C# 13 zu 14
- Verwende Extension Members für nahtlose APIs
- Nutze Null-Conditional Assignment für Sicherheit
- Implementiere First-Class Span Types für Performance

## Tools und Analyse

### Code-Analyzer
- Aktiviere alle verfügbaren Roslyn-Analyzer
- Verwende StyleCop für konsistente Formatierung
- Implementiere Custom Analyzer für Domain-spezifische Regeln

### Performance-Monitoring
- Verwende BenchmarkDotNet für Performance-Tests
- Implementiere Memory-Diagnostics für Speicherlecks
- Nutze Application Insights für Produktions-Monitoring

## Fazit

Die Entwicklung von C# 6 zu 14 zeigt einen klaren Trend zu:
- Mehr Sicherheit durch Nullable Types und Pattern Matching
- Besserer Performance durch Span<T>, Records und moderne Collections
- Erhöhte Produktivität durch expressive Syntax und Code-Generierung
- Verbesserter Asynchronität und Parallelität

Halte dich an diese Best Practices, um wartbaren, performanten und sicheren Code zu schreiben. Regelmäßige Überprüfung und Aktualisierung des Codes beim Upgrade auf neuere C#-Versionen ist empfehlenswert.

## Quellen
- Microsoft Learn Dokumentation
- .NET Runtime Coding Guidelines
- C# Language Feature Status
- Framework Design Guidelines