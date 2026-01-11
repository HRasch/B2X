using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace B2X.ErpConnector.Services
{
    /// <summary>
    /// AI response model
    /// </summary>
    public class AiResponse
    {
        public string Content { get; set; }
        public int TokensUsed { get; set; }
        public string Model { get; set; }
    }

    /// <summary>
    /// AI provider interface for local AI processing
    /// </summary>
    public interface ILocalAiProvider
    {
        string ProviderName { get; }
        Task<AiResponse> ExecuteChatCompletionAsync(
            string tenantId,
            string model,
            string prompt,
            string userMessage,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// AI service for processing tenant data related prompts using local AI providers
    /// </summary>
    public class LocalAiService
    {
        private readonly ILocalAiProvider _provider;
        private readonly bool _aiEnabled;
        private readonly PerformanceMonitor _performanceMonitor;

        public LocalAiService(ILocalAiProvider provider, bool aiEnabled = true)
            : this(provider, aiEnabled, null)
        {
        }

        public LocalAiService(ILocalAiProvider provider, bool aiEnabled, PerformanceMonitor performanceMonitor)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _aiEnabled = aiEnabled;
            _performanceMonitor = performanceMonitor;
        }

        /// <summary>
        /// Process tenant data using AI for validation, correction, or enrichment
        /// </summary>
        public async Task<AiResponse> ProcessTenantDataAsync(
            string tenantId,
            string operation,
            string data,
            CancellationToken cancellationToken = default)
        {
            if (!_aiEnabled)
            {
                return new AiResponse
                {
                    Content = "AI processing disabled",
                    TokensUsed = 0,
                    Model = "disabled"
                };
            }

            return await (_performanceMonitor?.TimeOperationAsync($"AI_ProcessTenantData_{tenantId}_{operation}", async () =>
            {
                var prompt = GetPromptForOperation(operation);
                var userMessage = $"Tenant: {tenantId}\nData: {data}";

                return await _provider.ExecuteChatCompletionAsync(
                    tenantId,
                    GetDefaultModel(),
                    prompt,
                    userMessage,
                    cancellationToken);
            }) ?? Task.Run(async () =>
            {
                var prompt = GetPromptForOperation(operation);
                var userMessage = $"Tenant: {tenantId}\nData: {data}";

                return await _provider.ExecuteChatCompletionAsync(
                    tenantId,
                    GetDefaultModel(),
                    prompt,
                    userMessage,
                    cancellationToken);
            }));
        }

        /// <summary>
        /// Validate ERP data using AI
        /// </summary>
        public async Task<AiResponse> ValidateErpDataAsync(
            string tenantId,
            string erpType,
            string data,
            CancellationToken cancellationToken = default)
        {
            if (!_aiEnabled)
            {
                return new AiResponse
                {
                    Content = "AI validation disabled",
                    TokensUsed = 0,
                    Model = "disabled"
                };
            }

            var prompt = $"Validate the following {erpType} ERP data for tenant {tenantId}. " +
                        "Check for data quality issues, format problems, and business rule violations. " +
                        "Provide specific recommendations for any issues found.";
            var userMessage = data;

            return await _provider.ExecuteChatCompletionAsync(
                tenantId,
                GetDefaultModel(),
                prompt,
                userMessage,
                cancellationToken);
        }

        /// <summary>
        /// Correct ERP data using AI suggestions
        /// </summary>
        public async Task<AiResponse> CorrectErpDataAsync(
            string tenantId,
            string erpType,
            string data,
            string validationErrors,
            CancellationToken cancellationToken = default)
        {
            if (!_aiEnabled)
            {
                return new AiResponse
                {
                    Content = "AI correction disabled",
                    TokensUsed = 0,
                    Model = "disabled"
                };
            }

            var prompt = $"Correct the following {erpType} ERP data for tenant {tenantId} based on the validation errors provided. " +
                        "Provide corrected data in the same format, with explanations for each change made.";
            var userMessage = $"Validation Errors:\n{validationErrors}\n\nOriginal Data:\n{data}";

            return await _provider.ExecuteChatCompletionAsync(
                tenantId,
                GetDefaultModel(),
                prompt,
                userMessage,
                cancellationToken);
        }

        private string GetPromptForOperation(string operation)
        {
            return operation.ToLower() switch
            {
                "validate" => "Validate the provided tenant data for consistency, completeness, and business rules compliance.",
                "enrich" => "Enrich the provided tenant data with additional insights and recommendations.",
                "transform" => "Transform the provided tenant data according to best practices and standards.",
                _ => "Process the provided tenant data according to the specified operation."
            };
        }

        private string GetDefaultModel()
        {
            // Default model based on provider
            return _provider.ProviderName.ToLower() switch
            {
                "ollama" => "deepseek-coder:33b",
                "lmstudio" => "local-model",
                _ => "default-model"
            };
        }
    }
}