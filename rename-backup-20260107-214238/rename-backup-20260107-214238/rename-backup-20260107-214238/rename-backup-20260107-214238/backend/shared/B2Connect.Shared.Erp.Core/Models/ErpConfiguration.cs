// <copyright file="ErpConfiguration.cs" company="NissenVelten">
// Copyright (c) NissenVelten. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace B2X.Shared.Erp.Core.Models;

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
    /// Gets or sets the connection settings for the ERP system.
    /// </summary>
    public Dictionary<string, string> ConnectionSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the authentication settings.
    /// </summary>
    public Dictionary<string, string> AuthenticationSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the synchronization settings.
    /// </summary>
    public ErpSyncSettings SyncSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets optional custom settings.
    /// </summary>
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Synchronization settings for ERP data.
/// </summary>
public class ErpSyncSettings
{
    /// <summary>
    /// Gets or sets the batch size for sync operations.
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the sync interval.
    /// </summary>
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets whether to use incremental sync.
    /// </summary>
    public bool UseIncrementalSync { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum retry count.
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry delay.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
}

/// <summary>
/// Schema definition for ERP configuration.
/// Used for configuration validation and UI generation.
/// </summary>
public class ErpConfigurationSchema
{
    /// <summary>
    /// Gets or sets the schema version.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the required configuration fields.
    /// </summary>
    public List<ErpConfigField> RequiredFields { get; set; } = new();

    /// <summary>
    /// Gets or sets the optional configuration fields.
    /// </summary>
    public List<ErpConfigField> OptionalFields { get; set; } = new();
}

/// <summary>
/// Definition of a configuration field.
/// </summary>
public class ErpConfigField
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field type.
    /// </summary>
    public ErpConfigFieldType Type { get; set; } = ErpConfigFieldType.String;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets whether this field contains sensitive data.
    /// </summary>
    public bool IsSensitive { get; set; }

    /// <summary>
    /// Gets or sets validation rules.
    /// </summary>
    public List<string> ValidationRules { get; set; } = new();
}

/// <summary>
/// Types of configuration fields.
/// </summary>
public enum ErpConfigFieldType
{
    /// <summary>
    /// String field.
    /// </summary>
    String,

    /// <summary>
    /// Integer field.
    /// </summary>
    Integer,

    /// <summary>
    /// Boolean field.
    /// </summary>
    Boolean,

    /// <summary>
    /// Password/secret field.
    /// </summary>
    Password,

    /// <summary>
    /// URL field.
    /// </summary>
    Url,

    /// <summary>
    /// Dropdown/selection field.
    /// </summary>
    Select,

    /// <summary>
    /// File path field.
    /// </summary>
    FilePath,

    /// <summary>
    /// Connection string field.
    /// </summary>
    ConnectionString
}
