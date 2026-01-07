# 🔍 ERP Domain Architecture Review

**Date**: 2. Januar 2026  
**Reviewers**: @Architect, @Enventa  
**Scope**: `backend/Domain/ERP/` Implementation  
**Status**: ✅ APPROVED with Recommendations

---

## Executive Summary

Die ERP-Domain-Implementierung folgt dem ADR-023 Plugin-Architektur-Design und ist **strukturell solide**. Die Actor-Pattern-Implementierung für Thread-Safety ist korrekt umgesetzt. Es gibt einige Verbesserungsmöglichkeiten für Production-Readiness.

**Overall Assessment**: 🟢 **APPROVED** - Implementierung ist ready für weitere Entwicklung

---

## 📊 Review Matrix

| Kategorie | @Architect | @Enventa | Status |
|-----------|-----------|----------|--------|
| Architecture Compliance | ✅ | ✅ | Pass |
| Thread-Safety (Actor) | ✅ | ✅ | Pass |
| ADR-023 Konformität | ✅ | ✅ | Pass |
| Interface Design | ✅ | ⚠️ | Minor Issues |
| Error Handling | ⚠️ | ⚠️ | Needs Improvement |
| enventa ORM Compatibility | N/A | ✅ | Pass (Skeleton) |
| gRPC Proto Design | ✅ | ✅ | Pass |
| Test Coverage | ✅ | ✅ | Pass (27 tests) |

---

## @Architect Review

### ✅ Positiv

#### 1. **Actor Pattern Implementation** - Excellent
```
ErpActor.cs + ErpActorPool.cs + ErpOperation.cs
```
- Channel<T> mit `SingleReader = true` ist korrekt für serialisierte Verarbeitung
- Per-Tenant-Isolation über ActorPool implementiert
- BoundedChannel mit `FullMode.Wait` verhindert Memory-Overflow
- Graceful Shutdown mit Timeout (30s) implementiert

#### 2. **Contract-First Design** - Good
```
Contracts/IErpProvider.cs, IPimProvider.cs, ICrmProvider.cs
```
- Klare Interface-Trennung (PIM/CRM/ERP)
- Bulk-Operationen reduzieren "chatty interfaces"
- Paged-Operationen für .NET 4.8 Kompatibilität
- ProviderResult<T> Pattern für konsistentes Error-Handling

#### 3. **Multi-Deployment Support** - Excellent (ADR-023 Update)
- Docker (Cloud) und Windows Service (On-Premise) unterstützt
- Connector Registration via gRPC für On-Premise
- Hybrid-Szenario ermöglicht (verschiedene Kunden, verschiedene Deployments)

#### 4. **gRPC Service Definitions** - Good
```
Protos/erp_services.proto, connector_registry.proto
```
- Streaming für große Datenmengen
- Heartbeat-Protokoll für On-Premise Monitoring
- Command-Channel für Remote-Steuerung

### ⚠️ Verbesserungsbedarf

#### 1. **Reflection in ErpActor.ProcessSingleOperationAsync** - Technical Debt

**Problem**: Reflection wird verwendet um ExecuteAsync aufzurufen:
```csharp
// Lines 188-210 in ErpActor.cs
var executeMethod = operationType.GetMethod(
    "ExecuteAsync",
    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
```

**Risiko**: 
- Performance-Overhead bei jedem Operation-Call
- Fehleranfällig bei Refactoring
- Kein Compile-Time-Check

**Empfehlung**: Generisches Interface `IErpOperation<TResult>` mit expliziter Execute-Methode:
```csharp
public interface IErpOperation
{
    Guid OperationId { get; }
    Task ExecuteAndCompleteAsync(CancellationToken ct);
}
```

**Priority**: Medium - Funktioniert, aber sollte vor Production refactored werden

---

#### 2. **Missing Circuit Breaker / Retry Policy**

**Problem**: Keine Resilience-Patterns implementiert

**Empfehlung**: Polly-Integration für:
- Circuit Breaker (ERP-Verbindungsprobleme)
- Retry mit Exponential Backoff
- Timeout-Policies

```csharp
// Empfohlene Polly-Policy
private static readonly AsyncCircuitBreakerPolicy _circuitBreaker =
    Policy.Handle<ErpConnectionException>()
          .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
```

**Priority**: High für Production

---

#### 3. **ProviderManager Dispose Pattern** - Incomplete

**Problem**: `ProviderManager.DisposeAsync()` fehlt in der aktuellen Implementierung

**Location**: `Services/ProviderManager.cs`

**Empfehlung**: Vollständige IAsyncDisposable-Implementierung

**Priority**: Medium

---

## @Enventa Review

### ✅ enventa-spezifische Aspekte

#### 1. **Thread-Safety für enventa ORM** - Correct
- Actor serialisiert alle ERP-Operationen pro Tenant ✅
- Kein paralleler Zugriff auf IcECArticle, FSUtil, etc. möglich ✅
- Entspricht den Erkenntnissen aus eGate Broker Analysis ✅

#### 2. **NVIdentity / Login Handling** - Ready for Implementation
- TenantContext enthält alle notwendigen Felder
- Connection-Initialisierung via `ErpActor.InitializeAsync()` vorgesehen
- FSGlobalPool-Pattern aus eGate kann adaptiert werden

#### 3. **Batch Processing** - Correct Approach
- Bulk-APIs definiert (GetProductsAsync, GetCustomersAsync)
- 1000er Batches wie in eGate empfohlen
- Streaming für große Datasets

### ⚠️ enventa-spezifische Empfehlungen

#### 1. **FSUtil.CreateScope() Pattern fehlt**

**Problem**: Transaktions-Scope für enventa nicht modelliert

**enventa Pattern** (aus eGate):
```csharp
using var scope = FSUtil.CreateScope();
foreach (var product in batch) {
    _articleService.Update(MapToArticle(product));
}
scope.Commit();
```

**Empfehlung**: Erweiterung von ErpOperation für Transaktionen:
```csharp
public interface ITransactionalErpOperation : IErpOperation
{
    bool RequiresTransaction { get; }
}
```

**Priority**: High - Notwendig für Write-Operationen

---

#### 2. **enventa Connection Pooling**

**Problem**: Nur Actor-Pool, kein Connection-Pool

**enventa-spezifisch**: FSGlobalPool hält mehrere IFSGlobalObjects bereit

**Empfehlung**: Hybrid-Ansatz
- Actor für Operation-Serialisierung (bleibt)
- Connection Pool innerhalb des Actors für Lease/Return

**Priority**: Medium - Optimierung für Throughput

---

#### 3. **Error Code Mapping**

**Problem**: enventa-spezifische Fehler nicht modelliert

**enventa wirft**:
- `FSException` - Allgemeine ERP-Fehler
- `FSLoginException` - Login fehlgeschlagen
- `FSLicenseException` - Lizenzprobleme
- `FSValidationException` - Datenvalidierung

**Empfehlung**: Exception Mapping zu `ProviderResult`:
```csharp
public enum ProviderErrorCode
{
    Unknown,
    ConnectionFailed,
    AuthenticationFailed,
    LicenseError,
    ValidationError,
    Timeout,
    RateLimited
}
```

**Priority**: Medium

---

#### 4. **NVArticleQueryBuilder Adapter**

**Problem**: QueryBuilder-Pattern aus eGate nicht adaptiert

**enventa Pattern**:
```csharp
var query = new NVArticleQueryBuilder()
    .ByCategory(filter.CategoryId)
    .WithPricing(filter.IncludePrices);
```

**Empfehlung**: Filter-zu-QueryBuilder Adapter:
```csharp
public class EnventaQueryAdapter
{
    public NVArticleQueryBuilder ToArticleQuery(ProductFilter filter) { ... }
    public NVCustomerQueryBuilder ToCustomerQuery(CustomerFilter filter) { ... }
}
```

**Priority**: High - Kernfunktionalität

---

## 🔧 Action Items

### Immediate (vor Production)

| # | Item | Owner | Priority | Effort |
|---|------|-------|----------|--------|
| 1 | Circuit Breaker / Retry Policy hinzufügen | @Backend | High | 2h |
| 2 | FSUtil Transaction Scope modellieren | @Enventa | High | 4h |
| 3 | Reflection in ErpActor eliminieren | @Backend | Medium | 3h |
| 4 | ProviderManager.DisposeAsync() implementieren | @Backend | Medium | 1h |

### Phase 2 (enventa Integration)

| # | Item | Owner | Priority | Effort |
|---|------|-------|----------|--------|
| 5 | EnventaQueryAdapter implementieren | @Enventa | High | 8h |
| 6 | enventa Exception Mapping | @Enventa | Medium | 4h |
| 7 | Connection Pool innerhalb Actor | @Enventa | Medium | 6h |
| 8 | Real enventa Provider (.NET 4.8) | @Enventa | High | 40h |

---

## 📁 Reviewed Files

### Core Structure ✅
- `B2X.ERP.csproj` - Projekt-Struktur korrekt
- `ServiceCollectionExtensions.cs` - DI-Registrierung

### Contracts ✅
- `Contracts/IErpProvider.cs` - Gut strukturiert
- `Contracts/IPimProvider.cs` - Konsistent
- `Contracts/ICrmProvider.cs` - Konsistent

### Core Models ✅
- `Core/TenantContext.cs` - Required properties korrekt
- `Core/ProviderResult.cs` - Result Pattern gut
- `Core/PagedResult.cs` - Paging korrekt

### Actor Infrastructure ✅
- `Infrastructure/Actor/ErpActor.cs` - Kernstück, funktioniert
- `Infrastructure/Actor/ErpActorPool.cs` - Thread-safe Pool
- `Infrastructure/Actor/ErpOperation.cs` - Operation Wrapper

### Services ✅
- `Services/IProviderManager.cs` - Interface klar
- `Services/ProviderManager.cs` - Lifecycle Management

### Providers ⚠️
- `Providers/Enventa/EnventaProviderFactory.cs` - Fake für Mac OK
- Real Implementation ausstehend (.NET 4.8)

### Protos ✅
- `Protos/erp_services.proto` - Vollständig
- `Protos/connector_registry.proto` - On-Premise Support

### Tests ✅
- 27 Tests, alle passing
- Shouldly Assertions (korrekt, nicht FluentAssertions)

---

## 🎯 Conclusion

### @Architect Verdict
> Die Architektur ist **ADR-023 konform** und sauber implementiert. Die Actor-Pattern-Umsetzung für Thread-Safety ist korrekt. Hauptkritikpunkt ist die Reflection-basierte Operation-Ausführung, die vor Production refactored werden sollte. Resilience-Patterns (Circuit Breaker) fehlen noch.

### @Enventa Verdict
> Die Struktur ist **ready für enventa Integration**. Die Fake-Provider ermöglichen parallele Entwicklung auf Mac. Die kritischen enventa-Aspekte (Single-Threading, Login-Serialisierung) sind korrekt berücksichtigt. Vor der echten Integration müssen Transaction Scopes und QueryBuilder-Adapter hinzugefügt werden.

---

**Sign-off**:
- [x] @Architect - Architecture Compliance
- [x] @Enventa - enventa ERP Compatibility
- [x] @SARAH - Coordination & Documentation

**Next Review**: Nach Phase 2 (enventa Real Implementation)

---

*Agents: @Architect, @Enventa | Owner: @SARAH*
