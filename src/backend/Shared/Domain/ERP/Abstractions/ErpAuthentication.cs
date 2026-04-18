// <copyright file="ErpAuthentication.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents authentication configuration for ERP connections.
/// </summary>
public class ErpAuthentication
{
    /// <summary>
    /// Gets or sets the authentication type (e.g., "basic", "oauth", "api-key").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for basic authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the API key for API key authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the token for OAuth or bearer token authentication.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets additional authentication parameters.
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; set; } = new();
}
