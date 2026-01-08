using B2X.Admin.MCP.Services;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Diagnostics;

namespace B2X.Admin.MCP.Tools;

/// <summary>
/// User Management Assistant Tool
/// </summary>
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
}

/// <summary>
/// AI Mode Switching Tool - Allows runtime switching between AI modes (Normal, Local, Network)
/// </summary>
public class AiModeSwitchingTool
{
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<AiModeSwitchingTool> _logger;

    public AiModeSwitchingTool(
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<AiModeSwitchingTool> logger)
    {
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(AiModeSwitchingArgs args)
    {
        _logger.LogInformation("AI mode switch requested by tenant {TenantId}. Target mode: {TargetMode}",
            _tenantContext.TenantId, args.TargetMode);

        try
        {
            // Validate the requested mode
            if (!Enum.TryParse<AiProviderSelector.AiMode>(args.TargetMode, true, out var targetMode))
            {
                var availableModes = string.Join(", ", Enum.GetNames<AiProviderSelector.AiMode>());
                return $"❌ Invalid AI mode '{args.TargetMode}'. Available modes: {availableModes}";
            }

            // Get current mode for logging
            var currentMode = _providerSelector.CurrentMode;

            // Perform the mode switch
            _providerSelector.SwitchMode(targetMode);

            // Verify the switch
            var newMode = _providerSelector.CurrentMode;
            if (newMode != targetMode)
            {
                _logger.LogError("AI mode switch failed. Expected {ExpectedMode}, got {ActualMode}",
                    targetMode, newMode);
                return $"❌ Failed to switch AI mode to {targetMode}";
            }

            _logger.LogInformation("AI mode successfully switched from {OldMode} to {NewMode} for tenant {TenantId}",
                currentMode, newMode, _tenantContext.TenantId);

            // Provide detailed response based on the new mode
            var response = GenerateModeSwitchResponse(currentMode, newMode, args.Reason);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch AI mode for tenant {TenantId}", _tenantContext.TenantId);
            return $"❌ Error switching AI mode: {ex.Message}";
        }
    }

    private string GenerateModeSwitchResponse(AiProviderSelector.AiMode oldMode, AiProviderSelector.AiMode newMode, string? reason)
    {
        var response = new System.Text.StringBuilder();

        response.AppendLine($"✅ AI mode switched successfully");
        response.AppendLine($"From: {oldMode} → To: {newMode}");

        if (!string.IsNullOrEmpty(reason))
        {
            response.AppendLine($"Reason: {reason}");
        }

        response.AppendLine();
        response.AppendLine("**Mode Details:**");

        switch (newMode)
        {
            case AiProviderSelector.AiMode.Normal:
                response.AppendLine("🔄 **Normal Mode**: Uses preferred AI providers (OpenAI, Anthropic, etc.)");
                response.AppendLine("   - Cost: Variable (based on API usage)");
                response.AppendLine("   - Speed: Fast (cloud infrastructure)");
                response.AppendLine("   - Reliability: High (managed services)");
                response.AppendLine("   - Privacy: Data sent to external providers");
                break;

            case AiProviderSelector.AiMode.Local:
                response.AppendLine("🏠 **Local Mode**: Uses local Ollama instance");
                response.AppendLine("   - Cost: Free (local resources)");
                response.AppendLine("   - Speed: Variable (depends on local hardware)");
                response.AppendLine("   - Reliability: Depends on local setup");
                response.AppendLine("   - Privacy: All processing local");
                response.AppendLine("   - Setup: Requires local Ollama server");
                break;

            case AiProviderSelector.AiMode.Network:
                response.AppendLine("🌐 **Network Mode**: Uses network-hosted Ollama servers");
                response.AppendLine("   - Cost: Low (shared infrastructure)");
                response.AppendLine("   - Speed: Fast (dedicated network servers)");
                response.AppendLine("   - Reliability: High (managed network infrastructure)");
                response.AppendLine("   - Privacy: Data stays within network");
                response.AppendLine("   - Setup: Requires network Ollama endpoints");
                break;
        }

        response.AppendLine();
        response.AppendLine("**Next Steps:**");
        response.AppendLine("- Test AI functionality with a sample request");
        response.AppendLine("- Monitor performance and costs");
        response.AppendLine("- Switch back if needed using the same tool");

        return response.ToString();
    }
}

/// <summary>
/// Arguments for AI mode switching
/// </summary>
public class AiModeSwitchingArgs
{
    public string TargetMode { get; set; } = null!;
    public string? Reason { get; set; }
}

/// <summary>
/// Individual validation issue - matches CMS domain structure
/// </summary>
public class ValidationIssue
{
    public string Category { get; set; } = null!; // "syntax", "security", "performance", "best-practices"
    public string Severity { get; set; } = null!; // "error", "warning", "info"
    public string Message { get; set; } = null!;
    public string? Code { get; set; } // Error code for programmatic handling
    public int? LineNumber { get; set; }
    public int? ColumnNumber { get; set; }
    public string? Source { get; set; } // Which validation stage found this issue
    public string? Suggestion { get; set; } // Single suggestion for simple cases
    public List<string> Suggestions { get; set; } = new(); // Multiple suggestions
    public Dictionary<string, object> Context { get; set; } = new();
}

/// <summary>
/// Result from AI analysis during template validation
/// </summary>
public class AiValidationResult
{
    public List<ValidationIssue> Issues { get; set; } = new();
    public string Suggestions { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
}