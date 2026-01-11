// <copyright file="ErpConnectionResult.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

namespace B2X.ERP.Abstractions;

/// <summary>
/// Represents the result of testing a connection to the ERP system.
/// </summary>
public class ErpConnectionResult
{
    /// <summary>
    /// Gets or sets whether the connection test was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets additional connection test data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}