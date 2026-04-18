using B2X.Types.Domain;

namespace B2X.Hosting.Domain;

/// <summary>
/// Repository interface for AppHost entities.
/// </summary>
public interface IAppHostRepository
{
    /// <summary>
    /// Gets an AppHost by ID.
    /// </summary>
    Task<AppHost?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an AppHost by name.
    /// </summary>
    Task<AppHost?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active AppHosts.
    /// </summary>
    Task<IEnumerable<AppHost>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new AppHost.
    /// </summary>
    Task AddAsync(AppHost appHost, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing AppHost.
    /// </summary>
    Task UpdateAsync(AppHost appHost, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an AppHost entity.
/// </summary>
public class AppHost : Entity
{
    /// <summary>
    /// The name of the AppHost.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The status of the AppHost.
    /// </summary>
    public AppHostStatus Status { get; set; } = AppHostStatus.Stopped;

    /// <summary>
    /// The last health check time.
    /// </summary>
    public DateTime? LastHealthCheck { get; set; }

    /// <summary>
    /// Whether the AppHost is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// AppHost status enumeration.
/// </summary>
public enum AppHostStatus
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Failed
}
