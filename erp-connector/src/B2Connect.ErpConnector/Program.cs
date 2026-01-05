using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using B2Connect.ErpConnector.Models;
using B2Connect.ErpConnector.Services;

namespace B2Connect.ErpConnector
{
    /// <summary>
    /// Entry point for the ERP Connector service.
    /// Simplified version using HttpListener for testing without external dependencies.
    /// </summary>
    public class Program
    {
        private static HttpListener _listener;
        private static CancellationTokenSource _cts;
        private static ManualResetEvent _exitEvent;
        private static EnventaErpService _erpService;
        private static JavaScriptSerializer _serializer;
        private static ApiKeyManager _apiKeyManager;
        private static string _legacyApiKey;
        private static string _baseAddress;
        private static HashSet<string> _allowedBusinessUnits;
        private static bool _useTenantKeys;

        public static int Main(string[] args)
        {
            Console.WriteLine("B2Connect ERP Connector starting...");

            // Check if running in service/daemon mode
            var serviceMode = args.Length > 0 && args[0] == "--service";
            var benchmarkMode = args.Length > 0 && args[0] == "--benchmark";

            if (benchmarkMode)
            {
                return RunBenchmarkMode(args);
            }

            try
            {
                _cts = new CancellationTokenSource();
                _exitEvent = new ManualResetEvent(false);
                _serializer = new JavaScriptSerializer();

                // Load configuration
                _baseAddress = ConfigurationManager.AppSettings["BaseAddress"] ?? "http://localhost:5080";
                _legacyApiKey = ConfigurationManager.AppSettings["ApiKey"] ?? Environment.GetEnvironmentVariable("ERP_CONNECTOR_API_KEY") ?? "";
                _useTenantKeys = bool.TryParse(ConfigurationManager.AppSettings["UseTenantKeys"], out var useTenant) && useTenant;

                // Load allowed business units (empty = unrestricted)
                var allowedBuConfig = ConfigurationManager.AppSettings["AllowedBusinessUnits"] ?? "";
                _allowedBusinessUnits = ParseAllowedBusinessUnits(allowedBuConfig);

                // Initialize API key manager
                _apiKeyManager = new ApiKeyManager();

                // First-run: generate admin key if no keys exist
                if (!_apiKeyManager.HasKeys())
                {
                    Console.WriteLine("\n=== FIRST RUN: Generating admin key ===");
                    var adminKey = _apiKeyManager.SetAdminKey();
                    Console.WriteLine($"ADMIN KEY (save this, shown only once):");
                    Console.WriteLine($"  {adminKey}");
                    Console.WriteLine("==========================================\n");
                }

                // Initialize AI service if enabled
                LocalAiService aiService = null;
                var aiProviderSelector = new ErpAiProviderSelector();
                if (aiProviderSelector.IsAiEnabled)
                {
                    try
                    {
                        aiService = aiProviderSelector.CreateAiServiceForTenant("default", performanceMonitor);
                        Console.WriteLine($"AI processing enabled using {aiProviderSelector.PreferredProvider} provider");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: AI service initialization failed: {ex.Message}");
                        Console.WriteLine("ERP service will continue without AI capabilities");
                    }
                }
                else
                {
                    Console.WriteLine("AI processing disabled in configuration");
                }

                // Initialize performance monitoring
                var performanceEnabled = bool.TryParse(ConfigurationManager.AppSettings["PerformanceMonitoring:Enabled"], out var perfEnabled) && perfEnabled;
                PerformanceMonitor performanceMonitor = null;
                if (performanceEnabled)
                {
                    var reportingInterval = TimeSpan.FromMinutes(5); // Default 5 minutes
                    if (TimeSpan.TryParse(ConfigurationManager.AppSettings["PerformanceMonitoring:ReportingInterval"], out var interval))
                    {
                        reportingInterval = interval;
                    }
                    performanceMonitor = new PerformanceMonitor(true, reportingInterval);
                    Console.WriteLine($"Performance monitoring enabled (reporting every {reportingInterval.TotalMinutes} minutes)");
                }
                else
                {
                    Console.WriteLine("Performance monitoring disabled");
                }

                // Initialize ERP service with AI and performance monitoring capabilities
                _erpService = new EnventaErpService(aiService, performanceMonitor);

                // Start the HTTP listener
                StartHttpListener();

                Console.WriteLine($"ERP Connector started successfully on {_baseAddress}.");

                // Show security mode
                if (_useTenantKeys)
                {
                    Console.WriteLine("Security: Tenant-coupled API keys (production mode)");
                    var keyCount = _apiKeyManager.ListKeys().Count;
                    Console.WriteLine($"Registered tenant keys: {keyCount}");
                }
                else if (!string.IsNullOrEmpty(_legacyApiKey))
                {
                    Console.WriteLine("Security: Legacy single API key mode");
                }
                else
                {
                    Console.WriteLine("WARNING: No API key configured - running in DEVELOPMENT mode (no auth)");
                }

                if (_allowedBusinessUnits.Count > 0)
                {
                    Console.WriteLine($"Restricted to business units: {string.Join(", ", _allowedBusinessUnits)}");
                }
                else
                {
                    Console.WriteLine("Business units: unrestricted (all allowed)");
                }

                // Handle Ctrl+C
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    _exitEvent.Set();
                };

                if (serviceMode)
                {
                    // In service mode, just wait forever
                    Console.WriteLine("Running in service mode. Send SIGINT/Ctrl+C to stop.");
                    _exitEvent.WaitOne();
                }
                else
                {
                    // In interactive mode, use Console.ReadLine
                    Console.WriteLine("Press Enter or Ctrl+C to stop.");

                    // Read synchronously - simpler and more reliable
                    try
                    {
                        Console.ReadLine();
                    }
                    catch { }

                    _exitEvent.Set();
                }

                Console.WriteLine("ERP Connector stopping...");
                StopHttpListener();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error during startup: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        private static void StartHttpListener()
        {
            _listener = new HttpListener();
            // Use configured base address with trailing slash
            var prefix = _baseAddress.TrimEnd('/') + "/";
            _listener.Prefixes.Add(prefix);
            _listener.Start();

            // Start listening for requests in a background task
            Task.Run(() => ListenForRequests(_cts.Token));
        }

        private static void StopHttpListener()
        {
            _cts.Cancel();
            _listener?.Stop();
            _listener?.Close();
        }

        private static async Task ListenForRequests(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    // Handle request in background
                    Task.Run(() => HandleRequest(context), token);
                }
                catch (HttpListenerException) when (token.IsCancellationRequested)
                {
                    // Expected when stopping
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling request: {ex.Message}");
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;
                var clientIp = request.RemoteEndPoint?.Address?.ToString();

                Console.WriteLine($"{request.HttpMethod} {request.Url.PathAndQuery} from {clientIp}");

                string path = request.Url.AbsolutePath.ToLowerInvariant();
                string originalPath = request.Url.AbsolutePath; // Preserve case for key prefixes

                // Health endpoint - no auth required
                if (path == "/health")
                {
                    WriteJsonResponse(response, new { status = "healthy", timestamp = DateTime.UtcNow.ToString("o"), mode = _useTenantKeys ? "tenant-keys" : "legacy" });
                    return;
                }

                // Admin endpoints - require admin key (pass original path for case-sensitive key prefixes)
                if (path.StartsWith("/admin/"))
                {
                    HandleAdminRequest(request, response, clientIp, originalPath);
                    return;
                }

                // Get business unit from header or query param
                var businessUnit = request.Headers["X-Business-Unit"] ?? request.QueryString["businessUnit"];

                // Validate API key
                var authResult = ValidateApiKey(request, businessUnit, clientIp);
                if (!authResult.IsValid)
                {
                    response.StatusCode = 401;
                    WriteJsonResponse(response, new { error = "Unauthorized", message = authResult.ErrorMessage });
                    return;
                }

                // Get tenant from key (tenant-coupled mode) or header (legacy mode)
                string tenantId;
                if (authResult.Key != null)
                {
                    tenantId = authResult.Key.TenantId;
                    // Check business unit against key's allowed list
                    if (!string.IsNullOrEmpty(businessUnit) && !authResult.Key.IsBusinessUnitAllowed(businessUnit))
                    {
                        response.StatusCode = 403;
                        WriteJsonResponse(response, new { error = "Forbidden", message = $"Business unit '{businessUnit}' not authorized for this key" });
                        return;
                    }
                }
                else
                {
                    tenantId = request.Headers["X-Tenant-Id"] ?? request.QueryString["tenant"] ?? "default";
                    // Legacy mode: check global BU restrictions
                    if (!string.IsNullOrEmpty(businessUnit) && !IsBusinessUnitAllowed(businessUnit))
                    {
                        response.StatusCode = 403;
                        WriteJsonResponse(response, new { error = "Forbidden", message = $"Business unit '{businessUnit}' is not allowed" });
                        return;
                    }
                }

                // Route API requests
                if (path.StartsWith("/api/articles"))
                {
                    HandleArticlesRequest(request, response, tenantId, authResult);
                }
                else if (path.StartsWith("/api/customers"))
                {
                    HandleCustomersRequest(request, response, tenantId, authResult);
                }
                else if (path.StartsWith("/api/orders"))
                {
                    HandleOrdersRequest(request, response, tenantId, authResult);
                }
                else if (path.StartsWith("/api/ai"))
                {
                    HandleAiRequest(request, response, tenantId, authResult);
                }
                else
                {
                    response.StatusCode = 404;
                    WriteJsonResponse(response, new { error = "Not Found", path = request.Url.AbsolutePath });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in request handler: {ex.Message}");
                context.Response.StatusCode = 500;
                WriteJsonResponse(context.Response, new { error = "Internal Server Error", message = ex.Message });
            }
            finally
            {
                context.Response.OutputStream.Close();
            }
        }

        private static ApiKeyValidationResult ValidateApiKey(HttpListenerRequest request, string businessUnit, string clientIp)
        {
            // Extract API key from request
            string apiKey = null;

            var authHeader = request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    apiKey = authHeader.Substring(7).Trim();
                else if (authHeader.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
                    apiKey = authHeader.Substring(7).Trim();
            }

            if (string.IsNullOrEmpty(apiKey))
                apiKey = request.Headers["X-Api-Key"];

            // Tenant-coupled key mode (production)
            if (_useTenantKeys)
            {
                if (string.IsNullOrEmpty(apiKey))
                    return ApiKeyValidationResult.Failure("API key is required");

                return _apiKeyManager.ValidateKey(apiKey, businessUnit, clientIp);
            }

            // Legacy mode: single global key or no auth
            if (string.IsNullOrEmpty(_legacyApiKey))
                return ApiKeyValidationResult.AdminSuccess(); // Dev mode - allow all

            if (apiKey == _legacyApiKey)
                return ApiKeyValidationResult.AdminSuccess();

            return ApiKeyValidationResult.Failure("Invalid API key");
        }

        private static void HandleAdminRequest(HttpListenerRequest request, HttpListenerResponse response, string clientIp, string originalPath)
        {
            // Extract admin key
            string apiKey = null;
            var authHeader = request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    apiKey = authHeader.Substring(7).Trim();
                else if (authHeader.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
                    apiKey = authHeader.Substring(7).Trim();
            }
            if (string.IsNullOrEmpty(apiKey))
                apiKey = request.Headers["X-Api-Key"];

            // Validate admin key
            var authResult = _apiKeyManager.ValidateKey(apiKey, null, clientIp);
            if (!authResult.IsAdmin)
            {
                response.StatusCode = 403;
                WriteJsonResponse(response, new { error = "Forbidden", message = "Admin key required" });
                return;
            }

            string path = request.Url.AbsolutePath.ToLowerInvariant();

            // POST /admin/keys/generate - Generate new tenant key
            if (path == "/admin/keys/generate" && request.HttpMethod == "POST")
            {
                HandleGenerateKey(request, response);
            }
            // GET /admin/keys - List all keys
            else if (path == "/admin/keys" && request.HttpMethod == "GET")
            {
                var tenantFilter = request.QueryString["tenant"];
                var keys = _apiKeyManager.ListKeys(tenantFilter);
                WriteJsonResponse(response, new { keys = keys });
            }
            // POST /admin/keys/rotate - Rotate a key
            else if (path == "/admin/keys/rotate" && request.HttpMethod == "POST")
            {
                HandleRotateKey(request, response);
            }
            // DELETE /admin/keys/{prefix} - Deactivate a key (use original path for case-sensitive key prefix)
            else if (path.StartsWith("/admin/keys/") && !path.EndsWith("/credentials") && request.HttpMethod == "DELETE")
            {
                var keyPrefix = originalPath.Substring("/admin/keys/".Length);
                if (_apiKeyManager.DeactivateKey(keyPrefix))
                {
                    WriteJsonResponse(response, new { success = true, message = $"Key {keyPrefix} deactivated" });
                }
                else
                {
                    response.StatusCode = 404;
                    WriteJsonResponse(response, new { error = "Not Found", message = $"Key {keyPrefix} not found" });
                }
            }
            // POST /admin/keys/admin/regenerate - Regenerate admin key
            else if (path == "/admin/keys/admin/regenerate" && request.HttpMethod == "POST")
            {
                var newAdminKey = _apiKeyManager.SetAdminKey();
                WriteJsonResponse(response, new
                {
                    success = true,
                    message = "Admin key regenerated - save this, shown only once",
                    adminKey = newAdminKey
                });
            }
            // POST /admin/keys/{prefix}/credentials - Set ERP credentials for a key (use original path)
            else if (path.StartsWith("/admin/keys/") && path.EndsWith("/credentials") && request.HttpMethod == "POST")
            {
                HandleSetErpCredentials(request, response, originalPath);
            }
            // DELETE /admin/keys/{prefix}/credentials - Remove ERP credentials from a key (use original path)
            else if (path.StartsWith("/admin/keys/") && path.EndsWith("/credentials") && request.HttpMethod == "DELETE")
            {
                var keyPrefix = originalPath.Replace("/admin/keys/", "").Replace("/credentials", "");
                if (_apiKeyManager.RemoveErpCredentials(keyPrefix))
                {
                    WriteJsonResponse(response, new { success = true, message = $"ERP credentials removed from key {keyPrefix}" });
                }
                else
                {
                    response.StatusCode = 404;
                    WriteJsonResponse(response, new { error = "Not Found", message = $"Key {keyPrefix} not found" });
                }
            }
            else
            {
                response.StatusCode = 404;
                WriteJsonResponse(response, new { error = "Not Found", path = path });
            }
        }

        private static void HandleGenerateKey(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                // Read request body
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    var body = reader.ReadToEnd();
                    var data = _serializer.Deserialize<Dictionary<string, object>>(body);

                    var tenantId = data.ContainsKey("tenantId") ? data["tenantId"]?.ToString() : null;
                    var name = data.ContainsKey("name") ? data["name"]?.ToString() : "Default";

                    List<string> allowedBus = null;
                    if (data.ContainsKey("allowedBusinessUnits") && data["allowedBusinessUnits"] is object[] busArray)
                    {
                        allowedBus = busArray.Select(b => b?.ToString()).Where(b => !string.IsNullOrEmpty(b)).ToList();
                    }

                    DateTime? expiresAt = null;
                    if (data.ContainsKey("expiresAt") && data["expiresAt"] != null)
                    {
                        if (DateTime.TryParse(data["expiresAt"].ToString(), out var exp))
                            expiresAt = exp;
                    }

                    if (string.IsNullOrWhiteSpace(tenantId))
                    {
                        response.StatusCode = 400;
                        WriteJsonResponse(response, new { error = "Bad Request", message = "tenantId is required" });
                        return;
                    }

                    var newKey = _apiKeyManager.GenerateKey(tenantId, name, allowedBus, expiresAt);
                    WriteJsonResponse(response, new
                    {
                        success = true,
                        message = "Key generated - save this, shown only once",
                        tenantId = tenantId,
                        apiKey = newKey,
                        allowedBusinessUnits = allowedBus ?? new List<string>(),
                        expiresAt = expiresAt?.ToString("o")
                    });
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to generate key", message = ex.Message });
            }
        }

        private static void HandleRotateKey(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    var body = reader.ReadToEnd();
                    var data = _serializer.Deserialize<Dictionary<string, object>>(body);

                    var keyPrefix = data.ContainsKey("keyPrefix") ? data["keyPrefix"]?.ToString() : null;

                    if (string.IsNullOrWhiteSpace(keyPrefix))
                    {
                        response.StatusCode = 400;
                        WriteJsonResponse(response, new { error = "Bad Request", message = "keyPrefix is required" });
                        return;
                    }

                    var newKey = _apiKeyManager.RotateKey(keyPrefix);
                    WriteJsonResponse(response, new
                    {
                        success = true,
                        message = "Key rotated - save new key, shown only once",
                        oldKeyPrefix = keyPrefix,
                        newApiKey = newKey
                    });
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to rotate key", message = ex.Message });
            }
        }

        private static void HandleSetErpCredentials(HttpListenerRequest request, HttpListenerResponse response, string path)
        {
            try
            {
                // Extract key prefix from path: /admin/keys/{prefix}/credentials
                var keyPrefix = path.Replace("/admin/keys/", "").Replace("/credentials", "");

                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    var body = reader.ReadToEnd();
                    var data = _serializer.Deserialize<Dictionary<string, object>>(body);

                    var erpUsername = data.ContainsKey("erpUsername") ? data["erpUsername"]?.ToString() : null;
                    var erpPassword = data.ContainsKey("erpPassword") ? data["erpPassword"]?.ToString() : null;
                    var defaultBu = data.ContainsKey("defaultBusinessUnit") ? data["defaultBusinessUnit"]?.ToString() : null;

                    if (string.IsNullOrWhiteSpace(erpUsername) || string.IsNullOrWhiteSpace(erpPassword))
                    {
                        response.StatusCode = 400;
                        WriteJsonResponse(response, new { error = "Bad Request", message = "erpUsername and erpPassword are required" });
                        return;
                    }

                    if (_apiKeyManager.SetErpCredentials(keyPrefix, erpUsername, erpPassword, defaultBu))
                    {
                        WriteJsonResponse(response, new
                        {
                            success = true,
                            message = "ERP credentials configured - credentials are encrypted and stored locally only",
                            keyPrefix = keyPrefix,
                            securityNote = "Credentials can only be decrypted on this machine"
                        });
                    }
                    else
                    {
                        response.StatusCode = 404;
                        WriteJsonResponse(response, new { error = "Not Found", message = $"Key {keyPrefix} not found or inactive" });
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to set ERP credentials", message = ex.Message });
            }
        }

        private static void HandleArticlesRequest(HttpListenerRequest request, HttpListenerResponse response, string tenantId, ApiKeyValidationResult authResult)
        {
            try
            {
                var query = new QueryRequest
                {
                    Skip = ParseInt(request.QueryString["skip"]),
                    Take = ParseInt(request.QueryString["take"]) ?? 50
                };

                // Create ERP service with tenant's credentials
                var erpService = authResult.ErpCredentials != null
                    ? new EnventaErpService(authResult.ErpCredentials)
                    : _erpService; // Fallback to default (should not happen in production)

                var result = erpService.QueryArticlesAsync(tenantId, query).GetAwaiter().GetResult();
                WriteJsonResponse(response, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying articles: {ex.Message}");
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to query articles", message = ex.Message });
            }
        }

        private static void HandleCustomersRequest(HttpListenerRequest request, HttpListenerResponse response, string tenantId, ApiKeyValidationResult authResult)
        {
            try
            {
                var query = new QueryRequest
                {
                    Skip = ParseInt(request.QueryString["skip"]),
                    Take = ParseInt(request.QueryString["take"]) ?? 50
                };

                // Create ERP service with tenant's credentials
                var erpService = authResult.ErpCredentials != null
                    ? new EnventaErpService(authResult.ErpCredentials)
                    : _erpService; // Fallback to default (should not happen in production)

                var result = erpService.QueryCustomersAsync(tenantId, query).GetAwaiter().GetResult();
                WriteJsonResponse(response, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying customers: {ex.Message}");
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to query customers", message = ex.Message });
            }
        }

        private static void HandleOrdersRequest(HttpListenerRequest request, HttpListenerResponse response, string tenantId, ApiKeyValidationResult authResult)
        {
            try
            {
                var query = new QueryRequest
                {
                    Skip = ParseInt(request.QueryString["skip"]),
                    Take = ParseInt(request.QueryString["take"]) ?? 50
                };

                // Create ERP service with tenant's credentials
                var erpService = authResult.ErpCredentials != null
                    ? new EnventaErpService(authResult.ErpCredentials)
                    : _erpService; // Fallback to default (should not happen in production)

                var result = erpService.QueryOrdersAsync(tenantId, query).GetAwaiter().GetResult();
                WriteJsonResponse(response, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying orders: {ex.Message}");
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Failed to query orders", message = ex.Message });
            }
        }

        private static void WriteJsonResponse(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            var json = _serializer.Serialize(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private static int? ParseInt(string value)
        {
            if (int.TryParse(value, out var result))
                return result;
            return null;
        }

        private static HashSet<string> ParseAllowedBusinessUnits(string config)
        {
            if (string.IsNullOrWhiteSpace(config))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            return new HashSet<string>(
                config.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(bu => bu.Trim())
                    .Where(bu => !string.IsNullOrEmpty(bu)),
                StringComparer.OrdinalIgnoreCase
            );
        }

        private static bool IsBusinessUnitAllowed(string businessUnit)
        {
            // If no restrictions configured, all business units are allowed
            if (_allowedBusinessUnits.Count == 0)
                return true;

            return _allowedBusinessUnits.Contains(businessUnit);
        }

        private static async void HandleAiRequest(HttpListenerRequest request, HttpListenerResponse response, string tenantId, ApiKeyValidationResult authResult)
        {
            try
            {
                var path = request.Url.AbsolutePath.ToLowerInvariant();

                // Check if AI is enabled
                if (!_erpService.IsAiEnabled)
                {
                    response.StatusCode = 503;
                    WriteJsonResponse(response, new { error = "Service Unavailable", message = "AI processing is not enabled" });
                    return;
                }

                if (request.HttpMethod == "POST")
                {
                    // POST /api/ai/validate - Validate data with AI
                    if (path == "/api/ai/validate")
                    {
                        using (var reader = new System.IO.StreamReader(request.InputStream))
                        {
                            var body = await reader.ReadToEndAsync();
                            var data = _serializer.Deserialize<dynamic>(body);
                            var content = data?["data"]?.ToString() ?? "";

                            var aiResponse = await _erpService.ValidateArticleDataWithAiAsync(tenantId, content);
                            WriteJsonResponse(response, new
                            {
                                tenantId,
                                operation = "validate",
                                aiResponse.Content,
                                aiResponse.TokensUsed,
                                aiResponse.Model,
                                timestamp = DateTime.UtcNow.ToString("o")
                            });
                        }
                    }
                    // POST /api/ai/correct - Correct data with AI
                    else if (path == "/api/ai/correct")
                    {
                        using (var reader = new System.IO.StreamReader(request.InputStream))
                        {
                            var body = await reader.ReadToEndAsync();
                            var data = _serializer.Deserialize<dynamic>(body);
                            var content = data?["data"]?.ToString() ?? "";
                            var errors = data?["errors"]?.ToString() ?? "";

                            var aiResponse = await _erpService.CorrectArticleDataWithAiAsync(tenantId, content, errors);
                            WriteJsonResponse(response, new
                            {
                                tenantId,
                                operation = "correct",
                                aiResponse.Content,
                                aiResponse.TokensUsed,
                                aiResponse.Model,
                                timestamp = DateTime.UtcNow.ToString("o")
                            });
                        }
                    }
                    // POST /api/ai/process - General AI processing
                    else if (path == "/api/ai/process")
                    {
                        using (var reader = new System.IO.StreamReader(request.InputStream))
                        {
                            var body = await reader.ReadToEndAsync();
                            var data = _serializer.Deserialize<dynamic>(body);
                            var operation = data?["operation"]?.ToString() ?? "process";
                            var content = data?["data"]?.ToString() ?? "";

                            var aiResponse = await _erpService.ProcessTenantDataWithAiAsync(tenantId, operation, content);
                            WriteJsonResponse(response, new
                            {
                                tenantId,
                                operation,
                                aiResponse.Content,
                                aiResponse.TokensUsed,
                                aiResponse.Model,
                                timestamp = DateTime.UtcNow.ToString("o")
                            });
                        }
                    }
                    else
                    {
                        response.StatusCode = 404;
                        WriteJsonResponse(response, new { error = "Not Found", message = "AI endpoint not found" });
                    }
                }
                else
                {
                    response.StatusCode = 405;
                    WriteJsonResponse(response, new { error = "Method Not Allowed", message = "Only POST method is supported for AI endpoints" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI request error: {ex.Message}");
                response.StatusCode = 500;
                WriteJsonResponse(response, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        /// <summary>
        /// Run performance benchmarking mode
        /// </summary>
        private static int RunBenchmarkMode(string[] args)
        {
            Console.WriteLine("=== ERP Connector Performance Benchmark ===");

            try
            {
                _cts = new CancellationTokenSource();

                // Load configuration
                _baseAddress = ConfigurationManager.AppSettings["BaseAddress"] ?? "http://localhost:5080";
                _legacyApiKey = ConfigurationManager.AppSettings["ApiKey"] ?? Environment.GetEnvironmentVariable("ERP_CONNECTOR_API_KEY") ?? "";
                _useTenantKeys = bool.TryParse(ConfigurationManager.AppSettings["UseTenantKeys"], out var useTenant) && useTenant;

                // Initialize performance monitoring
                var performanceMonitor = new PerformanceMonitor(true, TimeSpan.FromSeconds(30));

                // Initialize AI service if enabled
                LocalAiService aiService = null;
                var aiProviderSelector = new ErpAiProviderSelector();
                if (aiProviderSelector.IsAiEnabled)
                {
                    try
                    {
                        aiService = aiProviderSelector.CreateAiServiceForTenant("benchmark-tenant", performanceMonitor);
                        Console.WriteLine($"AI processing enabled for benchmarking using {aiProviderSelector.PreferredProvider} provider");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: AI service initialization failed: {ex.Message}");
                        Console.WriteLine("Benchmark will continue without AI capabilities");
                    }
                }

                // Initialize ERP service with performance monitoring
                _erpService = new EnventaErpService(aiService, performanceMonitor);

                // Run benchmarks
                Console.WriteLine("Starting performance benchmarks...");
                RunBenchmarks(performanceMonitor).Wait();

                // Export final results
                Console.WriteLine("\n=== Final Benchmark Results ===");
                Console.WriteLine(performanceMonitor.ExportMetrics());

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Benchmark failed: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Run performance benchmark tests
        /// </summary>
        private static async Task RunBenchmarks(PerformanceMonitor performanceMonitor)
        {
            Console.WriteLine("Running ERP operation benchmarks...");

            // Benchmark GetArticle operations
            await BenchmarkGetArticle("benchmark-tenant", 10);

            // Benchmark QueryArticles operations
            await BenchmarkQueryArticles("benchmark-tenant", 5);

            // Benchmark AI operations if available
            if (_erpService != null)
            {
                await BenchmarkAiOperations("benchmark-tenant", 3);
            }

            // Wait for final metrics report
            await Task.Delay(1000);
        }

        /// <summary>
        /// Benchmark GetArticle operations
        /// </summary>
        private static async Task BenchmarkGetArticle(string tenantId, int iterations)
        {
            Console.WriteLine($"Benchmarking GetArticle operations ({iterations} iterations)...");

            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    // Use a test article ID - this may fail in real ERP but that's ok for benchmarking
                    await _erpService.GetArticleAsync(tenantId, $"TEST{i:D4}");
                }
                catch (Exception)
                {
                    // Expected for test data - we just want timing
                }
            }
        }

        /// <summary>
        /// Benchmark QueryArticles operations
        /// </summary>
        private static async Task BenchmarkQueryArticles(string tenantId, int iterations)
        {
            Console.WriteLine($"Benchmarking QueryArticles operations ({iterations} iterations)...");

            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    var query = new QueryRequest
                    {
                        Take = 10,
                        Skip = i * 10
                    };
                    await _erpService.QueryArticlesAsync(tenantId, query);
                }
                catch (Exception)
                {
                    // Expected for test data - we just want timing
                }
            }
        }

        /// <summary>
        /// Benchmark AI operations
        /// </summary>
        private static async Task BenchmarkAiOperations(string tenantId, int iterations)
        {
            Console.WriteLine($"Benchmarking AI operations ({iterations} iterations)...");

            // This would require the LocalAiService to be properly configured
            // For now, we'll skip if not available
            Console.WriteLine("AI benchmarking skipped - requires proper AI service configuration");
        }
    }
}
