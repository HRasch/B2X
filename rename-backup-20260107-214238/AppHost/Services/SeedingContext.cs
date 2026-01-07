using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using B2X.AppHost.Configuration;

namespace B2X.AppHost.Services;

/// <summary>
/// Context for tracking seeding operations and their progress for error handling and rollback.
/// </summary>
public class SeedingContext : IDisposable
{
    private readonly TestingConfiguration _config;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, SeedingPhase> _phases = new();
    private readonly ConcurrentBag<string> _seededServices = new();
    private readonly ConcurrentBag<SeedingError> _errors = new();
    private bool _disposed;

    public SeedingContext(TestingConfiguration config, ILogger logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Starts tracking a seeding phase.
    /// </summary>
    public void StartPhase(string phaseName)
    {
        var phase = new SeedingPhase(phaseName, DateTime.UtcNow);
        _phases[phaseName] = phase;
        _logger.LogInformation("Started seeding phase: {Phase}", phaseName);
    }

    /// <summary>
    /// Marks a service as successfully seeded.
    /// </summary>
    public void MarkServiceSeeded(string serviceName)
    {
        _seededServices.Add(serviceName);
        _logger.LogDebug("Marked service as seeded: {Service}", serviceName);
    }

    /// <summary>
    /// Completes a seeding phase successfully.
    /// </summary>
    public void CompletePhase(string phaseName)
    {
        if (_phases.TryGetValue(phaseName, out var phase))
        {
            phase.Complete();
            _logger.LogInformation("Completed seeding phase: {Phase} in {Duration}ms",
                phaseName, phase.Duration?.TotalMilliseconds ?? 0);
        }
    }

    /// <summary>
    /// Marks a seeding phase as failed.
    /// </summary>
    public void FailPhase(string phaseName, Exception error)
    {
        if (_phases.TryGetValue(phaseName, out var phase))
        {
            var seedingError = new SeedingError(phaseName, error.Message, error);
            _errors.Add(seedingError);
            phase.Fail(error);

            _logger.LogError(error, "Failed seeding phase: {Phase} after {Duration}ms",
                phaseName, phase.Duration?.TotalMilliseconds ?? 0);
        }
    }

    /// <summary>
    /// Gets the list of services that were successfully seeded.
    /// </summary>
    public IReadOnlyCollection<string> GetSeededServices()
    {
        return _seededServices.ToArray();
    }

    /// <summary>
    /// Gets the list of services that were successfully seeded for a specific phase.
    /// </summary>
    public IReadOnlyCollection<string> GetSeededServices(string phaseName)
    {
        // Note: Current implementation doesn't track services per phase
        // This returns all seeded services for backward compatibility
        return GetSeededServices();
    }

    /// <summary>
    /// Gets all errors that occurred during seeding.
    /// </summary>
    public IReadOnlyCollection<SeedingError> GetErrors()
    {
        return _errors.ToArray();
    }

    /// <summary>
    /// Gets errors for a specific phase.
    /// </summary>
    public IReadOnlyCollection<Exception> GetPhaseErrors(string phaseName)
    {
        return _errors.Where(e => e.PhaseName == phaseName).Select(e => e.Exception).ToArray();
    }

    /// <summary>
    /// Gets the list of completed phases.
    /// </summary>
    public IReadOnlyCollection<string> GetCompletedPhases()
    {
        return _phases.Where(p => p.Value.IsCompleted && !p.Value.IsFailed).Select(p => p.Key).ToArray();
    }

    /// <summary>
    /// Gets the list of failed phases.
    /// </summary>
    public IReadOnlyCollection<string> GetFailedPhases()
    {
        return _phases.Where(p => p.Value.IsFailed).Select(p => p.Key).ToArray();
    }

    /// <summary>
    /// Gets the current status of all phases.
    /// </summary>
    public IReadOnlyDictionary<string, SeedingPhase> GetPhases()
    {
        return _phases.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Checks if any critical phases failed.
    /// </summary>
    public bool HasCriticalFailures()
    {
        return _errors.Any(e => IsCriticalPhase(e.PhaseName));
    }

    /// <summary>
    /// Determines if a phase is critical (failure should stop entire seeding process).
    /// </summary>
    private bool IsCriticalPhase(string phaseName)
    {
        return phaseName switch
        {
            "CoreServices" => true,
            "Auth" => true,
            "Tenant" => true,
            _ => false
        };
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _phases.Clear();
        _seededServices.Clear();
        _errors.Clear();

        _disposed = true;
    }
}

/// <summary>
/// Represents a seeding phase and its execution status.
/// </summary>
public class SeedingPhase
{
    public string Name { get; }
    public DateTime StartedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    public Exception? Error { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsFailed => Error != null;
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - StartedAt : null;

    public SeedingPhase(string name, DateTime startedAt)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        StartedAt = startedAt;
    }

    public void Complete()
    {
        CompletedAt = DateTime.UtcNow;
        IsCompleted = true;
    }

    public void Fail(Exception error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        CompletedAt = DateTime.UtcNow;
        IsCompleted = true;
    }
}

/// <summary>
/// Represents an error that occurred during seeding.
/// </summary>
public class SeedingError
{
    public string PhaseName { get; }
    public string Message { get; }
    public Exception Exception { get; }
    public DateTime OccurredAt { get; }

    public SeedingError(string phaseName, string message, Exception exception)
    {
        PhaseName = phaseName ?? throw new ArgumentNullException(nameof(phaseName));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        OccurredAt = DateTime.UtcNow;
    }
}