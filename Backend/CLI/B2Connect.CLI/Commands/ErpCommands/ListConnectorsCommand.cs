using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using Spectre.Console;
using System.Net.Http.Json;

namespace B2Connect.CLI.Commands.ErpCommands;

public static class ListConnectorsCommand
{
    public static Command Create()
    {
        var command = new Command("list-connectors", "List available ERP connectors");

        var erpTypeOption = new Option<string>(
            ["-t", "--erp-type"], "Filter by ERP type (e.g., enventa, sap, oracle)");
        var registryUrlOption = new Option<string>(
            ["-r", "--registry-url"], "Connector registry URL (uses default if not provided)");
        var showDetailsOption = new Option<bool>(
            ["-d", "--details"], "Show detailed information for each connector");

        command.AddOption(erpTypeOption);
        command.AddOption(registryUrlOption);
        command.AddOption(showDetailsOption);

        command.SetHandler(ExecuteAsync, erpTypeOption, registryUrlOption, showDetailsOption);

        return command;
    }

    private static async Task ExecuteAsync(string? erpType, string? registryUrl, bool showDetails)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            // Get registry URL
            var registryBaseUrl = registryUrl ?? config.GetConnectorRegistryUrl() ?? "https://registry.b2connect.com/api/connectors";

            console.Info($"Fetching connector list from {registryBaseUrl}");

            // Fetch connector list
            var connectors = await FetchConnectorListAsync(registryBaseUrl, erpType);

            if (connectors == null || !connectors.Any())
            {
                console.Warning("No connectors found");
                return;
            }

            if (showDetails)
            {
                await DisplayDetailedConnectorList(connectors);
            }
            else
            {
                await DisplaySummaryConnectorList(connectors);
            }

            console.Success($"Found {connectors.Count} connector(s)");
        }
        catch (Exception ex)
        {
            console.Error($"Failed to list connectors: {ex.Message}");
            if (ex.InnerException != null)
            {
                console.Error($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }

    private static async Task<List<ConnectorInfo>?> FetchConnectorListAsync(string registryUrl, string? erpType)
    {
        using var client = new HttpClient();
        var url = $"{registryUrl}/list";
        if (!string.IsNullOrEmpty(erpType))
        {
            url += $"?erpType={erpType}";
        }

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ConnectorInfo>>();
        }

        throw new Exception($"Failed to fetch connector list: {response.StatusCode}");
    }

    private static async Task DisplaySummaryConnectorList(List<ConnectorInfo> connectors)
    {
        var table = new Table();
        table.Title = new TableTitle("[green]Available ERP Connectors[/]");
        table.AddColumn("ERP Type");
        table.AddColumn("Name");
        table.AddColumn("Version");
        table.AddColumn("Supported Versions");
        table.AddColumn("Status");

        foreach (var connector in connectors.OrderBy(c => c.ErpType).ThenBy(c => c.Name))
        {
            var supportedVersions = connector.SupportedVersions != null && connector.SupportedVersions.Any()
                ? string.Join(", ", connector.SupportedVersions.Take(3)) + (connector.SupportedVersions.Count > 3 ? "..." : "")
                : "N/A";

            table.AddRow(
                connector.ErpType,
                connector.Name,
                connector.Version,
                supportedVersions,
                connector.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    private static async Task DisplayDetailedConnectorList(List<ConnectorInfo> connectors)
    {
        foreach (var connector in connectors.OrderBy(c => c.ErpType).ThenBy(c => c.Name))
        {
            var content = new Markup($"[bold]{connector.Name} v{connector.Version}[/]\n\n" +
                                   $"[bold]Description:[/] {connector.Description}\n" +
                                   $"[bold]Supported Versions:[/] {string.Join(", ", connector.SupportedVersions ?? new List<string>())}\n" +
                                   $"[bold]Created:[/] {connector.CreatedAt:yyyy-MM-dd}\n" +
                                   $"[bold]Status:[/] {(connector.IsActive ? "[green]Active[/]" : "[red]Inactive[/]")}\n" +
                                   $"[bold]Download Size:[/] {connector.DownloadSizeBytes / 1024} KB\n" +
                                   $"[bold]Requirements:[/] {connector.Requirements ?? "None"}");

            var panel = new Panel(content)
            {
                Header = new PanelHeader($"[blue]{connector.ErpType.ToUpper()}[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }

    private class ConnectorInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string ErpType { get; set; } = string.Empty;
        public List<string>? SupportedVersions { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public long DownloadSizeBytes { get; set; }
        public string? Requirements { get; set; }
    }
}