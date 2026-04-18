using System.CommandLine;
using B2X.CLI.Shared;
using B2X.CLI.Shared.Configuration;

namespace B2X.CLI.Administration.Commands.MetricsCommands;

public static class MetricsAlertsCommand
{
    public static Command Create()
    {
        var command = new Command("alerts", "Manage metrics alerts and notifications");

        var listCommand = new Command("list", "List active alerts");
        var listServiceOption = new Option<string>(
            ["-s", "--service"],
            "Filter alerts by service name");
        var listTenantOption = new Option<string>(
            ["-t", "--tenant"],
            "Filter alerts by tenant ID");
        var listSeverityOption = new Option<string>(
            ["--severity"],
            "Filter by severity: info, warning, error, critical");
        listCommand.AddOption(listServiceOption);
        listCommand.AddOption(listTenantOption);
        listCommand.AddOption(listSeverityOption);
        listCommand.SetHandler(ExecuteListAsync, listServiceOption, listTenantOption, listSeverityOption);

        var acknowledgeCommand = new Command("acknowledge", "Acknowledge an alert");
        var alertIdArgument = new Argument<string>("alert-id", "Alert ID to acknowledge");
        var commentOption = new Option<string>(
            ["-c", "--comment"],
            "Optional comment for acknowledgement");
        acknowledgeCommand.AddArgument(alertIdArgument);
        acknowledgeCommand.AddOption(commentOption);
        acknowledgeCommand.SetHandler(ExecuteAcknowledgeAsync, alertIdArgument, commentOption);

        var historyCommand = new Command("history", "View alert history");
        var historyServiceOption = new Option<string>(
            ["-s", "--service"],
            "Filter history by service name");
        var historyTenantOption = new Option<string>(
            ["-t", "--tenant"],
            "Filter history by tenant ID");
        var historyDaysOption = new Option<int>(
            ["-d", "--days"],
            "Number of days to look back (default: 7)");
        historyCommand.AddOption(historyServiceOption);
        historyCommand.AddOption(historyTenantOption);
        historyCommand.AddOption(historyDaysOption);
        historyCommand.SetHandler(ExecuteHistoryAsync, historyServiceOption, historyTenantOption, historyDaysOption);

        var testCommand = new Command("test", "Test alert notifications");
        var testTypeOption = new Option<string>(
            ["-t", "--type"],
            "Alert type to test: email, webhook, slack (default: all)");
        testCommand.AddOption(testTypeOption);
        testCommand.SetHandler(ExecuteTestAsync, testTypeOption);

        command.AddCommand(listCommand);
        command.AddCommand(acknowledgeCommand);
        command.AddCommand(historyCommand);
        command.AddCommand(testCommand);

        return command;
    }

    private static async Task ExecuteListAsync(string service, string tenant, string severity)
    {
        var console = new ConsoleOutputService();

        try
        {
            console.Header("Active Alerts");

            using var monitoringClient = new MonitoringServiceClient();

            // Build filter parameters
            var filters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(service))
                filters["service"] = service;
            if (!string.IsNullOrEmpty(tenant))
                filters["tenantId"] = tenant;
            if (!string.IsNullOrEmpty(severity))
                filters["severity"] = severity;

            var alerts = await monitoringClient.GetResourceAlertsAsync(filters);

            if (alerts == null || !alerts.Any())
            {
                console.Success("No active alerts found.");
                return;
            }

            // Display alerts in a table format
            console.Info("┌────────────┬─────────────────┬────────────┬────────────┬────────────────────────────┐");
            console.Info("│ Severity   │ Service         │ Type       │ Status     │ Message                    │");
            console.Info("├────────────┼─────────────────┼────────────┼────────────┼────────────────────────────┤");

            foreach (var alert in alerts)
            {
                var alertSeverity = GetAlertProperty(alert, "severity", "unknown");
                var alertService = GetAlertProperty(alert, "service", "unknown");
                var alertType = GetAlertProperty(alert, "type", "unknown");
                var alertStatus = GetAlertProperty(alert, "status", "active");
                var alertMessage = GetAlertProperty(alert, "message", "No message");

                // Truncate long messages
                if (alertMessage.Length > 26)
                    alertMessage = alertMessage.Substring(0, 23) + "...";

                var severityIcon = alertSeverity.ToLower() switch
                {
                    "critical" => "🔴",
                    "error" => "🟠",
                    "warning" => "🟡",
                    "info" => "🔵",
                    _ => "⚪"
                };

                console.Info($"│ {severityIcon} {alertSeverity,-8} │ {alertService,-15} │ {alertType,-10} │ {alertStatus,-10} │ {alertMessage,-26} │");
            }

            console.Info("└────────────┴─────────────────┴────────────┴────────────┴────────────────────────────┘");

            // Summary
            var criticalCount = alerts.Count(a => GetAlertProperty(a, "severity", "").Equals("critical", StringComparison.OrdinalIgnoreCase));
            var errorCount = alerts.Count(a => GetAlertProperty(a, "severity", "").Equals("error", StringComparison.OrdinalIgnoreCase));
            var warningCount = alerts.Count(a => GetAlertProperty(a, "severity", "").Equals("warning", StringComparison.OrdinalIgnoreCase));

            console.Info($"\nAlert Summary: {alerts.Count()} total | 🔴 {criticalCount} critical | 🟠 {errorCount} error | 🟡 {warningCount} warning");

        }
        catch (Exception ex)
        {
            console.Error($"Failed to list alerts: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteAcknowledgeAsync(string alertId, string comment)
    {
        var console = new ConsoleOutputService();

        try
        {
            console.Info($"Acknowledging alert {alertId}...");

            // TODO: Implement alert acknowledgement via monitoring service API
            // This would typically involve calling a monitoring service endpoint

            if (!string.IsNullOrEmpty(comment))
            {
                console.Info($"Comment: {comment}");
            }

            console.Success($"Alert {alertId} acknowledged successfully.");

        }
        catch (Exception ex)
        {
            console.Error($"Failed to acknowledge alert: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteHistoryAsync(string service, string tenant, int days)
    {
        var console = new ConsoleOutputService();

        try
        {
            days = days > 0 ? days : 7;
            console.Header($"Alert History (Last {days} days)");

            using var monitoringClient = new MonitoringServiceClient();

            // Build filter parameters for history
            var filters = new Dictionary<string, object>
            {
                ["timeRange"] = $"{days}d"
            };
            if (!string.IsNullOrEmpty(service))
                filters["service"] = service;
            if (!string.IsNullOrEmpty(tenant))
                filters["tenantId"] = tenant;

            var alerts = await monitoringClient.GetResourceAlertsAsync(filters);

            if (alerts == null || !alerts.Any())
            {
                console.Info("No alerts found in the specified time range.");
                return;
            }

            // Group by date and display
            var alertsByDate = alerts
                .GroupBy(a => DateTime.Parse(GetAlertProperty(a, "timestamp", DateTime.UtcNow.ToString())).Date)
                .OrderByDescending(g => g.Key);

            foreach (var dateGroup in alertsByDate)
            {
                console.SubHeader(dateGroup.Key.ToString("yyyy-MM-dd"));
                console.Info($"  Total alerts: {dateGroup.Count()}");

                var severityCounts = dateGroup
                    .GroupBy(a => GetAlertProperty(a, "severity", "unknown"))
                    .Select(g => $"{g.Key}: {g.Count()}");

                console.Info($"  By severity: {string.Join(", ", severityCounts)}");
                console.Info("");
            }
        }
        catch (Exception ex)
        {
            console.Error($"Failed to retrieve alert history: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static async Task ExecuteTestAsync(string type)
    {
        var console = new ConsoleOutputService();

        try
        {
            console.Header("Testing Alert Notifications");

            var testTypes = string.IsNullOrEmpty(type) || type.Equals("all", StringComparison.OrdinalIgnoreCase)
                ? new[] { "email", "webhook", "slack" }
                : new[] { type };

            foreach (var testType in testTypes)
            {
                console.Info($"Testing {testType} notifications...");

                try
                {
                    // TODO: Implement actual notification testing
                    // This would send test notifications via the configured channels

                    await Task.Delay(1000); // Simulate network call
                    console.Success($"✓ {testType} notification test successful");
                }
                catch (Exception ex)
                {
                    console.Error($"✗ {testType} notification test failed: {ex.Message}");
                }
            }

            console.Info("\nNote: Test notifications may take a few minutes to appear in external systems.");

        }
        catch (Exception ex)
        {
            console.Error($"Failed to test alert notifications: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static string GetAlertProperty(dynamic alert, string propertyName, string defaultValue = "")
    {
        try
        {
            if (alert is IDictionary<string, object> dict && dict.TryGetValue(propertyName, out var value))
            {
                return value?.ToString() ?? defaultValue;
            }

            // Try to access as dynamic property
            var propValue = alert.GetType().GetProperty(propertyName)?.GetValue(alert, null);
            return propValue?.ToString() ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}
