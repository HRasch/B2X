using System;
using System.Configuration;

namespace B2Connect.ErpConnector.Services
{
    /// <summary>
    /// AI Provider Selector for ERP Connector - selects between Ollama and LMStudio
    /// </summary>
    public class ErpAiProviderSelector
    {
        private readonly bool _aiEnabled;
        private readonly string _preferredProvider;
        private readonly string _ollamaEndpoint;
        private readonly string _lmStudioEndpoint;
        private readonly string _lmStudioApiKey;

        public ErpAiProviderSelector()
        {
            // Load configuration from app.config
            _aiEnabled = GetBoolConfig("AI:Enabled", true);
            _preferredProvider = ConfigurationManager.AppSettings["AI:PreferredProvider"] ?? "ollama";
            _ollamaEndpoint = ConfigurationManager.AppSettings["AI:Ollama:Endpoint"] ?? "http://localhost:11434";
            _lmStudioEndpoint = ConfigurationManager.AppSettings["AI:LMStudio:Endpoint"] ?? "http://localhost:1234";
            _lmStudioApiKey = ConfigurationManager.AppSettings["AI:LMStudio:ApiKey"] ?? "";
        }

        /// <summary>
        /// Get the AI provider for tenant data processing
        /// </summary>
        public ILocalAiProvider GetProviderForTenant(string tenantId)
        {
            if (!_aiEnabled)
            {
                throw new InvalidOperationException("AI processing is disabled in configuration");
            }

            return _preferredProvider.ToLower() switch
            {
                "ollama" => new OllamaProvider(_ollamaEndpoint),
                "lmstudio" => new LmStudioProvider(_lmStudioEndpoint, _lmStudioApiKey),
                _ => new OllamaProvider(_ollamaEndpoint) // Default to Ollama
            };
        }

        /// <summary>
        /// Create AI service for tenant data processing
        /// </summary>
        public LocalAiService CreateAiServiceForTenant(string tenantId)
        {
            if (!_aiEnabled)
            {
                return new LocalAiService(new OllamaProvider(), false);
            }

            var provider = GetProviderForTenant(tenantId);
            return new LocalAiService(provider, true);
        }

        /// <summary>
        /// Check if AI is enabled for the connector
        /// </summary>
        public bool IsAiEnabled => _aiEnabled;

        /// <summary>
        /// Get preferred AI provider name
        /// </summary>
        public string PreferredProvider => _preferredProvider;

        private bool GetBoolConfig(string key, bool defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(value) ? defaultValue : bool.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}