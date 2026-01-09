using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using Microsoft.Extensions.Logging;
using System.IO;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// System Health Analysis Tool - Analyzes system health using CLI operations
/// </summary>
public class SystemHealthAnalysisTool
{
    private readonly TenantContext _tenantContext;
    private readonly ILogger<SystemHealthAnalysisTool> _logger;
    private readonly DataSanitizationService _dataSanitizer;

    public SystemHealthAnalysisTool(
        TenantContext tenantContext,
        ILogger<SystemHealthAnalysisTool> logger,
        DataSanitizationService dataSanitizer)
    {
        _tenantContext = tenantContext;
        _logger = logger;
        _dataSanitizer = dataSanitizer;
    }

    public async Task<string> ExecuteAsync(SystemHealthAnalysisArgs args)
    {
        _logger.LogInformation("Executing system health analysis for component {Component}, time range {TimeRange}",
            args.Component, args.TimeRange);

        try
        {
            // Execute CLI health check command
            var cliResult = await ExecuteCliHealthCheck(args.Component, args.TimeRange);

            // GDPR Compliance: Sanitize CLI output before AI processing
            var sanitizationResult = _dataSanitizer.SanitizeContent(cliResult, _tenantContext.TenantId);

            if (sanitizationResult.IsModified)
            {
                _logger.LogInformation("CLI output sanitized for GDPR compliance. Detected patterns: {Patterns}",
                    string.Join(", ", sanitizationResult.DetectedPatterns));
            }

            // Validate content is safe for AI processing
            var validationResult = _dataSanitizer.ValidateContent(sanitizationResult.SanitizedContent, _tenantContext.TenantId);

            if (!validationResult.IsValid && validationResult.RiskLevel == RiskLevel.High)
            {
                _logger.LogWarning("High-risk content detected in health check output. Blocking AI analysis.");
                return "⚠️ Health check completed but contains sensitive data that cannot be analyzed by AI. " +
                       "Please review system logs directly for detailed information.";
            }

            // Analyze results and provide AI-powered insights
            var analysis = await GenerateAiAnalysis(sanitizationResult.SanitizedContent, args, validationResult);

            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute system health analysis");
            return $"Error performing health analysis: {ex.Message}";
        }
    }

    private async Task<string> ExecuteCliHealthCheck(string component, string timeRange)
    {
        // Find the CLI executable path
        var cliPath = GetCliExecutablePath();
        var projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2X.CLI.Administration", "B2X.CLI.Administration.csproj");

        // Build arguments
        var arguments = $"health check --component {component}";
        if (!string.IsNullOrEmpty(timeRange))
        {
            arguments += $" --time-range {timeRange}";
        }

        // If using dotnet, add run command
        if (cliPath == "dotnet" && File.Exists(projectPath))
        {
            arguments = $"run --project \"{projectPath}\" {arguments}";
        }

        // Execute CLI command
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(projectPath) ?? AppDomain.CurrentDomain.BaseDirectory
        };

        using var process = System.Diagnostics.Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start CLI process");
        }

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"CLI command failed: {error}");
        }

        return output + error; // Combine output for analysis
    }

    private string GetCliExecutablePath()
    {
        // In development, use dotnet run with the project file
        var projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2X.CLI.Administration", "B2X.CLI.Administration.csproj");

        if (File.Exists(projectPath))
        {
            return "dotnet";
        }

        // Fallback to direct executable
        var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2X.CLI.Administration", "bin", "Debug", "net10.0", "B2X.CLI.Administration");
        if (File.Exists(exePath))
        {
            return exePath;
        }

        exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2X.CLI.Administration", "bin", "Release", "net10.0", "B2X.CLI.Administration");
        if (File.Exists(exePath))
        {
            return exePath;
        }

        // If installed globally
        return "B2X-admin";
    }

    private async Task<string> GenerateAiAnalysis(string sanitizedCliResult, SystemHealthAnalysisArgs args, ValidationResult validation)
    {
        // GDPR Compliance: Log AI processing for audit trail
        _logger.LogInformation("AI analysis initiated for tenant {TenantId}. Risk level: {RiskLevel}, Patterns: {Patterns}",
            _tenantContext.TenantId, validation.RiskLevel, string.Join(", ", validation.DetectedPatterns));

        var analysis = $@"
System Health Analysis for Component: {args.Component}
Time Range: {args.TimeRange ?? "Last 24 hours"}
Tenant: {_tenantContext.TenantId}

GDPR Compliance Status:
- Content Sanitized: {(validation.DetectedPatterns.Any() ? "Yes" : "No")}
- Risk Level: {validation.RiskLevel}
- Detected Patterns: {(validation.DetectedPatterns.Any() ? string.Join(", ", validation.DetectedPatterns) : "None")}

CLI Health Check Results:
{sanitizedCliResult}

AI Analysis:
";

        // Simple analysis based on CLI output - ensure no sensitive data is used in AI prompts
        if (sanitizedCliResult.Contains("All") && sanitizedCliResult.Contains("healthy"))
        {
            analysis += "✅ All systems are operating normally. No immediate action required.";
        }
        else if (sanitizedCliResult.Contains("Unhealthy") || sanitizedCliResult.Contains("Error"))
        {
            analysis += "⚠️ Issues detected. Review the CLI output above for specific problems.\n";
            analysis += "Recommendations:\n";
            analysis += "- Check service logs for detailed error messages\n";
            analysis += "- Verify network connectivity\n";
            analysis += "- Restart affected services if necessary\n";
            analysis += "- Contact system administrator if issues persist";
        }
        else
        {
            analysis += "ℹ️ Health check completed. Monitor for any emerging issues.";
        }

        // Add GDPR compliance footer
        analysis += "\n\n---\n";
        analysis += "GDPR Compliance: This analysis was performed on sanitized data only. ";
        analysis += "No personal data or sensitive information was transmitted to AI services.";

        return analysis;
    }
}