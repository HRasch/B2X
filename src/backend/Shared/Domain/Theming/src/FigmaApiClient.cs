using System.Net.Http;
using System.Text.Json;
using B2X.ThemeService.Models;
using Microsoft.Extensions.Logging;

namespace B2X.ThemeService;

/// <summary>
/// Figma API Client for fetching design tokens and assets
/// </summary>
public interface IFigmaApiClient
{
    /// <summary>
    /// Fetch design tokens from a Figma file
    /// </summary>
    /// <param name="fileKey">Figma file key</param>
    /// <param name="accessToken">Figma access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Figma file data with design tokens</returns>
    Task<FigmaFileData> GetFileDataAsync(string fileKey, string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extract design tokens from Figma file data
    /// </summary>
    /// <param name="fileData">Figma file data</param>
    /// <returns>List of extracted design tokens</returns>
    Task<List<DesignToken>> ExtractDesignTokensAsync(FigmaFileData fileData);
}

/// <summary>
/// Figma API Client implementation
/// </summary>
public class FigmaApiClient : IFigmaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FigmaApiClient> _logger;

    public FigmaApiClient(HttpClient httpClient, ILogger<FigmaApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Set default timeout for Figma API calls
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <inheritdoc />
    public async Task<FigmaFileData> GetFileDataAsync(string fileKey, string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
            throw new ArgumentException("File key cannot be null or empty", nameof(fileKey));
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));

        try
        {
            _logger.LogInformation("Fetching Figma file data for file key: {FileKey}", fileKey);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.figma.com/v1/files/{fileKey}");
            request.Headers.Add("X-Figma-Token", accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var fileData = JsonSerializer.Deserialize<FigmaFileData>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (fileData == null)
                throw new InvalidOperationException("Failed to deserialize Figma file data");

            _logger.LogInformation("Successfully fetched Figma file data with {NodeCount} nodes", fileData.Nodes?.Count ?? 0);
            return fileData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching Figma file {FileKey}: {StatusCode}", fileKey, ex.StatusCode);
            throw new FigmaApiException($"Failed to fetch Figma file: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Figma file {FileKey}", fileKey);
            throw new FigmaApiException($"Unexpected error fetching Figma file: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<List<DesignToken>> ExtractDesignTokensAsync(FigmaFileData fileData)
    {
        var tokens = new List<DesignToken>();

        try
        {
            _logger.LogInformation("Extracting design tokens from Figma file data");

            if (fileData.Nodes == null || !fileData.Nodes.Any())
            {
                _logger.LogWarning("No nodes found in Figma file data");
                return tokens;
            }

            // Extract tokens from document structure
            foreach (var node in fileData.Nodes.Values)
            {
                await ExtractTokensFromNodeAsync(node, tokens, "").ConfigureAwait(false);
            }

            // Extract from variables if available (Figma Variables API)
            if (fileData.Variables != null)
            {
                foreach (var variable in fileData.Variables)
                {
                    var token = ConvertVariableToToken(variable);
                    if (token != null)
                    {
                        tokens.Add(token);
                    }
                }
            }

            _logger.LogInformation("Extracted {TokenCount} design tokens", tokens.Count);
            return tokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting design tokens from Figma data");
            throw new FigmaApiException($"Failed to extract design tokens: {ex.Message}", ex);
        }
    }

    private async Task ExtractTokensFromNodeAsync(FigmaNode node, List<DesignToken> tokens, string path)
    {
        var nodeName = node.Name ?? "unnamed";
        var currentPath = string.IsNullOrEmpty(path) ? nodeName : $"{path}.{nodeName}";

        // Extract fills (colors)
        if (node.Fills != null && node.Fills.Any())
        {
            foreach (var fill in node.Fills.Where(f => string.Equals(f.Type, "SOLID", StringComparison.Ordinal)))
            {
                if (fill.Color != null)
                {
                    var colorValue = $"rgba({(int)(fill.Color.R * 255)}, {(int)(fill.Color.G * 255)}, {(int)(fill.Color.B * 255)}, {fill.Color.A})";
                    tokens.Add(new DesignToken
                    {
                        Name = $"{currentPath}.color",
                        Value = colorValue,
                        Category = "Colors",
                        Type = VariableType.Color,
                        Source = "Figma",
                        Path = currentPath
                    });
                }
            }
        }

        // Extract strokes (border colors)
        if (node.Strokes != null && node.Strokes.Any())
        {
            foreach (var stroke in node.Strokes.Where(s => string.Equals(s.Type, "SOLID", StringComparison.Ordinal)))
            {
                if (stroke.Color != null)
                {
                    var colorValue = $"rgba({(int)(stroke.Color.R * 255)}, {(int)(stroke.Color.G * 255)}, {(int)(stroke.Color.B * 255)}, {stroke.Color.A})";
                    tokens.Add(new DesignToken
                    {
                        Name = $"{currentPath}.stroke",
                        Value = colorValue,
                        Category = "Colors",
                        Type = VariableType.Color,
                        Source = "Figma",
                        Path = currentPath
                    });
                }
            }
        }

        // Extract text styles
        if (string.Equals(node.Type, "TEXT", StringComparison.Ordinal) && node.Style != null)
        {
            if (node.Style.FontSize.HasValue)
            {
                tokens.Add(new DesignToken
                {
                    Name = $"{currentPath}.font-size",
                    Value = $"{node.Style.FontSize.Value}px",
                    Category = "Typography",
                    Type = VariableType.Size,
                    Source = "Figma",
                    Path = currentPath
                });
            }

            if (!string.IsNullOrEmpty(node.Style.FontFamily))
            {
                tokens.Add(new DesignToken
                {
                    Name = $"{currentPath}.font-family",
                    Value = node.Style.FontFamily,
                    Category = "Typography",
                    Type = VariableType.String,
                    Source = "Figma",
                    Path = currentPath
                });
            }

            if (node.Style.FontWeight.HasValue)
            {
                tokens.Add(new DesignToken
                {
                    Name = $"{currentPath}.font-weight",
                    Value = node.Style.FontWeight.Value.ToString(),
                    Category = "Typography",
                    Type = VariableType.Number,
                    Source = "Figma",
                    Path = currentPath
                });
            }
        }

        // Recursively process children
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                await ExtractTokensFromNodeAsync(child, tokens, currentPath).ConfigureAwait(false);
            }
        }
    }

    private DesignToken? ConvertVariableToToken(FigmaVariable variable)
    {
        try
        {
            string value;
            VariableType type;

            switch (variable.Type)
            {
                case "COLOR":
                    if (variable.Value is JsonElement colorElement && colorElement.ValueKind == JsonValueKind.Object)
                    {
                        var r = colorElement.GetProperty("r").GetDouble();
                        var g = colorElement.GetProperty("g").GetDouble();
                        var b = colorElement.GetProperty("b").GetDouble();
                        var a = colorElement.TryGetProperty("a", out var aProp) ? aProp.GetDouble() : 1.0;
                        value = $"rgba({(int)(r * 255)}, {(int)(g * 255)}, {(int)(b * 255)}, {a})";
                        type = VariableType.Color;
                    }
                    else
                    {
                        return null;
                    }
                    break;

                case "FLOAT":
                    if (variable.Value is JsonElement floatElement && floatElement.ValueKind == JsonValueKind.Number)
                    {
                        value = floatElement.GetDouble().ToString();
                        type = VariableType.Number;
                    }
                    else
                    {
                        return null;
                    }
                    break;

                case "STRING":
                    value = variable.Value?.ToString() ?? "";
                    type = VariableType.String;
                    break;

                default:
                    return null;
            }

            return new DesignToken
            {
                Name = variable.Name ?? "",
                Value = value,
                Category = DetermineCategory(variable.Name ?? ""),
                Type = type,
                Source = "Figma",
                Path = variable.Name ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert Figma variable {VariableName} to token", variable.Name);
            return null;
        }
    }

    private string DetermineCategory(string name)
    {
        var lowerName = name.ToLowerInvariant();

        if (lowerName.Contains("color") || lowerName.Contains("background") || lowerName.Contains("fill"))
            return "Colors";

        if (lowerName.Contains("font") || lowerName.Contains("text") || lowerName.Contains("typography"))
            return "Typography";

        if (lowerName.Contains("spacing") || lowerName.Contains("margin") || lowerName.Contains("padding") || lowerName.Contains("size"))
            return "Spacing";

        return "Other";
    }
}

/// <summary>
/// Custom exception for Figma API errors
/// </summary>
public class FigmaApiException : Exception
{
    public FigmaApiException(string message) : base(message) { }
    public FigmaApiException(string message, Exception innerException) : base(message, innerException) { }
}
