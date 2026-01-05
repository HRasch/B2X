using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using B2Connect.CLI.Commands.AuthCommands;
using B2Connect.CLI.Commands.MonitoringCommands;
using B2Connect.CLI.Commands.SystemCommands;
using B2Connect.CLI.Commands.TenantCommands;
using B2Connect.CLI.Commands;
using B2Connect.CLI.Shared.Configuration;
using Spectre.Console;

var rootCommand = new RootCommand("B2Connect CLI - Microservices Management Tool")
{
    TreatUnmatchedTokensAsErrors = true
};

// Auth Commands
var authCommand = new Command("auth", "User authentication and management");
authCommand.AddCommand(CreateUserCommand.Create());
authCommand.AddCommand(LoginCommand.Create());
rootCommand.AddCommand(authCommand);

// Tenant Commands
var tenantCommand = new Command("tenant", "Tenant management");
tenantCommand.AddCommand(CreateTenantCommand.Create());
rootCommand.AddCommand(tenantCommand);

// System Commands
var systemCommand = new Command("system", "System operations");
systemCommand.AddCommand(StatusCommand.Create());
rootCommand.AddCommand(systemCommand);

// Monitoring Commands
var monitoringCommand = new Command("monitoring", "Service monitoring and health checks");
monitoringCommand.AddCommand(RegisterServiceCommand.Create());
monitoringCommand.AddCommand(TestConnectivityCommand.Create());
monitoringCommand.AddCommand(ServiceStatusCommand.Create());
monitoringCommand.AddCommand(AlertsCommand.Create());
rootCommand.AddCommand(monitoringCommand);

// Info Command
var infoCommand = new Command("info", "Show configuration information");
infoCommand.SetHandler(async () => await ShowInfo());
rootCommand.AddCommand(infoCommand);

// Build and run
var parser = new CommandLineBuilder(rootCommand)
    .UseVersionOption()
    .UseHelp()
    .UseEnvironmentVariableDirective()
    .UseSuggestDirective()
    .RegisterWithDotnetSuggest()
    .UseParseErrorReporting()
    .Build();

try
{
    Environment.Exit(await parser.InvokeAsync(args));
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Fatal error: {ex.Message}[/]");
    Environment.Exit(1);
}

static async Task ShowInfo()
{
    var config = new ConfigurationService();

    AnsiConsole.Write(
        new Panel(
            "[bold]B2Connect CLI Configuration[/]\n" +
            $"[green]Status:[/] Ready\n" +
            $"[green]Services configured:[/] {config.GetAllServices().Count()}")
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1)
        });

    AnsiConsole.WriteLine();

    var table = new Table();
    table.AddColumn("Service");
    table.AddColumn("URL");
    table.AddColumn("Status");

    foreach ((string name, ServiceEndpoint endpoint) in config.GetAllServices())
    {
        table.AddRow(name, endpoint.Url, "✓");
    }

    AnsiConsole.Write(table);
}
