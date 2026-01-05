using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using B2Connect.CLI.Administration.Commands.AuthCommands;
using B2Connect.CLI.Administration.Commands.TenantCommands;
using B2Connect.CLI.Administration.Commands.CatalogCommands;
using B2Connect.CLI.Administration.Commands.HealthCommands;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using Spectre.Console;

var rootCommand = new RootCommand("B2Connect Administration CLI - Tenant Management Tool")
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

static async Task ShowInfo()
{
    var console = new ConsoleOutputService();
    var config = new ConfigurationService();

    console.Header("B2Connect Administration CLI Configuration");

    try
    {
        var services = config.GetAllServices();
        console.Info($"Configured services: {services.Count()}");

        foreach (var (name, endpoint) in services)
        {
            console.Info($"  - {name}: {endpoint.Url}");
        }

        console.Info("\nAuthentication:");
        console.Info("  Use B2CONNECT_TENANT_TOKEN environment variable");
        console.Info("  Token should be tenant-scoped API key");
        console.Info("\nExample:");
        console.Info("  export B2CONNECT_TENANT_TOKEN=\"your-tenant-api-key\"");
        console.Info("  b2connect-admin tenant create --name \"My Company\"");
    }
    catch (Exception ex)
    {
        console.Error($"Failed to load configuration: {ex.Message}");
    }
}