using B2X.Admin.Application.Services;
using B2X.Admin.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Admin.Presentation.Controllers;

/// <summary>
/// API endpoints for downloading and managing CLI tools
/// Provides tenant-scoped access to Administration-CLI and ERP-Connector
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ValidateTenant]
[Produces("application/json")]
public class CliToolsController : ApiControllerBase
{
    private readonly ICliToolsService _cliToolsService;

    public CliToolsController(ILogger<CliToolsController> logger, ICliToolsService cliToolsService)
        : base(logger)
    {
        _cliToolsService = cliToolsService;
    }

    /// <summary>
    /// Get available CLI tools for the tenant
    /// </summary>
    /// <returns>List of available CLI tools with metadata</returns>
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<CliToolInfo>>> GetAvailableTools()
    {
        try
        {
            var tenantId = GetTenantId();
            var tools = await _cliToolsService.GetAvailableToolsAsync(tenantId);
            return Ok(tools);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available CLI tools");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to retrieve CLI tools" });
        }
    }

    /// <summary>
    /// Download Administration CLI for the tenant
    /// </summary>
    /// <param name="version">CLI version to download (e.g., "latest", "1.0.0")</param>
    /// <param name="osType">Target OS (win, linux, osx)</param>
    /// <returns>File stream with CLI binary</returns>
    [HttpGet("download/administration-cli")]
    [AllowAnonymous]
    public async Task<ActionResult> DownloadAdministrationCli(
        [FromQuery] string version = "latest",
        [FromQuery] string osType = "win")
    {
        try
        {
            var tenantId = GetTenantId();
            var fileInfo = await _cliToolsService.GetAdministrationCliAsync(tenantId, version, osType);

            if (fileInfo == null)
            {
                return NotFound(new { message = "CLI version not found" });
            }

            _logger.LogInformation("Tenant {TenantId} downloading Administration-CLI v{Version} for {OS}",
                tenantId, version, osType);

            return File(fileInfo.FileStream, fileInfo.ContentType, fileInfo.FileName,
                enableRangeProcessing: true);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized CLI download attempt");
            return Unauthorized(new { message = "Not authorized to download CLI tools" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading Administration-CLI");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to download CLI tool" });
        }
    }

    /// <summary>
    /// Download ERP-Connector for the tenant
    /// </summary>
    /// <param name="erpType">ERP system type (e.g., "enventa")</param>
    /// <param name="version">Connector version to download</param>
    /// <returns>File stream with ERP-Connector binary</returns>
    [HttpGet("download/erp-connector")]
    [AllowAnonymous]
    public async Task<ActionResult> DownloadErpConnector(
        [FromQuery] string erpType,
        [FromQuery] string version = "latest")
    {
        try
        {
            if (string.IsNullOrEmpty(erpType))
            {
                return BadRequest(new { message = "erpType parameter is required" });
            }

            var tenantId = GetTenantId();
            var fileInfo = await _cliToolsService.GetErpConnectorAsync(tenantId, erpType, version);

            if (fileInfo == null)
            {
                return NotFound(new { message = "ERP-Connector version not found" });
            }

            _logger.LogInformation("Tenant {TenantId} downloading ERP-Connector ({ERP}) v{Version}",
                tenantId, erpType, version);

            return File(fileInfo.FileStream, fileInfo.ContentType, fileInfo.FileName,
                enableRangeProcessing: true);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized ERP-Connector download attempt");
            return Unauthorized(new { message = "Not authorized to download ERP-Connector" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading ERP-Connector");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to download ERP-Connector" });
        }
    }

    /// <summary>
    /// Get installation instructions for a CLI tool
    /// </summary>
    /// <param name="toolType">Tool type (administration-cli, erp-connector)</param>
    /// <param name="osType">Target OS (win, linux, osx)</param>
    /// <returns>Installation instructions</returns>
    [HttpGet("instructions/{toolType}")]
    public async Task<ActionResult<CliInstallationInstructions>> GetInstallationInstructions(
        string toolType,
        [FromQuery] string osType = "win")
    {
        try
        {
            var tenantId = GetTenantId();
            var instructions = await _cliToolsService.GetInstallationInstructionsAsync(
                tenantId, toolType, osType);

            if (instructions == null)
            {
                return NotFound(new { message = "Instructions not found for tool" });
            }

            return Ok(instructions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving installation instructions");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to retrieve installation instructions" });
        }
    }

    /// <summary>
    /// Get CLI tool versions available for download
    /// </summary>
    /// <param name="toolType">Tool type (administration-cli, erp-connector)</param>
    /// <returns>List of available versions</returns>
    [HttpGet("versions/{toolType}")]
    public async Task<ActionResult<IEnumerable<CliVersionInfo>>> GetAvailableVersions(string toolType)
    {
        try
        {
            var tenantId = GetTenantId();
            var versions = await _cliToolsService.GetAvailableVersionsAsync(tenantId, toolType);
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available versions");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Failed to retrieve available versions" });
        }
    }
}

/// <summary>
/// Information about available CLI tools
/// </summary>
public record CliToolInfo(
    string ToolType,
    string Name,
    string Description,
    string LatestVersion,
    bool IsAvailable,
    string[] SupportedOperatingSystems,
    DateTime LastUpdated);

/// <summary>
/// Information about CLI versions
/// </summary>
public record CliVersionInfo(
    string Version,
    DateTime ReleaseDate,
    string ReleaseNotes,
    bool IsPrerelease,
    string[] BreakingChanges);

/// <summary>
/// Installation instructions for CLI tools
/// </summary>
public record CliInstallationInstructions(
    string ToolType,
    string OperatingSystem,
    string[] Steps,
    string[] Prerequisites,
    string? ConfigurationTemplate,
    string[] TroubleshootingTips);

/// <summary>
/// File information for download
/// </summary>
public record CliToolFileInfo(
    Stream FileStream,
    string ContentType,
    string FileName);
