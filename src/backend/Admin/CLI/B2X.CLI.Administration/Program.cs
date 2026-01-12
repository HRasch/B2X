using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using B2X.CLI.Administration.Commands.AuthCommands;
using B2X.CLI.Administration.Commands.CatalogCommands;
using B2X.CLI.Administration.Commands.ConfigCommands;
using B2X.CLI.Administration.Commands.DiscoveryCommands;
using B2X.CLI.Administration.Commands.HealthCommands;
using B2X.CLI.Administration.Commands.MetricsCommands;
using B2X.CLI.Administration.Commands.TenantCommands;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

var host = CreateHostBuilder(args).Build();
var rootCommand = new RootCommand("B2X Administration CLI - Tenant Management Tool")
{
    TreatUnmatchedTokensAsErrors = true
};

// Auth Commands
var authCommand = new Command("auth", "User authentication and management");
authCommand.AddCommand(CreateUserCommand.Create());
authCommand.AddCommand(LoginCommand.Create());
rootCommand.AddCommand(authCommand);

// Tenant Commands
var tenantCommand = new Command("tenant", "Tenant management operations");
tenantCommand.AddCommand(CreateTenantCommand.Create());
rootCommand.AddCommand(tenantCommand);

// Catalog Commands
var catalogCommand = new Command("catalog", "Catalog management operations");
catalogCommand.AddCommand(ImportCatalogCommand.Create());
catalogCommand.AddCommand(ExportCatalogCommand.Create());
rootCommand.AddCommand(catalogCommand);

// Health Commands
var healthCommand = new Command("health", "System health monitoring and checks");
healthCommand.AddCommand(HealthCheckCommand.Create());
rootCommand.AddCommand(healthCommand);

// Discovery Commands
var discoveryCommand = new Command("discover", "AI service discovery and exploration");
discoveryCommand.AddCommand(DiscoverServicesCommand.Create());
rootCommand.AddCommand(discoveryCommand);

// Metrics Commands
var metricsCommand = new Command("metrics", "Metrics, monitoring, and benchmarking");
metricsCommand.AddCommand(BenchmarkCommand.Create());
metricsCommand.AddCommand(MetricsConfigCommand.Create());
metricsCommand.AddCommand(MetricsViewCommand.Create());
metricsCommand.AddCommand(MetricsAlertsCommand.Create());
rootCommand.AddCommand(metricsCommand);

// Configuration Commands
var configCommand = new Command("config", "Configuration management");
configCommand.AddCommand(ConfigListCommand.Create());
configCommand.AddCommand(ConfigGetCommand.Create());
configCommand.AddCommand(ConfigSetCommand.Create());
rootCommand.AddCommand(configCommand);

// Info Command
var infoCommand = new Command("info", "Show configuration information");
infoCommand.SetHandler(async () => await ShowInfo());
rootCommand.AddCommand(infoCommand);

// Build and run
var parser = new CommandLineBuilder(rootCommand)
    .UseVersionOption()
    .UseHelp()
    .UseEnvironmentVariableDirective()
    .UseParseDirective()
    .UseSuggestDirective()
    .RegisterWithDotnetSuggest()
    .UseTypoCorrections()
    .UseParseErrorReporting()
    .CancelOnProcessTermination()
    .Build();

return await parser.InvokeAsync(args);

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            // Register logging
            services.AddLogging();

            // Register shared services
            services.AddSingleton<ConfigurationService>();
            services.AddSingleton<ConsoleOutputService>();
        });

static async Task ShowInfo()
{
    var console = new ConsoleOutputService();
    var config = new ConfigurationService();

    console.Header("B2X Administration CLI Configuration");

    try
    {
        var services = config.GetAllServices();
        console.Info($"Configured services: {services.Count()}");

        foreach (var (name, endpoint) in services)
        {
            console.Info($"  - {name}: {endpoint.Url}");
        }

        console.Info("\nAuthentication:");
        console.Info("  Use B2X_TENANT_TOKEN environment variable");
        console.Info("  Token should be tenant-scoped API key");
        console.Info("\nExample:");
        console.Info("  export B2X_TENANT_TOKEN=\"your-tenant-api-key\"");
        console.Info("  B2X-admin tenant create --name \"My Company\"");
    }
    catch (Exception ex)
    {
        console.Error($"Failed to load configuration: {ex.Message}");
    }
}
