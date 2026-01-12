// <copyright file="ErpVersionInfo.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents version information for an ERP connector.
/// </summary>
public class ErpVersionInfo
{
    /// <summary>
    /// Gets or sets the system name.
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the system version.
    /// </summary>
    public string SystemVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string ApiVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether backward compatibility is supported.
    /// </summary>
    public bool SupportsBackwardCompatibility { get; set; }

    /// <summary>
    /// Gets or sets the minimum system version.
    /// </summary>
    public string MinimumSystemVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recommended system version.
    /// </summary>
    public string RecommendedSystemVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version string.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the build date.
    /// </summary>
    public DateTime BuildDate { get; set; } = DateTime.UtcNow;
}
