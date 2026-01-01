using B2Connect.Domain.Support.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.RegularExpressions;

namespace B2Connect.Domain.Support.Services;

/// <summary>
/// ML-based security analyzer for detecting malicious or abusive requests
/// </summary>
public interface IMaliciousRequestAnalyzer
{
    Task<SecurityAnalysisResult> AnalyzeAsync(CreateFeedbackCommand command);
    Task<bool> IsMaliciousAsync(CreateFeedbackCommand command);
    Task TrainModelAsync(IEnumerable<TrainingData> trainingData);
}

/// <summary>
/// Result of security analysis
/// </summary>
public class SecurityAnalysisResult
{
    public bool IsMalicious { get; set; }
    public double ConfidenceScore { get; set; }
    public SecurityThreatLevel ThreatLevel { get; set; }
    public IReadOnlyList<string> DetectedPatterns { get; set; } = Array.Empty<string>();
    public string AnalysisSummary { get; set; } = string.Empty;
}

/// <summary>
/// Threat level classification
/// </summary>
public enum SecurityThreatLevel
{
    Safe,
    Suspicious,
    Malicious,
    Critical
}

/// <summary>
/// Training data for ML model
/// </summary>
public class TrainingData
{
    public string Content { get; set; } = string.Empty;
    public bool IsMalicious { get; set; }
    public string[] Categories { get; set; } = Array.Empty<string>();
}

/// <summary>
/// ML-based implementation of malicious request analyzer
/// </summary>
public class MLMaliciousRequestAnalyzer : IMaliciousRequestAnalyzer
{
    private readonly ILogger<MLMaliciousRequestAnalyzer> _logger;
    private MLContext? _mlContext;
    private ITransformer? _model;
    private readonly string _modelPath = "ml-models/malicious-content-model.zip";

    public MLMaliciousRequestAnalyzer(ILogger<MLMaliciousRequestAnalyzer> logger)
    {
        _logger = logger;
    }

    public async Task<SecurityAnalysisResult> AnalyzeAsync(CreateFeedbackCommand command)
    {
        try
        {
            // Load or create ML model
            await EnsureModelLoadedAsync();

            // Extract features from the command
            var features = ExtractFeatures(command);

            // Make prediction
            var prediction = PredictMalicious(features);

            // Additional pattern-based analysis
            var patterns = AnalyzePatterns(command);

            // Combine ML prediction with pattern analysis
            var combinedResult = CombineAnalyses(prediction, patterns);

            _logger.LogInformation("Security analysis completed for feedback {CorrelationId}: {ThreatLevel} (Confidence: {Confidence:P2})",
                command.CorrelationId, combinedResult.ThreatLevel, combinedResult.ConfidenceScore);

            return combinedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze security for feedback {CorrelationId}",
                command.CorrelationId);

            // Fallback to pattern-based analysis
            return AnalyzePatterns(command);
        }
    }

    public async Task<bool> IsMaliciousAsync(CreateFeedbackCommand command)
    {
        var result = await AnalyzeAsync(command);
        return result.IsMalicious;
    }

    public async Task TrainModelAsync(IEnumerable<TrainingData> trainingData)
    {
        try
        {
            _mlContext = new MLContext(seed: 0);

            // Convert training data to ML.NET format
            var data = _mlContext.Data.LoadFromEnumerable(trainingData.Select(d =>
                new MaliciousContentData
                {
                    Content = d.Content,
                    IsMalicious = d.IsMalicious,
                    Categories = string.Join(",", d.Categories)
                }));

            // Define data processing pipeline
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(MaliciousContentData.Content))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            // Train the model
            _model = pipeline.Fit(data);

            // Save the model
            Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);
            _mlContext.Model.Save(_model, data.Schema, _modelPath);

            _logger.LogInformation("ML model trained and saved successfully with {Count} training samples",
                trainingData.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to train ML model: {Message}", ex.Message);
            throw;
        }
    }

    private async Task EnsureModelLoadedAsync()
    {
        if (_model != null) return;

        try
        {
            if (File.Exists(_modelPath))
            {
                _mlContext = new MLContext();
                _model = _mlContext.Model.Load(_modelPath, out _);
                _logger.LogInformation("ML model loaded from {Path}", _modelPath);
            }
            else
            {
                // Create a basic fallback model
                await CreateFallbackModelAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load ML model, using fallback analysis");
            await CreateFallbackModelAsync();
        }
    }

    private async Task CreateFallbackModelAsync()
    {
        // Create basic training data for fallback model
        var fallbackData = new[]
        {
            new TrainingData { Content = "How do I reset my password?", IsMalicious = false, Categories = new[] { "legitimate" } },
            new TrainingData { Content = "Please help me with login", IsMalicious = false, Categories = new[] { "legitimate" } },
            new TrainingData { Content = "Buy cheap viagra now!!!", IsMalicious = true, Categories = new[] { "spam" } },
            new TrainingData { Content = "How to hack the system?", IsMalicious = true, Categories = new[] { "malicious" } },
            new TrainingData { Content = "You are an idiot and this sucks", IsMalicious = true, Categories = new[] { "abusive" } }
        };

        await TrainModelAsync(fallbackData);
    }

    private MaliciousContentFeatures ExtractFeatures(CreateFeedbackCommand command)
    {
        return new MaliciousContentFeatures
        {
            Content = command.Description,
            ContentLength = command.Description.Length,
            WordCount = command.Description.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            CapsRatio = (double)command.Description.Count(char.IsUpper) / command.Description.Length,
            PunctuationRatio = (double)command.Description.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)) / command.Description.Length,
            UrlCount = Regex.Matches(command.Description, @"https?://[^\s]+").Count,
            HasBlockedKeywords = ContainsBlockedKeywords(command.Description),
            AttachmentCount = command.Attachments.Count,
            TotalAttachmentSize = command.Attachments.Sum(a => a.Size),
            ContextCompleteness = CalculateContextCompleteness(command.Context)
        };
    }

    private MaliciousContentPrediction PredictMalicious(MaliciousContentFeatures features)
    {
        if (_mlContext == null || _model == null)
        {
            return new MaliciousContentPrediction { Probability = 0.5f, PredictedLabel = false };
        }

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<MaliciousContentFeatures, MaliciousContentPrediction>(_model);
        return predictionEngine.Predict(features);
    }

    private SecurityAnalysisResult AnalyzePatterns(CreateFeedbackCommand command)
    {
        var patterns = new List<string>();
        var threatLevel = SecurityThreatLevel.Safe;
        var confidence = 0.0;

        // Pattern-based analysis
        var content = command.Description;

        // Check for SQL injection patterns
        if (Regex.IsMatch(content, @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER)\b.*;|\bUNION\b.*\bSELECT\b)", RegexOptions.IgnoreCase))
        {
            patterns.Add("potential_sql_injection");
            threatLevel = SecurityThreatLevel.Critical;
            confidence = Math.Max(confidence, 0.95);
        }

        // Check for XSS patterns
        if (Regex.IsMatch(content, @"<script|<iframe|<object|<embed", RegexOptions.IgnoreCase))
        {
            patterns.Add("potential_xss_attack");
            threatLevel = SecurityThreatLevel.Critical;
            confidence = Math.Max(confidence, 0.95);
        }

        // Check for command injection
        if (Regex.IsMatch(content, @"(\||&|;|\$\(|\`)", RegexOptions.IgnoreCase) &&
            Regex.IsMatch(content, @"\b(rm|del|format|shutdown|reboot|kill|exec)\b", RegexOptions.IgnoreCase))
        {
            patterns.Add("potential_command_injection");
            threatLevel = SecurityThreatLevel.Critical;
            confidence = Math.Max(confidence, 0.95);
        }

        // Check for excessive URLs (potential spam/phishing)
        var urlCount = Regex.Matches(content, @"https?://[^\s]+").Count;
        if (urlCount > 3)
        {
            patterns.Add("excessive_urls");
            threatLevel = SecurityThreatLevel.Malicious;
            confidence = Math.Max(confidence, 0.8);
        }

        // Check for credential stuffing patterns
        if (Regex.IsMatch(content, @"password.*=.*|username.*=.*|login.*=.*", RegexOptions.IgnoreCase))
        {
            patterns.Add("potential_credential_stuffing");
            threatLevel = SecurityThreatLevel.Malicious;
            confidence = Math.Max(confidence, 0.85);
        }

        // Check for abusive language
        var abusiveWords = new[] { "fuck", "shit", "damn", "asshole", "bastard", "idiot", "stupid" };
        if (abusiveWords.Any(word => content.Contains(word, StringComparison.OrdinalIgnoreCase)))
        {
            patterns.Add("abusive_language");
            threatLevel = SecurityThreatLevel.Malicious;
            confidence = Math.Max(confidence, 0.75);
        }

        // Check for spam patterns
        if (Regex.IsMatch(content, @"(.)\1{4,}") || // repeated characters
            content.Count(char.IsUpper) > content.Length * 0.7) // excessive caps
        {
            patterns.Add("spam_patterns");
            threatLevel = SecurityThreatLevel.Suspicious;
            confidence = Math.Max(confidence, 0.6);
        }

        var isMalicious = threatLevel == SecurityThreatLevel.Malicious || threatLevel == SecurityThreatLevel.Critical;

        return new SecurityAnalysisResult
        {
            IsMalicious = isMalicious,
            ConfidenceScore = confidence,
            ThreatLevel = threatLevel,
            DetectedPatterns = patterns,
            AnalysisSummary = isMalicious ?
                $"Detected {patterns.Count} malicious patterns with {confidence:P0} confidence" :
                "No malicious patterns detected"
        };
    }

    private SecurityAnalysisResult CombineAnalyses(MaliciousContentPrediction mlPrediction, SecurityAnalysisResult patternResult)
    {
        // Combine ML prediction with pattern analysis
        var combinedConfidence = Math.Max(mlPrediction.Probability, patternResult.ConfidenceScore);
        var combinedThreatLevel = patternResult.ThreatLevel;

        // ML prediction can override pattern analysis if confidence is high
        if (mlPrediction.Probability > 0.8 && mlPrediction.PredictedLabel)
        {
            combinedThreatLevel = SecurityThreatLevel.Malicious;
        }
        else if (mlPrediction.Probability < 0.2 && !mlPrediction.PredictedLabel && patternResult.ThreatLevel == SecurityThreatLevel.Safe)
        {
            combinedThreatLevel = SecurityThreatLevel.Safe;
        }

        var allPatterns = patternResult.DetectedPatterns.ToList();
        if (mlPrediction.PredictedLabel)
        {
            allPatterns.Add("ml_model_prediction");
        }

        return new SecurityAnalysisResult
        {
            IsMalicious = combinedThreatLevel == SecurityThreatLevel.Malicious || combinedThreatLevel == SecurityThreatLevel.Critical,
            ConfidenceScore = combinedConfidence,
            ThreatLevel = combinedThreatLevel,
            DetectedPatterns = allPatterns,
            AnalysisSummary = $"Combined analysis: ML confidence {mlPrediction.Probability:P2}, Pattern analysis {patternResult.ThreatLevel}"
        };
    }

    private bool ContainsBlockedKeywords(string content)
    {
        var blockedKeywords = new[] {
            "password", "hack", "exploit", "sql", "injection", "xss", "script",
            "malware", "virus", "trojan", "ransomware", "phishing", "spam"
        };

        return blockedKeywords.Any(keyword =>
            content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private double CalculateContextCompleteness(CollectedContext context)
    {
        if (context == null) return 0.0;

        var completeness = 0.0;
        if (!string.IsNullOrEmpty(context.Url)) completeness += 0.2;
        if (!string.IsNullOrEmpty(context.UserAgent)) completeness += 0.2;
        if (!string.IsNullOrEmpty(context.Timestamp)) completeness += 0.2;
        if (!string.IsNullOrEmpty(context.SessionId)) completeness += 0.2;
        if (context.AdditionalData?.Any() == true) completeness += 0.2;

        return completeness;
    }
}

/// <summary>
/// ML.NET data structures
/// </summary>
public class MaliciousContentData
{
    public string Content { get; set; } = string.Empty;
    public bool IsMalicious { get; set; }
    public string Categories { get; set; } = string.Empty;
}

public class MaliciousContentFeatures
{
    public string Content { get; set; } = string.Empty;
    public float ContentLength { get; set; }
    public float WordCount { get; set; }
    public float CapsRatio { get; set; }
    public float PunctuationRatio { get; set; }
    public float UrlCount { get; set; }
    public bool HasBlockedKeywords { get; set; }
    public float AttachmentCount { get; set; }
    public float TotalAttachmentSize { get; set; }
    public float ContextCompleteness { get; set; }
}

public class MaliciousContentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool PredictedLabel { get; set; }

    public float Probability { get; set; }
}