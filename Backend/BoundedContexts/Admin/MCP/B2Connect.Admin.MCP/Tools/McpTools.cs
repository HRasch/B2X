using B2Connect.Admin.MCP.Services;
using B2Connect.Admin.MCP.Middleware;
using B2Connect.Admin.MCP.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Diagnostics;

namespace B2Connect.Admin.MCP.Tools;

/// <summary>
/// CMS Page Design Tool - Uses AI to design and optimize CMS page layouts
/// </summary>
public class CmsPageDesignTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<CmsPageDesignTool> _logger;

    public CmsPageDesignTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<CmsPageDesignTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(CmsPageDesignArgs args)
    {
        var prompt = await GetSystemPromptAsync("cms_page_design");

        var userMessage = $@"
Design a CMS page layout for the following requirements:

Page Type: {args.PageType}
Content Requirements: {args.ContentRequirements}
Target Audience: {args.TargetAudience ?? "General audience"}

Please provide:
1. Overall page structure and layout recommendations
2. Content sections and their purposes
3. User experience considerations
4. Mobile responsiveness suggestions
5. SEO optimization recommendations
6. Accessibility considerations

Format your response as a structured design document.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "cms_page_design",
            "openai", // Default provider name for consumption tracking
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to generate page design";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        // TODO: Retrieve from database based on tenant and tool type
        return @"
You are an expert CMS page designer and UX specialist. Your task is to create professional, user-friendly page layouts that balance aesthetics, functionality, and conversion optimization.

Guidelines:
- Focus on user experience and conversion
- Ensure mobile-first responsive design
- Include accessibility best practices
- Optimize for SEO and performance
- Follow modern web design principles
- Consider content hierarchy and readability

Provide detailed, actionable recommendations with specific implementation suggestions.
";
    }
}

/// <summary>
/// Email Template Design Tool
/// </summary>
public class EmailTemplateDesignTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<EmailTemplateDesignTool> _logger;

    public EmailTemplateDesignTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<EmailTemplateDesignTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(EmailTemplateDesignArgs args)
    {
        var prompt = await GetSystemPromptAsync("email_template_design");

        var userMessage = $@"
Design an email template for the following requirements:

Email Type: {args.EmailType}
Content Purpose: {args.ContentPurpose}
Brand Guidelines: {args.BrandGuidelines ?? "Standard professional guidelines"}

Please provide:
1. Email structure and layout
2. Subject line suggestions
3. Content sections and hierarchy
4. Call-to-action placement
5. Mobile optimization
6. Deliverability considerations
7. HTML/CSS implementation notes

Format as a complete email template specification.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "email_template_design",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to generate email template";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert email marketing designer. Create professional, effective email templates that drive engagement and conversions.

Guidelines:
- Optimize for mobile devices first
- Ensure high deliverability and inbox placement
- Use clear, compelling calls-to-action
- Maintain brand consistency
- Follow email marketing best practices
- Consider accessibility and readability
- Include unsubscribe and contact information

Provide complete template specifications with HTML structure recommendations.
";
    }
}

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
        var projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2Connect.CLI.Administration", "B2Connect.CLI.Administration.csproj");

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
        var projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2Connect.CLI.Administration", "B2Connect.CLI.Administration.csproj");

        if (File.Exists(projectPath))
        {
            return "dotnet";
        }

        // Fallback to direct executable
        var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2Connect.CLI.Administration", "bin", "Debug", "net10.0", "B2Connect.CLI.Administration");
        if (File.Exists(exePath))
        {
            return exePath;
        }

        exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "CLI", "B2Connect.CLI.Administration", "bin", "Release", "net10.0", "B2Connect.CLI.Administration");
        if (File.Exists(exePath))
        {
            return exePath;
        }

        // If installed globally
        return "b2connect-admin";
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

/// <summary>
/// User Management Assistant Tool
/// </summary>
public class UserManagementAssistantTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<UserManagementAssistantTool> _logger;

    public UserManagementAssistantTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<UserManagementAssistantTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(UserManagementAssistantArgs args)
    {
        var prompt = await GetSystemPromptAsync("user_management");

        var userMessage = $@"
Analyze the following user management task and provide recommendations:

Task: {args.Task}
Context: {args.UserContext ?? "General user management"}

Please provide:
1. Task breakdown and steps
2. Security considerations
3. Best practices recommendations
4. Potential risks and mitigations
5. Implementation suggestions
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "user_management_assistant",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze user management task";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert in user management and identity administration. Provide secure, efficient solutions for user lifecycle management.

Guidelines:
- Prioritize security and compliance
- Follow principle of least privilege
- Ensure audit trails and accountability
- Consider user experience and ease of administration
- Recommend automation where appropriate
- Address GDPR and privacy requirements

Provide actionable recommendations with security considerations.
";
    }
}

/// <summary>
/// Content Optimization Tool
/// </summary>
public class ContentOptimizationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<ContentOptimizationTool> _logger;

    public ContentOptimizationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<ContentOptimizationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(ContentOptimizationArgs args)
    {
        var prompt = await GetSystemPromptAsync("content_optimization");

        var keywords = args.TargetKeywords != null ? string.Join(", ", args.TargetKeywords) : "None specified";

        var userMessage = $@"
Optimize the following content for SEO and user engagement:

Content Type: {args.ContentType}
Target Keywords: {keywords}

Content to optimize:
{args.Content}

Please provide:
1. SEO optimization recommendations
2. Content structure improvements
3. Keyword optimization suggestions
4. Readability enhancements
5. Engagement optimization tips
6. Performance considerations

Format as an optimization report with before/after comparisons.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "content_optimization",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to optimize content";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a content optimization expert specializing in SEO and user engagement. Improve content quality, readability, and search engine visibility.

Guidelines:
- Optimize for both users and search engines
- Improve readability and user experience
- Strategic keyword placement without keyword stuffing
- Enhance content structure and formatting
- Boost engagement and social sharing potential
- Ensure mobile-friendly presentation
- Follow SEO best practices and E-E-A-T guidelines

Provide specific, actionable optimization recommendations with measurable improvements.
";
    }
}

/// <summary>
/// Tenant Management Tool - Manages tenant lifecycle and resources
/// </summary>
public class TenantManagementTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<TenantManagementTool> _logger;

    public TenantManagementTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<TenantManagementTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(TenantManagementArgs args)
    {
        var prompt = await GetSystemPromptAsync("tenant_management");

        var userMessage = $@"
Analyze and provide recommendations for tenant management task:

Action: {args.Action}
Tenant ID: {args.TenantId}
Resource Type: {args.ResourceType ?? "General"}
Configuration: {args.Configuration ?? "Current configuration"}

Please provide:
1. Action assessment and feasibility analysis
2. Resource impact analysis
3. Security and compliance considerations
4. Implementation steps and timeline
5. Risk assessment and mitigation strategies
6. Monitoring and rollback procedures

Format as a comprehensive tenant management report.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "tenant_management",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze tenant management task";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert in multi-tenant SaaS architecture and tenant lifecycle management. Provide strategic guidance for tenant operations while ensuring system stability and security.

Guidelines:
- Prioritize system stability and performance
- Ensure proper resource isolation and limits
- Maintain security boundaries between tenants
- Consider scalability and cost implications
- Follow tenant management best practices
- Provide actionable, prioritized recommendations

Focus on operational excellence and risk mitigation in multi-tenant environments.
";
    }
}

/// <summary>
/// Store Operations Tool - Analyzes and optimizes store performance
/// </summary>
public class StoreOperationsTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<StoreOperationsTool> _logger;

    public StoreOperationsTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<StoreOperationsTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(StoreOperationsArgs args)
    {
        var prompt = await GetSystemPromptAsync("store_operations");

        var userMessage = $@"
Analyze store operations and provide optimization recommendations:

Operation: {args.Operation}
Store ID: {args.StoreId}
Time Range: {args.TimeRange ?? "Last 30 days"}
Analysis Type: {args.AnalysisType ?? "Performance"}

Please provide:
1. Current performance metrics analysis
2. Operational efficiency assessment
3. Inventory and sales optimization recommendations
4. Customer experience insights
5. Competitive positioning analysis
6. Actionable improvement roadmap

Format as a comprehensive store operations report with prioritized recommendations.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "store_operations",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze store operations";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an expert retail operations consultant specializing in e-commerce and B2B marketplace optimization. Provide data-driven insights and strategic recommendations for store performance improvement.

Guidelines:
- Focus on revenue growth and operational efficiency
- Analyze customer behavior and market trends
- Optimize inventory management and supply chain
- Enhance user experience and conversion rates
- Provide measurable KPIs and success metrics
- Consider seasonal and market dynamics
- Balance short-term gains with long-term strategy

Deliver actionable insights that drive tangible business results.
";
    }
}

/// <summary>
/// Security & Compliance Tool - Ensures security standards and compliance
/// </summary>
public class SecurityComplianceTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<SecurityComplianceTool> _logger;

    public SecurityComplianceTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<SecurityComplianceTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(SecurityComplianceArgs args)
    {
        var prompt = await GetSystemPromptAsync("security_compliance");

        var userMessage = $@"
Conduct security and compliance assessment:

Assessment Type: {args.AssessmentType}
Scope: {args.Scope}
Specific Component: {args.SpecificComponent ?? "All components"}
Include Recommendations: {args.IncludeRecommendations}

Please provide:
1. Current security posture analysis
2. Compliance gap identification
3. Risk assessment and prioritization
4. Remediation recommendations with timelines
5. Monitoring and auditing procedures
6. Regulatory compliance verification

Format as a comprehensive security and compliance report with actionable remediation steps.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "security_compliance",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to conduct security assessment";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a cybersecurity and compliance expert specializing in SaaS platforms and GDPR/NIS2 regulations. Provide comprehensive security assessments and compliance guidance for multi-tenant systems.

Guidelines:
- Follow OWASP Top 10 and industry security standards
- Ensure GDPR, NIS2, and BITV 2.0 compliance
- Prioritize critical vulnerabilities and data protection
- Provide practical, implementable security measures
- Consider both technical and organizational controls
- Balance security with usability and performance
- Include monitoring and incident response procedures

Deliver security recommendations that protect both the platform and tenant data while maintaining operational efficiency.
";
    }
}

/// <summary>
/// Performance Optimization Tool - Analyzes and improves system performance
/// </summary>
public class PerformanceOptimizationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<PerformanceOptimizationTool> _logger;

    public PerformanceOptimizationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<PerformanceOptimizationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(PerformanceOptimizationArgs args)
    {
        var prompt = await GetSystemPromptAsync("performance_optimization");

        var userMessage = $@"
Analyze system performance and provide optimization recommendations:

Component: {args.Component}
Metric Type: {args.MetricType}
Time Range: {args.TimeRange ?? "Last 7 days"}
Include Historical Data: {args.IncludeHistoricalData}

Please provide:
1. Current performance baseline analysis
2. Bottleneck identification and root cause analysis
3. Optimization recommendations with expected impact
4. Implementation priority and effort estimation
5. Monitoring and alerting recommendations
6. Scalability and capacity planning insights

Format as a comprehensive performance optimization report with quantified improvement projections.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "performance_optimization",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze performance";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are a performance engineering expert specializing in distributed systems and cloud-native applications. Provide data-driven performance analysis and optimization strategies for high-throughput SaaS platforms.

Guidelines:
- Focus on measurable performance improvements
- Consider scalability, reliability, and cost efficiency
- Analyze bottlenecks across the entire stack
- Provide evidence-based recommendations
- Include monitoring and observability requirements
- Balance optimization with maintainability
- Consider both immediate fixes and architectural improvements

Deliver performance insights that drive tangible improvements in speed, reliability, and user experience.
";
    }
}

/// <summary>
/// Integration Management Tool - Manages external integrations and APIs
/// </summary>
public class IntegrationManagementTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<IntegrationManagementTool> _logger;

    public IntegrationManagementTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        ILogger<IntegrationManagementTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(IntegrationManagementArgs args)
    {
        var prompt = await GetSystemPromptAsync("integration_management");

        var userMessage = $@"
Analyze and optimize external integration:

Integration Type: {args.IntegrationType}
Action: {args.Action}
Endpoint: {args.Endpoint ?? "N/A"}
Configuration: {args.Configuration ?? "Default"}

Please provide:
1. Integration health assessment
2. Performance and reliability analysis
3. Security and data protection evaluation
4. Optimization recommendations
5. Monitoring and alerting setup
6. Troubleshooting procedures and best practices

Format as a comprehensive integration management report with implementation guidance.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "integration_management",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        return aiResponse.Response?.Content ?? "Failed to analyze integration";
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        return @"
You are an integration architecture expert specializing in API design, microservices communication, and enterprise system integration. Provide strategic guidance for building robust, scalable integration solutions.

Guidelines:
- Ensure reliable, secure, and performant integrations
- Follow API design best practices and standards
- Implement proper error handling and resilience patterns
- Consider monitoring, logging, and observability
- Balance integration complexity with business value
- Provide migration and modernization strategies
- Include testing and validation procedures

Deliver integration solutions that are maintainable, scalable, and business-critical reliable.
";
    }
}

/// <summary>
/// Template Validation Tool - Validates CMS templates for syntax, security, and best practices (ADR-030 Phase 2)
/// </summary>
public class TemplateValidationTool
{
    private readonly AiConsumptionGateway _aiGateway;
    private readonly AiProviderSelector _providerSelector;
    private readonly TenantContext _tenantContext;
    private readonly CmsValidationClient _cmsValidationClient;
    private readonly ILogger<TemplateValidationTool> _logger;

    public TemplateValidationTool(
        AiConsumptionGateway aiGateway,
        AiProviderSelector providerSelector,
        TenantContext tenantContext,
        CmsValidationClient cmsValidationClient,
        ILogger<TemplateValidationTool> logger)
    {
        _aiGateway = aiGateway;
        _providerSelector = providerSelector;
        _tenantContext = tenantContext;
        _cmsValidationClient = cmsValidationClient;
        _logger = logger;
    }

    public async Task<TemplateValidationResult> ExecuteAsync(TemplateValidationArgs args)
    {
        _logger.LogInformation("Validating template for tenant {TenantId}, template key {TemplateKey}",
            _tenantContext.TenantId, args.TemplateKey);

        // Call the CMS domain validation service for comprehensive validation
        var result = await _cmsValidationClient.ValidateTemplateAsync(
            _tenantContext.TenantId,
            args.TemplateKey,
            args.TemplateContent);

        _logger.LogInformation(
            "Template validation completed for {TemplateKey}: Status={Status}, Issues={IssueCount}, Confidence={Confidence}",
            args.TemplateKey, result.OverallStatus, result.ValidationResults.Count, result.ConfidenceScore);

        return result;
    }

    private async Task<List<ValidationIssue>> ValidateSyntaxAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Basic syntax checks
        if (string.IsNullOrWhiteSpace(templateContent))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "error",
                Category = "syntax",
                Message = "Template content is empty",
                Suggestion = "Provide template content to validate"
            });
            return issues;
        }

        // Check for basic HTML structure
        if (!templateContent.Contains('<') || !templateContent.Contains('>'))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "syntax",
                Message = "Template does not appear to contain HTML markup",
                Suggestion = "Ensure template contains proper HTML structure"
            });
        }

        // Check for unclosed tags (basic check)
        var openTags = new Stack<string>();
        var tagRegex = new System.Text.RegularExpressions.Regex(@"<(/?)(\w+)[^>]*>");
        var matches = tagRegex.Matches(templateContent);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var isClosing = match.Groups[1].Value == "/";
            var tagName = match.Groups[2].Value.ToLower();

            if (!isClosing)
            {
                // Self-closing tags don't need to be closed
                if (!tagName.EndsWith('/') && !new[] { "br", "hr", "img", "input", "meta", "link" }.Contains(tagName))
                {
                    openTags.Push(tagName);
                }
            }
            else
            {
                if (openTags.Count == 0 || openTags.Pop() != tagName)
                {
                    issues.Add(new ValidationIssue
                    {
                        Severity = "error",
                        Category = "syntax",
                        Message = $"Mismatched closing tag: </{tagName}>",
                        Suggestion = "Check HTML tag structure and ensure proper nesting"
                    });
                }
            }
        }

        if (openTags.Count > 0)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "error",
                Category = "syntax",
                Message = $"Unclosed tags: {string.Join(", ", openTags)}",
                Suggestion = "Close all opened HTML tags"
            });
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidateSecurityAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for potentially dangerous patterns
        var dangerousPatterns = new[]
        {
            ("<script[^>]*>.*?</script>", "Inline JavaScript detected", "Move JavaScript to external files"),
            ("javascript:", "JavaScript URL scheme detected", "Avoid javascript: URLs for security"),
            ("on\\w+\\s*=", "Inline event handlers detected", "Use addEventListener instead of inline handlers"),
            ("eval\\(", "eval() function usage detected", "Avoid eval() for security reasons"),
            ("innerHTML\\s*=", "innerHTML assignment detected", "Use textContent or createElement for security"),
            ("document\\.write\\(", "document.write() usage detected", "Avoid document.write() as it can lead to XSS")
        };

        foreach (var (pattern, message, suggestion) in dangerousPatterns)
        {
            var regex = new System.Text.RegularExpressions.Regex(pattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                System.Text.RegularExpressions.RegexOptions.Singleline);

            if (regex.IsMatch(templateContent))
            {
                issues.Add(new ValidationIssue
                {
                    Severity = "error",
                    Category = "security",
                    Message = message,
                    Suggestion = suggestion
                });
            }
        }

        // Check for proper escaping of user input placeholders
        if (templateContent.Contains("{{") && templateContent.Contains("}}"))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "security",
                Message = "Template variables detected - ensure proper escaping",
                Suggestion = "Verify that all user input is properly escaped to prevent XSS"
            });
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidatePerformanceAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for large inline styles
        var styleRegex = new System.Text.RegularExpressions.Regex(@"<style[^>]*>(.*?)</style>",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |
            System.Text.RegularExpressions.RegexOptions.Singleline);

        var styleMatch = styleRegex.Match(templateContent);
        if (styleMatch.Success && styleMatch.Groups[1].Value.Length > 2000)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "performance",
                Message = "Large inline CSS detected (>2KB)",
                Suggestion = "Move CSS to external stylesheet for better caching and performance"
            });
        }

        // Check for excessive DOM depth (basic heuristic)
        var nestingLevel = 0;
        var maxNesting = 0;
        for (int i = 0; i < templateContent.Length; i++)
        {
            char c = templateContent[i];
            if (c == '<' && i + 1 < templateContent.Length)
            {
                if (templateContent[i + 1] != '/')
                {
                    nestingLevel++;
                    maxNesting = Math.Max(maxNesting, nestingLevel);
                }
                else
                {
                    nestingLevel = Math.Max(0, nestingLevel - 1);
                }
            }
        }

        if (maxNesting > 10)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "performance",
                Message = $"Deep DOM nesting detected (max depth: {maxNesting})",
                Suggestion = "Consider flattening DOM structure for better rendering performance"
            });
        }

        // Check for missing alt attributes on images
        var imgRegex = new System.Text.RegularExpressions.Regex(@"<img[^>]*>",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        foreach (System.Text.RegularExpressions.Match match in imgRegex.Matches(templateContent))
        {
            if (!match.Value.Contains("alt="))
            {
                issues.Add(new ValidationIssue
                {
                    Severity = "warning",
                    Category = "accessibility",
                    Message = "Image without alt attribute found",
                    Suggestion = "Add alt attribute to all images for accessibility"
                });
            }
        }

        return issues;
    }

    private async Task<List<ValidationIssue>> ValidateBestPracticesAsync(string templateContent)
    {
        var issues = new List<ValidationIssue>();

        // Check for semantic HTML usage
        var semanticTags = new[] { "header", "nav", "main", "section", "article", "aside", "footer" };
        var hasSemanticTags = semanticTags.Any(tag =>
            templateContent.Contains($"<{tag}") || templateContent.Contains($"<{tag} "));

        if (!hasSemanticTags)
        {
            issues.Add(new ValidationIssue
            {
                Severity = "info",
                Category = "best_practice",
                Message = "No semantic HTML tags detected",
                Suggestion = "Consider using semantic HTML tags (header, nav, main, section, etc.) for better SEO and accessibility"
            });
        }

        // Check for proper doctype
        if (!templateContent.TrimStart().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "info",
                Category = "best_practice",
                Message = "DOCTYPE declaration missing",
                Suggestion = "Add <!DOCTYPE html> declaration at the beginning of the template"
            });
        }

        // Check for viewport meta tag
        if (!templateContent.Contains("<meta name=\"viewport\"", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue
            {
                Severity = "warning",
                Category = "best_practice",
                Message = "Viewport meta tag missing",
                Suggestion = "Add <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> for mobile responsiveness"
            });
        }

        return issues;
    }

    private async Task<AiValidationResult> PerformAiAnalysisAsync(TemplateValidationArgs args)
    {
        var prompt = await GetSystemPromptAsync("template_validation");

        var userMessage = $@"
Analyze the following CMS template for validation and improvement suggestions:

Template Key: {args.TemplateKey}
Template Content:
{args.TemplateContent}

Please provide:
1. Overall template quality assessment
2. Specific improvement suggestions
3. Code quality and maintainability analysis
4. User experience considerations
5. SEO and accessibility recommendations

Format your response as a structured analysis with confidence scores for each suggestion.
";

        var aiResponse = await _aiGateway.ExecuteAiRequestAsync(
            _tenantContext.TenantId,
            "template_validation",
            "openai",
            async () =>
            {
                return await _providerSelector.ExecuteChatCompletionAsync(
                    _tenantContext.TenantId,
                    "gpt-4",
                    prompt,
                    userMessage,
                    "openai");
            });

        // Parse AI response and extract structured data
        // For now, return a basic structure - in production this would parse the AI response
        return new AiValidationResult
        {
            Issues = new List<ValidationIssue>(),
            Suggestions = aiResponse.Response?.Content ?? "AI analysis completed",
            ConfidenceScore = 0.85
        };
    }

    private async Task<string> GetSystemPromptAsync(string toolType)
    {
        // TODO: Integrate with ITemplateValidationService for comprehensive validation
        // This will replace the basic validation with full CMS domain validation
        // including syntax, security, performance, and best practices checks

        await Task.CompletedTask; // Placeholder for async database/config lookup

        return @"
You are an expert CMS template validator and frontend development consultant. Analyze templates for quality, security, performance, and best practices.

Guidelines:
- Focus on production-ready template validation
- Prioritize security and performance concerns
- Provide actionable, specific recommendations
- Consider both technical and user experience aspects
- Rate suggestions with confidence levels
- Follow modern web development standards
- Ensure accessibility and SEO best practices

Provide detailed analysis with prioritized improvement suggestions and implementation guidance.
";
    }
}

/// <summary>
/// Arguments for template validation
/// </summary>
public class TemplateValidationArgs
{
    public string TemplateKey { get; set; } = null!;
    public string TemplateContent { get; set; } = null!;
    public string? ValidationScope { get; set; } // "syntax", "security", "performance", "all"
}

/// <summary>
/// Result of template validation - matches CMS domain structure
/// </summary>
public class TemplateValidationResult
{
    public string TemplateKey { get; set; } = null!;
    public DateTime ValidationTimestamp { get; set; }
    public List<ValidationIssue> ValidationResults { get; set; } = new();
    public string OverallStatus { get; set; } = null!; // "passed", "failed", "warning"
    public double ConfidenceScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
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