using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;
using B2Connect.CLI.Shared.HttpClients;
using Spectre.Console;

namespace B2Connect.CLI.Administration.Commands.DiscoveryCommands;

public static class DiscoverServicesCommand
{
    public static Command Create()
    {
        var command = new Command("ai", "Discover available AI services and providers");

        var aiServerOption = new Option<string>(
            ["-s", "--ai-server"],
            "IP address or hostname of the AI server (prompts if not provided)");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            getDefaultValue: () => "table",
            description: "Output format (table, json, yaml)");
        var verboseOption = new Option<bool>(
            ["-v", "--verbose"],
            "Show detailed AI service information");

        command.AddOption(aiServerOption);
        command.AddOption(formatOption);
        command.AddOption(verboseOption);

        command.SetHandler(ExecuteAsync, aiServerOption, formatOption, verboseOption);

        return command;
    }

    private static async Task ExecuteAsync(string aiServer, string format, bool verbose)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            // Prompt for AI server if not provided
            if (string.IsNullOrEmpty(aiServer))
            {
                // Check if we're in an interactive terminal
                if (Console.IsInputRedirected || Console.IsOutputRedirected || !Environment.UserInteractive)
                {
                    // Non-interactive mode - use default
                    aiServer = "localhost";
                    console.WriteInfo("Using default AI server: localhost (non-interactive mode)");
                }
                else
                {
                    // Interactive mode - prompt user
                    aiServer = AnsiConsole.Prompt(
                        new TextPrompt<string>("Enter AI server IP or hostname:")
                            .DefaultValue("localhost")
                            .Validate(host =>
                            {
                                if (string.IsNullOrWhiteSpace(host))
                                    return ValidationResult.Error("AI server cannot be empty");
                                return ValidationResult.Success();
                            }));
                }
            }

            console.WriteInfo($"Discovering available AI services on {aiServer}...");

            var aiServices = await DiscoverAIServicesAsync(config, aiServer, console);

            if (format == "json")
            {
                console.WriteJson(aiServices);
            }
            else if (format == "yaml")

                if (format == "json")
                {
                    console.WriteJson(aiServices);
                }
                else if (format == "yaml")
                {
                    console.WriteYaml(aiServices);
                }
                else
                {
                    DisplayAIServicesTable(aiServices, verbose);
                }
        }
        catch (Exception ex)
        {
            console.WriteError($"AI discovery failed: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task<List<AIServiceInfo>> DiscoverAIServicesAsync(ConfigurationService config, string aiServer, ConsoleOutputService console)
    {
        var aiServices = new List<AIServiceInfo>();

        // Common AI services to check
        var aiServiceDefinitions = new[]
        {
            new { Name = "Ollama", BaseUrl = $"http://{aiServer}:11434", Type = "Local LLM Server", Endpoint = "/api/tags" },
            new { Name = "LM Studio", BaseUrl = $"http://{aiServer}:1234", Type = "Local AI Interface", Endpoint = "/v1/models" },
            new { Name = "OpenAI", BaseUrl = "https://api.openai.com", Type = "Cloud AI Provider", Endpoint = "/v1/models" },
            new { Name = "Anthropic Claude", BaseUrl = "https://api.anthropic.com", Type = "Cloud AI Provider", Endpoint = "/v1/messages" },
            new { Name = "Google Gemini", BaseUrl = "https://generativelanguage.googleapis.com", Type = "Cloud AI Provider", Endpoint = "/v1beta/models" },
            new { Name = "MeinGPT", BaseUrl = "https://api.meingpt.de", Type = "German AI Provider", Endpoint = "/v1/models" }
        };

        foreach (var serviceDef in aiServiceDefinitions)
        {
            try
            {
                var isAvailable = await CheckAIServiceAvailabilityAsync(serviceDef.BaseUrl, serviceDef.Endpoint);

                aiServices.Add(new AIServiceInfo
                {
                    Name = serviceDef.Name,
                    BaseUrl = serviceDef.BaseUrl,
                    Type = serviceDef.Type,
                    Endpoint = serviceDef.Endpoint,
                    IsAvailable = isAvailable,
                    Status = isAvailable ? "Available" : "Unavailable"
                });

                var status = isAvailable ? "[green]Available[/]" : "[yellow]Unavailable[/]";
                console.WriteInfo($"✓ Checked {serviceDef.Name}: {serviceDef.BaseUrl} ({status})");
            }
            catch (Exception ex)
            {
                console.WriteWarning($"Could not check {serviceDef.Name}: {ex.Message}");

                aiServices.Add(new AIServiceInfo
                {
                    Name = serviceDef.Name,
                    BaseUrl = serviceDef.BaseUrl,
                    Type = serviceDef.Type,
                    Endpoint = serviceDef.Endpoint,
                    IsAvailable = false,
                    Status = "Error"
                });
            }
        }

        // Check for configured AI services from config
        try
        {
            var aiSection = config.GetSection("ai");
            if (aiSection != null)
            {
                foreach (var configItem in aiSection)
                {
                    if (configItem.Key.Contains("Endpoint") && !string.IsNullOrEmpty(configItem.Value))
                    {
                        var serviceName = configItem.Key.Replace("Endpoint", "").Replace(":", "");
                        var isAvailable = await CheckAIServiceAvailabilityAsync(configItem.Value, "/v1/models");

                        aiServices.Add(new AIServiceInfo
                        {
                            Name = $"{serviceName} (Configured)",
                            BaseUrl = configItem.Value,
                            Type = "Configured AI Service",
                            Endpoint = "/v1/models",
                            IsAvailable = isAvailable,
                            Status = isAvailable ? "Available" : "Unavailable"
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            console.WriteWarning($"Could not check configured AI services: {ex.Message}");
        }

        return aiServices;
    }

    private static async Task<bool> CheckAIServiceAvailabilityAsync(string baseUrl, string endpoint)
    {
        try
        {
            using var client = new CliHttpClient(baseUrl);
            var response = await client.GetAsync<string>(endpoint);

            return response.Success;
        }
        catch
        {
            return false;
        }
    }

    private static void DisplayAIServicesTable(List<AIServiceInfo> aiServices, bool verbose)
    {
        var table = new Table();
        table.AddColumn("AI Service");
        table.AddColumn("Type");
        table.AddColumn("Status");

        if (verbose)
        {
            table.AddColumn("Base URL");
            table.AddColumn("Endpoint");
        }

        foreach (var service in aiServices)
        {
            var statusColor = service.Status switch
            {
                "Available" => "[green]",
                "Unavailable" => "[yellow]",
                "Error" => "[red]",
                _ => "[gray]"
            };
            var status = $"{statusColor}{service.Status}[/]";

            if (verbose)
            {
                table.AddRow(
                    service.Name,
                    service.Type,
                    status,
                    service.BaseUrl,
                    service.Endpoint
                );
            }
            else
            {
                table.AddRow(service.Name, service.Type, status);
            }
        }

        AnsiConsole.Write(table);
    }

    private class AIServiceInfo
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Type { get; set; }
        public string Endpoint { get; set; }
        public bool IsAvailable { get; set; }
        public string Status { get; set; }
    }
}