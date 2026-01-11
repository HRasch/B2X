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
    /// LMStudio provider for local AI processing (OpenAI-compatible API)
    /// </summary>
    public class LmStudioProvider : ILocalAiProvider
    {
        private readonly HttpClient _httpClient;
        private readonly JavaScriptSerializer _serializer;
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly int _timeoutSeconds;

        public string ProviderName => "lmstudio";

        public LmStudioProvider(string endpoint = "http://localhost:1234", string apiKey = "", int timeoutSeconds = 30)
        {
            _endpoint = endpoint.TrimEnd('/');
            _apiKey = apiKey;
            _timeoutSeconds = timeoutSeconds;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);

            // Add API key header if provided
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }

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
                    messages = new[]
                    {
                        new { role = "system", content = prompt },
                        new { role = "user", content = userMessage }
                    },
                    temperature = 0.7,
                    max_tokens = 2048,
                    stream = false
                };

                var jsonRequest = _serializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_endpoint}/v1/chat/completions", content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = _serializer.Deserialize<dynamic>(responseContent);

                var responseText = "";
                var tokensUsed = 0;

                if (result["choices"] != null && result["choices"].Length > 0)
                {
                    responseText = result["choices"][0]["message"]["content"]?.ToString() ?? "";
                }

                if (result["usage"] != null)
                {
                    tokensUsed = Convert.ToInt32(result["usage"]["total_tokens"] ?? 0);
                }

                return new AiResponse
                {
                    Content = responseText,
                    TokensUsed = tokensUsed,
                    Model = model
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"LMStudio request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if LMStudio is available
        /// </summary>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpoint}/v1/models");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get available models from LMStudio
        /// </summary>
        public async Task<List<string>> GetAvailableModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpoint}/v1/models");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = _serializer.Deserialize<dynamic>(content);
                var models = new List<string>();

                if (result["data"] != null)
                {
                    foreach (dynamic model in result["data"])
                    {
                        models.Add(model["id"]?.ToString() ?? "");
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
