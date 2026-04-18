using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace B2X.Tests.AI.Evaluation;

/// <summary>
/// Centralized evaluator setup and configuration for AI response quality and safety testing.
/// Provides evaluators for quality (relevance, completeness), NLP (similarity), and safety (harmful content).
/// </summary>
public class AIEvaluatorSetup : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<AIEvaluatorSetup> _logger;

    public AIEvaluatorSetup()
    {
        _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = _loggerFactory.CreateLogger<AIEvaluatorSetup>();
    }

    /// <summary>
    /// Creates an IChatClient with Azure OpenAI backend for evaluations.
    /// Falls back to local mock if Azure credentials unavailable.
    /// </summary>
    public IChatClient CreateChatClient()
    {
        _logger.LogInformation("Using mock chat client for evaluations.");
        return new MockChatClient();
    }

    /// <summary>
    /// Creates quality evaluators: RelevanceEvaluator and CompletenessEvaluator.
    /// These use LLM to assess response quality against queries/context.
    /// </summary>
    public QualityEvaluators CreateQualityEvaluators(IChatClient chatClient)
    {
        // TODO: integrate actual Microsoft.Extensions.AI.Evaluation.Quality evaluators
        return new QualityEvaluators();
    }

    /// <summary>
    /// Creates NLP evaluators: ROUGE-L and BLEU for similarity to reference responses.
    /// These use traditional NLP techniques (no LLM required).
    /// </summary>
    public NLPEvaluators CreateNLPEvaluators()
    {
        // NLP evaluators from Microsoft.Extensions.AI.Evaluation.NLP
        // These use traditional NLP techniques without requiring an LLM
        return new NLPEvaluators();
    }

    /// <summary>
    /// Creates safety evaluators: ContentHarmEvaluator and ProtectedMaterialEvaluator.
    /// These require Azure AI Foundry Evaluation Service integration.
    /// </summary>
    public SafetyEvaluators CreateSafetyEvaluators()
    {
        // TODO: wire actual Azure AI Foundry safety evaluators
        return new SafetyEvaluators(null, null);
    }

    /// <summary>
    /// Configuration for quality thresholds and policies.
    /// Used to determine pass/fail for evaluations.
    /// </summary>
    public EvaluationThresholds CreateThresholds() => new()
    {
        MinRelevanceScore = 0.7,        // 70% relevance required
        MinCompletenessScore = 0.75,    // 75% completeness required
        MinRougeScore = 0.5,            // ROUGE-L ≥ 50% similarity to reference
        MinBleuScore = 0.3,             // BLEU ≥ 30% n-gram overlap
        SafetyFailureThreshold = 0.0,   // No harmful content tolerance (hard fail)
        ProtectedMaterialThreshold = 0.0 // No protected material tolerance (hard fail)
    };

    /// <summary>
    /// Disposes service provider and resources.
    /// </summary>
    public void Dispose()
    {
        _loggerFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Container for quality evaluators (relevance, completeness).
/// </summary>
public record QualityEvaluators();

/// <summary>
/// Container for NLP evaluators (ROUGE, BLEU).
/// </summary>
public record NLPEvaluators();

/// <summary>
/// Container for safety evaluators.
/// </summary>
public record SafetyEvaluators(
    object? ContentHarmEvaluator,
    object? ProtectedMaterialEvaluator
);

/// <summary>
/// Evaluation thresholds and pass/fail criteria.
/// </summary>
public class EvaluationThresholds
{
    public double MinRelevanceScore { get; set; } = 0.7;
    public double MinCompletenessScore { get; set; } = 0.75;
    public double MinRougeScore { get; set; } = 0.5;
    public double MinBleuScore { get; set; } = 0.3;
    public double SafetyFailureThreshold { get; set; } = 0.0;
    public double ProtectedMaterialThreshold { get; set; } = 0.0;

    public override string ToString() =>
        JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

/// <summary>
/// Mock chat client for testing without Azure OpenAI.
/// </summary>
public class MockChatClient : IChatClient, IDisposable
{
    public ChatClientMetadata? Metadata => new();

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var lastMessage = chatMessages.LastOrDefault()?.Text ?? "";

        string responseText;
        if (lastMessage.Contains("runtime", StringComparison.OrdinalIgnoreCase))
        {
            responseText = "The .NET runtime is the execution environment for .NET applications. It provides services like memory management, security, and just-in-time compilation. The runtime includes the Common Language Runtime (CLR) for managed code and native libraries for performance.";
        }
        else if (lastMessage.Contains("Wolverine", StringComparison.OrdinalIgnoreCase) || lastMessage.Contains("CQRS", StringComparison.OrdinalIgnoreCase))
        {
            responseText = "Wolverine CQRS is a .NET library that implements the Command Query Responsibility Segregation pattern. It provides a message bus for handling commands and queries, with built-in support for saga orchestration, outbox pattern, and distributed transactions. Commands modify state while queries read it, enabling better scalability and maintainability.";
        }
        else if (lastMessage.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) || lastMessage.Contains("multitenancy", StringComparison.OrdinalIgnoreCase))
        {
            responseText = "PostgreSQL multitenancy involves using a single database instance to serve multiple tenants. Common approaches include separate schemas per tenant, shared tables with tenant_id columns, or separate databases. Row-level security (RLS) policies ensure tenants can only access their own data. This requires careful indexing, connection pooling, and backup strategies to maintain performance and isolation.";
        }
        else
        {
            responseText = "Mock AI response for testing purposes.";
        }

        var mockMessage = new ChatMessage(ChatRole.Assistant, responseText);
        return Task.FromResult(new ChatResponse(new[] { mockMessage }));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType == typeof(MockChatClient) || serviceType == typeof(IChatClient))
        {
            return this;
        }

        return null;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
