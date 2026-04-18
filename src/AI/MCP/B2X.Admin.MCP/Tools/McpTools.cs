using System.Diagnostics;
using System.IO;
using B2X.Admin.MCP.Middleware;
using B2X.Admin.MCP.Models;
using B2X.Admin.MCP.Services;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;

namespace B2X.Admin.MCP.Tools;

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
