using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using B2X.SmartDataIntegration.Models;
using B2X.SmartDataIntegration.Services;
using Microsoft.Extensions.Logging;

namespace B2X.SmartDataIntegration.Core;

/// <summary>
/// Basic AI-powered mapping engine implementation
/// Uses pattern matching and heuristics for intelligent field mapping suggestions
/// </summary>
public class BasicMappingEngine : IMappingEngine
{
    private readonly ILogger<BasicMappingEngine> _logger;

    public BasicMappingEngine(ILogger<BasicMappingEngine> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<MappingSuggestion>> AnalyzeAndSuggestMappingsAsync(
        IEnumerable<DataField> sourceFields,
        IEnumerable<DataField> targetFields,
        MappingContext context)
    {
        _logger.LogInformation("Analyzing mapping suggestions for {SourceSystem} -> {TargetSystem}",
            context.SourceSystem, context.TargetSystem);

        var suggestions = new List<MappingSuggestion>();
        var sourceFieldList = sourceFields.ToList();
        var targetFieldList = targetFields.ToList();

        foreach (var sourceField in sourceFieldList)
        {
            var bestMatches = FindBestMatches(sourceField, targetFieldList, context);
            suggestions.AddRange(bestMatches);
        }

        // Sort by confidence score
        suggestions = suggestions.OrderByDescending(s => s.ConfidenceScore).ToList();

        _logger.LogInformation("Generated {Count} mapping suggestions", suggestions.Count);
        return suggestions;
    }

    public async Task LearnFromFeedbackAsync(
        MappingSuggestion suggestion,
        bool wasAccepted,
        string? userFeedback = null)
    {
        // TODO: Implement learning mechanism
        // In Phase 2, this would update ML models based on user feedback
        _logger.LogInformation("Learning from feedback: Suggestion {Id} was {Accepted}",
            suggestion.Id, wasAccepted ? "accepted" : "rejected");
    }

    public async Task<double> CalculateMappingConfidenceAsync(
        DataField sourceField,
        DataField targetField)
    {
        var confidence = 0.0;

        // Name similarity (40% weight)
        var nameSimilarity = CalculateNameSimilarity(sourceField.Name, targetField.Name);
        confidence += nameSimilarity * 0.4;

        // Display name similarity (30% weight)
        var displayNameSimilarity = CalculateNameSimilarity(
            sourceField.DisplayName ?? sourceField.Name,
            targetField.DisplayName ?? targetField.Name);
        confidence += displayNameSimilarity * 0.3;

        // Data type compatibility (20% weight)
        var typeCompatibility = CalculateTypeCompatibility(sourceField.DataType, targetField.DataType);
        confidence += typeCompatibility * 0.2;

        // Business context similarity (10% weight)
        if (!string.IsNullOrEmpty(sourceField.BusinessContext) &&
            !string.IsNullOrEmpty(targetField.BusinessContext))
        {
            var contextSimilarity = CalculateNameSimilarity(
                sourceField.BusinessContext,
                targetField.BusinessContext);
            confidence += contextSimilarity * 0.1;
        }

        return Math.Min(confidence, 1.0); // Cap at 100%
    }

    public async Task<MappingTransformationType> SuggestTransformationTypeAsync(
        DataField sourceField,
        DataField targetField)
    {
        // Simple heuristic-based transformation suggestion
        if (sourceField.DataType == targetField.DataType)
        {
            return MappingTransformationType.Direct;
        }

        if (sourceField.DataType == DataFieldType.String && targetField.DataType == DataFieldType.String)
        {
            // Check if lengths suggest splitting or concatenation
            if (sourceField.MaxLength.HasValue && targetField.MaxLength.HasValue &&
                sourceField.MaxLength.Value > targetField.MaxLength.Value)
            {
                return MappingTransformationType.Split;
            }
        }

        // Default to direct mapping for now
        return MappingTransformationType.Direct;
    }

    public async Task<string?> GenerateTransformationParametersAsync(
        DataField sourceField,
        DataField targetField,
        MappingTransformationType transformationType)
    {
        // Generate basic transformation parameters based on the type
        switch (transformationType)
        {
            case MappingTransformationType.Direct:
                return null; // No parameters needed

            case MappingTransformationType.Split:
                // Suggest splitting by common delimiters
                return "{\"delimiter\": \" \", \"maxParts\": 1}";

            default:
                return null;
        }
    }

    public async Task<IDictionary<string, double>> AnalyzeMappingPatternsAsync(
        IEnumerable<DataMappingConfiguration> historicalMappings)
    {
        // TODO: Implement pattern analysis
        // In Phase 2, this would analyze historical mappings to identify patterns
        return new Dictionary<string, double>();
    }

    private List<MappingSuggestion> FindBestMatches(
        DataField sourceField,
        IEnumerable<DataField> targetFields,
        MappingContext context)
    {
        var suggestions = new List<MappingSuggestion>();

        foreach (var targetField in targetFields)
        {
            var confidence = CalculateMappingConfidenceAsync(sourceField, targetField).Result;

            // Only suggest mappings with confidence > 30%
            if (confidence > 0.3)
            {
                var transformationType = SuggestTransformationTypeAsync(sourceField, targetField).Result;
                var transformationParams = GenerateTransformationParametersAsync(
                    sourceField, targetField, transformationType).Result;

                var suggestion = new MappingSuggestion
                {
                    Id = Guid.NewGuid(),
                    DataMappingConfigurationId = Guid.Empty, // Will be set when applied
                    SourceField = sourceField,
                    SuggestedTargetField = targetField,
                    ConfidenceScore = confidence * 100, // Convert to percentage
                    SuggestedTransformation = transformationType,
                    TransformationParameters = transformationParams,
                    Reasoning = GenerateReasoning(sourceField, targetField, confidence),
                    IsAccepted = false,
                    IsRejected = false
                };

                suggestions.Add(suggestion);
            }
        }

        return suggestions;
    }

    private double CalculateNameSimilarity(string name1, string name2)
    {
        if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
            return 0.0;

        // Simple case-insensitive substring matching
        var lower1 = name1.ToLowerInvariant();
        var lower2 = name2.ToLowerInvariant();

        if (lower1.Contains(lower2) || lower2.Contains(lower1))
            return 0.8;

        // Levenshtein distance for similarity
        var distance = LevenshteinDistance(lower1, lower2);
        var maxLength = Math.Max(lower1.Length, lower2.Length);

        return maxLength == 0 ? 1.0 : 1.0 - (double)distance / maxLength;
    }

    private double CalculateTypeCompatibility(DataFieldType sourceType, DataFieldType targetType)
    {
        if (sourceType == targetType)
            return 1.0;

        // Define compatibility matrix
        return (sourceType, targetType) switch
        {
            (DataFieldType.Integer, DataFieldType.Decimal) => 0.9,
            (DataFieldType.Decimal, DataFieldType.Integer) => 0.7,
            (DataFieldType.String, DataFieldType.Integer) => 0.5,
            (DataFieldType.String, DataFieldType.Decimal) => 0.5,
            (DataFieldType.DateTime, DataFieldType.Date) => 0.8,
            (DataFieldType.Date, DataFieldType.DateTime) => 0.9,
            (DataFieldType.Boolean, DataFieldType.String) => 0.6,
            (DataFieldType.String, DataFieldType.Boolean) => 0.4,
            _ => 0.0
        };
    }

    private string GenerateReasoning(DataField sourceField, DataField targetField, double confidence)
    {
        var reasons = new List<string>();

        if (CalculateNameSimilarity(sourceField.Name, targetField.Name) > 0.7)
            reasons.Add("field names are very similar");

        if (sourceField.DataType == targetField.DataType)
            reasons.Add("data types match exactly");
        else if (CalculateTypeCompatibility(sourceField.DataType, targetField.DataType) > 0.7)
            reasons.Add("data types are compatible");

        if (!string.IsNullOrEmpty(sourceField.BusinessContext) &&
            !string.IsNullOrEmpty(targetField.BusinessContext) &&
            CalculateNameSimilarity(sourceField.BusinessContext, targetField.BusinessContext) > 0.7)
            reasons.Add("business contexts align");

        return $"Suggested mapping because {string.Join(" and ", reasons)}. Confidence: {confidence:P0}";
    }

    private static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
            return t?.Length ?? 0;
        if (string.IsNullOrEmpty(t))
            return s.Length;

        var matrix = new int[s.Length + 1, t.Length + 1];

        for (var i = 0; i <= s.Length; i++)
            matrix[i, 0] = i;

        for (var j = 0; j <= t.Length; j++)
            matrix[0, j] = j;

        for (var i = 1; i <= s.Length; i++)
        {
            for (var j = 1; j <= t.Length; j++)
            {
                var cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[s.Length, t.Length];
    }
}
