---
docid: ADR-056
title: SynchronizationContext Policy for ASP.NET Core
owner: @Architect
status: Accepted
created: 2026-01-11
---

# ADR-056: SynchronizationContext Policy for ASP.NET Core

**Status:** Accepted  
**Date:** 11. Januar 2026  
**Context:** B2X async/await patterns  
**Decision Authors:** @Architect, @TechLead, @Backend

---

## Problem

Die B2X-Codebase zeigt inkonsistente Verwendung von `ConfigureAwait(false)`:
- ~100 Stellen verwenden `ConfigureAwait(false)`
- Hunderte async Methoden verwenden es NICHT
- Unklarheit im Team über die Notwendigkeit

**Frage:** Brauchen wir `ConfigureAwait(false)` in unserer ASP.NET Core + Console App Architektur?

---

## Kontext

### B2X Technologie-Stack

| Komponente | Typ | SynchronizationContext |
|------------|-----|------------------------|
| Store Gateway | ASP.NET Core | ❌ Keiner |
| Admin Gateway | ASP.NET Core | ❌ Keiner |
| Management Gateway | ASP.NET Core | ❌ Keiner |
| ERP-Connector | Console App | ❌ Keiner |
| CLI Tools | Console App | ❌ Keiner |
| Unit Tests | xUnit | ❌ Keiner |

### Was ist SynchronizationContext?

Der `SynchronizationContext` bestimmt, auf welchem Thread Code nach einem `await` fortgesetzt wird:

- **WPF/WinForms**: Zurück zum UI-Thread (Deadlock-Gefahr bei `.Result`)
- **Legacy ASP.NET**: Zurück zum Request-Thread (Deadlock-Gefahr bei `.Result`)
- **ASP.NET Core**: KEIN SyncContext → beliebiger ThreadPool-Thread
- **Console Apps**: KEIN SyncContext → beliebiger ThreadPool-Thread

---

## Entscheidung

### 1. ConfigureAwait(false) ist NICHT erforderlich

Da B2X ausschließlich auf ASP.NET Core und Console Apps basiert, gibt es **keinen SynchronizationContext**. Daher:

```csharp
// BEIDE VARIANTEN SIND GLEICHWERTIG in B2X:

// Variante A - mit ConfigureAwait
var data = await _service.GetDataAsync().ConfigureAwait(false);

// Variante B - ohne ConfigureAwait (BEVORZUGT)
var data = await _service.GetDataAsync();
```

**ConfigureAwait(false) bringt in ASP.NET Core:**
- ❌ Keinen Performance-Vorteil
- ❌ Keine Deadlock-Vermeidung (gibt keine Deadlocks ohne SyncContext)
- ❌ Nur zusätzliche Code-Komplexität

### 2. Analyzer CA2007 deaktivieren

Der Roslyn Analyzer CA2007 warnt bei fehlendem `ConfigureAwait(false)`. Diese Warnung ist für ASP.NET Core **nicht relevant** und wird deaktiviert.

### 3. Bestehenden Code nicht ändern

Bestehende `ConfigureAwait(false)` Aufrufe:
- Schaden nicht
- Müssen nicht entfernt werden
- Neue Stellen brauchen es nicht

### 4. Blocking Calls dokumentieren

**Akzeptabel in:**
- CLI-Code (kein SyncContext)
- Unit Tests (kein SyncContext)
- Startup-Code (notwendig für Synchronisation)

**Vermeiden in:**
- Produktions-API-Code (schlechte Praxis, auch wenn nicht gefährlich)

---

## Richtlinie

```markdown
## Async/Await Policy für B2X

1. **ConfigureAwait(false)** ist NICHT erforderlich
   - B2X läuft auf ASP.NET Core (kein SyncContext)
   - CLI-Tools haben ebenfalls keinen SyncContext
   - Neue Code-Stellen NICHT mit ConfigureAwait(false) schreiben

2. **Blocking Calls** (.Result, .Wait(), .GetAwaiter().GetResult())
   - ✅ OK in: CLI, Tests, Startup
   - ⚠️ Vermeiden in: API-Controller, Services
   - Bevorzuge: async/await durchgängig

3. **Task.Run()** nur für CPU-bound Operationen
   - I/O-Operationen direkt awaiten
   - Fire-and-Forget explizit kennzeichnen: `_ = Task.Run(...)`

4. **Analyzer CA2007** ist deaktiviert
   - Keine Warnungen für fehlendes ConfigureAwait
```

---

## Konsequenzen

### Positiv
- ✅ Saubererer, lesbarer Code ohne `.ConfigureAwait(false)` überall
- ✅ Keine falschen Analyzer-Warnungen
- ✅ Klare Team-Policy
- ✅ Reduzierte kognitive Last beim Code-Review

### Negativ
- ⚠️ Wenn B2X jemals UI-Komponenten bekommt (WPF/Blazor WASM), muss Policy überprüft werden
- ⚠️ Library-Code, der außerhalb von B2X verwendet wird, sollte ConfigureAwait(false) haben

### Neutral
- Bestehender Code mit ConfigureAwait(false) bleibt unverändert

---

## Referenzen

- [Microsoft: ConfigureAwait FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/)
- [Stephen Cleary: ASP.NET Core SynchronizationContext](https://blog.stephencleary.com/2017/03/aspnetcore-synchronization-context.html)
- [Stephen Cleary: There Is No Thread](https://blog.stephencleary.com/2013/11/there-is-no-thread.html)
- [BS-SYNC-CTX-001](.ai/brainstorm/BS-SYNCHRONIZATION-CONTEXT-STRATEGY.md) - Analyse-Dokument

---

## Änderungshistorie

| Datum | Änderung | Autor |
|-------|----------|-------|
| 2026-01-11 | Initial | @Architect, @TechLead |
