using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using B2X.CLI.Operations.Commands.HealthCommands;
using B2X.CLI.Operations.Commands.MonitoringCommands;
using B2X.CLI.Operations.Commands.ServiceCommands;
using B2X.CLI.Operations.Commands.DeploymentCommands;
using B2X.CLI.Operations.Commands.ValidationCommands;
using B2X.CLI.Shared.Configuration;
using Spectre.Console;

var rootCommand = new RootCommand("B2X Operations CLI - Platform Management Tool")
{
    TreatUnmatchedTokensAsErrors = true
};

// Health Commands
var healthCommand = new Command("health", "System health checks and diagnostics");
healthCommand.AddCommand(CheckHealthCommand.Create());
healthCommand.AddCommand(SystemStatusCommand.Create());
rootCommand.AddCommand(healthCommand);

// Monitoring Commands
var monitoringCommand = new Command("monitoring", "Service monitoring and health checks");
monitoringCommand.AddCommand(DashboardCommand.Create());
monitoringCommand.AddCommand(MetricsCommand.Create());
rootCommand.AddCommand(monitoringCommand);

// Service Commands
var serviceCommand = new Command("service", "Service management operations");
serviceCommand.AddCommand(RestartCommand.Create());
serviceCommand.AddCommand(ScaleCommand.Create());
rootCommand.AddCommand(serviceCommand);

// Deployment Commands
var deploymentCommand = new Command("deployment", "Deployment and migration operations");
deploymentCommand.AddCommand(MigrateCommand.Create());
deploymentCommand.AddCommand(RollbackCommand.Create());
rootCommand.AddCommand(deploymentCommand);

// Validation Commands
var validationCommand = new Command("validation", "Data validation operations");
validationCommand.AddCommand(ValidateErpCommand.Create());
rootCommand.AddCommand(validationCommand);

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
            "[bold]B2X Operations CLI Configuration[/]\n" +
            $"[green]Status:[/] Ready\n" +
            $"[green]Services configured:[/] {config.GetAllServices().Count()}\n" +
            $"[green]Authentication:[/] Infrastructure credentials required")
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