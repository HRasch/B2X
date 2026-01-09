using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace B2X.ErpConnector.Services
{
    /// <summary>
    /// Ollama provider for local AI processing
    /// </summary>
    public class OllamaProvider : ILocalAiProvider
    {
        private readonly HttpClient _httpClient;
        private readonly JavaScriptSerializer _serializer;
        private readonly string _endpoint;
        private readonly int _timeoutSeconds;

        public string ProviderName => "ollama";

        public OllamaProvider(string endpoint = "http://localhost:11434", int timeoutSeconds = 30)
        {
            _endpoint = endpoint.TrimEnd('/');
            _timeoutSeconds = timeoutSeconds;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
            _serializer = new JavaScriptSerializer();
        }

        public async Task<AiResponse> ExecuteChatCompletionAsync(
            string tenantId,
            string model,
            string prompt,
            string userMessage,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new
                {
                    model = model,
                    prompt = $"{prompt}\n\n{userMessage}",
                    stream = false
                };

                var jsonRequest = _serializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_endpoint}/api/generate", content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = _serializer.Deserialize<dynamic>(responseContent);

                var responseText = result["response"]?.ToString() ?? string.Empty;
                var evalCount = Convert.ToInt32(result["eval_count"] ?? 0);

                return new AiResponse
                {
                    Content = responseText,
                    TokensUsed = evalCount,
                    Model = model
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ollama request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if Ollama is available
        /// </summary>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpoint}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get available models from Ollama
        /// </summary>
        public async Task<List<string>> GetAvailableModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpoint}/api/tags");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = _serializer.Deserialize<dynamic>(content);
                var models = new List<string>();

                if (result["models"] != null)
                {
                    foreach (dynamic model in result["models"])
                    {
                        models.Add(model["name"]?.ToString() ?? "");
                    }
                }

                return models;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}