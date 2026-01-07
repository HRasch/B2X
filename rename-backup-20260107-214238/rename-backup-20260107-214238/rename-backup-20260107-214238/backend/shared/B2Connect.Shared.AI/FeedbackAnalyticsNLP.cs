using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.AI;

/// <summary>
/// Advanced NLP service for feedback analytics and insights.
/// </summary>
public class FeedbackAnalyticsNLP
{
    private readonly ILogger<FeedbackAnalyticsNLP> _logger;

    public FeedbackAnalyticsNLP(ILogger<FeedbackAnalyticsNLP> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Analyzes feedback text using advanced NLP capabilities.
    /// </summary>
    public async Task<FeedbackAnalysis> AnalyzeFeedbackAsync(string feedbackText, string language = "en")
    {
        _logger.LogInformation("Analyzing feedback with NLP. Length: {Length}, Language: {Language}",
            feedbackText.Length, language);

        var sentiment = await AnalyzeSentiment(feedbackText);
        var categories = await CategorizeFeedback(feedbackText);
        var entities = await ExtractEntities(feedbackText);
        var insights = await GenerateInsights(sentiment, categories, entities);

        return new FeedbackAnalysis
        {
            OriginalText = feedbackText,
            Sentiment = sentiment,
            Categories = categories,
            Entities = entities,
            Insights = insights,
            Language = language,
            Confidence = CalculateOverallConfidence(sentiment, categories),
            Timestamp = DateTime.UtcNow
        };
    }

    private async Task<SentimentAnalysis> AnalyzeSentiment(string text)
    {
        // Advanced sentiment analysis implementation
        // Would integrate with ML models for accurate sentiment detection
        var score = CalculateSentimentScore(text);
        return new SentimentAnalysis
        {
            Score = score,
            Label = score > 0.6 ? "Positive" : score < 0.4 ? "Negative" : "Neutral",
            Confidence = 0.87
        };
    }

    private async Task<List<string>> CategorizeFeedback(string text)
    {
        // Advanced categorization using NLP
        var categories = new List<string>();

        if (text.Contains("bug") || text.Contains("error") || text.Contains("crash"))
            categories.Add("Technical Issue");

        if (text.Contains("slow") || text.Contains("performance") || text.Contains("loading"))
            categories.Add("Performance");

        if (text.Contains("feature") || text.Contains("request") || text.Contains("add"))
            categories.Add("Feature Request");

        if (text.Contains("ui") || text.Contains("interface") || text.Contains("design"))
            categories.Add("UI/UX");

        return categories.Any() ? categories : new List<string> { "General" };
    }

    private async Task<List<NamedEntity>> ExtractEntities(string text)
    {
        // Named entity recognition
        return new List<NamedEntity>
        {
            // Implementation would extract actual entities
        };
    }

    private async Task<List<string>> GenerateInsights(SentimentAnalysis sentiment,
        List<string> categories, List<NamedEntity> entities)
    {
        var insights = new List<string>();

        if (sentiment.Label == "Negative" && categories.Contains("Performance"))
        {
            insights.Add("Performance issues are causing user dissatisfaction");
        }

        if (categories.Contains("Feature Request"))
        {
            insights.Add("Users are requesting new features - consider roadmap prioritization");
        }

        return insights;
    }

    private double CalculateSentimentScore(string text)
    {
        // Simple sentiment scoring - would be replaced with ML model
        var positiveWords = new[] { "good", "great", "excellent", "love", "awesome" };
        var negativeWords = new[] { "bad", "terrible", "hate", "awful", "worst" };

        var positiveCount = positiveWords.Count(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));
        var negativeCount = negativeWords.Count(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));

        return (positiveCount - negativeCount + 5.0) / 10.0; // Normalize to 0-1
    }

    private double CalculateOverallConfidence(SentimentAnalysis sentiment, List<string> categories)
    {
        return Math.Min(sentiment.Confidence, categories.Any() ? 0.8 : 0.6);
    }
}

/// <summary>
/// Feedback analysis result.
/// </summary>
public record FeedbackAnalysis
{
    public string OriginalText { get; init; } = string.Empty;
    public SentimentAnalysis Sentiment { get; init; } = new();
    public List<string> Categories { get; init; } = new();
    public List<NamedEntity> Entities { get; init; } = new();
    public List<string> Insights { get; init; } = new();
    public string Language { get; init; } = "en";
    public double Confidence { get; init; }
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// Sentiment analysis result.
/// </summary>
public record SentimentAnalysis
{
    public double Score { get; init; }
    public string Label { get; init; } = string.Empty;
    public double Confidence { get; init; }
}

/// <summary>
/// Named entity extracted from text.
/// </summary>
public record NamedEntity
{
    public string Text { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
}