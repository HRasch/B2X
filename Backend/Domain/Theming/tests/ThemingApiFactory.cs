using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using B2Connect.ThemeService.Models;
using B2Connect.ThemeService.Services;

namespace B2Connect.Theming.Tests.Integration;

public class ThemingApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace the repository with a test-specific in-memory implementation
            services.AddSingleton<IThemeRepository, InMemoryThemeRepository>();
            services.AddSingleton<IThemeService, B2Connect.ThemeService.Services.ThemeService>();
        });
    }
}