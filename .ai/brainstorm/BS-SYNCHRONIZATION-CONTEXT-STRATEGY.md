---
docid: BS-SYNC-CTX-001
title: SynchronizationContext Strategy - B2X Solution
owner: @Backend
status: Brainstorm
created: 2026-01-11
---

# 🔄 SynchronizationContext Strategy - B2X Solution

**DocID**: `BS-SYNC-CTX-001`  
**Erstellt**: 11. Januar 2026  
**Status**: Brainstorm → Zur Prüfung durch @TechLead + @Architect

---

## 📊 Analyse-Ergebnis

### Projekt-Profil

| Kategorie | Stack | SyncContext vorhanden? |
|-----------|-------|------------------------|
| **Backend APIs** | ASP.NET Core + Aspire | ❌ Nein |
| **ERP-Connector** | Console App | ❌ Nein |
| **CLI Tools** | Console App | ❌ Nein |
| **Frontend** | Vue.js (Nuxt) | N/A (JavaScript) |
| **Tests** | xUnit | ❌ Nein |

**Fazit**: Die gesamte B2X-Solution läuft **ohne SynchronizationContext**.

---

## 🔍 Ist-Analyse: Gefundene Muster

### 1. ConfigureAwait(false) - Inkonsistente Verwendung

**Fundstellen mit ConfigureAwait(false)**:
- [InvoiceHandler.cs](../../src/Store/Backend/Customer/src/Handlers/InvoiceHandler.cs) ✅
- [InvoiceRepository.cs](../../src/Store/Backend/Customer/src/Data/InvoiceRepository.cs) ✅
- [InvoiceService.cs](../../src/Store/Backend/Customer/src/Services/InvoiceService.cs) ✅
- Einige Test-Dateien

**Problem**: Nur ~20 Dateien verwenden `ConfigureAwait(false)`, aber hunderte async Methoden existieren.

### 2. Blocking Calls - Potenzielle Risiken

| Datei | Pattern | Risiko |
|-------|---------|--------|
| [JurisdictionComplianceEngineTests.cs](../../src/tests/Admin/Backend/Compliance/tests/Jurisdictions/JurisdictionComplianceEngineTests.cs#L109) | `.Result` | 🔶 Test - akzeptabel |
| [ConsoleOutputService.cs](../../src/shared/B2X.CLI.Shared/ConsoleOutputService.cs#L156) | `.Wait()` | 🟡 CLI - akzeptabel (kein SyncContext) |
| [Program.cs (ERP)](../../src/erp-connector/src/B2X.ErpConnector/Program.cs#L604) | `.GetAwaiter().GetResult()` | 🟡 CLI - akzeptabel |
| [Program.cs (Identity)](../../src/backend/Shared/Domain/Identity/Program.cs#L268) | `.GetAwaiter().GetResult()` | ⚠️ Startup - prüfen |

### 3. Task.Run() Verwendung

```
12 Fundstellen - meist legitim für:
- Fire-and-Forget Jobs
- CPU-bound Operations (BMEcat Import)
- Background Processing
```

---

## 🎯 Strategie-Empfehlungen

### Empfehlung 1: **KEIN ConfigureAwait(false) erforderlich** ✅

**Begründung**:
- ASP.NET Core hat **keinen** SynchronizationContext
- Console Apps (CLI, ERP-Connector) haben **keinen** SynchronizationContext
- `ConfigureAwait(false)` ist hier **optional und bringt keinen Performance-Vorteil**

**Entscheidung**: 
```
📌 ConfigureAwait(false) ist in B2X NICHT erforderlich.
   Bestehende Verwendungen können bleiben, neue sind nicht nötig.
```

### Empfehlung 2: Analyzer-Regel deaktivieren

Falls CA2007 (ConfigureAwait) aktiv ist:

```xml
<!-- .editorconfig oder Directory.Build.props -->
<PropertyGroup>
  <NoWarn>$(NoWarn);CA2007</NoWarn>
</PropertyGroup>
```

Oder in `.editorconfig`:
```ini
# CA2007: ConfigureAwait - nicht relevant für ASP.NET Core
dotnet_diagnostic.CA2007.severity = none
```

### Empfehlung 3: Blocking Calls in Tests akzeptieren

**In Unit Tests** ist `.Result` oder `.GetAwaiter().GetResult()` **akzeptabel**, weil:
- xUnit hat keinen SynchronizationContext
- Kein Deadlock-Risiko
- Manchmal einfacher für Test-Setup

**Aber**: In Produktion sollte `async/await` bevorzugt werden.

### Empfehlung 4: CLI-Code ist akzeptabel

Die gefundenen `.Wait()` Aufrufe in [ConsoleOutputService.cs](../../src/shared/B2X.CLI.Shared/ConsoleOutputService.cs):

```csharp
// Das ist OK in CLI-Kontexten:
public void Spinner(string title, Func<Task> action)
{
    AnsiConsole.Status()
        .Start(title, ctx =>
        {
            action().Wait(); // ✅ Kein SyncContext, kein Deadlock
        });
}
```

**Grund**: Spectre.Console's `Start()` erwartet synchrone Callbacks.

### Empfehlung 5: Startup-Code prüfen

```csharp
// src/backend/Shared/Domain/Identity/Program.cs
db.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
```

**Empfehlung**: Bei Startup ist das akzeptabel, aber konsistent machen:

```csharp
// Alternative: Explizit synchron verwenden
db.Database.EnsureCreated(); // Synchrone Variante, wenn verfügbar
```

---

## 📋 Aktionsplan

### Phase 1: Dokumentation (Sofort) ✅

- [x] Diese Strategie dokumentieren
- [ ] README oder CONTRIBUTING.md aktualisieren mit Policy

### Phase 2: Analyzer-Konfiguration (Optional)

```xml
<!-- Directory.Build.props - Optional: CA2007 deaktivieren -->
<PropertyGroup>
  <!-- ConfigureAwait nicht erforderlich in ASP.NET Core -->
  <NoWarn>$(NoWarn);CA2007</NoWarn>
</PropertyGroup>
```

### Phase 3: Code-Cleanup (Niedrige Priorität)

| Aktion | Priorität | Aufwand |
|--------|-----------|---------|
| Bestehende `ConfigureAwait(false)` entfernen | 🔵 Niedrig | ~2h |
| Blocking Calls in Produktion zu async migrieren | 🟡 Mittel | ~4h |
| Konsistente Startup-Pattern etablieren | 🟢 Optional | ~1h |

---

## 🚫 Anti-Patterns zu vermeiden

### ❌ NIEMALS in Produktion:

```csharp
// NICHT TUN - Blockiert Thread unnötig
public ActionResult Get()
{
    var data = _service.GetDataAsync().Result; // ❌
    return Ok(data);
}

// RICHTIG
public async Task<ActionResult> Get()
{
    var data = await _service.GetDataAsync();
    return Ok(data);
}
```

### ❌ NIEMALS Task.Run für async I/O:

```csharp
// FALSCH - Task.Run für I/O ist Verschwendung
var data = await Task.Run(() => _db.QueryAsync()); // ❌

// RICHTIG
var data = await _db.QueryAsync(); // ✅
```

### ✅ Task.Run NUR für CPU-bound:

```csharp
// RICHTIG - CPU-intensive Arbeit
var result = await Task.Run(() => ParseLargeBmecatFile()); // ✅
```

---

## 📚 Team-Richtlinie

### SynchronizationContext Policy für B2X

```markdown
## Async/Await Richtlinie

1. **ConfigureAwait(false)** ist NICHT erforderlich
   - B2X läuft auf ASP.NET Core (kein SyncContext)
   - CLI-Tools haben ebenfalls keinen SyncContext

2. **Blocking Calls** vermeiden in Produktion
   - `.Result`, `.Wait()`, `.GetAwaiter().GetResult()` → `await` verwenden
   - Ausnahme: Startup-Code, CLI-Callbacks, Tests

3. **Task.Run()** nur für CPU-bound Operationen
   - I/O-Operationen direkt awaiten
   - Fire-and-Forget explizit kennzeichnen mit `_ = Task.Run(...)`

4. **Tests** dürfen synchrone Patterns verwenden
   - `.Result` in Test-Setup ist akzeptabel
   - Bevorzuge dennoch `async Task` Test-Methoden
```

---

## 🔗 Referenzen

- [Microsoft Docs: ConfigureAwait FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/)
- [Stephen Cleary: There Is No Thread](https://blog.stephencleary.com/2013/11/there-is-no-thread.html)
- [ASP.NET Core SynchronizationContext](https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html)

---

## ✅ Zusammenfassung

| Frage | Antwort |
|-------|---------|
| Brauchen wir `ConfigureAwait(false)`? | **Nein** - kein SyncContext vorhanden |
| Sind die `.Wait()` Aufrufe problematisch? | **Nein** - CLI/Test-Kontext akzeptabel |
| Sollen wir bestehende entfernen? | **Optional** - niedrige Priorität |
| Was ist die Team-Policy? | **async/await überall, keine blocking calls in Prod** |

---

**Nächste Schritte**:
1. @TechLead: Review dieser Strategie
2. @Architect: Bestätigung der Policy
3. @Backend: Optional: Cleanup der inkonsistenten Stellen

---
**Erstellt von**: @SARAH (Koordination)  
**Zur Prüfung**: @TechLead, @Architect
