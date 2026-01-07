using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2X.CMS.Application.Pages;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace B2X.CMS.Application.Pages
{
    /// <summary>
    /// Command to create a template override (ADR-030)
    /// </summary>
    public record CreateTemplateOverrideCommand(
        string TenantId,
        string TemplateKey,
        string? BaseTemplateKey,
        string TemplateContent,
        Dictionary<string, string> OverrideSections,
        string CreatedBy
    );

    /// <summary>
    /// Command to update a template override (ADR-030)
    /// </summary>
    public record UpdateTemplateOverrideCommand(
        string TenantId,
        string TemplateKey,
        string TemplateContent,
        Dictionary<string, string> OverrideSections,
        string UpdatedBy
    );

    /// <summary>
    /// Command to publish a template override (ADR-030)
    /// </summary>
    public record PublishTemplateOverrideCommand(
        string TenantId,
        string TemplateKey
    );

    /// <summary>
    /// Command to delete a template override (ADR-030)
    /// </summary>
    public record DeleteTemplateOverrideCommand(
        string TenantId,
        string TemplateKey
    );

    /// <summary>
    /// Command to validate a template override (ADR-030 Phase 2)
    /// </summary>
    public record ValidateTemplateOverrideCommand(
        string TenantId,
        string TemplateKey
    );

    /// <summary>
    /// Command to validate template content directly (ADR-030 Phase 2)
    /// Used by MCP server integration
    /// </summary>
    public record ValidateTemplateContentCommand(
        string TenantId,
        string TemplateKey,
        string TemplateContent
    );
}
