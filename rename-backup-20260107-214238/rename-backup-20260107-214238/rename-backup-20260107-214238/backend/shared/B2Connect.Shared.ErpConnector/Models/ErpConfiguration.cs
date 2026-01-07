using System.Collections.Generic;

namespace B2X.Shared.ErpConnector.Models;

/// <summary>
/// Configuration for ERP adapter initialization.
/// </summary>
public class ErpConfiguration
{
    /// <summary>
    /// Tenant identifier.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// ERP system connection settings.
    /// </summary>
    public ConnectionSettings Connection { get; set; } = new();

    /// <summary>
    /// Authentication settings.
    /// </summary>
    public AuthenticationSettings Authentication { get; set; } = new();

    /// <summary>
    /// Synchronization settings.
    /// </summary>
    public SyncSettings Sync { get; set; } = new();

    /// <summary>
    /// Custom configuration parameters.
    /// </summary>
    public Dictionary<string, string> CustomParameters { get; set; } = new();
}

/// <summary>
/// ERP connection settings.
/// </summary>
public class ConnectionSettings
{
    /// <summary>
    /// Server hostname or IP address.
    /// </summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// Port number.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Database name (if applicable).
    /// </summary>
    public string? Database { get; set; }

    /// <summary>
    /// Connection timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of concurrent connections.
    /// </summary>
    public int MaxConnections { get; set; } = 10;
}

/// <summary>
/// Authentication settings for ERP connection.
/// </summary>
public class AuthenticationSettings
{
    /// <summary>
    /// Authentication method.
    /// </summary>
    public string Method { get; set; } = "Basic";

    /// <summary>
    /// Username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// API key (if applicable).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Client ID for OAuth (if applicable).
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret for OAuth (if applicable).
    /// </summary>
    public string? ClientSecret { get; set; }
}

/// <summary>
/// Synchronization settings.
/// </summary>
public class SyncSettings
{
    /// <summary>
    /// Synchronization mode.
    /// </summary>
    public SyncMode Mode { get; set; } = SyncMode.Batch;

    /// <summary>
    /// Batch size for synchronization.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Synchronization interval in minutes.
    /// </summary>
    public int IntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Enable real-time synchronization.
    /// </summary>
    public bool EnableRealTime { get; set; }
}

/// <summary>
/// Synchronization modes.
/// </summary>
public enum SyncMode
{
    /// <summary>
    /// Batch synchronization at regular intervals.
    /// </summary>
    Batch,

    /// <summary>
    /// Real-time synchronization on demand.
    /// </summary>
    RealTime,

    /// <summary>
    /// Hybrid mode combining batch and real-time.
    /// </summary>
    Hybrid
}