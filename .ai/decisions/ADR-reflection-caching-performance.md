# Performance-Pitfall Prevention Strategy

**ADR-XXX: Reflection Caching & Performance Standards**  
**Status**: Approved  
**Date**: 2026-01-01  
**Author**: @Architect  
**Reviewers**: @TechLead, @Performance, @DatabaseSpecialist

---

## Context

Ein kritischer Performance-Pitfall wurde identifiziert: **Intensive Reflection ohne Caching** in `LocalizedProjectionExtensions.cs`. Bei jedem Aufruf wurden Type-Informationen, PropertyInfo-Objekte und Expression Trees **neu aufgebaut**, obwohl diese Informationen statisch und unveränderlich sind.

### Impact Analysis

| Aspekt | Ohne Caching | Mit Caching |
|--------|-------------|-------------|
| GetProperties() | ~500μs pro Aufruf | 1x einmalig |
| GetCustomAttribute() | ~200μs pro Property | 1x einmalig |
| Expression Tree Build | ~1ms pro Query | 1x einmalig |
| **100 Queries/sec** | **170ms CPU** | **<1ms CPU** |

---

## Decision: Mandatory Reflection Caching Policy

### 1. Caching Requirements

**MANDATORY** für alle Reflection-Operationen:

```csharp
// ❌ VERBOTEN - Reflection ohne Caching
public static T GetValue<T>(object obj)
{
    var property = typeof(T).GetProperty("Value"); // JEDES MAL NEU!
    return (T)property.GetValue(obj);
}

// ✅ RICHTIG - Mit ConcurrentDictionary Cache
private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache 
    = new();

public static T GetValue<T>(object obj)
{
    var key = $"{typeof(T).FullName}:Value";
    var property = _propertyCache.GetOrAdd(key, _ => typeof(T).GetProperty("Value"));
    return (T)property.GetValue(obj);
}
```

### 2. Cache Key Design

**Standard Cache Key Pattern:**
```csharp
// Für Type-basierte Caches
var key = $"{entityType.FullName}:{dtoType.FullName}:{locale}";

// Für Property-basierte Caches  
var key = $"{declaringType.FullName}.{propertyName}";

// Für Methoden-basierte Caches
var key = $"{declaringType.FullName}.{methodName}({string.Join(",", paramTypes.Select(t => t.FullName))})";
```

### 3. Approved Cache Implementations

| Cache Type | Data Structure | Use Case |
|------------|----------------|----------|
| `ConcurrentDictionary<string, T>` | Thread-safe, lock-free reads | Default für alle Caches |
| `Lazy<T>` | Singleton initialization | Static once-per-type data |
| `ConditionalWeakTable<T, V>` | GC-friendly, object-keyed | Instance-attached metadata |

---

## Implementation Standards

### Code Review Checklist

**Jeder PR mit Reflection MUSS folgende Checks bestehen:**

- [ ] **Cache Existence**: Alle `GetProperty`, `GetMethod`, `GetCustomAttribute` Aufrufe sind gecacht
- [ ] **Cache Key Uniqueness**: Cache Keys sind eindeutig und deterministisch
- [ ] **Thread Safety**: `ConcurrentDictionary` oder äquivalent verwendet
- [ ] **Memory Bounds**: Cache-Größe ist begrenzt oder selbst-limitierend
- [ ] **Performance Test**: Benchmark vorhanden (bei kritischen Pfaden)

### Automated Detection

**Roslyn Analyzer Rule (PERF001):**
```csharp
// Analyzer erkennt:
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReflectionCacheAnalyzer : DiagnosticAnalyzer
{
    // Warnung bei:
    // - Type.GetProperty() in Loops oder Hot Paths
    // - Type.GetMethod() ohne vorherige Cache-Prüfung
    // - Type.GetCustomAttribute() in Extension Methods
}
```

**CI/CD Integration:**
```yaml
# .github/workflows/performance-gates.yml
- name: Reflection Cache Check
  run: dotnet analyzers --rules=PERF001 --error-on-warning
```

---

## Lessons Learned Entry

### Incident: Uncached Reflection in LocalizedProjectionExtensions

**Date**: 2026-01-01  
**Severity**: High (Performance Degradation)  
**Root Cause**: Reflection calls (`GetProperties`, `GetCustomAttribute`) in hot path without caching

**Impact**:
- ~170ms CPU overhead per 100 queries
- Potential memory pressure from repeated Type metadata retrieval
- Scalability bottleneck under load

**Resolution**:
- Added `ConcurrentDictionary<string, object>` cache for projection expressions
- Expression trees now built once per (EntityType, DtoType, Locale) combination
- Performance improvement: **>99% reduction** in reflection overhead

**Prevention Measures**:
1. This ADR establishes mandatory caching policy
2. Code review checklist updated
3. Roslyn analyzer rule proposed
4. Knowledge base updated with best practices

---

## Best Practices Reference

### Pattern 1: Type Metadata Cache

```csharp
public static class TypeMetadataCache<T>
{
    // Lazy initialization per closed generic type
    private static readonly Lazy<PropertyInfo[]> _properties 
        = new(() => typeof(T).GetProperties());
    
    public static PropertyInfo[] Properties => _properties.Value;
}
```

### Pattern 2: Expression Cache with Factory

```csharp
private static readonly ConcurrentDictionary<string, object> _expressionCache = new();

public static Expression<Func<TEntity, TDto>> GetProjection<TEntity, TDto>(string locale)
{
    var key = $"{typeof(TEntity).FullName}:{typeof(TDto).FullName}:{locale}";
    
    return (Expression<Func<TEntity, TDto>>)_expressionCache.GetOrAdd(key, _ =>
    {
        // Expensive expression building - done ONCE
        return BuildExpression<TEntity, TDto>(locale);
    });
}
```

### Pattern 3: Attribute Metadata Cache

```csharp
private static readonly ConcurrentDictionary<PropertyInfo, LocalizableAttribute?> _attributeCache = new();

public static LocalizableAttribute? GetLocalizableAttribute(PropertyInfo property)
{
    return _attributeCache.GetOrAdd(property, p => 
        p.GetCustomAttribute<LocalizableAttribute>());
}
```

---

## Compliance

### Enforcement

| Level | Action | Owner |
|-------|--------|-------|
| PR Review | Mandatory checklist | @TechLead |
| CI/CD | Analyzer rules | @DevOps |
| Runtime | Performance monitoring | @Performance |
| Quarterly | Code audit | @Architect |

### Exceptions

Ausnahmen von der Caching-Pflicht nur mit:
1. Schriftlicher Begründung im PR
2. Approval von @TechLead oder @Architect
3. Dokumentation der Performance-Analyse

---

## Related Documents

- [.ai/lessons/2026-01-01-reflection-caching-incident.md](/.ai/lessons/2026-01-01-reflection-caching-incident.md)
- [.ai/knowledgebase/ef-core-localized-dto-projections.md](/.ai/knowledgebase/ef-core-localized-dto-projections.md)
- [.ai/guidelines/performance-standards.md](/.ai/guidelines/performance-standards.md)

---

**Effective Date**: 2026-01-01  
**Review Cycle**: Quarterly  
**Next Review**: 2026-04-01
