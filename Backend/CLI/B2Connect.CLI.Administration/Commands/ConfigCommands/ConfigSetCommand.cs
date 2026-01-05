using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;

namespace B2Connect.CLI.Administration.Commands.ConfigCommands;

public static class ConfigSetCommand
{
    public static Command Create()
    {
        var command = new Command("set", "Set a configuration value");

        var keyArgument = new Argument<string>("key", "Configuration key to set");
        var valueArgument = new Argument<string>("value", "Value to set");
        var environmentOption = new Option<string>(
            ["-e", "--environment"],
            "Target environment (development, staging, production)");
        var globalOption = new Option<bool>(
            ["-g", "--global"],
            "Set as global configuration (not environment-specific)");
        var forceOption = new Option<bool>(
            ["-f", "--force"],
            "Force setting sensitive values without confirmation");

        command.AddArgument(keyArgument);
        command.AddArgument(valueArgument);
        command.AddOption(environmentOption);
        command.AddOption(globalOption);
        command.AddOption(forceOption);

        command.SetHandler(ExecuteAsync, keyArgument, valueArgument, environmentOption, globalOption, forceOption);

        return command;
    }

    private static async Task ExecuteAsync(string key, string value, string environment, bool global, bool force)
    {
        var console = new ConsoleOutputService();
        var config = new ConfigurationService();

        try
        {
            // Validate key format
            if (!IsValidKey(key))
            {
                console.WriteError("Invalid configuration key format. Use format: category.key or category.subcategory.key");
                Environment.ExitCode = 1;
                return;
            }

            // Check if this is a sensitive key
            if (IsSensitiveKey(key) && !force)
            {
                console.WriteWarning($"Setting sensitive configuration key: {key}");
                console.WriteWarning("This may expose sensitive information. Use --force to proceed.");
                Environment.ExitCode = 1;
                return;
            }

            // Set the configuration
            if (global)
            {
                config.SetGlobalValue(key, value);
                console.WriteSuccess($"Set global configuration: {key} = {value}");
            }
            else
            {
                config.SetEnvironmentValue(key, value, environment);
                var env = environment ?? "default";
                console.WriteSuccess($"Set configuration for environment '{env}': {key} = {value}");
            }

            // Save configuration
            await config.SaveAsync();
            console.WriteInfo("Configuration saved successfully");

        }
        catch (Exception ex)
        {
            console.WriteError($"Failed to set configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static bool IsValidKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        // Basic validation: should contain at least one dot and be alphanumeric with dots/underscores
        if (!key.Contains('.'))
            return false;

        foreach (var c in key)
        {
            if (!char.IsLetterOrDigit(c) && c != '.' && c != '_' && c != '-')
                return false;
        }

        return true;
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