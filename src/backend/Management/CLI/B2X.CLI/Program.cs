using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using B2X.CLI.Commands.AuthCommands;
using B2X.CLI.Commands.ErpCommands;
using B2X.CLI.Commands.MonitoringCommands;
using B2X.CLI.Commands.SystemCommands;
using B2X.CLI.Commands.TenantCommands;
using B2X.CLI.Commands;
using B2X.CLI.Shared.Configuration;
using Spectre.Console;

// Show deprecation warning on startup
ShowDeprecationWarning();

var rootCommand = new RootCommand("B2X CLI - Microservices Management Tool [DEPRECATED]")
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

// ERP Commands
var erpCommand = new Command("erp", "ERP connector management");
erpCommand.AddCommand(DownloadConnectorCommand.Create());
erpCommand.AddCommand(DetectVersionCommand.Create());
erpCommand.AddCommand(ListConnectorsCommand.Create());
rootCommand.AddCommand(erpCommand);

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
            "[bold]B2X CLI Configuration[/]\n" +
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

static void ShowDeprecationWarning()
{
    var panel = new Panel(
        "[yellow]⚠️  DEPRECATION WARNING[/]\n\n" +
        "[white]This CLI tool (B2X.CLI) is deprecated and will be removed in v3.0.0.[/]\n\n" +
        "[bold]Please migrate to the new CLI tools:[/]\n" +
        "  • [green]B2X.CLI.Operations[/] - For platform operations (internal)\n" +
        "  • [green]B2X.CLI.Administration[/] - For tenant management (customer-facing)\n\n" +
        "[bold]Migration:[/]\n" +
        "  dotnet tool install -g B2X.CLI.Operations\n" +
        "  dotnet tool install -g B2X.CLI.Administration\n\n" +
        "[dim]Documentation: https://docs.B2X.com/cli/migration[/]")
    {
        Header = new PanelHeader("[yellow]⚠️  DEPRECATED[/]"),
        Border = BoxBorder.Double,
        BorderStyle = new Style(Color.Yellow),
        Padding = new Padding(2, 1)
    };

    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[dim]Proceeding with legacy CLI execution...[/]");
    AnsiConsole.WriteLine();
}
