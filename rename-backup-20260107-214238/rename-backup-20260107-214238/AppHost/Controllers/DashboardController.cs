using Microsoft.AspNetCore.Mvc;

namespace B2X.AppHost.Controllers;

public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        using var client = _httpClientFactory.CreateClient();
        var services = new List<ServiceStatus>();

        var serviceUrls = new[]
        {
            ("Auth Service", "http://localhost:5001", 5001),
            ("Tenant Service", "http://localhost:5002", 5002),
            ("API Gateway", "http://localhost:5000", 5000)
        };

        foreach (var (name, baseUrl, port) in serviceUrls)
        {
            try
            {
                using var response = await client.GetAsync($"{baseUrl}/health");
                services.Add(new ServiceStatus
                {
                    Name = name,
                    Url = baseUrl,
                    Port = port,
                    IsHealthy = response.IsSuccessStatusCode,
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    StatusBadge = response.IsSuccessStatusCode ? "success" : "danger"
                });
            }
            catch
            {
                services.Add(new ServiceStatus
                {
                    Name = name,
                    Url = baseUrl,
                    Port = port,
                    IsHealthy = false,
                    Status = "Unavailable",
                    StatusBadge = "secondary"
                });
            }
        }

        return View(services);
    }
}

public class ServiceStatus
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool IsHealthy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBadge { get; set; } = string.Empty;
}
