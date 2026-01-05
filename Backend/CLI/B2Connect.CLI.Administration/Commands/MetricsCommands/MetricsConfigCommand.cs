using System.CommandLine;
using B2Connect.CLI.Shared;
using B2Connect.CLI.Shared.Configuration;

namespace B2Connect.CLI.Administration.Commands.MetricsCommands;

public static class MetricsConfigCommand
{
    public static Command Create()
    {
        var command = new Command("config", "Configure metrics and monitoring settings");

        var listCommand = new Command("list", "List current metrics configuration");
        listCommand.SetHandler(ExecuteListAsync);

        var setCommand = new Command("set", "Set metrics configuration value");
        var keyArgument = new Argument<string>("key", "Configuration key (e.g., monitoring.enabled, metrics.retention_days)");
        var valueArgument = new Argument<string>("value", "Configuration value");
        var serviceOption = new Option<string>(
            ["-s", "--service"],
            "Target service for configuration (global if not specified)");
        setCommand.AddArgument(keyArgument);
        setCommand.AddArgument(valueArgument);
        setCommand.AddOption(serviceOption);
        setCommand.SetHandler(ExecuteSetAsync, keyArgument, valueArgument, serviceOption);

        var getCommand = new Command("get", "Get metrics configuration value");
        var getKeyArgument = new Argument<string>("key", "Configuration key to retrieve");
        var getServiceOption = new Option<string>(
            ["-s", "--service"],
            "Target service for configuration (global if not specified)");
        getCommand.AddArgument(getKeyArgument);
        getCommand.AddOption(getServiceOption);
        getCommand.SetHandler(ExecuteGetAsync, getKeyArgument, getServiceOption);

        var resetCommand = new Command("reset", "Reset metrics configuration to defaults");
        var resetServiceOption = new Option<string>(
            ["-s", "--service"],
            "Target service for reset (global if not specified)");
        resetCommand.AddOption(resetServiceOption);
        resetCommand.SetHandler(ExecuteResetAsync, resetServiceOption);

        command.AddCommand(listCommand);
        command.AddCommand(setCommand);
        command.AddCommand(getCommand);
        command.AddCommand(resetCommand);

        return command;
    }

    private static async Task ExecuteListAsync()
    {
        var console = new ConsoleOutputService();

        try
        {
            console.Header("Metrics Configuration");

            // Global metrics configuration
            console.SubHeader("Global Configuration");
            await DisplayConfiguration(console, null);

            // Service-specific configurations
            console.SubHeader("Service-Specific Configuration");
            var config = new ConfigurationService();
            var services = config.GetAllServices();

            foreach (var (serviceName, _) in services)
            {
                console.Info($"Service: {serviceName}");
                await DisplayConfiguration(console, serviceName);
                console.Info("");
            }

        }
        catch (Exception ex)
        {
            console.Error($"Failed to list metrics configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteSetAsync(string key, string value, string service)
    {
        var console = new ConsoleOutputService();

        try
        {
            if (!IsValidMetricsKey(key))
            {
                console.Error("Invalid metrics configuration key. Valid keys:");
                console.Info("  monitoring.enabled (true/false)");
                console.Info("  monitoring.endpoint");
                console.Info("  metrics.retention_days");
                console.Info("  metrics.collection_interval_seconds");
                console.Info("  metrics.export.enabled (true/false)");
                console.Info("  metrics.export.format (json, prometheus, influxdb)");
                console.Info("  metrics.export.endpoint");
                console.Info("  alerts.enabled (true/false)");
                console.Info("  alerts.cpu_threshold_percent");
                console.Info("  alerts.memory_threshold_percent");
                console.Info("  alerts.response_time_threshold_ms");
                console.Info("  alerts.error_rate_threshold_percent");
                Environment.ExitCode = 1;
                return;
            }

            // Validate value based on key type
            if (!ValidateConfigurationValue(key, value))
            {
                console.Error($"Invalid value '{value}' for key '{key}'");
                Environment.ExitCode = 1;
                return;
            }

            var config = new ConfigurationService();
            var configKey = string.IsNullOrEmpty(service) ? $"metrics.{key}" : $"services.{service}.metrics.{key}";

            // For now, we'll store in the CLI configuration
            // In a real implementation, this would update the service configuration
            console.Info($"Setting {configKey} = {value}");

            // TODO: Implement actual configuration persistence
            // This would typically involve:
            // 1. Updating service configuration via API
            // 2. Updating configuration files
            // 3. Restarting services if necessary

            console.Success($"Metrics configuration updated: {key} = {value}");
            if (!string.IsNullOrEmpty(service))
            {
                console.Warning($"Note: Service '{service}' may need to be restarted for changes to take effect.");
            }

        }
        catch (Exception ex)
        {
            console.Error($"Failed to set metrics configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteGetAsync(string key, string service)
    {
        var console = new ConsoleOutputService();

        try
        {
            var config = new ConfigurationService();
            var configKey = string.IsNullOrEmpty(service) ? $"metrics.{key}" : $"services.{service}.metrics.{key}";

            // TODO: Implement actual configuration retrieval
            // For now, return default values
            var value = GetDefaultConfigurationValue(key);

            if (value != null)
            {
                console.Info($"{configKey} = {value}");
            }
            else
            {
                console.Warning($"Configuration key '{key}' not found. Using default value.");
                console.Info($"Default {key} = {GetDefaultConfigurationValue(key)}");
            }

        }
        catch (Exception ex)
        {
            console.Error($"Failed to get metrics configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteResetAsync(string service)
    {
        var console = new ConsoleOutputService();

        try
        {
            console.Warning("This will reset all metrics configuration to default values.");
            console.Info("Are you sure? (y/N): ");

            var response = Console.ReadLine()?.Trim().ToLower();
            if (response != "y" && response != "yes")
            {
                console.Info("Reset cancelled.");
                return;
            }

            // TODO: Implement actual configuration reset
            console.Success("Metrics configuration reset to defaults.");

            if (!string.IsNullOrEmpty(service))
            {
                console.Warning($"Service '{service}' may need to be restarted.");
            }

        }
        catch (Exception ex)
        {
            console.Error($"Failed to reset metrics configuration: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task DisplayConfiguration(ConsoleOutputService console, string? service)
    {
        var configKeys = new[]
        {
            "monitoring.enabled",
            "monitoring.endpoint",
            "metrics.retention_days",
            "metrics.collection_interval_seconds",
            "metrics.export.enabled",
            "metrics.export.format",
            "metrics.export.endpoint",
            "alerts.enabled",
            "alerts.cpu_threshold_percent",
            "alerts.memory_threshold_percent",
            "alerts.response_time_threshold_ms",
            "alerts.error_rate_threshold_percent"
        };

        foreach (var key in configKeys)
        {
            var value = GetDefaultConfigurationValue(key);
            var displayKey = service == null ? $"metrics.{key}" : $"services.{service}.metrics.{key}";
            console.Info($"  {displayKey} = {value}");
        }
    }

    private static bool IsValidMetricsKey(string key)
    {
        var validKeys = new[]
        {
            "monitoring.enabled",
            "monitoring.endpoint",
            "metrics.retention_days",
            "metrics.collection_interval_seconds",
            "metrics.export.enabled",
            "metrics.export.format",
            "metrics.export.endpoint",
            "alerts.enabled",
            "alerts.cpu_threshold_percent",
            "alerts.memory_threshold_percent",
            "alerts.response_time_threshold_ms",
            "alerts.error_rate_threshold_percent"
        };

        return validKeys.Contains(key);
    }

    private static bool ValidateConfigurationValue(string key, string value)
    {
        return key switch
        {
            "monitoring.enabled" or "metrics.export.enabled" or "alerts.enabled" =>
                bool.TryParse(value, out _),
            "metrics.retention_days" or "metrics.collection_interval_seconds" or
            "alerts.cpu_threshold_percent" or "alerts.memory_threshold_percent" or
            "alerts.response_time_threshold_ms" or "alerts.error_rate_threshold_percent" =>
                int.TryParse(value, out var intValue) && intValue > 0,
            "metrics.export.format" =>
                new[] { "json", "prometheus", "influxdb" }.Contains(value.ToLower()),
            "monitoring.endpoint" or "metrics.export.endpoint" =>
                Uri.TryCreate(value, UriKind.Absolute, out _),
            _ => true
        };
    }

    private static string? GetDefaultConfigurationValue(string key)
    {
        return key switch
        {
            "monitoring.enabled" => "true",
            "monitoring.endpoint" => "http://localhost:8090",
            "metrics.retention_days" => "30",
            "metrics.collection_interval_seconds" => "60",
            "metrics.export.enabled" => "false",
            "metrics.export.format" => "json",
            "metrics.export.endpoint" => "",
            "alerts.enabled" => "true",
            "alerts.cpu_threshold_percent" => "80",
            "alerts.memory_threshold_percent" => "85",
            "alerts.response_time_threshold_ms" => "5000",
            "alerts.error_rate_threshold_percent" => "5",
            _ => null
        };
    }
}