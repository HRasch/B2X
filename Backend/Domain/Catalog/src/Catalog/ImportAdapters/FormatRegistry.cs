using System;
using System.Collections.Generic;
using System.Linq;
using B2Connect.Catalog.ImportAdapters.Formats;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.ImportAdapters;

/// <summary>
/// Registry and auto-detection for format adapters
/// </summary>
public class FormatRegistry
{
    private readonly List<IFormatAdapter> _adapters;
    private readonly ILogger<FormatRegistry> _logger;

    public FormatRegistry(IEnumerable<IFormatAdapter> adapters, ILogger<FormatRegistry> logger)
    {
        _adapters = adapters.OrderByDescending(a => GetAdapterPriority(a.FormatId)).ToList();
        _logger = logger;
    }

    /// <summary>
    /// Get adapter by format ID
    /// </summary>
    public IFormatAdapter? GetAdapterById(string formatId)
    {
        return _adapters.FirstOrDefault(a => a.FormatId.Equals(formatId, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Auto-detect format and return appropriate adapter
    /// </summary>
    public IFormatAdapter? DetectFormat(string content, string fileName)
    {
        _logger.LogInformation(
            "Auto-detecting format for file: {FileName} ({ContentLength} bytes)",
            fileName,
            content.Length);

        // Try adapters in priority order (most specific first)
        foreach (var adapter in _adapters)
        {
            var confidence = adapter.DetectFormat(content, fileName);

            if (confidence > 0.9) // High confidence
            {
                _logger.LogInformation(
                    "Format detected: {Format} (confidence: {Confidence:P})",
                    adapter.FormatId,
                    confidence);
                return adapter;
            }

            if (confidence > 0.0)
            {
                _logger.LogDebug(
                    "Format {Format} detected with confidence {Confidence:P}",
                    adapter.FormatId,
                    confidence);
            }
        }

        _logger.LogWarning("No format detected with sufficient confidence for file: {FileName}", fileName);
        return null;
    }

    /// <summary>
    /// Get all available adapters
    /// </summary>
    public IReadOnlyList<IFormatAdapter> GetAllAdapters() => _adapters.AsReadOnly();

    /// <summary>
    /// Determine priority order for adapter detection
    /// Higher = checked first
    /// </summary>
    private static int GetAdapterPriority(string formatId)
    {
        return formatId.ToLowerInvariant() switch
        {
            "bmecat" => 100,      // Most specific format
            "datanorm" => 90,     // Semi-structured
            "csv" => 10,          // Most generic
            _ => 0
        };
    }
}

/// <summary>
/// Extension methods for format adapters
/// </summary>
public static class FormatAdapterExtensions
{
    /// <summary>
    /// Register format adapters in DI container
    /// </summary>
    public static IServiceCollection AddFormatAdapters(
        this IServiceCollection services)
    {
        // Register all format adapters
        services.AddSingleton<IFormatAdapter, BmecatImportAdapter>();
        services.AddSingleton<IFormatAdapter, DatanormImportAdapter>();
        services.AddSingleton<IFormatAdapter, CsvImportAdapter>();

        // Register registry
        services.AddSingleton<FormatRegistry>();

        return services;
    }
}
