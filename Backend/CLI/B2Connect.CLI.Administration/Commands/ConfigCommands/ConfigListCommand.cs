using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using Spectre.Console;

namespace B2Connect.CLI.Administration.Commands.ConfigCommands;

public static class ConfigListCommand
{
    public static Command Create()
    {
        var command = new Command("list", "List current configuration settings");

        var environmentOption = new Option<string>(
            ["-e", "--environment"],
            "Target environment (development, staging, production)");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            getDefaultValue: () => "table",
            description: "Output format (table, json, yaml)");
        var showSecretsOption = new Option<bool>(
            ["-s", "--show-secrets"],
            "Show sensitive configuration values (use with caution)");
        var filterOption = new Option<string>(
            ["--filter"],
            "Filter configuration keys by pattern");

        command.AddOption(environmentOption);
        command.AddOption(formatOption);
        command.AddOption(showSecretsOption);
        command.AddOption(filterOption);

        command.SetHandler(ExecuteAsync, environmentOption, formatOption, showSecretsOption, filterOption);

        return command;
    }

    private static async Task ExecuteAsync(string environment, string format, bool showSecrets, string filter)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            console.WriteInfo("Loading configuration...");

            var configData = await LoadConfigurationAsync(config, environment, showSecrets, filter);

            if (format == "json")
            {
                console.WriteJson(configData);
            }
            else if (format == "yaml")
            {
                console.WriteYaml(configData);
            }
            else
            {
                DisplayConfigTable(configData);
            }
        }
        catch (Exception ex)
        {
            console.WriteError($"Failed to load configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task<List<ConfigItem>> LoadConfigurationAsync(
        ConfigurationService config,
        string environment,
        bool showSecrets,
        string filter)
    {
        var configItems = new List<ConfigItem>();

        // Load service URLs
        var serviceUrls = config.GetAllServiceUrls(environment);
        foreach (var kvp in serviceUrls)
        {
            if (MatchesFilter(kvp.Key, filter))
            {
                configItems.Add(new ConfigItem
                {
                    Key = $"services.{kvp.Key}",
                    Value = kvp.Value,
                    Category = "Services",
                    IsSecret = false
                });
            }
        }

        // Load authentication settings
        var token = config.GetToken();
        if (!string.IsNullOrEmpty(token) && MatchesFilter("auth.token", filter))
        {
            configItems.Add(new ConfigItem
            {
                Key = "auth.token",
                Value = showSecrets ? token : "***masked***",
                Category = "Authentication",
                IsSecret = true
            });
        }

        // Load environment settings
        if (MatchesFilter("environment.current", filter))
        {
            configItems.Add(new ConfigItem
            {
                Key = "environment.current",
                Value = environment ?? "default",
                Category = "Environment",
                IsSecret = false
            });
        }

        // Load AI settings
        var aiSettings = config.GetSection("AI");
        if (aiSettings != null)
        {
            foreach (var kvp in aiSettings)
            {
                var key = $"ai.{kvp.Key}";
                if (MatchesFilter(key, filter))
                {
                    configItems.Add(new ConfigItem
                    {
                        Key = key,
                        Value = kvp.Value,
                        Category = "AI",
                        IsSecret = key.Contains("key") || key.Contains("secret")
                    });
                }
            }
        }

        return configItems;
    }

    private static bool MatchesFilter(string key, string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return true;

        return key.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private static void DisplayConfigTable(List<ConfigItem> configItems)
    {
        var table = new Table();
        table.AddColumn("Category");
        table.AddColumn("Key");
        table.AddColumn("Value");
        table.AddColumn("Type");

        foreach (var item in configItems)
        {
            var type = item.IsSecret ? "[red]Secret[/]" : "[green]Public[/]";

            table.AddRow(
                item.Category,
                item.Key,
                item.Value,
                type
            );
        }

        AnsiConsole.Write(table);

        var secretCount = configItems.Count(x => x.IsSecret);
        if (secretCount > 0)
        {
            AnsiConsole.MarkupLine($"[yellow]Note: {secretCount} sensitive values are masked. Use --show-secrets to reveal them.[/]");
        }
    }

    private class ConfigItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
        public bool IsSecret { get; set; }
    }
}