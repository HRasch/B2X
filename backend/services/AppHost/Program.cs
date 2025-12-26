using Serilog;
using Aspire.Hosting;

// Configure Serilog
var serilogConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "AppHost")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

Log.Logger = serilogConfig.CreateLogger();

try
{
    var builder = DistributedApplication.CreateBuilder(args);

    Log.Information("🚀 B2Connect Aspire Application Host - Starting");

    // Register services from referenced projects (AddProject adds HTTP endpoints automatically)
    var authService = builder.AddProject("auth-service", "../auth-service/B2Connect.AuthService.csproj");

    var tenantService = builder.AddProject("tenant-service", "../tenant-service/B2Connect.TenantService.csproj");

    var localizationService = builder.AddProject("localization-service", "../LocalizationService/B2Connect.LocalizationService.csproj");

    // TODO: CatalogService - Has CQRS type mismatch issues in handlers (ICommandHandler signature)
    // var catalogService = builder.AddProject("catalog-service", "../CatalogService/B2Connect.CatalogService.csproj");

    var app = builder.Build();

    Log.Information("✅ Aspire Application Host initialized");
    Log.Information("");
    Log.Information("📊 Services:");
    Log.Information("  - Auth Service: http://localhost:9002");
    Log.Information("  - Tenant Service: http://localhost:9003");
    Log.Information("  - Localization Service: http://localhost:9004");
    Log.Information("");
    Log.Information("Frontend services (Port 5173, 5174) run via VS Code Tasks!");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}

