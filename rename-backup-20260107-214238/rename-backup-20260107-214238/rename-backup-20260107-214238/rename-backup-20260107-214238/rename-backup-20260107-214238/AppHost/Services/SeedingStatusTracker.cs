using B2X.AppHost.Configuration;
using System.Text.Json;

namespace B2X.AppHost.Services;

/// <summary>
/// Tracks seeding status and timestamps for monitoring and debugging.
/// Provides persistence of seeding operations across application restarts.
/// </summary>
public class SeedingStatusTracker
{
    private readonly string _statusFilePath;
    private readonly object _lock = new();
    private SeedingStatus? _currentStatus;

    public SeedingStatusTracker(string statusFilePath = "seeding-status.json")
    {
        _statusFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, statusFilePath);
    }

    /// <summary>
    /// Records the start of a seeding operation.
    /// </summary>
    public void RecordSeedingStart(TestingConfiguration config)
    {
        lock (_lock)
        {
            _currentStatus = new SeedingStatus
            {
                Mode = config.Mode,
                Environment = config.Environment,
                StartedAt = DateTime.UtcNow,
                IsInProgress = true,
                TenantCount = config.SeedData.DefaultTenantCount,
                UsersPerTenant = config.SeedData.UsersPerTenant,
                SampleProductCount = config.SeedData.IncludeCatalogDemo ? config.SeedData.SampleProductCount : 0,
                SamplePageCount = config.SeedData.IncludeCmsContent ? config.SeedData.SamplePageCount : 0,
                IncludeCatalogDemo = config.SeedData.IncludeCatalogDemo,
                IncludeCmsContent = config.SeedData.IncludeCmsContent
            };

            SaveStatus();
        }
    }

    /// <summary>
    /// Records the completion of a seeding operation.
    /// </summary>
    public void RecordSeedingComplete(TestDataStatistics statistics)
    {
        lock (_lock)
        {
            if (_currentStatus == null)
            {
                _currentStatus = new SeedingStatus();
            }

            _currentStatus.IsInProgress = false;
            _currentStatus.CompletedAt = DateTime.UtcNow;
            _currentStatus.LastStatistics = statistics;
            _currentStatus.Success = true;

            SaveStatus();
        }
    }

    /// <summary>
    /// Records a seeding failure.
    /// </summary>
    public void RecordSeedingFailure(string errorMessage, Exception? exception = null)
    {
        lock (_lock)
        {
            if (_currentStatus == null)
            {
                _currentStatus = new SeedingStatus();
            }

            _currentStatus.IsInProgress = false;
            _currentStatus.CompletedAt = DateTime.UtcNow;
            _currentStatus.Success = false;
            _currentStatus.LastError = errorMessage;
            _currentStatus.LastException = exception?.ToString();

            SaveStatus();
        }
    }

    /// <summary>
    /// Gets the current seeding status.
    /// </summary>
    public SeedingStatus? GetCurrentStatus()
    {
        lock (_lock)
        {
            return _currentStatus ?? LoadStatus();
        }
    }

    /// <summary>
    /// Checks if seeding was completed recently (within the specified time span).
    /// </summary>
    public bool WasSeedingCompletedRecently(TimeSpan maxAge)
    {
        var status = GetCurrentStatus();
        if (status == null || !status.Success || !status.CompletedAt.HasValue)
        {
            return false;
        }

        return (DateTime.UtcNow - status.CompletedAt.Value) <= maxAge;
    }

    /// <summary>
    /// Clears the seeding status (for testing or reset scenarios).
    /// </summary>
    public void ClearStatus()
    {
        lock (_lock)
        {
            _currentStatus = null;
            if (File.Exists(_statusFilePath))
            {
                File.Delete(_statusFilePath);
            }
        }
    }

    /// <summary>
    /// Gets a summary of seeding history for reporting.
    /// </summary>
    public SeedingStatusSummary GetStatusSummary()
    {
        var status = GetCurrentStatus();
        return new SeedingStatusSummary
        {
            HasSeededData = status?.Success == true,
            LastSeededAt = status?.CompletedAt,
            Mode = status?.Mode ?? "unknown",
            Environment = status?.Environment ?? "unknown",
            IsCurrentlySeeding = status?.IsInProgress == true,
            LastError = status?.LastError
        };
    }

    private void SaveStatus()
    {
        try
        {
            var json = JsonSerializer.Serialize(_currentStatus, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            File.WriteAllText(_statusFilePath, json);
        }
        catch (Exception ex)
        {
            // Log but don't fail - status persistence is not critical
            Console.WriteLine($"Warning: Failed to save seeding status: {ex.Message}");
        }
    }

    private SeedingStatus? LoadStatus()
    {
        try
        {
            if (!File.Exists(_statusFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(_statusFilePath);
            return JsonSerializer.Deserialize<SeedingStatus>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            // Log but don't fail - status loading is not critical
            Console.WriteLine($"Warning: Failed to load seeding status: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// Represents the current seeding status.
/// </summary>
public class SeedingStatus
{
    public string? Mode { get; set; }
    public string? Environment { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsInProgress { get; set; }
    public bool Success { get; set; }
    public string? LastError { get; set; }
    public string? LastException { get; set; }

    // Configuration that was used
    public int TenantCount { get; set; }
    public int UsersPerTenant { get; set; }
    public int SampleProductCount { get; set; }
    public int SamplePageCount { get; set; }
    public bool IncludeCatalogDemo { get; set; }
    public bool IncludeCmsContent { get; set; }

    // Results
    public TestDataStatistics? LastStatistics { get; set; }
}

/// <summary>
/// Summary of seeding status for quick checks.
/// </summary>
public class SeedingStatusSummary
{
    public bool HasSeededData { get; set; }
    public DateTime? LastSeededAt { get; set; }
    public string Mode { get; set; } = "unknown";
    public string Environment { get; set; } = "unknown";
    public bool IsCurrentlySeeding { get; set; }
    public string? LastError { get; set; }

    public override string ToString()
    {
        if (!HasSeededData)
        {
            return "No seeding data found";
        }

        var age = LastSeededAt.HasValue
            ? (DateTime.UtcNow - LastSeededAt.Value).TotalHours.ToString("F1") + " hours ago"
            : "unknown time";

        return $"Last seeded {age} in {Mode} mode ({Environment})";
    }
}