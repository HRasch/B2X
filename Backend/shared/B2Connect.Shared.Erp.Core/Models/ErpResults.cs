// <copyright file="ErpResults.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;

namespace B2Connect.Shared.Erp.Core.Models;

/// <summary>
/// Result of an ERP connection test.
/// </summary>
public class ErpConnectionResult
{
    /// <summary>
    /// Gets or sets whether the connection was successful.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Gets or sets the error message if connection failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the response time.
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Gets or sets the ERP system version detected.
    /// </summary>
    public string? DetectedVersion { get; set; }

    /// <summary>
    /// Creates a successful connection result.
    /// </summary>
    /// <param name="responseTime">The response time.</param>
    /// <param name="version">The detected version.</param>
    /// <returns>A successful result.</returns>
    public static ErpConnectionResult Success(TimeSpan responseTime, string? version = null)
        => new() { IsConnected = true, ResponseTime = responseTime, DetectedVersion = version };

    /// <summary>
    /// Creates a failed connection result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static ErpConnectionResult Failed(string errorMessage)
        => new() { IsConnected = false, ErrorMessage = errorMessage };
}

/// <summary>
/// Result of an ERP order operation.
/// </summary>
public class ErpOrderResult
{
    /// <summary>
    /// Gets or sets whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the ERP order number.
    /// </summary>
    public string? ErpOrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the error message if failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets additional details about the operation.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the operation.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates a successful order result.
    /// </summary>
    /// <param name="erpOrderNumber">The ERP order number.</param>
    /// <returns>A successful result.</returns>
    public static ErpOrderResult Succeeded(string erpOrderNumber)
        => new() { Success = true, ErpOrderNumber = erpOrderNumber };

    /// <summary>
    /// Creates a failed order result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result.</returns>
    public static ErpOrderResult Failed(string errorMessage)
        => new() { Success = false, ErrorMessage = errorMessage };
}

/// <summary>
/// Version information for an ERP connector.
/// </summary>
public class ErpVersionInfo
{
    /// <summary>
    /// Gets or sets the connector version.
    /// </summary>
    public Version ConnectorVersion { get; set; } = new(1, 0, 0);

    /// <summary>
    /// Gets or sets the ERP system version.
    /// </summary>
    public string? ErpSystemVersion { get; set; }

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string? ApiVersion { get; set; }

    /// <summary>
    /// Gets or sets the minimum supported ERP version.
    /// </summary>
    public string? MinSupportedErpVersion { get; set; }

    /// <summary>
    /// Gets or sets the maximum supported ERP version.
    /// </summary>
    public string? MaxSupportedErpVersion { get; set; }

    /// <summary>
    /// Gets or sets the recommended system version.
    /// </summary>
    public string? RecommendedSystemVersion { get; set; }
}
