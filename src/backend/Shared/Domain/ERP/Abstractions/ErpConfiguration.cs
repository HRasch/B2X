// <copyright file="ErpConfiguration.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Configuration for an ERP connector instance.
/// Contains tenant-specific settings for ERP connection.
/// </summary>
public class ErpConfiguration
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ERP type identifier.
    /// </summary>
    public string ErpType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ERP version.
    /// </summary>
    public string ErpVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the connection settings for the ERP system.
    /// </summary>
    public Dictionary<string, string> ConnectionSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the authentication configuration.
    /// </summary>
    public ErpAuthentication Authentication { get; set; } = new();

    /// <summary>
    /// Gets or sets optional custom settings.
    /// </summary>
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}