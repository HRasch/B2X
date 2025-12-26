using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace B2Connect.ApiGateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthGatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthGatewayController> _logger;

    public AuthGatewayController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AuthGatewayController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var authServiceUrl = _configuration["Services:AuthService"] ?? "http://localhost:5001";
            _logger.LogInformation("Forwarding login request to {AuthServiceUrl}", authServiceUrl);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{authServiceUrl}/api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<object>(content);
                return Ok(data);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorData = JsonSerializer.Deserialize<object>(errorContent);
            return StatusCode((int)response.StatusCode, errorData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding login request");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var authServiceUrl = _configuration["Services:AuthService"] ?? "http://localhost:5001";
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{authServiceUrl}/api/auth/refresh", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<object>(content);
                return Ok(data);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorData = JsonSerializer.Deserialize<object>(errorContent);
            return StatusCode((int)response.StatusCode, errorData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding refresh request");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var authServiceUrl = _configuration["Services:AuthService"] ?? "http://localhost:5001";
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync($"{authServiceUrl}/api/auth/logout", null);

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding logout request");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var authServiceUrl = _configuration["Services:AuthService"] ?? "http://localhost:5001";
            var client = _httpClientFactory.CreateClient();

            // Get the authorization header from the request
            var authHeader = Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                client.DefaultRequestHeaders.Add("Authorization", authHeader);
            }

            var response = await client.GetAsync($"{authServiceUrl}/api/auth/me");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<object>(content);
                return Ok(data);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorData = JsonSerializer.Deserialize<object>(errorContent);
            return StatusCode((int)response.StatusCode, errorData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding get current user request");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
