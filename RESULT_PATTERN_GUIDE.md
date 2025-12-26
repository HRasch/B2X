# Result-Pattern Implementation Guide

**This guide is part of B2Connect GitHub Specifications** - See [.copilot-specs.md](.copilot-specs.md#33-exception-handling---result-pattern-approach) for the authoritative policy.

## 📋 Overview

Das **Result-Pattern** ist eine funktionale Fehlerbehandlung, die **keine Exceptions für Code-Flow-Steuerung** verwendet.

**Vorteile:**
- ✅ Explicit Error Handling (Fehler sind sichtbar)
- ✅ No Hidden Exceptions (keine versteckten Fehler)
- ✅ Better Composability (leicht kombinierbar)
- ✅ Cleaner Code (keine Try-Catch Blöcke)

---

## 🔧 Core Result Types

### Base Result (für Operationen ohne Rückgabewert)

```csharp
namespace B2Connect.AppHost.Results;

/// <summary>
/// Represents the outcome of an operation without a return value
/// </summary>
public abstract record Result
{
    public sealed record Success(string Message) : Result;
    public sealed record Failure(string Error, Exception? Exception = null) : Result;

    /// <summary>
    /// Pattern match on the result
    /// </summary>
    public T Match<T>(
        Func<string, T> onSuccess,
        Func<string, Exception?, T> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Message),
            Failure f => onFailure(f.Error, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Execute side effects based on result
    /// </summary>
    public void Fold(
        Action<string> onSuccess,
        Action<string, Exception?> onFailure)
    {
        switch (this)
        {
            case Success s:
                onSuccess(s.Message);
                break;
            case Failure f:
                onFailure(f.Error, f.Exception);
                break;
        }
    }

    /// <summary>
    /// Check if result is successful
    /// </summary>
    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;
}

/// <summary>
/// Represents the outcome of an operation with a return value
/// </summary>
public abstract record Result<T> : Result
{
    public sealed record Success(T Value, string Message = "") : Result<T>;
    public sealed record Failure(string Error, Exception? Exception = null) : Result<T>;

    /// <summary>
    /// Pattern match on the result
    /// </summary>
    public TResult Match<TResult>(
        Func<T, string, TResult> onSuccess,
        Func<string, Exception?, TResult> onFailure) =>
        this switch
        {
            Success s => onSuccess(s.Value, s.Message),
            Failure f => onFailure(f.Error, f.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Map the value if successful, otherwise return failure
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> f) =>
        this switch
        {
            Success s => new Result<TResult>.Success(f(s.Value), s.Message),
            Failure failure => new Result<TResult>.Failure(failure.Error, failure.Exception),
            _ => throw new InvalidOperationException()
        };

    /// <summary>
    /// Bind operation (flatMap)
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> f) =>
        this switch
        {
            Success s => f(s.Value),
            Failure failure => new Result<TResult>.Failure(failure.Error, failure.Exception),
            _ => throw new InvalidOperationException()
        };

    /// <summary>
    /// Get value or throw exception
    /// </summary>
    public T GetValueOrThrow() =>
        this switch
        {
            Success s => s.Value,
            Failure f => throw new InvalidOperationException(f.Error, f.Exception),
            _ => throw new InvalidOperationException()
        };
}
```

---

## 🎯 Usage Examples

### Basic Pattern Matching

```csharp
var result = ValidatePath(path);

result.Match(
    onSuccess: msg => Console.WriteLine($"✅ {msg}"),
    onFailure: (error, ex) => Console.WriteLine($"❌ {error}")
);
```

### With Logging Integration

```csharp
var result = StartService("Auth Service", authServicePath, 9002);

result.Fold(
    onSuccess: msg => Log.Information(msg),
    onFailure: (error, ex) =>
    {
        Log.Error(ex, error);
    }
);
```

### Chaining Operations

```csharp
public Result<List<Process>> StartAllServices(List<ServiceDefinition> services)
{
    var processes = new List<Process>();
    var errors = new List<string>();

    foreach (var service in services)
    {
        var result = StartService(service.Name, service.Path, service.Port);
        
        result.Match(
            onSuccess: (process, msg) =>
            {
                processes.Add(process);
                Log.Information(msg);
            },
            onFailure: (error, ex) =>
            {
                errors.Add(error);
                Log.Error(ex, error);
            }
        );
    }

    return errors.Any()
        ? new Result<List<Process>>.Failure(
            $"Failed to start {errors.Count} service(s): {string.Join(", ", errors)}")
        : new Result<List<Process>>.Success(
            processes,
            $"All {processes.Count} services started successfully");
}
```

### Map & Bind (Functional Composition)

```csharp
// Map: Transform successful value
var mappedResult = GetServicePath()
    .Map(path => new ServiceDefinition { Path = path });

// Bind: Chain operations that return Result
var chainedResult = ValidatePath(path)
    .Bind(_ => StartProcess(path))
    .Bind(process => ConfigureService(process));
```

---

## 📋 Implementation Pattern in AppHost

### Before (Exception-based)

```csharp
try
{
    if (!Directory.Exists(path))
        throw new DirectoryNotFoundException($"Service not found: {path}");
    
    var process = Process.Start(psi) 
        ?? throw new InvalidOperationException("Cannot spawn process");
    
    Log.Information($"Service started: {process.Id}");
    return process;
}
catch (DirectoryNotFoundException ex)
{
    Log.Error(ex, "Path validation failed");
    return null;
}
catch (Exception ex)
{
    Log.Error(ex, "Unexpected error");
    return null;
}
```

### After (Result-Pattern)

```csharp
public Result<Process> StartService(string name, string path, int port)
{
    // Validate path
    var pathValidation = ValidatePath(path);
    if (pathValidation is Result.Failure pathFailure)
        return new Result<Process>.Failure(pathFailure.Error, pathFailure.Exception);
    
    // Start process
    return TryStartProcess(name, path, port);
}

private Result<Process> TryStartProcess(string name, string path, int port)
{
    try
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run",
            WorkingDirectory = path,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(psi);
        
        return process == null
            ? new Result<Process>.Failure(
                $"Process.Start returned null",
                new InvalidOperationException($"Cannot start process for {name}"))
            : new Result<Process>.Success(
                process,
                $"✓ {name} started (PID: {process.Id})");
    }
    catch (Exception ex)
    {
        return new Result<Process>.Failure(
            $"Exception starting {name}: {ex.Message}",
            ex);
    }
}

private Result ValidatePath(string path)
{
    return Directory.Exists(path)
        ? new Result.Success($"✓ Path validated: {path}")
        : new Result.Failure($"Service directory not found: {path}");
}
```

---

## 🔄 Orchestration with Results

```csharp
public async Task<Result> RunAsync(List<ServiceDefinition> services)
{
    Log.Information("🚀 Starting AppHost with Result-Pattern orchestration");
    
    var serviceResults = new List<Result<Process>>();

    foreach (var service in services)
    {
        var result = StartService(service.Name, service.Path, service.Port);
        serviceResults.Add(result);
        
        result.Fold(
            onSuccess: msg => Log.Information($"[INF] {msg}"),
            onFailure: (error, ex) =>
            {
                Log.Error(ex, $"[ERR] Failed to start {service.Name}");
                Log.Error($"Details: {error}");
            }
        );

        await Task.Delay(1000);
    }

    // Summary
    var successCount = serviceResults.OfType<Result<Process>.Success>().Count();
    var failureCount = serviceResults.OfType<Result<Process>.Failure>().Count();

    Log.Information($"✅ {successCount} services started, ❌ {failureCount} failed");

    if (failureCount > 0)
        return new Result.Failure(
            $"{failureCount} service(s) failed to start");

    return new Result.Success("All services running successfully");
}
```

---

## ⚙️ Error Recovery with Results

```csharp
public Result<Process> StartServiceWithRetry(
    string name,
    string path,
    int port,
    int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        var result = TryStartProcess(name, path, port);
        
        if (result is Result<Process>.Success)
            return result;
        
        Log.Warning($"Attempt {attempt}/{maxRetries} failed for {name}");
        
        if (attempt < maxRetries)
        {
            // Exponential backoff
            var delay = (int)(Math.Pow(2, attempt - 1) * 500);
            Thread.Sleep(delay);
        }
    }

    return new Result<Process>.Failure(
        $"Failed to start {name} after {maxRetries} attempts");
}
```

---

## 📊 Result Aggregation

```csharp
public class ResultAggregator
{
    private readonly List<Result> _results = new();

    public void Add(Result result) => _results.Add(result);

    public Result GetAggregatedResult()
    {
        var failures = _results.OfType<Result.Failure>().ToList();
        
        if (!failures.Any())
            return new Result.Success($"All {_results.Count} operations succeeded");

        var errorMessages = string.Join("; ", 
            failures.Select(f => f.Error));
        
        return new Result.Failure(
            $"{failures.Count} operation(s) failed: {errorMessages}");
    }

    public void LogSummary()
    {
        var successes = _results.OfType<Result.Success>().Count();
        var failures = _results.OfType<Result.Failure>().Count();
        
        Log.Information($"Summary: {successes} succeeded, {failures} failed");
        
        foreach (var failure in _results.OfType<Result.Failure>())
            Log.Error($"  • {failure.Error}");
    }
}
```

---

## 🎓 Best Practices

### ✅ DO

```csharp
// 1. Return Result from operations that can fail
public Result<T> DoSomething() { ... }

// 2. Use Match for explicit handling
result.Match(onSuccess: ..., onFailure: ...);

// 3. Log both success and failure
// 4. Use meaningful error messages
// 5. Preserve exception context for debugging
```

### ❌ DON'T

```csharp
// 1. Don't ignore result failures
_ = StartService(...); // BAD

// 2. Don't throw exceptions for flow control
if (result is Result.Failure) throw new Exception(...); // BAD

// 3. Don't swallow exceptions
catch (Exception) { } // BAD
```

---

## 🔗 References

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/posts/recipe-part2/)
- [Result Pattern in C#](https://github.com/CommunityToolkit/dotnet)
- [Error Handling Best Practices](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/creating-and-throwing-exceptions)

---

**Status: 🟢 RECOMMENDED FOR APPHOST IMPLEMENTATION**
