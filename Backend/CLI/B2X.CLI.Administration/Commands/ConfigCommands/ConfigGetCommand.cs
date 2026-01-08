using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;

namespace B2X.CLI.Administration.Commands.ConfigCommands;

public static class ConfigGetCommand
{
    public static Command Create()
    {
        var command = new Command("get", "Get a configuration value");

        var keyArgument = new Argument<string>("key", "Configuration key to retrieve");
        var environmentOption = new Option<string>(
            ["-e", "--environment"],
            "Target environment (development, staging, production)");
        var showSecretsOption = new Option<bool>(
            ["-s", "--show-secrets"],
            "Show sensitive configuration values");
        var formatOption = new Option<string>(
            ["-f", "--format"],
            getDefaultValue: () => "text",
            description: "Output format (text, json)");

        command.AddArgument(keyArgument);
        command.AddOption(environmentOption);
        command.AddOption(showSecretsOption);
        command.AddOption(formatOption);

        command.SetHandler(ExecuteAsync, keyArgument, environmentOption, showSecretsOption, formatOption);

        return command;
    }

    private static async Task ExecuteAsync(string key, string environment, bool showSecrets, string format)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            string value = null;
            bool isSecret = false;

            // Try to get the value
            if (key.StartsWith("services."))
            {
                var serviceName = key.Substring("services.".Length);
                value = config.GetServiceUrl(serviceName, environment);
            }
            else if (key == "auth.token")
            {
                value = config.GetToken();
                isSecret = true;
            }
            else if (key == "environment.current")
            {
                value = environment ?? "default";
            }
            else
            {
                // Try to get from configuration sections
                value = config.GetValue(key, environment);
                isSecret = IsSensitiveKey(key);
            }

            if (value == null)
            {
                console.WriteError($"Configuration key '{key}' not found");
                Environment.ExitCode = 1;
                return;
            }

            // Handle sensitive values
            if (isSecret && !showSecrets)
            {
                value = "***masked***";
                console.WriteWarning("Sensitive value is masked. Use --show-secrets to reveal it.");
            }

            if (format == "json")
            {
                var result = new
                {
                    key,
                    value,
                    environment = environment ?? "default",
                    isSecret
                };
                console.WriteJson(result);
            }
            else
            {
                console.WriteInfo($"{key}: {value}");
            }

        }
        catch (Exception ex)
        {
            console.WriteError($"Failed to get configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static bool IsSensitiveKey(string key)
    {
        var sensitivePatterns = new[]
        {
            "password",
            "secret",
            "key",
            "token",
            "apikey",
            "connectionstring"
        };

        var lowerKey = key.ToLowerInvariant();
        return sensitivePatterns.Any(pattern => lowerKey.Contains(pattern));
    }
}