using B2Connect.ThemeService.Models;
using B2Connect.ThemeService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Theming Services
builder.Services.AddSingleton<IThemeRepository, InMemoryThemeRepository>();
builder.Services.AddScoped<IThemeService, ThemeService>();

// Add Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
